// <copyright file="JobsExtensions.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// Jobs extensions for simpler job execution and coroutines waiting for jobs to finish.
/// </summary>
public static class JobsExtensions
{
	/// <summary>
	/// Schedules and completes jobs. After completing the job onJobCompleted action is invoked and the job is disposed.
	/// </summary>
	/// <typeparam name="T">Job type implementing IDisposableJobParallelFor interface.</typeparam>
	/// <param name="jobData">The job and data to Schedule.</param>
	/// <param name="arrayLength">The number of iterations the for loop will execute.</param>
	/// <param name="interloopBatchCount">Granularity in which workstealing is performed. A value of 32, means the job queue will steal 32 iterations and then perform them in an efficient inner loop.</param>
	/// <param name="onJobCompleted">Action to be invoked on Job completion.</param>
	/// <param name="dependsOn">Dependencies are used to ensure that a job executes on workerthreads after the dependency has completed execution. Making sure that two jobs reading or writing to same data do not run in parallel.</param>
	public static void ScheduleAndComplete<T>(this T jobData, int arrayLength, int interloopBatchCount, Action<T> onJobCompleted = null, JobHandle dependsOn = default) where T : struct, IDisposableJobParallelFor
	{
		var handle = jobData.Schedule(arrayLength, interloopBatchCount, dependsOn);
		handle.Complete();

		onJobCompleted?.Invoke(jobData);
		jobData.Dispose();
	}

	/// <summary>
	/// Schedules and completes the job as soon as it's done using a coroutine awaiting for the job to finish.
	/// After job is finished onJobCompleted action is invoked.
	/// </summary>
	/// <typeparam name="T">Job type implementing IDisposableJobParallelFor interface.</typeparam>
	/// <param name="jobData">The job and data to Schedule.</param>
	/// <param name="arrayLength">The number of iterations the for loop will execute.</param>
	/// <param name="interloopBatchCount">Granularity in which workstealing is performed. A value of 32, means the job queue will steal 32 iterations and then perform them in an efficient inner loop.</param>
	/// <param name="context">MonoBehaviour to get coroutine run on.</param>
	/// <param name="onJobScheduled">Action to be invoked when Job is scheduled.</param>
	/// <param name="onJobCompleted">Action to be invoked on Job completion.</param>
	/// <param name="dependsOn">Dependencies are used to ensure that a job executes on workerthreads after the dependency has completed execution. Making sure that two jobs reading or writing to same data do not run in parallel.</param>
	/// <returns>Coroutine awaiting for the job to complete.</returns>
	public static Coroutine ScheduleAndCompleteAsync<T>(this T jobData, int arrayLength, int interloopBatchCount, MonoBehaviour context, Action<JobHandle> onJobScheduled = null, Action<T> onJobCompleted = null, JobHandle dependsOn = default) where T : struct, IDisposableJobParallelFor
	{
		var handle = jobData.Schedule(arrayLength, interloopBatchCount, dependsOn);
		onJobScheduled?.Invoke(handle);
		var coroutine = context.StartCoroutine(WaitForJobToFinish(handle, jobData, onJobCompleted));

		return coroutine;
	}

	private static IEnumerator WaitForJobToFinish<T>(JobHandle jobHandle, T job, Action<T> onJobFinished) where T : struct, IDisposableJobParallelFor
	{
		yield return new WaitWhile(() => !jobHandle.IsCompleted);

		jobHandle.Complete();
		onJobFinished?.Invoke(job);
		job.Dispose();
	}
}

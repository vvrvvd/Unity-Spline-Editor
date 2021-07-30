using System;
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using Unity.Jobs;
using UnityEngine;
using Object = UnityEngine.Object;

public static class JobsUtility
{
	public static void ScheduleAndComplete<T>(this T job, int arrayLength, int interloopBatchCount, Action<T> OnJobCompleted = null, JobHandle dependsOn = default) where T : struct, IDisposableJobParallelFor
	{
		var handle = job.Schedule(arrayLength, interloopBatchCount, dependsOn);
		handle.Complete();

		OnJobCompleted?.Invoke(job);
		job.Dispose();
	}

	public static Coroutine ScheduleAndCompleteAsync<T>(this T job, int arrayLength, int interloopBatchCount, MonoBehaviour context, Action<T> OnJobCompleted = null, JobHandle dependsOn = default) where T : struct, IDisposableJobParallelFor
	{
		var handle = job.Schedule(arrayLength, interloopBatchCount, dependsOn);
		var coroutine = context.StartCoroutine(WaitForJobToFinish(handle, job, OnJobCompleted));

		return coroutine;
	}

#if UNITY_EDITOR
	public static EditorCoroutine ScheduleAndCompleteEditorAsync<T>(this T job, int arrayLength, int interloopBatchCount, Object context, Action<T> OnJobCompleted = null, JobHandle dependsOn = default) where T : struct, IDisposableJobParallelFor
	{
		var handle = job.Schedule(arrayLength, interloopBatchCount, dependsOn);
		var coroutine = EditorCoroutineUtility.StartCoroutine(WaitForJobToFinish(handle, job, OnJobCompleted), context);

		return coroutine;
	}
#endif

	private static IEnumerator WaitForJobToFinish<T>(JobHandle jobHandle, T job, Action<T> onJobFinished) where T : struct, IDisposableJobParallelFor
	{
		yield return new WaitWhile(() => !jobHandle.IsCompleted);

		jobHandle.Complete();
		onJobFinished?.Invoke(job);
		job.Dispose();
	}
}

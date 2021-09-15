// <copyright file="IDisposableJobParallelFor.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Unity.Jobs;

/// <summary>
/// Interface adding Dispose() method to IJobParallelFor interface.
/// </summary>
public interface IDisposableJobParallelFor : IJobParallelFor
{
	/// <summary>
	/// Disposing of all the allocated memory for the job.
	/// </summary>
	public void Dispose();
}

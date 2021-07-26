using System.Collections;
using Unity.Jobs;

public interface IDisposableJobParallelFor : IJobParallelFor
{
	public void Dispose();
}

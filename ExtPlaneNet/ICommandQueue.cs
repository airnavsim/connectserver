using System.Collections.Concurrent;
using ExtPlaneNetCore.Commands;

namespace ExtPlaneNetCore
{
	public interface ICommandQueue
	{
		void Enqueue(Command command);
		Command TryDequeue();
	}
}
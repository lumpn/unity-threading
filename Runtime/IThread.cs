//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
namespace Lumpn.Threading
{
    public interface IThread : ISynchronizationContext
    {
        bool IsRunning { get; }
        bool IsIdle { get; }
        int QueueLength { get; }
        ISynchronizationContext Context { get; }

        void Stop();
    }
}

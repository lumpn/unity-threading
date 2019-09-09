//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
namespace Lumpn
{
    public interface IThread : ISynchronizationContext
    {
        bool IsIdle { get; }
        int QueueLength { get; }
        ISynchronizationContext Context { get; }

        void Stop();
    }
}

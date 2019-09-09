//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
namespace Lumpn
{
    public interface IThread : ISynchronizationContext
    {
        ISynchronizationContext Context { get; }

        bool IsIdle { get; }

        int QueueLength { get; }

        void Stop();
    }
}

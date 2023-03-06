//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
namespace Lumpn.Threading
{
    public interface IThread : ISynchronizationContext
    {
        bool isRunning { get; }
        bool isIdle { get; }
        int queueLength { get; }
        ISynchronizationContext context { get; }

        void Stop();
    }
}

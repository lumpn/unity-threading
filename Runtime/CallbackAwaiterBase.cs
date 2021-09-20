//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Threading;

namespace Lumpn.Threading
{
    public abstract class CallbackAwaiterBase
    {
        private const int statusNone = 0;
        private const int statusInitialized = 1;
        private const int statusCalled = 2;

        private ISynchronizationContext originalContext;
        private CoroutineWrapper coroutineWrapper;
        private volatile int status = statusNone;

        internal void Initialize(ISynchronizationContext originalContext, CoroutineWrapper coroutineWrapper)
        {
            this.originalContext = originalContext;
            this.coroutineWrapper = coroutineWrapper;

            int previousStatus = Interlocked.CompareExchange(ref status, statusInitialized, statusNone);
            if (previousStatus == statusCalled)
            {
                // awaiter has already been called -> resume
                ResumeCoroutine();
            }
        }

        protected void SetCalled()
        {
            int previousStatus = Interlocked.CompareExchange(ref status, statusCalled, statusNone);
            if (previousStatus == statusInitialized)
            {
                // awaiter has already been initialized -> resume
                ResumeCoroutine();
            }
        }

        private void ResumeCoroutine()
        {
            // resume coroutine on its original synchronization context
            coroutineWrapper.ContinueOn(originalContext);
        }
    }
}

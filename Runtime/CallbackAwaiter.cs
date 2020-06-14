//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Threading;

namespace Lumpn.Threading
{
    public class CallbackAwaiter
    {
        private const int STATUS_NONE = 0;
        private const int STATUS_INITIALIZED = 1;
        private const int STATUS_CALLED = 2;

        private ISynchronizationContext originalContext;
        private CoroutineWrapper coroutineWrapper;
        private volatile int status = STATUS_NONE;

        internal void Initialize(ISynchronizationContext originalContext, CoroutineWrapper coroutineWrapper)
        {
            this.originalContext = originalContext;
            this.coroutineWrapper = coroutineWrapper;
            int previousStatus = Interlocked.CompareExchange(ref status, STATUS_INITIALIZED, STATUS_NONE);
            if (previousStatus == STATUS_CALLED)
            {
                // awaiter has already been called -> resume
                ResumeCoroutine();
            }
        }

        public void Call()
        {
            int previousStatus = Interlocked.CompareExchange(ref status, STATUS_CALLED, STATUS_NONE);
            if (previousStatus == STATUS_INITIALIZED)
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

    public sealed class CallbackAwaiter<T1> : CallbackAwaiter
    {
        public T1 arg1 { get; private set; }

        public void Call(T1 arg1)
        {
            this.arg1 = arg1;
            Call();
        }
    }

    public sealed class CallbackAwaiter<T1, T2> : CallbackAwaiter
    {
        public T1 arg1 { get; private set; }
        public T2 arg2 { get; private set; }

        public void Call(T1 arg1, T2 arg2)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
            Call();
        }
    }

    public sealed class CallbackAwaiter<T1, T2, T3> : CallbackAwaiter
    {
        public T1 arg1 { get; private set; }
        public T2 arg2 { get; private set; }
        public T3 arg3 { get; private set; }

        public void Call(T1 arg1, T2 arg2, T3 arg3)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            Call();
        }
    }
}

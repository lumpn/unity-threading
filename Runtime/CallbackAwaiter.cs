//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------

namespace Lumpn.Threading
{
    public sealed class CallbackAwaiter : CallbackAwaiterBase
    {
        public void Call()
        {
            SetCalled();
        }
    }

    public sealed class CallbackAwaiter<T1> : CallbackAwaiterBase
    {
        public T1 arg1 { get; private set; }

        public void Call(T1 arg1)
        {
            this.arg1 = arg1;
            SetCalled();
        }
    }

    public sealed class CallbackAwaiter<T1, T2> : CallbackAwaiterBase
    {
        public T1 arg1 { get; private set; }
        public T2 arg2 { get; private set; }

        public void Call(T1 arg1, T2 arg2)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
            SetCalled();
        }
    }

    public sealed class CallbackAwaiter<T1, T2, T3> : CallbackAwaiterBase
    {
        public T1 arg1 { get; private set; }
        public T2 arg2 { get; private set; }
        public T3 arg3 { get; private set; }

        public void Call(T1 arg1, T2 arg2, T3 arg3)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            SetCalled();
        }
    }
}

//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using UnityEngine;

namespace Lumpn.Threading
{
    internal sealed class YieldInstructionWrapper : IEnumerator
    {
        private static readonly ObjectPool<YieldInstructionWrapper> pool = new ObjectPool<YieldInstructionWrapper>(100);

        private YieldInstruction instruction;
        private ISynchronizationContext originalContext;
        private CoroutineWrapper coroutineWrapper;
        private bool yielded;

        private YieldInstructionWrapper()
        {
        }

        public object Current { get { return instruction; } }

        public bool MoveNext()
        {
            // yield once, exposing the yield instruction to the caller.
            if (!yielded)
            {
                yielded = true;
                return true;
            }

            // caller called again, therefore the yield instruction must
            // have completed. we now continue on the original context.
            coroutineWrapper.ContinueOn(originalContext);

            // also our work is complete -> return to pool
            lock (pool)
            {
                pool.TryPut(this);
            }
            return false;
        }

        public void Reset()
        {
        }

        public override string ToString()
        {
            return string.Format("instruction '{0}', context '{1}'", instruction, originalContext);
        }

        public static YieldInstructionWrapper Create(YieldInstruction instruction, ISynchronizationContext originalContext, CoroutineWrapper coroutineWrapper)
        {
            var yieldWrapper = GetWrapper();
            yieldWrapper.instruction = instruction;
            yieldWrapper.originalContext = originalContext;
            yieldWrapper.coroutineWrapper = coroutineWrapper;
            return yieldWrapper;
        }

        private static YieldInstructionWrapper GetWrapper()
        {
            lock (pool)
            {
                if (pool.TryGet(out YieldInstructionWrapper wrapper)) return wrapper;
            }
            return new YieldInstructionWrapper();
        }
    }
}

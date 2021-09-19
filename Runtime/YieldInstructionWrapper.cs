//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Lumpn.Threading
{
    internal sealed class YieldInstructionWrapper : IEnumerator
    {
        private static readonly Stack<YieldInstructionWrapper> pool = new Stack<YieldInstructionWrapper>(100);

        private YieldInstruction instruction;
        private ISynchronizationContext originalContext;
        private CoroutineWrapper coroutineWrapper;
        private bool yielded;

        public object Current { get { return instruction; } }

        public bool MoveNext()
        {
            // yield once, exposing the yield instruction to the caller.
            if (!yielded)
            {
                Log("yielding first time");
                yielded = true;
                return true;
            }

            // syncronization context called again, therefore the yield instruction
            // must have completed. we now continue on the original context.
            Log("yielded. continuing on original context.");
            coroutineWrapper.ContinueOn(originalContext);

            // also our work is complete -> return to pool
            Return(this);
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
            var yieldWrapper = Get();
            yieldWrapper.instruction = instruction;
            yieldWrapper.originalContext = originalContext;
            yieldWrapper.coroutineWrapper = coroutineWrapper;
            return yieldWrapper;
        }

        private static YieldInstructionWrapper Get()
        {
            lock (pool)
            {
                return pool.PopOrNew();
            }
        }

        private static void Return(YieldInstructionWrapper wrapper)
        {
            lock (pool)
            {
                pool.Push(wrapper);
            }
        }

        private static void Log(object msg)
        {
            var thread = Thread.CurrentThread;
            Debug.LogFormat("Thread {0} ({1}): {2}", thread.ManagedThreadId, thread.Name, msg);
        }

    }
}

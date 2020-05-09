//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lumpn.Threading
{
    public sealed class CoroutineHandler : CustomYieldInstruction
    {
        private readonly static ObjectPool<CoroutineHandler> pool = new ObjectPool<CoroutineHandler>(100);

        private readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();

        public override bool keepWaiting { get { return stack.Count > 0; } }

        public static CustomYieldInstruction StartCoroutine(IThread thread, IEnumerator coroutine)
        {
            var handler = GetHandler();
            handler.stack.Push(coroutine);

            thread.Post(AdvanceCoroutine, handler, thread);
            return handler;
        }

        private static CoroutineHandler GetHandler()
        {
            lock (pool)
            {
                CoroutineHandler handler;
                if (pool.TryGet(out handler)) return handler;
            }
            return new CoroutineHandler();
        }

        private CoroutineHandler()
        {
        }

        private static void AdvanceCoroutine(object owner, object state)
        {
            var handler = (CoroutineHandler)owner;
            var context = (ISynchronizationContext)state;

            if (handler.AdvanceCoroutine(context))
            {
                context.Post(AdvanceCoroutine, handler, context);
            }
        }

        private bool AdvanceCoroutine(ISynchronizationContext context)
        {
            if (stack.Count < 1)
            {
                lock (pool)
                {
                    pool.TryPut(this);
                }
                return false;
            }

            var iter = stack.Peek();
            if (!iter.MoveNext())
            {
                stack.Pop();
                return true;
            }

            var currentElement = iter.Current;

            // handle nesting
            if (currentElement is IEnumerator subroutine)
            {
                stack.Push(subroutine);
                return true;
            }

            // handle switching context
            if (currentElement is ISynchronizationContext newContext)
            {
                newContext.Post(AdvanceCoroutine, this, newContext);
                return false;
            }

            // handle yield instructions
            if (currentElement is YieldInstruction yieldInstruction)
            {
                // TODO Jonas: properly handle yield instructions on UnityThreads
                // Perhaps handle yield instructions on WorkerThreads by implicitly switching contexts back and forth?
                Debug.LogWarningFormat("CoroutineHandler on context {0} encountered yield instruction {1}", context, yieldInstruction);
                return true; // ignore yield instruction and carry on for now
            }

            return true;
        }
    }
}

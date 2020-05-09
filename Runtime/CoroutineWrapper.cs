//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lumpn.Threading
{
    public sealed class CoroutineWrapper : CustomYieldInstruction
    {
        private readonly static ObjectPool<CoroutineWrapper> pool = new ObjectPool<CoroutineWrapper>(100);

        private readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();

        public override bool keepWaiting { get { return stack.Count > 0; } }

        public static CustomYieldInstruction StartCoroutine(ISynchronizationContext context, IEnumerator coroutine)
        {
            var handler = GetHandler();
            handler.stack.Push(coroutine);

            context.Post(AdvanceCoroutine, handler, context);
            return handler;
        }

        private static CoroutineWrapper GetHandler()
        {
            lock (pool)
            {
                CoroutineWrapper handler;
                if (pool.TryGet(out handler)) return handler;
            }
            return new CoroutineWrapper();
        }

        private CoroutineWrapper()
        {
        }

        internal static void AdvanceCoroutine(object owner, object state)
        {
            var handler = (CoroutineWrapper)owner;
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

            // handle awaiting callbacks
            if (currentElement is CallbackAwaiter awaiter)
            {
                awaiter.Initialize(context, this);
                return false;
            }

            // handle yield instructions
            if (currentElement is YieldInstruction yieldInstruction)
            {
                // TODO Jonas: properly handle yield instructions on UnityThreads
                // Perhaps handle yield instructions on WorkerThreads by implicitly switching contexts back and forth?
                Debug.LogWarningFormat("CoroutineWrapper on context {0} encountered yield instruction {1}", context, yieldInstruction);
                return true; // ignore yield instruction and carry on for now
            }

            return true;
        }
    }
}

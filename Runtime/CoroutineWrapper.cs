//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lumpn.Threading
{
    internal sealed class CoroutineWrapper : CustomYieldInstruction
    {
        private static readonly ObjectPool<CoroutineWrapper> pool = new ObjectPool<CoroutineWrapper>(100);

        private readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();

        public override bool keepWaiting { get { return stack.Count > 0; } }

        public static CoroutineWrapper StartCoroutine(ISynchronizationContext context, IEnumerator coroutine)
        {
            var wrapper = GetWrapper();
            wrapper.stack.Push(coroutine);

            wrapper.ContinueOn(context);
            return wrapper;
        }

        private static CoroutineWrapper GetWrapper()
        {
            lock (pool)
            {
                if (pool.TryGet(out CoroutineWrapper wrapper)) return wrapper;
            }
            return new CoroutineWrapper();
        }

        private CoroutineWrapper()
        {
        }

        public void ContinueOn(ISynchronizationContext context)
        {
            context.Post(AdvanceCoroutine, this, context);
        }

        private static void AdvanceCoroutine(object owner, object state)
        {
            var wrapper = (CoroutineWrapper)owner;
            var context = (ISynchronizationContext)state;

            if (wrapper.AdvanceCoroutine(context))
            {
                wrapper.ContinueOn(context);
            }
        }

        /// <returns><c>true</c>, iff the coroutine should
        /// continue running on the same context
        private bool AdvanceCoroutine(ISynchronizationContext context)
        {
            if (stack.Count < 1)
            {
                lock (pool)
                {
                    pool.TryPut(this);
                }
                return false; // done
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
                ContinueOn(newContext);
                return false; // continuing on new context instead
            }

            // handle awaiting callbacks
            if (currentElement is CallbackAwaiter awaiter)
            {
                awaiter.Initialize(context, this);
                return false; // awaiting callback instead
            }

            // handle yield instructions
            if (currentElement is YieldInstruction yieldInstruction)
            {
                CoroutineHost.HandleYieldInstruction(yieldInstruction, context, this);
                return false; // awaiting yield instruction instead
            }

            return true;
        }
    }
}

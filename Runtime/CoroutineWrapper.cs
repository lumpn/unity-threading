//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lumpn.Threading
{
    internal sealed class CoroutineWrapper : CustomYieldInstruction
    {
        private static readonly Pool<CoroutineWrapper> pool = new Pool<CoroutineWrapper>(100);

        private readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        private CoroutineHost host;

        public override bool keepWaiting { get { return stack.Count > 0; } }

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

        /// <summary>returns <c>true</c>, iff the coroutine should
        /// continue running on the same context</summary>
        private bool AdvanceCoroutine(ISynchronizationContext context)
        {
            if (stack.Count < 1)
            {
                pool.Return(this);
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
            if (currentElement is CallbackAwaiterBase awaiter)
            {
                awaiter.Initialize(context, this);
                return false; // awaiting callback instead
            }

            // handle yield instructions
            if (currentElement is YieldInstruction yieldInstruction)
            {
                host.Handle(yieldInstruction, context, this);
                return false; // awaiting yield instruction instead
            }

            return true;
        }

        public static CoroutineWrapper StartCoroutine(CoroutineHost host, ISynchronizationContext context, IEnumerator coroutine)
        {
            var wrapper = pool.Get();
            wrapper.host = host;
            wrapper.stack.Push(coroutine);
            wrapper.ContinueOn(context);
            return wrapper;
        }
    }
}

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

        public static CustomYieldInstruction StartCoroutine(ISynchronizationContext context, IEnumerator coroutine)
        {
            var wrapper = GetWrapper();
            wrapper.stack.Push(coroutine);

            context.Post(AdvanceCoroutine, wrapper, context);
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

        internal static void AdvanceCoroutine(object owner, object state)
        {
            var wrapper = (CoroutineWrapper)owner;
            var context = (ISynchronizationContext)state;

            if (wrapper.AdvanceCoroutine(context))
            {
                context.Post(AdvanceCoroutine, wrapper, context);
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
                CoroutineHost.HandleYieldInstruction(yieldInstruction, context, this);
                return false;
            }

            return true;
        }
    }
}

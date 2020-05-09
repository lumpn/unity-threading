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
        private readonly YieldInstruction instruction;
        private readonly ISynchronizationContext originalContext;
        private readonly CoroutineWrapper coroutineWrapper;
        private bool yielded;

        public YieldInstructionWrapper(YieldInstruction instruction, ISynchronizationContext originalContext, CoroutineWrapper coroutineWrapper)
        {
            this.instruction = instruction;
            this.originalContext = originalContext;
            this.coroutineWrapper = coroutineWrapper;
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
            return false;
        }

        public void Reset()
        {
        }

        public override string ToString()
        {
            return string.Format("instruction '{0}', context '{1}'", instruction, originalContext);
        }
    }
}

//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lumpn.Threading
{
    public sealed class CoroutineHost : MonoBehaviour
    {
        private static readonly UnityThread unityThread = new UnityThread("CoroutineHost", 64);

        IEnumerator Start()
        {
            return unityThread.Run();
        }

        internal static void HandleYieldInstruction(YieldInstruction instruction, ISynchronizationContext context, CoroutineWrapper coroutineWrapper)
        {
            if (!unityThread.IsRunning)
            {
                // failed to locate the coroutine host which must be present
                // in the scene at this point. we might be on a thread here,
                // and Unity does not allow creating game objects outside the
                // main thread, therefore there's no way to create the coroutine
                // host on the fly.
                Debug.LogErrorFormat("CoroutineHost is missing from scene. (instruction '{0}', context '{1}')", instruction, context);
            }


            var yieldWrapper = new YieldInstructionWrapper(instruction, context, coroutineWrapper);
            unityThread.Post(HandleYieldInstructionImpl, yieldWrapper, null);
            var host = instance;
            if (!host)
            {
                return;
            }

            host.StartCoroutine(HandleYieldInstructionImpl(instruction, context, wrapper));
        }

        private static IEnumerator HandleYieldInstructionImpl(YieldInstruction instruction, ISynchronizationContext context, CoroutineWrapper wrapper)
        { }

    }
}

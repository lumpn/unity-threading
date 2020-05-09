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
                // our Unity thread is not running, which means that no
                // coroutine host has been created in the scene. in theory
                // you could add the host later and it would start working down
                // the queue, but in practice it is almost certain, that the
                // user didn't know they are supposed to put a coroutine host
                // in the scene.
                // there is no way to recover automatically, because we might
                // be on a thread here and Unity does not allow creating game
                // objects outside the main thread, therefore there's no way to
                // create the coroutine host on the fly. -> inform the user
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

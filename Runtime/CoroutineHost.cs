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
        private static readonly List<CoroutineHost> instances = new List<CoroutineHost>();

        public int QueueLength { get { return unityThread.QueueLength; } }

        IEnumerator Start()
        {
            instances.Add(this);
            return unityThread.Run();
        }

        void OnDestroy()
        {
            instances.Remove(this);
        }

        internal static void HandleYieldInstruction(YieldInstruction instruction, ISynchronizationContext context, CoroutineWrapper coroutineWrapper)
        {
            Debug.LogFormat("Handling yield instruction {0}, context {1}", instruction, context);

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

            // TODO Jonas: use object pool for instruction wrappers
            var yieldWrapper = new YieldInstructionWrapper(instruction, context, coroutineWrapper);
            Debug.LogFormat("Posting wrapper {0} to unity thread {1}", yieldWrapper, unityThread);
            unityThread.Post(HandleYieldInstructionImpl, instances, yieldWrapper);
        }

        private static void HandleYieldInstructionImpl(object owner, object state)
        {
            Debug.LogFormat("HandleYieldInstructionImpl owner {0}, state {1}", owner, state);

            var hosts = (List<CoroutineHost>)owner;
            var wrapper = (YieldInstructionWrapper)state;

            var host = GetActiveHost(hosts);
            if (!host)
            {
                // the fact that this method got called to begin with means that
                // must be a coroutine host currently running it. it should have
                // been in the list of hosts and active in hierarchy. not sure
                // how we ended up here.
                Debug.LogErrorFormat("Could not find active CoroutineHost. ('{0}')", wrapper);
                return;
            }

            Debug.LogFormat("HandleYieldInstructionImpl starting coroutine {0} on host {1}", wrapper, host);
            host.StartCoroutine(wrapper);
        }

        private static CoroutineHost GetActiveHost(List<CoroutineHost> hosts)
        {
            foreach (var host in hosts)
            {
                if (host && host.gameObject.activeInHierarchy)
                {
                    return host;
                }
            }
            return null;
        }
    }
}

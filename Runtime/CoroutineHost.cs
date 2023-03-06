//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Lumpn.Threading
{
    public sealed class CoroutineHost : MonoBehaviour
    {
        private readonly UnityThread unityThread = new UnityThread(nameof(CoroutineHost), 10);

        public int queueLength { get { return unityThread.queueLength; } }

        IEnumerator Start()
        {
            return unityThread.Start();
        }

        internal void Handle(YieldInstruction yieldInstruction, ISynchronizationContext context, CoroutineWrapper wrapper)
        {
            Log("Handle yield instruction");
            var yieldWrapper = YieldInstructionWrapper.Create(yieldInstruction, context, wrapper);
            unityThread.Post(StartYieldWrapper, this, yieldWrapper);
        }

        private static void StartYieldWrapper(object owner, object state)
        {
            Log("Start yield wrapper");
            var host = (CoroutineHost)owner;
            var wrapper = (YieldInstructionWrapper)state;

            host.StartCoroutine(wrapper);
        }

        private static void Log(object msg)
        {
            var thread = Thread.CurrentThread;
            Debug.LogFormat("Thread {0} ({1}): {2}", thread.ManagedThreadId, thread.Name, msg);
        }
    }
}

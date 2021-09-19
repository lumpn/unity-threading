//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Lumpn.Threading.Demo
{
    [AddComponentMenu("")]
    public class SwitchContextDemo : MonoBehaviour
    {
        [SerializeField] private CoroutineHost host;

        private IThread worker1, worker2, unity1, unity2;

        void Start()
        {
            var mainThread = Thread.CurrentThread;
            if (mainThread.Name != "Unity")
            {
                mainThread.Name = "Unity";
            }

            worker1 = ThreadUtils.StartWorkerThread("Demo", "Thread1", System.Threading.ThreadPriority.BelowNormal, 100);
            worker2 = ThreadUtils.StartWorkerThread("Demo", "Thread2", System.Threading.ThreadPriority.BelowNormal, 100);
            unity1 = ThreadUtils.StartUnityThread("Unity1", 100, this);
            unity2 = ThreadUtils.StartUnityThread("Unity2", 100, this);

            // automatically start demo
            StartSwitchContextCoroutine();
        }

        void OnDestroy()
        {
            ThreadUtils.StopThread(unity2);
            ThreadUtils.StopThread(unity1);
            ThreadUtils.StopThread(worker2);
            ThreadUtils.StopThread(worker1);
        }

        [ContextMenu("Start Coroutine")]
        public void StartSwitchContextCoroutine()
        {
            worker1.StartCoroutine(SwitchContext(), host);
        }

        IEnumerator SwitchContext()
        {
            Log("Switching to worker thread 1");
            yield return worker1.Context;
            Log("Read voxel data from file");

            Log("Switching to Unity thread 1");
            yield return unity1.Context;
            Log("Create GameObject");

            Log("Pretend waiting for GameObject");
            yield return new WaitForSeconds(1f);
            Log("Create GameObject done");

            Log("Switching to worker thread 2");
            yield return worker2.Context;
            Log("Compute mesh");

            Log("Pretend waiting for mesh");
            yield return new WaitForSeconds(1f);
            Log("Compute mesh done");

            Log("Switching to Unity thread 2");
            yield return unity2.Context;
            Log("Upload mesh");

            Log("Compute sum");
            var awaiter = new CallbackAwaiter<int>();
            ComputeSumAsync(this, 1, 2, awaiter.Call);
            yield return awaiter;
            Log(string.Format("Sum computed: {0}", awaiter.arg1));

            Log("Loading from Resources");
            var request = Resources.LoadAsync<Material>("Material");

            Log("Switching to worker thread 1");
            yield return worker1.Context;

            Log("Waiting for load to finish (implicit thread change)");
            yield return request;
            Log("Done waiting for load to finish (implicit thread change back)");

            Log("Switching to Unity thread 1");
            yield return unity1.Context;

            Log("Accessing loaded asset");
            Debug.Assert(request.asset, "Failed to load asset");
        }

        private delegate void SumCallback(int sum);

        private static void ComputeSumAsync(MonoBehaviour host, int a, int b, SumCallback callback)
        {
            host.StartCoroutine(ComputeSumAsyncImpl(a, b, callback));
        }

        private static IEnumerator ComputeSumAsyncImpl(int a, int b, SumCallback callback)
        {
            Log("Pretending long running computation");
            yield return new WaitForSeconds(2f);

            Log("Returning result via callback");
            int result = a + b;
            callback(result);
        }

        private static void Log(object msg)
        {
            var thread = Thread.CurrentThread;
            Debug.LogFormat("Thread {0} ({1}): {2}", thread.ManagedThreadId, thread.Name, msg);
        }
    }
}

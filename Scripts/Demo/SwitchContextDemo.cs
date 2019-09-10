//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Threading;
using UnityEngine;
using Lumpn;

public class SwitchContextDemo : MonoBehaviour
{
    private IThread thread1, thread2, unity1, unity2;

    void Start()
    {
        Thread.CurrentThread.Name = "Unity";

        thread1 = WorkerThread.Start("Demo", "Thread1", System.Threading.ThreadPriority.BelowNormal, 100);
        thread2 = WorkerThread.Start("Demo", "Thread2", System.Threading.ThreadPriority.BelowNormal, 100);
        unity1 = ThreadUtils.StartUnityThread("Unity1", 100, this);
        unity2 = ThreadUtils.StartUnityThread("Unity2", 100, this);

        thread1.StartCoroutine(TaskSwitch());
    }

    void OnDestroy()
    {
        unity2.Stop();
        unity1.Stop();
        thread2.Stop();
        thread1.Stop();
    }

    IEnumerator TaskSwitch()
    {
        yield return thread1.Context;
        Log("Read voxel data from file");

        yield return unity1.Context;
        Log("Create GameObject");

        yield return thread2.Context;
        Log("Compute mesh");

        yield return unity2.Context;
        Log("Upload mesh");
    }

    private static void Log(object msg)
    {
        var thread = Thread.CurrentThread;
        Debug.LogFormat("Thread {0} ({1}): {2}", thread.ManagedThreadId, thread.Name, msg);
    }
}

# unity-threads
Non-allocating threading facilities for Unity

## Installation
Download the entire repository from https://github.com/lumpn/unity-threads and put it into a subdirectory of your Unity project's *Asset* directory.
For example `MyProject/Assets/Plugins/lumpn/unity-threads`.

## Usage
```csharp
void Start()
{
    workerThread = WorkerThread.Start("Demo", "Thread", System.Threading.ThreadPriority.BelowNormal, 100);
    unityThread = ThreadUtils.StartUnityThread("Unity", 100, this);

    unityThread.StartCoroutine(Demo());
}

IEnumerator Demo()
{
    yield return workerThread.Context;
    // ...
    
    yield return unityThread.Context;
    // ...
}

```

### Notes
* See `SwitchContextDemo` for details.

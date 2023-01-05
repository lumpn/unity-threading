# unity-threading
Non-allocating async/await facilities for Unity. Coroutines that flow across threads, callbacks, and yield instructions.

## Disclaimer
You should probably just use [UniTask](https://github.com/Cysharp/UniTask) instead of this package. Coroutines are nice, but `async` is better, and [UniTask](https://github.com/Cysharp/UniTask) is a great package with good documentation and an active community.

## Installation
Download the entire repository from https://github.com/lumpn/unity-threading and use Unity's built in package manager to [add package from disk](https://docs.unity3d.com/Manual/upm-ui-local.html).

## Usage
```csharp
    IEnumerator SaveAsync()
    {
        saveIcon.SetActive(true); // on Unity thread

        yield return ioThread.Context; // switch to I/O thread
        File.WriteAllBytes("savegame.dat", data); // on I/O thread

        var awaiter = new CallbackAwaiter();
        StorageAPI.WriteAsync("savegame", awaiter.Call);
        yield return awaiter; // wait for callback

        yield return unityThread.Context; // switch back to Unity thread
        saveIcon.SetActive(false); // on Unity thread
    }
```

### Context switching
```csharp
class SaveGameManager : MonoBehaviour
{
    IThread unityThread, ioThread;

    void Start()
    {
        unityThread = ThreadUtils.StartUnityThread("SaveGameManager", 10, this);
        ioThread = ThreadUtils.StartWorkerThread("Workers", "I/O", ThreadPriority.Normal, 10);        
    }

    void OnDestroy()
    {
        ThreadUtils.StopThread(ioThread);
        ThreadUtils.StopThread(unityThread);
    }

    void Save()
    {
        unityThread.StartCoroutine(SaveAsync());
    }

    IEnumerator SaveAsync()
    {
        saveIcon.SetActive(true); // on Unity thread
        yield return ioThread.Context; // switch to I/O thread

        var data = GatherSaveData();
        File.WriteAllBytes("savegame.dat", data); // on I/O thread

        yield return unityThread.Context; // switch back to Unity thread
        saveIcon.SetActive(false); // on Unity thread
    }
}
```

### Callback handling
```csharp
class SaveGameManager : MonoBehaviour
{
    IEnumerator LoadAsync(string fileName)
    {
        var awaiter = new CallbackAwaiter<ByteBuffer>();
        StorageAPI.LoadBytesAsync(fileName, awaiter.Call);
        yield return awaiter; // wait for callback

        var buffer = awaiter.arg;
        RestoreSaveData(buffer);
    }
}
```

### Yield instructions
```csharp
class SaveGameManager : MonoBehaviour
{
    IEnumerator RestoreOptionsAsync()
    {
        busyIndicator.SetActive(true);

        var bundleRequest = AssetBundle.LoadFromFileAsync("options");
        yield return bundleRequest;

        var bundle = bundleRequest.bundle;
        var assetRequest = bundle.LoadAssetAsync<TextAsset>("options.bytes");
        yield return assetRequest;

        var text = (TextAsset)assetRequest.asset;
        var data = text.bytes;

        yield return ioThread.Context;
        File.WriteAllBytes("options.dat", data); // on I/O thread

        // simulate really slow HDD
        yield return new WaitForSeconds(5f); // on I/O thread

        yield return unityThread.Context;
        busyIndicator.SetActive(false);
    }
}
```

## Notes
* See `SwitchContextDemo` for details.

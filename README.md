# unity-threads
Non-allocating thread switching facilities for Unity. Coroutines that flow across thread contexts, like async/await but without all the GC alloc.

## Installation
Download the entire repository from https://github.com/lumpn/unity-threads and put it into a subdirectory of your Unity project's *Asset* directory.
For example `MyProject/Assets/Plugins/lumpn/unity-threads`.

## Usage
```csharp
void Start()
{
    unityThread.StartCoroutine(SomeCoroutine());
}

IEnumerator SomeCoroutine()
{
    yield return workerThread.Context;
    // on worker thread ...
    
    yield return unityThread.Context;
    // on main thread ...
}
```

### Notes
* See `SwitchContextDemo` for details.

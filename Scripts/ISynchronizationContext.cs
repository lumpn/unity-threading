//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
namespace Lumpn
{
    public interface ISynchronizationContext
    {
        void Post(Task.Callback callback, object owner, object state);
    }
}

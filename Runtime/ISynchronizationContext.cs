//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
namespace Lumpn.Threading
{
    public interface ISynchronizationContext
    {
        void Post(Callback callback, object owner, object state);
    }
}

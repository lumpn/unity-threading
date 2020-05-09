//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using UnityEngine;

namespace Lumpn.Threading
{
    public struct Task
    {
        private readonly Callback callback;
        private readonly object owner;
        private readonly object state;

        public Task(Callback callback, object owner, object state)
        {
            this.callback = callback;
            this.owner = owner;
            this.state = state;
        }

        public void Invoke()
        {
            try
            {
                callback(owner, state);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}

//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections;
using UnityEngine;

namespace Lumpn.Threading
{
    public static class ThreadExtensions
    {
        public static CustomYieldInstruction StartCoroutine(this IThread thread, IEnumerator coroutine)
        {
            return CoroutineHandler.StartCoroutine(thread, coroutine);
        }
    }
}

//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;

namespace Lumpn.Threading
{
    public static class StackExtensions
    {
        public static bool TryPop<T>(this Stack<T> stack, out T value)
        {
            if (stack.Count > 0)
            {
                value = stack.Pop();
                return true;
            }

            value = default;
            return false;
        }

        public static T PopOrNew<T>(this Stack<T> stack) where T : new()
        {
            return (stack.Count > 0) ? stack.Pop() : new T();
        }
    }
}

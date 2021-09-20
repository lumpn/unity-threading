//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using UnityEditor;
using UnityEngine;

namespace Lumpn.Threading.Demo
{
    public abstract class Editor<T> : Editor where T : Object
    {
        public sealed override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            OnInspectorGUI((T)target);
            EditorGUILayout.EndVertical();
        }

        public abstract void OnInspectorGUI(T target);
    }
}

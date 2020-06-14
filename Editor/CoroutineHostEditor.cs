//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using UnityEditor;
using UnityEngine;

namespace Lumpn.Threading
{
    [CustomEditor(typeof(CoroutineHost))]
    public sealed class CoroutineHostEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var host = (CoroutineHost)target;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.IntField("Queue Length", host.QueueLength);
            EditorGUILayout.EndVertical();

            Repaint();
        }
    }
}

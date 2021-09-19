//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using UnityEditor;

namespace Lumpn.Threading
{
    [CustomEditor(typeof(CoroutineHost))]
    public sealed class CoroutineHostEditor : Editor<CoroutineHost>
    {
        public override void OnInspectorGUI(CoroutineHost host)
        {
            EditorGUILayout.IntField("Queue Length", host.QueueLength);

            Repaint();
        }
    }
}

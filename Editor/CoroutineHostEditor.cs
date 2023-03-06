//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using UnityEditor;

namespace Lumpn.Threading
{
    [CustomEditor(typeof(CoroutineHost))]
    public sealed class CoroutineHostEditor : Editor<CoroutineHost>
    {
        public override void OnInspectorGUI(CoroutineHost host)
        {
            EditorGUILayout.IntField("Queue Length", host.queueLength);

            Repaint();
        }
    }
}

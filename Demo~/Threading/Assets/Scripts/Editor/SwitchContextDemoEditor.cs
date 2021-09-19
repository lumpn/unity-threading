//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using UnityEditor;
using UnityEngine;

namespace Lumpn.Threading.Demo
{
    [CustomEditor(typeof(SwitchContextDemo))]
    public sealed class SwitchContextDemoEditor : Editor<SwitchContextDemo>
    {
        public override void OnInspectorGUI(SwitchContextDemo demo)
        {
            if (GUILayout.Button("Start Coroutine"))
            {
                demo.StartSwitchContextCoroutine();
            }
        }
    }
}

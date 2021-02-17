//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using UnityEditor;
using UnityEngine;

namespace Lumpn.Threading.Demo
{
    [CustomEditor(typeof(SwitchContextDemo))]
    public class SwitchContextDemoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var demo = (SwitchContextDemo)target;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("Start Coroutine"))
            {
                demo.StartSwitchContextCoroutine();
            }
            EditorGUILayout.EndVertical();
        }
    }
}

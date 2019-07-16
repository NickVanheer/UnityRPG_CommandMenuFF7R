using UnityEngine;
using UnityEditor.IMGUI.Controls;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[CustomEditor(typeof(RPGMenu))]
public class RPGMenuEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Menu tree", EditorStyles.boldLabel);
    }
}

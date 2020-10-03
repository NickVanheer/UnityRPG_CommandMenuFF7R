using UnityEngine;
using UnityEditor.IMGUI.Controls;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[CustomEditor(typeof(RPGMenu))]
public class RPGMenuEditor : Editor {

    public override void OnInspectorGUI()
    {
        RPGMenu currentMenu = (RPGMenu)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RPGMenuItemPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuItemSelectedSprite"));

        EditorGUILayout.LabelField("Required scene objects");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuItemBackgroundHolder"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuTitle"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuHelp"));

        if (GUILayout.Button("Add menu item"))
        {
            currentMenu.AddMenuItemGOOnly();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

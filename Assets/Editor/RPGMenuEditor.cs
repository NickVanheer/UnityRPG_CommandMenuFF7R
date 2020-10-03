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

        EditorGUILayout.LabelField("Total Menus: " + RPGMenu.MenuCountExisting);
        EditorGUILayout.LabelField("Navigation stack: " + currentMenu.dbgGlobalStackCount);
        EditorGUILayout.LabelField("Sections in this menu: " + currentMenu.dbgSectionCount);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Items: ");

        foreach (var item in currentMenu.MenuItemsGO)
        {
            RPGMenuItem menuItem = item.GetComponent<RPGMenuItem>();
            EditorGUILayout.LabelField("- " + menuItem.MenuItemData.Text);
        }

        if (GUILayout.Button("Add menu item"))
        {
            currentMenu.AddMenuItemGOOnly();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

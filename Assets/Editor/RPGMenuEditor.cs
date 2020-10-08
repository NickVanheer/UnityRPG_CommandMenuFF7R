using UnityEngine;
using UnityEditor.IMGUI.Controls;
using JetBrains.Annotations;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[CustomEditor(typeof(RPGMenu))]
public class RPGMenuEditor : Editor {

    List<string> currentMenuItemNames = new List<string>();
    public override void OnInspectorGUI()
    {
        RPGMenu currentMenu = (RPGMenu)target;

        serializedObject.Update(); 
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuType"));
        switch(currentMenu.MenuType)
        {
            case RPGMenuType.CommandMenu:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HostWindowCommandMenuContent"));
                break;
            case RPGMenuType.TabMenu:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HostWindowTabControlContent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ChangeTabsOnMove"));
                break;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsHorizontalKeyboardControl"));

        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("RPGMenuItemPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuItemSelectedSprite"));

        EditorGUILayout.LabelField("Additional scene objects");
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuItemBackgroundHolder"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuTitle"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuHelp"));
        EditorGUILayout.Space(5);

        if (GUILayout.Button("Add menu item"))
        {
            currentMenu.AddMenuItemGOOnly();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Total Menus: " + RPGMenu.MenuCountExisting);
        EditorGUILayout.LabelField("Navigation stack: " + currentMenu.dbgGlobalStackCount);
        EditorGUILayout.LabelField("Sections in this menu: " + currentMenu.dbgSectionCount);
        EditorGUILayout.LabelField("Child windows open: " + currentMenu.WindowsOpenAtTheSameTime.Count);
        EditorGUILayout.LabelField(("Current input: " + RPGMenu.dbgCurrentInputMenu));


        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Items: ");

        currentMenuItemNames = currentMenu.GetMenuItemNames();

        if(currentMenuItemNames.Count != currentMenu.MenuItemCount)
            currentMenuItemNames = currentMenu.GetMenuItemNames();

        foreach (var name in currentMenuItemNames)
        {
            EditorGUILayout.LabelField("- " + name);
        }



        serializedObject.ApplyModifiedProperties();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RPGMenuSection))]
public class RPGMenuSectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RPGMenuSection currentSection = (RPGMenuSection)target;

        GUILayout.Label("Edit options:");
        if (GUILayout.Button("Add menu item to section"))
        {
            //currentMenu.AddMenuItemGOOnly();
            GameObject gO = new GameObject("Section menu item");
            gO.transform.parent = currentSection.gameObject.transform;
            gO.AddComponent<RPGMenuItem>();
        }

    }
}

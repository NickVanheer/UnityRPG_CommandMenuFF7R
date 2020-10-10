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

        foreach (Transform child in currentSection.transform)
        {
            if (child.gameObject.GetComponent<RPGMenuItem>() != null)
                EditorGUILayout.ObjectField(child.gameObject, typeof(GameObject), true);
        }

        GUILayout.Label("Menu items:");
        if (GUILayout.Button("Add new menu item to this section"))
        {
            //currentMenu.AddMenuItemGOOnly();
            GameObject gO = new GameObject("Section menu item");
            gO.transform.parent = currentSection.gameObject.transform;
            gO.AddComponent<RPGMenuItem>();
        }

    }
}

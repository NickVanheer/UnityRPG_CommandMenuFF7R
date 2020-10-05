using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuItemEditorWindow : EditorWindow
{
    // Start is called before the first frame update
    SerializedProperty property = null;

    public void ShowProperty(SerializedProperty prop)
    {
        this.property = prop;
        MenuItemEditorWindow window = GetWindow(typeof(MenuItemEditorWindow)) as MenuItemEditorWindow;
        this.Show(true);
    }

    private void OnGUI()
    {
        var list = property.FindPropertyRelative("MenuItems");
        var pos = EditorGUILayout.GetControlRect();
        pos.width /= 2;

        //Selection.activeGameObject.GetComponent<RPGMenuItem>().MenuItemData.DynamicMenuData.AddItem(new RPGMenuItemData());

        if (GUI.Button(pos, "Add item"))
        {
            //Selection.activeGameObject.GetComponent<RPGMenuItem>().MenuItemData.DynamicMenuData.AddItem(new RPGMenuItemData());
            list.InsertArrayElementAtIndex(list.arraySize);
        }
        
        pos.x += pos.width;
        if (GUI.Button(pos, "Clear all items"))
            list.ClearArray();

        for (int i = 0; i < list.arraySize; i++)
        {
            var listItem = list.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(listItem);
        }

        property.serializedObject.ApplyModifiedProperties();
        
    }
}

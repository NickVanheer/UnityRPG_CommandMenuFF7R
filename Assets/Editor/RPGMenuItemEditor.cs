using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Net.Http.Headers;
using UnityEngine.UI;
using System;
using Assets.Editor;

[CustomEditor(typeof(RPGMenuItem))]
public class RPGMenuItemEditor : Editor
{
    SerializedProperty MenuItemData;

    void OnEnable()
    {
        MenuItemData = serializedObject.FindProperty("MenuItemData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GameObject selectedGO = Selection.activeGameObject;
        RPGMenuItem menuItem = selectedGO.GetComponent<RPGMenuItem>();
        EditorGUILayout.LabelField("Currently editing menu item: " + selectedGO.name.ToString());

        var nameString = selectedGO.GetComponent<RPGMenuItem>().MenuItemData.Text;

        //Update scene objects with the data
        selectedGO.name = nameString;
        if (selectedGO.transform.childCount > 0)
        {
            Text textComp = selectedGO.transform.GetChild(0).GetComponent<Text>();
            if(textComp)
                textComp.text = nameString;
        }


        EditorGUILayout.PropertyField(MenuItemData);

        if (menuItem.MenuItemData.ItemType == MenuItemActionType.NewMenuSection && menuItem.MenuItemData.DynamicMenuData == null)
        {
            if (GUILayout.Button("Add section"))
            {
                GameObject gO = new GameObject("New section");
                gO.transform.parent = menuItem.gameObject.transform;
                gO.AddComponent<RPGMenuSection>();
                menuItem.MenuItemData.DynamicMenuData = gO;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(RPGMenuItemData))]
public class RPGMenuItemDataDrawer : PropertyDrawer
{
    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1));

    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
       EditorGUI.BeginProperty(position, label, property);

        if (label.text != "Menu Item Data")
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Item: " + property.FindPropertyRelative("Text").stringValue, MessageType.Info);
        }

        EditorGUILayout.PropertyField(property.FindPropertyRelative("Text"), new GUIContent("Name"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("HelpText"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("ItemType"));


    int enumValue = property.FindPropertyRelative("ItemType").enumValueIndex;
    //int enumValue = 1;
    //typeOfItem = (ActionType)EditorGUILayout.EnumPopup("Type", typeOfItem);

    //MenuItemActionType actionType = MenuItemActionType.NewWindow;
        MenuItemActionType actionType = (MenuItemActionType)enumValue;

        switch (actionType)
        {
            case MenuItemActionType.PerformAction:
                EditorGUILayout.PropertyField(property.FindPropertyRelative("ActionToPerform"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("ATBCost"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("MPCost"));
                //EditorGUILayout.Space(10);
                break;
            case MenuItemActionType.NewWindow:

                var list = property.FindPropertyRelative("WindowsToOpen");
                var pos = EditorGUILayout.GetControlRect();
                pos.width /= 2;

                if (GUI.Button(pos, "Add window"))
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                }
                pos.x += pos.width;
                if (GUI.Button(pos, "Clear windows"))
                {
                    list.ClearArray();
                }
    
                for (int i = 0; i < list.arraySize; i++)
                {
                    var listItem = list.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(listItem);
                }

                break;
            case MenuItemActionType.NewMenuSection:
                EditorGUILayout.PropertyField(property.FindPropertyRelative("DynamicMenuData"));
                break;
            default:
                break;
        }

        //MenuItemEditorWindow window = (MenuItemEditorWindow)EditorWindow.GetWindow(typeof(MenuItemEditorWindow), false, "Edit window");

        EditorGUI.EndProperty();
        //EditorGUI.indentLevel--;
    }
}

//[CustomPropertyDrawer(typeof(RPGMenuData))]
/*
public class RPGMenuDataDrawer : PropertyDrawer
{
    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(true, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.PropertyField(property.FindPropertyRelative("MenuName"));

        var list = property.FindPropertyRelative("MenuItems");
        var pos = EditorGUILayout.GetControlRect();

        pos.y += 20;
        for (int i = 0; i < list.arraySize; i++)
        {
            var listItem = list.GetArrayElementAtIndex(i);

            //EditorGUILayout.PropertyField(listItem);
            pos = EditorGUILayout.GetControlRect();
           // if (GUI.Button(pos, "Edit"))
            //{
                //MenuItemEditorWindow window = (MenuItemEditorWindow)EditorWindow.GetWindow(typeof(MenuItemEditorWindow), false, "Edit window");
              //  window.ShowProperty(listItem);
            //}
        }

        EditorGUI.EndProperty();
    }

    void EditScreen()
    {

    }
}
*/

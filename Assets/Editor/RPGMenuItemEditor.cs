using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Net.Http.Headers;
using UnityEngine.UI;
using System;

[CustomEditor(typeof(RPGMenuItem))]
public class RPGMenuItemEditor : Editor
{
    SerializedProperty MenuItemData;
    SerializedProperty NameData;

    void OnEnable()
    {
        MenuItemData = serializedObject.FindProperty("MenuItemData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GameObject selectedGO = Selection.activeGameObject;
        EditorGUILayout.LabelField("Currently editing menu item: " + selectedGO.name.ToString());

        var nameString = selectedGO.GetComponent<RPGMenuItem>().MenuItemData.Text;

        //Update scene objects with the data
        if (true)
        {
            selectedGO.name = nameString;
            selectedGO.transform.GetChild(0).GetComponent<Text>().text = nameString;
        }

        EditorGUILayout.PropertyField(MenuItemData);
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

        //typeOfItem = (ActionType)EditorGUILayout.EnumPopup("Type", typeOfItem);
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
                //EditorGUILayout.LabelField("Menu to show", EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("MenuToOpen"));
                break;
            case MenuItemActionType.NewMenuSection:
                EditorGUILayout.PropertyField(property.FindPropertyRelative("DynamicMenuData"), true);
                break;
            default:
                break;
        }

        //EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(RPGMenuData))]
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
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("MenuName"));

        var list = property.FindPropertyRelative("MenuItems");

        var pos = EditorGUILayout.GetControlRect();
        if (GUI.Button(pos, "Add Item"))
        {
            list.InsertArrayElementAtIndex(list.arraySize);
        }

        pos.y += 20;
        if (GUI.Button(pos, "Clear items"))
        {
            list.ClearArray();
        }

        for (int i = 0; i < list.arraySize; i++)
        {
            var listItem = list.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(listItem);
            pos = EditorGUILayout.GetControlRect();
            if (GUI.Button(pos, "Delete"))
            {
                list.DeleteArrayElementAtIndex(i);
            }

        }

        EditorGUI.EndProperty();
    }
}

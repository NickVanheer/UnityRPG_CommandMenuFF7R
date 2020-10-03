using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum Gender
{
    Male,
    Female,
    Other
}

// Needs the Serializable attribute otherwise the CustomPropertyDrawer wont be used
[Serializable]
public class UserInfo
{
    public string Name;
    public int Age;
    public Gender Gender;
}

public class Toko : MonoBehaviour
{
    public UserInfo UInfo;
}

[CustomPropertyDrawer(typeof(UserInfo))]
public class UserInfoDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // The 6 comes from extra spacing between the fields (2px each)
        return EditorGUIUtility.singleLineHeight * 4 + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //EditorGUI.LabelField(position, "Derp");

        var nameRect = new Rect(position.x, position.y + 18, position.width, 16);
        var ageRect = new Rect(position.x, position.y + 36, position.width, 16);
        var genderRect = new Rect(position.x, position.y + 54, position.width, 16);

        EditorGUI.indentLevel++;

        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"));
        EditorGUI.PropertyField(ageRect, property.FindPropertyRelative("Age"));
        EditorGUI.PropertyField(genderRect, property.FindPropertyRelative("Gender"));

        var prop = property.FindPropertyRelative("Gender");
        if (prop.enumValueIndex == 1)
            EditorGUILayout.TextField("Derping");

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}

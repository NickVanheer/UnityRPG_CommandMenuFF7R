using UnityEngine;
using UnityEditor.IMGUI.Controls;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsFocus"));

        EditorGUILayout.Space(10);
        
        EditorGUILayout.LabelField("Menu Items: ");

        foreach (Transform child in currentMenu.HostWindowCommandMenuContent.transform)
        {
            if (child.gameObject.GetComponent<RPGMenuItem>() != null)
                EditorGUILayout.ObjectField(child.gameObject, typeof(GameObject), true);
        }

        if (GUILayout.Button("Add menu item"))
        {
            currentMenu.AddMenuItemGOOnly();
        }


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug data (runtime only): ");
        EditorGUILayout.LabelField("Menu ID: " + currentMenu.ID);
        EditorGUILayout.LabelField("Total Menus: " + RPGMenu.MenuCountExisting);
        EditorGUILayout.LabelField("Navigation stack: " + currentMenu.dbgGlobalStackCount);
        EditorGUILayout.LabelField("Sections in this menu: " + currentMenu.dbgSectionCount);
        EditorGUILayout.LabelField("Child windows open: " + currentMenu.WindowsOpenAtTheSameTime.Count);
        EditorGUILayout.LabelField(("Current input: " + RPGMenu.dbgCurrentInputMenu));

        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/UI/CrystalUI Menu", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        var panelObject = Resources.Load("RPGMenuPanel") as GameObject;

        if(panelObject)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
                parent = GetOrCreateCanvasGameObject();

            GameObject gO = Instantiate<GameObject>(panelObject, parent.transform, false);

            //GameObject gO = PrefabUtility.InstantiatePrefab(panelObject) as GameObject;
            GameObjectUtility.SetParentAndAlign(gO, menuCommand.context as GameObject);
        
            Undo.RegisterCreatedObjectUndo(gO, "Create " + gO.name);
            Selection.activeObject = gO;
        }
    }

    static public GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        // Try to find a gameobject that is the selected GO or one if its parents.
        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in selection or its parents? Then use just any canvas..
        canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        return CreateNewUI();
    }

    static public GameObject CreateNewUI()
    {
        // Root for the UI
        var root = new GameObject("Canvas");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

        CreateEventSystem(false);
        return root;
    }

    public static void CreateEventSystem(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        CreateEventSystem(true, parent);
    }

    private static void CreateEventSystem(bool select)
    {
        CreateEventSystem(select, null);
    }

    private static void CreateEventSystem(bool select, GameObject parent)
    {
        var esys = Object.FindObjectOfType<EventSystem>();
        if (esys == null)
        {
            var eventSystem = new GameObject("EventSystem");
            GameObjectUtility.SetParentAndAlign(eventSystem, parent);
            esys = eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
        }

        if (select && esys != null)
        {
            Selection.activeGameObject = esys.gameObject;
        }
    }

}

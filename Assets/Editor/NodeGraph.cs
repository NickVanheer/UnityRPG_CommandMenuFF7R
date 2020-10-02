﻿using Assets.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static RPGMenu;
using static ToggleUINode;

public class NodeGraphEditor : EditorWindow
{
    public NodeGraphObject NodeGraphDataObject { get; private set; }
    private NodeGraphView graph;

    private string filename = "dataset";
    private string newMenuName = "RPGMenu";

    void OnEnable()
    {
        ConstructNodeGraph();
        ConstructToolbar();
    }

    // Update is called once per frame
    void OnDisable()
    {
        rootVisualElement.Remove(graph);
    }

    [OnOpenAsset(1)]
    public static bool ShowWindow(int instanceID, int line)
    {
        var item = EditorUtility.InstanceIDToObject(instanceID);

        if (item is NodeGraphObject)
        {
            var window = (NodeGraphEditor)GetWindow(typeof(NodeGraphEditor));
            window.titleContent = new GUIContent("UI Graph");
            window.NodeGraphDataObject = item as NodeGraphObject;

            window.minSize = new Vector2(500, 250);
            return true;
        }

        return false;
    }

    [MenuItem("CrystalUI/DialogueGraph")]
    public static void OpenWindow()
    {
        var window = GetWindow<NodeGraphEditor>();
        window.titleContent = new GUIContent("UI Graph");
        window.minSize = new Vector2(500, 250);
    }

    public void DoSaveLoad(bool isSave)
    {
        if(string.IsNullOrEmpty(filename))
        {
            EditorUtility.DisplayDialog("Invalid filename", "Please enter a valid filename", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(graph);

        if (isSave)
            saveUtility.SaveData(filename);
        else
            saveUtility.LoadData(filename);
    }

    void ConstructNodeGraph()
    {
        graph = new NodeGraphView() { name = "UI nodes" };
        graph.EditorData = this;
        graph.StretchToParentSize();
        rootVisualElement.Add(graph);
    }

    public void ConstructToolbar()
    {
        var toolbar = new Toolbar();
        
        var menuNameTextField = new TextField("Menu name: ");
        menuNameTextField.SetValueWithoutNotify(newMenuName);
        menuNameTextField.MarkDirtyRepaint();
        menuNameTextField.RegisterValueChangedCallback(evt => newMenuName = evt.newValue);
        toolbar.Add(menuNameTextField);

        //toolbar.Add(new Button(() => DoSaveLoad(true)) { text = "Save graph" }) ;
        //toolbar.Add(new Button(() => DoSaveLoad(false)) { text = "Load graph" });

        var rpgMenuCreateNode = new Button(() => { graph.AddRPGMenuNode(newMenuName); });
        rpgMenuCreateNode.text = "Add RPGMenu";
        toolbar.Add(rpgMenuCreateNode);

        /*
        var nodeCreate = new Button(() => { graph.AddToggleUINode(); });
        nodeCreate.text = "Add UI node";
        toolbar.Add(nodeCreate);
        */

        rootVisualElement.Add(toolbar);
    }
}

public class NodeGraphView : GraphView
{
    public NodeGraphEditor EditorData;
    private int numberOfRPGMenus = 0;

    public NodeGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("GridStylesheet"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if (startPort != port & startPort.node != port.node)
                compatiblePorts.Add(port);

        });

        return compatiblePorts;
    }

    public UINode GenerateEntryPointNode(string name)
    {
        var node = new UINode() { title = name, IsEntryPoint = true, NodeGUID = Guid.NewGuid().ToString() };
        node.SetPosition(new Rect(10, 10, 200, 150));
        node.AddOutputPort("Output");

        node.RefreshExpandedState();
        node.RefreshPorts();

        return node;
    }

    public UINode GenerateToggleUINode()
    {
        var node = new ToggleUINode() { title = "UI", IsEntryPoint = false, NodeGUID = Guid.NewGuid().ToString() };
        node.SetPosition(new Rect(10, 10, 200, 150));
        node.AddInputPort("Input");
        node.AddOutputPort("Output");

        node.titleContainer.Add(new Label(node.NodeGUID));

        ObjectField objField = new ObjectField("UI object");
        objField.objectType = typeof(GameObject);
        node.contentContainer.Add(objField);

        EnumField actionField = new EnumField("Action ", UIAction.Show);
        node.contentContainer.Add(actionField);

        return node;
    }

    public void CreateMenuInEditor(UINode menuNode)
    {
        Assert.IsNotNull(EditorData.NodeGraphDataObject.RPGMenuPanelPrefab, "RPG Menu Panel prefab object is not set");
        
        Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        GameObject gO = GameObject.Instantiate(EditorData.NodeGraphDataObject.RPGMenuPanelPrefab, canvas.transform);

        gO.name = menuNode.MenuData.MenuName;
 
        RPGMenu menu = gO.GetComponent<RPGMenu>();

        if (menu == null)
        {
            Utils.MessageBox("No RPGMenu Component attached to this game object");
            return;
        }

        //Add all items
        menuNode.MenuData.MenuItems.ForEach((item) => { menu.AddMenuItem(item);  });
    }

    public UINode GenerateRPGMenuNode(string menuName)
    {
        var node = new UINode() { title = menuName, IsEntryPoint = false, NodeGUID = Guid.NewGuid().ToString() };
        node.SetPosition(new Rect(10, 10, 480, 180));
        node.AddInputPort("Input");

        RPGMenuData thisNodeData = new RPGMenuData(menuName);
        node.MenuData = thisNodeData;

        EnumField interactTypeField = new EnumField(RPGMenuType.NewWindow);
        if (numberOfRPGMenus > 0)
        {
            //Allow user to select what kind of menu it is
            node.titleContainer.Add(interactTypeField);
        }

        //Create button 
        Button CreateMenuInEditorButton = new Button(() => { CreateMenuInEditor(node); });
        CreateMenuInEditorButton.text = "Build RPG Menu";
        node.titleButtonContainer.Add(CreateMenuInEditorButton);

        //Content Container
        var entryName = new TextField("Menu iten name");
        var helpText = new TextField("Help text");
        node.contentContainer.Add(entryName);
        node.contentContainer.Add(helpText);

        var mpCost = new IntegerField(120);
        mpCost.label = "MP cost";
        var apCost = new IntegerField(6);
        apCost.label = "AP cost";
        node.contentContainer.Add(apCost);
        node.contentContainer.Add(mpCost);
        
        //Add output connectors
        Button addButton = new Button(() => { AddRPGMenuItemToNode(node, entryName.text, helpText.text, mpCost.value, apCost.value); });

        addButton.text = "Add menu item";
        node.contentContainer.Add(addButton);

        node.RefreshExpandedState();
        node.RefreshPorts();
   
        return node;
    }
    public void AddRPGMenuItemToNode(UINode node, string name, string help, int mp, int atb)
    {
        RPGMenuItemData menuItem = new RPGMenuItemData(name, help);
        menuItem.Text = name;
        menuItem.HelpText = help;
        menuItem.MPCost = mp;
        menuItem.ATBCost = atb;

        node.MenuData.MenuItems.Add(menuItem);
        node.AddOutputPort(menuItem.Text);
    }

    public void AddRPGMenuNode(string menuName)
    {
        AddElement(GenerateRPGMenuNode(menuName));
        numberOfRPGMenus++;
    }

    public void AddToggleUINode()
    {
        AddElement(GenerateToggleUINode());
    }
}

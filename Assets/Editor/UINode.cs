using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public enum RPGMenuType { ChangeContent, NewWindow };

public class UINode : Node
{
    public string NodeGUID;
    public RPGMenuData MenuData;
    public RPGMenuType Type;
    public bool IsEntryPoint = false;

    public UINode()
    {
        //styleSheets.Add(Resources.Load<StyleSheet>("NodeStylesheet"));
        AddToClassList("node");
    }

    private Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
    }

    Port addOutputPort(string name)
    {
        var outputPort = GetPortInstance(Direction.Output);
        outputPort.portName = name;
        outputContainer.Add(outputPort);
        return outputPort;
    }

    public void AddInputPort(string name)
    {
        var inputPort = GetPortInstance(Direction.Input);
        inputPort.portName = name;
        inputContainer.Add(inputPort);
    }

    public void AddOutputPort(string outputName = "")
    {
        var generatedPort = addOutputPort(outputName);
        int portCount = outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = string.IsNullOrEmpty(outputName) ? $"Output {portCount}" : outputName;

        RefreshExpandedState();
        RefreshPorts();
    }
}

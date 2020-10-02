using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "CrystalUI/UI Node Graph")]
public class NodeGraphObject  : ScriptableObject
{
    public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
    public List<NodeData> nodeData = new List<NodeData>();

    public GameObject RPGMenuPanelPrefab;
}

[System.Serializable]
public class NodeData
{
    public string NodeGUID;
    public Vector2 Position;
    public string NodeType;
    public string AdditionalJSONData;
}

[System.Serializable]
public class NodeLinkData
{
    public string BaseNodeGUID;
    public string InputPortName;
    public string OutputPortName;
    public string TargetNodeGUID;
}

using Assets.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSaveUtility : MonoBehaviour
{
    private NodeGraphView targetGraphView;
    private List<Edge> Edges => targetGraphView.edges.ToList();
    private List<UINode> Nodes => targetGraphView.nodes.ToList().Cast<UINode>().ToList();
    public static GraphSaveUtility GetInstance(NodeGraphView graphView)
    {
        return new GraphSaveUtility
        {
            targetGraphView = graphView
        };
    }

    public void SaveData(string filename)
    {
        if (!Edges.Any())
            return; //No connections 

        var dialogueContainer = ScriptableObject.CreateInstance<NodeGraphObject>();
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as UINode;
            var inputNode = connectedPorts[i].input.node as UINode;

            Utils.MessageBox(inputNode.NodeGUID);
        }

    }
    public void LoadData(string filename)
    {

    }
}

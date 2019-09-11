using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voronoi2;

public class Graph
{
    private List<Vector3> nodes;
    private List<GraphEdge> edges;

    public Graph()
    {
        nodes = new List<Vector3>();
        edges = new List<GraphEdge>();
    }

    public void AddNode(Vector3 position)
    {
        nodes.Add(position);
    }
    public void AddEdge(GraphEdge edge)
    {
        edges.Add(edge);
    }

    public void ClearEdges()
    {
        edges.Clear();
    }
}

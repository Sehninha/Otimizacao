using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voronoi2;

public class Graph
{
    private List<Transform> nodes;
    private List<Edge> edges;

    public Graph()
    {
        nodes = new List<Transform>();
        edges = new List<Edge>();
    }

    public void AddNode(Transform point)
    {
        nodes.Add(point);
    }

    public void AddEdge(Edge edge)
    {
        edges.Add(edge);
    }

    public Transform[] GetNodes()
    {
        return nodes.ToArray();
    }

    public Edge[] GetEdges()
    {
        return edges.ToArray();
    }

    public void ClearEdges()
    {
        edges.Clear();
    }
}

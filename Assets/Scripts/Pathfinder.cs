using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private Transform begin, end;
    private List<Transform> nodesOnHold, examinedNodes;
    private Transform[] nodes;
    private Edge[] edges;

    public Edge[] CalculatePath(Graph graph, Transform begin, Transform end)
    {
        nodes = graph.GetNodes();
        edges = graph.GetEdges();

        this.begin = begin;
        this.end = end;

        nodesOnHold = new List<Transform>();
        examinedNodes = new List<Transform>();

        nodesOnHold.Add(begin);
        examinedNodes.Add(begin);
    }

    //Examina os nodos e analisar qual tem a menor heuristica
    public void Examine(Transform currentNode)
    {
        if(currentNode = end)
        {
            return;
        }
       
        nodesOnHold.Remove(currentNode);
       
        Transform[] adjacencies = CheckAdjacencies(currentNode);

        foreach(Transform adjacency in adjacencies)
        {
            if (!examinedNodes.Find( item => adjacency == item ))
            {
                nodesOnHold.Add(adjacency);
                examinedNodes.Add(adjacency);
            }
        }

        Examine(CalculateNextNode());
    }

    //passa um nodo para funçao, a função vai analisar todas as edges, e as edges que conterem esse nodo vão retornar o próximo nodo
    public Transform[] CheckAdjacencies(Transform node)
    {
        List<Transform> adjacencies = new List<Transform>();

        foreach(Edge edge in edges)
        {
            if (node == edge.nodeA)
            {
                adjacencies.Add(edge.nodeB);
            }
            else if (node == edge.nodeB)
            {
                adjacencies.Add(edge.nodeA);
            }
        }

        return adjacencies.ToArray();
    }


    //Cuida de analisa e retornar sempre o nodo de menor heristica
    public Transform CalculateNextNode()
    {
        float smallestHeuristic = Mathf.Infinity;
        Transform smallestHeuristicNode = begin;

        foreach (Transform node in nodesOnHold)
        {
            float heuristic = Vector3.Distance(node.position, begin.position) + Vector3.Distance(node.position, end.position);

            if(heuristic < smallestHeuristic)
            {
                smallestHeuristic = heuristic;
                smallestHeuristicNode = node;
            }
        }

        return smallestHeuristicNode;
    }

}

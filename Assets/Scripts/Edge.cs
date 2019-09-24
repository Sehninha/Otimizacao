using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Transform nodeA;
    public Transform nodeB;

    public Edge(Transform nodeA, Transform nodeB)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
    }
}

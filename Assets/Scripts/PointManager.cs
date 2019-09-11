using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voronoi2;

public class PointManager : MonoBehaviour
{
    private Transform[] hull;
    private List<GraphEdge> edges;

    private GameObject point;
    private new Camera camera;
    private LineRenderer line;

    private GrahamScan grahamScan;
    private Voronoi voronoi;
    private Graph graph;

    private void Start()
    {
        point = Resources.Load<GameObject>("Prefabs/Point");
        camera = Camera.main;
        line = GetComponent<LineRenderer>();

        grahamScan = new GrahamScan();
        voronoi = new Voronoi(0f);
        graph = new Graph();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 spawnPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            spawnPosition.z = 0;

            if (CheckPointAvailability(spawnPosition))
            {
                Instantiate(point, spawnPosition, Quaternion.identity, transform).name = "Point";
                graph.AddNode(spawnPosition);
                

                List<Transform> points = GetChildren();

                // Calculate Hull
                if (transform.childCount >= 3)
                {
                    hull = grahamScan.CalculateHull(points);
                }

                // Voronoi Diagram
                List<double> xPositions = new List<double>();
                List<double> yPositions = new List<double>();

                for(int i = 0; i < points.Count; i++)
                {
                    xPositions.Add(points[i].position.x);
                    yPositions.Add(points[i].position.y);
                }

                edges = voronoi.generateVoronoi(xPositions.ToArray(), yPositions.ToArray(), -15, 15, -15, 15);

                graph.ClearEdges();

                foreach(GraphEdge edge in edges)
                {
                    Vector3 positionA = new Vector3((float)xPositions[edge.site1], (float)yPositions[edge.site1], 0);
                    Vector3 positionB = new Vector3((float)xPositions[edge.site2], (float)yPositions[edge.site2], 0);

                    Debug.DrawLine(positionA, positionB, Color.blue, 1f);
                }
            }
        }

        // Draw Hull
        if (hull != null)
        {
            line.positionCount = hull.Length;
            for (int i = 0; i < hull.Length; i++)
            {
                line.SetPosition(i, hull[i].position);
            }
        }

        // Draw Edges
        //if (edges != null)
        //{
        //    foreach(GraphEdge edge in edges)
        //    {
        //        Debug.DrawLine(new Vector3((float)edge.x1, (float)edge.y1, 0), new Vector3((float)edge.x2, (float)edge.y2, 0), Color.blue);
        //    }
        //}
    }

    private bool CheckPointAvailability(Vector3 point)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (point == transform.GetChild(i).position)
            {
                return false;
            }
        }

        return true;
    }

    private List<Transform> GetChildren()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        return children;
    }

    private void DeleteChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

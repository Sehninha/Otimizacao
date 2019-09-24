using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voronoi2;

public class PointManager : MonoBehaviour
{
    private GameObject point;

    private new Camera camera;
    private LineRenderer hullLine;
    private Transform graphLines;

    private Transform[] hull;
    private Graph graph;
    private GrahamScan grahamScan;
    private Voronoi voronoi;

    private void Start()
    {
        point = Resources.Load<GameObject>("Prefabs/Point");

        camera = Camera.main;
        hullLine = FindObjectOfType<LineRenderer>();
        graphLines = GameObject.Find("Graph Lines").transform;

        graph = new Graph();
        grahamScan = new GrahamScan();
        voronoi = new Voronoi(0f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdatePoints();
        }
    }

    private void UpdatePoints()
    {
        Vector3 spawnPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0;

        if (CheckPointAvailability(spawnPosition))
        {
            GameObject newPoint = Instantiate(point, spawnPosition, Quaternion.identity, transform);
            newPoint.name = "Point";
            graph.AddNode(newPoint.transform);

            List<Transform> points = GetChildren();

            // Calculate Hull
            if (points.Count > 2)
            {
                hull = grahamScan.CalculateHull(points);
                UpdateHull();
            }

            // Calculate Voronoi
            List<double> xPositions = new List<double>();
            List<double> yPositions = new List<double>();

            for (int i = 0; i < points.Count; i++)
            {
                xPositions.Add(points[i].position.x);
                yPositions.Add(points[i].position.y);
            }

            List<GraphEdge> edges = voronoi.generateVoronoi(xPositions.ToArray(), yPositions.ToArray(), 0, 2, 0, 2);

            graph.ClearEdges();

            foreach (GraphEdge edge in edges)
            {
                Transform pointA = points[edge.site1];
                Transform pointB = points[edge.site2];

                graph.AddEdge(new Edge(pointA, pointB));
            }

            UpdateEdges();
        }
    }

    private void UpdateHull()
    {
        hullLine.positionCount = hull.Length;
        for (int i = 0; i < hull.Length; i++)
        {
            hull[i].GetComponent<RectTransform>().sizeDelta = new Vector2(12, 12);
            hull[i].GetComponent<Image>().color = Color.red;

            hullLine.SetPosition(i, hull[i].position);
        }
    }

    private void UpdateEdges()
    {
        for (int i = graphLines.childCount - 1; i >= 0; i--)
        {
            Destroy(graphLines.GetChild(i).gameObject);
        }

        Edge[] edges = graph.GetEdges();

        foreach (Edge edge in edges)
        {
            if (BelongsToHull(edge.nodeA) && BelongsToHull(edge.nodeB))
            {
                continue;
            }

            GameObject newLine = new GameObject();
            newLine.transform.parent = graphLines;
            LineRenderer line = newLine.AddComponent<LineRenderer>();

            line.material = hullLine.material;
            line.startWidth = 0.005f;
            line.startColor = Color.black;
            line.endColor = Color.black;

            line.positionCount = 2;
            line.SetPositions(new Vector3[]
            {
                edge.nodeA.position,
                edge.nodeB.position
            });
        }
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

    private bool BelongsToHull(Transform point)
    {
        if (hull == null)
        {
            return false;
        }

        for(int i = 0; i < hull.Length; i++)
        {
            if (hull[i] == point)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetPoint(Transform point)
    {
        point.GetComponent<RectTransform>().sizeDelta = new Vector2(8, 8);
        point.GetComponent<Image>().color = Color.black;
    }

    private List<Transform> GetChildren()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            ResetPoint(child);
            children.Add(child);
        }

        return children;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voronoi2;

public enum Mode
{
    Create,
    Select
}

public class PointManager : MonoBehaviour
{
    public Mode mode;

    private GameObject point;

    private Transform firstPathfinderPoint;
    private Transform secondPathfinderPoint;

    private new Camera camera;
    private LineRenderer hullLine;
    private Transform graphLines;
    private LineRenderer pathLine;

    private Transform[] hull;
    private Edge[] path;
    private Graph graph;
    private GrahamScan grahamScan;
    private Voronoi voronoi;
    private Pathfinder pathfinder;

    private void Start()
    {
        mode = Mode.Create;

        point = Resources.Load<GameObject>("Prefabs/Point");

        camera = Camera.main;
        hullLine = GameObject.Find("Hull Line").GetComponent<LineRenderer>();
        graphLines = GameObject.Find("Graph Lines").transform;
        pathLine = GameObject.Find("Path Line").GetComponent<LineRenderer>();

        hull = new Transform[0];
        path = new Edge[0];
        graph = new Graph();
        grahamScan = new GrahamScan();
        voronoi = new Voronoi(0f);
        pathfinder = new Pathfinder();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mode == Mode.Create)
            {
                mode = Mode.Select;
                camera.backgroundColor = Color.gray;
            }
            else
            {
                mode = Mode.Create;
                camera.backgroundColor = Color.white;
            }
        }

        switch (mode)
        {
            case Mode.Create:
                if (Input.GetMouseButtonDown(0))
                {
                    UpdatePoints();
                }
                break;
        }
    }

    public void SelectPoint(Transform point)
    {
        if (point == firstPathfinderPoint)
        {
            return;
        }

        if (path.Length > 0)
        {
            ResetAllPoints();
            UpdateHull();

            firstPathfinderPoint = null;
            secondPathfinderPoint = null;

            path = new Edge[0];
            pathLine.SetPositions(new Vector3[0]);
        }

        if (firstPathfinderPoint == null)
        {
            firstPathfinderPoint = point;
            firstPathfinderPoint.GetComponent<Image>().color = Color.blue;
        }
        else
        {
            secondPathfinderPoint = point;
            secondPathfinderPoint.GetComponent<Image>().color = Color.blue;
            path = pathfinder.CalculatePath(graph, graph.FindNode(firstPathfinderPoint), graph.FindNode(secondPathfinderPoint));

            pathLine.positionCount = path.Length + 1;

            pathLine.SetPosition(0, secondPathfinderPoint.position);

            //Desenha as linhas do segundo ponto selecionado até o primeiro seguindo os pontos já calculados
            for (int i = 0; i < path.Length - 1; i++)
            {
                float distanceNodeA = Mathf.Pow(firstPathfinderPoint.position.x - path[i].nodeA.transform.position.x, 2) + Mathf.Pow(firstPathfinderPoint.position.y - path[i].nodeA.transform.position.y, 2);
                float distanceNodeB = Mathf.Pow(firstPathfinderPoint.position.x - path[i].nodeB.transform.position.x, 2) + Mathf.Pow(firstPathfinderPoint.position.y - path[i].nodeB.transform.position.y, 2);

                if (distanceNodeA < distanceNodeB)
                {
                    pathLine.SetPosition(i + 1, path[i].nodeA.transform.position);
                }
                else
                {
                    pathLine.SetPosition(i + 1, path[i].nodeB.transform.position);
                }
            }
            pathLine.SetPosition(path.Length, firstPathfinderPoint.position);
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
            newPoint.GetComponent<Point>().manager = this;
            graph.AddNode(new Node(newPoint.transform));

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

                graph.AddEdge(new Edge(graph.FindNode(pointA),
                                       graph.FindNode(pointB)));
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
            if (BelongsToHull(edge.nodeA.transform) && BelongsToHull(edge.nodeB.transform))
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
                edge.nodeA.transform.position,
                edge.nodeB.transform.position
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

        for (int i = 0; i < hull.Length; i++)
        {
            if (hull[i] == point)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetAllPoints()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ResetPoint(transform.GetChild(i));
        }
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    private Transform[] hull;

    private GameObject point;
    private new Camera camera;
    private LineRenderer line;

    private GrahamScan grahamScan;

    private void Start()
    {
        point = Resources.Load<GameObject>("Prefabs/Point");
        camera = Camera.main;
        line = GetComponent<LineRenderer>();

        grahamScan = new GrahamScan();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 spawnPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            spawnPosition.z = 0;

            Instantiate(point, spawnPosition, Quaternion.identity, transform).name = "Point";

            // Reset Colors
            ResetColors();

            // Calculate Hull
            if (transform.childCount >= 3)
            {
                hull = grahamScan.CalculateHull(GetChildren());
            }
        }

        if (hull == null)
        {

        }
    }

    private void ResetColors()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
        }
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

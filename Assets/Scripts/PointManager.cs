using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    private GameObject point;
    private new Camera camera;

    private void Start()
    {
        point = Resources.Load<GameObject>("Prefabs/Point");
        camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 spawnPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            spawnPosition.z = 0;

            Instantiate(point, spawnPosition, Quaternion.identity, transform);
        }
    }

    private Transform[] GetChildren()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        return children.ToArray();
    }

    private void DeleteChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

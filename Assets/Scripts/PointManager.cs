using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PointManager))]
public class PointManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PointManager manager = (PointManager)target;

        if (GUILayout.Button("Default Points"))
        {
            manager.mousePlacement = false;
            manager.DefaultPoints();
        }

        if (GUILayout.Button("Random Points"))
        {
            manager.mousePlacement = false;
            manager.RandomPoints();
        }

        if (GUILayout.Button("Mouse Placement"))
        {
            manager.mousePlacement = true;
            manager.DeleteChildren();
        }
    }
}
#endif

public class PointManager : MonoBehaviour
{
    private new Camera camera;
    private GameObject point;

    [HideInInspector]
    public bool mousePlacement;

    private void OnValidate()
    {
        mousePlacement = false;

        camera = Camera.main;
        point = Resources.Load<GameObject>("Prefabs/Point");
    }

    private void Update()
    {
        if (mousePlacement)
        {
            MousePlacement();
        }
    }

    public void DefaultPoints()
    {
        DeleteChildren();

        Instantiate(point, new Vector3(-13, 0.5f, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(-10.5f, -11.5f, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(-10, 9, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(-4.5f, -2, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(-1, 8.5f, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(0.5f, 6, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(0.5f, -12, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(2, 12.5f, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(3.5f, 11, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(5, 11.5f, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(5.5f, 3, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(5.5f, -7, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(6.5f, 3.2f, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(7, -10, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(9, -5, 0), Quaternion.identity, transform).name = "Point";
        Instantiate(point, new Vector3(11.5f, -4, 0), Quaternion.identity, transform).name = "Point";
    }

    public void RandomPoints()
    {
        DeleteChildren();

        for (int i = 0; i < 16; i++)
        {
            float randomX = Random.Range(-15f, 15f);
            float randomY = Random.Range(-15f, 15f);

            Vector3 randomPosition = new Vector3(randomX, randomY, 0);
            Instantiate(point, randomPosition, Quaternion.identity, transform).name = "Point";
        }
    }

    public void MousePlacement()
    {
        // Spawn a new point when mouse is clicked
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 spawnPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            spawnPosition.z = 10;

            Instantiate(point, spawnPosition, Quaternion.identity, transform).name = "Point";
        }
    }

    public void DeleteChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

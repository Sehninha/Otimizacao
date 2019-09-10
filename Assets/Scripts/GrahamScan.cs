using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrahamScan
{
    public Transform[] CalculateHull(List<Transform> points)
    {
        List<Transform> convexHull = new List<Transform>();

        Transform lowestY = GetLowestY(points);
        lowestY.GetComponent<SpriteRenderer>().color = Color.blue;

        points = points.OrderBy(point => Vector3.Angle(Vector3.right, point.position - lowestY.position)).ToList();

        for (int i = 2; i < points.Count - 2; )
        {
            Vector3 firstAngle = points[i].position - points[i - 1].position;
            Vector3 secondAngle = points[i + 1].position - points[i].position;

            float dot = Vector3.Dot(firstAngle, secondAngle);

            if (dot >= 0)
            {
                i++;
            }
            else
            {
                points.RemoveAt(i);
                i--;
            }
        }

        return points.ToArray();
    }

    private Transform GetLowestY(List<Transform> points)
    {
        Transform lowestY = points[0];

        for(int i = 1; i < points.Count; i++)
        {
            if (points[i].position.y <= lowestY.position.y)
            {
                // In case of same Y, gets the higher X
                if (points[i].position.y == lowestY.position.y &&
                    points[i].position.x < lowestY.position.x)
                {
                    continue;
                }

                lowestY = points[i];
            }
        }

        return lowestY;
    }
}

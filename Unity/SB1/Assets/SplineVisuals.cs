using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FFPath))]
public class SplineVisuals : MonoBehaviour
{
    public Transform PathPointPrefab;
    public Transform PathEdgePrefab;

    // Use this for initialization
    void Start()
    {
        var path = GetComponent<FFPath>();
        if (path == null)
        {
            Debug.LogWarning("PathGenerator didn't have a ff path attached to the object");
            return;
        }

        Debug.Assert(PathPointPrefab != null, "ARG!!!");
        Debug.Assert(PathEdgePrefab != null, "ARG!!!!!!");

        GeneratePath(path);
    }

    private void GeneratePath(FFPath path)
    {
        // setup first point
        {
            var point = Instantiate(PathPointPrefab);
            point.transform.position = path.PositionAtPoint(0);

            point.SetParent(transform, true);
        }


        for (int i = 1; i < path.PointCount; ++i)
        {
            var pastPt = path.PositionAtPoint(i - 1);
            var pt = path.PositionAtPoint(i);
            var point = Instantiate(PathPointPrefab);
            var edge = Instantiate(PathEdgePrefab);

            // set point
            point.position = pt;
            point.SetParent(transform, true);


            // setup edge
            {
                var vecToPt = pt - pastPt;
                var edgePos = pastPt + (vecToPt * 0.5f);

                edge.position = edgePos;
                edge.up = vecToPt.normalized;
                edge.localScale = new Vector3(1, vecToPt.magnitude, 1);
                edge.SetParent(transform, true);
            }

        }



    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLine : MonoBehaviour
{
    internal void Setup(Level level, int snappedDir, int y, int x)
    {
        var rend = GetComponent<LineRenderer>();
        float angle = Mathf.Deg2Rad * (snappedDir * 45.0f);

        Vector3 pt0 = rend.GetPosition(0);
        Vector3 pt1 = rend.GetPosition(1);

        pt0 = new Vector3(x - level.width/2, 0, y - level.height/2);
        pt1 = new Vector3(-Mathf.Cos(angle), 0, -Mathf.Sin(angle));
        pt1 = pt1.normalized;

        // Is diagonal line?
        if (snappedDir % 2 == 1)
            pt1 = pt0 + pt1 * Level.diagonalOffsetDist;
        else
            pt1 = pt0 + pt1 * Level.cardinalOffsetDist;


        rend.SetPosition(0, pt0);
        rend.SetPosition(1, pt1);

        // Set Parent
        transform.SetParent(level.setupRoot.transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUtil
{
    public static void SnapToOffset(int snappedDir, out int yOffset, out int xOffset)
    {
        if (snappedDir % 2 == 1)
        {
            // right or left
            // pos = 3 or 5
            // neg = 1 or 7
            xOffset = -Mathf.Abs((snappedDir - 4) / 3) + Mathf.Abs(snappedDir -4) % 3;

            // up or down
            // pos = 5 or 7
            // neg = 1 or 3
            yOffset = (snappedDir - 4) % 2;
        }
        else
        {
            // right or left
            // pos = 4
            // neg = 0
            xOffset = ((snappedDir - 2) / 2) % 2;

            // up or down
            // pos = 6
            // neg = 2
            yOffset = ((snappedDir - 4) / 2) % 2;
        }
    }
}

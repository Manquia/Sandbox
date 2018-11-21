using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnsetLine : MonoBehaviour
{
    internal LineCommand Set(Level level, LineCommand.Command cmd)
    {
        var lvlInstance = level.levelInstance;
        var rend = GetComponent<LineRenderer>();

        Vector3 pt0 = rend.GetPosition(0);
        Vector3 pt1 = rend.GetPosition(1);
        Vector3 lineVec = pt1 - pt0;
        Vector3 lineVecNorm = lineVec.normalized;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(lineVecNorm.z, lineVecNorm.x) + 180.0f;
        // range: (0,7) even = W,S,E,N odd = SW, SE, NE, NW
        int snappedDir = Mathf.FloorToInt((angle / 45.0f) + 0.5f) % 8;

        int xOffset = 0;
        int yOffset = 0;
        float offsetDist = 0;

        if (snappedDir % 2 == 1)
        {
            offsetDist = Level.diagonalOffsetDist;

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
            offsetDist = Level.cardinalOffsetDist;

            // right or left
            // pos = 4
            // neg = 0
            xOffset = ((snappedDir - 2) / 2) % 2;

            // up or down
            // pos = 6
            // neg = 2
            yOffset = ((snappedDir - 4) / 2) % 2;
        }

        int lineCountToAdd = Mathf.FloorToInt((lineVec.magnitude + (0.5f * offsetDist)) / offsetDist);
        int startX = Mathf.FloorToInt(pt0.x + 0.5f) + Mathf.FloorToInt(level.width  / 2);
        int startY = Mathf.FloorToInt(pt0.z + 0.5f) + Mathf.FloorToInt(level.height / 2);

        int maxX = level.width / 2;
        int maxY = level.height / 2;


        // directional flags
        GameVertex.Direction directionFlags = (GameVertex.Direction)(1 << snappedDir);

        // make line command to run and record
        LineCommand lc;
        lc.gos = new GameObject[lineCountToAdd];
        lc.cmd = cmd;
        lc.moveDelta = Vector2Int.zero;
        lc.flags = GameVertex.Flags.none;

        // @TEMP @TODO @REPLACE
        lc.flags.AddToAll(directionFlags);

        for (int i = 0; i < lineCountToAdd; ++i)
        {
            int y = startY + (i * yOffset);
            int x = startX + (i * xOffset);

            if ((x < 0 || x >= level.width) ||
                (y < 0 || y >= level.height))
            {
                Debug.LogWarning("Unset line tried to play a line outside the bounds. This may be expected");
                break;
            }
                

            // Add array if not already added
            if (lvlInstance.grid[y, x].gos == null)
                lvlInstance.grid[y, x].gos = new GameObject[8];

            
            GameObject go = lvlInstance.grid[y, x].gos[snappedDir];
            if (go == null)
            {
                go = Instantiate(level.settings.prefabs.setLine);
                lvlInstance.grid[y, x].gos[snappedDir] = go;
                go.GetComponent<SetLine>().Setup(level, snappedDir, y, x);
            }
            lc.gos[i] = go;

            SetLine setLine = go.GetComponent<SetLine>();
            setLine.RunCommand(level, lc);
        }


        return lc;
    }

}

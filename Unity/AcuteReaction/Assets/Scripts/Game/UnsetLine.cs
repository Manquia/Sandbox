using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnsetLine : MonoBehaviour
{
    internal Level.LineCommand Set(Level level, Level.LineCommand.Command cmd)
    {
        var lvlInstance = level.levelInstance;
        var rend = GetComponent<LineRenderer>();
        Level.LineCommand lc;

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
        // directional flags
        Level.LevelInstance.GridVertex.Flags flags = (Level.LevelInstance.GridVertex.Flags)(1 << snappedDir);

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


        lc.gos = new GameObject[lineCountToAdd];
        lc.cmd = cmd;
        lc.moveDelta = Vector2Int.zero;

        for (int i = 0; i < lineCountToAdd; ++i)
        {
            int y = startY + (i * yOffset);
            int x = startX + (i * xOffset);
            switch (cmd)
            {
                case Level.LineCommand.Command.None:
                    break;
                case Level.LineCommand.Command.Place:
                    {
                        // Add array if not already added
                        if (lvlInstance.grid[y,x].gos == null)
                            lvlInstance.grid[y,x].gos = new GameObject[8];

                        // Create Objects
                        var go = Instantiate(level.settings.prefabs.setLine);
                        go.GetComponent<SetLine>().Setup(level, snappedDir, y, x);
                        lvlInstance.grid[y,x].gos[snappedDir] = go;
                        lc.gos[i] = go;

                        // Set flags (addative)
                        lvlInstance.grid[y,x].flags |= flags;
                        break;
                    }
                case Level.LineCommand.Command.Destroy:
                    {
                        // Destroy objects
                        if (lvlInstance.grid[y, x].gos != null && lvlInstance.grid[y,x].gos[snappedDir] != null)
                        {
                            var go = lvlInstance.grid[y, x].gos[snappedDir];
                            Destroy(go);
                        }

                        // Set flags (negative)
                        lvlInstance.grid[y,x].flags &= ~flags;
                        break;
                    }
                case Level.LineCommand.Command.Move:
                    // @ TODO
                    break;
            }
        }


        return lc;
    }
}

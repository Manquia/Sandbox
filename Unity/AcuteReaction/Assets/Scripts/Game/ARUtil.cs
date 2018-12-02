using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUtil
{
    // dir 
    //  7  6  5
    //    \|/
    //  0--+--4
    //    /|\
    //  1  2  3

    public static Vector2Int DirToOffset(GameVertex.Direction dir)
    {
        Vector2Int offset = Vector2Int.zero;
        if ((int)dir % 2 == 1)
        {
            // right or left
            // pos = 3 or 5
            // neg = 1 or 7
            offset.x = -Mathf.Abs(((int)dir - 4) / 3) + Mathf.Abs((int)dir -4) % 3;

            // up or down
            // pos = 5 or 7
            // neg = 1 or 3
            offset.y = ((int)dir - 4) % 2;
        }
        else
        {
            // right or left
            // pos = 4
            // neg = 0
            offset.x = (((int)dir - 2) / 2) % 2;

            // up or down
            // pos = 6
            // neg = 2
            offset.y = (((int)dir - 4) / 2) % 2;
        }
        return offset;
    }
    public static float DirToDist(GameVertex.Direction dir)
    {
        if ((int)dir % 2 == 1)
        {
            return Level.diagonalOffsetDist;
        }
        else
        {
            return Level.cardinalOffsetDist;
        }
    }
    public static GameVertex.Edge.Type SnapToType(GameVertex.Direction snappedDir, ref GameVertex.Edge vert)
    {
        switch (snappedDir)
        {
            case GameVertex.Direction.W: return vert.edgeW; 
            case GameVertex.Direction.SW: return vert.edgeSW;
            case GameVertex.Direction.S: return vert.edgeS; 
            case GameVertex.Direction.SE: return vert.edgeSE; 
            case GameVertex.Direction.E: return vert.edgeE; 
            case GameVertex.Direction.NE: return vert.edgeNE; 
            case GameVertex.Direction.N: return vert.edgeN; 
            case GameVertex.Direction.NW: return vert.edgeNW; 
            default: Debug.LogWarning("Warning! Unexpected snappedDir given to SnapToType"); return GameVertex.Edge.Type.None;
        }
    }

    public static GameObject TypeToPrefab(Level level, GameVertex.Edge.Type type)
    {
        switch (type)
        {
            case GameVertex.Edge.Type.solid:     return level.settings.prefabs.solidLine;
            case GameVertex.Edge.Type.conveyer:  return level.settings.prefabs.conveyerLine;
            //case GameVertex.Edge.Type.transform: return level.settings.prefabs.solidLine;
            //case GameVertex.Edge.Type.magnet:    return level.settings.prefabs.solidLine;
            //case GameVertex.Edge.Type.weld:      return level.settings.prefabs.solidLine;
            //case GameVertex.Edge.Type.laser:     return level.settings.prefabs.solidLine;
            //case GameVertex.Edge.Type.ice:       return level.settings.prefabs.solidLine;
            //case GameVertex.Edge.Type.spring:    return level.settings.prefabs.solidLine;


            case GameVertex.Edge.Type.None:
            default: Debug.LogError("Error: Unexpected type given to TypeToPrefab!"); return null;
        }
    }

}
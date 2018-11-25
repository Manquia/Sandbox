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
    public static GameVertex.Edge.Type SnapToType(int snappedDir, ref GameVertex.Edge vert)
    {
        switch (snappedDir)
        {
            case 0: return vert.edgeW; 
            case 1: return vert.edgeSW;
            case 2: return vert.edgeS; 
            case 3: return vert.edgeSE; 
            case 4: return vert.edgeE; 
            case 5: return vert.edgeNE; 
            case 6: return vert.edgeN; 
            case 7: return vert.edgeNW; 
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
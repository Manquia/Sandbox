using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLine : MonoBehaviour
{
    public Vector2Int pos;
    public GameVertex.Direction snappedDir;

    internal void Setup(Level level, GameVertex.Direction givenSnappedDir, int givenY, int givenX)
    {
        var lvlInstance = level.levelInstance;
        lvlInstance.grid[givenY, givenX].gos[(int)givenSnappedDir] = gameObject;

        var rend = GetComponent<LineRenderer>();
        float angle = Mathf.Deg2Rad * ((int)givenSnappedDir * 45.0f);

        Vector3 pt0 = rend.GetPosition(0);
        Vector3 pt1 = rend.GetPosition(1);

        pt0 = new Vector3(givenX - level.width/2, 0, givenY - level.height/2);
        pt1 = new Vector3(-Mathf.Cos(angle), 0, -Mathf.Sin(angle));
        pt1 = pt1.normalized;

        // Is diagonal line?
        if ((int)givenSnappedDir % 2 == 1)
            pt1 = pt0 + pt1 * Level.diagonalOffsetDist;
        else
            pt1 = pt0 + pt1 * Level.cardinalOffsetDist;

        rend.SetPosition(0, pt0);
        rend.SetPosition(1, pt1);

        // Set Parent
        transform.SetParent(level.setupRoot.transform);

        // record data about setline
        pos.x = givenX;
        pos.y = givenY;
        snappedDir = givenSnappedDir;
    }

    // Probaly almost never call this!
    internal void Destroy(Level level)
    {
        var lvlInstance = level.levelInstance;
        var rend = GetComponent<LineRenderer>();

        lvlInstance.grid[pos.y, pos.x].lines = GameVertex.Edge.none;
        lvlInstance.grid[pos.y, pos.x].gos[(int)snappedDir] = null;
    }

    internal void RunCommand(Level level, LineCommand lineCommand)
    {
        switch (lineCommand.cmd)
        {
            case LineCommand.Command.None:
                break;
            case LineCommand.Command.Place:
                Place(level, lineCommand.flags);
                break;
            case LineCommand.Command.Remove:
                Remove(level, lineCommand.flags);
                break;
            case LineCommand.Command.Move:
                break;
        }
    }

    internal void Remove(Level level, GameVertex.Edge flagDelta)
    {
        var lvlInstance = level.levelInstance;
        var rend = GetComponent<LineRenderer>();

        // complete removal?
        if (flagDelta == lvlInstance.grid[pos.y, pos.x].lines)
        {
            gameObject.SetActive(false);
        }

        // TODO: Fix this for the new format
        // lvlInstance.grid[y, x].flags &= ~flagDelta;
    }

    internal void Place(Level level, GameVertex.Edge flagDelta)
    {
        var lvlInstance = level.levelInstance;

        // Adding in line?
        if(lvlInstance.grid[pos.y,pos.x].lines == GameVertex.Edge.none)
        {
            gameObject.SetActive(true);
        }

        // TODO: Fix this for the new format
        // lvlInstance.grid[y, x].flags |= flagDelta;

    }



}
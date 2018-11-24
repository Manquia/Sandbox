using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInstance
{
    internal GameVertex[,] grid;

    // For traversals
    static private char gridTraversalIndex_P = (char)0;
    static internal char GetNewTraversalIndex(){ return gridTraversalIndex_P++; }
    static internal char[,] gridIndex;

    private int orderIndex = 0;
    public int GetOrderindex()
    {
        return ++orderIndex;
    }
}
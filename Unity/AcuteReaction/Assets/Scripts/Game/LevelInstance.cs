using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInstance
{
    internal GameVertex[,] grid;

    private int orderIndex = 0;
    public int GetOrderindex()
    {
        return ++orderIndex;
    }
}
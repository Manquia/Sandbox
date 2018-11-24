using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct LineCommand
{
    internal GameObject[] gos;
    internal Command cmd;
    internal GameVertex.Lines flags;

    public enum Command
    {
        None = 0,
        Place = 1,
        Remove = 2,
        Move = 3,
    }

    /*
    [StructLayout(LayoutKind.Explicit)]
    public struct Data
    {
        public struct Place
        {
        }
        public struct Destroy
        {
        }
        public struct Move
        {
            public Vector2Int delta;
        }

        [FieldOffset(0)] public Place   place;
        [FieldOffset(0)] public Destroy destroy;
        [FieldOffset(0)] public Move    move;
    }
    Data data;
    */

    public Vector2Int moveDelta;
    public static LineCommand ConstructDefault()
    {
        LineCommand lh;
        lh.cmd = Command.None;
        lh.gos = null;
        lh.moveDelta = Vector2Int.zero;
        lh.flags = GameVertex.Lines.none;
        return lh;
    }

    public void Invert()
    {
        switch (cmd)
        {
            case Command.None:
                Debug.LogWarning("Warning: Tried to invet a None command!");
                break;
            case Command.Place:
                cmd = Command.Remove;
                break;
            case Command.Remove:
                cmd = Command.Place;
                break;
            case Command.Move:
                Debug.LogWarning("Warning: inverting move command not yet implimented!");
                // @ TODO
                break;
        }
    }
}

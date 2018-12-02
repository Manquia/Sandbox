using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Events

public struct InitEvent
{
    public Vector2Int pos;
    public int dir;
}

#endregion

public enum ARLineMachineStages
{
    MagneticSet,
    Gravity,
    TransformSlot,
    TransformMove,
    ConveyerSlot,
    ConveyerMove,
}

public interface ARLineMachineInterface
{
    void Init(InitEvent e);
    void Destroy();
    void DoStageStep(ARLineMachineStages stage);
}

public class ARLineMachine : MonoBehaviour
{
    public struct Slot
    {
        public Vector2Int pos;
        public Space space;

        [System.Flags]
        public enum Space : short
        {
            None = 0,

            // full
            full = q_0 | q_1 | q_2 | q_3, 

            // halfs
            h_0 = q_0 | q_1, // bot half
            h_1 = q_1 | q_2, // right half
            h_2 = q_2 | q_3, // top half
            h_3 = q_3 | q_0, // left half

            // quarters
            q_0 = a_0_0 | a_0_1 | a_0_2 | a_0_3,
            q_1 = a_1_1 | a_1_1 | a_1_2 | a_1_3,
            q_2 = a_2_0 | a_2_1 | a_2_2 | a_2_3,
            q_3 = a_3_0 | a_3_1 | a_3_2 | a_3_3,

            // atoms
            a_0_0 = 0x1,
            a_0_1 = 0x2,
            a_0_2 = 0x4,
            a_0_3 = 0x8,
            a_1_0 = 0x01,
            a_1_1 = 0x02,
            a_1_2 = 0x04,
            a_1_3 = 0x08,
            a_2_0 = 0x001,
            a_2_1 = 0x002,
            a_2_2 = 0x004,
            a_2_3 = 0x008,
            a_3_0 = 0x0001,
            a_3_1 = 0x0002,
            a_3_2 = 0x0004,
            a_3_3 = 0x0008,
        }
    }
    // Data
    public Level level;
    public List<ARLineMachine> cluster;
    public Vector2Int pos;
    public GameVertex.Direction dir;
    public GameVertex.Edge.Type type;

    // Data helpers (grid)
    public Vector2Int DirOffset { get {return ARUtil.DirToOffset(dir);} }
    public Vector2Int Start { get { return pos; } }
    public Vector2Int End { get { return pos + DirOffset;  } }
    // Data helpers (world)
    public Vector3 WorldStart { get { Vector2Int start = Start; return new Vector3(start.x - level.width / 2, 0, start.y - level.height / 2); } }
    public Vector3 WorldEnd   { get { Vector2Int end = End;     return new Vector3(end.x - level.width / 2, 0, end.y - level.height / 2); } }

    public Slot In0()
    {
        Slot s;
        switch (type)
        {
            case GameVertex.Edge.Type.solid:
                {
                    s.pos = DirAffectPosIn(dir,pos);
                    s.space = Slot.Space.full;
                }
                break;


            case GameVertex.Edge.Type.None:
            default:
                s.pos = pos;
                s.space = Slot.Space.full;
                Debug.LogError("Error, unhandled type used to get Input in In0!");
                break;
        }


        return s;
    }

    public Slot In1()
    {
        Slot s = In0();
        s = DirMirrorSlot(dir, s);
        return s;
    }


    // dir 
    //  7  6  5
    //    \|/
    //  0--+--4
    //    /|\
    //  1  2  3
    public static Vector2Int DirAffectPosIn(GameVertex.Direction dir, Vector2Int in0)
    {
        switch (dir)
        {
            case GameVertex.Direction.W: in0.x -= 1; break;
            case GameVertex.Direction.S: in0.y -= 1; break;
            default: break;
        }
        return in0;
    }

    public static Slot DirMirrorSlot(GameVertex.Direction dir, Slot s)
    {
        switch (dir)
        {
            case GameVertex.Direction.W: case GameVertex.Direction.E: s.pos.y -= 1; break;
            case GameVertex.Direction.S: case GameVertex.Direction.N: s.pos.x -= 1; break;
            default: break;
        }

        s.space = s.space ^ Slot.Space.full;
        return s;
    }
}

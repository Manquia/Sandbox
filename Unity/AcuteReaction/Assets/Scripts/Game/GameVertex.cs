using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameVertex
{               
    [System.Flags]
    public enum Direction : byte
    {
        // Directions
        None = 0,

        W = 1,
        SW = 2,
        S = 4,
        SE = 8,
        E = 16,
        NE = 32,
        N = 64,
        NW = 128,

        All = W | SW | S | SE | E | NE | N | NW,
    }
    public struct Edge
    {
        [System.Flags]
        public enum Type : byte
        {
            None,
            solid,          // places a solid object with friction
            conveyer,       // directional
            transform,      // directional, Ordered
            magnet,         // directional? "<->" = levatate/ice, "->" and "<-" spring? Bounce?
            weld,
            laser,          // destroys anything it touches to create special shapes (grinder)
            ice,            // objects untill stopping, but once stopped they remain still
            spring,         // directional + magnitude ( "<-" = 3up,1 left, "->" = 3up, 1 right, "<->"= 6 up

            InboundConveyer, // Moves along the Z axis into the screen
            OutboutConveyer, // Move along the Z axis out of the screen
        }

        // Data
        public Direction dirs;       
        public Type edgeW;
        public Type edgeSW;
        public Type edgeS;
        public Type edgeSE;
        public Type edgeE;
        public Type edgeNE;
        public Type edgeN;
        public Type edgeNW;


        // Helpers
        internal static Edge none {
        get {
                Edge f;
                f.edgeW  = Type.None;
                f.edgeSW = Type.None;
                f.edgeS  = Type.None;
                f.edgeSE = Type.None;
                f.edgeE  = Type.None;
                f.edgeNE = Type.None;
                f.edgeN  = Type.None;
                f.edgeNW = Type.None;
                f.dirs   = Direction.None;
                return f;
            }
        }

        // @PERF, use 8 byte int, union?
        public static bool operator==(Edge f0, Edge f1)
        {
            return
                f0.edgeW  == f1.edgeW  &&
                f0.edgeSW == f1.edgeSW &&
                f0.edgeS  == f1.edgeS  &&
                f0.edgeSE == f1.edgeSE &&
                f0.edgeE  == f1.edgeE  &&
                f0.edgeNE == f1.edgeNE &&
                f0.edgeN  == f1.edgeN  &&
                f0.edgeNW == f1.edgeNW &&
                f0.dirs   == f1.dirs  ;
        }
        // @PERF, use 8 byte int, union?
        public static bool operator!=(Edge f0, Edge f1)
        {
            return
                f0.edgeW  != f1.edgeW  ||
                f0.edgeSW != f1.edgeSW ||
                f0.edgeS  != f1.edgeS  ||
                f0.edgeSE != f1.edgeSE ||
                f0.edgeE  != f1.edgeE  ||
                f0.edgeNE != f1.edgeNE ||
                f0.edgeN  != f1.edgeN  ||
                f0.edgeNW != f1.edgeNW ||
                f0.dirs   != f1.dirs  ;
        }
        
        public void Set(Direction dir, Type type)
        {
            dirs |= dir;
            switch (dir)
            {
                case Direction.W:   edgeW = type;       break;
                case Direction.SW: edgeSW = type;       break;
                case Direction.S:   edgeS = type;       break;
                case Direction.SE: edgeSE = type;       break;
                case Direction.E:   edgeE = type;       break;
                case Direction.NE: edgeNE = type;       break;
                case Direction.N:   edgeN = type;       break;
                case Direction.NW: edgeNW = type;       break;

                case Direction.None:
                case Direction.All:
                default: Debug.LogWarning("Warning Called Set on GameVertex.Line with a dir not set to a single direction"); break;
            }

        }
        public Direction AllToOne()
        {
            return dirs;
        }
        public Direction AllToOne(Direction mask)
        {
            return dirs & mask;
        }
    }
    public GameObject[] gos;
    public Edge lines;

    public int converyOrder; // @TODO for transform blocks
}


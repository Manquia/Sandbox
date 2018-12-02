using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameVertex
{               
    [System.Flags]
    public enum DirectionFlags : byte
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
    public enum Direction : byte
    {
        W  = 0,
        SW = 1,
        S  = 2,
        SE = 3,
        E  = 4,
        NE = 5,
        N  = 6,
        NW = 7,
    }
    public struct Edge
    {
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
        public DirectionFlags dirs;       
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
                f.dirs   = DirectionFlags.None;
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
        
        public void Set(DirectionFlags dir, Type type)
        {
            dirs |= dir;
            switch (dir)
            {
                case DirectionFlags.W:   edgeW = type;       break;
                case DirectionFlags.SW: edgeSW = type;       break;
                case DirectionFlags.S:   edgeS = type;       break;
                case DirectionFlags.SE: edgeSE = type;       break;
                case DirectionFlags.E:   edgeE = type;       break;
                case DirectionFlags.NE: edgeNE = type;       break;
                case DirectionFlags.N:   edgeN = type;       break;
                case DirectionFlags.NW: edgeNW = type;       break;

                case DirectionFlags.None:
                case DirectionFlags.All:
                default: Debug.LogWarning("Warning Called Set on GameVertex.Line with a dir not set to a single direction"); break;
            }

        }
        public DirectionFlags AllToOne()
        {
            return dirs;
        }
        public DirectionFlags AllToOne(DirectionFlags mask)
        {
            return dirs & mask;
        }
    }
    public GameObject[] gos;
    public Edge lines;

    public int converyOrder; // @TODO for transform blocks
}


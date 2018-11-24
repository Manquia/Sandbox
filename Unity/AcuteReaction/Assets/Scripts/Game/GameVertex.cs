using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameVertex
{

    [System.Flags]
    public enum EdgeType : byte
    {
        solid,
        conveyer,
        transform,
        magnet,
        weld,
        laser,
        ice,
        spring,
    }
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
    public struct Lines
    {
        public Direction solid;       // places a solid object with friction
        public Direction conveyer;    // directional
        public Direction transform;   // directional, Ordered
        public Direction magnet;      // directional? "<->" = levatate/ice, "->" and "<-" spring? Bounce?

        public Direction weld;
        public Direction laser;       // destroys anything it touches to create special shapes (grinder)
        public Direction ice;         // objects untill stopping, but once stopped they remain still
        public Direction spring;      // directional + magnitude ( "<-" = 3up,1 left, "->" = 3up, 1 right, "<->"= 6 up

        internal static Lines none {
        get {
                Lines f;
                f.solid = 0;
                f.conveyer = 0;
                f.transform = 0;
                f.magnet = 0;
                f.weld = 0;
                f.laser = 0;
                f.ice = 0;
                f.spring = 0;
                return f;
            }
        }

        // @PERF, use 8 byte int, union?
        public static bool operator==(Lines f0, Lines f1)
        {
            return
                f0.solid     == f1.solid &&
                f0.conveyer  == f1.conveyer &&
                f0.transform == f1.transform &&
                f0.magnet    == f1.magnet &&
                f0.weld      == f1.weld &&
                f0.laser     == f1.laser &&
                f0.ice       == f1.ice &&
                f0.spring    == f1.spring;
        }
        // @PERF, use 8 byte int, union?
        public static bool operator!=(Lines f0, Lines f1)
        {
            return
                f0.solid     != f1.solid ||
                f0.conveyer  != f1.conveyer ||
                f0.transform != f1.transform ||
                f0.magnet    != f1.magnet ||
                f0.weld      != f1.weld ||
                f0.laser     != f1.laser ||
                f0.ice       != f1.ice ||
                f0.spring    != f1.spring;
        }
        // @PERF, use 8 byte int, union?
        public static Lines operator~(Lines f)
        {
            f.solid     = ~f.solid    ;
            f.conveyer  = ~f.conveyer ;
            f.transform = ~f.transform;
            f.magnet    = ~f.magnet   ;
            f.weld      = ~f.weld     ;
            f.laser     = ~f.laser    ;
            f.ice       = ~f.ice      ;
            f.spring    = ~f.spring   ;
            return f;
        }
        // @PERF, use 8 byte int, union?
        public static Lines operator&(Lines f0, Lines f1)
        {
            f0.solid     &= f1.solid    ;
            f0.conveyer  &= f1.conveyer ;
            f0.transform &= f1.transform;
            f0.magnet    &= f1.magnet   ;
            f0.weld      &= f1.weld     ;
            f0.laser     &= f1.laser    ;
            f0.ice       &= f1.ice      ;
            f0.spring    &= f1.spring   ;
            return f0;
        }
        // @PERF, use 8 byte int, union?
        public static Lines operator|(Lines f0, Lines f1)
        {
            f0.solid     |= f1.solid    ;
            f0.conveyer  |= f1.conveyer ;
            f0.transform |= f1.transform;
            f0.magnet    |= f1.magnet   ;
            f0.weld      |= f1.weld     ;
            f0.laser     |= f1.laser    ;
            f0.ice       |= f1.ice      ;
            f0.spring    |= f1.spring   ;
            return f0;
        }

        public void AddToAll(Direction f)
        {
            solid     |= f;
            conveyer  |= f;
            transform |= f;
            magnet    |= f;
            weld      |= f;
            laser     |= f;
            ice       |= f;
            spring    |= f;
        }
        public Direction AllToOne()
        {
            Direction d = GameVertex.Direction.None;
            d |= solid    ;
            d |= conveyer ;
            d |= transform;
            d |= magnet   ;
            d |= weld     ;
            d |= laser    ;
            d |= ice      ;
            d |= spring   ;
            return d;
        }
        public Direction AllToOne(Direction mask)
        {
            Direction d = GameVertex.Direction.None;
            d |= solid;
            d |= conveyer;
            d |= transform;
            d |= magnet;
            d |= weld;
            d |= laser;
            d |= ice;
            d |= spring;
            return d & mask;
        }
    }
    public GameObject[] gos;
    public Lines flags;

    public int converyOrder; // @TODO for transform blocks
}


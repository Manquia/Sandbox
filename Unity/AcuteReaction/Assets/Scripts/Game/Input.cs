using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputData
{
    internal const int ID_MOUSE = -10;
    internal const int ID_GAMEPAD0 = -11;
    internal const int ID_GAMEPAD1 = -12;
    internal const int ID_GAMEPAD2 = -13;
    internal const int ID_GAMEPAD3 = -14;
    internal const int ID_INVALID = int.MaxValue;

    public InputData(Vector2 inputPosition, Phase inputPhase, int inputId)
    {
        phase = inputPhase;
        id = inputId;
        pos = inputPosition;
    }
    public InputData(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:      phase = Phase.Begin;    break;
            case TouchPhase.Moved:      phase = Phase.Update;    break;
            case TouchPhase.Stationary: phase = Phase.Update;   break;
            case TouchPhase.Ended:      phase = Phase.Ended;    break;
            case TouchPhase.Canceled:   phase = Phase.Canceled; break;
            default:                    phase = Phase.None;     break;
        }
        id = touch.fingerId;
        pos = touch.position;
    }
    public InputData(Vector2 inputPosition, KeyState state, int inputId)
    {
        if      (state.pressed())   phase = Phase.Begin;
        else if (state.down())      phase = Phase.Update;
        else if (state.released())  phase = Phase.Ended;
        else                        phase = Phase.None;

        pos = inputPosition;
        id = inputId;
    }

    public enum Phase
    {
        None,
        Begin,
        Update,
        Ended,
        Canceled,
    }

    internal Phase phase;
    internal int id;
    internal Vector2 pos;

    static public InputData Construct()
    {
        InputData inData;
        inData.id = ID_INVALID;
        inData.pos = Vector2.zero;
        inData.phase = Phase.None;
        return inData;
    }
}
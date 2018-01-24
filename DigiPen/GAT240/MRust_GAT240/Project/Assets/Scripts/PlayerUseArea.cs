using UnityEngine;
using System.Collections;
using System;

class PlayerUseEvent
{
    public Transform player;
    public bool BlockingAction;
}

struct UnBlockPlayerAction
{
}

public class PlayerUseArea : FFComponent {


    bool PlayerActionBlocked = false;
    bool PlayerWantsToUse = false;

    // Use this for initialization
    void Start ()
    {
        cursorDelaySeq = action.Sequence();
        FFMessage<UnBlockPlayerAction>.Connect(OnUnblockPlayerAction);
	}

    //FFMessage<UnblockPlayerAction>.SendToLocal(upa); // Will unlock the player
    private void OnUnblockPlayerAction(UnBlockPlayerAction e)
    {
        PlayerActionBlocked = false;
    }

    // Update is called once per frame
    void Update ()
    {
        // Is Button Pressed
        if(Input.GetMouseButtonDown(0)) // left mouse button
        {
            PlayerWantsToUse = true;
        }
        else
        {
            PlayerWantsToUse = false;
        }



        // Player action is Busy
        if (PlayerActionBlocked)
        {
            if (PlayerWantsToUse)
            {
            }
            else
            {
                SetCursor(BusySprite, Color.white);
            }
            return; // do not continue
        }

    }

    
    void OnTriggerStay(Collider other)
    {
        HandleInsideTrigger(other);
    }

    public Sprite BusySprite;
    public Sprite IdleSprite;
    public Sprite GrabSprite;
    public Sprite TooHeavySprite;
    public Sprite FlipSprite;

    void HandleInsideTrigger(Collider other)
    {
    }

    void SetCursor(Sprite sprite, Color color)
    {
    }


    FFAction.ActionSequence cursorDelaySeq;
    Sprite cursorDelaySprite;
    Color cursorDelayColor;
    void SetCursorDelayed(Sprite sprite, Color color, float delay = 0.5f)
    {
        cursorDelaySprite = sprite;
        cursorDelayColor = color;

        cursorDelaySeq.ClearSequence();
        cursorDelaySeq.Delay(delay);
        cursorDelaySeq.Sync();
        cursorDelaySeq.Call(SetCursorDelayedSeq);
    }
    void SetCursorDelayedSeq()
    {
        SetCursor(cursorDelaySprite, cursorDelayColor);
    }
}

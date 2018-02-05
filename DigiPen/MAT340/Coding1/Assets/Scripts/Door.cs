using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite ClosedSprite;
    public Sprite OpenSprite;

    public Sprite CarSprite;
    public Sprite NoCarSprite;

    public static Door PickedDoor = null;

    enum State
    {
        Open,
        Close,
    }

    State doorState = State.Close;
    public bool hasCar = false;
    public bool isPicked = false;
    
    public void PlaceCar()
    {
        hasCar = true;
    }

    public void RevealDoor()
    {
        var resultImage = transform.Find("ResultImage").GetComponent<UnityEngine.UI.Image>();

        if (hasCar)
        {
            resultImage.sprite = CarSprite;
        }
        else
        {
            resultImage.sprite = NoCarSprite;
        }

        // open door
        {
            var image = GetComponent<UnityEngine.UI.Image>();
            image.sprite = OpenSprite;
        }
    }
	
    public void PickDoor()
    {

        if(PickedDoor != null)
        {
            var oldDoor = PickedDoor.GetComponent<UnityEngine.UI.Image>();
            oldDoor.color = Color.white;
            isPicked = false;
        }
        var door = GetComponent<UnityEngine.UI.Image>();
        door.color = Color.green;
        PickedDoor = this;
        isPicked = true;


        var p2Controller = GameObject.Find("P2Controller").GetComponent<P2Controller>();
        if (p2Controller.state == P2Controller.State.Start)
        {
            p2Controller.ProgressP2ControllerState();
        }
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}

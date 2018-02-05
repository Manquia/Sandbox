using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Controller : MonoBehaviour
{
    public GameObject DoorPrefab;
    Transform doorsRoot;


    public int doorsToCreate = 3;
    public int carsToCreate = 1;
    public UnityEngine.UI.InputField inputDoorCount;
    public UnityEngine.UI.InputField inputCarCount;

    public UnityEngine.UI.Text instructionText;
    public UnityEngine.UI.Button actionButton;
    public UnityEngine.UI.Text actionButtonText;

    public enum State
    {
        Start,
        RevealOrSwap,
        Finish
    }
    public State state;

	// Use this for initialization
	void Start ()
    {
		
	}

    public void ProgressP2ControllerState()
    {
        switch (state)
        {
            case State.Start:
                RevealOneDoor();
                state = State.RevealOrSwap;
                break;
            case State.RevealOrSwap:
                state = State.Finish;
                break;
            case State.Finish:
                break;
        }
    }
    public void FinalizeChoice()
    {
        foreach (Transform door in doorsRoot)
        {
            var doorComp = door.GetComponent<Door>();
            doorComp.RevealDoor();
        }
        // goto finished
        ProgressP2ControllerState();
    }
    void RevealOneDoor()
    {
        List<Door> doorsToChooseFrom = new List<Door>();

        foreach(Transform door in doorsRoot)
        {
            var doorComp = door.GetComponent<Door>();
            if (doorComp.hasCar == false &&
                doorComp.isPicked == false)
            {
                doorsToChooseFrom.Add(doorComp);
            }
        }

        int doorToRevealIndex = UnityEngine.Random.Range(0, doorsToChooseFrom.Count);
        doorsToChooseFrom[doorToRevealIndex].RevealDoor();
    }

    // Update is called once per frame
    void Update ()
    {
        switch (state)
        {
            case State.Start:
                instructionText.text = "Click on a door to pick it";
                actionButton.gameObject.SetActive(false);
                break;
            case State.RevealOrSwap:
                instructionText.text = "Change selection?";
                actionButton.gameObject.SetActive(true);
                actionButtonText.text = "Finalize Choice";
                break;
            case State.Finish:
                actionButton.gameObject.SetActive(false);
                instructionText.text = "Thanks for playing";
                break;
        }
    }


    public void PlayOrRestart()
    {
        state = State.Start;
        GetInput();
        CreateDoors();


    }


    void GetInput()
    {
        if(inputDoorCount != null)
        {
            bool outputGood = int.TryParse(inputDoorCount.text, out doorsToCreate);
            if (outputGood == false)
                doorsToCreate = 3;
        }

        if(inputCarCount != null)
        {
            bool outputGood = int.TryParse(inputCarCount.text, out carsToCreate);
            if (outputGood == false)
                carsToCreate = 1;
        }
    }

    void CreateDoors()
    {
        // find Or Create DoorsRoot
        if (doorsRoot != null) Destroy(doorsRoot.gameObject); // destroy any old doors
        doorsRoot = transform.Find("DoorsRoot");
        if(doorsRoot != null) Destroy(doorsRoot.gameObject); // destroy any old doors
        doorsRoot = new GameObject("DoorsRoot").transform;
        doorsRoot.SetParent(transform, false);
        doorsRoot.localPosition = Vector3.zero;

        // Place cars randomly
        HashSet<int> doorsWithCars = new HashSet<int>();
        for(int i = 0; i < carsToCreate; ++i)
        {
            // every door has a car
            if (carsToCreate == doorsWithCars.Count)
                break;

            int randDoorIndex = UnityEngine.Random.Range(0, doorsToCreate);
            if (doorsWithCars.Contains(randDoorIndex) == false)
                doorsWithCars.Add(randDoorIndex);
            else
                --i; //  try again
        }
        
        // Calculate widths the door's placements
        var spaceForDoor = new Vector3(500.0f, 125.0f, 0.0f);
        float distBetweenDoors = spaceForDoor.x / (doorsToCreate - 1);
        float widthOffset = spaceForDoor.x / 2.0f;

        // Create new doors
        List<Door> doors = new List<Door>();
        for (int i = 0; i < doorsToCreate; ++i)
        {
            var newDoor = Instantiate(DoorPrefab);
            Transform newDoorTrans = newDoor.transform;

            newDoorTrans.SetParent(doorsRoot, false);
            newDoorTrans.localPosition = new Vector3(
                i * distBetweenDoors - (widthOffset), 0.0f, 0.0f);

            doors.Add(newDoor.GetComponent<Door>());
            
            // Listen to events?
        }

        foreach (var doorIndex in doorsWithCars)
            doors[doorIndex].PlaceCar();
    }


}

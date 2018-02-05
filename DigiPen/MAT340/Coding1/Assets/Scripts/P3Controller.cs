using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P3Controller : MonoBehaviour
{
    Transform doorsRoot;

    public int doorsToOpen = 1;
    public int doorsToCreate = 3;
    public int carsToCreate = 1;
    public int loopsToSimulate = 25;
    public UnityEngine.UI.InputField inputLoopCount;
    public UnityEngine.UI.InputField inputDoorsOpenedCount;
    public UnityEngine.UI.InputField inputDoorCount;
    public UnityEngine.UI.InputField inputCarCount;


    public UnityEngine.UI.Text output;
    // Use this for initialization
    void Start()
    {

    }
    


    public void Simulate()
    {
        GetInput();

        int winCounter = 0;
        for(int i = 0; i < loopsToSimulate; ++i)
        {
            bool result = RunSimulation();
            if (result)
                ++winCounter;
        }

        float winChance = (float)winCounter / (float)loopsToSimulate;

        output.text = winChance.ToString();
    }


    void GetInput()
    {
        if (inputDoorCount != null)
        {
            bool outputGood = int.TryParse(inputDoorCount.text, out doorsToCreate);
            if (outputGood == false)
            {
                doorsToCreate = 3;
                inputDoorCount.text = "3";
            }
            else
            {
                doorsToCreate = Mathf.Clamp(doorsToCreate, 3, 1000);
                inputDoorCount.text = doorsToCreate.ToString();
            }

        }

        if (inputCarCount != null)
        {
            int.TryParse(inputCarCount.text, out carsToCreate);
            carsToCreate = Mathf.Clamp(carsToCreate, 1, doorsToCreate - 2);
            inputCarCount.text = carsToCreate.ToString();
        }

        if (inputDoorsOpenedCount != null)
        {
            int.TryParse(inputDoorsOpenedCount.text, out doorsToOpen);
            doorsToOpen = Mathf.Clamp(doorsToOpen, 1, doorsToCreate - carsToCreate - 1);
            inputDoorsOpenedCount.text = doorsToOpen.ToString();
        }

        if (inputLoopCount != null)
        {
            bool outputGood = int.TryParse(inputLoopCount.text, out loopsToSimulate);
            if (outputGood == false)
            {
                inputLoopCount.text = "100";
                loopsToSimulate = 100;
            }
            else
            {
                loopsToSimulate = Mathf.Clamp(loopsToSimulate, 1, 1000);
                inputLoopCount.text = loopsToSimulate.ToString();

            }
        }
    }


    public class SimDoor
    {
        public bool hasCar = false;
        public bool picked = false;
    }
    bool RunSimulation()
    {
        // Place cars randomly
        HashSet<int> doorsWithCars = new HashSet<int>();
        for (int i = 0; i < carsToCreate; ++i)
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

        // Create new doors
        List<SimDoor> doors = new List<SimDoor>();
        for (int i = 0; i < doorsToCreate; ++i)
        {
            doors.Add(new SimDoor());
        }

        // place cars in doors
        foreach (var doorIndex in doorsWithCars)
            doors[doorIndex].hasCar = true;

        // Stage a door pick randomly
        SimDoor pickedDoor = doors[UnityEngine.Random.Range(0, doors.Count)];
        pickedDoor.picked = true;

        // Stage 2 open k doors
        {
            // randomly choose from those doors
            HashSet<int> chosenDoorsToReveal = new HashSet<int>();
            for (int i = 0; i < doorsToOpen; ++i)
            {
                int randDoorIndex = UnityEngine.Random.Range(0, doorsToCreate);
                if (doors[randDoorIndex].picked == false &&
                    doors[randDoorIndex].hasCar == false &&
                    chosenDoorsToReveal.Contains(randDoorIndex) == false)
                    chosenDoorsToReveal.Add(randDoorIndex);
                else
                    --i; //  try again
            }

            var indexesToReveal = new int [chosenDoorsToReveal.Count];
            chosenDoorsToReveal.CopyTo(indexesToReveal);
            Array.Sort<int>(indexesToReveal);
            Array.Reverse(indexesToReveal);

            foreach (var doorIndex in indexesToReveal)
                doors.RemoveAt(doorIndex);
        }

        // Stage 3 choose another door randomly
        {
            while(true)
            {
                int randomIndex = UnityEngine.Random.Range(0, doors.Count);
                if (doors[randomIndex].picked)
                    continue; // try again

                pickedDoor.picked = false;
                pickedDoor = doors[randomIndex];
                pickedDoor.picked = true;
                break;
            }
        }

        // stage 4 record results
        return pickedDoor.hasCar;
    }


}

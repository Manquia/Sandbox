using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P4Controller : MonoBehaviour
{
    
    public UnityEngine.UI.InputField inputdimensions;
    public UnityEngine.UI.InputField inputWalkLength;
    public UnityEngine.UI.InputField InputTrialCount;

    
    public UnityEngine.UI.Text outputText;


    int dimensions;
    int walkLength;
    int trialCount;

    public void RunP4()
    {
        GetInput();

        float[] distances = new float[trialCount];
        
        // run trials and record distances
        int countOfCollisions = 0;

        for (int trialIndex = 0; trialIndex < trialCount; ++trialIndex)
        {
            for (int walkIndex = 0; walkIndex < walkLength; ++walkIndex)
            {
                // d-demensional random walk
                int rw = 0;
            }
        }

        // show result
        outputText.text = "Seems like a challenging problem...";
    }


    void GetInput()
    {
        if (inputdimensions != null)
        {
            int.TryParse(inputdimensions.text, out dimensions);
            dimensions = Mathf.Clamp(dimensions, 2, 2048);
            inputdimensions.text = dimensions.ToString();
        }

        if (inputWalkLength != null)
        {
            int.TryParse(inputWalkLength.text, out walkLength);
            walkLength = Mathf.Clamp(walkLength, 5, 1000000);
            inputWalkLength.text = walkLength.ToString();
        }

        if (InputTrialCount != null)
        {
            int.TryParse(InputTrialCount.text, out trialCount);
            trialCount = Mathf.Clamp(trialCount, 500, 5000000);
            InputTrialCount.text = trialCount.ToString();
        }
    }
    

}

/*
            int rw1 =   1;
            int rw2 = - 1;
            Vector2Int rangeRW1 = Vector2Int.zero * 1;
            Vector2Int rangeRW2 = Vector2Int.zero * -1;
            for (int walkIndex = 0; walkIndex < walkLength; ++walkIndex)
            {
                {
                    // +-1 per walk
                    rw1 += UnityEngine.Random.value < 0.5f ? 1 : -1;
                    // see if outside of original range
                    rangeRW1.y = Mathf.Max(rangeRW1.y, rw1);
                    rangeRW1.x = Mathf.Min(rangeRW1.x, rw1);
                }
                {
                    // +-1 per walk
                    rw2 += UnityEngine.Random.value < 0.5f ? 1 : -1;
                    // see if outside of original range
                    rangeRW2.y = Mathf.Max(rangeRW2.y, rw2);
                    rangeRW2.x = Mathf.Min(rangeRW2.x, rw2);
                }

            }
            // if collisions
            if()
 */

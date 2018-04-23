using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P4Controller : MonoBehaviour
{
    
    public UnityEngine.UI.InputField inputdimensions;
    public UnityEngine.UI.InputField inputNumSteps;
    public UnityEngine.UI.InputField InputTrialCount;

    
    public UnityEngine.UI.Text outputText;


    int dimensions;
    int numSteps;
    int trialCount;

    public void RunP4()
    {
        GetInput();

        float[] distances = new float[trialCount];
        
        // run trials and record distances
        for (int trialIndex = 0; trialIndex < trialCount; ++trialIndex)
        {
            int[] dim = new int[dimensions];

            for(int step = 0; step < numSteps; ++step)
            {
                int selectedDim = UnityEngine.Random.Range(0, dimensions);
                int selectedDir = UnityEngine.Random.Range(0, 2) == 1 ? -1 : 1;

                dim[selectedDim] += selectedDir;
            }

            for (int dimIndex = 0; dimIndex < dim.Length; ++dimIndex)
            {
                distances[trialIndex] += dim[dimIndex] * dim[dimIndex];
            }
        }


        // get ave distance
        double aveDistance = 0.0;
        for (int distIndex = 0; distIndex < distances.Length; ++distIndex)
        {
            aveDistance += Mathf.Sqrt(distances[distIndex]);
        }
        aveDistance /= distances.Length;

        // show result
        outputText.text = aveDistance.ToString();
    }


    void GetInput()
    {
        if (inputdimensions != null)
        {
            int.TryParse(inputdimensions.text, out dimensions);
            dimensions = Mathf.Clamp(dimensions, 1, 1024);
            inputdimensions.text = dimensions.ToString();
        }

        if (inputNumSteps != null)
        {
            int.TryParse(inputNumSteps.text, out numSteps);
            numSteps = Mathf.Clamp(numSteps, 100, 25000);
            inputNumSteps.text = numSteps.ToString();
        }

        if (InputTrialCount != null)
        {
            int.TryParse(InputTrialCount.text, out trialCount);
            trialCount = Mathf.Clamp(trialCount, 2500, 25000);
            InputTrialCount.text = trialCount.ToString();
        }
    }
    

}

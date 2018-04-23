using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class P1Controller : MonoBehaviour
{
    public UnityEngine.UI.InputField inputNumPeople;
    public UnityEngine.UI.InputField inputNumTrials;
    public UnityEngine.UI.Text outputText;

    int numPeople;
    int numTrials;

    public void ButtonRunP1()
    {
        // input data
        {
            if (inputNumPeople != null)
            {
                bool outputGood = int.TryParse(inputNumPeople.text, out numPeople);
                if (outputGood == false)
                {
                    inputNumPeople.text = "25";
                    numPeople = 25;
                }
                else
                {
                    numPeople = Mathf.Clamp(numPeople, 2, 50000);
                    inputNumPeople.text = numPeople.ToString();
                }
            }

            if (inputNumTrials != null)
            {
                bool outputGood = int.TryParse(inputNumTrials.text, out numTrials);
                if (outputGood == false)
                {
                    inputNumTrials.text = "100";
                    numTrials = 100;
                }
                else
                {
                    numTrials = Mathf.Clamp(numTrials, 100, 5000);
                    inputNumTrials.text = numTrials.ToString();
                }
            }
        }
        
        RunP1();
    }
    

    void RunP1()
    {
        // Do trials

        int totalSame = 0;
        int totalSame_2 = 0;
        for (int trialIndex = 0; trialIndex < numTrials; ++trialIndex)
        {
            int indexMax = numPeople;

            // make lists
            List<int> list = new List<int>();
            for (int i = 0; i < indexMax; ++i) list.Add(i);
            List<int> listRandom = new List<int>(list);

            // randomize listRandom
            for(int times = 0; times < 10; ++times)
            {
                for (int i = 0; i < indexMax; ++i)
                {
                    int randomIndex = UnityEngine.Random.Range(0, indexMax);

                    int temp = listRandom[i];
                    listRandom[i] = listRandom[randomIndex];
                    listRandom[randomIndex] = temp;
                }
            }

            int countOfSame = 0;
            for (int i = 0; i < indexMax; ++i)
            {
                if (list[i] == listRandom[i])
                    ++countOfSame;
            }


            totalSame += countOfSame;
            totalSame_2 += countOfSame * countOfSame;
        }

        double expectedValue = totalSame / (double)(numTrials);

        // Take expected value and take the moment of it to find the variance ???
        // Mx(t) = M(t) = E[e^tx] = Int -inf -> inf e^tx * f(x) dx
        // M'(t) = Int -inf -> inf x * e^tx dx => M'(0) = E[X]
        // M''(t) = Int -inf -> inf x^2 * e^tx dx => M''(0) = E[X^2]
        //
        // variance = E[X^2] - E[X]

        double EX_2 = totalSame_2 / (double)(numTrials);
        double variance = EX_2 - (expectedValue * expectedValue);

        OutputResult(expectedValue, variance);
    }

    void OutputResult(double expectedValue, double variance)
    {
        string text = "";

        text += "Expected Value: " + expectedValue + "\n";
        text += "Variance: " + variance + "\n";

        outputText.text = text;
    }
}

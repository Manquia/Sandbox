using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P3Controller : MonoBehaviour
{

    public UnityEngine.UI.InputField inputGreenBalls;
    public UnityEngine.UI.InputField inputOrangeBalls;
    public UnityEngine.UI.InputField inputTrialCount;


    public UnityEngine.UI.Text output;


    int greenBalls = 3;
    int orangeBalls = 2;
    int trialCount = 2500;


    // Use this for initialization
    void Start()
    {

    }



    public void RunP3()
    {
        GetInput();

        // Count of green balls falls into interval
        //  [0, 0.05), [0.05, 0.1), [0.1, 0.15), · · · , [0.95, 1).
        int[] states = new int[20];

        long sampleCount = 0;
        for(int trialIndex = 0; trialIndex < trialCount; ++trialIndex)
        {
            int green = this.greenBalls;
            int orange = this.orangeBalls;
            for (int ballsInBag = green + orange; ballsInBag <= 1000; ++ballsInBag)
            {
                sampleCount += 1;

                if (GrabbedGreenBall(green, orange))
                {
                    green += 1;
                }
                else
                {
                    orange += 1;
                }

                float fractionOfGreen = (float)green / (float)(green + orange);
                int greenIndex = (int)Mathf.Floor(fractionOfGreen * 20.0f);
                states[greenIndex] += 1;
            }
        }


        // get averages
        double[] statesAve = new double[20];
        for(int i = 0; i < 20; ++i)
        {
            statesAve[i] = states[i] / (double)sampleCount;
        }

        PresentOutput(statesAve);
    }

    bool GrabbedGreenBall(int green, int orange)
    {
        return UnityEngine.Random.Range(0, green + orange) < green;
    }


    void GetInput()
    {
        if (inputGreenBalls != null)
        {
            bool outputGood = int.TryParse(inputGreenBalls.text, out greenBalls);
            if (outputGood == false)
            {
                greenBalls = 3;
                inputGreenBalls.text = "3";
            }
            else
            {
                greenBalls = Mathf.Clamp(greenBalls, 1, 999);
                inputGreenBalls.text = greenBalls.ToString();
            }
        }

        if (inputOrangeBalls != null)
        {
            bool outputGood = int.TryParse(inputOrangeBalls.text, out orangeBalls);
            if (outputGood == false)
            {
                orangeBalls = 2;
                inputOrangeBalls.text = orangeBalls.ToString();
            }
            else
            {
                orangeBalls = Mathf.Clamp(orangeBalls, 1, 1000 - greenBalls);
                inputOrangeBalls.text = orangeBalls.ToString();
            }
        }

        if (inputTrialCount != null)
        {
            bool outputGood = int.TryParse(inputTrialCount.text, out trialCount);
            if (outputGood == false)
            {
                trialCount = 2500;
                inputTrialCount.text = trialCount.ToString();
            }
            else
            {
                trialCount = Mathf.Clamp(trialCount, 1, 50000);
                inputTrialCount.text = trialCount.ToString();
            }
        }
    }
    

    void PresentOutput(double[] data)
    {
        string text = "{";
        
        int newlineEvery = 4;

        for (int i = 0; i < data.Length; ++i)
        {
            text += data[i].ToString("0.####") + (i < data.Length - 1 ? "," : "");
            if ((i + 1) % newlineEvery == 0 && i < data.Length - 2)
                text += "\n";
        }
        text += "}";


        output.text = text;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class P3Controller : MonoBehaviour
{
    public UnityEngine.UI.Text output;
    public UnityEngine.UI.InputField inputN;
    int N= 9;


    // Use this for initialization
    void Start()
    {
    }
    
    
    public void RunP3()
    {
        GetInput();

        const int loopCount = 100;

        int[] perm = null;
        int[] seqCounterLongest =  new int[loopCount];
        for (int i = 0; i < loopCount; ++i)
        {
            // Init perm!
            InitPerm(ref perm, N);

            // Get Longest increasing sequence
            int seqCounter = 0;
            int lastValue = perm[0];
            for(int s = 1; s < perm.Length; ++s)
            {
                if(lastValue == perm[s] - 1)
                {
                    ++seqCounter;
                    seqCounterLongest[i] = Mathf.Max(seqCounterLongest[i], seqCounter);
                }
                else
                {
                    seqCounter = 0;
                }

                lastValue = perm[s];
            }
        }


        // get average
        int sum = 0;
        double averageLength;
        {
            foreach (var counter in seqCounterLongest)
            {
                sum += counter;
            }
            averageLength = sum / (double)seqCounterLongest.Length;

        }


        // output text
        string outText = "";
        outText += "Average length of sequence: " + averageLength + "\n";
        outText += "Sum of lengths of each sequence: " + sum + "\n";

        output.text = outText;
    }
    void InitPerm(ref int[] perm, int length)
    {
        if(perm == null)
            perm = new int[length];

        for (int i = 0; i < length; ++i) perm[i] = i;
        for (int i = 0; i < length; ++i)
        {
            int randomIndex = UnityEngine.Random.Range(0, length);
            int temp = perm[i];
            perm[i] = perm[randomIndex];
            perm[randomIndex] = temp;
        }
    }


    void GetInput()
    {

        if (inputN != null)
        {
            bool outputGood = int.TryParse(inputN.text, out N);
            if (outputGood == false)
            {
                N = 9;
                inputN.text = "9";
            }
            else
            {
                N = Mathf.Clamp(N, 2, 50000);
                inputN.text = N.ToString();
            }
        }


    }
    
}

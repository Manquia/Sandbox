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
    int N;


    // Use this for initialization
    void Start()
    {
    }
    

    // I THINK I AM DOING THIS ALL WRONG!!!
    public void RunP3()
    {
        GetInput();

        const int loopCount = 100;

        int[] perm = null;

        int seqCounterLongest = 0;
        int[] longestPerm = null;
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
                    seqCounterLongest = Mathf.Max(seqCounterLongest, seqCounter);
                }
                else
                {
                    seqCounter = 0;
                }

                lastValue = perm[s];
            }
        }



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
                inputN.text = "9";
                N = 9;
            }
            else
            {
                N = Mathf.Clamp(N, 2, 50000);
                inputN.text = N.ToString();
            }
        }


    }
    
}

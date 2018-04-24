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
        
        // run trials and record distances
        int totalCountOfCollisions = 0;
        
        int[] rw1Cur = new int[dimensions];
        int[] rw1Start = new int[dimensions];
        for (int i = 0; i < rw1Start.Length; ++i) rw1Start[i] = 1;

        int[] rw2Cur = new int[dimensions];
        int[] rw2Start = new int[dimensions];
        for (int i = 0; i < rw2Start.Length; ++i) rw2Start[i] = -1;

        int[][] rw1 = new int[walkLength + 1][];
        for (int i = 0; i < rw1.Length; ++i) rw1[i] = new int[dimensions];

        // copy over the first starting pos of rw1Cur
        CopyVector(ref rw1Start, ref rw1[0]);
        

        for (int trialIndex = 0; trialIndex < trialCount; ++trialIndex)
        {
            // Init Rw1cur Rw1Cur
            CopyVector(ref rw1Start, ref rw1Cur);
            CopyVector(ref rw2Start, ref rw2Cur);

            // first walk
            for (int walkIndex = 0; walkIndex < walkLength; ++walkIndex)
            {
                int randDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
                int randDim = UnityEngine.Random.Range(0, dimensions);
                rw1Cur[randDim] += randDir;
                CopyVector(ref rw1Cur, ref rw1[walkIndex + 1]);
            }


            // second walk
            for (int walkIndex = 0; walkIndex < walkLength + 1; ++walkIndex)
            {
                bool IntersectionHappened = false;
                for (int i = 0; i < rw1.Length; ++i)
                {
                    if (CmpVector(ref rw2Cur, ref rw1[i]))
                    {
                        ++totalCountOfCollisions;
                        IntersectionHappened = true;
                        break;
                    }
                }

                if (IntersectionHappened)
                    break;


                int randDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
                int randDim = UnityEngine.Random.Range(0, dimensions);
                rw2Cur[randDim] += randDir;
            }
        }
        double aveCountOfCollisions = totalCountOfCollisions / (double)trialCount;
        double exponent = (Math.Log(trialCount) - Math.Log(trialCount - totalCountOfCollisions)) / Math.Log(walkLength);

        string outText = "";
        outText += "Total Collisions: " + totalCountOfCollisions + "\n";
        outText += "Average Collisions: " + aveCountOfCollisions + "\n";
        outText += "Exponent: " + exponent + "\n";

        // show result
        outputText.text = outText;
    }

    public static void CopyVector(ref int[] from, ref int[] to)
    {
        Debug.Assert(from.Length == to.Length);
        for (int i = 0; i < from.Length; ++i)
            to[i] = from[i];
    }
    public static bool CmpVector(ref int[] v1, ref int[] v2)
    {
        Debug.Assert(v1.Length == v2.Length);
        int i = 0;
        for (i = 0; i < v1.Length; ++i)
        {
            if (v1[i] != v2[i])
                break;
        }

        if (i == v1.Length)
            return true;
        else
            return false;
    }


    void GetInput()
    {
        if (inputdimensions != null)
        {
            if(int.TryParse(inputdimensions.text, out dimensions))
            {
                dimensions = Mathf.Clamp(dimensions, 1, 256);
                inputdimensions.text = dimensions.ToString();
            }
            else
            {
                dimensions = 2;
                inputdimensions.text = dimensions.ToString();
            }
        }

        if (inputWalkLength != null)
        {
            if (int.TryParse(inputWalkLength.text, out walkLength))
            {
                walkLength = Mathf.Clamp(walkLength, 1, 1000000);
                inputWalkLength.text = walkLength.ToString();
            }
            else
            {
                walkLength = 25;
                inputWalkLength.text = walkLength.ToString();
            }
        }

        if (InputTrialCount != null)
        {
            if(int.TryParse(InputTrialCount.text, out trialCount))
            {
                trialCount = Mathf.Clamp(trialCount, 1, 5000000);
                InputTrialCount.text = trialCount.ToString();
            }
            else
            {
                trialCount = 100;
                InputTrialCount.text = trialCount.ToString();
            }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class P1Controller : MonoBehaviour
{
    public GameObject DebugPrefabPoint;
    public UnityEngine.UI.Text outputText;
    public UnityEngine.UI.InputField inputPointCount;
    int pointCount = 25000;

    public Transform debugPtOffSet;

    public void ButtonRunP1()
    {
        // input data
        {
            if (inputPointCount != null)
            {
                bool outputGood = int.TryParse(inputPointCount.text, out pointCount);
                if (outputGood == false)
                {
                    inputPointCount.text = "5";
                    pointCount = 5;
                }
                else
                {
                    pointCount = Mathf.Clamp(pointCount, 50, 50000);
                    inputPointCount.text = pointCount.ToString();
                }
            }
        }

        WhipeDebugPointsClean();
        RunP1();
    }

    void RunP1()
    {
        var ptCount = this.pointCount;
        Vector2[] points = new Vector2[ptCount];
        //float sqrt_2 = Mathf.Sqrt(2.0f);
        
        // Generate random Points
        {
            Vector2 value; value.x = 0; value.y = 0;
            for (int i = 0; i < ptCount; ++i)
            {
                // Creates a distribution of  P(x,y uniform distrubution in unit circle of radius sqrt_2 | x,y inside square)
                //do
                //{
                //    float randAngle = UnityEngine.Random.value * Mathf.PI * 2;
                //    float randMag = UnityEngine.Random.value * sqrt_2;
                //    value.x = Mathf.Cos(randAngle) * randMag;
                //    value.y = Mathf.Sin(randAngle) * randMag;
                //
                //    // If not inside the square try again
                //} while (value.x < -1.0f || value.x > 1.0f || value.y < -1.0f || value.y > 1.0f);

                value.x = UnityEngine.Random.value;
                value.y = UnityEngine.Random.value;

                var debugPt = GetOrMakeDebugPoint();
                debugPt.localPosition = value;
                points[i] = value;
            }
        }
        
        // Calculate pi by the points inside and outside the points
        int pointsInsideCircle = 0;
        {
            for(int i = 0; i < ptCount; ++i)
            {
                if(points[i].sqrMagnitude <= 1.0f)
                {
                    ++pointsInsideCircle;
                }
            }
        }
        
        OutputResult(pointsInsideCircle, ptCount - pointsInsideCircle);
    }

    void OutputResult(int ptsInCircle, int ptsOutsideCircle)
    {
        string text = "";

        // Total Points
        text += "Random Points: " + (ptsInCircle + ptsOutsideCircle) + "\n";

        // Points Inside Circle
        text += "Points Inside Circle: " + ptsInCircle + "\n";
        // points Outside Circle
        text += "Points Outside Circle: " + ptsOutsideCircle + "\n";
        // Percent Inside Circle
        text += "Percent Inside Circle: " + (100.0 * ((double) ptsInCircle / (double)(ptsInCircle + ptsOutsideCircle))).ToString("#.##") + "%\n";
        // PI Estimation
        text += "PI estimation: " + (4.0 *((double)ptsInCircle / (double)(ptsOutsideCircle + ptsInCircle))).ToString("#.####") + "\n";

        outputText.text = text;
    }


    void PopulateMatrix(float[,] matrix, int dimensions, float winProb)
    {
        matrix[0, 0] = 1.0f;
        for (int i = 1; i < dimensions - 1; ++i)
        {
            matrix[i, i - 1] = 1.0f - winProb;
            matrix[i, i + 1] = winProb;
        }
        matrix[dimensions - 1, dimensions - 1] = 1.0f;
    }
    Stack<Transform> usedDebugPoints = new Stack<Transform>();
    Stack<Transform> inactiveDebugPoints = new Stack<Transform>();

    Transform GetOrMakeDebugPoint()
    {
        if(inactiveDebugPoints.Count > 0)
        {
            var debugPt = inactiveDebugPoints.Pop();
            usedDebugPoints.Push(debugPt);
            debugPt.gameObject.SetActive(true);
            return debugPt;
        }
        else
        {
            var debugPt = MakeDebugPoint();
            usedDebugPoints.Push(debugPt);
            return debugPt;
        }
    }
    void WhipeDebugPointsClean()
    {
        while(usedDebugPoints.Count > 0)
        {
            inactiveDebugPoints.Push(usedDebugPoints.Pop());
        }

        foreach(var pt in inactiveDebugPoints)
        {
            pt.gameObject.SetActive(false);
        }
    }
    Transform MakeDebugPoint()
    {
        var debugPt = Instantiate(DebugPrefabPoint).transform;
        debugPt.SetParent(debugPtOffSet, false);
        return debugPt;
    }

}

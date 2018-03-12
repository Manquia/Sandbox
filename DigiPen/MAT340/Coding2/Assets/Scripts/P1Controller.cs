using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class P1Controller : MonoBehaviour
{
    public UnityEngine.UI.Text outputText;

    public UnityEngine.UI.InputField inputAmount;
    public UnityEngine.UI.InputField inputWinAmount;
    public UnityEngine.UI.InputField inputWinProb;
    public UnityEngine.UI.InputField inputMatchCount;

    int amount = 5;
    int winAmount = 10;
    float winProb = 0.5f;
    int matchCount = 10;


    public void ButtonRunP1()
    {
        // input data
        {
            if (inputAmount != null)
            {
                bool outputGood = int.TryParse(inputAmount.text, out amount);
                if (outputGood == false)
                {
                    inputAmount.text = "5";
                    amount = 5;
                }
                else
                {
                    amount = Mathf.Clamp(amount, 0, 500000);
                    inputAmount.text = amount.ToString();
                }
            }

            if (inputWinAmount != null)
            {
                bool outputGood = int.TryParse(inputWinAmount.text, out winAmount);
                if (outputGood == false)
                {
                    winAmount = Mathf.Clamp(winAmount, amount, 500000);
                    inputWinAmount.text = winAmount.ToString();
                }
                else
                {
                    winAmount = Mathf.Clamp(winAmount, amount, 500000);
                    inputWinAmount.text = winAmount.ToString();
                }
            }

            if (inputWinProb != null)
            {
                bool outputGood = float.TryParse(inputWinProb.text, out winProb);
                if (outputGood == false)
                {
                    inputWinProb.text = "5";
                    winProb = 5.0f;
                }
                else
                {
                    winProb = Mathf.Clamp(winProb, 0.0f, 1.0f);
                    inputWinProb.text = winProb.ToString();
                }
            }

            if (inputMatchCount != null)
            {
                bool outputGood = int.TryParse(inputMatchCount.text, out matchCount);
                if (outputGood == false)
                {
                    inputMatchCount.text = "5";
                    matchCount = 5;
                }
                else
                {
                    matchCount = Mathf.Clamp(matchCount, 1, 500000);
                    inputMatchCount.text = matchCount.ToString();
                }
            }

        }

        RunP1();
    }

    void RunP1()
    {
        int dim = winAmount + 1;
        float[,] matrix = new float[dim, dim];
        PopulateMatrix(matrix, dim, winProb);

        var output = MAT340.RaisePowerMatrix(matrix, dim, matchCount);


        OutputResult(output, dim, amount);
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

    void OutputResult(float [,] output, int dimensions, int startState)
    {
        string text = "";
        text += "{";

        int newlineEvery = (dimensions / 10) + 1;

        for(int i = 0; i < dimensions; ++i)
        {
            text += output[startState, i].ToString("0.###") + (i < dimensions - 1 ? "," : "");
            if (i % newlineEvery == 0 && i < dimensions -1)
                text += "\n";
        }
        text += "}";

        outputText.text = text;
    }
}

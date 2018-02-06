using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P4Controller : MonoBehaviour
{
    public int a = 10;
    public int b = 15;
    public float p = 0.5f;
    public int loopsToSimulate = 100;

    public UnityEngine.UI.InputField inputLoopCount;
    public UnityEngine.UI.InputField InputA;
    public UnityEngine.UI.InputField InputB;
    public UnityEngine.UI.InputField InputP;


    public UnityEngine.UI.Text outputWinCounter;
    public UnityEngine.UI.Text outputWinChance;
    

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

        double winChance = (double)winCounter / (double)loopsToSimulate;

        outputWinChance.text = winChance.ToString("0.00000");
        outputWinCounter.text = winCounter.ToString();
    }


    void GetInput()
    {
        if (InputA != null)
        {
            int.TryParse(InputA.text, out a);
            a = Mathf.Clamp(a, 1, 100000);
            InputA.text = a.ToString();
        }

        if (InputB != null)
        {
            int.TryParse(InputB.text, out b);
            b = Mathf.Clamp(b, a + 1, 100000);
            InputB.text = b.ToString();
        }

        if (InputP != null)
        {
            float.TryParse(InputP.text, out p);
            p = Mathf.Clamp(p, 0.000000f, 1.0f);
            InputP.text = p.ToString();
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
                loopsToSimulate = Mathf.Clamp(loopsToSimulate, 1, 10000);
                inputLoopCount.text = loopsToSimulate.ToString();

            }
        }
    }
    
    //
    // true -> reached  b
    // false -> reached 0
    //
    bool RunSimulation()
    {
        int curAmount = a;
        int goalAmount = b;
        float probability = p;

        while(curAmount > 0 && curAmount < goalAmount)
        {
            float roll = UnityEngine.Random.value;
            
            if(roll < probability) // win
            {
                ++curAmount;
            }
            else // lose
            {
                --curAmount;
            }
        }

        return curAmount == goalAmount;
    }


}

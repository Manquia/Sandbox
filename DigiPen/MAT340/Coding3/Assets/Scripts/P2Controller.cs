using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Controller : MonoBehaviour
{
    public UnityEngine.UI.InputField inputConsecutiveHeads;
    public UnityEngine.UI.InputField inputTrialCount;

    public UnityEngine.UI.Text outputText;

    int consecutiveHeads;
    int trialCount;

	// Use this for initialization
	void Start ()
    {
		
	}

    public void RunP2()
    {
        GetInput();


        // flip all the coins
        long[] tries = new long[trialCount];

        for (int tryIndex = 0; tryIndex < trialCount; ++tryIndex)
        {
            for (long heads = 0; heads < consecutiveHeads; ++heads)
            {
                ++tries[tryIndex];
                bool isHeads = Random.value > 0.5f;

                if (!isHeads)
                    heads = -1; // -1 negates the ++ above
            }
        }

        // get average!!
        long totalTries = 0;
        for(int i = 0; i < trialCount; ++i)
        {
            totalTries += tries[i];
        }

        double averageTries = (double)totalTries / (double)trialCount;
        outputText.text = averageTries.ToString("0.#####");


    }

    void GetInput()
    {
        if (inputConsecutiveHeads != null)
        {
            bool outputGood = int.TryParse(inputConsecutiveHeads.text, out consecutiveHeads);
            if (outputGood == false)
            {
                inputConsecutiveHeads.text = "5";
                consecutiveHeads = 5;
            }
            else
            {
                consecutiveHeads = Mathf.Clamp(consecutiveHeads, 1, 5000);
                inputConsecutiveHeads.text = consecutiveHeads.ToString();
            }
        }

        if (inputTrialCount != null)
        {
            bool outputGood = int.TryParse(inputTrialCount.text, out trialCount);
            if (outputGood == false)
            {
                trialCount = Mathf.Clamp(trialCount, 1, 50000);
                inputTrialCount.text = inputTrialCount.ToString();
            }
            else
            {
                trialCount = Mathf.Clamp(trialCount, 1, 50000);
                inputTrialCount.text = trialCount.ToString();
            }
        }

    }
    
}

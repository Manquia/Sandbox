using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Controller : MonoBehaviour
{
    public UnityEngine.UI.InputField inputProb;

    public UnityEngine.UI.Text outputText;
    float probability = 0.5f;

	// Use this for initialization
	void Start ()
    {
		
	}

    public void RunP2()
    {
        GetInput();

        const int trialCount = 100;
        const float runCount = 10000;

        float mean = probability;
        float variance = (probability)*(1.0f - probability);
        float stdev = Mathf.Sqrt(variance);

        // 95% confidence interval 
        // 95% CI = [mean - (2*Var)/Sqrt(runs), mean + (2*Var)/Sqrt(runs)] , Delta = (2*Var)/Sqrt(runs)
        // 95% CI = [mean - Delta, mean + Delta]
        float CI95Delta = ((2.0f * stdev) / Mathf.Sqrt(runCount));
        Vector2 CI95 = new Vector2(mean - CI95Delta, mean + CI95Delta);

        int countWithinCI95 = 0;
        for(int trialIndex = 0; trialIndex < trialCount; ++trialIndex)
        {
            int successCount = 0;
            // run random variables Bernoulii
            for(int runIndex = 0; runIndex < runCount; ++runIndex)
            {
                if (UnityEngine.Random.value < probability)
                    ++successCount;
            }

            double successPercent = (double)successCount / (double)runCount;

            // within CI 95?
            if(CI95.x < successPercent && successPercent < CI95.y)
            {
                ++countWithinCI95;
            }
        }

        // output results
        {
            string text = "";
            text += "Times Within CI95: " + countWithinCI95 + "\n";
            text += "Percent of times within CI95: " + (100.0 *((double)countWithinCI95 / (double)trialCount)) + "%\n";
            outputText.text = text;
        }
    }

    void GetInput()
    {
        if (inputProb != null)
        {
            bool outputGood = float.TryParse(inputProb.text, out probability);
            if (outputGood == false)
            {
                inputProb.text = "0.5";
                probability = 0.5f;
            }
            else
            {
                probability = Mathf.Clamp(probability, 0.0f, 1.0f);
                inputProb.text = probability.ToString();
            }
        }
        
    }
    
}

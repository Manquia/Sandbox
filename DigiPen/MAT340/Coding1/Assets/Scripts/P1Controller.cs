using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P1Controller : MonoBehaviour
{
    public UnityEngine.UI.Text outputText;
    public UnityEngine.UI.InputField inputTrialCount;
    public UnityEngine.UI.InputField inputCouponCount;
    
    int trialCount;
    int couponCount;
    

    long couponsCollected;

    public void ButtonRunP1()
    {
        // reset member variables
        couponsCollected = 0;

        // input data
        {
            bool failedInput = false;
            failedInput |= !int.TryParse(inputTrialCount.text, out trialCount);
            failedInput |= !int.TryParse(inputCouponCount.text, out couponCount);
            if (failedInput) return;

            if(trialCount > 1000)
            {
                inputTrialCount.text = "1000";
                trialCount = 1000;
            }
            if(couponCount > 2500)
            {
                inputCouponCount.text = "2500";
                couponCount = 2500;
            }
        }

        RunP1();
    }

    void RunP1()
    {
        for(int trialIndex = 0; trialIndex < trialCount; ++trialIndex)
        {
            HashSet<long> set = new HashSet<long>();
            
            // haven't het collected all of the coupons
            while (set.Count != couponCount)
            {
                int couponId = UnityEngine.Random.Range(0, couponCount);
                ++couponsCollected;

                if (set.Contains(couponId) == false)
                    set.Add(couponId);
            }
        }


        OutputResult();
    }

    void OutputResult()
    {
        // Get average
        double averageCouponsCollected = (double)couponsCollected / (double)trialCount;
        outputText.text = averageCouponsCollected.ToString("{0.00}");
    }
}

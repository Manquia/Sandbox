using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Controller : MonoBehaviour
{
    public UnityEngine.UI.InputField inputN;
    public UnityEngine.UI.InputField inputColorCount;
    public UnityEngine.UI.InputField inputStepCount;
    
    public Gradient colorGradient;
    public Material torusMat;

    int n = 25;
    int colorCount = 5;
    int stepCount = 50;

	// Use this for initialization
	void Start ()
    {
        Init();
        Display();
	}

    int[,] array;

    public void RunP2()
    {
        int oldN = n;
        GetInput();
        if(n != oldN)
        {
            Init();
        }

        // do steps
        for(int stepIndex = 0; stepIndex < stepCount; ++stepIndex)
        {
            int randXIndex = UnityEngine.Random.Range(0, n);
            int randYIndex = UnityEngine.Random.Range(0, n);

            SampleNearbyRandomly(randXIndex, randYIndex);
        }

        Display();
    }
    void SampleNearbyRandomly(int x, int y)
    {
        bool right = Random.value < 0.5f;
        bool up = Random.value < 0.5f;

        int newXIndex = x;
        int newYIndex = y;

        newXIndex += right ?  1 + n : -1 + n;
        newXIndex = newXIndex % n;
        newYIndex += up ? 1 + n : -1 + n;
        newYIndex = newYIndex % n;

        array[x, y] = array[newXIndex, newYIndex];
    }
    public void InitAndDisplay()
    {
        GetInput();
        Init();
        Display();
    }
    public void Init()
    {
        array = new int[n, n];
        for (int y = 0; y < n; ++y)
        {
            for (int x = 0; x < n; ++x)
            {
                array[x, y] = UnityEngine.Random.Range(0, colorCount);
            }
        }
    }

    void GetInput()
    {
        if (inputN != null)
        {
            bool outputGood = int.TryParse(inputN.text, out n);
            if (outputGood == false)
            {
                inputN.text = "25";
                n = 25;
            }
            else
            {
                n = Mathf.Clamp(n, 3, 100);
                inputN.text = n.ToString();
            }
        }

        { 
            int.TryParse(inputColorCount.text, out colorCount);
            colorCount = Mathf.Clamp(colorCount, 2, (n * n) - 1);
            inputColorCount.text = colorCount.ToString();


            outputGood = int.TryParse(inputStepCount.text, out stepCount);
            if (outputGood == false)
            {
                stepCount = 50;
                inputStepCount.text = stepCount.ToString();
            }
            else
            {
                stepCount = Mathf.Clamp(stepCount, 1, 10000);
                inputStepCount.text = stepCount.ToString();
            }
        }
        
    }
    
    void Display()
    {
        Color[] colors = new Color[n*n];

        for (int y = 0; y < n; ++y)
        {
            for (int x = 0; x < n; ++x)
            {
                float mu = array[x, y] / (float)(colorCount - 1);
                colors[x + (n*y)] = colorGradient.Evaluate(mu);
            }
        }
        

        Texture2D texture = new Texture2D(n, n);
        texture.SetPixels(colors);

        //torusMat.SetTexture("_MainTex", texture);
        torusMat.mainTexture = texture;
        torusMat.SetTexture("_EmissionMap", texture);
    }
    
}

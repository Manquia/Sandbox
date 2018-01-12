using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCreator : MonoBehaviour, ICreator
{
    public int seed = 123;


    const int blockSize = 64;
    float[,] calcBlock = new float[blockSize, blockSize];
    public GameObject groundPrefab;


    void Start()
    {
        Create();
    }

    public void Create()
    {


        BuildRec(10.0f  /*height Var*/,
                1024     /*dim*/,
                16.0f    /*posOffset*/,
                0       /*xPos*/,
                0       /*zPos*/);
    }

    public Transform BuildRec(float heightVariance, int dim, float posOffset, float xPos, float zPos)
    {
        Debug.Assert(dim > 0);           // is greater than 0
                                                //Debug.Assert(((dimensions | 1) != 1));  // is event dimensions

        Transform root;
        float levelValue = UnityEngine.Random.Range(0, heightVariance / 2);
        if (dim == 1)// Base case
        {
            root = new GameObject().transform;
            root.position = new Vector3(xPos, levelValue, zPos);

            MakeObject(new Vector3(xPos + 0.5f, 0.0f, zPos + 0.5f), root);
            MakeObject(new Vector3(xPos - 0.5f, 0.0f, zPos + 0.5f), root);
            MakeObject(new Vector3(xPos + 0.5f, 0.0f, zPos - 0.5f), root);
            MakeObject(new Vector3(xPos - 0.5f, 0.0f, zPos - 0.5f), root);
        }
        else // Recursive
        {
            root = new GameObject().transform;
            int lowerDim = dim / 4;
            float lowerOffset = posOffset / 2;

            
            Transform r0 = BuildRec(heightVariance / 2, lowerDim, lowerOffset, xPos + lowerOffset, zPos + lowerOffset); r0.SetParent(root, false);
            Transform r1 = BuildRec(heightVariance / 2, lowerDim, lowerOffset, xPos - lowerOffset, zPos + lowerOffset); r1.SetParent(root, false);
            Transform r2 = BuildRec(heightVariance / 2, lowerDim, lowerOffset, xPos + lowerOffset, zPos - lowerOffset); r2.SetParent(root, false);
            Transform r3 = BuildRec(heightVariance / 2, lowerDim, lowerOffset, xPos - lowerOffset, zPos - lowerOffset); r3.SetParent(root, false);

            SpreadInfluence(r0, r1, r2, r3);
            SpreadInfluence(r1, r0, r2, r3);
            SpreadInfluence(r2, r0, r1, r3);
            SpreadInfluence(r3, r0, r1, r2);

            root.Translate(Vector3.up * levelValue);
            
        }

        return root;
    }

    public void MakeObject(Vector3 localPos, Transform parent)
    {
        Transform newObj = Instantiate(groundPrefab, parent, false).transform;
        newObj.localPosition = localPos;
    }
    public void SpreadInfluence(Transform source, Transform root1, Transform root2, Transform root3)
    {

    }

    public void Destroy()
    {


    }

    public void SetSeed(int seed)
    {
        this.seed = seed;
    }
    
	
	// Update is called once per frame
	void Update () {
		
	}
}

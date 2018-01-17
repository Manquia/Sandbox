using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCreator : MonoBehaviour, ICreator
{
    private bool gradientInitialized = false;
    public int seed = 123;
    [Range(16,512)]
    public int dimensions = 128;
    [Range(0.01f, 0.8f)]
    public float noiseScale = 0.5f;
    [Range(1.0f, 50.0f)]
    public float heightDelta = 25.0f;
    public GameObject groundPrefab;

    public void SetSeed(int seed)
    {
        gradientInitialized = false;
        this.seed = seed;
    }

    void Start()
    {
        Create();
    }
    
    public void Create()
    {
        if (!gradientInitialized)
            CalculateGradient();

        GenerateGround(transform, dimensions, noiseScale);
    }

    void GenerateGround(Transform parent, int dimensions, float noiseScale)
    {
        Vector2 samplePos = Vector2.zero;
        Vector3 objPosition = Vector3.zero;
        for (int i = 0; i < dimensions * dimensions; ++i)
        {
            var go = Instantiate(groundPrefab);
            var trans = go.transform;

            samplePos.Set(i % dimensions, (i / dimensions));
            objPosition.Set(samplePos.x, Perlin(samplePos.x * noiseScale, samplePos.y * noiseScale) * heightDelta, samplePos.y);

            trans.SetParent(parent, false);
            trans.localPosition = objPosition;
        }
    }

    void CalculateGradient()
    {
        gradient = new Vector2[IYMAX, IXMAX];

        UnityEngine.Random.InitState(seed);

        // randomize vectors
        Vector2 value = Vector2.zero;
        for (int iy = 0; iy < IYMAX; ++iy)
        {
            for (int ix = 0; ix < IXMAX; ++ix)
            {
                value.Set(UnityEngine.Random.Range(-1.0f, 1.0f),
                          UnityEngine.Random.Range(-1.0f, 1.0f));
                value = value.normalized;

                gradient[iy, ix] = value;
            }
        }

        gradientInitialized = true; // seed has created gradient
    }

    public void Destroy()
    {
        throw new NotImplementedException();
    }

    const int IYMAX = 256;
    const int IXMAX = 256;
    Vector2 [,]gradient;
    
    // Computes the dot product of the distance and gradient vectors.
    float DotGridGradient(int ix, int iy, float x, float y)
    {
        // Compute the distance vector
        float dx = x - (float)ix;
        float dy = y - (float)iy;
 
        // Compute the dot-product
        return (dx* gradient[iy,ix].x + dy* gradient[iy,ix].y);
    }

    // Compute Perlin noise at coordinates x, y
    float Perlin(float x, float y)
    {
        // Determine grid cell coordinates
        int x0 = (int)(x);
        int x1 = x0 + 1;
        int y0 = (int)(y);
        int y1 = y0 + 1;

        // Determine interpolation weights
        // Could also use higher order polynomial/s-curve here
        float sx = x - (float)x0;
        float sy = y - (float)y0;

        // Interpolate between grid point gradients
        float n0, n1, ix0, ix1, value;
        n0 = DotGridGradient(x0, y0, x, y);
        n1 = DotGridGradient(x1, y0, x, y);
        ix0 = Mathf.Lerp(n0, n1, sx);
        n0 = DotGridGradient(x0, y1, x, y);
        n1 = DotGridGradient(x1, y1, x, y);
        ix1 = Mathf.Lerp(n0, n1, sx);

        value = Mathf.Lerp(ix0, ix1, sy);

        return value;
    }



// First attempt to make this work. Not really that possible in any reasonable amount of time.
/*

public void Create()
{
    Transform root = BuildRec(10.0f,
            1024,
            16.0f,
            0,
            0);
    SmoothGround(root);
}

private Transform BuildRec(float heightVariance, int dim, float posOffset, float xPos, float zPos)
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

private void SmoothGround(Transform root)
{
    if (root.childCount != 4) // base case
        return;

    for (int i = 0; i < 4; ++i)
    {
        var i1 = i;
        var i2 = (i + 1) % 4;
        var i3 = (i + 2) % 4;
        var i4 = (i + 3) % 4;

        SpreadInfluence(
            root.GetChild(i1),
            root.GetChild(i2),
            root.GetChild(i3),
            root.GetChild(i4));
    }
}
public void SpreadInfluence(Transform source, Transform root1, Transform root2, Transform root3)
{
    SmoothGround(source);

    float d1 = (source.position - root1.position).sqrMagnitude;
    float d2 = (source.position - root2.position).sqrMagnitude;
    float d3 = (source.position - root3.position).sqrMagnitude;


    // itterator over all chuncks to change them
}

*/

}

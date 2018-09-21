using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct WarpRegionSampleData
{
    public Vector3 center;
    public float radius;
}
interface WarpRegion
{
    WarpRegionSampleData Sample(Transform sampleTransform);
    bool Negative();
}

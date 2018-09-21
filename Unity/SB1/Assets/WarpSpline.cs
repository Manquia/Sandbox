using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FFPath))]
public class WarpSpline : MonoBehaviour, WarpRegion {

    FFPath path;
    public bool negative;
    public float radius = 0.7f;

    public bool Negative()
    {
        return negative;
    }

    public WarpRegionSampleData Sample(Transform sampleTransform)
    {
        WarpRegionSampleData d;
        float f;

        d.radius = radius;
        d.center = path.NearestPointAlongPath(sampleTransform.position, out f);
        return d;
    }

    // Use this for initialization
    void Start ()
    {
        path = GetComponent<FFPath>();
	}
	
	// Update is called once per frame
	void Update () {

		
	}
}

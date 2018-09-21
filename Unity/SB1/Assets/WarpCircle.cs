using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class WarpCircle : MonoBehaviour, WarpRegion
{
    CircleCollider2D col;
    public bool negative = false;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
    }

    public WarpRegionSampleData Sample(Transform sampleTransform)
    {
        var scale = transform.localScale;
        WarpRegionSampleData d;
        d.radius = col.radius * Mathf.Max(scale.x, scale.y);
        d.center = transform.position;
        return d;
    }

    public bool Negative()
    {
        return negative;
    }
}

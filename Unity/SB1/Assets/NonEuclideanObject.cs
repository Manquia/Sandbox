using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonEuclideanObject : MonoBehaviour {

    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;

    public Rigidbody body;

	// Use this for initialization
	void Start ()
    {
        pos = transform.position;
        scale = transform.localScale;
        rot = transform.rotation;
	}

    private void FixedUpdate()
    {
        NonEuclideanGrid.UpdatePhysics(this);
    }
}

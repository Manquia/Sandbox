using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {


    public Vector3 axisNormal = Vector3.up;
    public float speed = 2.5f;
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        var rigid = GetComponent<Rigidbody>();
        rigid.angularVelocity = axisNormal.normalized * speed;
	}
}

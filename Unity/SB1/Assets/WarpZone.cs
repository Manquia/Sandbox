using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpZone : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        radius = GetComponent<CircleCollider2D>().radius * transform.localScale.magnitude;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public float radius;
}

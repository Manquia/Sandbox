using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Breakable : MonoBehaviour {

    public GameObject droppedObjPrefab;
    public GameObject brokenObjPrefab;

    // effects should clean themself up!
    public GameObject[] breakEffects;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            gameObject.SetActive(false);
            SpawnBrokenVersion();
        }
    }

    void SpawnBrokenVersion()
    {
        // Create broken version, then parent old version to
        // new broken obj
        var brokenVer = Instantiate(brokenObjPrefab);
        brokenVer.transform.position = transform.position;
        transform.SetParent(brokenVer.transform);

        // if we drop an object, spawn it and parent to broken version
        if (droppedObjPrefab != null)
        {
            var droppedObj = Instantiate(droppedObjPrefab);
            droppedObj.transform.SetParent(brokenVer.transform, false);
            droppedObj.transform.localPosition = Vector3.zero;
        }

        foreach (var obj in breakEffects)
        {
            var effect = Instantiate(obj);
            effect.transform.position = transform.position;
        }
    }
}

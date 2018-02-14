using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    public struct LookingAt
    {
        public Transform playerCamera;
    }
    public struct Looking
    {
        public Transform playerCamera;
    }
    public struct Use
    {
        public Transform playerCamera;
    }
    public struct LookingAway
    {
        public Transform playerCamera;

    }

    public float maxInteractDistance = 10.0f;
    public float interactionRadius = 0.5f;

    public Camera interactorCamera;

    public Transform interactedObject;
    public Transform prevInteractedObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;

        // If we hit something, send the use command
        if(LookRaycast(out hit))
        {
            // clicked on object
            if (Input.GetMouseButton(0))
            {
                Use u = new Use();
                FFMessageBoard<Use>.SendToLocalToAllConnected(u, hit.transform.gameObject);
            }

            // looking at new object
            if (hit.transform != interactedObject)
            {
                prevInteractedObject = interactedObject;
                interactedObject = hit.transform;

                // looking away from prev object
                if(prevInteractedObject != null)
                {
                    LookingAway lookAway = new LookingAway();
                    lookAway.playerCamera = interactorCamera.transform;
                    FFMessageBoard<LookingAway>.SendToLocalToAllConnected(lookAway, prevInteractedObject.gameObject);
                }

                LookingAt lookAt = new LookingAt();
                lookAt.playerCamera = interactorCamera.transform;
                FFMessageBoard<LookingAt>.SendToLocalToAllConnected(lookAt, hit.transform.gameObject);
                
            }

            Looking looking = new Looking();
            looking.playerCamera = interactorCamera.transform;
            FFMessageBoard<Looking>.SendToLocalToAllConnected(looking, hit.transform.gameObject);
        }
        else
        {
            // looking away
            if(interactedObject != null)
            {
                LookingAway lookAway = new LookingAway();
                lookAway.playerCamera = interactorCamera.transform;
                FFMessageBoard<LookingAway>.SendToLocalToAllConnected(lookAway, interactedObject.gameObject);
            }

            prevInteractedObject = interactedObject;
            interactedObject = null;
        }
    }

    bool LookRaycast(out RaycastHit hit)
    {
        return Physics.SphereCast(
            interactorCamera.transform.position,
            interactionRadius,
            interactorCamera.transform.forward,
            out hit, maxInteractDistance);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    List<WarpZone> warpEffects = new List<WarpZone>();

	// Use this for initialization
	void Start ()
    {
		
	}

    [System.Serializable]
    public class Movement
    {
        public Annal<Vector2> localMove = new Annal<Vector2>(16, Vector2.zero);

        public float Speed = 5.0f;
    }
    public Movement movement;
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float dt = Time.fixedDeltaTime;

        UpdateMovement(dt);
	}

    void UpdateMovement(float dt)
    {
        // get localMove
        {
            Vector2 localMove = Vector2.zero;

            // Key effects
            if (Input.GetKey(KeyCode.W)) localMove += Vector2.up;
            if (Input.GetKey(KeyCode.S)) localMove -= Vector2.up;
            if (Input.GetKey(KeyCode.D)) localMove += Vector2.right;
            if (Input.GetKey(KeyCode.A)) localMove -= Vector2.right;

            if(localMove != Vector2.zero)
            {
                movement.localMove.Record(localMove);
            }
            else if(movement.localMove != Vector2.zero)
            {
                movement.localMove.Record(localMove);
            }
        }


        Vector2 forward = movement.localMove.Recall(0);
        Vector2 right = Quaternion.AngleAxis(90.0f, Vector3.forward) * forward;

        Quaternion forwardRot = Quaternion.identity;
        Vector3 pos = transform.position;

        foreach (var we in warpEffects)
        {
            Vector2 vecToCenter = we.transform.position - pos;

            float distFromCenter = vecToCenter.magnitude;
            float mu = 1.0f - Mathf.Min((distFromCenter / we.radius) , 1.0f);

            //Debug.Log(mu);

            bool onRightSide = Vector2.Dot(right, -vecToCenter) > 0.0f ? true : false;
            Quaternion rotDelta = Quaternion.AngleAxis(onRightSide ? 90.0f * mu : -90.0f * mu, Vector3.forward);


            forward = rotDelta * forward;
            right = rotDelta * right;
        }

        // @TODO modify from warpZones


        // Move Character

        {
            Vector3 movementDelta = forward * dt * movement.Speed;
            transform.position += movementDelta;

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var warpZone = collision.GetComponent<WarpZone>();
        if (warpZone != null)
        {
            warpEffects.Add(warpZone);
            //Debug.Log("Have warp zone");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var warpZone = collision.GetComponent<WarpZone>();
        if (warpZone != null)
        {
            warpEffects.Remove(warpZone);
        }

    }
}

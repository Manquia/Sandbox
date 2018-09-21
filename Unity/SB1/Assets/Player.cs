using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//  EVENTS ARE BEING FORWARDED FROM CHILDEREN TO THIS OBJECT, BE AWARE!!
//
//
public class Player : MonoBehaviour {

    List<WarpRegion> bufferEffects = new List<WarpRegion>();

    public Transform unwarpedTransform;
    public Transform warpedTransform;

	// Use this for initialization
	void Start ()
    {
		
	}

    [System.Serializable]
    public class Movement
    {
        public Annal<Vector2> localMove = new Annal<Vector2>(16, Vector2.zero);

        public float Speed = 5.0f;
        public float resetCoef = 0.8f;
        public float warpCoef = 2.5f;
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
        var boxCollider = warpedTransform.GetComponent<BoxCollider2D>();
        var playerBoundingSphereRadius = boxCollider.size.magnitude * warpedTransform.localScale.magnitude;

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


        Vector3 forward = movement.localMove.Recall(0); 
        if (forward == Vector3.zero) forward = movement.localMove.Recall(1);
        if (forward == Vector3.zero) forward = Vector3.up;
        Vector3 right = Quaternion.AngleAxis(90.0f, Vector3.forward) * forward;

        // @TODO calculate warped speed to apply to unwarpedTransform
        float speed = movement.Speed;
        {

        }

        // Apply movement to unwarped Transform
        {
            Vector3 movementDelta = forward * dt * speed;
            unwarpedTransform.position += movementDelta;
            warpedTransform.position = unwarpedTransform.position;
        }

        Vector3 pos = unwarpedTransform.position;

        // Update the warped transform to show in the world
        foreach (var we in bufferEffects)
        {
            var sample = we.Sample(unwarpedTransform);
            sample.radius += playerBoundingSphereRadius;
            bool negative = we.Negative();

            // Adjust position
            Vector3 vecToCenter = sample.center - pos;
            float distFromCenter = vecToCenter.magnitude;
            float mu = 1.0f - Mathf.Clamp(distFromCenter / sample.radius, 0.0f, 1.0f);
            float amplitude = mu * sample.radius;

            if(mu > 1.0f || mu < 0.0f || amplitude > sample.radius)
            {
                Debug.Log("OHHH");
            }

            warpedTransform.position = pos + -vecToCenter * amplitude;
            pos = warpedTransform.position;

            // Adjust Scale
            const float scaleDelta = 0.5f;
            float scaleSign = negative ? -1.0f : 1.0f;
            float scaleDiff = scaleSign * mu * scaleDelta;
            warpedTransform.localScale = new Vector3(1.0f + scaleDiff, 1.0f + scaleDiff, 1.0f + scaleDiff);

            // Adjust rotation

            //Vector3 vecToCenter3DOffset = negative ? -Vector3.forward : Vector3.forward;
            //var angleSin = Mathf.Sin(mu * Mathf.PI * 0.5f);
            //Vector3 vecToCenter3D = (vecToCenter / sample.radius) + vecToCenter3DOffset * angleSin;
            //
            //var rotToAlongTangent = Quaternion.AngleAxis((negative ? -90.0f : 90.0f), vecToCenter3D);
            //var angleCos = Mathf.Cos(mu * Mathf.PI * 0.5f);
            //var tangentVec = Vector3.Cross(vecToCenter3D.normalized, right);
            //tangentVec = rotToAlongTangent * tangentVec;
            //var rot = Quaternion.AngleAxis(angleCos * Mathf.Rad2Deg, tangentVec);
            //warpedTransform.localRotation = rot;

            //Debug.Log("angleSin: " + angleSin);
            //Debug.Log("angleCos: " + angleCos);
            //Debug.DrawLine(warpedTransform.position, warpedTransform.position + tangentVec * 2.0f, Color.red);
            break; // @POIKUJDSF:LIFGKJSDFLKGJDSFL:KFDS:LKFJ:LKDSF HACK REMOVE ME
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var warpZone = collision.GetComponent<WarpRegion>();
        if (warpZone != null)
        {
            bufferEffects.Add(warpZone);
            //Debug.Log("Have warp zone");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var warpZone = collision.GetComponent<WarpRegion>();
        if (warpZone != null)
        {
            bufferEffects.Remove(warpZone);
        }

    }





    /*
     * // Gives the soft wall effect for a kinematic object.
     
    
        foreach (var we in bufferEffects)
        {
            Vector2 vecToCenter = we.transform.position - pos;

            float distFromCenter = vecToCenter.magnitude;
            float mu = 1.0f - Mathf.Min((distFromCenter / we.radius) , 1.0f);

            //Debug.Log(mu);

            // @ Change to a sin/cos implimentation for warping

            bool onRightSide = Vector2.Dot(right, -vecToCenter) > 0.0f ? true : false;
            Quaternion rotDelta = Quaternion.AngleAxis(onRightSide ? 90.0f * mu : -90.0f * mu, Vector3.forward);


            forward = rotDelta * forward;
            right = rotDelta * right;
        }


    */
}

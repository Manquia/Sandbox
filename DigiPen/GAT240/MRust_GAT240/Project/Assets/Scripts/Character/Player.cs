using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public CameraController cameraController;
    public DynamicAudioPlayer dynAudioPlayer;
    public IK_Snap ikSnap;
    public Rigidbody myBody;

    private FFRef<Vector3> velocityRef;
    private FFRef<Vector3> GetVelocityRef()
    {
        return velocityRef;
    }
    void SetVelocityRef(FFRef<Vector3> velocityRef)
    {
        Debug.Assert(velocityRef != null);
        this.velocityRef = velocityRef;

    }

    [System.Serializable]
    public class RopeConnection
    {
        public RopeController rope;

        public float pumpSpeed = 0.003f;
        public float pumpAcceleration = 0.5f;
        public float pumpResetSpeed = 0.05f;

        public float climbSpeed = 0.5f;
        public float rotateSpeed = 20.0f;
        public float leanSpeed = 0.9f;
        
        public float distUpRope;
        public float distPumpUp = 0.0f;
        
        public float maxPumpUpDist = 0.1f;
        public float maxPumpDownDist = 0.0f;

        // Offset Along Rope (vertical along rope)
        public float leftHandOffsetOnRope;
        public float rightHandOffsetOnRope;
        public float leftFootOffsetOnRope;
        public float rightFootOffsetOnRope;

        // Left/Right posiition OFfset
        public Vector3 leftHandOffset;
        public Vector3 rightHandOffset;
        public Vector3 leftFootOffset;
        public Vector3 rightFootOffset;

        // Rotations
        public Vector3 leftHandRot;
        public Vector3 rightHandRot;
        public Vector3 leftFootRot;
        public Vector3 rightFootRot;
        
        // Functional stuff
        public float angleOnRope; // in degrees
        public float distFromRope;
        public float rotationYaw;
        public float rotationPitch;
        
        // @TODO @Polish
        public float onRopeAngularVelocity; // <--- Rename...
        
    }
    public RopeConnection OnRope;

    [System.Serializable]
    public class GroundMovement
    {
        public float moveForce = 100.0f;
        public float jumpForce = 1000.0f;
        public float friction = 0.1f;
        public float maxSlopeAngle = 55.0f;

        public struct Details
        {
            public bool grounded_2;
            public bool grounded_1;
            public bool grounded;

            public bool touchGround_2;
            public bool touchGround_1;
            public bool touchGround;
        }
        public Details details;
    }
    public GroundMovement OnGround;

    [System.Serializable]
    public class JumpMovement
    {
        public float moveForce = 25.0f;
        public float revJumpVel = 1.0f;
        public float friction = 0.1f;
        public State state= State.GoingDown;
        [Flags]
        public enum State
        {
            None = 0,
            Launching = 1,
            GoingUp = 2,
            Peaking = 4,
            GoingDown = 8,
            Grounded = 16,
        }
    }
    public JumpMovement OnJump;

    bool grounded
    {
        get { return OnGround.details.touchGround; }
    }

    // @TODO make this into an annal


    //bool groundedThisFrame = false;

    // Use this for initialization
    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }
    void Start ()
    {
        // Set referece to 0 for initialization
        SetVelocityRef(new FFVar<Vector3>(Vector3.zero));
        if (dynAudioPlayer != null)
        {
            dynAudioPlayer.SetDynamicValue(new FFRef<float>(
                    () => GetVelocityRef().Getter().magnitude,
                    (v) => { }));
        }
        
        
    }
    void OnDestroy()
    {
        if (OnRope != null)
            DestroyOnRope();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        HandleCollision(collision);
    }
    private void HandleCollision(Collision col)
    {
        var contactPoints = col.contacts;
        Vector3 aveNormal = Vector3.zero;
        foreach (var pt in contactPoints)
        {
            aveNormal += pt.normal;
        }

        float normalDotUp = Vector3.Dot(aveNormal.normalized, Vector3.up);
        Debug.Log("Dot " + normalDotUp);


        // Shift history @TODO replace with ANNAL type
        OnGround.details.grounded_2 = OnGround.details.grounded_1;
        OnGround.details.grounded_1 = OnGround.details.grounded;
        OnGround.details.touchGround_2 = OnGround.details.touchGround_1;
        OnGround.details.touchGround_1 = OnGround.details.touchGround;

        // Ground not too steep to be considered ground
        if (normalDotUp <= OnGround.maxSlopeAngle)
        {
            OnGround.details.touchGround = true;
            OnGround.details.grounded = true;
        }
        else
        {
            OnGround.details.grounded = false;
        }


        // @TODO Make this detect the start of a fall
        // OR use raycasts to detect. Raycasts is probbly easier
        // fell?
        //if (OnGround.details.touchGround == false &&
        //    OnGround.details.grounded == false &&
        //    OnGround.details.grounded_1 == true)
        //{
        //
        //}

    }


    // Called right before physics, use this for dynamic Player actions
    void FixedUpdate()
    {
        OnGround.details.touchGround = false;

        UpdateInput();


        UpdateMoveGround(up, down, left, right, space, dt);
    }
    

    // Called right after physics, before rendering. Use for Kinimatic player actions
    void Update ()
    {
        UpdateInput();

        // @TODO have this work through an event or something
        //if (OnRope.rope != null)
        //    SetupOnRope();
        // @TODO add statemachine OR check on OnRope
        // @ROPE @FIX
        // Controls for movement on a rope
        // UpdateRope(up, down, left, right, space, modifier, dt);
    }

    // @CLEAN UP
    bool up;
    bool down;
    bool left;
    bool right;
    bool space;
    bool modifier;
    float dt;
    void UpdateInput()
    {
        up = Input.GetKey(KeyCode.W);
        down = Input.GetKey(KeyCode.S);
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);
        space = Input.GetKey(KeyCode.Space);
        modifier = Input.GetKey(KeyCode.LeftShift);
        dt = Time.deltaTime;
    }


    void UpdateMoveGround(bool up, bool down, bool left, bool right, bool space, float dt)
    {
        // movement in the Z axis
        if (up) myBody.AddForce(transform.forward * OnGround.moveForce);
        else if (down) myBody.AddForce(-transform.forward * OnGround.moveForce);

        // movement in the X axis
        if (right) myBody.AddForce(transform.right * OnGround.moveForce);
        else if (left) myBody.AddForce(-transform.right * OnGround.moveForce);

        // movement in the Y axis (jump)
        if (grounded && space)
        {
            OnGround.details.grounded = false;
            myBody.AddForce(transform.up * OnGround.jumpForce);
        }
        
        // Rotate based on mouse look
        if(cameraController.lookVec.x != 0)
        {
            float lookVec = cameraController.lookVec.x;
            float turnAmount = lookVec * cameraController.lookSensitivity.x;
            var rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * turnAmount, Vector3.up);
            transform.localRotation = transform.localRotation * rotation;
        }
    }
    void UpdateMoveJump(bool up, bool down, bool left, bool right, bool space, float dt)
    {

    }
    void UpdateRopeActions(bool up, bool down, bool left, bool right, bool space, bool modifier, float dt)
    {
        float pumpAmount = OnRope.pumpSpeed * dt;
        float climbAmount = OnRope.climbSpeed * dt;
        float rotateAmount = OnRope.rotateSpeed * dt;
        float leanAmount = OnRope.leanSpeed * dt;

        Vector3 leanVec = Vector3.zero;
        float climbVec = 0.0f;

        if (space)
        {
            RopePump(pumpAmount);
        }

        bool flipClimbMod = false;
        // going up
        if (up)
        {
            if (modifier == flipClimbMod)
                leanVec += new Vector3(0.0f, 0.0f, 1.0f);
            else
                climbVec += 1.0f;
        }
        // going down
        if (down)
        {
            if (modifier == flipClimbMod)
                leanVec += new Vector3(0.0f, 0.0f, -1.0f);
            else
                climbVec += -1.0f;
        }

        bool flipRotateMod = false;
        // going right
        if (right && !left)
        {
            if (modifier == flipRotateMod)
                leanVec += new Vector3(1.0f, 0.0f, 0.0f);
        }
        // going left
        if (left && !right)
        {
            if (modifier == flipRotateMod)
                leanVec += new Vector3(-1.0f, 0.0f, 0.0f);
        }

        // Pump
        if (space)
        {
            RopePump(pumpAmount);
        }
        else
        {
            var vecToRestingPump = Mathf.Clamp(
                -OnRope.distPumpUp,
                -OnRope.pumpResetSpeed * dt,
                    OnRope.pumpResetSpeed * dt);

            RopePump(vecToRestingPump);
        }

        if (leanVec != Vector3.zero)
            RopeLean(Vector3.Normalize(leanVec) * leanAmount);

        RopeClimb(climbVec * climbAmount);

        // Rotate based on mouse look
        {
            float lookVec = cameraController.lookVec.x;
            float sensitivityRotate = Mathf.Abs(cameraController.cameraTurn / cameraController.maxTurnAngle);
            sensitivityRotate = sensitivityRotate * sensitivityRotate;

            float turnAmount = lookVec * sensitivityRotate;

            RopeRotateOn(-turnAmount * OnRope.rotateSpeed * dt);
        }

    }

    void RopeClimb(float amountUp)
    {
        OnRope.distUpRope = Mathf.Clamp(
            amountUp + OnRope.distUpRope,
            0.0f,
            OnRope.rope.GetPath().PathLength);
    }
    void RopeRotateOn(float amountRight)
    {
        OnRope.angleOnRope += amountRight;
    }
    void RopePump(float amountUp)
    {
        var epsilon = 0.00001f;
        var oldDist = OnRope.distPumpUp;
        OnRope.distPumpUp = Mathf.Clamp(OnRope.distPumpUp + amountUp, -OnRope.maxPumpDownDist, OnRope.maxPumpUpDist);

        // Done with the pump
        if (OnRope.distPumpUp + epsilon > OnRope.maxPumpUpDist)
            return;
        if (OnRope.distPumpUp - epsilon < -OnRope.maxPumpDownDist)
            return;

        if (oldDist != OnRope.distFromRope)
            OnRope.rope.velocity += OnRope.rope.velocity * (amountUp * OnRope.pumpAcceleration);
    }
    void RopeLean(Vector3 amountVec)
    {
        OnRope.rope.velocity += transform.rotation * amountVec;
        //Debug.DrawLine(transform.position, transform.position + amountVec * 20.0f, Color.grey);
    }

    

    private void OnRopeChange(RopeChange e)
    {
        UpdateRope(e.dt);
    }
    void UpdateRope(float dt)
    {
        var rope = OnRope.rope;
        var ropePath = rope.GetPath();
        var ropeLength = ropePath.PathLength;
        var ropeVecNorm = rope.RopeVecNorm();

        var distOnPath = Mathf.Clamp(ropeLength - (OnRope.distUpRope), 0.0f, ropeLength);
        //var velocity = rope.VelocityAtLength(OnRope.distUpRope);

        // update Character Position
        var AngleFromDown = Quaternion.FromToRotation(Vector3.down, ropeVecNorm);
        var angularRotationOnRope = Quaternion.AngleAxis(OnRope.angleOnRope, ropeVecNorm) * AngleFromDown;
        var positionOnRope = ropePath.PointAlongPath(distOnPath);

        // @ TODO: Add charater offset!
        transform.position = positionOnRope +                                   // Position on rope
            (angularRotationOnRope * -Vector3.forward * OnRope.distFromRope) +  // set offset out from rope based on rotation
            (ropeVecNorm * -OnRope.distPumpUp);                                  // vertical offset from pumping
        
        var vecForward = positionOnRope - transform.position;
        

        //Debug.DrawLine(positionOnRope, transform.position, Color.yellow);
        var forwardRot = Quaternion.LookRotation(vecForward, -ropeVecNorm);
        transform.rotation = forwardRot;
        var characterRot = forwardRot * Quaternion.AngleAxis(OnRope.rotationPitch, transform.right) * Quaternion.AngleAxis(OnRope.rotationYaw, transform.forward);
        transform.rotation = characterRot;

        // update Snapping IK
        {
            ikSnap.rightHandPos = ropePath.PointAlongPath(distOnPath - OnRope.rightHandOffsetOnRope) + (angularRotationOnRope * OnRope.rightHandOffset);
            ikSnap.leftHandPos = ropePath.PointAlongPath(distOnPath - OnRope.leftHandOffsetOnRope) + (angularRotationOnRope * OnRope.leftHandOffset);
            ikSnap.rightFootPos = ropePath.PointAlongPath(distOnPath - OnRope.rightFootOffsetOnRope) + (angularRotationOnRope * OnRope.rightFootOffset);
            ikSnap.leftFootPos = ropePath.PointAlongPath(distOnPath - OnRope.leftFootOffsetOnRope) + (angularRotationOnRope * OnRope.leftFootOffset);
            
            ikSnap.rightHandRot = angularRotationOnRope * Quaternion.Euler(OnRope.rightHandRot);
            ikSnap.leftHandRot =  angularRotationOnRope * Quaternion.Euler(OnRope.leftHandRot) ;
            ikSnap.rightFootRot = angularRotationOnRope * Quaternion.Euler(OnRope.rightFootRot);
            ikSnap.leftFootRot =  angularRotationOnRope * Quaternion.Euler(OnRope.leftFootRot) ;
        }
    }


    void SetupOnRope()
    {
        SetVelocityRef(new FFRef<Vector3>(
            () => OnRope.rope.VelocityAtDistUpRope(OnRope.distUpRope),
            (v) => {} ));

        FFMessageBoard<RopeChange>.Connect(OnRopeChange, OnRope.rope.gameObject);
    }

    void DestroyOnRope()
    {
        if(OnRope.rope != null)
            FFMessageBoard<RopeChange>.Disconnect(OnRopeChange, OnRope.rope.gameObject);
    }
    
}

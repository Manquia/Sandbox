using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Worm : FFComponent
{
    #region Properties
    [Serializable]
    public class Spitting
    {
        public bool activeOnPeakRidge;
        public bool activeOnRearHead;
        public GameObject attackProjectile;

        public enum AttackType
        {
            Targeted,
            Random,
        }
        public AttackType type;
    }

    [Serializable]
    public class Coiling
    {
        // if not active, will goto the next wall
        public bool active = true;
        public float time = 0.5f;
        
        public enum AttackType
        {
            Targeted,
            Random,
        }
        public AttackType type;
    }

    [Serializable]
    public class Movement
    {
        public float movementSpeed;

        public float arcLength = 0.8f;
        public float arcLengthRandDelta = 0.1f;

        public float arcHeight = 0.9f;
        public float arcHeightRandDelta = 0.1f;

        public int arcsPerCycle = 3;
        public int arcsPerCycleRandDelta = 2;
    }

    [Serializable]
    public class Body
    {
        public GameObject tailSegment;
        [Range(0.02f, 5.0f)]
        public float tailSegmentSize;
        [Range(2.5f, 15.0f)]
        public float tailLength;

        public GameObject fin;
        public Vector3 finOffset;
        [Range(0.01f, 1.0f)] // fins per tail segment
        public float finDensity;

        public GameObject head;
        public GameObject tailTip;
    }
    
    public Body body;
    public Movement movement;
    public Coiling coiling;
    public Spitting spitting;

    #endregion Properties

    #region data

    FFPath movePath;
    public float movePathDist;

    FFAction.ActionSequence seq;

    // Body stuff
    Transform bodyRoot;
    List<Transform> bodyParts = new List<Transform>();
    Transform head;

    #endregion

    #region UnityEvents
    // Use this for initialization
    void Start ()
    {
        // Get Path
        movePath = transform.Find("MovePath").GetComponent<FFPath>();

        // Create body based on given properties
        CreateBody();
        // make and start the worm's logic sequence
        seq = action.Sequence();
        seq.Call(StageMoveGround);
	}

    void Update()
    {
        UpdateBodyPosition();
        UpdateDestroyTraversedPoints();

        // @TEMP
        movePathDist += Time.deltaTime * 3.0f;

        if (Input.GetMouseButtonDown(0))
        {
            //GameObject.Find("Main Camera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            var pt1 = new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f), 0.0f);
            var pt2 = new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f), 0.0f);

            var addedPoints = new Vector3[2];
            addedPoints[0] = pt1;
            addedPoints[1] = pt2;

            AddPointsToPath(addedPoints);

        }
    }

    #endregion UnityEvents

    #region Creation
    void CreateBody()
    {
        bodyRoot = new GameObject("BodyRoot").transform;
        bodyRoot.SetParent(transform, false);
        bodyRoot.localPosition = Vector3.zero;

        bodyParts.Add(CreateHead());

        float finDensityAccumulator = 0.0f;
        for(float s = 0.0f; s < body.tailLength; s += body.tailSegmentSize)
        {
            finDensityAccumulator += body.finDensity;

            if (finDensityAccumulator > 1.0f)
                bodyParts.Add(CreateTail(true));
            else
                bodyParts.Add(CreateTail(false));
        }

        bodyParts.Add(CreateTailTip());
    }
    Transform CreateTail(bool hasFin)
    {
        Transform tail = Instantiate(body.tailSegment).transform;
        if (hasFin)
        {
            Transform fin = Instantiate(body.fin).transform;
            fin.SetParent(tail, false);
            fin.localPosition = body.finOffset;
        }
        tail.SetParent(bodyRoot, false);
        return tail;
    }
    Transform CreateHead()
    {
        Transform head = Instantiate(body.head).transform;
        head.SetParent(bodyRoot, false);
        return head;
    }
    Transform CreateTailTip()
    {
        Transform tailTip = Instantiate(body.tailTip).transform;
        tailTip.SetParent(bodyRoot, false);
        return tailTip;
    }
    #endregion
    

    #region sequenceStages
    void StageMoveGround()
    {
        // Do the Arc stuff for movement!!!




        seq.Sync();
        seq.Call(StageMoveGround);
    }

    void StagePeak()
    {

    }

    void StageMoveAir()
    {

    }

    void StageCoil()
    {

    }


    #endregion sequenceStages


    #region helpers
    // Update's worm's body positions and rotation
    // @TODO @Polish possibly ignore the head + tailtip so that we can do special animation stuff.
    void UpdateBodyPosition()
    {
        Vector3 tailOffset      = Vector3.forward * 0.2f;
        Vector3 headOffset      = Vector3.forward * 0.1f;
        Vector3 tailTipOffset   = Vector3.forward * 0.1f;
        float eplison = 0.1f;

        // clamp dist along path to prevent looping
        movePathDist = Mathf.Clamp(movePathDist, 0.0f, movePath.PathLength - eplison);

        // place body onto path
        {
            float dist = movePathDist;
            int bodyPartIndex = bodyParts.Count - 1;
            Vector3 lastBodyPartPos = movePath.PointAlongPath(dist + eplison);

            while (dist > body.tailSegmentSize + eplison &&
                   bodyPartIndex > 0)
            {
                Vector3 bodyPartPos = movePath.PointAlongPath(dist);
                Vector3 forwardVector = (lastBodyPartPos - bodyPartPos);
                Vector3 faceVec = Vector3.Cross(Vector3.forward, forwardVector);

                // Debug
                //Debug.DrawLine(bodyPartPos, bodyPartPos + forwardVector * 10, Color.white);
                //Debug.DrawLine(bodyPartPos, bodyPartPos + (faceVec * 10.0f), Color.yellow);

                Transform bodyPartTrans = bodyParts[bodyPartIndex];

                bodyPartTrans.position = bodyPartPos;
                bodyPartTrans.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(-faceVec.y, faceVec.x), -Vector3.forward);

                lastBodyPartPos = bodyPartPos;
                --bodyPartIndex;
                dist -= body.tailSegmentSize;
            }
        }
    }
    // Destroys points on the path that we will never visit again
    void UpdateDestroyTraversedPoints()
    {
        // can destroy a point?
        int pointsToDestroy = 0;
        {
            float movePathLength = movePath.PathLength;
            // Length = tail length + (head + tail)
            float lengthOfWorm = body.tailLength + (2.0f * body.tailSegmentSize);
            float distAheadOfWorm = (movePathLength - movePathDist);
            while ((movePathLength - movePath.LengthAlongPathToPoint(pointsToDestroy + 2)) > distAheadOfWorm + lengthOfWorm)
            {
                ++pointsToDestroy;
            }
        }

        // destroy point, if any possible
        if(pointsToDestroy > 0)
        {
            int newPointCount = movePath.points.Length - pointsToDestroy;
            var oldPoints = movePath.points;
            float distanceLost = movePath.LengthAlongPathToPoint(pointsToDestroy);
            movePath.points = new Vector3[newPointCount];

            // @SPEED Make these standard copies
            for (int i = 0; i < newPointCount; ++i)
            {
                movePath.points[i] = oldPoints[i + pointsToDestroy];
            }

            movePath.SetupPointData();      // setup path
            movePathDist -= distanceLost;   // adjust dist for new path
        }

    }

    void AddPointsToPath(Vector3[] pointsToAdd)
    {
        var oldPathPoints = movePath.points;
        var newPoints = new Vector3[oldPathPoints.Length + pointsToAdd.Length];
        
        // @SPEED Make these standard copies
        // copy over old points
        for(int i = 0; i < oldPathPoints.Length; ++i)
        {
            newPoints[i] = oldPathPoints[i];
        }

        // @SPEED Make these standard copies
        // copy over new points
        for (int i = 0; i < pointsToAdd.Length; ++i)
        {
            newPoints[i + oldPathPoints.Length] = pointsToAdd[i];
        }

        // Assign new points to path
        movePath.points = newPoints;

        // Setup path
        movePath.SetupPointData();
    }
    #endregion helpers
}

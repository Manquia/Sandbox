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
        public float moveSpeed;

        public float arcDist = 0.8f;
        public float arcDistRandDelta = 0.1f;

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
        int arcsToComplete = movement.arcsPerCycle * UnityEngine.Random.Range(0, movement.arcsPerCycleRandDelta);
        seq = action.Sequence();
        seq.Call(StageMoveGround, arcsToComplete);
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

    // Set the ArcsToComplete in movment before calling
    void StageMoveGround(object int_arcsToComplete)
    {
        int arcsToComplete = (int)int_arcsToComplete;
        const float epsilon = 0.01f;

        // Movment Cycle ('*' == points on a path)
        //  |0       |1       |2       |<--- Offset (i)
        //  |0 1 2 3 |0 1 2 3 |0 1 2 3 |<--- Index
        //  |*       |*       |*       |
        //--|--*---*-|--*---*-|--*---*-|<--- "Ground"
        //  |    *   |    *   |    *   |
        //  |        |        |        |
        // Add all of the needed arcs while moving on the ground

        // copy over the last 4 points in reverse order
        const int pointsAdded = 4;
        // a set of pts that are described in the diagram above
        var pts = new Vector3[pointsAdded * (arcsToComplete + 1)];
        for(int i = 0; i < pointsAdded; ++i)
        {
            pts[i] = movePath.points[(movePath.points.Length - 1) - i];
        }

    
        const int raycastMask = Physics2D.AllLayers;  // @TODO @SPEED
        // Ground Movement cycle
        for (int i = 0; i < arcsToComplete; ++i)
        {
            int off = i * arcsToComplete;
            int nextOff = (i + 1) * arcsToComplete;

            // suggestd down = Normalize(v1_2 + v3_2)
            Vector3 suggestedDown = Vector3.Normalize((pts[2+off] - pts[1+off]) + (pts[2+off] - pts[3+off]));
            
            { // calculate p0
                Vector3 v2_3 = pts[3 + off] - pts[2 + off];
                Vector3 v2_3Norm = v2_3.normalized;
                float arcDist = movement.arcDist + UnityEngine.Random.Range(0.0f, movement.arcDistRandDelta);
                float arcHeight = movement.arcHeight + UnityEngine.Random.Range(0.0f, movement.arcHeightRandDelta);

                Vector3 newP0 = pts[3 + off] + (v2_3Norm * arcDist);

                Vector3 newP0Temp = newP0;
                bool newP0Good = false;
                for (int tryIndex = 0; tryIndex < 8; ++tryIndex) // try 4 times, then give up we aren't going above 
                {
                    var hit = Physics2D.OverlapCircle(newP0Temp, arcHeight, raycastMask);
                    if (!hit)
                    {
                        newP0Good = true;
                        newP0 = newP0Temp;
                        break; // didn't hit anything, point is good
                    }
                    else // hit something, adjust position and try again
                    {
                        // @Stupid @Hack Unity Doesn't give us penetration data on overlapps and doesn't have ComputerPenetrations for 2D.
                        // if we goto using Phsyics instead of 2D Phsyics this should use Phsyics.ComputePenetration instead of this hack.
                        // Test Left and rigth by some degrees         -Micah Rust Jan 19 2018
                        switch (tryIndex)
                        {
                            case 0: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(15.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                            case 1: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(-15.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                            case 2: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(40.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                            case 3: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(-40.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                            case 4: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(70.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                            case 5: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(-70.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                            case 6: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(85.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                            case 7: newP0Temp = pts[3 + off] + (Quaternion.AngleAxis(-85.0f, Vector3.forward) * (newP0 - pts[3 + off])); break;
                        }
                    }
                }

                // @TODO
                if (newP0Good)
                {
                    // Mark path Points as open to the air for shooting
                }
                else
                {
                    // Mark the path point as closed to the air
                }

                // Save value to pts output
                pts[0 + nextOff] = newP0;
            } // end calculate p0

            {// calculate p1


                //Vector3 probe0_3 = ProbeForCollider(pts[3+off] + (v2_3Norm *arcDist),

            }// end calcule p0

  


            // First We probe up out of the ground into the air
            

            // Add new points toPath
        }

        var pointsToAdd = new Vector3[pointsAdded * arcsToComplete];
        pts.CopyTo(pointsToAdd, pointsAdded);
        AddPointsToPath(pointsToAdd);

        // calculate time till we reach end of path
        float distToMove = movePath.PathLength - movePathDist;
        float timeToCompleteMove = distToMove / movement.moveSpeed;
        seq.Delay(timeToCompleteMove);
        seq.Sync();
        seq.Call(StagePeak);
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
    
    RaycastHit2D ProbeForCollider(Vector3 pos, Vector3 dir, float dist, int raycastMask, Vector3 suggestedDown, int recursionCount = 0)
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(pos, dir, dist, raycastMask);
        
        // give up, its probably not goint to happen
        if (recursionCount > 5)
        {
            Debug.Assert(false, "Worm failed to find a Collider on probing, this probably means that there is either no geometry to find OR the mask is wrong");
            return hit;
        }

        if (!hit) //  fail initial guess
        {
            var rotRight10 = Quaternion.AngleAxis(10.0f, Vector3.forward);
            var rotLeft10 = Quaternion.AngleAxis(10.0f, Vector3.forward);

            var rotRight = rotRight10;
            var rotLeft = rotLeft10;

            RaycastHit2D hitRight;
            RaycastHit2D hitLeft;

            // try with rotations in both directions
            for (int i = 0; i< 8 && !hit; ++i)
            {
                var dirRight = rotRight * dir;
                var dirLeft = rotLeft * dir;

                hitRight = Physics2D.Raycast(pos, dirRight, dist, raycastMask);
                hitLeft = Physics2D.Raycast(pos, dirLeft, dist, raycastMask);

                if (hitRight && hitLeft) // got 2 hits
                {
                    // choose hit based on dot with "down"
                    float dotRight = Vector3.Dot(suggestedDown, dirRight);
                    float dotLeft = Vector3.Dot(suggestedDown, dirLeft);

                    // choose whichever is more aligned with the suggested down
                    hit = dotRight > dotLeft? hitRight : hitLeft;
                }
                else // 0 Or 1 hit
                {
                    hit = hitLeft? hitLeft : hitRight;
                }

                // move rotations 
                rotRight = rotRight* rotRight10;
                rotLeft = rotLeft* rotLeft10;
            }
        }
        // try again with more distance
        if (!hit)
            return ProbeForCollider(pos, dir, dist * 2.0f, raycastMask, suggestedDown, recursionCount + 1);
        else
            return hit;
    }
    

    Vector3 PathForwardVec()
    {
        const float epsilon = 0.01f;

        var pointAtEndOfPath = movePath.PositionAtPoint(movePath.points.Length - 1);
        var pointNearEndOfPath = movePath.PointAlongPath(movePath.PathLength - epsilon);

        return (pointAtEndOfPath - pointNearEndOfPath).normalized;
    }
    #endregion helpers
}

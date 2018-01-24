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
        
        public float coilRadius = 2.5f;
        public float coilRadiusRandDelta = 0.5f;
        public float coilMinRadius = 0.5f;

        public int coilCount = 2;
        public int coilCountRandDelta = 1;

        public int coilRings = 1;
        public int coilRingsRandDelta = 1;
        
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

    public Transform player;

    #endregion Properties

    #region data

    // movement stuff
    FFPath movePath;
    public float movePathDist;

    // coiling stuff
    int coilCounter;

    // Sequnce stuff
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
        player = GameObject.Find("Player").transform;
        // Get Path
        movePath = transform.Find("MovePath").GetComponent<FFPath>();

        // Create body based on given properties
        CreateBody();

        // make and start the worm's logic sequence
        int arcsToComplete = movement.arcsPerCycle + UnityEngine.Random.Range(0, movement.arcsPerCycleRandDelta);
        seq = action.Sequence();
        seq.Call(StageMoveGround, arcsToComplete);
	}

    void Update()
    {
        UpdateBodyPosition();
        UpdateDestroyTraversedPoints();

        // @TEMP
        movePathDist += Time.deltaTime * movement.moveSpeed;

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
            pts[i] = movePath.PositionAtPoint(movePath.points.Length - pointsAdded + i);
        }

    
        const int raycastMask = Physics2D.AllLayers;  // @TODO @SPEED

        float arcDist = movement.arcDist + UnityEngine.Random.Range(0.0f, movement.arcDistRandDelta);
        float arcHeight = movement.arcHeight + UnityEngine.Random.Range(0.0f, movement.arcHeightRandDelta);

        // Ground Movement cycle
        for (int i = 0; i < arcsToComplete; ++i)
        {
            int off = i * pointsAdded;
            int nextOff = (i + 1) * pointsAdded;

            // suggestd down = Normalize(v1_2 + v3_2)
            Vector3 suggestedDown = Vector3.Normalize((pts[2+off] - pts[1+off]) + (pts[2+off] - pts[3+off]));
            
            { // P0, look for open area above ground
                Vector3 v2_3 = pts[3 + off] - pts[2 + off];
                Vector3 v2_3Norm = v2_3.normalized;

                Vector3 newP0 = pts[3 + off] + (v2_3Norm * arcDist);

                Vector3 newP0Temp = newP0;
                bool newP0Good = false;
                for (int tryIndex = 0; tryIndex < 8; ++tryIndex) // try a few times, then give up we aren't going above ground on this run.
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

                            default: Debug.Assert(false, "THIS SHOULDN'T EVER HAPPEN"); break;
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
            }// end calculate p0

            {// P1, Probe to ground form P0
                Vector3 vecCurDir = pts[3 + off] - pts[0+nextOff];
                Vector3 vecPastDir = pts[1+off] - pts[0+off];
                Vector3 vecForward = pts[3+off] - pts[1+off];
                Vector3 vecTowardGrond = vecPastDir.normalized + vecCurDir.normalized + vecForward.normalized * 2.0f;
                var hit = ProbeForCollider(pts[0 + nextOff], vecTowardGrond, arcDist, raycastMask, suggestedDown);

                if (hit)
                {
                    pts[1 + nextOff] = hit.point;
                }
                else // just add the point and see if we recover later
                {
                    pts[1 + nextOff] = pts[2 + off] + vecTowardGrond;
                }
            }// end calculate p1
            
            {// P2 -  Go into ground by arc length

                Vector3 vecBackDown    = pts[3 + off] - pts[0 + nextOff];
                Vector3 vecForwardDown = pts[1 + nextOff] - pts[0 + nextOff];
                Vector3 vecToGrond = vecForwardDown.normalized + vecBackDown.normalized;

                Vector3 forward = -vecBackDown + vecForwardDown;
                pts[2 + nextOff] = pts[1 + nextOff] + (forward + vecToGrond).normalized * arcDist;
            }// end calculate p2
            
            // P3 - Raycast forward, if that fails, then probe back to P2 from ending of raycast
            {
                Vector3 vecAlongGroundNorm = (pts[3 + off] - pts[1 + off]).normalized;
                RaycastHit2D rayHit = Physics2D.Raycast(pts[0 + nextOff], vecAlongGroundNorm, arcDist * 2.5f, raycastMask);

                if(rayHit) // we hit something we are good to place point
                {
                    pts[3 + nextOff] = rayHit.point;
                }
                else // raycast failed, probe back toP2
                {
                    Vector3 probePos = pts[0 + nextOff] + (vecAlongGroundNorm * arcDist * 3.0f);
                    Vector3 proveVec = pts[2 + nextOff] - probePos;
                    var probeHit = ProbeForCollider(probePos, proveVec, proveVec.magnitude, raycastMask, suggestedDown);

                    if (probeHit)
                    {
                        pts[3 + nextOff] = probeHit.point;
                    }
                    else
                    {
                        Debug.Assert(false, "previous point was p2 was bad");
                    }
                }
            }
        }

        var pointsToAdd = new Vector3[pointsAdded * arcsToComplete];
        for(int i = 0; i < pointsAdded * arcsToComplete; ++i)
        {
            pointsToAdd[i] = movePath.transform.InverseTransformPoint(pts[i + pointsAdded]);
        }

        AddPointsToPath(pointsToAdd);

        // calculate time till we reach end of path
        float distToMove = movePath.PathLength - movePathDist;
        float timeToCompleteMove = distToMove / movement.moveSpeed;

        // @Idea @Polish
        // simplify stage? nodes which are very close together and are in close sequence get pruned...

        // @Idea @Polish
        // Make path go in and out of the screen when it goes underground

        // @Idea @Polsih
        // Make worm scale 
        seq.Delay(timeToCompleteMove);
        seq.Sync();
        seq.Call(StagePeak);
    }

    public AnimationCurve tempPeakColorCurve;
    public AnimationCurve tempPeakMoveCurve;
    void StagePeak()
    {
        // Do animation stuff here...
        // @TODO Add attacking here?

        // Placeholder anim @Replace probably
        Vector3 endOfPathWorld = movePath.PositionAtPoint(movePath.points.Length - 1);
        Vector3 vecToPlayerWorld = player.position - endOfPathWorld;
        Vector3 vecToPlayerLocal = movePath.transform.InverseTransformVector(vecToPlayerWorld);
        Vector3 vecRightOfPath = (Quaternion.AngleAxis(90.0f, Vector3.forward) * vecToPlayerLocal).normalized;

        var headColor = new FFRef<Color>(() => bodyParts[0].GetComponent<SpriteRenderer>().color, (v) => bodyParts[0].GetComponent<SpriteRenderer>().color = v);
        var endPathPt = new FFRef<Vector3>(() => movePath.points[movePath.points.Length - 1], (v) => { movePath.points[movePath.points.Length - 1] = v; movePath.SetupPointData(); });

        seq.Property(headColor, Color.black, tempPeakColorCurve, 1.5f);
        seq.Property(endPathPt, endPathPt + vecRightOfPath, tempPeakMoveCurve, 1.5f);


        coilCounter = coiling.coilCount + UnityEngine.Random.Range(0, coiling.coilCountRandDelta);
        seq.Sync();
        seq.Call(StageMoveAir);
    }

    // SHould set coilCounter before entering, cannot 
    // change to parameter since we need to calculate each coil between 
    // attack to the player... so the data needs to be held on the Worm Componenet
    void StageMoveAir()
    {
        //@Polish, Change Speed when in air stage?

        // Attack player @TODO remove type from coiling class above
        const float epsilon = 0.1f;
        const float dist = 9999.0f;
        const int raycastMask = -1; // @SPEED, @POLISH, @BUG (hts the player)
        Vector3 endOfPathWorld = movePath.PositionAtPoint(movePath.points.Length - 1);
        Vector3 vecToPlayerWorld = player.position - endOfPathWorld;

        var hit = Physics2D.Raycast(endOfPathWorld, vecToPlayerWorld, dist, raycastMask);

        if(hit)
        {
            Vector3 endpoint = hit.point;

            if (coiling.active && coilCounter > 0)
            {
                --coilCounter;
                // Add all points for coiling
                AddCoilAtPos(endOfPathWorld + vecToPlayerWorld);

                // sync then move air toward the player again without coiling this time
                float distOfn_1 = movePath.LengthAlongPathToPoint(movePath.points.Length - 1);
                float distOfn_2 = movePath.LengthAlongPathToPoint(movePath.points.Length - 2);
                float distOfEndSegment = distOfn_1 - distOfn_2;

                float distToTravel = (movePath.PathLength - movePathDist) - distOfEndSegment;
                float timeToCompleteCoil = distToTravel / movement.moveSpeed;

                seq.Delay(timeToCompleteCoil);
                seq.Sync();
                seq.Call(StageMoveAir);
                return; // comes back here later after delay which is added in AddCoilPos
            }
            else // end StageMoveAir
            {
                // Adds all of the points to move back into the snaking motion
                InitGroundMoveSeq(hit);
            }
        }
        else
        {
            Debug.Assert(false, "Worm raycasted but found nothing this should never happen. Fix the level (to be enclosing) of script or maybe something else like it escaped...");
        }
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
            string message = "Probe failed at dist " + dist + " with suggestedDown " + suggestedDown;
            Debug.Assert(false, message);
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

    void InitGroundMoveSeq(RaycastHit2D hit)
    {

    }

    void AddCoilAtPos(Vector3 pos)
    {
        float coilRadiusDelta =  Mathf.Max(0.0f, coiling.coilRadius - coiling.coilMinRadius) + UnityEngine.Random.Range(0.0f, coiling.coilRadiusRandDelta);
        int coilRingCount = coiling.coilRings + UnityEngine.Random.Range(0, coiling.coilRingsRandDelta);

        const float epsilon = 0.001f;
        int pointCount = coilRingCount * 7;
        var pts = new Vector3[pointCount];
        var zWorld = pos.z;

        float maxF = (Mathf.PI / 7.0f) * pointCount + epsilon;

        { // calculate coil points
            Vector3 pt = Vector3.zero;
            float f = 0.0f;
            int i = 0;
            while (i < pointCount)
            { 
                var xLocal = Mathf.Cos(f) * (coiling.coilMinRadius + coilRadiusDelta * (1 - (f/maxF)));
                var yLocal = Mathf.Sin(f) * (coiling.coilMinRadius + coilRadiusDelta * (1 - (f/maxF)));

                pts[i].Set(pos.x + xLocal, pos.y + yLocal, pos.z);

                ++i;
                f += Mathf.PI / 7.0f;
            }
        }

        // make points local to path
        for(int i = 0; i < pts.Length; ++i)
            pts[i] = movePath.transform.InverseTransformPoint(pts[i]);

        AddPointsToPath(pts);
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

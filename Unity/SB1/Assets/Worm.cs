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
        
        public float coilRadius = 2.5f;
        public float coilRadiusRandDelta = 0.5f;
        public float coilMinRadius = 0.5f;

        public int coilCount = 2;
        public int coilCountRandDelta = 1;

        public int coilRings = 1;
        public int coilRingsRandDelta = 1;

        public AnimationCurve toAirMoveCurve;
        public AnimationCurve toGroundMoveCurve;
        public AnimationCurve coilMoveCurve;
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

        public AnimationCurve moveCycleCurve;
    }

    [Serializable]
    public class Body
    {
        public GameObject tailSegment;
        [Range(0.02f, 5.0f)]
        public float tailSegmentSize;
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

    // These are pased to base work object
    #region PathEvents

    public struct Events
    {


    // Trigger next StageAir Or ground attack
    public struct NearingEndOfCoil
    {
        public bool endStageMoveAir;
    }
    // Trigger Peak Player
    public struct NearingPeakAtPlayer
    {
    }


    // Sound Effects, screen shake
    public struct BreakOutOfGround
    {
    }
    // Sound Effects, screen shake
    public struct BreakIntoGround
    {
    }
    // Attacking with projectiles
    public struct GroundMovementPeak
    {
    }


    public struct StartCoiling
    {
    }
    public struct EndCoiling
    {
    }

    // Attacking with projectiles
    public struct PeakAtPlayer
    {
    }
    }


    #endregion
    #region data

    // movement stuff
    FFPath movePath;
    public float movePathDist = 0.0f;
    public int movePathIndex = 0;

    class MovementCurve
    {
        public int offset;
        public int startIndex;
        public int endIndex;
        public float lostDist;
        public AnimationCurve curve;
    }
    List<MovementCurve> movementCurves = new List<MovementCurve>();


    class PointEventDisbatcher
    {
        public int offset;
        public int index;
        public Action dispatcher;
    }
    List<PointEventDisbatcher> pointEvents = new List<PointEventDisbatcher>();

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
        const float epsilon = 0.001f;

        // Get Speed Mulitplier
        float speed = movement.moveSpeed * Time.deltaTime;
        for (int i = 0; i < movementCurves.Count; ++i)
        {
            int startIndex = movementCurves[i].startIndex;
            int endIndex = movementCurves[i].endIndex;
            int offset = movementCurves[i].offset;

            // the path hasn't been made yet for this move curve
            int maxIndex = movePath.linearDistanceAlongPath.Length - 1;
            if (endIndex + offset > maxIndex ||
                startIndex + offset > maxIndex)
            {
                continue;
            }

            float mu;
            {// calculate mu (normalized distance along the movement Curve)

                float distToEndOfSegment = movePath.LengthAlongPathToPoint(offset + endIndex);

                // points will be removed from the segment so we must add the lost to the remaining to get the
                float distToStartOfSegment;
                if (offset + startIndex < 0)
                {
                    distToStartOfSegment = 0.0f; // start is at 0 when we are counting lost distance
                    distToEndOfSegment += movementCurves[i].lostDist; // end dist is gets all lost distance added to it
                }
                else
                {
                    distToStartOfSegment = movePath.LengthAlongPathToPoint(offset + startIndex);
                }

                // Move Curve is no longer affecting us
                if (distToEndOfSegment < movePathDist)
                    continue;

                // @SPEED, this can probably be made into a break because we don't add curves to apply to the past. Only things moveing forward
                // Have not reached MovementCurve
                if (distToStartOfSegment > movePathDist)
                    continue;

                float distOfSegment =
                    distToEndOfSegment -
                    distToStartOfSegment;
                float distAlongSegment = distOfSegment - (distToEndOfSegment - movePathDist);

                mu = distAlongSegment / distOfSegment;
            } // end calculate mu
            
            AnimationCurve moveCurve = movementCurves[i].curve;
            float speedScale = moveCurve.Evaluate(mu * moveCurve.TimeToComplete());
            
            speed *= speedScale;
        }// end get speed multiplier

        // Move Worm along path
        movePathDist = Mathf.Clamp(movePathDist + speed, 0.0f, movePath.PathLength - epsilon);
        
        // destroy any points we went past
        UpdateDestroyTraversedPoints();

        // Send out points events
        int indexOfNext;
        Vector3 nextPt = movePath.NextPoint(movePathDist, out indexOfNext);
        int indexOfPrev = indexOfNext - 1;
        for (int i = 0; i < pointEvents.Count; ++i)
        {
            int eventIndex = pointEvents[i].index;
            int eventOffset = pointEvents[i].offset;
            int curIndex = eventIndex + eventOffset;

            if (curIndex <= indexOfNext)
            {
                pointEvents[i].dispatcher();
                pointEvents.RemoveAt(i);
                --i;
                continue;
            }
        }

        // Update Body parts's pos and rot
        UpdateBodyParts();
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
            {
                finDensityAccumulator -= 1.0f;
                bodyParts.Add(CreateTail(true));
            }
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

    #region SequenceStages

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
        for (int i = 0; i < pointsAdded; ++i)
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
            int pathOffset = nextOff + movePath.points.Length;

            // suggestd down = Normalize(v1_2 + v3_2)
            Vector3 suggestedDown = Vector3.Normalize((pts[2 + off] - pts[1 + off]) + (pts[2 + off] - pts[3 + off]));

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

                Events.GroundMovementPeak gmp;
                AddPointEvent(gmp, 0 + pathOffset);


                // Save value to pts output
                pts[0 + nextOff] = newP0;
            }// end calculate p0

            {// P1, Probe to ground form P0
                Vector3 vecCurDir = pts[3 + off] - pts[0 + nextOff];
                Vector3 vecPastDir = pts[1 + off] - pts[0 + off];
                Vector3 vecForward = pts[3 + off] - pts[1 + off];
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

                Events.BreakIntoGround big;
                AddPointEvent(big, 1 + pathOffset);

            }// end calculate p1

            {// P2 -  Go into ground by arc length

                Vector3 vecBackDown = pts[3 + off] - pts[0 + nextOff];
                Vector3 vecForwardDown = pts[1 + nextOff] - pts[0 + nextOff];
                Vector3 vecToGrond = vecForwardDown.normalized + vecBackDown.normalized;

                Vector3 forward = -vecBackDown + vecForwardDown;
                pts[2 + nextOff] = pts[1 + nextOff] + (forward + vecToGrond).normalized * arcDist;


            }// end calculate p2

            // P3 - Raycast forward, if that fails, then probe back to P2 from ending of raycast
            {
                Vector3 vecAlongGroundNorm = (pts[3 + off] - pts[1 + off]).normalized;
                RaycastHit2D rayHit = Physics2D.Raycast(pts[0 + nextOff], vecAlongGroundNorm, arcDist * 2.5f, raycastMask);

                if (rayHit) // we hit something we are good to place point
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

                Events.BreakOutOfGround bog;
                AddPointEvent(bog, 3 + pathOffset);
            }

            AddMovementCurve(movement.moveCycleCurve, pathOffset + 0, pathOffset + 3);
        }

        var pointsToAdd = new Vector3[pointsAdded * arcsToComplete];
        var movePathTrans = movePath.transform;
        for (int i = 0; i < pointsAdded * arcsToComplete; ++i)
            pointsToAdd[i] = movePathTrans.InverseTransformPoint(pts[i + pointsAdded]);

        AddPointsToPath(pointsToAdd);

        // Add point events
        {
            Events.NearingPeakAtPlayer npap;
            Events.PeakAtPlayer pap;
            AddPointEvent(npap, movePath.points.Length - 3);
            AddPointEvent(pap, movePath.points.Length - 1);
        }

        // @Idea @Polish
        // nodes which are very close together and are in close sequence get pruned...

        // @Idea @Polish
        // Make path go in and out of the screen when it goes underground

        // @Idea @Polsih
        // Make worm scale 
        
        FFMessageBoard<Events.PeakAtPlayer>.Connect(OnPeakAtPlayer, gameObject);
    }
    void OnPeakAtPlayer(Events.PeakAtPlayer npap)
    {
        FFMessageBoard<Events.PeakAtPlayer>.Disconnect(OnPeakAtPlayer, gameObject);
        StagePeak();
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
        

        if (coiling.active && coilCounter > 0)
        {
            --coilCounter;

            // Add movement Curve for flying out to coil
            int lastPointOfPath = movePath.points.Length - 1;
            AddMovementCurve(coiling.toAirMoveCurve, lastPointOfPath, lastPointOfPath+1);

            // Add all points for coiling
            AddCoilAtPos(endOfPathWorld + vecToPlayerWorld);

            // add PointEvent for coil
            Events.NearingEndOfCoil neoc;
            neoc.endStageMoveAir = coilCounter <= 0;
            AddPointEvent(neoc, movePath.points.Length-3);

            // sync then move air toward the player again without coiling this time
            float distOfn_1 = movePath.LengthAlongPathToPoint(movePath.points.Length - 1);
            float distOfn_2 = movePath.LengthAlongPathToPoint(movePath.points.Length - 2);
            float distOfEndSegment = distOfn_1 - distOfn_2;

            float distToTravel = (movePath.PathLength - movePathDist) - distOfEndSegment;
            float timeToCompleteCoil = distToTravel / movement.moveSpeed;

            FFMessageBoard<Events.NearingEndOfCoil>.Connect(OnNearingEndOfCoil, gameObject);
            return; // comes back here through the event connection
        }

        var hit = Physics2D.Raycast(endOfPathWorld, vecToPlayerWorld, dist, raycastMask);
        if (hit)
        {
            Vector3 endpoint = hit.point;
            // Add movement Curve for flying out to ground
            int lastPointOfPath = movePath.points.Length - 1;
            AddMovementCurve(coiling.toAirMoveCurve, lastPointOfPath, lastPointOfPath + 1);

            // Adds all of the points to move back into the snaking motion on the ground
            InitGroundMoveSeq(hit);

            int arcCycles = movement.arcsPerCycle + UnityEngine.Random.Range(0, movement.arcsPerCycleRandDelta);

            // We just did a good number of raycasts, wait a bit to calculate the next part of the path OR
            // Change To happens when 2nd to last pt is reached Event. The delay is variable
            // because of the Movment Curves.
            seq.Delay(0.2f); 
            seq.Sync();
            seq.Call(StageMoveGround, arcCycles);
        }
    }
    private void OnNearingEndOfCoil(Events.NearingEndOfCoil e)
    {
        FFMessageBoard<Events.NearingEndOfCoil>.Disconnect(OnNearingEndOfCoil, gameObject);
        StageMoveAir();
    }

    #endregion sequenceStages

    #region helpers
    void AddMovementCurve(AnimationCurve curve, int startIndex, int endIndex)
    {
        MovementCurve mc = new MovementCurve
        {
            curve = curve,
            startIndex = startIndex,
            endIndex = endIndex,
            offset = 0
        };

        movementCurves.Add(mc);
    }

    void AddPointEvent<EventType>(EventType e, int pointsIndex)
    {
        PointEventDisbatcher ped = new PointEventDisbatcher()
        {
            dispatcher = () => FFMessageBoard<EventType>.SendToLocal(e, gameObject),
            index = pointsIndex,
            offset = 0
        };

        pointEvents.Add(ped);
    }
    // Update's worm's body positions and rotation
    // @TODO @Polish possibly ignore the head + tailtip so that we can do special animation stuff.
    void UpdateBodyParts()
    {
        Vector3 tailOffset      = Vector3.forward * 0.2f;
        Vector3 headOffset      = Vector3.forward * 0.1f;
        Vector3 tailTipOffset   = Vector3.forward * 0.1f;
        float epsilon = 0.1f;

        // place body onto path
        {
            float dist = movePathDist;
            int bodyPartIndex = bodyParts.Count - 1;
            Vector3 lastBodyPartPos = movePath.PointAlongPath(dist + epsilon);

            while (dist > body.tailSegmentSize + epsilon &&
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
            const float epsilon = 0.001f;
            int newPointCount = movePath.points.Length - pointsToDestroy;
            var oldPoints = movePath.points;
            float distanceLost = movePath.LengthAlongPathToPoint(pointsToDestroy);
            movePath.points = new Vector3[newPointCount];

            // Update offsets of movementCurves and record any lost distance
            for(int i = 0; i < movementCurves.Count; ++i)
            {
                movementCurves[i].offset -= pointsToDestroy;

                // has distance been lost?
                if (movementCurves[i].startIndex + movementCurves[i].offset < 0)
                {
                    movementCurves[i].lostDist += movePath.linearDistanceAlongPath[pointsToDestroy];
                }

                // Move curve is no longer important, remove it b/c we
                // will never use it again
                if (movementCurves[i].endIndex + movementCurves[i].offset < 0)
                {
                    movementCurves.RemoveAt(i);
                    --i;
                }
            }

            // Update offsets of Point events
            for(int i = 0; i < pointEvents.Count; ++i)
            {
                pointEvents[i].offset -= pointsToDestroy;
            }

            // copy over old points into new points array
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
        Vector3[] pts = new Vector3[4];
        // This function creates cycle 0 from a raycast hit
        // Movment Cycle ('*' == points on a path)
        //  |0       |1       |2       |<--- Offset (i)
        //  |0 1 2 3 |0 1 2 3 |0 1 2 3 |<--- Index
        //  |*       |*       |*       |
        //--|--*---*-|--*---*-|--*---*-|<--- "Ground"
        //  |    *   |    *   |    *   |
        //  |        |        |        |
        // Add all of the needed arcs while moving on the ground

        Vector3 up = hit.normal;
        const float sqrt_2 = 1.41421356f;
        int raycastMask = Physics2D.AllLayers;  // @TODO @SPEED
        bool goesRight = UnityEngine.Random.value > 0.5f;
        var rot90Forward = Quaternion.AngleAxis(goesRight ? 90.0f : -90.0f, Vector3.forward);
        float arcDist = movement.arcDist + UnityEngine.Random.Range(0.0f, movement.arcDistRandDelta);

        
        Vector3 forward = rot90Forward * hit.normal;
        Vector3 diagonalIntoGround = (forward + -up).normalized;


        pts[1] = hit.point;
        pts[0] = pts[1] + (-diagonalIntoGround * arcDist);
        pts[2] = pts[1] + (diagonalIntoGround * arcDist);
        pts[3] = pts[1] + (forward * arcDist * sqrt_2);

        // ensure pts3 is on the ground
        {
            //raycast from 0 to 3
            var ray0_3 = Physics2D.Raycast(pts[0], pts[3] - pts[0], arcDist * sqrt_2 * 0.25f, raycastMask);

            if(ray0_3)
            {
                pts[3] = ray0_3.point;
            }
            else // failed, do probe from pts3 to pts2
            {
                var probe3_2 = ProbeForCollider(pts[3], pts[2] - pts[3], arcDist * 2.0f,raycastMask, -up);

                if(probe3_2) // probe succeeded
                {
                    pts[3] = probe3_2.point;
                }
            }
        }

        // point Events
        {
            int eventsOffset = movePath.points.Length;
            //AddPointEvent(new GroundMovementPeak(), 0 + eventsOffset); // doesn't happen on init
            AddPointEvent(new Events.BreakIntoGround(),    1 + eventsOffset);
            AddPointEvent(new Events.BreakOutOfGround(),   3 + eventsOffset);
        }

        var movePathTrans = movePath.transform;
        for (int i = 0; i < pts.Length; ++i)
            pts[i] = movePathTrans.InverseTransformPoint(pts[i]);

        AddPointsToPath(pts);
    }

    void AddCoilAtPos(Vector3 pos)
    {
        float coilRadiusDelta =  Mathf.Max(0.0f, coiling.coilRadius - coiling.coilMinRadius) + UnityEngine.Random.Range(0.0f, coiling.coilRadiusRandDelta);
        int coilRingCount = coiling.coilRings + UnityEngine.Random.Range(0, coiling.coilRingsRandDelta);

        const float epsilon = 0.001f;
        int pointCount = coilRingCount * 7 + 1;
        var pts = new Vector3[pointCount];
        
        Vector3 vecToCoil = pos - movePath.PositionAtPoint(movePath.points.Length - 1);
        Vector3 vecToCoilNorm = Vector3.Normalize(vecToCoil);

        // calculate coil points
        {
            bool clockwise = UnityEngine.Random.value > 0.5f;
            // @TODO, @POLISH, @MAYBE
            // Have the worm be able to coil inward and outward?
            //bool inwardCoil = UnityEngine.Random.value > 0.5f;

            float degreeDelta = (Mathf.PI / 7.0f) * (clockwise ? -1.0f : 1.0f);
            float radiusDelta = -(Mathf.PI / 7.0f);

            // to make sooth transition from the worm't current position
            float startOffset = Mathf.Atan2(vecToCoilNorm.y, vecToCoilNorm.x) - ((clockwise ? -1.0f : 1.0f) * 90.0f * Mathf.Deg2Rad);
            float fAngle = startOffset;

            float maxF = (Mathf.PI / 7.0f) * pointCount + epsilon;
            float fRadius = maxF - epsilon;

            // calculations for first point
            {
                var xLocalFirst = Mathf.Cos(fAngle) * (coiling.coilMinRadius + coilRadiusDelta * (fRadius / maxF));
                var yLocalFirst = Mathf.Sin(fAngle) * (coiling.coilMinRadius + coilRadiusDelta * (fRadius / maxF));

                // Set center to be offset by the radius so that the coil starts pos
                pos.Set(pos.x - xLocalFirst, pos.y - yLocalFirst, pos.z);

                // set first points
                pts[0].Set(pos.x + xLocalFirst, pos.y + yLocalFirst, pos.z);
                pts[1].Set(pos.x + xLocalFirst, pos.y + yLocalFirst, pos.z);

                // Set fAngle and fRadius to be at position of 2nd point on the sprile (3rd actual pt)
                fAngle += degreeDelta;
                fRadius += radiusDelta;
            }

            int i = 2;
            while (i < pointCount)
            {
                float mu = fRadius / maxF;

                var xLocal = Mathf.Cos(fAngle) * (
                    coiling.coilMinRadius +
                    coilRadiusDelta * mu);

                var yLocal = Mathf.Sin(fAngle) * (
                    coiling.coilMinRadius +
                    coilRadiusDelta * mu);

                pts[i].Set(pos.x + xLocal, pos.y + yLocal, pos.z);

                ++i;
                fAngle += degreeDelta;
                fRadius += radiusDelta;
            }

            // setup last point
            //fAngle -= degreeDelta;
            //fRadius -= radiusDelta;
            //var xLocalLast = Mathf.Cos(fAngle) * (coiling.coilMinRadius + coilRadiusDelta * (fRadius / maxF));
            //var yLocalLast = Mathf.Sin(fAngle) * (coiling.coilMinRadius + coilRadiusDelta * (fRadius / maxF));
            //pts[pts.Length-1].Set(pos.x + xLocalLast, pos.y + yLocalLast, pos.z);
        }
        
        // Coil Movement Curves
        int firstIndexOfCoil = movePath.points.Length;
        AddMovementCurve(coiling.coilMoveCurve, firstIndexOfCoil, firstIndexOfCoil + pointCount);

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

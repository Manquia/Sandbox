using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : JobComponentSystem
{
    [ComputeJobOptimization]
    [Unity.Burst.BurstCompile]
    struct MovementJob : IJobProcessComponentData<Position,Rotation, MoveSpeed>
    {
        public float dt;

        public void Execute(ref Position positionRef, [ReadOnly] ref Rotation rotationRef, [ReadOnly] ref MoveSpeed moveSpeedRef)
        {
            float3 value = positionRef.Value;

            value += dt * moveSpeedRef.Value * math.forward(rotationRef.Value);

            positionRef.Value = value;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MovementJob moveJob = new MovementJob
        {
            dt = Time.deltaTime,
        };

        JobHandle moveHandle = moveJob.Schedule(this, inputDeps);

        return moveHandle;
    }
}

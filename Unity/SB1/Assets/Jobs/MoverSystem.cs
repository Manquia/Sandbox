using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using System;

public class MoverSystem : MonoBehaviour
{
    TransformAccessArray transforms;
    MovementJob moveJob;
    JobHandle moveHandle;

    private void OnDisable()
    {
        moveHandle.Complete();
        transforms.Dispose();
    }
    private void Start()
    {
        int count = 5000;
        AddMoreMovers(count);
    }

    private void Update()
    {
        // finsih job this frame
        moveHandle.Complete();

        if(Input.GetKeyDown("Space"))
        {
            int count = 5000;
            AddMoreMovers(count);
        }

        moveJob = new MovementJob()
        {
            moveSpeed = 4,  
            dt = 23,
        };

        moveHandle = moveJob.Schedule(transforms);

        JobHandle.ScheduleBatchedJobs();
    }
    public GameObject moveablePrefab;

    private void AddMoreMovers(int count)
    {
        // finish the job if we are still doing it
        moveHandle.Complete();

        transforms.capacity += 5000;

        for(int i = 0; i < count; ++i)
        {
            var obj = Instantiate(moveablePrefab);

            transforms.Add(obj.transform);
        }
    }
}

public struct MovementJob : IJobParallelForTransform
{
    public float moveSpeed;
    public float dt;

    public void Execute(int index, TransformAccess transform)
    {
        Vector3 vec = transform.position;
        vec += moveSpeed * dt * (transform.rotation * Vector3.forward);
        transform.localPosition += vec;
    }
}

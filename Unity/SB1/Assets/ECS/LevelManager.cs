using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;


public class LevelManager : MonoBehaviour
{
    public float moveSpeed = 10;
    public float moveSpeedDelta = 2;

    public GameObject movablePrefab;
    EntityManager manager;

	// Use this for initialization
	void Start ()
    {
        manager = World.Active.GetOrCreateManager<EntityManager>();
	}

	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            int count = 1000;
            AddMoveable(count);
        }
		
	}

    private void AddMoveable(int count)
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
        manager.Instantiate(movablePrefab, entities);

        for(int i = 0; i < count; ++i)
        {
            float xVal = UnityEngine.Random.Range(-10f, 10f);
            float yVal = UnityEngine.Random.Range(0f, 10f);
            manager.SetComponentData(entities[i], new Position { Value = new float3(xVal, yVal, 0) });
            manager.SetComponentData(entities[i], new Rotation { Value = new quaternion(0, 1, 0, 0) });
            manager.SetComponentData(entities[i], new MoveSpeed { Value = moveSpeed + UnityEngine.Random.Range(-moveSpeedDelta, moveSpeedDelta) });
        }
        entities.Dispose();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[System.Serializable]
public struct MoveSpeed : IComponentData
{
    public float Value;
}

public class MoveSpeedComponent : ComponentDataWrapper<MoveSpeed> { }

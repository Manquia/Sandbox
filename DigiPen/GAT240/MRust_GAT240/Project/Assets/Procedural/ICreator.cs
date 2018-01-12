using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICreator
{
    void Create();
    void Destroy();
    void SetSeed(int seed);
}

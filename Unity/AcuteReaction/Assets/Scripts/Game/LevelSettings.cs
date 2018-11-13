using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "AcuteReaction/Settings", order = 1)]
public class LevelSettings : ScriptableObject
{
    [System.Serializable]
    public struct Prefabs
    {
        public GameObject VisualDot;
        public GameObject VisualLine;
        public GameObject LevelRoot;
    }
    public Prefabs prefabs;

    [System.Serializable]
    public struct Variables
    {
    }
    public Variables variables;
}

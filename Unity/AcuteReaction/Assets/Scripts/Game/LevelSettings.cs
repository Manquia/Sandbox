using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "AcuteReaction/Settings", order = 1)]
public class LevelSettings : ScriptableObject
{
    [System.Serializable]
    public struct Prefabs
    {
        public GameObject dot;
        public GameObject unsetLine;
        public GameObject setLine;

        public GameObject levelRoot;
        public GameObject setupRoot;
        public GameObject runtimeRoot;

        public GameObject clusterRoot;
        public GameObject solidLine;
        public GameObject conveyerLine;
    }
    public Prefabs prefabs;

    [System.Serializable]
    public class Variables
    {
        public float DrawingLineYOffset = 0.1f;
    }
    public Variables variables;
}

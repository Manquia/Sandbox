using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{
    static internal int g_traveralIndex = -1;

    public struct FieldInfo
    {
        public string name;
        public int size;
        public int offset;

        public FieldInfo(string name, int size, int offset)
        {
            this.name = name;
            this.size = size;
            this.offset = offset;
        }
    }

    public enum CacheAffinity
    {
        Robust,
        Unsteady,
        Grave,
    }
    [System.Serializable]
    public class DataNode
    {
        internal int traversalIndex = -1;
        public string containerIdentifier;
        public List<DataConnection> output = new List<DataConnection>();
        public List<FieldInfo> nodeData = new List<FieldInfo>();
        public Transform visualNode;

        public DataNode(string containerIdentifier, List<DataConnection> output, List<FieldInfo> nodeData)
        {
            this.containerIdentifier = containerIdentifier;
            this.output = output;
            this.nodeData = nodeData;
        }

        internal void CreateGraph()
        {
            throw new NotImplementedException();
        }
    }
    [System.Serializable]
    public class DataConnection
    {
        public DataNode source;
        public List<FieldInfo> dataStream = new List<FieldInfo>();
        public DataNode dest;

        public long byteFlowCount;
        public long cycleCount;

        internal CacheAffinity affinity
        {
            get
            {
                CalculateThresholds();

                double ratio = byteFlowCount / cycleCount;
                if (ratio > robustThreshold)
                    return CacheAffinity.Robust;
                else if (ratio > unsteadyThreshold)
                    return CacheAffinity.Unsteady;
                else
                    return CacheAffinity.Grave;
            }
        }



        static void AddRatio(double ratio)
        {
            bytesPerCycleDataCached = false;
            bytesPerCycle.Add(ratio);
        }
        static void WipeRatios()
        {
            bytesPerCycleDataCached = false;
            bytesPerCycle.Clear();
        }

        static List<double> bytesPerCycle = new List<double>();

        // Cached and refreshed on CalculateThresholds
        static bool bytesPerCycleDataCached = false;
        internal static double bytesPerCycleAverage;
        internal static double robustThreshold;
        internal static double unsteadyThreshold;
        internal static double graveThreshold;

        public DataConnection(DataNode source, List<FieldInfo> souceData, DataNode dest)
        {
            this.source = source;
            this.dataStream = souceData;
            this.dest = dest;
        }

        void CalculateThresholds()
        {
            if (bytesPerCycleDataCached)
                return;

            double aveRatio = 0;
            foreach(var ratio in bytesPerCycle)
            {
                aveRatio += ratio;
            }
            aveRatio /= bytesPerCycle.Count;

            bytesPerCycleAverage = aveRatio;
            robustThreshold = aveRatio * 1.05;
            unsteadyThreshold = aveRatio * 0.95;
            graveThreshold = aveRatio * 0.75;
            bytesPerCycleDataCached = true;
        }
    }


	// Use this for initialization
	void Start ()
    {
        // populate data

        DataNode level = new DataNode("Level", new List<DataConnection>(), new List<FieldInfo>());
        level.nodeData.Add(new FieldInfo("ObjectId", 4, 0));
        level.nodeData.Add(new FieldInfo("TransformPos", 12, 4));
        level.nodeData.Add(new FieldInfo("TransformRot", 16, 16));
        level.nodeData.Add(new FieldInfo("TransformScale", 12, 32));

        List<DataNode> files = new List<DataNode>();

        files.Add(new DataNode("File1", new List<DataConnection>(), level.nodeData));
        files[0].output.Add(new DataConnection(files[0], files[0].nodeData, level));
        files[0].output[0].cycleCount = 600;
        files[0].output[0].byteFlowCount = 6300;

        files.Add(new DataNode("File2", new List<DataConnection>(), level.nodeData));
        files[1].output.Add(new DataConnection(files[1], files[1].nodeData, level));
        files[1].output[0].cycleCount = 5600;
        files[1].output[0].byteFlowCount = 90000;

        files.Add(new DataNode("File3", new List<DataConnection>(), level.nodeData));
        files[2].output.Add(new DataConnection(files[2], files[2].nodeData, level));
        files[2].output[0].cycleCount = 10000;
        files[2].output[0].byteFlowCount = 9001;

        ++g_traveralIndex;

        foreach(var n in files)
        {
            createNode(n);
        }

        

        //head.CreateGraph();
        Debug.Log("We made the graph!");
    }

    void createNode(DataNode node)
    {
        // base case, already generated for this node
        if (node.traversalIndex >= g_traveralIndex)
        {
            node.traversalIndex = g_traveralIndex;
            return;
        }
        else
        {
            node.traversalIndex = g_traveralIndex;
        }



        foreach(var o in node.output)
        {
            // no known visual hierarchy is valid
            // start at the center, will be parented later hopefully
            if(o.dest.visualNode == null)
            {

            }
            else
            {

            }

        }
    }


    Dictionary<string, Transform> objectPool = new Dictionary<string, Transform>();
	

    void CreateDataNodeVisual(DataNode n, Transform parent)
    {
        


    }

    public Transform DataNodeVisualsPrefab;
    public Transform DataConnectionVisualPrefab;

    void Generate()
    {

    }
}

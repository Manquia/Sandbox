using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour {

    public enum CacheAffinity
    {
        Robust,
        Unsteady,
        Grave,
    }
    [System.Serializable]
    public class DataNode
    {
        public string name;
        public List<DataConnection> input  = new List<DataConnection>();
        public List<DataConnection> output = new List<DataConnection>();

        internal void CreateGraph()
        {
            throw new NotImplementedException();
        }
    }
    [System.Serializable]
    public class DataConnection
    {
        public DataNode source;
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

        //DataNode nodes[] = 
        //DataNode head = new DataNode();
        //
        //head.name = "Root";
        //
        //
        //// Teir 1
        //{
        //    head.output.Add(new DataConnection())
        //}
        //
        //
        //
        //
        //
        //head.CreateGraph();
	}
	



    void Generate()
    {

    }
}

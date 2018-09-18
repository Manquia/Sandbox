using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NonEuclideanGrid : MonoBehaviour {

    // Doing 2D version
    public class GridData
    {
        public Quaternion rot0;
        public Quaternion rot1;
        public Quaternion rot2;
        public Quaternion rot3;
        public Vector3 scale0;
        public Vector3 scale1;
        public Vector3 scale2;
        public Vector3 scale3;

        public Vector2 pos;
        public Vector2 halfWidth; // Half width

        public GridData subGrid0;
        public GridData subGrid1;
        public GridData subGrid2;
        public GridData subGrid3;

    }
    public struct GridSample
    {
        public Quaternion rot;
        public Vector3 scale;
    }

    static NonEuclideanGrid singleton;
    static NonEuclideanGrid sing()
    {
        return singleton;
    }

    public static void UpdatePhysics(NonEuclideanObject obj)
    {
        Vector2 pos = obj.pos;
        Vector3 scale = obj.scale;
        Quaternion rot = obj.rot;

        // Transform NonEuclideanObject To The gridSpace
        {
            //GridData data = GetSample(pos);
            // -r s r



        }


        // Make Matrix for object and assign it
        
    }

    GridSample GetSample(Vector2 pos)
    {
        GridData data = head;
        GridSample sample;

        sample.rot = Quaternion.AngleAxis(0, Vector3.up);
        sample.scale = Vector3.one;
        int sampleCount = 0;

        while (true)
        {
            Vector2 vec = data.pos - pos;
            GridSample sampledData = SampleData(data, vec);

            sample.rot = sample.rot * sampledData.rot;
            sample.scale += sampledData.scale;

            ++sampleCount;
            // is a subgrid?
            if (data.subGrid0 == null) break;

            // find correct sub grid
            if (Inside(data.subGrid0.pos, data.subGrid0.halfWidth, pos)) data = data.subGrid0;
            else if (Inside(data.subGrid1.pos, data.subGrid1.halfWidth, pos)) data = data.subGrid1;
            else if (Inside(data.subGrid2.pos, data.subGrid2.halfWidth, pos)) data = data.subGrid2;
            else if (Inside(data.subGrid3.pos, data.subGrid3.halfWidth, pos)) data = data.subGrid3;
            else break;

        }

        // get average scale
        sample.scale /= sampleCount;

        return sample;
    }

    private GridSample SampleData(GridData data, Vector2 vec)
    {
        GridSample sample;

        // @TODO this algorithm I think it wrong... We should do something better in the future?

        // betweem -0.5 and 0.5
        float halfVecNormX = (vec.x / data.halfWidth.x) * 0.5f;
        float halfVecNormY = (vec.y / data.halfWidth.y) * 0.5f;

        //0|1
        //-+-
        //2|3


        float mu0 = Mathf.Abs(halfVecNormX + 0.5f) + Mathf.Abs(halfVecNormY - 0.5f);
        float mu1 = Mathf.Abs(halfVecNormX - 0.5f) + Mathf.Abs(halfVecNormY - 0.5f);
        float mu2 = Mathf.Abs(halfVecNormX + 0.5f) + Mathf.Abs(halfVecNormY + 0.5f);
        float mu3 = Mathf.Abs(halfVecNormX - 0.5f) + Mathf.Abs(halfVecNormY + 0.5f);

        sample.scale =  mu0 * data.scale0 +
                        mu1 * data.scale1 +
                        mu2 * data.scale2 +
                        mu3 * data.scale3;


        sample.rot = Quaternion.Normalize(
                        new Quaternion(
                            mu0 * data.rot0.x + mu1 * data.rot1.x + mu2 * data.rot2.x + mu3 * data.rot3.x,
                            mu0 * data.rot0.y + mu1 * data.rot1.y + mu2 * data.rot2.y + mu3 * data.rot3.y,
                            mu0 * data.rot0.z + mu1 * data.rot1.z + mu2 * data.rot2.z + mu3 * data.rot3.z,
                            mu0 * data.rot0.w + mu1 * data.rot1.w + mu2 * data.rot2.w + mu3 * data.rot3.w));

        return sample;
    }


    static bool Inside(Vector2 rectPos, Vector2 RectHalfWidth, Vector2 ptPos)
    {
        Vector2 vec = rectPos - ptPos;

        if (Mathf.Abs(vec.x) < RectHalfWidth.x && Mathf.Abs(vec.y) < RectHalfWidth.y)
            return true;
        else
            return false;
    }
    private void Awake()
    {
        Debug.Assert(singleton == null);
        singleton = this;

        MakeGrid();
    }


    GridData head;
    private void MakeGrid()
    {
        head = new GridData();

        head.halfWidth = new Vector3(50.0f, 50.0f, 50.0f);
        head.pos = Vector3.zero;

        RandomizeData(head);
    }

    void RandomizeData(GridData data)
    {
        data.rot0 = Random.rotation;
        data.rot1 = Random.rotation;
        data.rot2 = Random.rotation;
        data.rot3 = Random.rotation;

        data.scale0 = new Vector3(Random.value, Random.value);
        data.scale1 = new Vector3(Random.value, Random.value);
        data.scale2 = new Vector3(Random.value, Random.value);
        data.scale3 = new Vector3(Random.value, Random.value);

        // recurse randomly?
        if (UnityEngine.Random.value > 0.8f)
        {
            data.subGrid0 = new GridData(); data.subGrid0.pos = new Vector2(data.pos.x + (data.halfWidth.x * 0.25f), data.pos.y + (data.halfWidth.y * 0.25f)); data.subGrid0.halfWidth = data.halfWidth * 0.5f;
            data.subGrid1 = new GridData(); data.subGrid1.pos = new Vector2(data.pos.x - (data.halfWidth.x * 0.25f), data.pos.y + (data.halfWidth.y * 0.25f)); data.subGrid1.halfWidth = data.halfWidth * 0.5f;
            data.subGrid2 = new GridData(); data.subGrid2.pos = new Vector2(data.pos.x + (data.halfWidth.x * 0.25f), data.pos.y - (data.halfWidth.y * 0.25f)); data.subGrid2.halfWidth = data.halfWidth * 0.5f;
            data.subGrid3 = new GridData(); data.subGrid3.pos = new Vector2(data.pos.x - (data.halfWidth.x * 0.25f), data.pos.y - (data.halfWidth.y * 0.25f)); data.subGrid3.halfWidth = data.halfWidth * 0.5f;

            RandomizeData(data.subGrid0);
            RandomizeData(data.subGrid1);
            RandomizeData(data.subGrid2);
            RandomizeData(data.subGrid3);
        }
    }

    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

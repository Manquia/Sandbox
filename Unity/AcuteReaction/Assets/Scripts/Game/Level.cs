using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class Level : MonoBehaviour
{
    public struct LineHistory
    {
        internal GameObject[] gos;
        internal Command cmd;

        public enum Command
        {
            None    = 0,
            Place   = 1,
            Destroy = 2,
            Move    = 3,
        }

        /*
        [StructLayout(LayoutKind.Explicit)]
        public struct Data
        {
            public struct Place
            {
            }
            public struct Destroy
            {
            }
            public struct Move
            {
                public Vector2Int delta;
            }

            [FieldOffset(0)] public Place   place;
            [FieldOffset(0)] public Destroy destroy;
            [FieldOffset(0)] public Move    move;
        }
        Data data;
        */

        public Vector2Int moveDelta;
        public static LineHistory ConstructDefault()
        {
            LineHistory lh;
            lh.cmd = Command.None;
            lh.gos = null;
            lh.moveDelta = Vector2Int.zero;
            return lh;
        }
    }
    public class SetupInstance
    {
        public class DrawingLine
        {
            public Annal<InputData> input;
            public GameObject go;

            static public DrawingLine Construct()
            {
                DrawingLine dl = new DrawingLine
                {
                    input = new Annal<InputData>(8, InputData.Construct()),
                    go = null
                };
                return dl;
            }
        }
        internal Transform levelRoot;
        internal Dictionary<int, DrawingLine> drawingLines = new Dictionary<int, DrawingLine>();

        internal Dictionary<int, GameObject> lines      = new Dictionary<int, GameObject>();
        internal Annal<LineHistory> history             = new Annal<LineHistory>(120, LineHistory.ConstructDefault());

        internal int id = 0;
        internal int GetUniqueLineId()
        {
            return ++id;
        }

    }
    public class LevelInstance
    {
        public struct GridVertex
        {
            [System.Flags]
            public enum Flags
            {
                // Directional
                none = 0,
                up = 1,
                down = 2,
                right = 4,
                left = 8,

                posZ = up,
                negZ = down,
                posX = right,
                negX = left,

                // State
                isStatic = 256,
            }

            public Flags flags;
            public int orderNumber;
        }

        internal GridVertex [,]grid;
    }

    internal SetupInstance setupInstance = new SetupInstance();
    internal LevelInstance levelInstance = new LevelInstance();



    public LevelSettings settings;

    private void Awake()
    {
        transform.position = Vector3.zero;
        CreateLevel();
    }


#region LevelStartup
    void CreateLevel()
    {
        const int height = 50; // @ make dynamic?
        const int width = 50; // @ make dynamic?

        // Create Root of level
        setupInstance.levelRoot = Instantiate(settings.prefabs.levelRoot).transform;
        // Create dots so that players can see them
        {
            var root = setupInstance.levelRoot;
            root.position = Vector3.zero;
            var rot = Quaternion.AngleAxis(90.0f, Vector3.right);

            for (int z = 0; z < height; ++z)
            {
                for (int x = 0; x < width; ++x)
                {
                    var dot = Instantiate(settings.prefabs.dot).transform;
                    dot.SetParent(root);
                    dot.localPosition = new Vector3(x - width / 2, 0, z - height / 2);
                    dot.localRotation = rot;
                }
            }
        }

        // allocate grid
        {
            levelInstance.grid = new LevelInstance.GridVertex[height,width];
        }


    }

    #endregion


    #region Update
    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        
        UpdateInput();
        HandleInput(dt);

        DeveloperCheats();
    }

    private void DeveloperCheats()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
            UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused;
#endif

    }

    void UpdateInput()
    {
        // Remove any touches no longer in use
        {
            var keysToRemove = new List<int>(4);
            foreach (var kvp in setupInstance.drawingLines)
            {
                var phase = kvp.Value.input.Recall(0).phase;
                if (phase == InputData.Phase.Ended || phase == InputData.Phase.Canceled)
                    keysToRemove.Add(kvp.Key);
            }
            foreach (var k in keysToRemove) setupInstance.drawingLines.Remove(k);
        }

        // Add/update touches
        int touchIndex = 0;
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
                var touch = Input.GetTouch(i);
                InputData inputData = new InputData(touch);
                RecordInputDataAtId(inputData, touch.fingerId);

                setupInstance.drawingLines[touchIndex].input.Record(inputData);
                ++touchIndex;
            }
        }

        // Add/update mouse
        {
            InputData inputData;
            // Left Mouse button
            if (Input.GetMouseButtonDown(0))    inputData = new InputData(Input.mousePosition, KeyState.GetPressedKeyState(), InputData.ID_MOUSE);  
            else if (Input.GetMouseButtonUp(0)) inputData = new InputData(Input.mousePosition, KeyState.GetReleasedKeyState(), InputData.ID_MOUSE); 
            else if (Input.GetMouseButton(0))   inputData = new InputData(Input.mousePosition, KeyState.GetDownKeyState(), InputData.ID_MOUSE);     
            else                                inputData = new InputData(Input.mousePosition, KeyState.GetUpKeyState(), InputData.ID_MOUSE);       

            RecordInputDataAtId(inputData, InputData.ID_MOUSE);
        }

    }
    void RecordInputDataAtId(InputData data, int id)
    {
        if (data.phase == InputData.Phase.Begin)
        {
            if (setupInstance.drawingLines.ContainsKey(id) == false)
            {
                var dl = SetupInstance.DrawingLine.Construct();
                setupInstance.drawingLines.Add(id, dl);
            }
            var record = setupInstance.drawingLines[id].input;
            record.Record(data);
        }
        else // The input may have been canceled, so we only add when its the begin phase
        {
            if (setupInstance.drawingLines.ContainsKey(id))
            {
                var record = setupInstance.drawingLines[id].input;
                record.Record(data);
            }
        }
    }


    void HandleInput(float dt)
    {
        // find new touches not claimed
        foreach(var kvp in setupInstance.drawingLines)
        {
            var dl = kvp.Value;
            var annal = dl.input;

            // no changes to the touch
            if (annal.changed == false)
                continue;

            var inputId = kvp.Key;
            // Debug.Log("Handling inputId: " + inputId);
            int frame = Time.frameCount;
            int framesToRun = annal.recordCount;
            if (framesToRun == annal.size) Debug.LogWarning("Input Annal possible loss of input information!!!");

            // Process touch events in the order they were recieved
            for(int i = framesToRun - 1; i >= 0; --i)
            {
                var inputData = annal.Recall(i);

                switch (inputData.phase)
                {
                    case InputData.Phase.Begin:
                        MakeUnsetLine(inputId);
                        BeginUnsetLine(i, dl);
                        break;
                    case InputData.Phase.Update:
                        MoveUnsetLine(i, dl);
                        break;
                    case InputData.Phase.Ended:
                        MoveUnsetLine(i, dl);
                        EndUnsetLine(i, dl);
                        CancelUnsetLine(dl, inputId);
                        i = 0; // Stop processing Input, we are ending the line
                        break;
                    case InputData.Phase.Canceled:
                        CancelUnsetLine(dl, inputId);
                        i = 0; // Stop processing Input, we are canceling the line
                        break;
                    case InputData.Phase.None:
                        Debug.LogWarning("Input phase of type None found! (Input Id: " + inputId + " )");
                        break;
                }
            }
        }


        // Process Cancel drawing
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelUnsetLines();
    }

    void CancelUnsetLines()
    {
        foreach (var kvp in setupInstance.drawingLines)
        {
            CancelUnsetLine(kvp.Value, kvp.Key);
        }
    }
    void CancelUnsetLine(int inputId)
    {
        SetupInstance.DrawingLine dl;
        GameObject go;
        if (setupInstance.drawingLines.TryGetValue(inputId, out dl))
        {
            CancelUnsetLine(dl, inputId);
        }
    }

    void CancelUnsetLine(SetupInstance.DrawingLine dl, int id)
    {
        Debug.Log("CancelUnsetLine");
        // Record to have the Unset line taken out of our records at the begining of next update
        InputData inData = new InputData(Vector2.zero, InputData.Phase.Canceled, id);
        dl.input.Record(inData);
        if(dl.go != null)
        {
            Destroy(dl.go);
            dl.go = null;
        }
    }

    // Place line if possible
    void EndUnsetLine(int recalloffset, SetupInstance.DrawingLine dl)
    {
        Debug.Log("EndUnsetLine");
        GameObject unsetLine = dl.go;

        // Setup line
        GameObject[] placedLines = unsetLine.GetComponent<PlacedLine>().Setup(levelInstance);

        // Record Place History
        LineHistory lh;
        lh.gos = placedLines;
        lh.cmd = LineHistory.Command.Place;
        lh.moveDelta = Vector2Int.zero;
        setupInstance.history.Record(lh);
    }

    void MoveUnsetLine(int recalloffset, SetupInstance.DrawingLine dl)
    {
        Debug.Log("MoveUnsetLine");
        GameObject line = dl.go;
        var rend = line.GetComponent<LineRenderer>();
        var ptCount = rend.positionCount;

        rend.SetPosition(ptCount - 1, Camera.main.ScreenToWorldPoint(dl.input.Recall(recalloffset).pos));

        SnapUnsetLine(line);
    }

    void BeginUnsetLine(int recalloffset, SetupInstance.DrawingLine dl)
    {
        Debug.Log("BeginUnsetLine");
        GameObject line = dl.go;
        var rend = line.GetComponent<LineRenderer>();
        var ptCount = rend.positionCount;

        rend.SetPosition(0, Camera.main.ScreenToWorldPoint(dl.input.Recall(recalloffset).pos));
        rend.SetPosition(1, Camera.main.ScreenToWorldPoint(dl.input.Recall(recalloffset).pos));

        SnapUnsetLine(line);
    }

    GameObject SnapUnsetLine(GameObject unsetLine)
    {
        var rend = unsetLine.GetComponent<LineRenderer>();
        float unsetLineOffsetY = settings.variables.DrawingLineYOffset;

        // snap begin point
        Vector3 pt0 = rend.GetPosition(0);
        pt0 = Snap(pt0, unsetLineOffsetY);
        rend.SetPosition(0, pt0);

        // find direcitonal vector
        Vector3 pt1 = rend.GetPosition(1);
        Vector3 lineVec = pt1 - pt0;
        Vector3 lineVecNorm = lineVec.normalized;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(lineVecNorm.z, lineVecNorm.x) + 180.0f;
        // range: (0,7) 
        int snappedDir = Mathf.FloorToInt((angle / 45.0f) + 0.5f) % 8;
        float snappedAngle = snappedDir * 45.0f;
        Vector3 snappedLineNorm = new Vector3(Mathf.Cos(Mathf.Deg2Rad * snappedAngle), 0, Mathf.Sin(Mathf.Deg2Rad * snappedAngle));

        // project input into snappedLine
        Vector3 snappedLineVec = Vector3.Project(lineVec, snappedLineNorm);

        // snap end point
        pt1 = Snap(pt0 + snappedLineVec, unsetLineOffsetY);
        rend.SetPosition(1, pt1);

        return unsetLine;
    }
    static Vector3 Snap(Vector3 vec, float yOffset)
    {
        return new Vector3(Mathf.Floor(vec.x + 0.5f), yOffset, Mathf.Floor(vec.z + 0.5f));
    }


    GameObject MakeUnsetLine(int inputId)
    {
        var line = Instantiate(settings.prefabs.line);
        setupInstance.drawingLines[inputId].go = line;
        return line;
    }
    GameObject GetUnsetLine(int inputId)
    {
        SetupInstance.DrawingLine dl;
        setupInstance.drawingLines.TryGetValue(inputId, out dl);
        return dl.go;
    }

    #endregion

    #region Helpers
    Vector2 TouchToPos(Touch touch)
    {
        return Camera.main.ScreenToWorldPoint(touch.position);
    }
    #endregion
    // Use this for initialization
    void Start ()
    {
		
	}



}

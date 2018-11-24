using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Level : MonoBehaviour
{
    public enum InputMode
    {
        Menu,
        Setup,
        Run,
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
        internal Dictionary<int, DrawingLine> drawingLines = new Dictionary<int, DrawingLine>();

        internal Dictionary<int, GameObject> lines      = new Dictionary<int, GameObject>();

        internal Annal<LineCommand> history             = new Annal<LineCommand>(120, LineCommand.ConstructDefault());
        internal int totalHistoriesRecorded = 0;
        internal int historyBacktraceOffset = 0;

        internal int id = 0;
        internal int GetUniqueLineId()
        {
            return ++id;
        }

    }
    

    internal SetupInstance setupInstance = new SetupInstance();
    internal LevelInstance levelInstance = new LevelInstance();

    public GameObject levelRoot   { get; private set; }
    public GameObject setupRoot   { get; private set; } 
    public GameObject runtimeRoot { get; private set; }
    public InputMode inputMode { get; private set; }

    public LevelSettings settings;

    private void Awake()
    {
        transform.position = Vector3.zero;
        CreateLevel();
    }


    public const float diagonalOffsetDist = 1.41421356237f; // (2)^.5
    public const float cardinalOffsetDist = 1.0f;
    public int height = 50; // @ make dynamic?
    public int width = 50; // @ make dynamic?

    #region Creation
    void CreateLevel()
    {
        // Create Root of level
        levelRoot = Instantiate(settings.prefabs.levelRoot);
        setupRoot = Instantiate(settings.prefabs.setupRoot);
        setupRoot.transform.SetParent(levelRoot.transform);
        runtimeRoot = Instantiate(settings.prefabs.runtimeRoot);
        runtimeRoot.transform.SetParent(levelRoot.transform);

        // Create dots so that players can see them
        {
            var root = setupRoot.transform;
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
            levelInstance.grid = new GameVertex[height,width];
        }


    }


    public struct LineVec
    {
        public int x;
        public int y;
        public int dir;
    }
    
    // Takes a level Instance and creates its runtime equivalent
    void InvokeLevel(LevelInstance inst)
    {
        List<HashSet<LineVec>> clusters = new List<HashSet<LineVec>>(4);
        // Generate index lists for each cluster of lines
        {
            // Get a new traversal index
            char traversalIndex = LevelInstance.GetNewTraversalIndex();
            // on looping around we don't use 0 since it is our reset value
            if (traversalIndex == 0) traversalIndex = LevelInstance.GetNewTraversalIndex();

            // Is index grid valid?
            if(LevelInstance.gridIndex == null ||
               LevelInstance.gridIndex.GetLength(0) < inst.grid.GetLength(0) ||
               LevelInstance.gridIndex.GetLength(1) < inst.grid.GetLength(1) ||
               traversalIndex == 1) // looped around, reset gridIndexes to 0 always
            {
                LevelInstance.gridIndex = new char[inst.grid.GetLength(0), inst.grid.GetLength(1)];
            }

            Vector2Int max = Vector2Int.zero;
            max.y = inst.grid.GetLength(0);
            max.x = inst.grid.GetLength(1);

            int clusterIndex = -1;
            Queue<Vector2Int> SearchList = new Queue<Vector2Int>(16);
            for (int y = 0; y < max.y; ++y)
            {
                for(int x = 0; x < max.x; ++x)
                {
                    // have we visited this space before?
                    if (LevelInstance.gridIndex[y, x] == traversalIndex)
                        continue;

                    // Any out bound connections means this is a part of a new cluster
                    if (inst.grid[y, x].lines.AllToOne() != GameVertex.Direction.None)
                    {
                        ++clusterIndex;
                        if (clusters.Count == clusterIndex)
                            clusters.Add(new HashSet<LineVec>());

                        // Add point to search through
                        SearchList.Enqueue(new Vector2Int(x, y));
                    }

                    // traverse cluster
                    while (SearchList.Count > 0)
                    {
                        // Get next item on the
                        Vector2Int look = SearchList.Dequeue();

                        // already looked at item?
                        if (LevelInstance.gridIndex[look.y, look.x] == traversalIndex)
                            continue;

                        // Mark node as looked at
                        LevelInstance.gridIndex[look.y, look.x] = traversalIndex;

                        // Get inbound Connections and enqueue them to add them to the cluster
                        { 
                            Vector2Int c = Vector2Int.zero;
                            c.y=y  ; c.x=x-1; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.E )) SearchList.Enqueue(c);
                            c.y=y-1; c.x=x-1; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.NE)) SearchList.Enqueue(c);
                            c.y=y-1; c.x=x  ; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.N )) SearchList.Enqueue(c);
                            c.y=y-1; c.x=x+1; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.NW)) SearchList.Enqueue(c);
                            c.y=y  ; c.x=x+1; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.W )) SearchList.Enqueue(c);
                            c.y=y+1; c.x=x+1; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.SW)) SearchList.Enqueue(c);
                            c.y=y+1; c.x=x  ; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.S )) SearchList.Enqueue(c);
                            c.y=y+1; c.x=x-1; if (ValidPoint(c, max, inst.grid, traversalIndex, GameVertex.Direction.SE)) SearchList.Enqueue(c);
                        }

                        // Add outbound connections from the GridVertex
                        {
                            var cluster = clusters[clusterIndex];
                            GameVertex.Direction outDirs = inst.grid[look.y, look.x].lines.AllToOne();
                            LineVec lv;
                            lv.y = y;
                            lv.x = x;

                            // Add connections to the cluster
                            lv.dir = 0; if ((outDirs & GameVertex.Direction.W ) == GameVertex.Direction.W  && !cluster.Contains(lv)) cluster.Add(lv);
                            lv.dir = 1; if ((outDirs & GameVertex.Direction.SW) == GameVertex.Direction.SW && !cluster.Contains(lv)) cluster.Add(lv);
                            lv.dir = 2; if ((outDirs & GameVertex.Direction.S ) == GameVertex.Direction.S  && !cluster.Contains(lv)) cluster.Add(lv);
                            lv.dir = 3; if ((outDirs & GameVertex.Direction.SE) == GameVertex.Direction.SE && !cluster.Contains(lv)) cluster.Add(lv);
                            lv.dir = 4; if ((outDirs & GameVertex.Direction.E ) == GameVertex.Direction.E  && !cluster.Contains(lv)) cluster.Add(lv);
                            lv.dir = 5; if ((outDirs & GameVertex.Direction.NE) == GameVertex.Direction.NE && !cluster.Contains(lv)) cluster.Add(lv);
                            lv.dir = 6; if ((outDirs & GameVertex.Direction.N ) == GameVertex.Direction.N  && !cluster.Contains(lv)) cluster.Add(lv);
                            lv.dir = 7; if ((outDirs & GameVertex.Direction.NW) == GameVertex.Direction.NW && !cluster.Contains(lv)) cluster.Add(lv);
                        }
                    }
                }
            }
        }

        // resolve the clusters into blocks
        foreach(var cluster in clusters)
        {
            foreach(var line in cluster)
            {
                // Make cluster root

                // Create lines with behaviors which are indexable from  LevelRUNTIME or something!
                {
                    int xLineEnd;
                    int yLineEnd;
                    ARUtil.SnapToOffset(line.dir, out yLineEnd, out xLineEnd);
                }






            }
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
        if (inputMode == InputMode.Setup)
        {
            // find new touches not claimed
            foreach (var kvp in setupInstance.drawingLines)
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
                for (int i = framesToRun - 1; i >= 0; --i)
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

            // undo/redo
            {
                int count = 1;
                bool shiftHeld = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
#if UNITY_EDITOR
                bool ctrlHeld = true;
#else
            bool ctrlHeld = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
#endif

                if (shiftHeld)
                    count = 5;

                // Undo
                if (ctrlHeld && Input.GetKeyDown(KeyCode.Z))
                    UndoLineCommand(count);

                // Redo
                if (ctrlHeld && Input.GetKeyDown(KeyCode.Y))
                    RedoLineCommand(count);
            }
        }

        if(inputMode == InputMode.Run)
        {
            // Handle Input
        }
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
        //Debug.Log("EndUnsetLine");
        GameObject unsetLine = dl.go;

        // validate line
        {
            var rend = unsetLine.GetComponent<LineRenderer>();
            var pt0 = rend.GetPosition(0);
            var pt1 = rend.GetPosition(1);

            if ((pt0 - pt1).magnitude < 0.001f) // line does not exist
            {
                Debug.Log("Drawn line unsetLine was malformed, rejecting Set");
                return;
            }
        }

        // Setup line
        LineCommand lc = unsetLine.GetComponent<UnsetLine>().Set(this, LineCommand.Command.Place);

        // Record Place History
        RecordLineCommand(lc);
    }

    void MoveUnsetLine(int recalloffset, SetupInstance.DrawingLine dl)
    {
        //Debug.Log("MoveUnsetLine");
        GameObject line = dl.go;
        var rend = line.GetComponent<LineRenderer>();
        var ptCount = rend.positionCount;

        rend.SetPosition(ptCount - 1, Camera.main.ScreenToWorldPoint(dl.input.Recall(recalloffset).pos));

        SnapUnsetLine(line);
    }

    void BeginUnsetLine(int recalloffset, SetupInstance.DrawingLine dl)
    {
        //Debug.Log("BeginUnsetLine");
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
        var line = Instantiate(settings.prefabs.unsetLine);
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

    #region Run
    void StartRunLevel()
    {
        // if level is being run, Reset it!

        // Build Level from setupInstance

        // start Entity component creation/simulation

        // Enter Input mode for running
        inputMode = InputMode.Run;
    }

    void StopRunLevel()
    {
        // If level is being run, erase it, stop ECS creation/simulation


        // Enter Inputmode for setup
        inputMode = InputMode.Setup;
    }

    #endregion



    #region Helpers

    // record a single line command
    void RecordLineCommand(LineCommand lc)
    {
        Debug.Log("RecordLineCommand");

        // Forget history to remove backtrace
        setupInstance.history.Forget(setupInstance.historyBacktraceOffset);
        setupInstance.totalHistoriesRecorded -= setupInstance.historyBacktraceOffset;
        setupInstance.historyBacktraceOffset = 0;

        // record command at front of annal
        setupInstance.history.Record(lc);
        ++setupInstance.totalHistoriesRecorded;
    }
    // count: number of commands to undo
    void UndoLineCommand(int count)
    {
        for (int i = 0; i < count; ++i)
            UndoLineCommand();
    }
    void UndoLineCommand()
    {
        Debug.Log("UndoLineCommmand");
        // If we have recorded the history AND have a record going back that far.
        if (setupInstance.totalHistoriesRecorded > setupInstance.historyBacktraceOffset &&
            setupInstance.history.size > setupInstance.historyBacktraceOffset)
        {
            // execute inverted co*mmand up to count times
            LineCommand lc = setupInstance.history.Recall(setupInstance.historyBacktraceOffset);
            lc.Invert();
            ApplyCommand(ref lc);

            // pull back historyOffset+1
            ++setupInstance.historyBacktraceOffset;
        }
    }

    // count: number of Commands to Redo
    void RedoLineCommand(int count)
    {
        for (int i = 0; i < count; ++i)
            RedoLineCommand();
    }
    void RedoLineCommand()
    {
        Debug.Log("RedoLineCommand");
        // have some commands to redo
        if (setupInstance.historyBacktraceOffset > 0)
        {
            // move offset up 1
            --setupInstance.historyBacktraceOffset;

            // execute linecommands in annal up to count times
            LineCommand lc = setupInstance.history.Recall(setupInstance.historyBacktraceOffset);
            ApplyCommand(ref lc);
        }
    }

    void ApplyCommand(ref LineCommand lineCommand)
    {
        foreach (var go in lineCommand.gos)
        {
            // @TODO @FIX
            if (go != null)
            {
                var setLine = go.GetComponent<SetLine>();
                setLine.RunCommand(this, lineCommand);
            }
            else
            {
                Debug.LogWarning("Saving null objects because they can't be added to grid or something, fix this!");
            }
            
        }
    }

    Vector2 TouchToPos(Touch touch)
    {
        return Camera.main.ScreenToWorldPoint(touch.position);
    }
    static bool ValidPoint(Vector2Int c, Vector2Int max, GameVertex[,] grid, char traversalIndex, GameVertex.Direction mask)
    {
        return c.y > 0 && c.y < max.y && c.x > 0 && c.x < max.x &&              // Bounds
               LevelInstance.gridIndex[c.y,c.x] != traversalIndex &&            // traversal Index
               grid[c.y,c.x].lines.AllToOne(mask) != GameVertex.Direction.None; // direction maks
    }
    #endregion

}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
    internal SetupInstance instance = new SetupInstance();



    public LevelSettings settings;

    private void Awake()
    {
        CreateLevel();
    }


#region LevelStartup
    void CreateLevel()
    {
        // Create Root of level
        instance.levelRoot = Instantiate(settings.prefabs.levelRoot).transform;

        // Create dots so that players can see them
        {
            var root = instance.levelRoot;
            var rot = Quaternion.AngleAxis(90.0f, Vector3.right);

            const int height = 50; // @ make dynamic?
            const int width = 50; // @ make dynamic?
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
    }

    #endregion


    #region Update
    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        
        UpdateInput();
        HandleInput(dt);
    }

    void UpdateInput()
    {
        // Remove any touches no longer in use
        {
            var keysToRemove = new List<int>(4);
            foreach (var kvp in instance.drawingLines)
            {
                var phase = kvp.Value.input.Recall(0).phase;
                if (phase == InputData.Phase.Ended || phase == InputData.Phase.Canceled)
                    keysToRemove.Add(kvp.Key);
            }
            foreach (var k in keysToRemove) instance.drawingLines.Remove(k);
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

                instance.drawingLines[touchIndex].input.Record(inputData);
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
            if (instance.drawingLines.ContainsKey(id) == false)
            {
                var dl = SetupInstance.DrawingLine.Construct();
                instance.drawingLines.Add(id, dl);
            }
            var record = instance.drawingLines[id].input;
            record.Record(data);
        }
        else // The input may have been canceled, so we only add when its the begin phase
        {
            if (instance.drawingLines.ContainsKey(id))
            {
                var record = instance.drawingLines[id].input;
                record.Record(data);
            }
        }
    }


    void HandleInput(float dt)
    {
        // find new touches not claimed
        foreach(var kvp in instance.drawingLines)
        {
            var annal = kvp.Value.input;

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
                        BeginUnsetLine(inputId);
                        break;
                    case InputData.Phase.Update:
                        MoveUnsetLine(inputId);
                        break;
                    case InputData.Phase.Ended:
                        EndUnsetLine(inputId);
                        i = 0; // Stop processing Input, we are ending the line
                        break;
                    case InputData.Phase.Canceled:
                        CancelUnsetLine(inputId);
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
        foreach (var kvp in instance.drawingLines)
        {
            CancelUnsetLine(kvp.Value, kvp.Key);
        }
    }
    void CancelUnsetLine(int inputId)
    {
        SetupInstance.DrawingLine dl;
        GameObject go;
        if (instance.drawingLines.TryGetValue(inputId, out dl))
        {
            CancelUnsetLine(dl, inputId);
        }
    }
    void CancelUnsetLine(SetupInstance.DrawingLine dl, int id)
    {
        // Record to have the Unset line taken out of our records at the begining of next update
        InputData inData = new InputData(Vector2.zero, InputData.Phase.Canceled, id);
        dl.input.Record(inData);
        if(dl.go != null)
        {
            Destroy(dl.go);
            dl.go = null;
        }
    }

    void EndUnsetLine(int inputId)
    {
        PlaceUnsetLine(inputId);
        CancelUnsetLine(inputId);
    }

    void PlaceUnsetLine(int inputId)
    {
        GameObject unsetLine = SnapUnsetLine(inputId);

        // Setup line
        GameObject[] placedLines = unsetLine.GetComponent<PlacedLine>().Setup(instance);


        // Record Place History
        LineHistory lh;
        lh.gos = placedLines;
        lh.cmd = LineHistory.Command.Place;
        lh.moveDelta = Vector2Int.zero;
        instance.history.Record(lh);

        // Destroy the Unset Line
        CancelUnsetLine(inputId);
    }
    void MoveUnsetLine(int inputId)
    {
        SnapUnsetLine(inputId);
    }

    void BeginUnsetLine(int inputId)
    {
        GameObject line = MakeUnsetLine(inputId);
        SnapUnsetLine(inputId);

    }

    GameObject SnapUnsetLine(int inputId)
    {
        var unsetLine = GetUnsetLine(inputId);

        return unsetLine;
    }


    GameObject MakeUnsetLine(int inputId)
    {
        var line = Instantiate(settings.prefabs.line);
        instance.drawingLines[inputId].go = line;
        return line;
    }
    GameObject GetUnsetLine(int inputId)
    {
        SetupInstance.DrawingLine dl;
        instance.drawingLines.TryGetValue(inputId, out dl);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Level : MonoBehaviour
{
    public struct Instance
    {
        internal Transform LevelRoot;
        internal Dictionary<int, Annal<Touch>> Touches;
    }
    internal Instance instance;


    public LevelSettings settings;

    private void Awake()
    {
        CreateLevel();
    }


#region LevelStartup
    void CreateLevel()
    {
        // Create Root of level
        instance.LevelRoot = Instantiate(settings.prefabs.LevelRoot).transform;

        // Create dots so that players can see them
        {
            var root = instance.LevelRoot;

            const int height = 50; // @ make dynamic?
            const int width = 50; // @ make dynamic?
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var dot = Instantiate(settings.prefabs.VisualDot).transform;
                    dot.SetParent(root);
                    dot.localPosition = new Vector3(x - width / 2, y - height / 2, 0);
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
        // Update Input
        UpdateInput();

        // DrawLine;
        DrawLines(dt);

    }

    private void DrawLines(float dt)
    {

    }

    void UpdateInput()
    {
        // Remove any touches no longer in use
        foreach(var kvp in instance.Touches)
        {
            var phase = kvp.Value.Recall(0).phase;
            if(phase == TouchPhase.Canceled || phase == TouchPhase.Ended)
            {
                instance.Touches.Remove(kvp.Key);
            }
        }

        // Add/update touches
        int touchIndex = 0;
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
                var touch = Input.GetTouch(i);

                if(instance.Touches.ContainsKey(touch.fingerId))
                {
                    var record = instance.Touches[touch.fingerId];
                    record.Record(touch);
                }
                else
                {
                    instance.Touches.Add(touch.fingerId, new Annal<Touch>(8, touch));
                }

                instance.Touches[touchIndex].Record(touch);
                ++touchIndex;
            }
        }
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

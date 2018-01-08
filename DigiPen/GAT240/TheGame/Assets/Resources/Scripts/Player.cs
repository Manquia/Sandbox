using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

struct UseEvent
{
}

public class Player : FFComponent
{
    public bool m_InputActive;

#region Data

    public struct PlayerController
    {
        public struct Input
        {
            interface SourceBase
            {
                bool InEnd();
                bool InProcess();
                bool InIdle();
                bool InBegin();
            }
            public class Source<Type> : SourceBase
            {
                public delegate Type del_get();
                // Retrieves a new value
                private del_get mGetter;
                private const int valuesBufferSize = 16;
                private Type[] historyValues = new Type[valuesBufferSize];
                private int valueIndex = 0;
                public del_get Getter { get { return mGetter; } }
                public Source(del_get getter)
                {
                    mGetter = getter;
                }
                public Type RetrieveNewValue()
                { 
                    ++valueIndex;
                    valueIndex = valueIndex % valuesBufferSize;
                    historyValues[valueIndex] = mGetter();
                    return historyValues[valueIndex];
                }
                // Retrieves a new Input value, modifies history
                static public implicit operator Type(Source<Type> src)
                {
                    return src.RetrieveNewValue();
                }

                public Type[] GetHistory
                {
                    get
                    {
                        Type[] retHistory = new Type[valuesBufferSize];

                        // copy values in reverse order into buffer
                        for(int i = 0; i < valuesBufferSize; ++i)
                        {
                            int srcIndex = valueIndex - i;
                            if (srcIndex < 0) srcIndex += valuesBufferSize;
                            retHistory[i] = historyValues[srcIndex];
                        }
                        return retHistory;
                    }
                }
                // get a previously retrieved input value, does not modify history
                public Type GetValue(int prevValueIndex)
                {
                    int srcIndex = valueIndex - prevValueIndex;
                    if (srcIndex < 0) srcIndex += valuesBufferSize;
                    return historyValues[srcIndex];

                }

                /// Common C# why you have to do this!>!>>!!LKHDSP:I 


                public bool InEnd()
                {
                    Type prevValue = GetValue(1);
                    Type curValue = GetValue(0);
                    //Type defaultValue = new Type();
                
                    // at end?
                    if (curValue == prevValue)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                public bool InProcess()
                {
                    throw new NotImplementedException();
                }
                public bool InIdle()
                {
                    throw new NotImplementedException();
                }
                public bool InBegin()
                {
                    throw new NotImplementedException();
                }
            }
            public Source<bool>      digital;
            public Source<float>     analog;
            public Source<Vector2>   analog2D;
            public Source<Vector3>   analog3D;
        }

        public Input forward;
        public Input backward;
        public Input left;
        public Input right;
    }
    PlayerController controller;
    #endregion Data


    #region UnityEvents
    // Use this for initialization
    void Start ()
    {
        Activate();
	}
    private void OnDestroy()
    {
        Deactivate();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!m_InputActive)
            return;

        // Update Input Getters
        {
            // default to keyboard input
            {
                controller.forward.digital = new FFRef<bool>( () => Input.GetKey())

            }
        }

        // Handle Input
        {


        }

    }
    #endregion UnityEvents


    #region Helpers
    void Activate()
    {

    }
    void Deactivate()
    {

    }
    #endregion Helpers


    #region CustomEvents


    #endregion CustomEvents

}

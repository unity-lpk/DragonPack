/***************************************************
File:           LPK_Counter.cs
Authors:        Christopher Onorati
Last Updated:   10/8/2019
Last Version:   2019.1.4

Description:
  This component can be added to any object to have
  it track a value relevant to the game. That value
  might be a collectible, currency, score, etc.
  This component can be connected to a display to
  show the value and forward events when updating
  or upon reaching a specified threshold.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_Counter
* DESCRIPTION : Class which acts as a counter.
**/
public class LPK_Counter : LPK_Component
{
    /************************************************************************************/

    public enum LPK_CounterThresholdMode
    {
        EQUAL_TO,
        NOT_EQUAL_TO,
        LESS_THAN,
        LESS_EQUAL,
        GREATER_THAN,
        GREATER_EQUAL,
        NONE,
    };

    public enum LPK_CounterModifyMode
    {
        SET,
        ADD,
        MULTIPLY,
        DIVIDE,
        SUBTRACT,
    }

    /************************************************************************************/

    public int m_iValue = 0;
    public int m_iMinValue = -10;
    public int m_iMaxValue = 10;

    public bool m_bUpdateDisplayOnStart = true;

    [Tooltip("What relation between the counter value and threshold value should trigger an event.")]
    [Rename("Threshold Mode")]
    public LPK_CounterThresholdMode m_eThresholdMode = LPK_CounterThresholdMode.NONE;

    public int m_iThresholdValue = 0;

    //Sending info below

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the counter's value is past a certain threshold.")]
    public LPK_EventSendingInfo m_CounterThresholdEvent;

    [Tooltip("Event sent when the counter's value is increased.")]
    public LPK_EventSendingInfo m_CounterIncreasedEvent;

    [Tooltip("Event sent when the counter's value is decreased.")]
    public LPK_EventSendingInfo m_CounterDecreasedEvent;

    [Tooltip("Event sent when the counter's value hits is maximum value.")]
    public LPK_EventSendingInfo m_CounterHitMaxEvent;

    [Tooltip("Event sent when the counter's value hits its minimum value.")]
    public LPK_EventSendingInfo m_CounterHitMinEvent;

    [Header("Displays")]

    [Tooltip("List of LPK Display components to update.")]
    public LPK_DisplayObject[] m_DisplayComponents;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up initial value and event connections.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_bUpdateDisplayOnStart)
            StartCoroutine(DelayedStart());
    }

    /**
    * FUNCTION NAME: DelayedStart
    * DESCRIPTION  : Update the display object(s) once everything is initialized.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator DelayedStart()
    {
        yield return null;

        UpdateDisplay();
    }

    /**
    * FUNCTION NAME: OnEvent
    * DESCRIPTION  : Updates the counter value.
    * INPUTS       : _data - modify value.
    * OUTPUTS      : None
    **/
    public void UpdateCounter(LPK_CounterModifyMode _mode, int _change)
    {
        int prevValue = m_iValue;

        if (_mode == LPK_CounterModifyMode.SET)
            m_iValue = _change;
        else if(_mode == LPK_CounterModifyMode.ADD)
          m_iValue += _change;
        else if (_mode == LPK_CounterModifyMode.SUBTRACT)
            m_iValue -= _change;
        else if (_mode == LPK_CounterModifyMode.MULTIPLY)
            m_iValue *= _change;
        else if (_mode == LPK_CounterModifyMode.DIVIDE)
            m_iValue /= _change;

        //Clamp the incoming counter value
        m_iValue = Mathf.Clamp(m_iValue, m_iMinValue, m_iMaxValue);

        //Increase/Decrease events.
        if (m_iValue > prevValue)
            DispatchIncreaseEvent();
        else if (m_iValue < prevValue)
            DispatchDecreaseEvent();

        //Min/max events
        if(m_iValue >= m_iMaxValue)
            DispatchHitMaxEvent();
        else if (m_iValue <= m_iMinValue)
            DispatchHitMinEvent();
                
        UpdateDisplay();

        //Dispatch threshold event
        if (m_eThresholdMode == LPK_CounterThresholdMode.EQUAL_TO && m_iValue == m_iThresholdValue)
            DispatchThresholdEvent();
        else if (m_eThresholdMode == LPK_CounterThresholdMode.NOT_EQUAL_TO && m_iValue != m_iThresholdValue)
            DispatchThresholdEvent();
        else if (m_eThresholdMode == LPK_CounterThresholdMode.GREATER_THAN && m_iValue > m_iThresholdValue)
            DispatchThresholdEvent();
        else if (m_eThresholdMode == LPK_CounterThresholdMode.LESS_THAN && m_iValue < m_iThresholdValue)
            DispatchThresholdEvent();
        else if (m_eThresholdMode == LPK_CounterThresholdMode.GREATER_EQUAL && m_iValue >= m_iThresholdValue)
            DispatchThresholdEvent();
        else if (m_eThresholdMode == LPK_CounterThresholdMode.LESS_EQUAL && m_iValue <= m_iThresholdValue)
            DispatchThresholdEvent();
        
        if(m_bPrintDebug)
            LPK_PrintDebug(this, "New Counter Value: " + m_iValue);
    }

    /**
    * FUNCTION NAME: UpdateDisplay
    * DESCRIPTION  : Updates the displays of all connected game objects.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void UpdateDisplay()
    {
        for(int i = 0; i < m_DisplayComponents.Length; i++)
        {
            if(m_DisplayComponents[i])
                m_DisplayComponents[i].UpdateDisplay(m_iValue, m_iMaxValue);
        }
    }

    /**
    * FUNCTION NAME: DispatchThresholdEvent
    * DESCRIPTION  : Sends threshold event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchThresholdEvent()
    {
        if(m_CounterThresholdEvent != null && m_CounterThresholdEvent != null)
        {
            if(m_CounterThresholdEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_CounterThresholdEvent.m_Event.Dispatch(null);
            else if(m_CounterThresholdEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_CounterThresholdEvent.m_Event.Dispatch(gameObject);
            else if (m_CounterThresholdEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_CounterThresholdEvent.m_Event.Dispatch(gameObject, m_CounterThresholdEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_CounterThresholdEvent, this, "Counter Threshold Reached");
        }
    }

    /**
    * FUNCTION NAME: DispatchIncreaseEvent
    * DESCRIPTION  : Sends counter increased event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchIncreaseEvent()
    {
        if(m_CounterIncreasedEvent != null && m_CounterIncreasedEvent.m_Event != null)
        {
            if(m_CounterIncreasedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_CounterIncreasedEvent.m_Event.Dispatch(null);
            else if(m_CounterIncreasedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_CounterIncreasedEvent.m_Event.Dispatch(gameObject);
            else if (m_CounterIncreasedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_CounterIncreasedEvent.m_Event.Dispatch(gameObject, m_CounterIncreasedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_CounterIncreasedEvent, this, "Counter Increased");
        }
    }

    /**
    * FUNCTION NAME: DispatchDecreaseEvent
    * DESCRIPTION  : Sends counter decreased event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchDecreaseEvent()
    {
        if(m_CounterDecreasedEvent != null && m_CounterDecreasedEvent.m_Event != null)
        {
            if(m_CounterDecreasedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_CounterDecreasedEvent.m_Event.Dispatch(null);
            else if(m_CounterDecreasedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_CounterDecreasedEvent.m_Event.Dispatch(gameObject);
            else if (m_CounterDecreasedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_CounterDecreasedEvent.m_Event.Dispatch(gameObject, m_CounterDecreasedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_CounterDecreasedEvent, this, "Counter Decreased");
        }
    }

    /**
    * FUNCTION NAME: DispatchHitMaxEvent
    * DESCRIPTION  : Sends counter hit max event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchHitMaxEvent()
    {
        if(m_CounterHitMaxEvent != null && m_CounterHitMaxEvent.m_Event != null)
        {
            if(m_CounterHitMaxEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_CounterHitMaxEvent.m_Event.Dispatch(null);
            else if(m_CounterHitMaxEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_CounterHitMaxEvent.m_Event.Dispatch(gameObject);
            else if (m_CounterHitMaxEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_CounterHitMaxEvent.m_Event.Dispatch(gameObject, m_CounterHitMaxEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_CounterHitMaxEvent, this, "Counter Hit Max Value");
        }
    }

    /**
    * FUNCTION NAME: DispatchHitMinEvent
    * DESCRIPTION  : Sends counter hit max event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchHitMinEvent()
    {
        if(m_CounterHitMinEvent != null && m_CounterHitMinEvent.m_Event != null)
        {
            if(m_CounterHitMinEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_CounterHitMinEvent.m_Event.Dispatch(null);
            else if(m_CounterHitMinEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_CounterHitMinEvent.m_Event.Dispatch(gameObject);
            else if (m_CounterHitMinEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_CounterHitMinEvent.m_Event.Dispatch(gameObject, m_CounterHitMinEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_CounterHitMinEvent, this, "Counter Hit Min Value");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_Counter))]
public class LPK_CounterEditor : Editor
{
    SerializedProperty thresholdMode;

    SerializedProperty counterThresholdEvent;
    SerializedProperty counterIncreasedReceivers;
    SerializedProperty counterDecreasedReceivers;
    SerializedProperty m_CounterHitMaxEvent;
    SerializedProperty m_CounterHitMinEvent;

    SerializedProperty m_DisplayComponents;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        thresholdMode = serializedObject.FindProperty("m_eThresholdMode");

        counterThresholdEvent = serializedObject.FindProperty("m_CounterThresholdEvent");
        counterIncreasedReceivers = serializedObject.FindProperty("m_CounterIncreasedEvent");
        counterDecreasedReceivers = serializedObject.FindProperty("m_CounterDecreasedEvent");
        m_CounterHitMaxEvent = serializedObject.FindProperty("m_CounterHitMaxEvent");
        m_CounterHitMinEvent = serializedObject.FindProperty("m_CounterHitMinEvent");

        m_DisplayComponents = serializedObject.FindProperty("m_DisplayComponents");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_Counter owner = (LPK_Counter)target;

        LPK_Counter editorOwner = owner.GetComponent<LPK_Counter>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_Counter)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_Counter), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_Counter");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_iValue = EditorGUILayout.IntField(new GUIContent("Value", "What value to start the counter at."), owner.m_iValue);
        owner.m_iMinValue = EditorGUILayout.IntField(new GUIContent("Min Value", "Minimum value permitted for the counter."), owner.m_iMinValue);
        owner.m_iMaxValue = EditorGUILayout.IntField(new GUIContent("Max Value", "Maximum value permitted for the counter."), owner.m_iMaxValue);

        owner.m_bUpdateDisplayOnStart = EditorGUILayout.Toggle(new GUIContent("Update Display On Start", "Update display objects when this component is activated."), owner.m_bUpdateDisplayOnStart);

        EditorGUILayout.PropertyField(thresholdMode, true);
        owner.m_iThresholdValue = EditorGUILayout.IntField(new GUIContent("Threshold Value", "What value should be used as a threshold for sending an event."), owner.m_iThresholdValue);

        //Events
        EditorGUILayout.PropertyField(counterThresholdEvent, true);
        EditorGUILayout.PropertyField(counterIncreasedReceivers, true);
        EditorGUILayout.PropertyField(counterDecreasedReceivers, true);
        EditorGUILayout.PropertyField(m_CounterHitMaxEvent, true);
        EditorGUILayout.PropertyField(m_CounterHitMinEvent, true);

        //Displays
        LPK_EditorArrayDraw.DrawArray(m_DisplayComponents, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNITY_EDITOR

}   //LPK

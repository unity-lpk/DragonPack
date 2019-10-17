/***************************************************
File:           LPK_Timer.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be added to an object to keep track
  of time starting when the object gets created. The timer 
  will tick up and send out an event when reaching the end.

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
* CLASS NAME  : LPK_Timer
* DESCRIPTION : Class which manages basic timer functionality.
**/
public class LPK_Timer : LPK_Component
{
    /************************************************************************************/

    public enum LPK_TimerPolicy
    {
        RESET,
        STOP,
    };

    public enum LPK_CountType
    {
        COUNTUP,
        COUNTDOWN,
    }

    /************************************************************************************/

    [Tooltip("How to behave when timer reaches MaxTime")]
    [Rename("Timer Policy")]
    public LPK_TimerPolicy m_eTimerPolicy = LPK_TimerPolicy.STOP;

    [Tooltip("Set to determine if the timer should count up or down.")]
    [Rename("Timer Type")]
    public LPK_CountType m_eCountType = LPK_CountType.COUNTUP;

    public float m_flEndTime = 2.0f;
    public float m_flVariance = 0.0f;
    public float m_flStartDelay = 0.0f;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the timer has reached its goal value.")]
    public LPK_EventSendingInfo m_TimerCompletedEvent;

    [Header("Displays")]

    [Tooltip("List of LPK Display components to update.")]
    public LPK_DisplayObject[] m_DisplayComponents;

    /************************************************************************************/

    //Internal timer
    float m_flCurrentTime = 0.0f;
  
    //Whether the timer is currently paused
    bool m_bPaused = true;

    //Internal goal timer.
    float m_flCurrentGoalTime = 0.0f;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Applies initial delay to timer if appropriate.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        SetTime();

        StartCoroutine(DelayTimer());
	}

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manages behavior of the timer.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if (m_bPaused)
            return;

        if (m_eCountType == LPK_CountType.COUNTUP)
            m_flCurrentTime += Time.deltaTime;
        else if (m_eCountType == LPK_CountType.COUNTDOWN)
            m_flCurrentTime -= Time.deltaTime;

        UpdateDisplay();

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Timer: " + m_flCurrentTime);


        if (m_eCountType == LPK_CountType.COUNTUP && m_flCurrentTime >= m_flCurrentGoalTime)
            CountUp();
        else if (m_eCountType == LPK_CountType.COUNTDOWN && m_flCurrentTime <= 0)
            CountDown();
    }

    /**
    * FUNCTION NAME: CountUp
    * DESCRIPTION  : Detects when timer has finished counting up.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CountUp()
    {
        //Send out event.
        DispatchTimerCompletedEvent();

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Timer Completed");

        if (m_eTimerPolicy == LPK_TimerPolicy.RESET)
            SetTime();
        else
            m_bPaused = true;
    }

    /**
    * FUNCTION NAME: CountDown
    * DESCRIPTION  : Detects when timer has finished counting down.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CountDown()
    {
        //Send out event.
        DispatchTimerCompletedEvent();

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Timer Completed");

        if (m_eTimerPolicy == LPK_TimerPolicy.RESET)
            SetTime();
        else
            m_bPaused = true;
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
                m_DisplayComponents[i].UpdateDisplay(m_flCurrentTime, m_flEndTime);
        }
    }

    /**
    * FUNCTION NAME: SetTime
    * DESCRIPTION  : Sets the timer to a random goal within a specified range.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetTime()
    {
        if (m_eCountType == LPK_CountType.COUNTUP)
        {   
            m_flCurrentTime = 0.0f;
            m_flCurrentGoalTime = m_flEndTime + Random.Range(-m_flVariance, m_flVariance);
        }
        else if (m_eCountType == LPK_CountType.COUNTDOWN)
            m_flCurrentTime = m_flEndTime + Random.Range(-m_flVariance, m_flVariance);
    }

    /**
    * FUNCTION NAME: DelayTimer
    * DESCRIPTION  : Forces initial delay before timer activates.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator DelayTimer ()
    {
        yield return new WaitForSeconds(m_flStartDelay);
        m_bPaused = false;
	}

    /**
    * FUNCTION NAME: DispatchTimerCompletedEvent
    * DESCRIPTION  : Dispatches event when the timer hits its goal.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchTimerCompletedEvent()
    {
        if(m_TimerCompletedEvent != null && m_TimerCompletedEvent.m_Event != null)
        {
            if(m_TimerCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_TimerCompletedEvent.m_Event.Dispatch(null);
            else if(m_TimerCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_TimerCompletedEvent.m_Event.Dispatch(gameObject);
            else if (m_TimerCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_TimerCompletedEvent.m_Event.Dispatch(gameObject, m_TimerCompletedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Timer Compelted event dispatched");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_Timer))]
public class LPK_TimerEditor : Editor
{
    SerializedProperty timerPolicy;
    SerializedProperty countType;

    SerializedProperty timerCompletedReceiver;

    SerializedProperty m_DisplayComponents;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        timerPolicy = serializedObject.FindProperty("m_eTimerPolicy");
        countType = serializedObject.FindProperty("m_eCountType");

        timerCompletedReceiver = serializedObject.FindProperty("m_TimerCompletedEvent");

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
        LPK_Timer owner = (LPK_Timer)target;

        LPK_Timer editorOwner = owner.GetComponent<LPK_Timer>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_Timer)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_Timer), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_Timer");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(timerPolicy, true);
        EditorGUILayout.PropertyField(countType, true);
        owner.m_flEndTime = EditorGUILayout.FloatField(new GUIContent("Duration", "Maximum time (in seconds) should the timer track. Will send a LPK_TimerCompleted event when done."), owner.m_flEndTime);
        owner.m_flVariance = EditorGUILayout.FloatField(new GUIContent("Variance", "Allows a randomized variance to be applied to the goal time for each cycle of the timer."), owner.m_flVariance);
        owner.m_flStartDelay = EditorGUILayout.FloatField(new GUIContent("Start Delay", "How long to wait until the timer begins (in seconds)."), owner.m_flStartDelay);

        //Events
        EditorGUILayout.PropertyField(timerCompletedReceiver, true);

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

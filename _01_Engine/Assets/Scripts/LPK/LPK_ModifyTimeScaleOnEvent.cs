/***************************************************
File:           LPK_ModifyTimeScaleOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component allows for the user to modify the game's
  timescale when an event is received.  Note that if you
  set the timescale to 0, the game is not considered paused,
  although it effecitvly is.  To consider the game as paused,
  you must use LPK_ModifyPauseStateOnEvent.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ModifyTimeScaleOnEvent
* DESCRIPTION : Component to manage the pause state of the scene.
**/
public class LPK_ModifyTimeScaleOnEvent : LPK_Component
{
    /************************************************************************************/

    public float m_flNewTimeScale;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initializes pausing functions.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger)
            m_EventTrigger.Register(this);
    }

    /**
    * FUNCTION NAME: OnEvent
    * DESCRIPTION  : Event validation.
    * INPUTS       : _activator - Game object that activated the event.  Null is all objects.
    * OUTPUTS      : None
    **/
    override public void OnEvent(GameObject _activator)
    {
        if(!ShouldRespondToEvent(_activator))
            return;

        Time.timeScale = m_flNewTimeScale;

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Changing time scale.");    
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Detach pause detection functions as they are not named OnEvent (since there are two).
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        if(m_EventTrigger)
            m_EventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_ModifyTimeScaleOnEvent))]
public class LPK_ModifyTimeScaleOnEventEditor : Editor
{
    SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_EventTrigger = serializedObject.FindProperty("m_EventTrigger");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_ModifyTimeScaleOnEvent owner = (LPK_ModifyTimeScaleOnEvent)target;

        LPK_ModifyTimeScaleOnEvent editorOwner = owner.GetComponent<LPK_ModifyTimeScaleOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyTimeScaleOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyTimeScaleOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyTimeScaleOnEvent");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_flNewTimeScale = EditorGUILayout.FloatField(new GUIContent("new Time Scale", "Time scale to switch to when the event is triggered.  Note that setting the timescale to 0, while effetivly pausing the game, does not make the game consider itself paused.  Use LPK_ModifyPauseStateOnEvent for this purpose.."), owner.m_flNewTimeScale);

        //Events.
        EditorGUILayout.PropertyField(m_EventTrigger, true);

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

}

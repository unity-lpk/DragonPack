/***************************************************
File:           LPK_StopSoundOnEvent
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to stop a 
  sound upon receving an event notice.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_StopSoundOnEvent
* DESCRIPTION : Stops a sound when an event is parsed.
**/
public class LPK_StopSoundOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Audio Source(s) whose emitter should be stopped.")]
    public AudioSource[] m_TargetObjects;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for sound stopping.
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

        if (m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        for (int i = 0; i < m_TargetObjects.Length; i++)
        {
            if (m_TargetObjects[i].GetComponent<AudioSource>() != null)
                m_TargetObjects[i].GetComponent<AudioSource>().Stop();
        }
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Removes game object from the event queue.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        if(m_EventTrigger != null)
            m_EventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_StopSoundOnEvent))]
public class LPK_StopSoundOnEventEditor : Editor
{
    SerializedProperty targetObjects;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        targetObjects = serializedObject.FindProperty("m_TargetObjects");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_StopSoundOnEvent owner = (LPK_StopSoundOnEvent)target;

        LPK_StopSoundOnEvent editorOwner = owner.GetComponent<LPK_StopSoundOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_StopSoundOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_StopSoundOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_StopSoundOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(targetObjects, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);

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

/***************************************************
File:           LPK_DispatchOnStart.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This script provides a mechanism to activate an event
  as soon as the Start function is called.  Useful for
  initalizing data and states for your game.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ActivateOnStart
* DESCRIPTION : Allows activation of scripts OnEvent functions on start.
**/
public class LPK_DispatchOnStart : LPK_Component
{
    [Tooltip("Event sent when ths component is enabled for the first time.")]
    public LPK_EventSendingInfo m_StartEvent;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Activate OnEvent functions on start.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        StartCoroutine(DispatchDelay());
    }

    /**
    * FUNCTION NAME: DispatchDelay
    * DESCRIPTION  : Forces delay before sending the event out.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator DispatchDelay()
    {
        //HACKHACK:  Delay event sending by a single frame.
        yield return new WaitForSeconds(0.0f);

        if(m_StartEvent != null && m_StartEvent.m_Event != null)
        {
            if(m_StartEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_StartEvent.m_Event.Dispatch(null);
            else if(m_StartEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_StartEvent.m_Event.Dispatch(gameObject);
            else if (m_StartEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_StartEvent.m_Event.Dispatch(gameObject, m_StartEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_StartEvent, this, "Start");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnStart))]
public class LPK_DispatchOnStartEditor : Editor
{
    SerializedProperty m_StartEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_StartEvent = serializedObject.FindProperty("m_StartEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnStart owner = (LPK_DispatchOnStart)target;

        LPK_DispatchOnStart editorOwner = owner.GetComponent<LPK_DispatchOnStart>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnStart)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnStart), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnStart");

        //Component propertes.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        //Events.
        EditorGUILayout.PropertyField(m_StartEvent, true);
            
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

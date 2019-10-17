/***************************************************
File:           LPK_DispatchOnUpdate.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This script provides a mechanism to activate an event
  every frame this component's Update function is called.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_DispatchOnUpdate
* DESCRIPTION : Allows activation of scripts OnEvent functions on every frame.
**/
public class LPK_DispatchOnUpdate : LPK_Component
{
    [Tooltip("Event sent when ths component calls its Update function.")]
    public LPK_EventSendingInfo m_UpdateEvent;

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Activate OnEvent functions on update.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if(m_UpdateEvent != null && m_UpdateEvent.m_Event != null)
        {
            if(m_UpdateEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_UpdateEvent.m_Event.Dispatch(null);
            else if(m_UpdateEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_UpdateEvent.m_Event.Dispatch(gameObject);
            else if (m_UpdateEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_UpdateEvent.m_Event.Dispatch(gameObject, m_UpdateEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_UpdateEvent, this, "Update");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnUpdate))]
public class LPK_DispatchOnUpdateEditor : Editor
{
    SerializedProperty m_UpdateEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_UpdateEvent = serializedObject.FindProperty("m_UpdateEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnUpdate owner = (LPK_DispatchOnUpdate)target;

        LPK_DispatchOnUpdate editorOwner = owner.GetComponent<LPK_DispatchOnUpdate>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnUpdate)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnUpdate), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnStart");

        //Component propertes.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        //Events.
        EditorGUILayout.PropertyField(m_UpdateEvent, true);
            
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

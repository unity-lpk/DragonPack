/***************************************************
File:           LPK_DispatchOnButtonInput.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component can be added to any object to cause it to 
  dispatch a LPK_ButtonInput event on a specified 
  target upon a given unity button being pressed,
  released or held.

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
* CLASS NAME  : LPK_DispatchOnButtonInput
* DESCRIPTION : Component to manage user input responses via virtual buttons.
**/
public class LPK_DispatchOnButtonInput : LPK_Component
{
    /************************************************************************************/

    public enum LPK_InputMode
    {
        PRESSED,
        RELEASED,
        HELD,
    };

    /************************************************************************************/

    public string m_sButton;

    [Tooltip("What mode should cause the event dispatch.")]
    [Rename("Input Mode")]
    public LPK_InputMode m_eInputMode = LPK_InputMode.PRESSED;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when a virtual button gives input.")]
    public LPK_EventSendingInfo m_ButtonInputEvent;

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Handles input checking
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if (m_eInputMode == LPK_InputMode.PRESSED && Input.GetButtonDown(m_sButton))
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Virtual button PRESSED " + m_sButton);

            DispatchButtonEvent();
        }
        else if (m_eInputMode == LPK_InputMode.RELEASED && Input.GetButtonUp(m_sButton))
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Virtual button RELEASED " + m_sButton);

            DispatchButtonEvent();
        }
        else if (m_eInputMode == LPK_InputMode.HELD && Input.GetButton(m_sButton))
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Virtual button HELD " + m_sButton);

            DispatchButtonEvent();
        }
    }

    /**
    * FUNCTION NAME: DispatchButtonEvent
    * DESCRIPTION  : Send out event for virtual button input.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchButtonEvent()
    {
        if(m_ButtonInputEvent != null && m_ButtonInputEvent.m_Event != null)
        {
            if(m_ButtonInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_ButtonInputEvent.m_Event.Dispatch(null);
            else if(m_ButtonInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_ButtonInputEvent.m_Event.Dispatch(gameObject);
            else if (m_ButtonInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_ButtonInputEvent.m_Event.Dispatch(gameObject, m_ButtonInputEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_ButtonInputEvent, this, "Virtual Button Input");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnButtonInput))]
public class LPK_DispatchOnButtonInputEditor : Editor
{
    SerializedProperty inputMode;

    SerializedProperty eventTriggers;
    SerializedProperty virtualButtonReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        inputMode = serializedObject.FindProperty("m_eInputMode");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");
        virtualButtonReceivers = serializedObject.FindProperty("m_ButtonInputEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnButtonInput owner = (LPK_DispatchOnButtonInput)target;

        LPK_DispatchOnButtonInput editorOwner = owner.GetComponent<LPK_DispatchOnButtonInput>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnButtonInput)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnButtonInput), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnButtonInput");

        

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_sButton = EditorGUILayout.TextField(new GUIContent("Trigger Button", "What virtual key will trigger the event dispatch."), owner.m_sButton);
        EditorGUILayout.PropertyField(inputMode, true);

        //Events
        EditorGUILayout.PropertyField(virtualButtonReceivers, true);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Base Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}
        
#endif  //UNITY_EDITOR

}   //LPK

/***************************************************
File:           LPK_DispatchOnKeyboardInput.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component can be added to any object to cause it to 
  dispatch a LPK_KeyboardInput event on a specified 
  target upon a given key being pressed, released or held.

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
* CLASS NAME  : LPK_DispatchOnKeyboardInput
* DESCRIPTION : Component to manage user input responses via keyboard.
**/
public class LPK_DispatchOnKeyboardInput : LPK_Component
{
    /************************************************************************************/

    public enum LPK_InputMode
    {
        PRESSED,
        RELEASED,
        HELD,
    };

    /************************************************************************************/

    [Tooltip("What key will trigger the event dispatch.")]
    [Rename("Trigger Key")]
    public KeyCode m_iKey;

    [Tooltip("Set any key to trigger event dispatch.  Overrides Trigger Key property.")]
    [Rename("Detect Any Key")]
    public bool m_bAnyKey;

    [Tooltip("What mode should cause the event dispatch.")]
    [Rename("Input Mode")]
    public LPK_InputMode m_eInputMode = LPK_InputMode.PRESSED;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when input is given from the keyboard.")]
    public LPK_EventSendingInfo m_KeyboardInputEvent;

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Handles input checking
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        //Dispatch event based on selected mode
        if (m_eInputMode == LPK_InputMode.PRESSED && (Input.GetKeyDown(m_iKey) || (m_bAnyKey && Input.anyKeyDown
            && !Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1) && !Input.GetKey(KeyCode.Mouse2) && !Input.GetKey(KeyCode.Mouse3) && !Input.GetKey(KeyCode.Mouse4) && !Input.GetKey(KeyCode.Mouse5) && !Input.GetKey(KeyCode.Mouse6))))
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Key PRESSED ");

            DispatchKeyboardEvent();
        }
        else if (m_eInputMode == LPK_InputMode.RELEASED && (Input.GetKeyUp(m_iKey) || (m_bAnyKey && Input.anyKeyDown
            && !Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1) && !Input.GetKey(KeyCode.Mouse2) && !Input.GetKey(KeyCode.Mouse3) && !Input.GetKey(KeyCode.Mouse4) && !Input.GetKey(KeyCode.Mouse5) && !Input.GetKey(KeyCode.Mouse6))))
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Key RELEASED ");

            DispatchKeyboardEvent();
        }
        else if (m_eInputMode == LPK_InputMode.HELD && (Input.GetKey(m_iKey) || (m_bAnyKey && Input.anyKeyDown
            && !Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1) && !Input.GetKey(KeyCode.Mouse2) && !Input.GetKey(KeyCode.Mouse3) && !Input.GetKey(KeyCode.Mouse4) && !Input.GetKey(KeyCode.Mouse5) && !Input.GetKey(KeyCode.Mouse6))))
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Key HELD ");

            DispatchKeyboardEvent();
        }
    }

    /**
    * FUNCTION NAME: DispatchKeyboardEvent
    * DESCRIPTION  : Sends event for keyboard input.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchKeyboardEvent()
    {
        if(m_KeyboardInputEvent != null && m_KeyboardInputEvent.m_Event != null)
        {
            if(m_KeyboardInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_KeyboardInputEvent.m_Event.Dispatch(null);
            else if(m_KeyboardInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_KeyboardInputEvent.m_Event.Dispatch(gameObject);
            else if (m_KeyboardInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_KeyboardInputEvent.m_Event.Dispatch(gameObject, m_KeyboardInputEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_KeyboardInputEvent, this, "Keyboard Input");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnKeyboardInput))]
public class LPK_DispatchOnKeyboardInputEditor : Editor
{
    SerializedProperty key;
    SerializedProperty inputMode;

    SerializedProperty eventTriggers;
    SerializedProperty collisionInfo;
    SerializedProperty inputInfo;

    SerializedProperty keyboardEventReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        key = serializedObject.FindProperty("m_iKey");
        inputMode = serializedObject.FindProperty("m_eInputMode");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        keyboardEventReceivers = serializedObject.FindProperty("m_KeyboardInputEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnKeyboardInput owner = (LPK_DispatchOnKeyboardInput)target;

        LPK_DispatchOnKeyboardInput editorOwner = owner.GetComponent<LPK_DispatchOnKeyboardInput>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnKeyboardInput)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnKeyboardInput), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnKeyboardInput");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_bAnyKey = EditorGUILayout.Toggle(new GUIContent("Detect Any Key", "Set any key to trigger event dispatch.  Overrides Trigger Key property."), owner.m_bAnyKey);

        if(!owner.m_bAnyKey)
            EditorGUILayout.PropertyField(key, true);

        EditorGUILayout.PropertyField(inputMode, true);

        //Events
        EditorGUILayout.PropertyField(keyboardEventReceivers, true);

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

/***************************************************
File:           LPK_DispatchOnMouseInput.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.14

Description:
  This component can be added to any object to cause it to 
  dispatch a LPK_MouseInput event on a specified target 
  upon a given button being pressed, released or held.

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
* CLASS NAME  : LPK_DispatchOnMouseInput
* DESCRIPTION : Component to manage user input responses via mice.
**/
public class LPK_DispatchOnMouseInput : LPK_Component
{
    /************************************************************************************/

    public enum LPK_MouseButtons
    {
        LEFT_BUTTON_PRESSED,
        LEFT_BUTTON_RELEASED,
        LEFT_BUTTON_HELD,
        RIGHT_BUTTON_PRESSED,
        RIGHT_BUTTON_RELEASED,
        RIGHT_BUTTON_HELD,
        MIDDLE_BUTTON_PRESSED,
        MIDDLE_BUTTON_RELEASED,
        MIDDLE_BUTTON_HELD,
        SCROLL_WHEEL_UP,
        SCROLL_WHEEL_DOWN,
        MOUSE_ENTER_GAME_OBJECT,
        MOUSE_EXIT_GAME_OBJECT,
        MOUSE_STAY_ON_GAME_OBJECT,
    };

    /************************************************************************************/

    [Tooltip("Which mouse button will trigger sending the event.  Note that Any does not detect scrollwheel.  Note all options that interact with game objects will only detect game objects with colliders.")]
    [Rename("Mouse Button")]
    public LPK_MouseButtons m_eInputMode = LPK_MouseButtons.LEFT_BUTTON_PRESSED;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the mouse gives input.")]
    public LPK_EventSendingInfo m_MouseInputEvent;

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Handles input checking for mice.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        //Left button.
        if (m_eInputMode == LPK_MouseButtons.LEFT_BUTTON_PRESSED)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Left Mouse Button PRESSED");

                DispatchEvent();
            }
        }

        else if (m_eInputMode == LPK_MouseButtons.LEFT_BUTTON_RELEASED)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Left Mouse Button RELEASED");

                DispatchEvent();
            }
        }

        else if (m_eInputMode == LPK_MouseButtons.LEFT_BUTTON_HELD)
        {
            if(Input.GetMouseButton(0))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Left Mouse Button HELD");

                DispatchEvent();
            }
        }

        //Right button.
        else if (m_eInputMode == LPK_MouseButtons.RIGHT_BUTTON_PRESSED)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Right Mouse Button PRESSED");

                DispatchEvent();
            }
        }

        else if (m_eInputMode == LPK_MouseButtons.RIGHT_BUTTON_RELEASED)
        {
            if (Input.GetMouseButtonUp(1))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Right Mouse Button RELEASED ");

                DispatchEvent();
            }
        }

        else if (m_eInputMode == LPK_MouseButtons.RIGHT_BUTTON_HELD)
        {
            if (Input.GetMouseButton(1))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Right Mouse Button HELD ");

                DispatchEvent();
            }
        }

        //Middle button.
        else if (m_eInputMode == LPK_MouseButtons.MIDDLE_BUTTON_PRESSED)
        {
            if (Input.GetMouseButtonDown(2))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Middle Mouse Button PRESSED");

                DispatchEvent();
            }
        }

        else if (m_eInputMode == LPK_MouseButtons.MIDDLE_BUTTON_RELEASED)
        {
            if (Input.GetMouseButtonUp(2))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Middle Mouse Button RELEASED");

                DispatchEvent();
            }
        }

        else if (m_eInputMode == LPK_MouseButtons.MIDDLE_BUTTON_HELD)
        {
            if (Input.GetMouseButton(2))
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Middle Mouse Button HELD");

                DispatchEvent();
            }
        }

        //Mouse scroll
        else if(m_eInputMode == LPK_MouseButtons.SCROLL_WHEEL_UP || m_eInputMode == LPK_MouseButtons.SCROLL_WHEEL_DOWN)
        {
            float mouseScrollDelta = Input.mouseScrollDelta.y;

            if (mouseScrollDelta < 0 && m_eInputMode == LPK_MouseButtons.SCROLL_WHEEL_UP)
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Mouse Wheel Scroll Up");

                DispatchEvent();
            }

            else if (mouseScrollDelta > 0 && m_eInputMode == LPK_MouseButtons.SCROLL_WHEEL_DOWN)
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Mouse Wheel Scroll Down");

                DispatchEvent();
            }
        }
    }

    /**
    * FUNCTION NAME: OnMouseEnter
    * DESCRIPTION  : Detects when the mouse starts hovering over this object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnMouseEnter()
    {
        if (m_eInputMode == LPK_MouseButtons.MOUSE_ENTER_GAME_OBJECT)
            return;

        DispatchEvent();
    }

    /**
    * FUNCTION NAME: OnMouseExit
    * DESCRIPTION  : Detects when the mouse stops hovering over this object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnMouseExit()
    {
        if (m_eInputMode == LPK_MouseButtons.MOUSE_EXIT_GAME_OBJECT)
            return;

        DispatchEvent();
    }

    /**
    * FUNCTION NAME: OnMouseOver
    * DESCRIPTION  : Detects when the mouse is hovering over this object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnMouseOver()
    {
        if(m_eInputMode == LPK_MouseButtons.MOUSE_STAY_ON_GAME_OBJECT)
            return;

        DispatchEvent();
    }

    /**
    * FUNCTION NAME: DispatchEvent
    * DESCRIPTION  : Dispatches an event and prints debug info if set.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchEvent()
    {
        if(m_MouseInputEvent != null && m_MouseInputEvent.m_Event != null)
        {
            if(m_MouseInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_MouseInputEvent.m_Event.Dispatch(null);
            else if(m_MouseInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_MouseInputEvent.m_Event.Dispatch(gameObject);
            else if (m_MouseInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_MouseInputEvent.m_Event.Dispatch(gameObject, m_MouseInputEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_MouseInputEvent, this, "Mouse Input");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnMouseInput))]
public class LPK_DispatchOnMouseInputEditor : Editor
{
    SerializedProperty m_eInputMode;

    SerializedProperty m_MouseInputEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_eInputMode = serializedObject.FindProperty("m_eInputMode");

        m_MouseInputEvent = serializedObject.FindProperty("m_MouseInputEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnMouseInput owner = (LPK_DispatchOnMouseInput)target;

        LPK_DispatchOnMouseInput editorOwner = owner.GetComponent<LPK_DispatchOnMouseInput>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnMouseInput)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnMouseInput), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnMouseInput");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_eInputMode, true);

        //Events
        EditorGUILayout.PropertyField(m_MouseInputEvent, true);

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

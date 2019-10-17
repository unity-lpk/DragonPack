/***************************************************
File:           LPK_UIModifyWindowedStateOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be used to change the fullscreen mode
  of the game window during runtime.  This component is
  ideally used on an options menu.

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
* CLASS NAME  : LPK_UIModifyWindowedStateOnEvent
* DESCRIPTION : Component used to change windowed state of game.
**/
public class LPK_UIModifyWindowedStateOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_WindowToggleType
    {
        CHANGE_FULLSCREEN,
        CHANGE_WINDOWED,
        CHANGE_TOGGLE,
    };

    /************************************************************************************/

    [Tooltip("How to change the windowed state when activated via event.")]
    [Rename("Toggle Type")]
    public LPK_WindowToggleType m_eWindowToggleType;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for sprite and color modification.
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

        if(m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        SetWindowType();
    }

    /**
    * FUNCTION NAME: SetWindowType
    * DESCRIPTION  : Changes windowed state.  Moved to public so UI buttons can interact with this.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void SetWindowType()
    {
        if (m_eWindowToggleType == LPK_WindowToggleType.CHANGE_FULLSCREEN)
            Screen.fullScreen = true;
        else if (m_eWindowToggleType == LPK_WindowToggleType.CHANGE_WINDOWED)
            Screen.fullScreen = false;
        else
            Screen.fullScreen = !Screen.fullScreen;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_UIModifyWindowedStateOnEvent))]
public class LPK_UIModifyWindowedStateOnEventEditor : Editor
{
    SerializedProperty windowToggleType;

    SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        windowToggleType = serializedObject.FindProperty("m_eWindowToggleType");
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
        LPK_UIModifyWindowedStateOnEvent owner = (LPK_UIModifyWindowedStateOnEvent)target;

        LPK_UIModifyWindowedStateOnEvent editorOwner = owner.GetComponent<LPK_UIModifyWindowedStateOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_UIModifyWindowedStateOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_UIModifyWindowedStateOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_UIModifyWindowedStateOnEvent");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(windowToggleType, true);

        //Event properties.
        EditorGUILayout.PropertyField(m_EventTrigger, true);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNITY_EDITOR

}   //LPK

/***************************************************
File:           LPK_ModifyScriptActiveStateOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component can be used to enable/disable the active
  state of other scripts.

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
* CLASS NAME  : LPK_ModifyScriptActiveStateOnEvent
* DESCRIPTION : Component used to enable/disable other components.
**/
public class LPK_ModifyScriptActiveStateOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_ToggleType
    {
        ON,
        OFF,
        TOGGLE,
    };

    /************************************************************************************/

    [Tooltip("How to change the active state of the declared script(s).")]
    [Rename("Toggle Type")]
    public LPK_ToggleType m_eToggleType;

    [Tooltip("Script(s) to change the enabled state of.")]
    [Rename("Scripts")]
    public Behaviour[] m_ModifyScripts;

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

        for(int i = 0; i < m_ModifyScripts.Length; i++)
        {
            if (m_ModifyScripts[i] == null)
                continue;

            if (m_eToggleType == LPK_ToggleType.ON)
                m_ModifyScripts[i].enabled = true;
            else if (m_eToggleType == LPK_ToggleType.OFF)
                m_ModifyScripts[i].enabled = false;
            else if (m_eToggleType == LPK_ToggleType.TOGGLE)
            {
                if (!m_ModifyScripts[i].enabled)
                    m_ModifyScripts[i].enabled = true;
                else if (m_ModifyScripts[i].enabled)
                    m_ModifyScripts[i].enabled = false;
            }

            //Debug info.
            if(m_bPrintDebug && m_ModifyScripts[i].enabled)
                LPK_PrintDebug(this, "Changing active state of " + m_ModifyScripts[i] + " to ON.");
            else if (m_bPrintDebug && !m_ModifyScripts[i].enabled)
                LPK_PrintDebug(this, "Changing active state of " + m_ModifyScripts[i] + " to OFF.");
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

[CustomEditor(typeof(LPK_ModifyScriptActiveStateOnEvent))]
public class LPK_ModifyScriptActiveStateOnEventEditor : Editor
{
    SerializedProperty toggleType;
    SerializedProperty m_ModifyScripts;

    SerializedProperty eventTriggers;

    SerializedProperty reachNodeReceivers;
    SerializedProperty reachFinalNodeReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        toggleType = serializedObject.FindProperty("m_eToggleType");
        m_ModifyScripts = serializedObject.FindProperty("m_ModifyScripts");

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
        LPK_ModifyScriptActiveStateOnEvent owner = (LPK_ModifyScriptActiveStateOnEvent)target;

        LPK_ModifyScriptActiveStateOnEvent editorOwner = owner.GetComponent<LPK_ModifyScriptActiveStateOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyScriptActiveStateOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyScriptActiveStateOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
            
        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyScriptActiveStateOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(toggleType, true);
        LPK_EditorArrayDraw.DrawArray(m_ModifyScripts, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

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

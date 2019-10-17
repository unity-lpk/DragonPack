/***************************************************
File:           LPK_ModifyPauseStateOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component manages the pause state of a scene.

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
* CLASS NAME  : LPK_ModifyPauseStateOnEvent
* DESCRIPTION : Component to manage the pause state of the scene.
**/
public class LPK_ModifyPauseStateOnEvent : LPK_Component
{
    /************************************************************************************/

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_PauseEventTrigger;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initializes pausing functions.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_PauseEventTrigger)
            m_PauseEventTrigger.Register(this);
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

        StartCoroutine(FrameDelay());
    }

     /**
     * FUNCTION NAME: FrameDelay
     * DESCRIPTION  : Delays pause by 1 frame so objects can settle and spawn (like the pause menu).
     * INPUTS       : None
     * OUTPUTS      : None
     **/
    IEnumerator FrameDelay()
    {
        //Assume 60 FPS
        yield return true;
        Pause();
    }

    /**
    * FUNCTION NAME: Pause
    * DESCRIPTION  : Pauses the scene.  Set to public so UI buttons can interact with this.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void Pause()
    {
        LPK_PauseManager.Pause(0.0f);

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Pausing scene.");
    }

    /**
    * FUNCTION NAME: Unpause
    * DESCRIPTION  : Unpauses the scene.  Set to public so UI buttons can interact with this.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void Unpause()
    {
        LPK_PauseManager.Unpause();
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Detach pause detection functions as they are not named OnEvent (since there are two).
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        if(m_PauseEventTrigger)
            m_PauseEventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_ModifyPauseStateOnEvent))]
public class LPK_ModifyPauseStateOnEventEditor : Editor
{
    SerializedProperty m_PauseEventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_PauseEventTrigger = serializedObject.FindProperty("m_PauseEventTrigger");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_ModifyPauseStateOnEvent owner = (LPK_ModifyPauseStateOnEvent)target;

        LPK_ModifyPauseStateOnEvent editorOwner = owner.GetComponent<LPK_ModifyPauseStateOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyPauseStateOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyPauseStateOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyPauseStateOnEvent");

        //Events
        EditorGUILayout.PropertyField(m_PauseEventTrigger, true);

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

/***************************************************
File:           LPK_ModifyMouseVisibilityOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   9/6/2019
Last Version:   2019.1.14

Description:
  This component is used to modify the visibility of the mouse
  cursor when an event is received.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ModifyMouseVisibilityOnEvent
* DESCRIPTION : Basic component used to show/hide the mouse cursor on an event.
**/
public class LPK_ModifyMouseVisibilityOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_MouseVisibilityMode
    {
        SHOW,
        HIDE
    };

    /************************************************************************************/

    [Header("Component Properties")]

    [Tooltip("Sets how this component will effect mouse visibility.")]
    [Rename("Mouse Visibility Mode")]
    public LPK_MouseVisibilityMode m_eMouseVisibilityMode;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component to be active.")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Set up event listening.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if (m_EventTrigger != null)
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

        if(m_eMouseVisibilityMode == LPK_MouseVisibilityMode.HIDE)
           Cursor.visible = false;
        else
           Cursor.visible = true;
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

[CustomEditor(typeof(LPK_ModifyMouseVisibilityOnEvent))]
public class LPK_ModifyMouseVisibilityOnEventEditor : Editor
{
    SerializedProperty m_eMouseVisibilityMode;
    SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_eMouseVisibilityMode = serializedObject.FindProperty("m_eMouseVisibilityMode");
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
        LPK_ModifyMouseVisibilityOnEvent owner = (LPK_ModifyMouseVisibilityOnEvent)target;

        LPK_ModifyMouseVisibilityOnEvent editorOwner = owner.GetComponent<LPK_ModifyMouseVisibilityOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyMouseVisibilityOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyMouseVisibilityOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyMouseVisibilityOnEvent");

        //Component Properties

        EditorGUILayout.PropertyField(m_eMouseVisibilityMode, true);

        //Events
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

#endif  //UNITY_EDTIOR

}   //LPK

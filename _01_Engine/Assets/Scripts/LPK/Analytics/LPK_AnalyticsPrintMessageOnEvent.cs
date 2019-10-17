/***************************************************
File:           LPK_AnalyticsPrintMessageOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  Prints messages to an analytics file when events are
  receieved.

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
* CLASS NAME  : LPK_AnalyticsPrintMessageOnEvent
* DESCRIPTION : Analytics script to print messages to a file when events are received.
**/
public class LPK_AnalyticsPrintMessageOnEvent : LPK_AnalyticsBase
{
    [Header("Component Properties")]

    public string m_sMessage;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initializes event detection.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger != null)
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
        
        PrintLogMessage(m_sMessage);
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

[CustomEditor(typeof(LPK_AnalyticsPrintMessageOnEvent))]
public class LPK_AnalyticsPrintMessageOnEventEditor : Editor
{
    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
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
        LPK_AnalyticsPrintMessageOnEvent owner = (LPK_AnalyticsPrintMessageOnEvent)target;

        LPK_AnalyticsPrintMessageOnEvent editorOwner = owner.GetComponent<LPK_AnalyticsPrintMessageOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_AnalyticsPrintMessageOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_AnalyticsPrintMessageOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_AnalyticsPrintMessageOnEvent");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Base Analytics Properties", EditorStyles.boldLabel);

        owner.m_sFileName = EditorGUILayout.TextField(new GUIContent("File Name", "File name to print analytics messages into."), owner.m_sFileName);
        owner.m_sDirectoryPath = EditorGUILayout.TextField(new GUIContent("Path Name", "Directory path to print analytics messages into.  Starts at asset folder."), owner.m_sDirectoryPath);
        owner.m_bPrintFileMessages = EditorGUILayout.Toggle(new GUIContent("Print File Messages", "Print messages to state the analytic recording has started and stopped.  This can disable multiple analytics scripts writing to the same file from all writing initlaization and closing messages."), owner.m_bPrintFileMessages);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_sMessage = EditorGUILayout.TextField(new GUIContent("Message", "Message to print to the log file."), owner.m_sMessage);

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

#endif

}   //LPK

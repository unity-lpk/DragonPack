/***************************************************
File:           LPK_AnalyticsRecordGameSessionTime.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  Records time that a gamesession lasted in seconds.

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
* CLASS NAME  : LPK_AnalyticsRecordGameSessionTime
* DESCRIPTION : Analytics script to record length a play session.
**/
public class LPK_AnalyticsRecordGameSessionTime : LPK_AnalyticsBase
{
    /************************************************************************************/

    float m_flStartTime;
    bool m_bHasPrintedEndSessionMessage;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Get start time and mark object to not be destroyed on scene loading.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        base.OnStart();

        m_flStartTime = Time.time;
        Object.DontDestroyOnLoad(gameObject);

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Marking " + gameObject.name + " as persistent to accuratly track play session length.  This object will never be destroyed!");
    }

    /**
    * FUNCTION NAME: OnDestroyed
    * DESCRIPTION  : Print end of session information.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        PrintGameEndMessage();
    }

    /**
    * FUNCTION NAME: OnApplicationQuit
    * DESCRIPTION  : Print end of session information.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnApplicationQuit()
    {
        PrintGameEndMessage();
    }

    /**
    * FUNCTION NAME: PrintGameEndMessage
    * DESCRIPTION  : Print end of session information.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void PrintGameEndMessage()
    {
        if (m_bHasPrintedEndSessionMessage)
            return;

        m_bHasPrintedEndSessionMessage = true;

        float totalTime = Time.time - m_flStartTime;
        PrintLogMessage("Game lasted for " + totalTime + " seconds.", true, 3);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_AnalyticsRecordGameSessionTime))]
public class LPK_AnalyticsRecordGameSessionTimeEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_AnalyticsRecordGameSessionTime owner = (LPK_AnalyticsRecordGameSessionTime)target;

        LPK_AnalyticsRecordGameSessionTime editorOwner = owner.GetComponent<LPK_AnalyticsRecordGameSessionTime>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_AnalyticsRecordGameSessionTime)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_AnalyticsRecordGameSessionTime), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_AnalyticsRecordGameSessionTime");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Base Analytics Properties", EditorStyles.boldLabel);

        owner.m_sFileName = EditorGUILayout.TextField(new GUIContent("File Name", "File name to print analytics messages into."), owner.m_sFileName);
        owner.m_sDirectoryPath = EditorGUILayout.TextField(new GUIContent("Path Name", "Directory path to print analytics messages into.  Starts at asset folder."), owner.m_sDirectoryPath);
        owner.m_bPrintFileMessages = EditorGUILayout.Toggle(new GUIContent("Print File Messages", "Print messages to state the analytic recording has started and stopped.  This can disable multiple analytics scripts writing to the same file from all writing initlaization and closing messages."), owner.m_bPrintFileMessages);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);
    }
}

#endif

}   //LPK
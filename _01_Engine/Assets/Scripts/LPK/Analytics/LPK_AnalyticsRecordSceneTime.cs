/***************************************************
File:           LPK_AnalyticsRecordSceneTime.cs
Authors:        Christopher Onorati
Last Updated:   5/16/2019
Last Version:   2018.3.14

Description:
  Records time that a scene was active in seconds.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_AnalyticsRecordSceneTime
* DESCRIPTION : Analytics script to record length a scene is active.
**/
public class LPK_AnalyticsRecordSceneTime : LPK_AnalyticsBase
{
    /************************************************************************************/

    //Start time of recording.
    float m_flStartTime;

    //Flag to check if the end of session message has been printed.
    bool m_bHasPrintedEndSessionMessage;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Get start time and hook up to scene changing event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        base.OnStart();

        SceneManager.activeSceneChanged += SceneChanged;
        m_flStartTime = Time.time;

        Debug.Log(Time.realtimeSinceStartup);
    }

    /**
    * FUNCTION NAME: SceneChanged
    * DESCRIPTION  : Print scene changed message.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SceneChanged(Scene current, Scene next)
    {
        PrintSceneChangeMessage();
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Print scene changed message.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        PrintSceneChangeMessage();
    }

    /**
    * FUNCTION NAME: OnApplicationQuit
    * DESCRIPTION  : Print scene changed message.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnApplicationQuit()
    {
        PrintSceneChangeMessage();
    }

    /**
    * FUNCTION NAME: OnApplicationQuit
    * DESCRIPTION  : Print end of session information.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void PrintSceneChangeMessage()
    {
        if (m_bHasPrintedEndSessionMessage)
            return;

        m_bHasPrintedEndSessionMessage = true;

        SceneManager.activeSceneChanged -= SceneChanged;

        float totalTime = Time.time - m_flStartTime;
        PrintLogMessage("Scene " + SceneManager.GetActiveScene().name + " lasted for " + totalTime + " seconds.", true, 3);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_AnalyticsRecordSceneTime))]
public class LPK_AnalyticsRecordSceneTimeEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_AnalyticsRecordSceneTime owner = (LPK_AnalyticsRecordSceneTime)target;

        LPK_AnalyticsRecordSceneTime editorOwner = owner.GetComponent<LPK_AnalyticsRecordSceneTime>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_AnalyticsRecordSceneTime)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_AnalyticsRecordSceneTime), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_AnalyticsRecordSceneTime");

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

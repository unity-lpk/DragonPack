/***************************************************
File:           LPK_LoadLevelOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to cause it to 
  load a given level upon receiving a specific event.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.SceneManagement; /* Access to scene datatype and scenemanager */
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_LoadSceneOnEvent
* DESCRIPTION : Loads a new scene on parsing of an event.
**/
public class LPK_LoadSceneOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Level to load when triggered.")]
    [SceneDropdown]
    public string m_LevelToLoad = "";

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for level/scene switching.
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

        LoadScene();
    }

    /**
    * FUNCTION NAME: LoadScene
    * DESCRIPTION  : Launches a new level.  Seperated from OnEvent so it is exposed
    *                and accessible by Unity UI system (buttons).
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(m_LevelToLoad))
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Loading new level.");

            SceneManager.LoadScene(m_LevelToLoad);
        }
        else
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "No Scene specified.");
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

[CustomEditor(typeof(LPK_LoadSceneOnEvent))]
public class LPK_LoadSceneOnEventEditor : Editor
{
    SerializedProperty levelToLoad;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        levelToLoad = serializedObject.FindProperty("m_LevelToLoad");

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
        LPK_LoadSceneOnEvent owner = (LPK_LoadSceneOnEvent)target;

        LPK_LoadSceneOnEvent editorOwner = owner.GetComponent<LPK_LoadSceneOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_LoadSceneOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_LoadSceneOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_LoadSceneOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(levelToLoad, true);

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

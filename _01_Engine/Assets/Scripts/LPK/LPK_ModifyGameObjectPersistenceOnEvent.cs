/***************************************************
File:           LPK_ModifyGameObjectPersistenceOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/24/2019
Last Version:   2019.1.14

Description:
  This component modifies the persistence of gameobjects
  across scene loads.


This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ModifyGameObjectPersistenceOnEvent
* DESCRIPTION : Component to modify the persistence of game objects across scenes.
**/
public class LPK_ModifyGameObjectPersistenceOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_ToggleType
    {
        ON,
        OFF,
        TOGGLE,
    };

    /************************************************************************************/

    [Tooltip("How to modify the persistent state of objects.")]
    [Rename("Toggle Type")]
    public LPK_ToggleType m_eToggleType;

    [Tooltip("Game objects to mark as persistent.")]
    public GameObject[] m_GameObjects;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component to be active.")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    [SerializeField]
    bool m_bHasSetup = false;


    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up event listening and initial persistence.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger)
            m_EventTrigger.Register(this);
    }

    /**
    * FUNCTION NAME: OnDrawGizmosSelected
    * DESCRIPTION  : Set the default game object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDrawGizmosSelected()
    {
        if (!m_bHasSetup)
        {
            m_GameObjects = new GameObject[] { gameObject };
            m_bHasSetup = true;
        }
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

        //NOTENOTE:  Technically these could be written as one function, and there is good argument for doing this.  I have seperated them out though, for easy
        //           of code readability.  The tradeoff of being able to read this code easily compared to having duplicate lines is okay here, I think.
        if (m_eToggleType == LPK_ToggleType.ON)
            SetObjectPersistence();
        else if (m_eToggleType == LPK_ToggleType.OFF)
            SetObjectDestroyable();
        else if (m_eToggleType == LPK_ToggleType.TOGGLE)
            ToggleObjectPersistence();
    }

    /**
    * FUNCTION NAME: SetObjectPersistence
    * DESCRIPTION  : Marks specified objects to not be destroyed between scene loads.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetObjectPersistence()
    {
        for (int i = 0; i < m_GameObjects.Length; i++)
        {
            Object.DontDestroyOnLoad(m_GameObjects[i]);

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Game object " + m_GameObjects[i].name + " set to be persistent.");
        }
    }

    /**
    * FUNCTION NAME: SetObjectDestroyable
    * DESCRIPTION  : Marks specified objects to be destroyed on the next scene load.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetObjectDestroyable()
    {
        for (int i = 0; i < m_GameObjects.Length; i++)
        {
            SceneManager.MoveGameObjectToScene(m_GameObjects[i], SceneManager.GetActiveScene());

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Game object " + m_GameObjects[i].name + " set to be destroyed on next scene load.");
        }
    }

    /**
    * FUNCTION NAME: ToggleObjectPersistence
    * DESCRIPTION  : Toggle the persistence of objects between active and inactive.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void ToggleObjectPersistence()
    {
        for (int i = 0; i < m_GameObjects.Length; i++)
        {
            if (m_GameObjects[i].scene.buildIndex == -1)
            {
                SceneManager.MoveGameObjectToScene(m_GameObjects[i], SceneManager.GetActiveScene());

                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Game object " + m_GameObjects[i].name + " set to be destroyed on next scene load.");
            }
            else
            {
                Object.DontDestroyOnLoad(m_GameObjects[i]);

                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Game object " + m_GameObjects[i].name + " set to be persistent.");
            }
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

[CustomEditor(typeof(LPK_ModifyGameObjectPersistenceOnEvent))]
public class LPK_ModifyGameObjectPersistenceOnEventEditor : Editor
{
    SerializedProperty toggleType;
    SerializedProperty gameObjects;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        toggleType = serializedObject.FindProperty("m_eToggleType");
        gameObjects = serializedObject.FindProperty("m_GameObjects");

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
        LPK_ModifyGameObjectPersistenceOnEvent owner = (LPK_ModifyGameObjectPersistenceOnEvent)target;

        LPK_ModifyGameObjectPersistenceOnEvent editorOwner = owner.GetComponent<LPK_ModifyGameObjectPersistenceOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyGameObjectPersistenceOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyGameObjectPersistenceOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyGameObjectPersistenceOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(toggleType, true);
        LPK_EditorArrayDraw.DrawArray(gameObjects, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

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

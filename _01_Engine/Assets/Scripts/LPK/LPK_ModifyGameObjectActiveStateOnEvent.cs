/***************************************************
File:           LPK_ModifyGameObjectActiveStateOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be used to enable/disable the active
  state of gameobjects.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

NOTE THE GAME OBJECTS TURNED OFF MUST NOT BE PARENTED TO ANY
OTHER GAME OBJECT.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ModifyGameObjectActiveStateOnEvent
* DESCRIPTION : Component used to enable/disable game objects.
**/
public class LPK_ModifyGameObjectActiveStateOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_ToggleType
    {
        ON,
        OFF,
        TOGGLE,
    };

    /************************************************************************************/

    [Tooltip("How to change the active state of the declared gameobject(s).")]
    [Rename("Toggle Type")]
    public LPK_ToggleType m_eToggleType;

    [Tooltip("Gameobject(s) to change the active state of.")]
    public GameObject[] m_ModifyGameObject;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    [SerializeField]
    bool m_bHasSetup = false;

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
    * FUNCTION NAME: OnDrawGizmosSelected
    * DESCRIPTION  : Set the default game object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDrawGizmosSelected()
    {
        if (!m_bHasSetup)
        {
            m_ModifyGameObject = new GameObject[] { gameObject };
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

        //Debug search.
        for (int i = 0; i < m_ModifyGameObject.Length; i++)
        {
            if (m_ModifyGameObject[i] == null)
                continue;

            if (m_eToggleType == LPK_ToggleType.ON)
                m_ModifyGameObject[i].SetActive(true);
            else if (m_eToggleType == LPK_ToggleType.OFF)
                m_ModifyGameObject[i].SetActive(false);
            else if (m_eToggleType == LPK_ToggleType.TOGGLE)
            {
                if (!m_ModifyGameObject[i].activeSelf)
                    m_ModifyGameObject[i].SetActive(true);
                else if (m_ModifyGameObject[i].activeSelf)
                    m_ModifyGameObject[i].SetActive(false);
            }

            //Debug info.
            if (m_bPrintDebug && m_ModifyGameObject[i].activeSelf)
                LPK_PrintDebug(this, "Changing active state of " + m_ModifyGameObject[i] + " to ON.");
            else if (m_bPrintDebug && !m_ModifyGameObject[i].activeSelf)
                LPK_PrintDebug(this, "Changing active state of " + m_ModifyGameObject[i] + " to OFF.");
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

[CustomEditor(typeof(LPK_ModifyGameObjectActiveStateOnEvent))]
public class LPK_ModifyGameObjectActiveStateOnEventEditor : Editor
{
    SerializedProperty toggleType;
    SerializedProperty modifyGameObject;

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
        modifyGameObject = serializedObject.FindProperty("m_ModifyGameObject");

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
        LPK_ModifyGameObjectActiveStateOnEvent owner = (LPK_ModifyGameObjectActiveStateOnEvent)target;

        LPK_ModifyGameObjectActiveStateOnEvent editorOwner = owner.GetComponent<LPK_ModifyGameObjectActiveStateOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyGameObjectActiveStateOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyGameObjectActiveStateOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyGameObjectActiveStateOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(toggleType, true);
        LPK_EditorArrayDraw.DrawArray(modifyGameObject, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

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

/***************************************************
File:           LPK_DestroyOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component can be added to any object to cause 
  it to destroy specified target(s) upon receiving a
  specified event.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_DestroyOnEvent
* DESCRIPTION : Destroys an object on parsing user-specified event.
**/
public class LPK_DestroyOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Game objects to destroy on receiving event.  If this array and the tag array are set to length 0, assume self.")]
    public GameObject[] m_DestructionTargets; 

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    [Header("Event Sending Info")]

    [Tooltip("Event sent the event for a game object being destroyed.")]
    public LPK_EventSendingInfo m_GameObjectDestroyedEvent;


    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    bool m_bHasSetup = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for object destruction.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger != null)
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
        if(!m_bHasSetup)
        {
            m_DestructionTargets = new GameObject[] { gameObject };
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
        
        StartCoroutine(DestructionDelay());
    }

    /**
    * FUNCTION NAME: DestructionDelay
    * DESCRIPTION  : Forces delay before destroying set object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator DestructionDelay()
    {
        //HACKHACK:  Delay destruction by a single frame.
        yield return new WaitForSeconds(0.0f);
        DestroyTarget();
    }

    /**
    * FUNCTION NAME: DestroyTarget
    * DESCRIPTION  : Manages object destruction.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void DestroyTarget()
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Destroying game objects...");
        
        List<GameObject> destroyedObjects = new List<GameObject>();

        //Destroy game objects.
        for (int i = 0; i < m_DestructionTargets.Length; i++)
            destroyedObjects.Add(m_DestructionTargets[i]);

        DispatchDestructionEvent();

        for(int i = 0; i < destroyedObjects.Count; i++)
            Destroy(destroyedObjects[i]);
    }

    /**
    * FUNCTION NAME: DispatchDestructionEvent
    * DESCRIPTION  : Dispatches the land event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchDestructionEvent()
    {
        if(m_GameObjectDestroyedEvent != null && m_GameObjectDestroyedEvent.m_Event != null)
        {
            if(m_GameObjectDestroyedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_GameObjectDestroyedEvent.m_Event.Dispatch(null);
            else if(m_GameObjectDestroyedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_GameObjectDestroyedEvent.m_Event.Dispatch(gameObject);
            else if (m_GameObjectDestroyedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_GameObjectDestroyedEvent.m_Event.Dispatch(gameObject, m_GameObjectDestroyedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_GameObjectDestroyedEvent, this, "Detaching (Un-Parenting) Game Objects");
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

[CustomEditor(typeof(LPK_DestroyOnEvent))]
public class LPK_DestroyOnEventEditor : Editor
{
    SerializedProperty destructionTargets;

    SerializedProperty eventTriggers;

    SerializedProperty gameObjectDeletedEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        destructionTargets = serializedObject.FindProperty("m_DestructionTargets");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        gameObjectDeletedEvent = serializedObject.FindProperty("m_GameObjectDestroyedEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DestroyOnEvent owner = (LPK_DestroyOnEvent)target;

        LPK_DestroyOnEvent editorOwner = owner.GetComponent<LPK_DestroyOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DestroyOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DestroyOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DestroyOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(destructionTargets, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);

        EditorGUILayout.PropertyField(gameObjectDeletedEvent, true);

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

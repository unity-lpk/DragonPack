/***************************************************
File:           LPK_DetachOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4
Description:
  This component can be added to any object to have it 
  detach an object from its parent upon receiving an event.

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
* CLASS NAME  : LPK_DetachOnEvent
* DESCRIPTION : Component to unparent an object on events.
**/
public class LPK_DetachOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Game object to detach from parent.  If not set and tag is not set, assume self.")]
    [Rename("Detach Game Object")]
    public GameObject m_pDetachObject;

    [Tooltip("Tag to detach from parent.  Only used if Detach Object is set to null.  If not set and detach object is not set, assume self.")]
    [TagDropdown]
    public string m_DetachTag;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when unparenting a game object from another.")]
    public LPK_EventSendingInfo m_DetachEvent;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for object parenting.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_EventTrigger.Register(this);

        if (m_pDetachObject == null && string.IsNullOrEmpty(m_DetachTag))
            m_pDetachObject = gameObject;

        if (m_pDetachObject == null && !string.IsNullOrEmpty(m_DetachTag))
        {
            m_pDetachObject = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_DetachTag);

            if(LPK_MultiTagManager.FindGameObjectsWithTag(gameObject, m_DetachTag).Count > 1 && m_bPrintDebug)
                LPK_PrintWarning(this, "WARNNG: Undefined behavior for detach selection!  Multiple game objects found with the tag: " + m_DetachTag + 
                                 "Please note that in a build, it is undefined which  game object will be selected.");
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

        Detach();
    }

    /**
    * FUNCTION NAME: Detach
    * DESCRIPTION  : Detaches two objects together on an event occurance.  Seperated from OnEvent for Start functionality.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Detach()
    {
        //Detach object
        if (m_pDetachObject.transform.parent != null)
        {
            m_pDetachObject.transform.parent = null;

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Object Detached");

            //Send out event.
            DispatchDetachEvent();
        }
    }

    /**
    * FUNCTION NAME: DispatchDetach
    * DESCRIPTION  : Dispatches the land event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchDetachEvent()
    {
        if(m_DetachEvent != null && m_DetachEvent.m_Event != null)
        {
            if(m_DetachEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_DetachEvent.m_Event.Dispatch(null);
            else if(m_DetachEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_DetachEvent.m_Event.Dispatch(gameObject);
            else if (m_DetachEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_DetachEvent.m_Event.Dispatch(gameObject, m_DetachEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_DetachEvent, this, "Detaching (Un-Parenting) Game Objects");
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

[CustomEditor(typeof(LPK_DetachOnEvent))]
public class LPK_DetachOnEventEditor : Editor
{
    SerializedProperty detachObject;
    SerializedProperty detachTag;

    SerializedProperty eventTriggers;

    SerializedProperty detachEventReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        detachObject = serializedObject.FindProperty("m_pDetachObject");
        detachTag = serializedObject.FindProperty("m_DetachTag");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        detachEventReceivers = serializedObject.FindProperty("m_DetachEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DetachOnEvent owner = (LPK_DetachOnEvent)target;

        LPK_DetachOnEvent editorOwner = owner.GetComponent<LPK_DetachOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DetachOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DetachOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DetachOnEvent");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Base Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(detachObject, true);
        EditorGUILayout.PropertyField(detachTag, true);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);

        EditorGUILayout.PropertyField(detachEventReceivers, true);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNITY_EDITOR

}   //LPK

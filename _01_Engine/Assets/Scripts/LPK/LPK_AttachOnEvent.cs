/***************************************************
File:           LPK_AttachOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component causes and object to be attached 
  (parented) to another upon receiving a specified event.

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
* CLASS NAME  : LPK_AttachOnEvent
* DESCRIPTION : Component to enable parenting of objects on events.
**/
public class LPK_AttachOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Parent game object to attach the child to.  If not set and tag is not set, assume self.")]
    [Rename("Parent")]
    public GameObject m_pParentObject;

    [Tooltip("Tag to find the parent with.  Only used if Parent is set to null. If not set and tag is not set, assume self.")]
    [TagDropdown]
    public string m_ParentTag;

    [Tooltip("Child to attach to the parent.  If not set and tag is not set, assume self.")]
    [Rename("Child")]
    public GameObject m_pChildObject;

    [Tooltip("Tag to find the child with.  Only used if Child is set to null.  If not set and Child is not set, assume self.")]
    [TagDropdown]
    public string m_ChildTag;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when parenting an object to another.")]
    public LPK_EventSendingInfo m_AttachEvent;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for object parenting.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start ()
    {
        if(m_EventTrigger != null)
            m_EventTrigger.Register(this);

        if (m_pParentObject == null && string.IsNullOrEmpty(m_ParentTag))
            m_pParentObject = gameObject;

        if (m_pChildObject == null && string.IsNullOrEmpty(m_ChildTag))
        {
            if (m_pParentObject == gameObject)
            {
                LPK_PrintError(this, "Attempted to set child and parent to the same object!  Game is now closing.");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif  //UNITY_EDITOR
            }
            else
                m_pChildObject = gameObject;
        }

        if (m_pParentObject == null && !string.IsNullOrEmpty(m_ParentTag))
        {
            m_pParentObject = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_ParentTag);

            if(LPK_MultiTagManager.FindGameObjectsWithTag(gameObject, m_ParentTag).Count > 1 && m_bPrintDebug)
                LPK_PrintWarning(this, "WARNNG: Undefined behavior for parent selection!  Multiple game objects found with the tag: " + m_ParentTag + 
                                 "Please note that in a build, it is undefined which game object will be selected.");
        }

        if (m_pChildObject == null && !string.IsNullOrEmpty(m_ChildTag))
        {
            m_pChildObject = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_ChildTag);

            if(LPK_MultiTagManager.FindGameObjectsWithTag(gameObject, m_ChildTag).Count > 1 && m_bPrintDebug)
                LPK_PrintWarning(this, "WARNNG: Undefined behavior for child selection!  Multiple game objects found with the tag: " + m_ChildTag + 
                                 "Please note that in a build, it is undefined which game object will be selected.");
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

        Attach();
    }

    /**
    * FUNCTION NAME: Attach
    * DESCRIPTION  : Attach child to parent.  Seperated from OnEvent for Start functionality.
    * INPUTS       : data - Event data to parse for validation.
    * OUTPUTS      : None
    **/
    void Attach()
    {
        if (m_pChildObject == null)
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "No Child Object set for parenting.");

            return;
        }

        if (m_pParentObject == null)
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "No Parent Object set for parenting.");

            return;
        }

        //NOTENOTE:  The below code DETACHES a game object in the event the attach event is a simple parent swap.
        if(m_pParentObject.transform.parent == m_pChildObject.transform.parent)
            m_pChildObject.transform.parent = null;

        //Attach object if it isn't already attached to that object
        if (m_pChildObject.transform.parent != m_pParentObject.transform)
        {
            m_pChildObject.transform.SetParent(m_pParentObject.transform);

            //Send out event.
            DispatchAttachEvent();


            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Object Attached");
        }
    }

    /**
    * FUNCTION NAME: DispatchAttachEvent
    * DESCRIPTION  : Dispatches the land event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchAttachEvent()
    {
        if(m_AttachEvent != null && m_AttachEvent.m_Event != null)
        {
            if(m_AttachEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_AttachEvent.m_Event.Dispatch(null);
            else if(m_AttachEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_AttachEvent.m_Event.Dispatch(gameObject);
            else if (m_AttachEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                    m_AttachEvent.m_Event.Dispatch(gameObject, m_AttachEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_AttachEvent, this, "Attaching (Parenting) Game Objects");
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

[CustomEditor(typeof(LPK_AttachOnEvent))]
public class LPK_AttachOnEventEditor : Editor
{
    SerializedProperty parentObject;
    SerializedProperty parentTag;
    SerializedProperty childObject;
    SerializedProperty childTag;

    SerializedProperty eventTriggers;

    SerializedProperty attachEventReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        parentObject = serializedObject.FindProperty("m_pParentObject");
        parentTag = serializedObject.FindProperty("m_ParentTag");
        childObject = serializedObject.FindProperty("m_pChildObject");
        childTag = serializedObject.FindProperty("m_ChildTag");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        attachEventReceivers = serializedObject.FindProperty("m_AttachEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_AttachOnEvent owner = (LPK_AttachOnEvent)target;

        LPK_AttachOnEvent editorOwner = owner.GetComponent<LPK_AttachOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_AttachOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_AttachOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_AttachOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(parentObject, true);
        EditorGUILayout.PropertyField(parentTag, true);
        EditorGUILayout.PropertyField(childObject, true);
        EditorGUILayout.PropertyField(childTag, true);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);

        EditorGUILayout.PropertyField(attachEventReceivers, true);

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

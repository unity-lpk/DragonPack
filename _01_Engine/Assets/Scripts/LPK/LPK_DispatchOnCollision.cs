/***************************************************
File:           LPK_DispatchOnCollision.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component detects when a collision occurs with
  another game object.  If the game object is valid as
  per specifications by the user, then an event is sent.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEditor;
using UnityEngine;

namespace LPK
{

/**
* CLASS NAME  : LPK_DispatchOnCollision
* DESCRIPTION : Dispatches collision events when the game object this component is on collides
*               with another game object.
**/
[RequireComponent(typeof(Collider2D))]
public sealed class LPK_DispatchOnCollision: LPK_Component
{
    /************************************************************************************/
    
    public enum LPK_CollisionCheckType
    {
        COLLISION_ENTER,
        COLLISION_EXIT,
        COLLISION_STAY,
    };

    /************************************************************************************/

    [Tooltip("What type of collision interaction will cause this component to activate the event.")]
    public LPK_CollisionCheckType m_CollisionCheckType;

    [Tooltip("Direct game objects which will activate the dispatcher.  If both properties are set to null any object will activate the event.  Note this is an OR search with the Activator Tags.")]
    public GameObject[] m_ActivatorGameObjects;

    [Tooltip("Tags to activate collisions.  If both properties are set to null any object will activate the event.  Note this is an OR search with the Activator Objects")]
    [TagDropdown]
    public string[] m_ActivatorTags;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when a valid collision occurs.")]
    public LPK_CollisionEventSendingInfo m_CollisionEvent;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : HACKHACK:  Used to make the checkbox to disable the component actually appear...
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
    }

    /**
    * FUNCTION NAME: OnCollisionEnter2D
    * DESCRIPTION  : Sends an event on colliding with another object if applicable.
    * INPUTS       : col - Collider information.  Not used for event sending.
    * OUTPUTS      : None
    **/
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnCollisionEnter
    * DESCRIPTION  : Sends an event on colliding with another object if applicable.
    * INPUTS       : col - Collider information.  Not used for event sending.
    * OUTPUTS      : None
    **/
    void OnCollisionEnter(Collision col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnCollisionStay2D
    * DESCRIPTION  : Sends an event on colliding with another object if applicable.
    * INPUTS       : col - Collider information.  Not used for event sending.
    * OUTPUTS      : None
    **/
    void OnCollisionStay2D(Collision2D col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_STAY)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnCollisionStay
    * DESCRIPTION  : Sends an event on colliding with another object if applicable.
    * INPUTS       : col - Collider information.  Not used for event sending.
    * OUTPUTS      : None
    **/
    void OnCollisionStay(Collision col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnCollisionExit2D
    * DESCRIPTION  : Sends an event on stop colliding with another object if applicable.
    * INPUTS       : col - Collider information.  Not used for event sending.
    * OUTPUTS      : None
    **/
    void OnCollisionExit2D(Collision2D col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_EXIT)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnCollisionExit
    * DESCRIPTION  : Sends an event on colliding with another object if applicable.
    * INPUTS       : col - Collider information.  Not used for event sending.
    * OUTPUTS      : None
    **/
    void OnCollisionExit(Collision col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnTriggerEnter2D
    * DESCRIPTION  : Sends an event on colldiing with a trigger object if applicable.
    * INPUTS       : col - Trigger collided with.
    * OUTPUTS      : None
    **/
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnTriggerEnter
    * DESCRIPTION  : Sends an event on colldiing with a trigger object if applicable.
    * INPUTS       : col - Trigger collided with.
    * OUTPUTS      : None
    **/
    void OnTriggerEnter(Collider col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnTriggerStay2D
    * DESCRIPTION  : Sends an event on colldiing with a trigger object if applicable.
    * INPUTS       : col - Trigger collided with.
    * OUTPUTS      : None
    **/
    void OnTriggerStay2D(Collider2D col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_STAY)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnTriggerStay
    * DESCRIPTION  : Sends an event on colldiing with a trigger object if applicable.
    * INPUTS       : col - Trigger collided with.
    * OUTPUTS      : None
    **/
    void OnTriggerStay(Collider col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnTriggerExit2D
    * DESCRIPTION  : Sends an event on stop colldiing with a trigger object if applicable.
    * INPUTS       : col - Trigger collided with.
    * OUTPUTS      : None
    **/
    void OnTriggerExit2D(Collider2D col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_EXIT)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: OnTriggerExit
    * DESCRIPTION  : Sends an event on colldiing with a trigger object if applicable.
    * INPUTS       : col - Trigger collided with.
    * OUTPUTS      : None
    **/
    void OnTriggerExit(Collider col)
    {
        if (!isActiveAndEnabled)
            return;

        if(m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
        {
            if (!CheckValidCollision(col.gameObject))
                return;

            DispatchCollisionEvent(col.gameObject);
        }
    }

    /**
    * FUNCTION NAME: DispatchCollisionEvent
    * DESCRIPTION  : Send out event for virtual button input.
    * INPUTS       : _other - Game object hit.  Used for OTHER sending mode.
    * OUTPUTS      : None
    **/
    void DispatchCollisionEvent(GameObject _other)
    {
        if(m_CollisionEvent != null && m_CollisionEvent.m_Event != null)
        {
            if(m_CollisionEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.ALL)
                m_CollisionEvent.m_Event.Dispatch(null);
            else if(m_CollisionEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OWNER)
                m_CollisionEvent.m_Event.Dispatch(gameObject);
            else if (m_CollisionEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OTHER)
                m_CollisionEvent.m_Event.Dispatch(_other);
            else if (m_CollisionEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.TAGS)
                m_CollisionEvent.m_Event.Dispatch(gameObject, m_CollisionEvent.m_Tags);
        
            if (m_bPrintDebug && m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_ENTER)
                LPK_PrintDebugDispatchingEvent(m_CollisionEvent, this, "COLLISION ENTER");
            else if (m_bPrintDebug && m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_EXIT)
                LPK_PrintDebugDispatchingEvent(m_CollisionEvent, this, "COLLISION EXIT");
            else if (m_bPrintDebug && m_CollisionCheckType == LPK_CollisionCheckType.COLLISION_STAY)
                LPK_PrintDebugDispatchingEvent(m_CollisionEvent, this, "COLLISION STAY");
        }
    }

    /**
    * FUNCTION NAME: ChecKValidCollision
    * DESCRIPTION  : Determines if a collision should result in event sending.
    * INPUTS       : _gameObj - Object to check for collision validation.
    * OUTPUTS      : bool - Result of validation check
    **/
    bool CheckValidCollision(GameObject _gameObj)
    {
        //None set - anything is valid.
        if (m_ActivatorGameObjects.Length < 1 && m_ActivatorTags.Length < 1)
            return true;

        //Not the right game object.
        for (int i = 0; i < m_ActivatorGameObjects.Length; i++)
        {
            if (_gameObj == m_ActivatorGameObjects[i])
                return true;
        }

        //Tag search.
        if(LPK_MultiTagManager.CheckGameObjectForTags(_gameObj, m_ActivatorTags))
            return true;

        //All conditions failed.
        return false;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnCollision))]
public class LPK_DispatchOnCollision2DEditor : Editor
{
    SerializedProperty m_CollisionCheckType;
    SerializedProperty m_ActivatorGameObjects;
    SerializedProperty m_ActivatorTags;

    SerializedProperty m_CollisionEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_CollisionCheckType = serializedObject.FindProperty("m_CollisionCheckType");
        m_ActivatorGameObjects = serializedObject.FindProperty("m_ActivatorGameObjects");
        m_ActivatorTags = serializedObject.FindProperty("m_ActivatorTags");

        m_CollisionEvent = serializedObject.FindProperty("m_CollisionEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnCollision owner = (LPK_DispatchOnCollision)target;

        LPK_DispatchOnCollision editorOwner = owner.GetComponent<LPK_DispatchOnCollision>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnCollision)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnCollision), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnCollision");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_CollisionCheckType, false);
        LPK_EditorArrayDraw.DrawArray(m_ActivatorGameObjects, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
        LPK_EditorArrayDraw.DrawArray(m_ActivatorTags, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        //Events
        EditorGUILayout.PropertyField(m_CollisionEvent, true);

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

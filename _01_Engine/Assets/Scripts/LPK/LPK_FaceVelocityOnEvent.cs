/***************************************************
File:           LPK_FaceVelocityOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   11/13/2019
Last Version:   2019.1.14

Description:
  This component causes an object to face its velocity
  when activated.

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
* CLASS NAME  : LPK_FaceVelocityOnEvent
* DESCRIPTION : Basic facing component.
**/
[RequireComponent(typeof(Rigidbody2D))]
public class LPK_FaceVelocityOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_FaceVelocityModes
    {
        DONT_FACE,
        ROTATE_TO_FACE,
        SNAP_TO_FACE,
    };

    /************************************************************************************/

    [Header("Component Properties")]

    [Tooltip("Whether this object should rotate to face the direction of movement.")]
    [Rename("Face Velocity")]
    public LPK_FaceVelocityModes m_eFaceVelocity = LPK_FaceVelocityModes.SNAP_TO_FACE;

    public float m_flRotationSpeed = 90.0f;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component to be active.")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    Transform m_cTransform;
    Rigidbody2D m_cRigidBody;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Checks to ensure proper components are on the object for facing.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
        m_cRigidBody = GetComponent<Rigidbody2D>();

        if (m_EventTrigger != null)
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

        Vector2 dir = m_cRigidBody.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        m_cTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        dir.Normalize();

        if (m_eFaceVelocity == LPK_FaceVelocityModes.SNAP_TO_FACE)
            m_cTransform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

        else if (m_eFaceVelocity == LPK_FaceVelocityModes.ROTATE_TO_FACE)
            m_cTransform.rotation = Quaternion.Slerp(m_cTransform.rotation, Quaternion.LookRotation(Vector3.forward, dir), Time.fixedDeltaTime * m_flRotationSpeed);

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Setting game object " + gameObject.name + "to face current velocity.");
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

[CustomEditor(typeof(LPK_FaceVelocityOnEvent))]
public class LPK_FaceVelocityOnEventEditor : Editor
{
    SerializedProperty m_eFaceVelocity;
    SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_eFaceVelocity = serializedObject.FindProperty("m_eFaceVelocity");
        m_EventTrigger = serializedObject.FindProperty("m_EventTrigger");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_FaceVelocityOnEvent owner = (LPK_FaceVelocityOnEvent)target;

        LPK_FaceVelocityOnEvent editorOwner = owner.GetComponent<LPK_FaceVelocityOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_FaceVelocityOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_FaceVelocityOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_FaceVelocity");

        //Component Properties

        EditorGUILayout.PropertyField(m_eFaceVelocity, true);

        if (m_eFaceVelocity.enumValueIndex == (int)LPK_FaceVelocityOnEvent.LPK_FaceVelocityModes.ROTATE_TO_FACE)
            EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "How many degrees per second to rotate to face the current velocity."), owner.m_flRotationSpeed);

        //Events
        EditorGUILayout.PropertyField(m_EventTrigger, true);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNITY_EDTIOR

}   //LPK

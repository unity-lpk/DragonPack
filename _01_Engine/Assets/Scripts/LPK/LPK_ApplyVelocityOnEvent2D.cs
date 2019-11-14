/***************************************************
File:           LPK_ApplyVelocityOnEvent2D
Authors:        Christopher Onorati
Last Updated:   11/13/2019
Last Version:   2019.1.14

Description:
  This component can be used to apply velocity to Rigidbody2D
  components when an event is received.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ApplyVelocityOnEvent2D
* DESCRIPTION : This component can be added to any game object with a RigidBody to make it receive a change in
*               velocity when an event is received.
**/
public class LPK_ApplyVelocityOnEvent2D : LPK_Component
{
    /************************************************************************************/

    public enum LPK_VelocityApplyMode
    {
        DIRECTION,
        TARGET,
    };

    /************************************************************************************/

    [Header("Component Properties")]

    [Tooltip("How to determine how velocity is applied to a game object when an event is received.  TARGET = Home in on a game object.  DIRECTION = Set velocity in a specific direction.")]
    [Rename("Velocity Application Mode")]
    public LPK_VelocityApplyMode m_eVelocityApplicationMode;

    [Tooltip("Game object to move towards.  If set to null, try to find the first object with the specified tag.")]
    [Rename("Target Game Object")]
    public Transform m_pTargetGameObject;

    [Tooltip("Tag to find a game object with to apply velocity towards.")]
    [TagDropdown]
    public string m_TargetTags;

    public float m_flRadius = 10.0f;

    [Tooltip("Direction to set the velocity of the RigidBody2D towards.  This value will be normalized.")]
    [Rename("Direction")]
    public Vector3 m_vecDirection;

    [Tooltip("RigidBody2D component to change the velocity on.")]
    public Rigidbody2D m_ModifyRigidBody2D;

    public float m_flSpeed = 5;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    [SerializeField]
    bool m_bHasSetup = false;

    /************************************************************************************/
    
    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets rigidbody component.
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
    * DESCRIPTION  : Set the default Rigidbody2D.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDrawGizmosSelected()
    {
        if (!m_bHasSetup)
        {   
            if(GetComponent<Rigidbody2D>())
                m_ModifyRigidBody2D = GetComponent<Rigidbody2D>();
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

        if(m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        if(m_ModifyRigidBody2D == null)
        {
            if(m_bPrintDebug)
                LPK_PrintWarning(this, "No RigidBody2D set to apply velocity on.");

            return;
        }

        ApplyVelocity();
    }

    /**
     * FUNCTION NAME: ApplyVelocity
     * DESCRIPTION  : Applies velocity to object.
     * INPUTS       : None
     * OUTPUTS      : None
     **/
    void ApplyVelocity()
    {
        if (m_eVelocityApplicationMode == LPK_VelocityApplyMode.TARGET)
        {
            if (m_pTargetGameObject == null)
            {
                if (!FindGameObject())
                    return;
            }

            m_ModifyRigidBody2D.velocity = (m_pTargetGameObject.position - m_ModifyRigidBody2D.transform.position).normalized * m_flSpeed;
        }

        else if (m_eVelocityApplicationMode == LPK_VelocityApplyMode.DIRECTION)
                m_ModifyRigidBody2D.velocity = m_vecDirection.normalized * m_flSpeed;       
    }

    /**
     * FUNCTION NAME: FindGameObject
     * DESCRIPTION  : Applies ongoing velocity if appropriate.
     * INPUTS       : None
     * OUTPUTS      : bool - True/false of if a game object was found and set.
     **/
    bool FindGameObject()
    {
        for (int i = 0; i < m_TargetTags.Length; i++)
        {
            List<GameObject> objects = new List<GameObject>();
            GetGameObjectsInRadius(objects, m_flRadius, 1, m_TargetTags);

            if(objects[0] != null)
            {
                m_pTargetGameObject = objects[0].transform;
                return true;
            }
        }

        return false;
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

[CustomEditor(typeof(LPK_ApplyVelocityOnEvent2D))]
public class LPK_ApplyVelocityOnEvent2DEditor : Editor
{
    SerializedProperty m_eVelocityApplicationMode;
    SerializedProperty targetObject;
    SerializedProperty targetTags;
    SerializedProperty m_vecDirection;
    SerializedProperty m_ModifyRigidBody2Ds;
    SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_eVelocityApplicationMode = serializedObject.FindProperty("m_eVelocityApplicationMode");
        targetObject = serializedObject.FindProperty("m_pTargetGameObject");
        targetTags = serializedObject.FindProperty("m_TargetTags");
        m_vecDirection = serializedObject.FindProperty("m_vecDirection");
        m_ModifyRigidBody2Ds = serializedObject.FindProperty("m_ModifyRigidBody2D");
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
        LPK_ApplyVelocityOnEvent2D owner = (LPK_ApplyVelocityOnEvent2D)target;

        LPK_ApplyVelocityOnEvent2D editorOwner = owner.GetComponent<LPK_ApplyVelocityOnEvent2D>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ApplyVelocityOnEvent2D)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ApplyVelocityOnEvent2D), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ApplyVelocityOnEvent2D");

        //Component properties.
        EditorGUILayout.PropertyField(m_eVelocityApplicationMode, true);

        if(m_eVelocityApplicationMode.enumValueIndex == (int)LPK_ApplyVelocityOnEvent2D.LPK_VelocityApplyMode.TARGET)
        {
            EditorGUILayout.PropertyField(targetObject, true);
            LPK_EditorArrayDraw.DrawArray(targetTags, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
            owner.m_flRadius = EditorGUILayout.FloatField(new GUIContent("Detect Radius", "Max distance used to search for game objects.  If set to 0, detect objects anywhere."), owner.m_flRadius);
        }
        else if (m_eVelocityApplicationMode.enumValueIndex == (int)LPK_ApplyVelocityOnEvent2D.LPK_VelocityApplyMode.DIRECTION)
            EditorGUILayout.PropertyField(m_vecDirection, true);

        EditorGUILayout.PropertyField(m_ModifyRigidBody2Ds, true);

        owner.m_flSpeed = EditorGUILayout.FloatField(new GUIContent("Speed", "Speed of the force to be applied."), owner.m_flSpeed);

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

#endif  //UNITY_EDITOR

}   //LPK

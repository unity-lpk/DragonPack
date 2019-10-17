/***************************************************
File:           LPK_DynamicTopDownRotationController.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component replicates the player controls of a tank 
  or space ship using keys to move forward / backward 
  and rotating. This uses a dynamic RigidBody.

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
* CLASS NAME  : LPK_DynamicTopDownRotationController
* DESCRIPTION : Implementation of a vehicle top down character controller.
**/
[RequireComponent(typeof(Transform), typeof(Rigidbody2D))]
public class LPK_DynamicTopDownRotationController : LPK_Component
{
    /************************************************************************************/

    public string m_sHorizontal = "Horizontal";
    public string m_sVertical = "Vertical";

    public float m_flRotationSpeed = 360.0f;
    public float m_flAccelerationSpeed = 5.0f;
    public float m_flMaxSpeed = 10.0f;

    [Tooltip("Drag factor to be applied to the character.  Higher values are less drag.")]
    [Rename("Drag Factor")]
    [Range(0, 1)]
    public float m_DragFactor = .98f;

    /************************************************************************************/

    [HideInInspector]
    public bool m_bNoclipping = false;

    [HideInInspector]
    public float m_flNoclipRigidBodyGravityScale;

    [HideInInspector]
    public bool m_bNoclipWasTrigger;

    /************************************************************************************/

    Transform m_cTransform;
    Rigidbody2D m_cRigidBody;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Checks to ensure proper components are on the object for movement.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
        m_cRigidBody = GetComponent<Rigidbody2D>();
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manages movement.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        //Limit the maximum movement speed
        m_cRigidBody.velocity = Vector3.ClampMagnitude(m_cRigidBody.velocity, m_flMaxSpeed);
        m_cRigidBody.velocity *= m_DragFactor;

        //Handle forward movement
        if (!string.IsNullOrEmpty(m_sVertical) && Input.GetAxis(m_sVertical) > 0)
        {
            m_cRigidBody.velocity += (Vector2)m_cTransform.up * m_flAccelerationSpeed;

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move forward.");
        }

        //Handle backward movement
        if (!string.IsNullOrEmpty(m_sVertical) && Input.GetAxis(m_sVertical) < 0)
        {
            m_cRigidBody.velocity -= (Vector2)m_cTransform.up * m_flAccelerationSpeed;

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move backwards.");
        }

        //Handle left rotation
        if (!string.IsNullOrEmpty(m_sHorizontal) && Input.GetAxis(m_sHorizontal) < 0)
        {
            m_cTransform.Rotate(new Vector3(0, 0, m_flRotationSpeed * Time.deltaTime));

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Rotate left.");
        }

        //Handle right rotation
        if (!string.IsNullOrEmpty(m_sHorizontal) && Input.GetAxis(m_sHorizontal) > 0)
        {
            m_cTransform.Rotate(new Vector3(0, 0, -m_flRotationSpeed * Time.deltaTime));

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Rotate right.");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DynamicTopDownRotationController))]
public class LPK_DynamicTopDownRotationControllerEditor : Editor
{
    SerializedProperty dragFactor;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        dragFactor = serializedObject.FindProperty("m_DragFactor");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DynamicTopDownRotationController owner = (LPK_DynamicTopDownRotationController)target;

        LPK_DynamicTopDownRotationController editorOwner = owner.GetComponent<LPK_DynamicTopDownRotationController>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DynamicTopDownRotationController)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DynamicTopDownRotationController), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DynamicTopDownRotationController");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_sHorizontal = EditorGUILayout.TextField(new GUIContent("Horizontal Input", "Virtual button used to rotate left and right."), owner.m_sHorizontal);
        owner.m_sVertical = EditorGUILayout.TextField(new GUIContent("Vertical Input", "Virtual button used to move up and down."), owner.m_sVertical);

        owner.m_flRotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "How fast the character will rotate."), owner.m_flRotationSpeed);
        owner.m_flAccelerationSpeed = EditorGUILayout.FloatField(new GUIContent("Acceleration Speed", "Speed the character will move at."), owner.m_flAccelerationSpeed);
        owner.m_flMaxSpeed = EditorGUILayout.FloatField(new GUIContent("Max Speed", "Maximum speed the character is allowed to move at."), owner.m_flMaxSpeed);
        EditorGUILayout.PropertyField(dragFactor, true);

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

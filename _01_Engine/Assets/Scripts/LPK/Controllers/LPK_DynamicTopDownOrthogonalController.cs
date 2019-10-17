/***************************************************
File:           LPK_DynamicTopDownOrthogonalController.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component replicates the movement functionality of 
  top-down characters moving orthogonally at constant 
  velocity. This uses a dynamic RigidBody.

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
* CLASS NAME  : LPK_DynamicTopDownOrthogonalController
* DESCRIPTION : Implementation of a basic top down character controller.
**/
[RequireComponent(typeof(Transform), typeof(Rigidbody2D))]
public class LPK_DynamicTopDownOrthogonalController : LPK_Component
{
    /************************************************************************************/

    public string m_sHorizontalInput = "Horizontal";
    public string m_sVerticalInput = "Vertical";

    public float m_flMoveSpeed = 8.0f;

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
    * DESCRIPTION  : Manages movement of the object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        bool bDidMove = false;

        //Variable to determine the movement direction
        Vector3 dir = Vector3.zero;

        //Handle absolute movement type (always orthogonal in the X and Y axis)
        if (!string.IsNullOrEmpty(m_sVerticalInput) && Input.GetAxis(m_sVerticalInput) > 0)
        {
            dir += Vector3.up;

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move forward.");
        }

        if (!string.IsNullOrEmpty(m_sHorizontalInput) && Input.GetAxis(m_sHorizontalInput) < 0)
        {
            dir -= Vector3.right;

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move left.");
        }

        if (!string.IsNullOrEmpty(m_sVerticalInput) && Input.GetAxis(m_sVerticalInput) < 0)
        {
            dir -= Vector3.up;

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move down.");
        }

        if (!string.IsNullOrEmpty(m_sHorizontalInput) && Input.GetAxis(m_sHorizontalInput) > 0)
        {
            dir += Vector3.right;

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move right.");
        }

        if (dir != Vector3.zero)
            bDidMove = true;

        if (bDidMove)
            dir.Normalize();

        //Apply velocity
        m_cRigidBody.velocity = dir.normalized * m_flMoveSpeed;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DynamicTopDownOrthogonalController))]
public class LPK_DynamicTopDownOrthogonalControllerEditor : Editor
{
    SerializedProperty faceVelocity;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        faceVelocity = serializedObject.FindProperty("m_eFaceVelocity");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DynamicTopDownOrthogonalController owner = (LPK_DynamicTopDownOrthogonalController)target;

        LPK_DynamicTopDownOrthogonalController editorOwner = owner.GetComponent<LPK_DynamicTopDownOrthogonalController>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DynamicTopDownOrthogonalController)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DynamicTopDownOrthogonalController), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DynamicTopDownOrthogonalController");
        
        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_sHorizontalInput = EditorGUILayout.TextField(new GUIContent("Horizontal Input", "Virtual button used to move left and right."), owner.m_sHorizontalInput);
        owner.m_sVerticalInput = EditorGUILayout.TextField(new GUIContent("Vertical Input", "Virtual button used to move up and down backwards."), owner.m_sVerticalInput);
        owner.m_flMoveSpeed = EditorGUILayout.FloatField(new GUIContent("Move Speed", "Speed at which the character will move."), owner.m_flMoveSpeed);
        
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

}

/***************************************************
File:           LPK_FollowMouse.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to cause it to 
  follow the mouse position. This can be used to create a 
  custom mouse cursor.

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
* CLASS NAME  : LPK_FollowMouse
* DESCRIPTION : Component to force an object to follow the mouse.
**/
[RequireComponent(typeof(Transform))]
public class LPK_FollowMouse : LPK_Component
{
    /************************************************************************************/

    public float m_flZDepth = 0.0f;

    public Vector3 m_vecOffset;

    [Tooltip("What percentage of the distance between the current position and the target's should the object move every frame.")]
    [Range(0, 1)]
    public float m_InterpolationFactor = 0.1f;

    /************************************************************************************/

    Transform m_cTransform;
    Camera m_cMainCamera;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up event listening if necessary.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
        m_cMainCamera = Camera.main;
    }

    /**
    * FUNCTION NAME: FixedUpdate
    * DESCRIPTION  : Manages movement of object to follow the mouse.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FixedUpdate()
    {
        if (m_cMainCamera == null)
        {
            m_cMainCamera = Camera.main;

            if (m_bPrintDebug && m_cMainCamera)
                LPK_PrintDebug(this, "Main camera was lost for LPK_FollowMouse.  Attempted to find a new camera succeeded.");
            else if (m_bPrintDebug)
                LPK_PrintDebug(this, "Main camera was lost for LPK_FollowMouse.  Attempted to find a new camera failed.");  
        }

        Vector3 mousePos = m_cMainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = m_flZDepth;
        mousePos += m_vecOffset;

        m_cTransform.position = Vector3.Lerp(m_cTransform.position, mousePos, m_InterpolationFactor);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_FollowMouse))]
public class LPK_FollowMouseEditor : Editor
{
    SerializedProperty interpolationFactor;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        interpolationFactor = serializedObject.FindProperty("m_InterpolationFactor");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_FollowMouse owner = (LPK_FollowMouse)target;

        LPK_FollowMouse editorOwner = owner.GetComponent<LPK_FollowMouse>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_FollowMouse)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_FollowMouse), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_FollowMouse");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_flZDepth = EditorGUILayout.FloatField(new GUIContent("Z Depth", "At what Z depth should the object be relocated to."), owner.m_flZDepth);
        owner.m_vecOffset = EditorGUILayout.Vector3Field(new GUIContent("Offset", "Offset to apply to mouse position when setting translation."), owner.m_vecOffset);
        EditorGUILayout.PropertyField(interpolationFactor, true);

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

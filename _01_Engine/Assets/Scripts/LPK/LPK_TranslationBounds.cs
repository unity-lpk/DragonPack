/***************************************************
File:           LPK_TranslationBounds.cs
Authors:        Christopher Onorati
Last Updated:   8/27/2019
Last Version:   2019.1.14

Description:
  This component keeps its owner within the specified
  bounds on Translation. It depends on initialization
  order to be properly compatible with other components,
  such as LPK_FollowObject.

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
* CLASS NAME  : LPK_TranslationBounds
* DESCRIPTION : Locks an object to a specified bounds.
**/
[RequireComponent(typeof(Transform))]
public class LPK_TranslationBounds : LPK_Component
{
    /************************************************************************************/

    public Vector3 m_vecMin = new Vector3( -10, -10, -10 );
    public Vector3 m_vecMax = new Vector3(10, 10, 10);

    public bool m_bLocal = false;

    /************************************************************************************/
    private Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up the component dependencies.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
    }

    /**
    * FUNCTION NAME: FixedUpdate
    * DESCRIPTION  : Manages clamping of translations.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FixedUpdate()
    {
        Vector3 vecModifiedTransform;

        if (m_bLocal)
            vecModifiedTransform = m_cTransform.localPosition;
        else
            vecModifiedTransform = m_cTransform.position;

        vecModifiedTransform.x = Mathf.Clamp(vecModifiedTransform.x, m_vecMin.x, m_vecMax.x);
        vecModifiedTransform.y = Mathf.Clamp(vecModifiedTransform.y, m_vecMin.y, m_vecMax.y);
        vecModifiedTransform.z = Mathf.Clamp(vecModifiedTransform.z, m_vecMin.z, m_vecMax.z);

        if (m_bLocal)
            m_cTransform.localPosition = vecModifiedTransform;
        else
            m_cTransform.position = vecModifiedTransform;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_TranslationBounds))]
public class LPK_TranslationBoundsEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_TranslationBounds owner = (LPK_TranslationBounds)target;

        LPK_TranslationBounds editorOwner = owner.GetComponent<LPK_TranslationBounds>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_TranslationBounds)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_TranslationBounds), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_TranslationBounds");


        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_vecMin = EditorGUILayout.Vector3Field(new GUIContent("Min Bounds", "The minimum value that the translation can reach."), owner.m_vecMin);
        owner.m_vecMax = EditorGUILayout.Vector3Field(new GUIContent("Max Bounds", "The maximum value that the translation can reach."), owner.m_vecMax);
        owner.m_bLocal = EditorGUILayout.Toggle(new GUIContent("Local", "Whether this component should constrain local position or world position."), owner.m_bLocal);

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

/***************************************************
File:           LPK_FollowGameObject.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to cause it to 
  follow another object's position.  Note this does so
  without any parenting.
  

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
* CLASS NAME  : LPK_FollowGameObject
* DESCRIPTION : Component to force an object to follow another object's movements (pseudo parenting).
**/
[RequireComponent(typeof(Transform))]
public class LPK_FollowGameObject : LPK_Component
{
    /************************************************************************************/

    public enum LPK_FollowType
    {
        ANCHOR_POINT,
        BELOW,
        ABOVE,
        LEFT,
        RIGHT,
    }

    /************************************************************************************/

    [Tooltip("Game object to follow.  If deleted or set to null, this script will try to find a tagged object to follow.")]
    [Rename("Follow Object")]
    public GameObject m_pCurFollowObj;

    [Tooltip("Tag to search for to find a game object to follow.")]
    [TagDropdown]
    public string m_TargetFollowTag = "";

    [Tooltip("How this object will behave when following its target.")]
    [Rename("Follow Type")]
    public LPK_FollowType m_eFollowType;

    public Vector3 m_vecOffset;
    public float m_flOffset = 4;

    [Tooltip("What percentage of the distance between my current position and the target's should I move every frame.")]
    [Range(0, 1)]
    public float m_InterpolationFactor = 0.1f;

    public bool m_bBecomeChild = false;

    /************************************************************************************/

    Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up event listening if necessary.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();

        if(string.IsNullOrEmpty(m_TargetFollowTag))
        {
            if (m_bPrintDebug)
                LPK_PrintError(this, "No string set as a follow object!");
        }
    }

    /**
    * FUNCTION NAME: FixedUpdate
    * DESCRIPTION  : Manages movement of object following.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FixedUpdate()
    {
        if (m_pCurFollowObj == null)
            FindFollowObject();

        //No object was able to be found.
        if (m_pCurFollowObj == null)
            return;

        //If the target object doesnt exist or doesnt have a tranform, do nothing.
        if (m_pCurFollowObj == null || m_pCurFollowObj.transform == null)
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "No object set to follow.  Set in inspector or use a different event activator.");

            return;
        }

        Vector3 targetPos = new Vector3();

        if (m_eFollowType == LPK_FollowType.ANCHOR_POINT)
            targetPos = m_pCurFollowObj.transform.position + m_vecOffset;
        else if(m_eFollowType == LPK_FollowType.ABOVE)
            targetPos = m_pCurFollowObj.transform.position + m_pCurFollowObj.transform.up.normalized * m_flOffset;
        else if (m_eFollowType == LPK_FollowType.BELOW)
            targetPos = m_pCurFollowObj.transform.position + -m_pCurFollowObj.transform.up.normalized * m_flOffset;
        else if (m_eFollowType == LPK_FollowType.LEFT)
            targetPos = m_pCurFollowObj.transform.position + -m_pCurFollowObj.transform.right.normalized * m_flOffset;
        else if (m_eFollowType == LPK_FollowType.RIGHT)
            targetPos = m_pCurFollowObj.transform.position + m_pCurFollowObj.transform.right.normalized * m_flOffset;

        //Interpolate from current position to target object's position
        m_cTransform.position = Vector3.Lerp(m_cTransform.position, targetPos, m_InterpolationFactor);
    }

    /**
    * FUNCTION NAME: FindFollowObject
    * DESCRIPTION  : Sets the ideal object to follow.  Will always be the first object with the tag found.  As such
    *                the tag used to find while following should only ever exist once in a scene.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FindFollowObject()
    {
        if (string.IsNullOrEmpty(m_TargetFollowTag))
            return;

        m_pCurFollowObj = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_TargetFollowTag);

        if(LPK_MultiTagManager.FindGameObjectsWithTag(gameObject, m_TargetFollowTag).Count > 1 && m_bPrintDebug)
                LPK_PrintWarning(this, "WARNNG: Undefined behavior for follow object selection!  Multiple game objects found with the tag: " + m_TargetFollowTag + 
                                 "Please note that in a build, it is undefined which game object will be selected.");

        if (m_bBecomeChild)
            m_cTransform.SetParent(m_pCurFollowObj.transform);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_FollowGameObject))]
public class LPK_FollowObjectEditor : Editor
{
    SerializedProperty curFollowObj;
    SerializedProperty targetFollowTag;
    SerializedProperty followType;
    SerializedProperty interpolationFactor;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        curFollowObj = serializedObject.FindProperty("m_pCurFollowObj");
        targetFollowTag = serializedObject.FindProperty("m_TargetFollowTag");
        followType = serializedObject.FindProperty("m_eFollowType");
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
        LPK_FollowGameObject owner = (LPK_FollowGameObject)target;

        LPK_FollowGameObject editorOwner = owner.GetComponent<LPK_FollowGameObject>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_FollowGameObject)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_FollowGameObject), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_FollowGameObject");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(curFollowObj, true);
        EditorGUILayout.PropertyField(targetFollowTag, true);
        EditorGUILayout.PropertyField(followType, true);

        if(followType.intValue == (int)LPK_FollowGameObject.LPK_FollowType.ANCHOR_POINT)
            owner.m_vecOffset = EditorGUILayout.Vector3Field(new GUIContent("Anchor Offset", "What offset to keep from the followed object.  Used for ANCHOR_POINT."), owner.m_vecOffset);

        owner.m_flOffset = EditorGUILayout.FloatField(new GUIContent("Directional Offset", "What offset to keep from the followed object.Used for every follow type == BUT == ANCHOR_POINT."), owner.m_flOffset);
        EditorGUILayout.PropertyField(interpolationFactor, true);
        owner.m_bBecomeChild = EditorGUILayout.Toggle(new GUIContent("Become Child", "Set this game object to become a child of whatever it is currently following."), owner.m_bBecomeChild);

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

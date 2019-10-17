/***************************************************
File:           LPK_RotateTowardsGameObjectOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/27/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to cause it to 
  rotate itself to face the position of a specified object 
  either every frame or just once.

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
* CLASS NAME  : LPK_RotateTowardsGameObjectOnEvent
* DESCRIPTION : Component used to cause a game object to always face another game object.
**/
public class LPK_RotateTowardsGameObjectOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Initial game object to rotate towards.  If deleted or set to null, this script will try to find a tagged game object to face.")]
    [Rename("Initial Transform Game Object")]
    public GameObject m_pTargetTransformObject;

    [Tooltip("Tag to search for to find a game object to face.")]
    [TagDropdown]
    public string m_TargetFacingTag;

    public float m_flRotationSpeed = 360.0f;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component to be active.")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Stops rotating from starting by default if set.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();

        if (string.IsNullOrEmpty(m_TargetFacingTag) && m_pTargetTransformObject == null)
        {
            if (m_bPrintDebug)
                LPK_PrintError(this, "No game object set to follow!");
        }

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
        if (!ShouldRespondToEvent(_activator))
            return;

        if (m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        RotateToFace();
    }

    /**
    * FUNCTION NAME: RotateToFace
    * DESCRIPTION  : Manages rotation of objects.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void RotateToFace()
    {
        if (m_pTargetTransformObject == null)
            FindFacingObject();

        if (m_pTargetTransformObject != null && m_pTargetTransformObject.transform != null)
        {
            //Look at desired object.

            Vector3 diff = m_pTargetTransformObject.transform.position - m_cTransform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            m_cTransform.rotation = Quaternion.RotateTowards(m_cTransform.rotation, targetRotation, Time.deltaTime * m_flRotationSpeed);

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Rotation Applied");
        }
        else
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Invalid Transform Path");
        }
    }

    /**
    * FUNCTION NAME: FindFacingObject
    * DESCRIPTION  : Sets the ideal object to face.  Will always be the first object with the tag found.  As such
    *                the tag used to find while following should only ever exist once in a scene.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FindFacingObject()
    {
        if (string.IsNullOrEmpty(m_TargetFacingTag))
            return;

        m_pTargetTransformObject = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_TargetFacingTag);

        if(LPK_MultiTagManager.FindGameObjectsWithTag(gameObject, m_TargetFacingTag).Count > 1 && m_bPrintDebug)
            LPK_PrintWarning(this, "WARNNG: Undefined behavior for target facing!  Multiple game objects found with the tag: " + m_TargetFacingTag + 
                             "Please note that in a build, it is undefined which game object will be selected.");
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Removes game object from the event queue.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        if (m_EventTrigger != null)
            m_EventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_RotateTowardsGameObjectOnEvent))]
public class LPK_RotateTowardsGameObjectOnEventEditor : Editor
{
    SerializedProperty targetObject;
    SerializedProperty targetFacingTag;

        SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        targetObject = serializedObject.FindProperty("m_pTargetTransformObject");
        targetFacingTag = serializedObject.FindProperty("m_TargetFacingTag");

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
        LPK_RotateTowardsGameObjectOnEvent owner = (LPK_RotateTowardsGameObjectOnEvent)target;

        LPK_RotateTowardsGameObjectOnEvent editorOwner = owner.GetComponent<LPK_RotateTowardsGameObjectOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_RotateTowardsGameObjectOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_RotateTowardsGameObjectOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_RotateTowardsGameObject");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(targetObject, true);
        EditorGUILayout.PropertyField(targetFacingTag, true);
        owner.m_flRotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "Speed to rotate towards end goal."), owner.m_flRotationSpeed);

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

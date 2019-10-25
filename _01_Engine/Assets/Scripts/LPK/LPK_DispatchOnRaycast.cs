/***************************************************
File:           LPK_DispatchOnRaycast.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component checks the LOS between two gameobjects.
  Note both gameobjects need to have colliders to be detected.

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
* CLASS NAME  : LPK_DispatchOnRaycast
* DESCRIPTION : Check the line of sight between two objects with a raycast.
**/
public class LPK_DispatchOnRaycast : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Game Object that is casting the ray.  This is useful for prefab setup.")]
    [Rename("Source Game Object Transform")]
    public Transform m_pSource;

    [Tooltip("Game Objects the source is casting rays towards.")]
    public GameObject[] m_Targets;

    [Tooltip("Tags to try to cast a ray towards.")]
    [TagDropdown]
    public string[] m_TargetTags;

    public float m_flDistance = Mathf.Infinity;

    [Tooltip("Layer mask to use for filtering.  Leave empty to collider with everything.")]
    [Rename("Layer Mask")]
    public LayerMask m_layerMask = ~0;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the raycast starts hitting a target game object.")]
    public LPK_CollisionEventSendingInfo m_RaycastHitEvent;

    [Tooltip("Event sent when the raycast continues to hit a target game object.")]
    public LPK_CollisionEventSendingInfo m_RaycastMaintainedEvent;

    [Tooltip("Event sent when the raycast stops detecting a target game object.")]
    public LPK_CollisionEventSendingInfo m_RaycastLostEvent;

    /************************************************************************************/

    //Stores currently found game objects.
    List<GameObject> m_pFoundObjects = new List<GameObject>();

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    [SerializeField]
    bool m_bHasSetup = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up event detection for toggling active state.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if (m_pSource == null)
            m_pSource = gameObject.transform;
    }

    /**
    * FUNCTION NAME: OnDrawGizmosSelected
    * DESCRIPTION  : Set the default game object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDrawGizmosSelected()
    {
        if (!m_bHasSetup)
            m_pSource = gameObject.transform;
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Sends out raycasts to the objects we are trying to establish a line of sight for.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        //Clean the list
        for(int i = 0; i < m_pFoundObjects.Count; i++)
        {
            if (m_pFoundObjects[i] == null)
                m_pFoundObjects.RemoveAt(i);
        }


        //Direct game objects.
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if(m_Targets[i] != null && m_Targets[i] != gameObject)
                CheckLOS(m_pSource, m_Targets[i].transform);
            else if(m_Targets[i] != gameObject)
            {
                m_Targets[i] = gameObject;

                if(m_pFoundObjects.Count > i)
                    m_pFoundObjects.RemoveAt(i);

                DispatchLostEvent(null);
            }
        }

        //Tags
        for(int i = 0; i < m_TargetTags.Length; i++)
        {
            List<GameObject> taggedGameObjects = new List<GameObject>();
            GetGameObjectsInRadius(taggedGameObjects, m_flDistance, -1, m_TargetTags[i]);
            
            for(int j = 0; j < taggedGameObjects.Count; j++)
            {
                if(taggedGameObjects[j] != gameObject)
                    CheckLOS(m_pSource, taggedGameObjects[j].transform);
                else if(taggedGameObjects[j] != gameObject)
                {
                    if(m_pFoundObjects.Count > j)
                        m_pFoundObjects.Remove(taggedGameObjects[j]);

                    DispatchLostEvent(null);
                }
            }
        }
    }

    /**
    * FUNCTION NAME: CheckLOS
    * DESCRIPTION  : Tests LOS for each object we are tracking.
    * INPUTS       : _source - The object doing the looking.
    *                _target - The object the source is looking for.
    * OUTPUTS      : None
    **/
    void CheckLOS(Transform _source, Transform _target)
    {
        bool bCanSee = false;
        
        Vector3 dir = _target.position - _source.position;
        dir.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(_source.position, dir, m_flDistance, m_layerMask);

        if (!hit.collider)
            return;

        //Epsilon of 0.05f
        if (hit.collider.gameObject.transform == _target)
            bCanSee = true;

        //Target has been found
        if (bCanSee && !m_pFoundObjects.Find(obj => obj.name == _target.name))
            DispatchFoundEvent(_target.gameObject);

        else if (bCanSee && m_pFoundObjects.Find(obj => obj.name == _target.name))
            DispatchMaintainEvent(_target.gameObject);

        //Target has been lost.
        else if (!bCanSee && _target != null && m_pFoundObjects.Find(obj => obj.name == _target.name))
            DispatchLostEvent(_target.gameObject);
    }

    /**
    * FUNCTION NAME: DispatchFoundEvent
    * DESCRIPTION  : Dispatches the established LOS event.
    * INPUTS       : _target - Game object to add to the found objects list.
    * OUTPUTS      : None
    **/
    void DispatchFoundEvent(GameObject _target)
    {
        //This object has been found.
        m_pFoundObjects.Add(_target);

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Line of sight established between " + m_pSource.name + " and " + _target);

        if(m_RaycastHitEvent != null && m_RaycastHitEvent.m_Event != null)
        {
            if(m_RaycastHitEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.ALL)
                m_RaycastHitEvent.m_Event.Dispatch(null);
            else if(m_RaycastHitEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OWNER)
                m_RaycastHitEvent.m_Event.Dispatch(gameObject);
            else if (m_RaycastHitEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OTHER)
                m_RaycastHitEvent.m_Event.Dispatch(_target);
            else if (m_RaycastHitEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.TAGS)
                m_RaycastHitEvent.m_Event.Dispatch(gameObject, m_RaycastHitEvent.m_Tags);
        
            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_RaycastHitEvent, this, "Raycast Hit");
        }
    }

    /**
    * FUNCTION NAME: DispatchMaintainEvent
    * DESCRIPTION  : Dispatches the maintained LOS event.
    * INPUTS       : _target - Object that was seen.  Only used for debug logging here.
    * OUTPUTS      : None
    **/
    void DispatchMaintainEvent(GameObject _target)
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Line of sight maintained between " + m_pSource.name + " and " + _target);

        if(m_RaycastMaintainedEvent != null && m_RaycastMaintainedEvent.m_Event != null)
        {
            if(m_RaycastMaintainedEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.ALL)
                m_RaycastMaintainedEvent.m_Event.Dispatch(null);
            else if(m_RaycastMaintainedEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OWNER)
                m_RaycastMaintainedEvent.m_Event.Dispatch(gameObject);
            else if (m_RaycastMaintainedEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OTHER)
                m_RaycastMaintainedEvent.m_Event.Dispatch(_target);
            else if (m_RaycastMaintainedEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.TAGS)
                m_RaycastMaintainedEvent.m_Event.Dispatch(gameObject, m_RaycastMaintainedEvent.m_Tags);
        
            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_RaycastMaintainedEvent, this, "Raycast Maintained");
        }
    }

    /**
    * FUNCTION NAME: DispatchFoundEvent
    * DESCRIPTION  : Dispatches the lost LOS event.
    * INPUTS       : _target - Game object to remove from the found objects list.
    * OUTPUTS      : None
    **/
    void DispatchLostEvent(GameObject _target)
    {
        //This object has been lost.
        m_pFoundObjects.Remove(_target);

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Raycast missed between " + m_pSource.name + " and " + _target);

        if(m_RaycastLostEvent != null && m_RaycastLostEvent.m_Event != null)
        {
            if(m_RaycastLostEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.ALL)
                m_RaycastLostEvent.m_Event.Dispatch(null);
            else if(m_RaycastLostEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OWNER)
                m_RaycastLostEvent.m_Event.Dispatch(gameObject);
            else if (m_RaycastLostEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.OTHER)
                m_RaycastLostEvent.m_Event.Dispatch(_target);
            else if (m_RaycastLostEvent.m_EventSendingMode == LPK_CollisionEventSendingInfo.LPK_EventSendingMode.TAGS)
                m_RaycastLostEvent.m_Event.Dispatch(gameObject, m_RaycastLostEvent.m_Tags);
        
            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_RaycastLostEvent, this, "Raycast Lost");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnRaycast))]
public class LPK_DispatchOnRaycastEditor : Editor
{
    SerializedProperty source;
    SerializedProperty ctargets;
    SerializedProperty m_TargetTags;
    SerializedProperty layerMask;

    SerializedProperty m_RaycastHitEvent;
    SerializedProperty m_RaycastMaintainedEvent;
    SerializedProperty m_RaycastLostEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        source = serializedObject.FindProperty("m_pSource");
        ctargets = serializedObject.FindProperty("m_Targets");
        m_TargetTags = serializedObject.FindProperty("m_TargetTags");
        layerMask = serializedObject.FindProperty("m_layerMask");

        m_RaycastHitEvent = serializedObject.FindProperty("m_RaycastHitEvent");
        m_RaycastMaintainedEvent = serializedObject.FindProperty("m_RaycastMaintainedEvent");
        m_RaycastLostEvent = serializedObject.FindProperty("m_RaycastLostEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnRaycast owner = (LPK_DispatchOnRaycast)target;

        LPK_DispatchOnRaycast editorOwner = owner.GetComponent<LPK_DispatchOnRaycast>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnRaycast)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnRaycast), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnRaycast");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(source, true);
        LPK_EditorArrayDraw.DrawArray(ctargets, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
        LPK_EditorArrayDraw.DrawArray(m_TargetTags, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
        owner.m_flDistance = EditorGUILayout.FloatField(new GUIContent("Search Distance", "How far to cast the ray.  By default, look forever."), owner.m_flDistance);
        EditorGUILayout.PropertyField(layerMask, true);

        //Events
        EditorGUILayout.PropertyField(m_RaycastHitEvent, true);
        EditorGUILayout.PropertyField(m_RaycastMaintainedEvent, true);
        EditorGUILayout.PropertyField(m_RaycastLostEvent, true);

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

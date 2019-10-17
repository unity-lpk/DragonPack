/***************************************************
File:           LPK_AggressivePathFollower.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component causes its gameobject to follow a pre-determined
  path by the designer.  This object will also be aggressive to
  a set GameObject or tag.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using System.Runtime.CompilerServices;  /*  Method Attributes */
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_AggressivePathFollower
* DESCRIPTION : Sets an object to follow nodes along a path, while also being territorial.
*               Note this gameobject moves via transform so will not be stopped by physics
*               objects or walls.
**/
[RequireComponent(typeof(Transform))]
public class LPK_AggressivePathFollower : LPK_Component
{
    /************************************************************************************/

    public enum LPK_PathFollowerLoopType
    {
        SINGLE,
        LOOP,
        LOOP_TELEPORT,
        LOOP_BACKTRACK,
    };

    /************************************************************************************/

    [Tooltip("How to handle path looping once the end of the path is hit.")]
    [Rename("Loop Type")]
    public LPK_PathFollowerLoopType m_eLoopType;

    [System.Serializable]
    public class AggressionProperties
    {
        [Tooltip("Enemies to attack/run from if set.")]
        public Transform[] m_Enemies;

        [Tooltip("Tag enemies will have.  Useful if this path follower should react to multiple different GameObjects.")]
        [TagDropdown]
        public string[] m_EnemyTag;

        [Tooltip("Distance at which the character becomes aggressive.")]
        [Rename("Aggression Range")]
        public float m_flAggressionRange = 10.0f;

        [Tooltip("Speed (units per second) the object will move at while aggressive.")]
        [Rename("Aggression Speed")]
        public float m_flAgressionSpeed = 5.0f;

        [Tooltip("Run from the enemy instead of running towards the enemy.")]
        [Rename("Is Coward")]
        public bool m_bIsCoward;

        [Tooltip("Once an enemy is found, do not forget about it.")]
        [Rename("Don't Forget")]
        public bool m_bDontForget;
    }

    public AggressionProperties m_AggressionProperties;

    public float m_flSpeed = 5.0f;

    [Tooltip("Nodes that make up the path.")]
    public Transform[] m_Nodes;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the path follower notices an enemy.")]
    public LPK_EventSendingInfo m_PathFollowerNoticeEnemyEvent;

    [Tooltip("Event sent when the path follower looses an enemy.")]
    public LPK_EventSendingInfo m_PathFollowerLostEnemyEvent;

    [Tooltip("Event sent when the path follower reaches a node in its path.")]
    public LPK_EventSendingInfo m_PathFollowerReachNodeEvent;

    [Tooltip("Event sent when the path follower reaches the final node in its current path.")]
    public LPK_EventSendingInfo m_PathFollowerReachFinalNodeEvent;

    /************************************************************************************/

    //Keep track of which object to move towards.
    int m_iCounter = 0;

    //Flag to detect when the path follower has reached a node.
    bool m_bReachedNode = false;

    //Flag used for LOOP_BACKTRACK type.  
    bool m_bGoingBackwards = false;

    //Enemy to move towards.
    Transform m_pCurEnemyPos;

    /************************************************************************************/

    Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up event listening.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
    }

    /**
    * FUNCTION NAME: FixedUpdate
    * DESCRIPTION  : Manages object movement and looping.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FixedUpdate()
    {
        bool m_bEnemyInRange = false;

        //Optimize to not have to call notice enemy as often.
        if(m_pCurEnemyPos && Vector3.Distance(m_cTransform.position, m_pCurEnemyPos.position) <= m_AggressionProperties.m_flAggressionRange)
            m_bEnemyInRange = true;
        else
            m_bEnemyInRange = NoticeEnemy();

        if ((m_pCurEnemyPos && m_AggressionProperties.m_bDontForget) || m_bEnemyInRange)
            HuntEnemy();
        else
        {
            if (m_pCurEnemyPos != null)
            {
                m_pCurEnemyPos = null;
                DispatchLostEvent();
            }

            MoveAlongPath();
            DetectReachNode();
        }
    }

    /**
    * FUNCTION NAME: OnEvent
    * DESCRIPTION  : Manages object movement towards the next node in the path.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    [MethodImpl(256)]
    void MoveAlongPath()
    {
        if (m_bReachedNode)
            return;

        if (m_Nodes.Length <= 0)
            return;

        m_cTransform.position = Vector3.MoveTowards(m_cTransform.position, m_Nodes[m_iCounter].position, Time.deltaTime * m_flSpeed);

        if (m_cTransform.position == m_Nodes[m_iCounter].position)
        {
            m_bReachedNode = true;

            //Dispatch reach node event.
            DispatchReachNodeEvent();

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Reached Node");
        }
    }

    /**
    * FUNCTION NAME: NoticeEnemy
    * DESCRIPTION  : Detect if an enemy has been noticed based on user specified detection paramaters.
    * INPUTS       : None
    * OUTPUTS      : bool - Detection state.
    **/
    bool NoticeEnemy()
    {
        //Hunt specific enemy.
        for (int i = 0; i < m_AggressionProperties.m_Enemies.Length; i++)
        {
            if (m_AggressionProperties.m_Enemies[i] != null
                && Vector3.Distance(m_cTransform.position, m_AggressionProperties.m_Enemies[i].position) <= m_AggressionProperties.m_flAggressionRange)
            {
                m_pCurEnemyPos = m_AggressionProperties.m_Enemies[i];
                DispatchNoticeEvent();
                return true;
            }
        }

        //Hunt enemy tag.
        List<GameObject> taggedObjects = LPK_MultiTagManager.FindGameObjectsWithTags(gameObject, m_AggressionProperties.m_EnemyTag, true);

        for(int i = 0; i < taggedObjects.Count; i++)
        {
            if (Vector3.Distance(m_cTransform.position, taggedObjects[i].transform.position) <= m_AggressionProperties.m_flAggressionRange)
            {
                m_pCurEnemyPos = taggedObjects[i].transform;
                DispatchNoticeEvent();
                return true;
            }
        }

        return false;
    }

    /**
    * FUNCTION NAME: HuntEnemy
    * DESCRIPTION  : Manages the character hunting its target.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    [MethodImpl(256)]
    void HuntEnemy()
    {
        if(!m_AggressionProperties.m_bIsCoward)
            m_cTransform.position = Vector3.MoveTowards(m_cTransform.position, m_pCurEnemyPos.position, Time.deltaTime * m_AggressionProperties.m_flAgressionSpeed);
        else
            m_cTransform.position = Vector3.MoveTowards(m_cTransform.position, m_pCurEnemyPos.position, Time.deltaTime * -m_AggressionProperties.m_flAgressionSpeed);
    }

    /**
    * FUNCTION NAME: DetectReachNode
    * DESCRIPTION  : Manage behavior once reaching a node.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    [MethodImpl(256)]
    void DetectReachNode()
    {
        if (!m_bReachedNode)
            return;

        if (!m_bGoingBackwards)
            m_iCounter++;
        else
            m_iCounter--;

        //Final node reached event sending.
        if (m_iCounter >= m_Nodes.Length)
            DispatchReachFinalNodeEvent();

        //Move towards the start of the path manually.
        if (m_iCounter >= m_Nodes.Length && m_eLoopType == LPK_PathFollowerLoopType.LOOP)
            m_iCounter = 0;

        //Teleport to the beggining of the path and resume.
        else if (m_iCounter >= m_Nodes.Length && m_eLoopType == LPK_PathFollowerLoopType.LOOP_TELEPORT)
        {
            m_iCounter = 1;
            m_cTransform.position = m_Nodes[0].transform.position;
        }

        //Begin going backwards down the path.
        else if ((m_iCounter >= m_Nodes.Length && m_eLoopType == LPK_PathFollowerLoopType.LOOP_BACKTRACK)
                 || (m_iCounter < 0 && m_bGoingBackwards))
        {
            if (!m_bGoingBackwards)
                m_iCounter = m_Nodes.Length - 2;
            else
            {
                //NOTENOTE: Technically the start of the path is now also an end of track, so we call the event here as well.
                DispatchReachFinalNodeEvent();

                m_iCounter = 1;
            }

            m_bGoingBackwards = !m_bGoingBackwards;
        }

        //Reset and move again.
        if (m_iCounter < m_Nodes.Length)
            m_bReachedNode = false;
    }

    /**
    * FUNCTION NAME: DispatchReachNodeEvent
    * DESCRIPTION  : Send out reach node event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchReachNodeEvent()
    {
        if(m_PathFollowerReachNodeEvent != null && m_PathFollowerReachNodeEvent.m_Event != null)
        {
            if(m_PathFollowerReachNodeEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_PathFollowerReachNodeEvent.m_Event.Dispatch(null);
            else if(m_PathFollowerReachNodeEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_PathFollowerReachNodeEvent.m_Event.Dispatch(gameObject);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_PathFollowerReachNodeEvent, this, "Reached Node");
        }
    }

    /**
    * FUNCTION NAME: DispatchReachFinalNodeEvent
    * DESCRIPTION  : Send out reach final node event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchReachFinalNodeEvent()
    {
        if(m_PathFollowerReachFinalNodeEvent != null && m_PathFollowerReachFinalNodeEvent.m_Event != null)
        {
            if(m_PathFollowerReachFinalNodeEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_PathFollowerReachFinalNodeEvent.m_Event.Dispatch(null);
            else if(m_PathFollowerReachFinalNodeEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_PathFollowerReachFinalNodeEvent.m_Event.Dispatch(gameObject);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_PathFollowerReachFinalNodeEvent, this, "Reached Final Node");
        }
    }

    /**
    * FUNCTION NAME: DispatchNoticeEvent
    * DESCRIPTION  : Send out a notice event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchNoticeEvent()
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Enemy found");

        if(m_PathFollowerNoticeEnemyEvent != null && m_PathFollowerNoticeEnemyEvent.m_Event)
        {
            if (m_PathFollowerNoticeEnemyEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_PathFollowerNoticeEnemyEvent.m_Event.Dispatch(null);
            else if (m_PathFollowerNoticeEnemyEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_PathFollowerNoticeEnemyEvent.m_Event.Dispatch(gameObject);
            else if (m_PathFollowerNoticeEnemyEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_PathFollowerNoticeEnemyEvent.m_Event.Dispatch(gameObject, m_PathFollowerNoticeEnemyEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_PathFollowerNoticeEnemyEvent, this, "Notice Enemy");
        }
    }

    /**
    * FUNCTION NAME: DispatchLostEvent
    * DESCRIPTION  : Send out a lost event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchLostEvent()
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Enemy lost");

        if(m_PathFollowerLostEnemyEvent != null && m_PathFollowerLostEnemyEvent.m_Event != null)
        {
            if(m_PathFollowerLostEnemyEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_PathFollowerLostEnemyEvent.m_Event.Dispatch(null);
            else if(m_PathFollowerLostEnemyEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_PathFollowerLostEnemyEvent.m_Event.Dispatch(gameObject);
            else if (m_PathFollowerLostEnemyEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_PathFollowerLostEnemyEvent.m_Event.Dispatch(gameObject, m_PathFollowerLostEnemyEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_PathFollowerLostEnemyEvent, this, "Lost Enemy");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_AggressivePathFollower))]
public class LPK_AggressivePathFollowerEditor : Editor
{
    SerializedProperty loopType;
    SerializedProperty aggressionProperties;
    SerializedProperty nodes;

    SerializedProperty noticeEnemyReceivers;
    SerializedProperty lostEnemyReceivers;
    SerializedProperty reachNodeReceivers;
    SerializedProperty reachFinalNodeReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        loopType = serializedObject.FindProperty("m_eLoopType");
        aggressionProperties = serializedObject.FindProperty("m_AggressionProperties");
        nodes = serializedObject.FindProperty("m_Nodes");

        noticeEnemyReceivers = serializedObject.FindProperty("m_PathFollowerNoticeEnemyEvent");
        lostEnemyReceivers = serializedObject.FindProperty("m_PathFollowerLostEnemyEvent");
        reachNodeReceivers = serializedObject.FindProperty("m_PathFollowerReachNodeEvent");
        reachFinalNodeReceivers = serializedObject.FindProperty("m_PathFollowerReachFinalNodeEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_AggressivePathFollower owner = (LPK_AggressivePathFollower)target;

        LPK_AggressivePathFollower editorOwner = owner.GetComponent<LPK_AggressivePathFollower>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_AggressivePathFollower)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_AggressivePathFollower), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_AggressivePathFollower");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(loopType, true);
        owner.m_flSpeed = EditorGUILayout.FloatField(new GUIContent("Speed", "Speed (units per second) to move along the path."), owner.m_flSpeed);
        EditorGUILayout.PropertyField(aggressionProperties, true);
        LPK_EditorArrayDraw.DrawArray(nodes, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        //Events.
        EditorGUILayout.PropertyField(noticeEnemyReceivers, true);
        EditorGUILayout.PropertyField(lostEnemyReceivers, true);
        EditorGUILayout.PropertyField(reachNodeReceivers, true);
        EditorGUILayout.PropertyField(reachFinalNodeReceivers, true);

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

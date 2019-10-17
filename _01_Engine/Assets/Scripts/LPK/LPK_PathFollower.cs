/***************************************************
File:           LPK_PathFollower.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component causes its gameobject to follow a pre-determined
  path by the designer.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Runtime.CompilerServices;  /*  Method Attributes */
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_PathFollower
* DESCRIPTION : Sets an object to follow nodes along a path.
**/
[RequireComponent(typeof(Transform))]
public class LPK_PathFollower : LPK_Component
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

    public float m_flSpeed = 5.0f;

    [Tooltip("Nodes that make up the path.")]
    public Transform[] m_Nodes;

    [Header("Event Sending Info")]

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
      if (m_iCounter < m_Nodes.Length)
      {
          if (m_Nodes.Length != 0 && m_Nodes[m_iCounter] != null)
              MoveAlongPath();
      }

      DetectReachNode();
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

        m_cTransform.position = Vector3.MoveTowards(m_cTransform.position, m_Nodes[m_iCounter].position, Time.deltaTime * m_flSpeed);

        if (m_cTransform.position == m_Nodes[m_iCounter].position)
        {
            m_bReachedNode = true;

            //Dispatch event.
            DispatchReachNodeEvent();

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Reached Node");
        }
    }

    /**
    * FUNCTION NAME: DetectReachNode
    * DESCRIPTION  : Manage behavior once reaching a node.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DetectReachNode()
    {
        if (!m_bReachedNode)
            return;

        if (!m_bGoingBackwards)
            m_iCounter++;
        else
            m_iCounter--;

        //Final node reached event sending.
        if (m_iCounter > m_Nodes.Length)
        {
            DispatchReachFinalNodeEvent();

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Reached end of path.");
        }

        //Move towards the start of the path manually.
        if (m_iCounter >= m_Nodes.Length && m_eLoopType == LPK_PathFollowerLoopType.LOOP)
            m_iCounter = 0;

        //Teleport to the beggining of the path and resume.
        else if (m_iCounter >= m_Nodes.Length && m_eLoopType == LPK_PathFollowerLoopType.LOOP_TELEPORT)
        {
            m_iCounter = 1;
            m_cTransform.position = m_Nodes[0].position;
        }

        //Begin going backwards down the path.
        else if ( (m_iCounter >= m_Nodes.Length && m_eLoopType == LPK_PathFollowerLoopType.LOOP_BACKTRACK)
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
            else if (m_PathFollowerReachNodeEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_PathFollowerReachNodeEvent.m_Event.Dispatch(gameObject, m_PathFollowerReachNodeEvent.m_Tags);

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
            else if (m_PathFollowerReachFinalNodeEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_PathFollowerReachFinalNodeEvent.m_Event.Dispatch(gameObject, m_PathFollowerReachFinalNodeEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_PathFollowerReachNodeEvent, this, "Reached Final Node");
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_PathFollower))]
public class LPK_PathFollowerEditor : Editor
{
    SerializedProperty toggleType;
    SerializedProperty loopType;
    SerializedProperty nodes;

    SerializedProperty eventTriggers;

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
        toggleType = serializedObject.FindProperty("m_eToggleType");
        loopType = serializedObject.FindProperty("m_eLoopType");
        nodes = serializedObject.FindProperty("m_Nodes");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

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
        LPK_PathFollower owner = (LPK_PathFollower)target;

        LPK_PathFollower editorOwner = owner.GetComponent<LPK_PathFollower>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_PathFollower)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_PathFollower), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_PathFollower");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(loopType, true);
        owner.m_flSpeed = EditorGUILayout.FloatField(new GUIContent("Speed", "Speed (units per second) to move along the path."), owner.m_flSpeed);
        LPK_EditorArrayDraw.DrawArray(nodes, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        //Events
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

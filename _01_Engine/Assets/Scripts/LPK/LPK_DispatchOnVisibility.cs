/***************************************************
File:           LPK_DispatchOnVisibility.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.14

Description:
  This component detects when a game object changes
  its rendered state (on or off screen) and sends
  events accordingly.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_DispatchOnVisibility
* DESCRIPTION : Dispatches events when the game object changes its visibility status.
**/
[RequireComponent(typeof(Renderer))]
public class LPK_DispatchOnVisibility : LPK_Component
{
    /************************************************************************************/

    public enum LPK_VisibilityCheckType
    {
        VISIBLITY_ENTER_SCREEN,
        VISIBLITY_EXIT_SCREEN,
        VISIBLITY_PERSIST_ON_SCREEN,
        VISIBLITY_PERSIST_OFF_SCREEN,
    };

    /************************************************************************************/

    [Tooltip("How to check for the game object being visible on screen.")]
    public LPK_VisibilityCheckType m_VisibilityCheckType;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the game object enters the screen.")]
    public LPK_EventSendingInfo m_EnterScreenEvent;

    [Tooltip("Event sent when the game object exits the screen.")]
    public LPK_EventSendingInfo m_ExitScreenEvent;

    [Tooltip("Event sent when the game object stays on the screen.")]
    public LPK_EventSendingInfo m_PersistOnScreenEvent;

    [Tooltip("Event sent when the game object stays off the screen.")]
    public LPK_EventSendingInfo m_PersistOffScreenEvent;

    /************************************************************************************/

    //Used to detect if a game object is on screen.
    bool m_bIsVisible = false;

    /************************************************************************************/

    Renderer m_cRenderer;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Cache the renderer component.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cRenderer = GetComponent<Renderer>();
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Checks visibility for event sending every frame.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        CheckVisibility();
    }

    /**
    * FUNCTION NAME: CheckVisibility
    * DESCRIPTION  : Object visibility detection.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CheckVisibility()
    {
        //Game object now on screen.
        if (m_cRenderer.isVisible)
        {
            //Enter screen.
            if (!m_bIsVisible && m_VisibilityCheckType == LPK_VisibilityCheckType.VISIBLITY_ENTER_SCREEN)
            {
                if(m_EnterScreenEvent != null && m_EnterScreenEvent.m_Event != null)
                {
                    if(m_EnterScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                        m_EnterScreenEvent.m_Event.Dispatch(null);
                    else if(m_EnterScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                        m_EnterScreenEvent.m_Event.Dispatch(gameObject);
                    else if (m_EnterScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                        m_EnterScreenEvent.m_Event.Dispatch(gameObject, m_EnterScreenEvent.m_Tags);

                    if (m_bPrintDebug)
                        LPK_PrintDebugDispatchingEvent(m_EnterScreenEvent, this, "Game Object Enter Screen");
                }
            }

            //Enter screen persist.
            if (m_bIsVisible && m_VisibilityCheckType == LPK_VisibilityCheckType.VISIBLITY_PERSIST_ON_SCREEN)
            {
                if(m_PersistOnScreenEvent != null && m_PersistOnScreenEvent.m_Event != null)
                {
                    if(m_PersistOnScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                        m_PersistOnScreenEvent.m_Event.Dispatch(null);
                    else if(m_PersistOnScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                        m_PersistOnScreenEvent.m_Event.Dispatch(gameObject);
                    else if (m_PersistOnScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                        m_PersistOnScreenEvent.m_Event.Dispatch(gameObject, m_PersistOnScreenEvent.m_Tags);

                    if (m_bPrintDebug)
                        LPK_PrintDebugDispatchingEvent(m_PersistOnScreenEvent, this, "Game Object Persist On Screen");
                }
            }

            m_bIsVisible = true;
        }

        //Game object now off screen.
        else if (m_cRenderer && !m_cRenderer.isVisible)
        {
            //Exit screen.
            if (m_bIsVisible && m_VisibilityCheckType == LPK_VisibilityCheckType.VISIBLITY_EXIT_SCREEN)
            {
                if(m_ExitScreenEvent != null && m_ExitScreenEvent.m_Event != null)
                {
                    if(m_ExitScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                        m_ExitScreenEvent.m_Event.Dispatch(null);
                    else if(m_ExitScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                        m_ExitScreenEvent.m_Event.Dispatch(gameObject);
                    else if (m_ExitScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                        m_ExitScreenEvent.m_Event.Dispatch(gameObject, m_ExitScreenEvent.m_Tags);

                    if (m_bPrintDebug)
                        LPK_PrintDebugDispatchingEvent(m_ExitScreenEvent, this, "Game Object Exit Screen");
                }
            }

            //Exit screen persist.
            if (!m_bIsVisible && m_VisibilityCheckType == LPK_VisibilityCheckType.VISIBLITY_PERSIST_OFF_SCREEN)
            {
                if(m_PersistOffScreenEvent != null && m_PersistOffScreenEvent.m_Event != null)
                {
                    if(m_PersistOffScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                        m_PersistOffScreenEvent.m_Event.Dispatch(null);
                    else if(m_PersistOffScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                        m_PersistOffScreenEvent.m_Event.Dispatch(gameObject);
                    else if (m_PersistOffScreenEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                        m_PersistOffScreenEvent.m_Event.Dispatch(gameObject, m_PersistOffScreenEvent.m_Tags);

                    if (m_bPrintDebug)
                        LPK_PrintDebugDispatchingEvent(m_PersistOffScreenEvent, this, "Game Object Persist Off Screen");
                }
            }

            m_bIsVisible = false;
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnVisibility))]
public class LPK_DispatchOnVisibilityEditor : Editor
{
    SerializedProperty m_VisibilityCheckType;

    SerializedProperty m_EnterScreenEvent;
    SerializedProperty m_ExitScreenEvent;
    SerializedProperty m_PersistOnScreenEvent;
    SerializedProperty m_PersistOffScreenEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_VisibilityCheckType = serializedObject.FindProperty("m_VisibilityCheckType");

        m_EnterScreenEvent = serializedObject.FindProperty("m_EnterScreenEvent");
        m_ExitScreenEvent = serializedObject.FindProperty("m_ExitScreenEvent");
        m_PersistOnScreenEvent = serializedObject.FindProperty("m_PersistOnScreenEvent");
        m_PersistOffScreenEvent = serializedObject.FindProperty("m_PersistOffScreenEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnVisibility owner = (LPK_DispatchOnVisibility)target;

        LPK_DispatchOnVisibility editorOwner = owner.GetComponent<LPK_DispatchOnVisibility>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnVisibility)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnVisibility), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnVisibility");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_VisibilityCheckType, true);

        //Events
        EditorGUILayout.PropertyField(m_EnterScreenEvent, true);
        EditorGUILayout.PropertyField(m_ExitScreenEvent, true);
        EditorGUILayout.PropertyField(m_PersistOnScreenEvent, true);
        EditorGUILayout.PropertyField(m_PersistOffScreenEvent, true);

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

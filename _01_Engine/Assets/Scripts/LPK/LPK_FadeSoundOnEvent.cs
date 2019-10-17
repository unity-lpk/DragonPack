/***************************************************
File:           LPK_FadeSoundOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component can be used to fade volume levels on
  AudioSource components.

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
* CLASS NAME  : LPK_FadeSoundOnEvent
* DESCRIPTION : Component used to fade volume levels on audio.
**/
public class LPK_FadeSoundOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Audio Sources to fade when a valid event is received.  If both this and the tags array are set to length 0, default to self.")]
    public AudioSource[] m_FadeSources;

    public float m_flFadeDuration = 2;
    public float m_flGoalVolume;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when an audio source finishes fading to a new volume level.")]
    public LPK_EventSendingInfo m_AudioFadeCompletedEvent;

    /************************************************************************************/

    //Initial volume settings when fade was called.
    float[] m_aInitialVolumes;

    float m_flTimer = 0;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Begins event receiving and gathering of fade sources as needed.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        SetInitialLevels();

        if(m_EventTrigger != null)
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
        if(!ShouldRespondToEvent(_activator))
            return;

        SetInitialLevels();
    }

    /**
    * FUNCTION NAME: DispatchFadeCompletedEvent
    * DESCRIPTION  : Dispatch event when audio levels have finished fading.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchFadeCompletedEvent()
    {
        if(m_AudioFadeCompletedEvent != null && m_AudioFadeCompletedEvent.m_Event != null)
        {
            if(m_AudioFadeCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_AudioFadeCompletedEvent.m_Event.Dispatch(null);
            else if(m_AudioFadeCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_AudioFadeCompletedEvent.m_Event.Dispatch(gameObject);
            else if (m_AudioFadeCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_AudioFadeCompletedEvent.m_Event.Dispatch(gameObject, m_AudioFadeCompletedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_AudioFadeCompletedEvent, this, "Audio Fade Completed");
        }
    }

    /**
    * FUNCTION NAME: SetInitialLevels
    * DESCRIPTION  : Set initial audio levels for the fade.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetInitialLevels()
    {
        m_aInitialVolumes = new float[m_FadeSources.Length];

        //Gather all of the initial volumes and store them.
        for (int i = 0; i < m_FadeSources.Length; i++)
             m_aInitialVolumes[i] = m_FadeSources[i].volume;
    }

    /**
    * FUNCTION NAME: FadeAudioLevels
    * DESCRIPTION  : Manages fade of audio overtime.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FadeAudioLevels()
    {
        if (m_flTimer < m_flFadeDuration)
        {
            m_flTimer += Time.deltaTime;

            for (int i = 0; i < m_FadeSources.Length; i++)
            {
                if (m_FadeSources[i] == null)
                    continue;

                if (m_FadeSources[i].volume > m_flGoalVolume)
                    m_FadeSources[i].volume = m_aInitialVolumes[i] - (m_flTimer / m_flFadeDuration);
                else
                    m_FadeSources[i].volume = m_aInitialVolumes[i] + (m_flTimer / m_flFadeDuration);
            }

            if (m_flTimer <= m_flFadeDuration)
                return;
        }

        //Ensure all audio levels are properly set.
        for (int i = 0; i < m_FadeSources.Length; i++)
        {
            if (m_FadeSources[i] == null)
                continue;

            m_FadeSources[i].volume = m_flGoalVolume;
        }

        m_flTimer = 0;
        DispatchFadeCompletedEvent();
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Removes game object from the event queue.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        if(m_EventTrigger != null)
            m_EventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_FadeSoundOnEvent))]
public class LPK_FadeSoundOnEventEditor : Editor
{
    SerializedProperty fadeSources;

    SerializedProperty eventTriggers;

    SerializedProperty m_FadeSoundEventSendingMode;
    SerializedProperty audioFadeCompletedReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        fadeSources = serializedObject.FindProperty("m_FadeSources");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        audioFadeCompletedReceivers = serializedObject.FindProperty("m_AudioFadeCompletedEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_FadeSoundOnEvent owner = (LPK_FadeSoundOnEvent)target;

        LPK_FadeSoundOnEvent editorOwner = owner.GetComponent<LPK_FadeSoundOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_FadeSoundOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_FadeSoundOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_FadeSoundOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(fadeSources, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
        owner.m_flFadeDuration = EditorGUILayout.FloatField(new GUIContent("Fade Duration", "How long for the fade to last."), owner.m_flFadeDuration);
        owner.m_flGoalVolume = EditorGUILayout.FloatField(new GUIContent("Target Volume", "Target volume to fade all sources to."), owner.m_flGoalVolume);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);
        EditorGUILayout.PropertyField(audioFadeCompletedReceivers, true);

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

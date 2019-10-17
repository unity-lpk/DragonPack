/***************************************************
File:           LPK_PlaySoundOnEvent
Authors:        Christopher Onorati
Last Updated:   8/27/2019
Last Version:   2019.1.14

Description:
  This component can be added to any object to play a 
  sound upon receiving an event notice.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_PlaySoundOnEvent
* DESCRIPTION : Plays a sound effect when an event is sent and parsed.
**/
public class LPK_PlaySoundOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Audio Source(s) who will be used to play sound.")]
    public AudioSource[] m_TargetAudioSources;

    public float m_flVolumeRangeMin = 1.0f;
    public float m_flVolumeRangeMax = 1.0f;

    public float m_flPitchRangeMin = 1.0f;
    public float m_flPitchRangeMax = 1.0f;

    public float m_flCooldown = 0.0f;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Whether this component is waiting its cooldown
    bool m_bOnCooldown = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for sound playback.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger)
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

        if(m_bOnCooldown)
        {
            if(m_bPrintDebug)
                LPK_PrintDebug(this, "On cool down.");
            return;
        }

        if (m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        for(int i = 0; i < m_TargetAudioSources.Length; i++)
        {
            if (m_TargetAudioSources[i] != null)
            {
                m_TargetAudioSources[i].GetComponent<AudioSource>().volume = m_TargetAudioSources[i].GetComponent<AudioSource>().volume + Random.Range(m_flVolumeRangeMin, m_flVolumeRangeMax);
                m_TargetAudioSources[i].GetComponent<AudioSource>().pitch = Random.Range(m_flPitchRangeMin, m_flPitchRangeMax);
                m_TargetAudioSources[i].GetComponent<AudioSource>().Play();
            }
        }

        m_bOnCooldown = true;
        StartCoroutine(CoolDownDelay());
    }

    /**
    * FUNCTION NAME: CoolDownDelay
    * DESCRIPTION  : Forces cooldown between calls to play the sound effect.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator CoolDownDelay()
    {
        yield return new WaitForSeconds(m_flCooldown);
        m_bOnCooldown = false;
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

[CustomEditor(typeof(LPK_PlaySoundOnEvent))]
public class LPK_PlaySoundOnEventEditor : Editor
{
    SerializedProperty targetAudioSources;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        targetAudioSources = serializedObject.FindProperty("m_TargetAudioSources");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_PlaySoundOnEvent owner = (LPK_PlaySoundOnEvent)target;

        LPK_PlaySoundOnEvent editorOwner = owner.GetComponent<LPK_PlaySoundOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_PlaySoundOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_PlaySoundOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_PlaySoundOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(targetAudioSources, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
        owner.m_flVolumeRangeMin = EditorGUILayout.FloatField(new GUIContent("Volume Variance Min", "Minimum adjusment to the volume for the sound to be played at.  The volume will be the Audio Source's volume plus a randomized each time the sound is called to be played."), owner.m_flVolumeRangeMin);
        owner.m_flVolumeRangeMax = EditorGUILayout.FloatField(new GUIContent("Volume Variance Max", "Maximum adjustment to the  volume for the sound to be played at.  The volume will be the Audio Source's volume plus a randomized each time the sound is called to be played."), owner.m_flVolumeRangeMax);
        
        owner.m_flPitchRangeMin = EditorGUILayout.FloatField(new GUIContent("Pitch Variance Min", "Minimum pitch for the sound to be played at.  The pitch will be randomized each time the sound is called to be played."), owner.m_flPitchRangeMin);
        owner.m_flPitchRangeMax = EditorGUILayout.FloatField(new GUIContent("Pitch Variance Max", "Maximum pitch for the sound to be played at.  The pitch will be randomized each time the sound is called to be played."), owner.m_flPitchRangeMax);

        owner.m_flCooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "Number of seconds to wait until an event can trigger another instance of sound played."), owner.m_flCooldown);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);

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

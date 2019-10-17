/***************************************************
File:           LPK_ModifyParticleSystemOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/2/2019
Last Version:   2018.3.14

Description:
  This component can be used to control the active state
  of a particle system, when an event is received.

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
* CLASS NAME  : LPK_ModifyParticleSystemOnEvent
* DESCRIPTION : Class to modify properties of a particle system during gameplay.
**/
public class LPK_ModifyParticleSystemOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_ToggleType
    {
        ON,
        OFF,
        TOGGLE,
    };

    /************************************************************************************/

    [Tooltip("Paritcle System to modify when the specified event is received.")]
    [Rename("Target Particle Systems")]
    public ParticleSystem[] m_pTargetModifyParticleSystems;

    [Tooltip("Set how to change the active state of the particle system.")]
    [Rename("Toggle Type")]
    public LPK_ToggleType m_eToggleType;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    bool m_bHasSetup = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to particle active state changing.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger != null)
            m_EventTrigger.Register(this);
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
        {
            if(gameObject.GetComponent<ParticleSystem>())
                m_pTargetModifyParticleSystems = new ParticleSystem[] { gameObject.GetComponent<ParticleSystem>() };

            m_bHasSetup = true;
        }
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

        for (int i = 0; i < m_pTargetModifyParticleSystems.Length; i++)
        {
            //Modify active state of particles.
            if (m_eToggleType == LPK_ToggleType.ON)
                m_pTargetModifyParticleSystems[i].Play();
            else if (m_eToggleType == LPK_ToggleType.OFF)
                m_pTargetModifyParticleSystems[i].Stop();
            else if (m_eToggleType == LPK_ToggleType.TOGGLE)
            {
                if (m_pTargetModifyParticleSystems[i].isPlaying)
                    m_pTargetModifyParticleSystems[i].Stop();
                else
                    m_pTargetModifyParticleSystems[i].Play();
            }

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Particle sytem on game object " + m_pTargetModifyParticleSystems[i].gameObject.name + " active state = " + m_pTargetModifyParticleSystems[i].isPlaying.ToString());
        }
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

[CustomEditor(typeof(LPK_ModifyParticleSystemOnEvent))]
public class LPK_ModifyParticleSystemOnEventEditor : Editor
{
    SerializedProperty m_pTargetModifyParticleSystems;
    SerializedProperty toggleType;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_pTargetModifyParticleSystems = serializedObject.FindProperty("m_pTargetModifyParticleSystems");
        toggleType = serializedObject.FindProperty("m_eToggleType");

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
        LPK_ModifyParticleSystemOnEvent owner = (LPK_ModifyParticleSystemOnEvent)target;

        LPK_ModifyParticleSystemOnEvent editorOwner = owner.GetComponent<LPK_ModifyParticleSystemOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyParticleSystemOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyParticleSystemOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyParticleSystemOnEvent");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_pTargetModifyParticleSystems, true);
        EditorGUILayout.PropertyField(toggleType, true);

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

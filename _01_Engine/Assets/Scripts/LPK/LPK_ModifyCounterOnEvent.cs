/***************************************************
File:           LPK_ModifyCounterOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to cause it to 
  dispatch an event to modify a target counter upon receiving an event.

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
* CLASS NAME  : LPK_ModifyCounterOnEvent
* DESCRIPTION : Component used to modify the values of counters.
**/
public class LPK_ModifyCounterOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Sets whether the modify will add a value, or set a specified value.")]
    [Rename("Mode")]
    public LPK_Counter.LPK_CounterModifyMode m_eMode = LPK_Counter.LPK_CounterModifyMode.ADD;

    public int m_iValue = 0;

    public float m_flCooldown = 0.0f;

    [Tooltip("Counter to modify.")]
    public LPK_Counter m_Counter;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Whether this component is waiting its cooldown
    bool m_bOnCooldown = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for counter modifying.
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

        //Spawn an object if not recharging and the max count hasnt been reached
        if (!m_bOnCooldown)
            ChangeCounter();
        else
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Counter modifier on cooldown.");
        }
    }

    /**
    * FUNCTION NAME: ChangeCounter
    * DESCRIPTION  : Changes counter values on event receiving.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void ChangeCounter()
    {
        //Skip null counters.
        if(m_Counter != null)
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Modified counter on game object " + m_Counter.name);

            m_Counter.UpdateCounter(m_eMode, m_iValue);
        }

        else if(m_bPrintDebug)
        {
            LPK_PrintWarning(this, "Counter target not set on ModifyCounter component.");
        }
        
        //Set recharging
        m_bOnCooldown = true;

        StartCoroutine(DelayTimer());
    }

    /**
    * FUNCTION NAME: DelayTimer
    * DESCRIPTION  : Forces delay between counter modification event sends.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator DelayTimer()
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

[CustomEditor(typeof(LPK_ModifyCounterOnEvent))]
public class LPK_ModifyCounterOnEventEditor : Editor
{
    SerializedProperty mode;
    SerializedProperty m_Counter;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        mode = serializedObject.FindProperty("m_eMode");
        m_Counter = serializedObject.FindProperty("m_Counter");

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
        LPK_ModifyCounterOnEvent owner = (LPK_ModifyCounterOnEvent)target;

        LPK_ModifyCounterOnEvent editorOwner = owner.GetComponent<LPK_ModifyCounterOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyCounterOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyCounterOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyCounterOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(mode, true);
        owner.m_iValue = EditorGUILayout.IntField(new GUIContent("Value", "Value to add or set"), owner.m_iValue);
        owner.m_flCooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "Number of seconds to wait until an event can trigger another instance of counter change."), owner.m_flCooldown);
        EditorGUILayout.PropertyField(m_Counter, true);


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

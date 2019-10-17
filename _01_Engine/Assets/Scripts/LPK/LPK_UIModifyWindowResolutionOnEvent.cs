/***************************************************
File:           LPK_UIModifyWindowResolutionOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be used to change the resolution
  of the game window during runtime.  This component is
  ideally used on an options menu.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.UI;   /*Text*/
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_UIModifyWindowResolutionOnEvent
* DESCRIPTION : Component used to change resolution of game.
**/
public class LPK_UIModifyWindowResolutionOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_ModifyWindowResolutionMode
    {
        INCREASE,
        DECREASE,
    };

    /************************************************************************************/

    [Header("Component Properties")]

    [Tooltip("How to modify the window resolution when recieving an LPK Event.")]
    [Rename("Window Resolution Modify Mode")]
    public LPK_ModifyWindowResolutionMode m_eModifyWindowResolutionMode;

    public bool m_bWrap = true;

    [Tooltip("Object to change text of to reflect current resolution.")]
    [Rename("Text Object")]
    public GameObject m_pText;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Position in the array of vectors to default on
    int m_iCounter;

    //List of all resolution values valid for monitor.
    Resolution[] m_aResolutions;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Make sure counter is set to a legal value.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_aResolutions = Screen.resolutions;

        if(m_EventTrigger)
            m_EventTrigger.Register(this);

        SetText();
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

        if(m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        if(m_eModifyWindowResolutionMode == LPK_ModifyWindowResolutionMode.INCREASE)
            OnPositiveEvent();
        else if (m_eModifyWindowResolutionMode == LPK_ModifyWindowResolutionMode.DECREASE)
            OnNegativeEvent();
    }

    /**
    * FUNCTION NAME: OnPositiveEvent
    * DESCRIPTION  : Changes window resolution up in the array.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void OnPositiveEvent()
    {
        if (m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        //Find current set resolution in array of valid values.
        for (int i = 0; i < m_aResolutions.Length; i++)
        {
            if(Screen.currentResolution.height == m_aResolutions[i].height && Screen.currentResolution.width == m_aResolutions[i].width)
            {
                m_iCounter = i;
                break;
            }
        }

        m_iCounter++;
        CheckBounds();

        Screen.SetResolution(m_aResolutions[m_iCounter].width, m_aResolutions[m_iCounter].height, Screen.fullScreen);

        SetText();
    }

    /**
    * FUNCTION NAME: OnNegativeEvent
    * DESCRIPTION  : Changes window resolution down in the array.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void OnNegativeEvent()
    {
        if (m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        //Find current set resolution in array of valid values.
        for (int i = 0; i < m_aResolutions.Length; i++)
        {
            if(Screen.currentResolution.height == m_aResolutions[i].height && Screen.currentResolution.width == m_aResolutions[i].width)
            {
                m_iCounter = i;
                break;
            }
        }

        m_iCounter--;
        CheckBounds();

        Screen.SetResolution(m_aResolutions[m_iCounter].width, m_aResolutions[m_iCounter].height, Screen.fullScreen);

        SetText();
    }

    /**
    * FUNCTION NAME: CheckBounds
    * DESCRIPTION  : Keeps the counter in the legal range.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CheckBounds()
    {
        //Valid value.
        if (m_iCounter >= 0 && m_iCounter < m_aResolutions.Length)
            return;

        //Counter has gone below legal range.
        if (m_iCounter < 0 && m_bWrap)
            m_iCounter = m_aResolutions.Length - 1;
        else if (m_iCounter < 0 && !m_bWrap)
            m_iCounter = 0;

        //Counter has gone above legal range.
        if (m_iCounter >= m_aResolutions.Length && m_bWrap)
            m_iCounter = 0;
        else if (m_iCounter >= m_aResolutions.Length && !m_bWrap)
            m_iCounter = m_aResolutions.Length;
    }

    /**
    * FUNCTION NAME: SetText
    * DESCRIPTION  : Sets the text display.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetText()
    {
        //Error - invalid object or missing component.
        if (m_pText == null || m_pText.GetComponent<Text>() == null)
        {
            if (m_bPrintDebug)
                LPK_PrintError(this, "Invalid text display set for resolution switcher!");

            return;
        }

        //Update the text display.
        m_pText.GetComponent<Text>().text = m_aResolutions[m_iCounter].width.ToString() + " x " + m_aResolutions[m_iCounter].height.ToString();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_UIModifyWindowResolutionOnEvent))]
public class LPK_UIModifyWindowResolutionOnEventEditor : Editor
{
    SerializedProperty m_eModifyWindowResolutionMode;
    SerializedProperty text;

    SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_eModifyWindowResolutionMode = serializedObject.FindProperty("m_eModifyWindowResolutionMode");
        text = serializedObject.FindProperty("m_pText");

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
        LPK_UIModifyWindowResolutionOnEvent owner = (LPK_UIModifyWindowResolutionOnEvent)target;

        LPK_UIModifyWindowResolutionOnEvent editorOwner = owner.GetComponent<LPK_UIModifyWindowResolutionOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_UIModifyWindowResolutionOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_UIModifyWindowResolutionOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_UIModifyWindowResolutionOnEvent");

        //Component Properties.
        EditorGUILayout.PropertyField(m_eModifyWindowResolutionMode, true);

        owner.m_bWrap = EditorGUILayout.Toggle(new GUIContent("Wrap Counter", "Sets the resolution counter to wrap.  Example: If at the end of the array, and a positive event is received, go to the beggining of the array."), owner.m_bWrap);
        EditorGUILayout.PropertyField(text, true);

        //Events.
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

/***************************************************
File:           LPK_TextDisplay.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description: 
  This component controls the appearance of a text display
  such as a health or cooldown bar. It can be hooked up to 
  LPK_Health or other components.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.UI;   /* Text */
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_TextDisplay
* DESCRIPTION : This component can be added to any object with a TextMesh to act as a display of sorts.
**/
public class LPK_TextDisplay : LPK_DisplayObject
{
    /************************************************************************************/

    public enum DisplayType
    {
        TIMER,
        COUNTER,
        COUNTER_OVER_TOTAL,
    }

    /************************************************************************************/

    public string m_sStartText;

    [Tooltip("What display mode to use for the text.")]
    [Rename("Display Mode")]
    public DisplayType m_eDisplayMode = DisplayType.TIMER;

    public bool m_bAnimateCounter;
    public bool m_bAnimateFirstDisplay;
    public float m_flAnimateTime = 1.0f;

    [Tooltip("Max number of digits to display past the decimal.")]
    [Rename("Max Decimals")]
    public uint m_iMaxDecimals = 3;

    /************************************************************************************/

    //Flag used to hide changes from inspector to avoid designer confusion.
    bool m_bInternalAnimateFirstDisplay;

    //Check to see if animation should be occurng.
    bool m_bIsAnimating;

    //How long has the counter been incrementing.
    float m_flIncrementTime;

    //Starting value of the animation.
    float m_flStartingValue;

    //Current value of the counter.
    float m_flCurrentValue;

    //Goal to increment towards.
    float m_flGoalValue;

    //Counter max value.
    float m_flMaxValue;

    /************************************************************************************/

    TextMesh m_cTextMesh;
    Text m_cText;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initializes components and events.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_bInternalAnimateFirstDisplay = m_bAnimateFirstDisplay;

        m_cTextMesh = GetComponent<TextMesh>();
        m_cText = GetComponent<Text>();
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manage animated counters, if applicable.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if(m_bIsAnimating && (m_eDisplayMode == DisplayType.COUNTER || m_eDisplayMode == DisplayType.COUNTER_OVER_TOTAL))
        {
            m_flCurrentValue = m_flStartingValue + ((m_flGoalValue - m_flStartingValue) / m_flAnimateTime * m_flIncrementTime);

            m_flIncrementTime += Time.deltaTime;

            //Done with animation.
            if(m_flCurrentValue >= m_flGoalValue)
            {
                m_bIsAnimating = false;
                m_flCurrentValue = m_flGoalValue;
            }

            if(m_cTextMesh != null)
                UpdateGameText(m_flCurrentValue, m_flMaxValue);
            else if (m_cText != null)
                UpdateUIText(m_flCurrentValue, m_flMaxValue);
        }
    }

    /**
    * FUNCTION NAME: UpdateDisplay
    * DESCRIPTION  : Updates the display text of the mesh based on passed data.
    * INPUTS       : _currentVal - Current value of the display.
    *                _maxVal     - Max value of the display.
    * OUTPUTS      : None
    **/
    override public void UpdateDisplay(float _currentVal, float _maxVal)
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Display update.");
        
        //Save the current value if we do not animate initially.
        if(!m_bInternalAnimateFirstDisplay)
            m_flCurrentValue = _currentVal;

        //Handle animation reset.
        if(m_bAnimateCounter && m_bInternalAnimateFirstDisplay && m_eDisplayMode != DisplayType.TIMER)
        {
            m_flStartingValue = m_flCurrentValue;
            m_flGoalValue = _currentVal;
            m_flMaxValue = _maxVal;
            m_flIncrementTime = 0.0f;
            m_bIsAnimating = true;
        }

        else if (m_cTextMesh != null)
            UpdateGameText(_currentVal, _maxVal);

        else if (m_cText != null)
            UpdateUIText(_currentVal, _maxVal);

        m_bInternalAnimateFirstDisplay = true;
    }

    /**
    * FUNCTION NAME: UpdateGameText
    * DESCRIPTION  : Updates the display text of the mesh based on passed data.
    * INPUTS       : _initialValue - Initial value for the display to use.
    *                _goalValue     - Goal value for the display to use.
    * OUTPUTS      : None
    **/
    void UpdateGameText(float _initialValue, float _goalValue)
    {
        if (m_eDisplayMode == DisplayType.COUNTER)
        {
            string displayText = m_sStartText + string.Format("{0:F" + m_iMaxDecimals + "}", Mathf.Min(_initialValue, _goalValue));
            m_cTextMesh.text = displayText;
        }
        else if (m_eDisplayMode == DisplayType.COUNTER_OVER_TOTAL)
        {
            if (m_iMaxDecimals > 0)
            {
                string displayText = m_sStartText + string.Format("{0:F" + m_iMaxDecimals + "}", Mathf.Min(_initialValue, _goalValue) + " / " + _goalValue);
                m_cTextMesh.text = displayText;
            }
            else
            {
                string displayText = m_sStartText + (int)Mathf.Min(_initialValue, _goalValue) + " / " + _goalValue;
                m_cTextMesh.text = displayText;
            }
        }
        else if (m_eDisplayMode == DisplayType.TIMER)
        {
            if (m_iMaxDecimals > 0)
            {
                string displayText = m_sStartText + string.Format("{0:F" + m_iMaxDecimals + "}", Mathf.Min(_initialValue, _goalValue));
                m_cTextMesh.text = displayText;
            }
            else
            {
                string displayText = m_sStartText + ((int)Mathf.Min(_initialValue, _goalValue)).ToString();
                m_cTextMesh.text = displayText;
            }
        }
    }

    /**
    * FUNCTION NAME: UpdateUIText
    * DESCRIPTION  : Updates the display text of the mesh based on passed data.
    * INPUTS       : _initialValue - Initial value for the display to use.
    *                _goalValue     - Goal value for the display to use.
    * OUTPUTS      : None
    **/
    void UpdateUIText(float _initialValue = 0, float _goalValue = 0)
    {
        if (m_eDisplayMode == DisplayType.COUNTER)
        {
            string displayText = m_sStartText + string.Format("{0:F" + m_iMaxDecimals + "}", Mathf.Min(_initialValue, _goalValue));
            m_cText.text = displayText;
        }
        else if (m_eDisplayMode == DisplayType.COUNTER_OVER_TOTAL)
        {
            if (m_iMaxDecimals > 0)
            {
                string displayText = m_sStartText + string.Format("{0:F" + m_iMaxDecimals + "}", Mathf.Min(_initialValue, _goalValue) + " / " + _goalValue);
                m_cText.text = displayText;
            }
            else
            {
                string displayText = m_sStartText + (int)Mathf.Min(_initialValue, _goalValue) + " / " + _goalValue;
                m_cText.text = displayText;
            }
        }
        else if (m_eDisplayMode == DisplayType.TIMER)
        {
            if (m_iMaxDecimals > 0)
            {
                string displayText = m_sStartText + string.Format("{0:F" + m_iMaxDecimals + "}", Mathf.Min(_initialValue, _goalValue));
                m_cText.text = displayText;
            }
            else
            {
                string displayText = m_sStartText + ((int)Mathf.Min(_initialValue, _goalValue)).ToString();
                m_cText.text = displayText;
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_TextDisplay))]
public class LPK_TextDisplayEditor : Editor
{
    SerializedProperty displayMode;
    SerializedProperty maxDecimals;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        displayMode = serializedObject.FindProperty("m_eDisplayMode");
        maxDecimals = serializedObject.FindProperty("m_iMaxDecimals");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_TextDisplay owner = (LPK_TextDisplay)target;

        LPK_TextDisplay editorOwner = owner.GetComponent<LPK_TextDisplay>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_TextDisplay)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_TextDisplay), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_TextDisplay");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_sStartText = EditorGUILayout.TextField(new GUIContent("Prefix Text", "Text to put before the display text.  EX: Timer."), owner.m_sStartText);
        EditorGUILayout.PropertyField(displayMode, true);

        if (displayMode.intValue == (int)LPK_TextDisplay.DisplayType.TIMER)
            EditorGUILayout.PropertyField(maxDecimals, true);

        if(owner.m_eDisplayMode == LPK_TextDisplay.DisplayType.COUNTER || owner.m_eDisplayMode == LPK_TextDisplay.DisplayType.COUNTER_OVER_TOTAL)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Animation Settings", EditorStyles.miniBoldLabel);

            owner.m_bAnimateCounter = EditorGUILayout.Toggle(new GUIContent("Animate Counter", "Set to animate the counter as it reaches it's new value, rather than snapping to the new value."), owner.m_bAnimateCounter);
            
            if(owner.m_bAnimateCounter)
            {   
                owner.m_bAnimateFirstDisplay = EditorGUILayout.Toggle(new GUIContent("Animate First Display", "Should the first update to this counter be animated.  This can be used to NOT animate setting the intitial value, if desired."), owner.m_bAnimateFirstDisplay);
                owner.m_flAnimateTime = EditorGUILayout.FloatField(new GUIContent("Animation Duration", "How long to animate going from an old value to a new one."), owner.m_flAnimateTime);
            }
        }

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

/***************************************************
File:           LPK_BarDisplay.cs
Authors:        Christopher Onorati
Last Updated:   4/2/2019
Last Version:   2018.3.14

Description:
  This component controls the appearance of a display bar 
  such as a health or cooldown bar.  You can use this with
  sprites, images, or raw images.  If using a sprite, set
  the pivot point in the Sprite Editor to determine
  how the bar should scale.  This can be done in the editor
  for raw images and images.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_BarDisplay
* DESCRIPTION : Create a display using a slider in the UI canvas.
**/
public class LPK_BarDisplay : LPK_DisplayObject
{
    /************************************************************************************/

    public enum LPK_BarDisplayMode
    {
        HORIZONTAL,
        VERTICAL,
    };

    /************************************************************************************/

    [Header("Component Properties")]

    [Tooltip("Display mode for the bar.  Horizontal = scales the bar based on the X axis.  Vertical = scales the bar based on the Y axis.")]
    [Rename("Display Mode")]
    public LPK_BarDisplayMode m_eDisplayMode = LPK_BarDisplayMode.HORIZONTAL;

    public float m_flMaxScaleX = 1.0f;
    public float m_flMaxScaleY = 1.0f;


    /************************************************************************************/

    SpriteRenderer m_cSpriteRenderer;
    RawImage m_cRawImage;
    Image m_cImage;

    Transform m_cTransform;
    RectTransform m_cRectTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initializes events.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cSpriteRenderer = GetComponent<SpriteRenderer>();
        m_cRawImage = GetComponent<RawImage>();
        m_cImage = GetComponent<Image>();

        m_cTransform = GetComponent<Transform>();
        m_cRectTransform = GetComponent<RectTransform>();

        if(m_bPrintDebug && m_cSpriteRenderer == null && m_cRawImage == null && m_cImage == null)
            LPK_PrintError(this, "No modifiable component placed on bar display.  Please add a sprite renderer, an image, or a raw image component.");
    }

    /**
    * FUNCTION NAME: UpdateDisplay
    * DESCRIPTION  : Changes the visible display of the bar.
    * INPUTS       : _currentVal - Current value of the counter.
    *                _maxVal - Maximum value of the counter.
    * OUTPUTS      : None
    **/
    override public void UpdateDisplay(float _currentVal, float _maxVal)
    {        
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Display Update");

        if (m_eDisplayMode == LPK_BarDisplayMode.HORIZONTAL)
        {
            if (m_cSpriteRenderer != null)
                m_cTransform.localScale = new Vector3(_currentVal / _maxVal * m_flMaxScaleX, m_cTransform.localScale.y, m_cTransform.localScale.z);
            else if (m_cRawImage != null || m_cImage != null)
                m_cRectTransform.localScale = new Vector3(_currentVal / _maxVal * m_flMaxScaleX, m_cTransform.localScale.y, m_cTransform.localScale.z);
        }

        else if (m_eDisplayMode == LPK_BarDisplayMode.VERTICAL)
        {
            if (m_cSpriteRenderer != null)
                m_cTransform.localScale = new Vector3(m_cTransform.localScale.x, _currentVal / _maxVal * m_flMaxScaleY, m_cTransform.localScale.z);
            else if (m_cRawImage != null || m_cImage != null)
                m_cRectTransform.localScale = new Vector3(m_cTransform.localScale.x, _currentVal / _maxVal * m_flMaxScaleY, m_cTransform.localScale.z);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_BarDisplay))]
public class LPK_BarDisplayEditor : Editor
{
    SerializedProperty m_eDisplayMode;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_eDisplayMode = serializedObject.FindProperty("m_eDisplayMode");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_BarDisplay owner = (LPK_BarDisplay)target;

        LPK_BarDisplay editorOwner = owner.GetComponent<LPK_BarDisplay>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_BarDisplay)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_BarDisplay), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_BarDisplay");

        //Component properties.
        EditorGUILayout.PropertyField(m_eDisplayMode, true);      
        
        if(m_eDisplayMode.enumValueIndex == (int)LPK_BarDisplay.LPK_BarDisplayMode.HORIZONTAL)
            owner.m_flMaxScaleX = EditorGUILayout.FloatField(new GUIContent("Max X Scale", "Set the maximum scale the bar will be.  In other words, this is the scale in the X axis the bar will be at when representing 100%"), owner.m_flMaxScaleX);
        else if(m_eDisplayMode.enumValueIndex == (int)LPK_BarDisplay.LPK_BarDisplayMode.VERTICAL)
            owner.m_flMaxScaleY = EditorGUILayout.FloatField(new GUIContent("Max Y Scale", "Set the maximum scale the bar will be.  In other words, this is the scale in the Y axis the bar will be at when representing 100%"), owner.m_flMaxScaleY);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);
    }
}

#endif  //UNITY_EDITOR

}   //LPK

/***************************************************
File:           LPK_RadialDisplay.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component controls the appearance of a radial 
  for things such as a health or cooldown display.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;   /* Image */

namespace LPK
{

/**
* CLASS NAME  : LPK_RadialDisplay
* DESCRIPTION : Create a display using a radial display in the UI canvas.
**/
[RequireComponent(typeof(Image))]
public class LPK_RadialDisplay : LPK_DisplayObject
{
    /************************************************************************************/

    Image m_cImage;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initializes events and sets the image to the right type.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cImage = GetComponent<Image>();

        //Error checking.
        if(m_cImage.type != Image.Type.Filled)
        {
            if(m_bPrintDebug)
                LPK_PrintWarning(this, "Image on radial display is not set to use Filled as Image Type.  Switching automatically.");

            m_cImage.type = Image.Type.Filled;
        }
    }

    /**
    * FUNCTION NAME: UpdateDisplay
    * DESCRIPTION  : Updates the filled amount of the radial display.
    * INPUTS       : _currentVal - Current value of the display.
    *                _maxVal     - Max value of the display.
    * OUTPUTS      : None
    **/
    override public void UpdateDisplay(float _currentVal, float _maxVal)
    {        
        //Update fill amount.
        m_cImage.fillAmount = _currentVal / _maxVal;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_RadialDisplay))]
public class LPK_RadialDisplayEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_RadialDisplay owner = (LPK_RadialDisplay)target;

        LPK_RadialDisplay editorOwner = owner.GetComponent<LPK_RadialDisplay>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_RadialDisplay)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_RadialDisplay), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_RadialDisplay");

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);
    }
}

#endif  //UNITY_EDITOR

}   //LPK

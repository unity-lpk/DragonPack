/***************************************************
File:           LPK_EditorArrayDraw.cs
Authors:        Christopher Onorati
Last Updated:   5/9/2019
Last Version:   2018.3.14

Description:
  Script that holds functionality to draw arrays in
  custom ways.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

/**
* CLASS NAME  : LPK_EditorArrayDraw
* DESCRIPTION : Allows customization of array drawing via custom inspectors.
**/
public static class LPK_EditorArrayDraw
{
    /************************************************************************************/

    public enum LPK_EditorArrayDrawMode
    {
        DRAW_MODE_DEFAULT,
        DRAW_MODE_BUTTONS
    };

    /************************************************************************************/

    //Add button content.
    private static GUIContent m_AddButtonContent = new GUIContent("+", "add new element");

    //Delete button content.
    private static GUIContent m_DeleteButtonContent = new GUIContent("-", "delete last element");

    /**
    * FUNCTION NAME: DrawArray
    * DESCRIPTION  : Allows customization of how arrays are drawn in the inspector.
    * INPUTS       : _array    - Array to modify the inspector draw of.
    *                _drawMode - Defines the way the array will be drawn in the inspector.
    * OUTPUTS      : None
    **/
    public static void DrawArray(SerializedProperty _array, LPK_EditorArrayDrawMode _drawMode =LPK_EditorArrayDrawMode.DRAW_MODE_DEFAULT)
    {
        if(_drawMode == LPK_EditorArrayDrawMode.DRAW_MODE_DEFAULT)
            DrawArrayDefault(_array);
        else if(_drawMode == LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS)
            DrawArrayWithButtons(_array);
    }

    /**
    * FUNCTION NAME: DrawArrayDefault
    * DESCRIPTION  : Draw an array to look like the standard Unity inspector.  This begs into question
    *                why someone would use this method...
    * INPUTS       : _array - Array to modify the inspector draw of.
    * OUTPUTS      : None
    **/
    static void DrawArrayDefault(SerializedProperty _array)
    {
        EditorGUILayout.PropertyField(_array);

        //Draw array children.
        if(_array.isExpanded)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(_array.FindPropertyRelative("Array.size"));

            for(int i = 0; i < _array.arraySize; i++)
                EditorGUILayout.PropertyField(_array.GetArrayElementAtIndex(i), new GUIContent(_array.displayName + " " + (i+1) ));
        }

        EditorGUI.indentLevel -= 1;
    }

    /**
    * FUNCTION NAME: DrawArrayWithButtons
    * DESCRIPTION  : Draw an array to have buttons instead of a "size" field.
    * INPUTS       : _array - Array to modify the inspector draw of.
    * OUTPUTS      : None
    **/
    static void DrawArrayWithButtons(SerializedProperty _array)
    {
        EditorGUILayout.PropertyField(_array);

        //Draw array children.
        if(_array.isExpanded)
        {
            EditorGUI.indentLevel += 1;

            for(int i = 0; i < _array.arraySize; i++)
                EditorGUILayout.PropertyField(_array.GetArrayElementAtIndex(i), new GUIContent(_array.displayName + " " + (i+1)));
        
            //Button draw.
            GUILayout.Space(5.0f);
            EditorGUILayout.LabelField("Add/Delete " + _array.displayName, EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(17.0f * EditorGUI.indentLevel);

            //Add button.
            if(GUILayout.Button(m_AddButtonContent, EditorStyles.miniButtonLeft, GUILayout.MaxWidth(50.0f)))
            {
                if (_array.arraySize > 1)
                {
                    _array.InsertArrayElementAtIndex(_array.arraySize - 1);
                    //TODO:  Reset value to default.
                }
                else
                    _array.arraySize += 1;
            }
        
            //Delete button.
            if(GUILayout.Button(m_DeleteButtonContent, EditorStyles.miniButtonRight, GUILayout.MaxWidth(50.0f)))
            {   
                if(_array.arraySize > 0)
                    _array.arraySize -= 1;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 1;
       }
    }
}

#endif  //UNITY_EDITOR

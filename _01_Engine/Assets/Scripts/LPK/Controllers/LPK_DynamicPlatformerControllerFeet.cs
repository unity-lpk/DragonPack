/***************************************************
File:           LPK_DynamicPlatformerControllerFeet.cs
Authors:        Christopher Onorati
Last Updated:   11/3/2019
Last Version:   2019.1.14

Description:
  This component allows for grounded detection via the feet
  collider of an LPK_DynamicPlatformerController.

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
* CLASS NAME  : LPK_DynamicPlatformerControllerFeet
* DESCRIPTION : Used for grounded detection.
**/
public class LPK_DynamicPlatformerControllerFeet : LPK_Component
{
    /************************************************************************************/

    //Counter used to detect how many contacts have been made.
    public int m_iContacts;

    /************************************************************************************/

    //Transform of the feet.
    Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Cache the Transform component.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
    }

    /**
    * FUNCTION NAME: OnCollisionEnter2D
    * DESCRIPTION  : Detect collision to determine grounded state.
    * INPUTS       : collision - Infomration RE: collision interaction.
    * OUTPUTS      : None
    **/
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Ignore our own parent.
        if (m_cTransform.parent == collision.gameObject)
            return;

        m_iContacts++;

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Feet started colliding with: " + collision.gameObject.name + ".  Current collision count is now: " + m_iContacts + ".");
    }

    /**
    * FUNCTION NAME: OnCollisionEnter2D
    * DESCRIPTION  : Detect collision to determine grounded state.
    * INPUTS       : collision - Infomration RE: collision interaction.
    * OUTPUTS      : None
    **/
    void OnCollisionExit2D(Collision2D collision)
    {
        //Ignore our own parent.
        if (m_cTransform.parent == collision.gameObject)
            return;

        m_iContacts--;

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Feet stopped colliding with: " + collision.gameObject.name + ".  Current collision count is now: " + m_iContacts + ".");
    }

    /**
    * FUNCTION NAME: OnTriggerEnter2D
    * DESCRIPTION  : Detect collision to determine grounded state.
    * INPUTS       : collision - Infomration RE: collision interaction.
    * OUTPUTS      : None
    **/
    void OnTriggerEnter2D (Collider2D collision)
    {
        //Ignore our own parent.
        if (m_cTransform.parent == collision.gameObject)
            return;

        if (collision.isTrigger)
            return;

        m_iContacts++;

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Feet started colliding with: " + collision.gameObject.name + ".  Current collision count is now: " + m_iContacts + ".");
    }

    /**
    * FUNCTION NAME: OnTriggerExit2D
    * DESCRIPTION  : Detect collision to determine grounded state.
    * INPUTS       : collision - Infomration RE: collision interaction.
    * OUTPUTS      : None
    **/
    void OnTriggerExit2D(Collider2D collision)
    {
        //Ignore our own parent.
        if (m_cTransform.parent == collision.gameObject)
            return;

        if (collision.isTrigger)
            return;

        m_iContacts--;

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Feet stopped colliding with: " + collision.gameObject.name + ".  Current collision count is now: " + m_iContacts + ".");
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DynamicPlatformerControllerFeet))]
public class LPK_DynamicPlatformerControllerFeetEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DynamicPlatformerControllerFeet owner = (LPK_DynamicPlatformerControllerFeet)target;

        LPK_DynamicPlatformerControllerFeet editorOwner = owner.GetComponent<LPK_DynamicPlatformerControllerFeet>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DynamicPlatformerControllerFeet)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DynamicPlatformerControllerFeet), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DynamicPlatformerControllerFeet");

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

}
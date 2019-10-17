/***************************************************
File:           LPK_ScreenSpaceRefractions.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component is added to a camera to make it render a
  RT texture that is used for LPK_ScreenRefract effects.
  This script is a tad advanced, so if you have any
  questions on usage, please contact the author.

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
* CLASS NAME  : LPK_ScreenSpaceRefractions
* DESCRIPTION : Create an RT texture via camera for use in LPK_ScreenRefract.
**/
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class LPK_ScreenSpaceRefractions : LPK_Component
{
    /************************************************************************************/

    public Vector2Int m_vecTextureResolution = new Vector2Int( 450, 285 );

    /************************************************************************************/

    //Name of the texture to generate.
    string m_sRealTimeTextureName = "_RTScreenSpaceRefractions";

    /************************************************************************************/

    Camera m_cCamera;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Create the RT texture and set shader properties.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_cCamera = GetComponent<Camera>();

        CreateRTTexture();
    }

    /**
    * FUNCTION NAME: CreateRTTexture
    * DESCRIPTION  : Create a real time texture for global use.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CreateRTTexture()
    {
        if(m_cCamera.targetTexture != null)
        {
            RenderTexture texture = m_cCamera.targetTexture;

            if(m_bPrintDebug)
                LPK_PrintWarning(this, "Attempted to create a screenspace refract texture on a camera that already had a texture set.  Destroying texture and replacing...");

            //Destroy existing texture to replace with our own.
            m_cCamera.targetTexture = null;
            DestroyImmediate(texture);
        }

        m_cCamera.targetTexture = new RenderTexture(m_vecTextureResolution.x, m_vecTextureResolution.y, 16);
        
        //Avoid chunky appearence of rendered objects.
        m_cCamera.targetTexture.filterMode = FilterMode.Bilinear;

        //Create global refernece to the texture for shaders to use.
        Shader.SetGlobalTexture(m_sRealTimeTextureName, m_cCamera.targetTexture);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_ScreenSpaceRefractions))]
public class LPK_ScreenSpaceRefractionsEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_ScreenSpaceRefractions owner = (LPK_ScreenSpaceRefractions)target;

        LPK_ScreenSpaceRefractions editorOwner = owner.GetComponent<LPK_ScreenSpaceRefractions>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ScreenSpaceRefractions)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ScreenSpaceRefractions), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ScreenSpaceRefractions");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_vecTextureResolution = EditorGUILayout.Vector2IntField(new GUIContent("Texture Resolution", "Resolution of the real time texture created.  High resolutions are more detailed, but significantly more expensive."), owner.m_vecTextureResolution);

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

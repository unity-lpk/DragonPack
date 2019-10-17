/***************************************************
File:           LPK_CameraShaderEnabler.cs
Authors:        Christopher Onorati
Last Updated:   6/10/2019
Last Version:   2018.3.14

Description:
  Allows the user to apply a post processing effect on
  a camera renderer.

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
* CLASS NAME  : LPK_CameraShaderEnabler
* DESCRIPTION : Component to enable post processing effects from
*               a single camera.
**/
[RequireComponent(typeof(Camera))]
public class LPK_CameraShaderEnabler : LPK_Component
{
    /************************************************************************************/

    public enum LPK_PostProcessorRenderMode
    {
        STANDARD = 0,
        MULTIPASS,
    }

    /************************************************************************************/

    [Tooltip("How to render this post processing effect.")]
    [Rename("Render Mode")]
    public LPK_PostProcessorRenderMode m_eRenderMode = LPK_PostProcessorRenderMode.STANDARD;

    [Tooltip("Shader to apply to the camera.  For multiple effects just add more of this component.")]
    [Rename("Shader Material")]
    public Material m_ShaderMat;

    [Tooltip("How many passes of this shader to make before rendering the screen.")]
    [Range(0, 10)]
    public int m_Iterations;

    [Tooltip("How many times to scale the resolution of the image down.  This will help with performance.")]
    [Range(0, 4)]
    public int m_ResolutionScale;

    /**
    * FUNCTION NAME: OnRenderImage
    * DESCRIPTION  : Sets up which shaders to apply to a rendering camera.
    * INPUTS       : src - Source image to modify before rendering.
    *                dst - Destination of the render (usually the screen).
    * OUTPUTS      : None
    **/
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if(m_ShaderMat == null)
            return;
        
        if(m_eRenderMode == LPK_PostProcessorRenderMode.STANDARD)
            RenderEffectStandard(src, dst);
        else if (m_eRenderMode == LPK_PostProcessorRenderMode.MULTIPASS)
            RenderEffectMultipass(src, dst);
    }

    /**
    * FUNCTION NAME: RenderEffectStandard
    * DESCRIPTION  : Render the post processing effect straight to the caemra.
    * INPUTS       : _src - Source image to modify before rendering.
    *                _dst - Destination of the render (usually the screen).
    * OUTPUTS      : None
    **/
    void RenderEffectStandard(RenderTexture _src, RenderTexture _dst)
    {
        Graphics.Blit(_src, _dst, m_ShaderMat);
    }

    /**
    * FUNCTION NAME: RenderEffectMultipass
    * DESCRIPTION  : Downscale the source image and render the effect multiple times before
    *                applying.  Used primarily for blur effects.
    * INPUTS       : _src - Source image to modify before rendering.
    *                _dst - Destination of the render (usually the screen).
    * OUTPUTS      : None
    **/
    void RenderEffectMultipass(RenderTexture _src, RenderTexture _dst)
    {
        //Downscale the camera resoluton.
        int width = _src.width >> m_ResolutionScale;
        int height = _src.height >> m_ResolutionScale;

        //Store each pass of the shader render here to apply after all iterations.
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(_src, rt);

        //Perform the shader for however many passes specified.
        for (int i = 0; i < m_Iterations; i++)
        {
            RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(rt, rt2, m_ShaderMat);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        Graphics.Blit(rt, _dst);
        RenderTexture.ReleaseTemporary(rt);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_CameraShaderEnabler))]
public class LPK_CameraShaderEnablerEditor : Editor
{
    SerializedProperty renderMode;
    SerializedProperty shaderMat;
    SerializedProperty iterations;
    SerializedProperty resolutionScale;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        renderMode = serializedObject.FindProperty("m_eRenderMode");
        shaderMat = serializedObject.FindProperty("m_ShaderMat");
        iterations = serializedObject.FindProperty("m_Iterations");
        resolutionScale = serializedObject.FindProperty("m_ResolutionScale");

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
        LPK_CameraShaderEnabler owner = (LPK_CameraShaderEnabler)target;

        LPK_CameraShaderEnabler editorOwner = owner.GetComponent<LPK_CameraShaderEnabler>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_CameraShaderEnabler)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_CameraShaderEnabler), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_CameraShaderEnabler");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(renderMode, true);
        EditorGUILayout.PropertyField(shaderMat, true);

        //Properties only needed for multipass rendering.
        if(renderMode.enumValueIndex == (int)LPK_CameraShaderEnabler.LPK_PostProcessorRenderMode.MULTIPASS)
        {
            EditorGUILayout.PropertyField(iterations, true);
            EditorGUILayout.PropertyField(resolutionScale, true);
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

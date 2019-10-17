/***************************************************
File:           LPK_DebugDrawForwardVector.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4f1

Description:
  Draws a forward pointing vector for the owner,
  with a set length and color by the user.

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
* CLASS NAME  : LPK_DebugDrawForwardVector
* DESCRIPTION : Draws a forward pointing vector for each object in the class.
**/
[ExecuteInEditMode]
public class LPK_DebugDrawForwardVector : LPK_DebugBase
{
    /************************************************************************************/

    [Tooltip("Length of the line to draw for forward vector.")]
    [Rename("Length")]
    public float m_flLength = 2.0f;

    [Tooltip("Color of the debug line if drawn.")]
    [Rename("Scene Line Color")]
    public Color m_vecDebugLineColor = Color.red;

    [Tooltip("Color of the line renderer if drawn in game.")]
    [Rename("Game Renderer Color")]
    public Color m_vecLineRendererColor = Color.red;

    [Tooltip("Color of the line renderer if drawn in build.")]
    [Rename("Build Renderer Color")]
    public Color m_vecBuildLineRendererColor = Color.red;

    /************************************************************************************/

    //Debug line to draw.
    LPK_DebugLineDrawer m_line;

    /**
    * FUNCTION NAME: OnUpdate
    * DESCRIPTION  : Used to clean up any leftover data when deactive.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    override protected void OnUpdate()
    {
        //if ((m_line != null && !m_bDrawInGame) && (m_line != null && !m_bDrawInEditor && !Application.isEditor && !Application.isPlaying))
            //DestroyImmediate(m_line.m_pGameObject);
    }

    /**
    * FUNCTION NAME: Draw
    * DESCRIPTION  : Draw debug info for the gameobject.
    * INPUTS       : _gameObj - Game object to draw debug info for.
    * OUTPUTS      : None
    **/
    override protected void Draw(GameObject _gameObj)
    {
        //Create line if appropriate
        if (m_bDrawInGame && (m_line == null || m_line.m_pGameObject == null) && Application.isEditor)
            m_line = new LPK_DebugLineDrawer(m_vecLineRendererColor, gameObject);
        else if (m_bDrawInScene && (m_line == null || m_line.m_pGameObject == null) && Application.isEditor)
          m_line = new LPK_DebugLineDrawer(m_vecDebugLineColor, gameObject);
        else if (m_bDrawInBuild && (m_line == null || m_line.m_pGameObject == null) && !Application.isEditor)
            m_line = new LPK_DebugLineDrawer(m_vecBuildLineRendererColor, gameObject);        

        if (Application.isEditor && m_bDrawInScene)
            Debug.DrawRay(m_cTransform.position, m_cTransform.right * m_flLength, m_vecDebugLineColor, 0.01f, true);
        
        if ((m_bDrawInBuild && !Application.isEditor && m_line != null) || (m_bDrawInGame && Application.isEditor && Application.isPlaying && m_line != null))
            m_line.DrawLineInGameView(m_cTransform.position, m_cTransform.position + (m_cTransform.right * m_flLength));
    }

    /**
    * FUNCTION NAME: Draw
    * DESCRIPTION  : Draw debug info for the gameobject.
    * INPUTS       : _gameObj - Game object to draw debug info for.
    * OUTPUTS      : None
    **/
    override protected void Undraw(GameObject _gameObj)
    {
        if (m_line != null)
            DestroyImmediate(m_line.m_pGameObject);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DebugDrawForwardVector))]
public class LPK_DebugDrawForwardVectorEditor : Editor
{
    SerializedProperty m_bDrawInGame;
    SerializedProperty m_bDrawInScene;
    SerializedProperty m_bDrawInBuild;
    SerializedProperty m_bDrawHierarchy;

    SerializedProperty m_flLength;
    SerializedProperty m_vecDebugLineColor;
    SerializedProperty m_vecLineRendererColor;
    SerializedProperty m_vecBuildLineRendererColor;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_bDrawInGame = serializedObject.FindProperty("m_bDrawInGame");
        m_bDrawInScene = serializedObject.FindProperty("m_bDrawInScene");
        m_bDrawInBuild = serializedObject.FindProperty("m_bDrawInBuild");
        m_bDrawHierarchy = serializedObject.FindProperty("m_bDrawHierarchy");

        m_flLength = serializedObject.FindProperty("m_flLength");
        m_vecDebugLineColor = serializedObject.FindProperty("m_vecDebugLineColor");
        m_vecLineRendererColor = serializedObject.FindProperty("m_vecLineRendererColor");
        m_vecBuildLineRendererColor = serializedObject.FindProperty("m_vecBuildLineRendererColor");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DebugDrawForwardVector owner = (LPK_DebugDrawForwardVector)target;

        LPK_DebugDrawForwardVector editorOwner = owner.GetComponent<LPK_DebugDrawForwardVector>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DebugDrawForwardVector)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DebugDrawForwardVector), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DebugDrawForwardVector");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Draw Modes", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_bDrawInScene, true);
        EditorGUILayout.PropertyField(m_bDrawInGame, true);
        EditorGUILayout.PropertyField(m_bDrawInBuild, true);
        EditorGUILayout.PropertyField(m_bDrawHierarchy, true);

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_flLength, true);

        if(m_bDrawInScene.boolValue == true)
            EditorGUILayout.PropertyField(m_vecDebugLineColor, true);
        if(m_bDrawInGame.boolValue == true)
            EditorGUILayout.PropertyField(m_vecLineRendererColor, true);
        if (m_bDrawInBuild.boolValue == true)
            EditorGUILayout.PropertyField(m_vecBuildLineRendererColor, true);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNITY_EDITOR

}   //LPK

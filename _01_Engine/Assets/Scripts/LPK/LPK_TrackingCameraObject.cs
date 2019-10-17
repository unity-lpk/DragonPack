/***************************************************
File:           LPK_TrackingCameraObject.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  This component allows for an object to be added to a
  dynamic tracking system for a camera in 2D gameplay.
  This will work with perspective or orthogonal cameras.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_TrackingCamereaInstantaneousObjectManager
* DESCRIPTION : Keep track of game objects cameras should be tracking.
**/
static class LPK_TrackingCamereaInstantaneousObjectManager
{
    //List of all game objects to track.
    static public List<GameObject> m_pGameObjects = new List<GameObject>();
    static public List<float> m_pWeights = new List<float>();

    /**
     * FUNCTION NAME: AddObject
     * DESCRIPTION  : Addan object to the track list.
     * INPUTS       : _addGameObject - Game object to potentially add to be tracked.
     * OUTPUTS      : None
     **/
    static public void AddObject(GameObject _addGameObject, float _weight)
    {
        if (m_pGameObjects.Contains(_addGameObject))
            return;

        m_pGameObjects.Add(_addGameObject);
        m_pWeights.Add(_weight);
    }

     /**
     * FUNCTION NAME: RemoveObject
     * DESCRIPTION  : Remove an object from the track list.
     * INPUTS       : _removeGameObject - Object to potentially remove to be tracked.
     * OUTPUTS      : None
     **/
    static public void RemoveObject(GameObject _removeGameObject)
    {
        int location = -1;

        for (int i = 0; i < m_pGameObjects.Count; i++)
        {
            if (m_pGameObjects[i] == _removeGameObject)
            {
                location = i;
                break;
            }
        }

        //Unable to find the object.
        if (location == -1)
            return;

        m_pGameObjects.RemoveAt(location);
        m_pWeights.RemoveAt(location);
    }
}

/**
* CLASS NAME  : LPK_TrackingCameraObject
* DESCRIPTION : Used to communicate to the game that an important object exists.
**/
[RequireComponent(typeof(Transform))]
public class LPK_TrackingCameraObject : LPK_Component
{
    /************************************************************************************/

    public float m_flImportanceWeight = 1.0f;

    /************************************************************************************/

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Manage initial event hookup.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        LPK_TrackingCamereaInstantaneousObjectManager.AddObject(gameObject, m_flImportanceWeight);

        //Kinda just here for consistency across all components.
        if(m_bPrintDebug)
        {
            LPK_PrintDebug(this, "Added object to be tracked: " + gameObject.name);
        }
    }

    /**
    * FUNCTION NAME: OnDestroyed
    * DESCRIPTION  : Used to remove instantaneously tracked game objects.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        LPK_TrackingCamereaInstantaneousObjectManager.RemoveObject(gameObject);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_TrackingCameraObject))]
public class LPK_TrackingCameraObjectEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_TrackingCameraObject owner = (LPK_TrackingCameraObject)target;

        LPK_TrackingCameraObject editorOwner = owner.GetComponent<LPK_TrackingCameraObject>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_TrackingCameraObject)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_TrackingCameraObject), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_TrackingCameraObject");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_flImportanceWeight = EditorGUILayout.FloatField(new GUIContent("Importance Factor", "Weight of the object's importance towards the camera."), owner.m_flImportanceWeight);
       
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

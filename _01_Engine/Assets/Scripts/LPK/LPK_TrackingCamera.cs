/***************************************************
File:           LPK_TrackingCamera.cs
Authors:        Christopher Onorati
Last Updated:   10/29/2019
Last Version:   2019.1.14

Description:
  This component allows for a dynamic tracking camera in a
  2D game.  This will work with perspective or orthogonal
  cameras.

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
* CLASS NAME  : LPK_TrackingCamera
* DESCRIPTION : Implementation of a weighted tracking camera for 2D games.
**/
[RequireComponent(typeof(Camera), typeof(Transform))]
public class LPK_TrackingCamera : LPK_Component
{
    /************************************************************************************/

    public Vector3 m_vecOffset;

    public float m_flCullingDistane = Mathf.Infinity;

    public bool m_bLockXTranslation = false;
    public bool m_bLockYTranslation = false;

    public float m_flMaxTranslationChangeScalar = 0.2f;
    public float m_flMaxSizeScalar = 0.2f;

    public float m_flMinSize = 6.5f;
    public float m_flMaxSize = 10.0f;

    public float m_flSizeScalarPoint = 10.0f;

    /************************************************************************************/

    //Stores initial Z value of camera.
    float m_flCameraZ;

    //Stores the initial size of the camera.
    float m_flInitialSize;

    //Holds a list of all important objects.
    List<Transform> m_aImportantObjects = new List<Transform>();
    //Holds a list of each object's importance.
    List<float> m_aImportanceWeights = new List<float>();

    float m_flLockedXValue;
    float m_flLockedYValue;

    //Location of the camera on the last frame.
    Vector3 m_vecPreviousLocation;

    /************************************************************************************/

    Transform m_cTransform;
    Camera m_cCamera;

    /**
    * FUNCTION NAME: Awake
    * DESCRIPTION  : Sets up initial values for the camera to use. Awake
    *                is good to use in this case as we must make sure all
    *                objects are spawned before we send out distance events.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Awake ()
    {
        m_cTransform = GetComponent<Transform>();
        m_cCamera = GetComponent<Camera>();

        m_flCameraZ = m_cTransform.position.z;

        m_vecPreviousLocation = m_cTransform.position;

        if (m_cCamera.orthographic)
            m_flInitialSize = m_cCamera.orthographicSize;
        else
            m_flInitialSize = m_cCamera.fieldOfView;

        if (m_bLockXTranslation)
            m_flLockedXValue = m_cTransform.position.x;
        if (m_bLockYTranslation)
            m_flLockedYValue = m_cTransform.position.y;
    }

    /**
    * FUNCTION NAME: FixedUpdate
    * DESCRIPTION  : Manages camera movement.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FixedUpdate()
    {
        //Stores the average position for the camera.
        Vector3 pos = new Vector3();
        
        //Add instantaneous objects.
        for(int i = 0; i < LPK_TrackingCamereaInstantaneousObjectManager.m_pGameObjects.Count; i++)
        {
            if (m_aImportantObjects.Contains(LPK_TrackingCamereaInstantaneousObjectManager.m_pGameObjects[i].transform))
               continue;

            AddObject(LPK_TrackingCamereaInstantaneousObjectManager.m_pGameObjects[i], LPK_TrackingCamereaInstantaneousObjectManager.m_pWeights[i]);
        }

        //Preventing camera updating if there are no objects to check for.
        if (m_aImportantObjects.Count <= 0)
            return;

        //Averaging the location of all important objects.
        for (int i = 0; i < m_aImportantObjects.Count; i++)
        {
            //NOTENOTE:  Added ability to turn off a camera object from being tracked by checking enable state.  Makes perf worse but oh well?
            if (m_aImportantObjects[i] != null && m_aImportantObjects[i].GetComponent<LPK_TrackingCameraObject>().enabled &&
                Vector2.Distance(m_aImportantObjects[i].transform.position, m_cTransform.position) <= m_flCullingDistane)
            {
                pos.x += m_aImportantObjects[i].position.x * m_aImportanceWeights[i];
                pos.y += m_aImportantObjects[i].position.y * m_aImportanceWeights[i];
            }

            //Cleaning the array.
            else if(m_aImportantObjects[i] == null)
            {
                m_aImportantObjects.RemoveAt(i);
                m_aImportanceWeights.RemoveAt(i);
                i--;
            }
        }

        float totalImportance = 0.0f;

        //Finding the total weight.
        if (m_aImportanceWeights.Count > 0)
        {
            for (int i = 0; i < m_aImportanceWeights.Count; i++)
            {
                if(Vector2.Distance(m_aImportantObjects[i].transform.position, m_cTransform.position) <= m_flCullingDistane)
                    totalImportance += m_aImportanceWeights[i];
            }
        }

        //Prevent divide by 0 error.
        if(totalImportance <= 0)
        {
            if (m_bPrintDebug)
                LPK_PrintWarning(this, "Divide by 0 error in tracking camera.  This likely means there are no tracked game objects.  The camera will not move this frame.");

            return;
        }

        pos = pos / totalImportance;

        if (m_bLockXTranslation)
            pos.x = m_flLockedXValue;
        if (m_bLockYTranslation)
            pos.y = m_flLockedYValue;

        pos += m_vecOffset;

        m_cTransform.position = Vector3.Lerp(m_cTransform.position, pos, m_flMaxTranslationChangeScalar);
        m_cTransform.position = new Vector3(m_cTransform.position.x, m_cTransform.position.y, m_flCameraZ);

        m_vecPreviousLocation = m_cTransform.position;

        ScaleCameraView();

        //Print out list of all tracked game objects.
        if(m_bPrintDebug)
        {
            string printString = "Tracked Objects:\n";

            for (int i = 0; i < m_aImportantObjects.Count; i++)
                printString += m_aImportantObjects[i].name + "\n";

            LPK_PrintDebug(this, printString);
        }
    }

    /**
    * FUNCTION NAME: ScaleCameraView
    * DESCRIPTION  : Scales the view of the camera based on the position of objects from the camera.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void ScaleCameraView()
    {
        float maxdistance = 0.0f;
        Vector2 camLocation = new Vector2(m_cTransform.position.x, m_cTransform.position.y);

        //Determining the max distance any object is from the camera's position.
        for (int i = 0; i< m_aImportantObjects.Count; i += 1 )
        {
            if(m_aImportantObjects[i] != null)
            {
                Vector2 objLocation = new Vector2(m_aImportantObjects[i].position.x, m_aImportantObjects[i].position.y);
                float testDistance = Vector2.Distance(camLocation, objLocation);
                
                if(testDistance > maxdistance)
                    maxdistance = testDistance;
            }
            
            //Cleaning the array.
            else
            {
                m_aImportantObjects.RemoveAt(i);
                m_aImportanceWeights.RemoveAt(i);
            }
        }
        
        float scalar = (maxdistance / m_flSizeScalarPoint);
        float changeAmount = m_flInitialSize + scalar;

        if (m_cCamera.orthographic)
            UpdateCameraSize(scalar, changeAmount);
        else
            UpdateCameraFOV(scalar, changeAmount);
    }

    /**
    * FUNCTION NAME: UpdateCameraSize
    * DESCRIPTION  : Updates the size of the camera if orthographic.
    * INPUTS       : _scalar - Value to modify the size of the camera by.
    *                _changeAmount - Amount of change to make in camera size.
    * OUTPUTS      : None
    **/
    void UpdateCameraSize(float _scalar, float _changeAmount)
    {
        if (m_cCamera.orthographicSize != m_flInitialSize + _scalar)
        {
            //Max cap for size change.
            if (_changeAmount - m_cCamera.orthographicSize > m_flMaxSizeScalar)
                _changeAmount = m_cCamera.orthographicSize + m_flMaxSizeScalar;

            //Min cap for size change.
            else if (_changeAmount - m_cCamera.orthographicSize < -m_flMaxSizeScalar)
                _changeAmount = m_cCamera.orthographicSize - m_flMaxSizeScalar;


            m_cCamera.orthographicSize = _changeAmount;

            //Max cap.
            if (m_cCamera.orthographicSize > m_flMaxSize)
                m_cCamera.orthographicSize = m_flMaxSize;

            //Min cap.
            else if (m_cCamera.orthographicSize < m_flMinSize)
                m_cCamera.orthographicSize = m_flMinSize;
        }
    }

    /**
    * FUNCTION NAME: UpdateCameraFOV
    * DESCRIPTION  : Updates the FOV of the camera if perspective.
    * INPUTS       : scalar - Value to modify the FOV of the camera by.
    *                changeAmount - Amount of change to make in camera FOV.
    * OUTPUTS      : None
    **/
    void UpdateCameraFOV(float scalar, float changeAmount)
    {
        if (m_cCamera.fieldOfView != m_flInitialSize + scalar)
        {
            //Max cap for size change.
            if (changeAmount - m_cCamera.fieldOfView > m_flMaxSizeScalar)
                changeAmount = m_cCamera.fieldOfView + m_flMaxSizeScalar;

            //Min cap for size change.
            else if (changeAmount - m_cCamera.fieldOfView < -m_flMaxSizeScalar)
                changeAmount = m_cCamera.fieldOfView - m_flMaxSizeScalar;


            m_cCamera.fieldOfView = changeAmount;

            //Max cap.
            if (m_cCamera.fieldOfView > m_flMaxSize)
                m_cCamera.fieldOfView = m_flMaxSize;

            //Min cap.
            else if (m_cCamera.fieldOfView < m_flMinSize)
                m_cCamera.fieldOfView = m_flMinSize;
        }
    }

    /**
    * FUNCTION NAME: AddObject
    * DESCRIPTION  : Add an object to be tracked to the list of tracked objects.
    * INPUTS       : objectToAdd - Object to add to the track list.
    *                weight      - Weight of the new object to track.
    * OUTPUTS      : None
    **/
    void AddObject(GameObject objectToAdd, float weight)
    {
        m_aImportantObjects.Add(objectToAdd.transform);
        m_aImportanceWeights.Add(weight);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_TrackingCamera))]
public class LPK_TrackingCameraEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_TrackingCamera owner = (LPK_TrackingCamera)target;

        LPK_TrackingCamera editorOwner = owner.GetComponent<LPK_TrackingCamera>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_TrackingCamera)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_TrackingCamera), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_TrackingCamera");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_vecOffset = EditorGUILayout.Vector3Field(new GUIContent("Position Offset", "Offset to add to the end of the calculation on where the camera should look."), owner.m_vecOffset);
        owner.m_flCullingDistane = EditorGUILayout.FloatField(new GUIContent("Max Track Distance", "Distance at which tracked game objects will not be factored into determining the camera's position"), owner.m_flCullingDistane);
        owner.m_bLockXTranslation = EditorGUILayout.Toggle(new GUIContent("Lock X Translation", "Lock X translation to disable movement of the camera in this axis."), owner.m_bLockXTranslation);
        owner.m_bLockYTranslation = EditorGUILayout.Toggle(new GUIContent("Lock Y Translation", "Lock Y translation to disable movement of the camera in this axis."), owner.m_bLockYTranslation);
        owner.m_flMaxTranslationChangeScalar = EditorGUILayout.FloatField(new GUIContent("Translation Movement", "Maximum allowed movement in translation per frame."), owner.m_flMaxTranslationChangeScalar);
        owner.m_flMaxSizeScalar = EditorGUILayout.FloatField(new GUIContent("Camera Size Scalar", "Max change in camera size per frame."), owner.m_flMaxSizeScalar);
        owner.m_flMinSize = EditorGUILayout.FloatField(new GUIContent("Min Camera Size", "Minimum allowed size of the camera."), owner.m_flMinSize);
        owner.m_flMaxSize = EditorGUILayout.FloatField(new GUIContent("Max Camera Size", "Maximum allowed size of the camera."), owner.m_flMaxSize);
        owner.m_flSizeScalarPoint = EditorGUILayout.FloatField(new GUIContent("Size Scalar Point", "Scalar for the alternation of camera size."), owner.m_flSizeScalarPoint);

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

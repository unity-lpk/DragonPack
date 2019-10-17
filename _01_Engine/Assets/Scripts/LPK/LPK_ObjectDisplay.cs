/***************************************************
File:           LPK_ObjectDisplay.cs
Authors:        Christopher Onorati
Last Updated:   10/3/2019
Last Version:   2019.1.4f1

Description:
  This component serves as a display of values that is
  indicated by spawning and destroying objects.

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
* CLASS NAME  : LPK_ObjectDisplay
* DESCRIPTION : Create a display using objects (like hearts, arrows, etc.)
**/
[RequireComponent(typeof(Transform))]
public class LPK_ObjectDisplay : LPK_DisplayObject
{
    /************************************************************************************/

    public enum LPK_ObjectDisplayMode
    {
        SPRITE,
        PREFAB,
        COPY_SPRITE,
        COPY_SPRITE_PROPERTIES,
    };

    /************************************************************************************/

    [Tooltip("Display mode for the object display.  NOTE:  Copy Sprite will copy all of the Sprite properties.  Copy Sprite Properties will copy all properties of the sprite renderer ==EXCEPT== sprite.")]
    [Rename("Display Mode")]
    public LPK_ObjectDisplayMode m_eDisplayMode;

    [Tooltip("What object to serve as a display.")]
    [Rename("Display Prefab")]
    public GameObject m_pObjectPrefab;

    [Tooltip("Sprite Renderer to copy settings of.  If left empty, it will use default settings.")]
    [Rename("Copy Sprite Renderer")]
    public SpriteRenderer m_cCopySpriteRenderer;

    [Tooltip("What sprite to serve as a display.")]
    [Rename("Display Sprite")]
    public Sprite m_SpriteDisplay;

    public Vector3 m_vecSpawnOffset;

    public int m_iMaxDisplayObjects = 5;

    /************************************************************************************/

    //Previous value of the counter from last call.
    int m_iPreviousValue = 0;

    List<GameObject> m_pDisplayObjects = new List<GameObject>();

    /************************************************************************************/

    Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initializes events.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Todo:  Figure out how to shallow copy for a better implementation.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if (m_cCopySpriteRenderer == null || m_eDisplayMode == LPK_ObjectDisplayMode.SPRITE || m_eDisplayMode == LPK_ObjectDisplayMode.PREFAB)
            return;

        for(int i = 0; i < m_pDisplayObjects.Count; i++)
        {
            SpriteRenderer cModRenderer = m_pDisplayObjects[i].GetComponent<SpriteRenderer>();

            if (cModRenderer == null)
                continue;

            cModRenderer.color = m_cCopySpriteRenderer.color;
            cModRenderer.sortingLayerID = m_cCopySpriteRenderer.sortingLayerID;

            if (m_eDisplayMode == LPK_ObjectDisplayMode.COPY_SPRITE)
                cModRenderer.sprite = m_cCopySpriteRenderer.sprite;
        }
    }

    /**
    * FUNCTION NAME: UpdateDisplay
    * DESCRIPTION  : Changes the count of objects to use in the display.
    * INPUTS       : _currentVal - Current value of the display.
    *                _maxVal     - Max value of the display.
    * OUTPUTS      : None
    **/
    override public void UpdateDisplay(float _currentVal, float _maxVal)
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Display Update");

        //If the new value is greater than the older, create new display objects
        CreateObjects((int)_currentVal);

        //if the new value is smaller, then destroy the difference
        RemoveObjects((int)_currentVal);
    
        //Update old value to match the new one
        m_iPreviousValue = (int)_currentVal;
    }

    /**
    * FUNCTION NAME: CreateObjects
    * DESCRIPTION  : Creates objects used in the display.
    * INPUTS       : _createCount - How many objects to create in this pass.
    * OUTPUTS      : None
    **/
    void CreateObjects(int _createCount)
    {
        //If the new value is greater than the older, create new display objects
        if (_createCount > m_iPreviousValue)
        {
            for (int i = 0; i < _createCount - m_iPreviousValue && m_pDisplayObjects.Count < m_iMaxDisplayObjects; i++)
            {
                //Draw display using prefabs.
                if (m_eDisplayMode == LPK_ObjectDisplayMode.PREFAB)
                {
                    GameObject obj;
                    obj = Instantiate(m_pObjectPrefab, m_cTransform.position + (m_vecSpawnOffset * m_pDisplayObjects.Count), m_cTransform.rotation);

                    obj.transform.SetParent(transform);
                    m_pDisplayObjects.Add(obj);
                }

                //Draw sprite images (make components at runtime).
                else
                {
                    
                    GameObject obj = new GameObject();
                    obj.transform.position = m_cTransform.position + (m_vecSpawnOffset * m_pDisplayObjects.Count);
                    obj.name = "DisplayUI";

                    obj.AddComponent<SpriteRenderer>();
                    
                    if (m_eDisplayMode == LPK_ObjectDisplayMode.COPY_SPRITE_PROPERTIES || m_eDisplayMode == LPK_ObjectDisplayMode.SPRITE)
                        obj.GetComponent<SpriteRenderer>().sprite = m_SpriteDisplay;
                    else
                        obj.GetComponent<SpriteRenderer>().sprite = m_cCopySpriteRenderer.sprite; ;    

                    m_pDisplayObjects.Add(obj);
                }
            }
        }
    }

    /**
    * FUNCTION NAME: RemoveObjects
    * DESCRIPTION  : Removes objects used in the display.
    * INPUTS       : _removeCount - How many objects to remove in this pass.
    * OUTPUTS      : None
    **/
    void RemoveObjects(int _removeCount)
    {
        if (_removeCount < m_iPreviousValue && m_pDisplayObjects.Count > 0)
        {
            int totalLosses = Mathf.Abs(_removeCount - m_iPreviousValue);

            for (int i = 0; i < totalLosses; i++)
            {
                GameObject obj = m_pDisplayObjects[m_pDisplayObjects.Count - 1];
                Object.Destroy(obj);
                m_pDisplayObjects.RemoveAt(m_pDisplayObjects.Count - 1);
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_ObjectDisplay))]
public class LPK_ObjectDisplayEditor : Editor
{
    SerializedProperty m_eDisplayMode;
    SerializedProperty objectPrefab;
    SerializedProperty m_cCopySpriteRenderer;
    SerializedProperty m_SpriteDisplay;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_eDisplayMode = serializedObject.FindProperty("m_eDisplayMode");
        objectPrefab = serializedObject.FindProperty("m_pObjectPrefab");
        m_cCopySpriteRenderer = serializedObject.FindProperty("m_cCopySpriteRenderer");
        m_SpriteDisplay = serializedObject.FindProperty("m_SpriteDisplay");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_ObjectDisplay owner = (LPK_ObjectDisplay)target;

        LPK_ObjectDisplay editorOwner = owner.GetComponent<LPK_ObjectDisplay>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ObjectDisplay)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ObjectDisplay), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ObjectDisplay");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_eDisplayMode, true);

        if (m_eDisplayMode.enumValueIndex == 0 || m_eDisplayMode.enumValueIndex == 3)
            EditorGUILayout.PropertyField(m_SpriteDisplay, true);
        if (m_eDisplayMode.enumValueIndex == 2 || m_eDisplayMode.enumValueIndex == 3)
            EditorGUILayout.PropertyField(m_cCopySpriteRenderer, true);
        if (m_eDisplayMode.enumValueIndex == 1)
            EditorGUILayout.PropertyField(objectPrefab, true);

        owner.m_vecSpawnOffset = EditorGUILayout.Vector3Field(new GUIContent("Display Object Offset", "Where to spawn the next object relative to the previous."), owner.m_vecSpawnOffset);
        owner.m_iMaxDisplayObjects = EditorGUILayout.IntField(new GUIContent("Max Display Objects", "Maximum amount of objects to show in the display."), owner.m_iMaxDisplayObjects);

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

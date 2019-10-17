/***************************************************
File:           LPK_MultiTagManager.cs
Authors:        Christopher Onorati
Last Updated:   5/9/2019
Last Version:   2018.3.14

Description:
  This script contains a method for the LPK to support
  multiple tags on a single object.

  Ideally, this is added to every game object.  Game objects
  with LPK components have to contain this script.  Game
  objects that do not, however, will not be gauaranteed to have
  this script.  Therefore, anywhere tags are used will still check
  the game object's direct tag if the search for this script
  returns null.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LPK
{
/**
* CLASS NAME  : LPK_MultiTagManager
* DESCRIPTION : Manager to allow LPK game objects to have multiple tags.
**/
[DisallowMultipleComponent]
public class LPK_MultiTagManager : MonoBehaviour
{
    /************************************************************************************/

    [TagDropdown]
    [Tooltip("Tags that are recognized by the LPK system for the game object to have.")]
    public string[] m_Tags;

    /**
    * FUNCTION NAME: CheckForTag
    * DESCRIPTION  : Checks if a single tag is on a game object or the LPK_TagManager.
    * INPUTS       : _tag - Tag to search for.
    * OUTPUTS      : bool - True/false on if the tag was found.
    **/
    public bool CheckForTag(string _tag)
    {
        //Check immediate tag.
        if (gameObject.tag == _tag)
            return true;

        //Check tag array.
        for (int i = 0; i < m_Tags.Length; i++)
        {
            //Skip null tags.
            if(string.IsNullOrEmpty(m_Tags[i]))
                continue;
            
            if (m_Tags[i] == _tag)
                return true;
        }

        return false;
    }

    /**
    * FUNCTION NAME: CheckForTags
    * DESCRIPTION  : Checks if multiple tags are on a game object or the LPK_TagManager.
    * INPUTS       : _tags - Tags to search for.
    * OUTPUTS      : bool - True/false on if a match was found.
    **/
    public bool CheckForTags(string[] _tags)
    {
        //Check immediate tag.
        for (int i = 0; i < _tags.Length; i++)
        {
            //Skip null tags.
            if(string.IsNullOrEmpty(_tags[i]))
                continue;
            
            if (_tags[i] == gameObject.tag)
                return true;
        }

        //Check tag array.
        for (int i = 0; i < m_Tags.Length; i++)
        {
            //Skip null tags.
            if(string.IsNullOrEmpty(m_Tags[i]))
                continue;
            
            for (int j = 0; j < _tags.Length; j++)
            {
                //Skip null tags.
                if(string.IsNullOrEmpty(_tags[j]))
                    continue;

                if (m_Tags[i] == _tags[j])
                    return true;
            }
        }

        return false;
    }

    /**
    * FUNCTION NAME: CheckGameObjectForTags
    * DESCRIPTION  : Checks if multiple tags are on a passed object.
    * INPUTS       : _obj  - Object to search for tags on.
    *                _tags - Tags to search for on the passed game object.
    * OUTPUTS      : bool - True/false on if a match was found.
    **/
    static public bool CheckGameObjectForTags(GameObject _obj, string[] _tags)
    {
        //Tag test via LPK_TagManager.
        if(_obj.GetComponent<LPK_MultiTagManager>())
        {
            if(_obj.GetComponent<LPK_MultiTagManager>().CheckForTags(_tags))
                return true;
        }

        //Tag test direct.
        else 
        {
            for(int i = 0; i < _tags.Length; i++)
            {
                if(!string.IsNullOrEmpty(_tags[i]) && _obj.tag == _tags[i])
                    return true;
            }
        }

        return false;
    }

    /**
    * FUNCTION NAME: FindGameObjectsWithTag
    * DESCRIPTION  : Finds and returns the first game object found with a specific tag.  Note the search prioritizes
    *                the LPK_TagManager over direct game object tags.
    * INPUTS       : _gameObject - game object that caused this search.
    *                _tag        - Tag to search for.
    *                _ignoreSelf - Ignore the gameobject this tag manager is on for the search.  Default to false.
    * OUTPUTS      : GameObject - First game object found with the tag.
    **/
    static public GameObject FindGameObjectWithTag(GameObject _gameObject, string _tag, bool _ignoreSelf = false)
    {
        if(string.IsNullOrEmpty(_tag))
            return null;

        //Get all tag managers.
        LPK_MultiTagManager[] allObjects = Object.FindObjectsOfType<LPK_MultiTagManager>();

        //Check all of the LPK_TagManagers for the tags.
        for(int i = 0; i < allObjects.Length; i++)
        {
            if(allObjects[i].CheckForTag(_tag))
            {
                if(allObjects[i].gameObject == _gameObject && _ignoreSelf)
                    continue;
                else
                    return allObjects[i].gameObject;
            }
        }

        //Direct tag.
        GameObject[] directGameObjectsWithTag = GameObject.FindGameObjectsWithTag(_tag);

        for(int j = 0; j < directGameObjectsWithTag.Length; j++)
        {
            if(directGameObjectsWithTag[j].gameObject == _gameObject && _ignoreSelf)
                continue;
            else
                return directGameObjectsWithTag[j].gameObject;  
        }

        return null;
    }

    /**
    * FUNCTION NAME: FindGameObjectsWithTag
    * DESCRIPTION  : Finds all game objects in the scene with a tag.
    * INPUTS       : _gameObject - game object that caused this search.
    *                _tag        - Tag to search for.
    *                _ignoreSelf - Ignore the gameobject this tag manager is on for the search.  Default to false.
    * OUTPUTS      : list - List of game objects with the tag provided.
    **/
    static public List<GameObject> FindGameObjectsWithTag(GameObject _gameObject, string _tag, bool _ignoreSelf = false)
    {
        if(string.IsNullOrEmpty(_tag))
            return new List<GameObject>();

        //List of tagged objects that match requirements.
        List<GameObject>  taggedObjects = new List<GameObject>();

        //Get all tag managers.
        LPK_MultiTagManager[] allObjects = Object.FindObjectsOfType<LPK_MultiTagManager>();

        //Check all of the LPK_TagManagers for the tags.
        for(int i = 0; i < allObjects.Length; i++)
        {
            if(allObjects[i].CheckForTag(_tag))
            {
                if(allObjects[i].gameObject == _gameObject && _ignoreSelf)
                    continue;
                else
                    taggedObjects.Add(allObjects[i].gameObject);
            }
        }

        //Direct tag.
        GameObject[] directGameObjectsWithTag = GameObject.FindGameObjectsWithTag(_tag);

        for(int j = 0; j < directGameObjectsWithTag.Length; j++)
        {
            if(!taggedObjects.Contains(directGameObjectsWithTag[j]))
            {
                if(directGameObjectsWithTag[j].gameObject == _gameObject && _ignoreSelf)
                    continue;
                else
                    taggedObjects.Add(directGameObjectsWithTag[j].gameObject);  
            }
        }

        return taggedObjects;
    }

    /**
    * FUNCTION NAME: FindGameObjectsWithTags
    * DESCRIPTION  : Finds all game objects in the scene with a set of tags.
    * INPUTS       : _gameObject - game object that caused this search.
    *                _tags       - Tags to search for.
    *                _ignoreSelf - Ignore the gameobject this tag manager is on for the search.  Default to false.
    * OUTPUTS      : list - List of game objects with the tags provided.
    **/
    static public List<GameObject> FindGameObjectsWithTags(GameObject _gameObject, string[] _tags, bool _ignoreSelf = false)
    {
        //List of tagged objects that match requirements.
        List<GameObject>  taggedObjects = new List<GameObject>();

        //Get all tag managers.
        LPK_MultiTagManager[] allObjects = Object.FindObjectsOfType<LPK_MultiTagManager>();

        //Check all of the LPK_TagManagers for the tags.
        for(int i = 0; i < allObjects.Length; i++)
        {
            if(allObjects[i].CheckForTags(_tags))
            {
                if(allObjects[i].gameObject == _gameObject && _ignoreSelf)
                    continue;
                else
                    taggedObjects.Add(allObjects[i].gameObject);
            }
        }

        //Direct tags.
        for (int i = 0; i < _tags.Length; i++)
        {
            GameObject[] directGameObjectsWithTag = GameObject.FindGameObjectsWithTag(_tags[i]);

            for(int j = 0; j < directGameObjectsWithTag.Length; j++)
            {
                if(!taggedObjects.Contains(directGameObjectsWithTag[j]))
                {
                    if(directGameObjectsWithTag[j].gameObject == _gameObject && _ignoreSelf)
                        continue;
                    else
                        taggedObjects.Add(directGameObjectsWithTag[j].gameObject);  
                }
            }
        }

        return taggedObjects;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_MultiTagManager))]
public class LPK_MultiTagManagerEditor : Editor
{
    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_MultiTagManager owner = (LPK_MultiTagManager)target;

        LPK_MultiTagManager editorOwner = owner.GetComponent<LPK_MultiTagManager>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_MultiTagManager)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_MultiTagManager), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_MultiTagManager");

        GUILayout.Space(10);
        LPK_EditorArrayDraw.DrawArray(serializedObject.FindProperty("m_Tags"), LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNITY_EDITOR

}   //LPK

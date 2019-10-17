/***************************************************
File:           LPK_BaseClass.cs
Authors:        Christopher Onorati
Last Updated:   8/2/2019
Last Version:   2018.3.14

Description:
  This script contains the base class all LPK objects
  inheret from.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LPK_CONSOLE;  /* Developer console to print log messages. */

namespace LPK
{

/**
* CLASS NAME  : LPK_Component
* DESCRIPTION : Base object for all LPK objects to inherited off of.
**/
public class LPK_Component : MonoBehaviour
{

    //Commonly used enums.
    /************************************************************************************/

    public enum LPK_NonNumericModifyMode
    {
        NONE,
        SET,
        COPY,
    };

    public enum LPK_NumericModifyMode
    {
        NONE,
        SET,
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE,
        COPY,
    };

    public enum LPK_BasicNumericModifyMode
    {
        NONE,
        SET,
        ADD,
        SUBTRACT,
        COPY,
    };

    /************************************************************************************/

    //Print debug info.
    public bool m_bPrintDebug = false;

    //Label per-component.  Does not do anything, just allows notes in the inspector.
    public string m_sLabel;

    /************************************************************************************/

    /**
    * FUNCTION NAME: LPK_PrintDebug
    * DESCRIPTION  : Prints a formatted debug message.
    * INPUTS       : _caller  - Unity object (component) that invoked this function.
    *                _message - Additional message to print along with caller data.
    * OUTPUTS      : None
    **/
    protected void LPK_PrintDebug(Object _caller, string _message)
    {
        Debug.Log(_message + " | Caller: " + _caller + " | Game Object: " + gameObject.name);

        if (FindObjectOfType<LPK_DeveloperConsole>())
            LPK_DeveloperConsole.AddMessageToConsole("[LOG] " + _message + " | Caller: " + _caller + " | Game Object: " + gameObject.name);
    }

    /**
    * FUNCTION NAME: LPK_PrintDebugDispatchingEvent
    * DESCRIPTION  : Prints a formatted debug message for dispatching an event.
    * INPUTS       : _caller  - Unity object (component) that invoked this function.
    *                _event   - The LPK_EventObject being sent. 
*                    _action  - Action causing the event to be dispatched.
    * OUTPUTS      : None
    **/
    protected void LPK_PrintDebugDispatchingEvent(LPK_EventSendingInfo _event, Object _caller, string _action)
    {
        Debug.Log("Dispatching Event: " + _event.m_Event.name + " | Dispatching Mode: " + _event.m_EventSendingMode + "| Dispatched Due To: | " + _action + " | Caller: " + _caller + " | Game Object: " + gameObject.name);

        if(FindObjectOfType<LPK_DeveloperConsole>())
            LPK_DeveloperConsole.AddMessageToConsole("[LOG] " + "Dispatching Event: " + _event.m_Event.name + " | Dispatching Mode: " + _event.m_EventSendingMode + "| Dispatched Due To: | " + _action + " | Caller: " + _caller + " | Game Object: " + gameObject.name);
    }

    /**
    * FUNCTION NAME: LPK_PrintDebugReceiveEvent
    * DESCRIPTION  : Prints a formatted debug message for receiving an event.
    * INPUTS       : _caller  - Unity object (component) that invoked this function.
    *                _event   - The LPK_EventObject being sent. 
    * OUTPUTS      : None
    **/
    protected void LPK_PrintDebugReceiveEvent(LPK_EventObject _event, Object _caller)
    {
        Debug.Log("Received Event: " + _event.name + " | Caller: " + _caller + " | Game Object: " + gameObject.name);

        if(FindObjectOfType<LPK_DeveloperConsole>())
            LPK_DeveloperConsole.AddMessageToConsole("[LOG] " + "Received Event: " + _event.name + " | Caller: " + _caller + " | Game Object: " + gameObject.name);
    }

    /**
    * FUNCTION NAME: LPK_PrintDebugDispatchingEvent
    * DESCRIPTION  : Prints a formatted debug message for dispatching an event.
    * INPUTS       : _caller  - Unity object (component) that invoked this function.
    *                _event   - The LPK_EventObject being sent. 
*                    _action  - Action causing the event to be dispatched.
    * OUTPUTS      : None
    **/
    protected void LPK_PrintDebugDispatchingEvent(LPK_CollisionEventSendingInfo _event, Object _caller, string _action)
    {
        Debug.Log("Dispatching Event: " + _event.m_Event.name + " | Dispatching Mode: " + _event.m_EventSendingMode + "| Dispatched Due To: | " + _action + " | Caller: " + _caller + " | Game Object: " + gameObject.name);

        if(FindObjectOfType<LPK_DeveloperConsole>())
            LPK_DeveloperConsole.AddMessageToConsole("[LOG] " + "Dispatching Event: " + _event.m_Event.name + " | Dispatching Mode: " + _event.m_EventSendingMode + "| Dispatched Due To: | " + _action + " | Caller: " + _caller + " | Game Object: " + gameObject.name);
    }

    /**
    * FUNCTION NAME: LPK_PrintWarning
    * DESCRIPTION  : Prints a formatted debug warning.
    * INPUTS       : _caller  - Unity object (component) that invoked this function.
    *                _message - Additional message to print along with caller data.
    * OUTPUTS      : None
    **/
    protected void LPK_PrintWarning(Object _caller, string _message)
    {
        Debug.LogWarning(_message + " | Caller: " + _caller + " | Game Object: " + gameObject.name);

        if(FindObjectOfType<LPK_DeveloperConsole>())
            LPK_DeveloperConsole.AddMessageToConsole("[WARNING] " + _message + " | Caller: " + _caller + " | Game Object: " + gameObject.name);
    }

    /**
    * FUNCTION NAME: LPK_PrintError
    * DESCRIPTION  : Prints a formatted debug error.
    * INPUTS       : _caller  - Unity object (component) that invoked this function.
    *                _message - Additional message to print along with caller data.
    * OUTPUTS      : None
    **/
    protected void LPK_PrintError(Object _caller, string _message)
    {
        Debug.LogError(_message + " | Caller: " + _caller + " | Game Object: " + gameObject.name);

        if(FindObjectOfType<LPK_DeveloperConsole>())
            LPK_DeveloperConsole.AddMessageToConsole("[ERROR] " + _message + " | Caller: " + _caller + " | Game Object: " + gameObject.name);
    }

    /**
    * FUNCTION NAME: OnEvent
    * DESCRIPTION  : Empty function overriden to handle script action implementation.
    * INPUTS       : _activator - Activator specified for the event.  If null, all objects are considered the acivator.
    * OUTPUTS      : None
    **/
    public virtual void OnEvent(GameObject _activator)
    {
        //Implemented in each indivitual script.
    }

    /**
    * FUNCTION NAME: ShouldRespondToEvent
    * DESCRIPTION  : Checks if the event should be activated based on activator game object.
    * INPUTS       : _activator - Activator specified for the event.  If null, all objects are considered the acivator.
    * OUTPUTS      : true/false value of if the OnEvent function should be activated.
    **/
    protected bool ShouldRespondToEvent(GameObject _activator)
    {
        if(_activator != null && _activator != gameObject)
            return false;

        return true;
    }

    /**
    * FUNCTION NAME: GetGameObjectsInRadius
    * DESCRIPTION  : Fills a list with gameobjects found within a certain radius of the owner gameobject.
    * INPUTS       : _Gameobjects     - List to return objects in.
    *                _radius          - Max distance an object can be to be valid.
    *                _objectCount     - How many objects to find.  -1 is as many as possible.
    *                _tagName         - Specify to search for a specific tag.  This is much less expensive.
    * OUTPUTS      : None
    **/
    public void GetGameObjectsInRadius(List<GameObject> _gameObjects, float _radius, int _objectCount = -1, string _tagName = "")
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(_tagName);

        for (int i = 0; i < taggedObjects.Length; i++)
        {
            if(Vector3.Distance(taggedObjects[i].transform.position, transform.position) <= _radius)
            {
                _gameObjects.Add(taggedObjects[i]);
                _objectCount--;

                if (_objectCount == 0)
                    break;
            }
        }
    }
}

/**
* CLASS NAME  : LPK_EventSendingInfo
* DESCRIPTION : Event sending info for LPK events.
**/
[System.Serializable]
public class LPK_EventSendingInfo
{
    /************************************************************************************/

    public enum LPK_EventSendingMode
    {
        OWNER,
        TAGS,
        ALL,
    };

        /************************************************************************************/

    [Tooltip("Event to dispatch when conditions are met.")]
    public LPK_EventObject m_Event;

    [Tooltip("Event sending mode.  OWNER = only send to components on the same game object.  ALL = send to all game objects. TAGS = send to specified tags.")]
    public LPK_EventSendingMode m_EventSendingMode = LPK_EventSendingMode.ALL;

    [Tooltip("Tags which are valid receivers for the event.")]
    [TagDropdown]
    public string[] m_Tags;
}

/**
* CLASS NAME  : LPK_CollisionEventSendingInfo
* DESCRIPTION : Event sending info for LPK events with added "other" mode.
**/
[System.Serializable]
public class LPK_CollisionEventSendingInfo
{
    /************************************************************************************/

    public enum LPK_EventSendingMode
    {
        OTHER,
        OWNER,
        TAGS,
        ALL,
    };

    /************************************************************************************/

    [Tooltip("Event to dispatch when conditions are met.")]
    public LPK_EventObject m_Event;

    [Tooltip("Event sending mode.  OTHER = only send to components on the game object that interacted with this component.  OWNER = only send to components on the same game object.  ALL = send to all game objects. TAGS = send to specified tags.")]
    public LPK_EventSendingMode m_EventSendingMode;

    [Tooltip("Tags which are valid receivers for the event.")]
    [TagDropdown]
    public string[] m_Tags;
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(LPK_EventSendingInfo))]
public class LPK_EventSendingInfo_PropertiesDrawer : PropertyDrawer
{
    /**
    * FUNCTION NAME: OnGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : position - Position of the property drawer.
    *                property - Property to draw.
    *                label    - Display info.
    * OUTPUTS      : None
    **/
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty m_Event = property.FindPropertyRelative("m_Event");
        SerializedProperty m_EventSendingMode = property.FindPropertyRelative("m_EventSendingMode");
        SerializedProperty m_Tags = property.FindPropertyRelative("m_Tags");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        string DisplayName = fieldInfo.Name.Substring(fieldInfo.Name.IndexOf('_') + 1);

        m_Event.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                         m_Event.isExpanded, new GUIContent(DisplayName), true);

        if (m_Event.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1, position.width, EditorGUIUtility.singleLineHeight), m_Event);
            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_EventSendingMode);

            if (m_EventSendingMode.enumValueIndex == (int)LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                LPK_EditorArrayDraw.DrawArray(m_Tags, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

            EditorGUI.indentLevel = indent;
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    /**
    * FUNCTION NAME: GetPropertyHeight
    * DESCRIPTION  : Set height for this property in the inspector.
    * INPUTS       : property - Property to draw.
    *                label    - Display info.
    * OUTPUTS      : None
    **/
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty m_Event = property.FindPropertyRelative("m_Event");

        if (m_Event.isExpanded)
            return EditorGUIUtility.singleLineHeight * 3;
        else
            return EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_CollisionEventSendingInfo))]
public class LPK_CollisionEventSendingInfo_PropertiesDrawer : PropertyDrawer
{
    /**
    * FUNCTION NAME: OnGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : position - Position of the property drawer.
    *                property - Property to draw.
    *                label    - Display info.
    * OUTPUTS      : None
    **/
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty m_Event = property.FindPropertyRelative("m_Event");
        SerializedProperty m_EventSendingMode = property.FindPropertyRelative("m_EventSendingMode");
        SerializedProperty m_Tags = property.FindPropertyRelative("m_Tags");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        string DisplayName = fieldInfo.Name.Substring(fieldInfo.Name.IndexOf('_') + 1);


        m_Event.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                     m_Event.isExpanded, new GUIContent(DisplayName), true);

        if (m_Event.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1, position.width, EditorGUIUtility.singleLineHeight), m_Event);
            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_EventSendingMode);

            if (m_EventSendingMode.enumValueIndex == (int)LPK_CollisionEventSendingInfo.LPK_EventSendingMode.TAGS)
                LPK_EditorArrayDraw.DrawArray(m_Tags, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

            EditorGUI.indentLevel = indent;
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    /**
    * FUNCTION NAME: GetPropertyHeight
    * DESCRIPTION  : Set height for this property in the inspector.
    * INPUTS       : property - Property to draw.
    *                label    - Display info.
    * OUTPUTS      : None
    **/
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty m_Event = property.FindPropertyRelative("m_Event");

        if (m_Event.isExpanded)
            return EditorGUIUtility.singleLineHeight * 3;
        else
            return EditorGUIUtility.singleLineHeight;
    }
}

#endif  //UNITY_EDITOR

}   //LPK

/***************************************************
File:           LPK_ModifyTransformOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/27/2019
Last Version:   2019.1.14

Description:
  This component can be added to any object to cause
  it to modify an object's Transform properties upon
  receiving an event.

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
* CLASS NAME  : LPK_ModifyTransformOnEvent
* DESCRIPTION : Component used to modify the values of a transform.
**/
public class LPK_ModifyTransformOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_TransformMode
    {
        WORLD,
        LOCAL,
    };

    /************************************************************************************/

    [Tooltip("Transforms which will be modified.")]
    public Transform[] m_TargetModifyObjects;

    [Tooltip("How the change in the transform component will be applied.")]
    [Rename("Transform Mode")]
    public LPK_TransformMode m_eTransformMode;

    [System.Serializable]
    public class TranslateProperties
    {
        [Tooltip("How to modify the property upon receiving the event.")]
        [Rename("Translation Mode")]
        public LPK_NumericModifyMode m_eTranslationModifyMode;

        [Tooltip("New value to be used in the operation upon receiving the event.  If set to copy, this is used as an offset.")]
        [Rename("Translation Value")]
        public Vector3 m_vecTranslationValue;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Translation Copy Target")]
        public GameObject m_pTranslationCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Translation Copy Tag")]
        public string m_sTranslationCopyTag;
    }

    public TranslateProperties m_TranslateProperties;

    [System.Serializable]
    public class ScaleProperties
    {
        [Tooltip("How to modify the property upon receiving the event.")]
        [Rename("Scale Mode")]
        public LPK_NumericModifyMode m_eScaleModifyMode;

        [Tooltip("New value to be used in the operation upon receiving the event.  If set to copy, this is used as an offset.")]
        [Rename("Scale Value")]
        public Vector3 m_vecScaleValue;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Scale Copy Target")]
        public GameObject m_pScaleCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Scale Copy Tag")]
        public string m_sScaleCopyTag;
    }

    public ScaleProperties m_ScaleProperties;

    [System.Serializable]
    public class RotationProperties
    {
        [Tooltip("How to modify the property upon receiving the event.")]
        [Rename("Rotation Mode")]
        public LPK_NumericModifyMode m_eRotateModifyMode;

        [Tooltip("New value to be used in the operation upon receiving the event.  If set to copy, this is used as an offset.")]
        [Rename("Rotation Value")]
        public Vector3 m_vecRotateValue;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Rotation Copy Target")]
        public GameObject m_pRotateCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Rotation Copy Tag")]
        public string m_sRotationCopyTag;
    }

    public RotationProperties m_RotationProperties;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    [SerializeField]
    bool m_bHasSetup = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for object transformation modification.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger)
            m_EventTrigger.Register(this);
    }

    /**
    * FUNCTION NAME: OnDrawGizmosSelected
    * DESCRIPTION  : Set the default game object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDrawGizmosSelected()
    {
        if (!m_bHasSetup)
        {   
            m_TargetModifyObjects = new Transform[] { transform };
            m_bHasSetup = true;
        }
    }

    /**
    * FUNCTION NAME: OnEvent
    * DESCRIPTION  : Event validation.
    * INPUTS       : _activator - Game object that activated the event.  Null is all objects.
    * OUTPUTS      : None
    **/
    override public void OnEvent(GameObject _activator)
    {
        if(!ShouldRespondToEvent(_activator))
            return;

        SearchForModifyObjects();

        if (m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);
    }

    /**
    * FUNCTION NAME: SearchForModifyObjects
    * DESCRIPTION  : Performs the search for game objects to modify the transform of.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SearchForModifyObjects()
    {
        SetCopyTargets();

        for (int i = 0; i < m_TargetModifyObjects.Length; i++)
        {
            //Null checking.
            if (m_TargetModifyObjects[i] == null)
                return;

            if (m_eTransformMode == LPK_TransformMode.WORLD)
                ModifyTransformWorld(m_TargetModifyObjects[i]);
            else if (m_eTransformMode == LPK_TransformMode.LOCAL)
                ModifyTransformLocal(m_TargetModifyObjects[i]);

            //NOTENOTE:  The below code prevents a game object from getting negative scale as this is a tad confusing.
            //           Remove the below code to allow this component to modify a game object to actually have negative scale.
            Vector3 currentScale = m_TargetModifyObjects[i].transform.localScale;
            m_TargetModifyObjects[i].transform.localScale = new Vector3(Mathf.Clamp(currentScale.x, 0, Mathf.Infinity),
                                                                        Mathf.Clamp(currentScale.y, 0, Mathf.Infinity),
                                                                        Mathf.Clamp(currentScale.z, 0, Mathf.Infinity));
        }
    }

    /**
    * FUNCTION NAME: ModifyTransformWorld
    * DESCRIPTION  : Preforms change on transform of desired game object in world space.
    * INPUTS       : _modifyGameObject - Transform to modify.
    * OUTPUTS      : None
    **/
    void ModifyTransformWorld(Transform _modifyGameObject)
    {
        //Modify the Translation property based on the mode selected
            if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.SET)
            _modifyGameObject.position = m_TranslateProperties.m_vecTranslationValue;
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.ADD)
            _modifyGameObject.position += m_TranslateProperties.m_vecTranslationValue;
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _modifyGameObject.position -= m_TranslateProperties.m_vecTranslationValue;
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _modifyGameObject.position = Vector3.Scale(_modifyGameObject.position, m_TranslateProperties.m_vecTranslationValue);
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.DIVIDE)
            _modifyGameObject.position = Vector3.Scale(_modifyGameObject.position,
                                                                     new Vector3(1 / m_TranslateProperties.m_vecTranslationValue.x,
                                                                     1 / m_TranslateProperties.m_vecTranslationValue.y,
                                                                     1 / m_TranslateProperties.m_vecTranslationValue.z));
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_TranslateProperties.m_pTranslationCopyTarget != null && m_TranslateProperties.m_pTranslationCopyTarget.transform != null)
                _modifyGameObject.position = m_TranslateProperties.m_pTranslationCopyTarget.transform.position + m_TranslateProperties.m_vecTranslationValue;
        }

        //Modify the Scale property based on the mode selected
        if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.SET)
            _modifyGameObject.localScale = m_ScaleProperties.m_vecScaleValue;
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.ADD)
            _modifyGameObject.localScale += m_ScaleProperties.m_vecScaleValue;
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _modifyGameObject.localScale -= m_ScaleProperties.m_vecScaleValue;
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _modifyGameObject.localScale = Vector3.Scale(_modifyGameObject.localScale, m_ScaleProperties.m_vecScaleValue);
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.DIVIDE)
            _modifyGameObject.localScale = Vector3.Scale(_modifyGameObject.localScale,
                                                                      new Vector3(1 / m_ScaleProperties.m_vecScaleValue.x,
                                                                      1 / m_ScaleProperties.m_vecScaleValue.y,
                                                                      1 / m_ScaleProperties.m_vecScaleValue.z));
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_ScaleProperties.m_pScaleCopyTarget != null && m_ScaleProperties.m_pScaleCopyTarget.transform != null)
                _modifyGameObject.localScale = m_ScaleProperties.m_pScaleCopyTarget.transform.localScale + m_ScaleProperties.m_vecScaleValue;
        }

        //Modify the Rotation property based on the mode selected
        if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.SET)
            _modifyGameObject.eulerAngles = m_RotationProperties.m_vecRotateValue;
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.ADD)
            _modifyGameObject.eulerAngles += m_RotationProperties.m_vecRotateValue;
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _modifyGameObject.eulerAngles -= m_RotationProperties.m_vecRotateValue;
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _modifyGameObject.eulerAngles = Vector3.Scale(_modifyGameObject.eulerAngles, m_RotationProperties.m_vecRotateValue);
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.DIVIDE)
            _modifyGameObject.eulerAngles = Vector3.Scale(_modifyGameObject.eulerAngles,
                                                                      new Vector3(1 / m_RotationProperties.m_vecRotateValue.x,
                                                                      1 / m_RotationProperties.m_vecRotateValue.y,
                                                                      1 / m_RotationProperties.m_vecRotateValue.z));
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_RotationProperties.m_pRotateCopyTarget != null && m_RotationProperties.m_pRotateCopyTarget.transform != null)
                _modifyGameObject.eulerAngles = m_RotationProperties.m_pRotateCopyTarget.transform.eulerAngles + m_RotationProperties.m_vecRotateValue;
        }
    }

    /**
    * FUNCTION NAME: ModifyTransformLocal
    * DESCRIPTION  : Preforms change on transform of desired game object in local space.
    * INPUTS       : _modifyGameObject  - Transform to modify.
    * OUTPUTS      : None
    **/
    void ModifyTransformLocal(Transform _modifyGameObject)
    {
        //Modify the Translation property based on the mode selected
        if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.SET)
            _modifyGameObject.localPosition = m_TranslateProperties.m_vecTranslationValue;
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.ADD)
            _modifyGameObject.localPosition += m_TranslateProperties.m_vecTranslationValue;
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _modifyGameObject.localPosition -= m_TranslateProperties.m_vecTranslationValue;
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _modifyGameObject.localPosition = Vector3.Scale(_modifyGameObject.localPosition, m_TranslateProperties.m_vecTranslationValue);
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.DIVIDE)
            _modifyGameObject.localPosition = Vector3.Scale(_modifyGameObject.localPosition,
                                                                     new Vector3(1 / m_TranslateProperties.m_vecTranslationValue.x,
                                                                     1 / m_TranslateProperties.m_vecTranslationValue.y,
                                                                     1 / m_TranslateProperties.m_vecTranslationValue.z));
        else if (m_TranslateProperties.m_eTranslationModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_TranslateProperties.m_pTranslationCopyTarget != null && m_TranslateProperties.m_pTranslationCopyTarget.transform != null)
                _modifyGameObject.localPosition = m_TranslateProperties.m_pTranslationCopyTarget.transform.position + m_TranslateProperties.m_vecTranslationValue;

            if (m_bPrintDebug && m_TranslateProperties.m_pTranslationCopyTarget == null)
                LPK_PrintWarning(this, "Cannot find a transform copy target.");
        }

        //Modify the Scale property based on the mode selected
        if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.SET)
            _modifyGameObject.localScale = m_ScaleProperties.m_vecScaleValue;
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.ADD)
            _modifyGameObject.localScale += m_ScaleProperties.m_vecScaleValue;
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _modifyGameObject.localScale -= m_ScaleProperties.m_vecScaleValue;
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _modifyGameObject.localScale = Vector3.Scale(_modifyGameObject.localScale, m_ScaleProperties.m_vecScaleValue);
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.DIVIDE)
            _modifyGameObject.localScale = Vector3.Scale(_modifyGameObject.localScale,
                                                                      new Vector3(1 / m_ScaleProperties.m_vecScaleValue.x,
                                                                      1 / m_ScaleProperties.m_vecScaleValue.y,
                                                                      1 / m_ScaleProperties.m_vecScaleValue.z));
        else if (m_ScaleProperties.m_eScaleModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_ScaleProperties.m_pScaleCopyTarget != null && m_ScaleProperties.m_pScaleCopyTarget.transform != null)
                _modifyGameObject.localScale = m_ScaleProperties.m_pScaleCopyTarget.transform.localScale + m_ScaleProperties.m_vecScaleValue;

            if (m_bPrintDebug && m_ScaleProperties.m_pScaleCopyTarget == null)
                LPK_PrintWarning(this, "Cannot find a scale copy target.");
        }

        //Modify the Rotation property based on the mode selected
        if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.SET)
            _modifyGameObject.localEulerAngles = m_RotationProperties.m_vecRotateValue;
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.ADD)
            _modifyGameObject.localEulerAngles += m_RotationProperties.m_vecRotateValue;
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _modifyGameObject.localEulerAngles -= m_RotationProperties.m_vecRotateValue;
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _modifyGameObject.localEulerAngles = Vector3.Scale(_modifyGameObject.localEulerAngles, m_RotationProperties.m_vecRotateValue);
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.DIVIDE)
            _modifyGameObject.localEulerAngles = Vector3.Scale(_modifyGameObject.localEulerAngles,
                                                                      new Vector3(1 / m_RotationProperties.m_vecRotateValue.x,
                                                                      1 / m_RotationProperties.m_vecRotateValue.y,
                                                                      1 / m_RotationProperties.m_vecRotateValue.z));
        else if (m_RotationProperties.m_eRotateModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_RotationProperties.m_pRotateCopyTarget != null && m_RotationProperties.m_pRotateCopyTarget.transform != null)
                _modifyGameObject.localEulerAngles = m_RotationProperties.m_pRotateCopyTarget.transform.eulerAngles + m_RotationProperties.m_vecRotateValue;

            if (m_bPrintDebug && m_RotationProperties.m_pRotateCopyTarget == null)
                LPK_PrintWarning(this, "Cannot find a rotate copy target.");
        }
    }

    /**
    * FUNCTION NAME: SetCopyTargets
    * DESCRIPTION  : Sets targets to copy.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetCopyTargets()
    {
        if (m_TranslateProperties.m_pTranslationCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_TranslateProperties.m_sTranslationCopyTag))
                m_TranslateProperties.m_pTranslationCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_TranslateProperties.m_sTranslationCopyTag);
            else if (m_TranslateProperties.m_pTranslationCopyTarget == null)
                m_TranslateProperties.m_pTranslationCopyTarget = gameObject;
        }

        if (m_ScaleProperties.m_pScaleCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_ScaleProperties.m_sScaleCopyTag))
                m_ScaleProperties.m_pScaleCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_ScaleProperties.m_sScaleCopyTag);
            else if (m_ScaleProperties.m_pScaleCopyTarget == null)
                m_ScaleProperties.m_pScaleCopyTarget = gameObject;
        }

        if (m_RotationProperties.m_pRotateCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_RotationProperties.m_sRotationCopyTag))
                m_RotationProperties.m_pRotateCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_RotationProperties.m_sRotationCopyTag);
            else if (m_RotationProperties.m_pRotateCopyTarget == null)
                m_RotationProperties.m_pRotateCopyTarget = gameObject;
        }
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Removes game object from the event queue.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        if(m_EventTrigger != null)
            m_EventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_ModifyTransformOnEvent))]
public class LPK_ModifyTransformOnEventEditor : Editor
{
    SerializedProperty targetModifyObject;
    SerializedProperty transformMode;
    SerializedProperty translateProperties;
    SerializedProperty scaleProperties;
    SerializedProperty rotationProperties;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        targetModifyObject = serializedObject.FindProperty("m_TargetModifyObjects");

        transformMode = serializedObject.FindProperty("m_eTransformMode");
        translateProperties = serializedObject.FindProperty("m_TranslateProperties");
        scaleProperties = serializedObject.FindProperty("m_ScaleProperties");
        rotationProperties = serializedObject.FindProperty("m_RotationProperties");

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
        LPK_ModifyTransformOnEvent owner = (LPK_ModifyTransformOnEvent)target;

        LPK_ModifyTransformOnEvent editorOwner = owner.GetComponent<LPK_ModifyTransformOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyTransformOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyTransformOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyTransformOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(targetModifyObject, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
        EditorGUILayout.PropertyField(transformMode, true);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Modifications", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(translateProperties, true);
        EditorGUILayout.PropertyField(scaleProperties, true);
        EditorGUILayout.PropertyField(rotationProperties, true);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyTransformOnEvent.TranslateProperties))]
public class LPK_ModifyTransformOnEvent_TranslatePropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eTranslationModifyMode = property.FindPropertyRelative("m_eTranslationModifyMode");
        SerializedProperty m_vecTranslationValue = property.FindPropertyRelative("m_vecTranslationValue");
        SerializedProperty m_pTranslationCopyTarget = property.FindPropertyRelative("m_pTranslationCopyTarget");
        SerializedProperty m_sTranslationCopyTag = property.FindPropertyRelative("m_sTranslationCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eTranslationModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eTranslationModifyMode.isExpanded, new GUIContent("Translate Properties"), true);

        if(m_eTranslationModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eTranslationModifyMode);

            if(m_eTranslationModifyMode.enumValueIndex == (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pTranslationCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sTranslationCopyTag);
            }

            else if(m_eTranslationModifyMode.enumValueIndex != (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_vecTranslationValue);
        }

        EditorGUI.indentLevel = indent;

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
        SerializedProperty m_eTranslationModifyMode = property.FindPropertyRelative("m_eTranslationModifyMode");

        if (m_eTranslationModifyMode.enumValueIndex == (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.COPY)
            return m_eTranslationModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eTranslationModifyMode.enumValueIndex != (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.NONE)
            return m_eTranslationModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eTranslationModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyTransformOnEvent.ScaleProperties))]
public class LPK_ModifyTransformOnEvent_ScalePropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eScaleModifyMode = property.FindPropertyRelative("m_eScaleModifyMode");
        SerializedProperty m_vecScaleValue = property.FindPropertyRelative("m_vecScaleValue");
        SerializedProperty m_pScaleCopyTarget = property.FindPropertyRelative("m_pScaleCopyTarget");
        SerializedProperty m_sScaleCopyTag = property.FindPropertyRelative("m_sScaleCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eScaleModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eScaleModifyMode.isExpanded, new GUIContent("Scale Properties"), true);

        if(m_eScaleModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eScaleModifyMode);

            if(m_eScaleModifyMode.enumValueIndex == (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pScaleCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sScaleCopyTag);
            }

            else if(m_eScaleModifyMode.enumValueIndex != (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_vecScaleValue);
        }

        EditorGUI.indentLevel = indent;

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
        SerializedProperty m_eScaleModifyMode = property.FindPropertyRelative("m_eScaleModifyMode");

        if (m_eScaleModifyMode.enumValueIndex == (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.COPY)
            return m_eScaleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eScaleModifyMode.enumValueIndex != (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.NONE)
            return m_eScaleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eScaleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyTransformOnEvent.RotationProperties))]
public class LPK_ModifyTransformOnEvent_RotationPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eRotateModifyMode = property.FindPropertyRelative("m_eRotateModifyMode");
        SerializedProperty m_vecRotateValue = property.FindPropertyRelative("m_vecRotateValue");
        SerializedProperty m_pRotateCopyTarget = property.FindPropertyRelative("m_pRotateCopyTarget");
        SerializedProperty m_sRotationCopyTag = property.FindPropertyRelative("m_sRotationCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eRotateModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eRotateModifyMode.isExpanded, new GUIContent("Rotation Properties"), true);

        if(m_eRotateModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eRotateModifyMode);

            if(m_eRotateModifyMode.enumValueIndex == (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pRotateCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sRotationCopyTag);
            }

            else if(m_eRotateModifyMode.enumValueIndex != (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_vecRotateValue);
        }

        EditorGUI.indentLevel = indent;

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
        SerializedProperty m_eRotateModifyMode = property.FindPropertyRelative("m_eRotateModifyMode");

        if (m_eRotateModifyMode.enumValueIndex == (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.COPY)
            return m_eRotateModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eRotateModifyMode.enumValueIndex != (int)LPK_ModifyTransformOnEvent.LPK_NumericModifyMode.NONE)
            return m_eRotateModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eRotateModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

#endif  //UNITY_EDITOR

}   //LPK

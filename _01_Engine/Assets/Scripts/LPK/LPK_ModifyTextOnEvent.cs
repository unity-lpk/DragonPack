/***************************************************
File:           LPK_ModifyTextOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/24/2019
Last Version:   2019.1.14

Description:
  This component will change a specified game object's
  SpriteText Visible, Font, FontSize, VertexColor and
  Text properties to change upon receiving a specified
  event.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.UI; /* Text */
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ModifySpriteOnEvent
* DESCRIPTION : Component used to modify the values of text.
**/
public class LPK_ModifyTextOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Tagged object(s) whose text will be used to play sound.")]
    public GameObject[] m_TargetModifyObjects;

    [System.Serializable]
    public class VisibleProperties
    {
        [Tooltip("How to modify visibility upon receiving the event.")]
        [Rename("Visible Mode")]
        public LPK_NonNumericModifyMode m_eVisibleModifyMode;

        [Tooltip("New value to set visible flag to be.")]
        [Rename("Visible")]
        public bool m_bVisible;

        [Tooltip("Game Object whose visible flag will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Visible Copy Target")]
        public GameObject m_pVisibleCopyTarget;

        [Tooltip("Game Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only grab the first game object with the tag found.")]
        [Rename("Visible Copy Tag")]
        public string m_sVisibleCopyTag;
    }

    public VisibleProperties m_VisibleProperties;

    [System.Serializable]
    public class FontProperties
    {
        [Tooltip("How to modify the font upon receiving the event.")]
        [Rename("Font Mode")]
        public LPK_NonNumericModifyMode m_eFontModifyMode;

        [Tooltip("New font to use on the text.")]
        [Rename("Font")]
        public Font m_Font;

        [Tooltip("Game Object whose font will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Font Copy Target")]
        public GameObject m_pFontCopyTarget;

        [Tooltip("Game Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only grab the first game object with the tag found.")]
        [Rename("Font Copy Tag")]
        public string m_sFontCopyTag;
    }

    public FontProperties m_FontProperties;

    [System.Serializable]
    public class FontSizeProperties
    {
        [Tooltip("How to modify the font size upon receiving the event.")]
        [Rename("Font Mode")]
        public LPK_NumericModifyMode m_eFontSizeModifyMode;

        [Tooltip("Value to use for font size modification")]
        [Rename("Font Size Value")]
        public int m_iFontSize;

        [Tooltip("Game Object whose font size will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Font Size Copy Target")]
        public GameObject m_pFontSizeCopyTarget;

        [Tooltip("Game Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only grab the first game object with the tag found.")]
        [Rename("Font Size Copy Tag")]
        public string m_sFontSizeCopyTag;
    }

    public FontSizeProperties m_FontSizeProperties;

    [System.Serializable]
    public class ColorProperties
    {
        [Tooltip("How to modify color upon receiving the event.")]
        [Rename("Color Mode")]
        public LPK_BasicNumericModifyMode m_eVertexColorModifyMode;

        [Tooltip("Value to use for text color modification.")]
        [Rename("Color Value")]
        public Color m_vecColorValue;

        [Tooltip("Game Object whose color will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Color Copy Target")]
        public GameObject m_pVertexColorCopyTarget;

        [Tooltip("Game Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only grab the first game object with the tag found.")]
        [Rename("Color Copy Tag")]
        public string m_sColorCopyTag;
    }

    public ColorProperties m_VertexColorProperties;

    [System.Serializable]
    public class TextProperties
    {
        [Tooltip("How to modify text upon receiving the event.")]
        [Rename("Text Mode")]
        public LPK_NonNumericModifyMode m_eTextModifyMode;

        [Tooltip("New text to display.")]
        [Rename("Text")]
        public string m_sText;

        [Tooltip("Game Object whose text will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Color Copy Target")]
        public GameObject m_pTextCopyTarget;

        [Tooltip("Game Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Text Copy Tag")]
        public string m_sTextCopyTag;
    }

    public TextProperties m_TextProperties;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    [SerializeField]
    bool m_bHasSetup = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for text modification.
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
            if(gameObject.GetComponent<Text>() || gameObject.GetComponent<TextMesh>())
                m_TargetModifyObjects = new GameObject[] { gameObject };

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

        if (m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        SearchForModifyObjects();
    }

    /**
    * FUNCTION NAME: SearchForModifyObjects
    * DESCRIPTION  : Modify set objects text properties.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SearchForModifyObjects()
    {
        SetCopyTargets();

        for (int i = 0; i < m_TargetModifyObjects.Length; i++)
        {
            if (m_TargetModifyObjects[i].GetComponent<TextMesh>() != null)
                ModifyGameText(m_TargetModifyObjects[i]);
            else if (m_TargetModifyObjects[i].GetComponent<Text>() != null)
                ModifyUIText(m_TargetModifyObjects[i]);
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
        if (m_VisibleProperties.m_pVisibleCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_VisibleProperties.m_sVisibleCopyTag))
                m_VisibleProperties.m_pVisibleCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_VisibleProperties.m_sVisibleCopyTag);
            else if (m_VisibleProperties.m_pVisibleCopyTarget == null)
                m_VisibleProperties.m_pVisibleCopyTarget = gameObject;
        }

        if (m_FontProperties.m_pFontCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_FontProperties.m_sFontCopyTag))
                m_FontProperties.m_pFontCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_FontProperties.m_sFontCopyTag);
            else if (m_FontProperties.m_sFontCopyTag == null)
                m_FontProperties.m_pFontCopyTarget = gameObject;
        }

        if (m_FontSizeProperties.m_pFontSizeCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_FontSizeProperties.m_sFontSizeCopyTag))
                m_FontSizeProperties.m_pFontSizeCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_FontSizeProperties.m_sFontSizeCopyTag);
            else if (m_FontSizeProperties.m_sFontSizeCopyTag == null)
                m_FontSizeProperties.m_pFontSizeCopyTarget = gameObject;
        }

        if (m_VertexColorProperties.m_pVertexColorCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_VertexColorProperties.m_sColorCopyTag))
                m_VertexColorProperties.m_pVertexColorCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_VertexColorProperties.m_sColorCopyTag);
            else if (m_VertexColorProperties.m_sColorCopyTag == null)
                m_VertexColorProperties.m_pVertexColorCopyTarget = gameObject;
        }

        if (m_TextProperties.m_pTextCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_TextProperties.m_sTextCopyTag))
                m_TextProperties.m_pTextCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_TextProperties.m_sTextCopyTag);
            else if (m_TextProperties.m_sTextCopyTag == null)
                m_TextProperties.m_pTextCopyTarget = gameObject;
        }
    }

    /**
    * FUNCTION NAME: ModifyGameText
    * DESCRIPTION  : Modify text that is present in the scene.
    * INPUTS       : _modifyGameObject - Object to modify.
    * OUTPUTS      : None
    **/
    void ModifyGameText(GameObject _modifyGameObject)
    {
        TextMesh modText = _modifyGameObject.GetComponent<TextMesh>();

        //Modify the Visible property based on the mode selected
        if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.SET)
            _modifyGameObject.GetComponent<MeshRenderer>().enabled = m_VisibleProperties.m_bVisible;
        else if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_VisibleProperties.m_pVisibleCopyTarget != null && m_VisibleProperties.m_pVisibleCopyTarget.GetComponent<MeshRenderer>() != null)
                _modifyGameObject.GetComponent<MeshRenderer>().enabled = m_VisibleProperties.m_pVisibleCopyTarget.GetComponent<MeshRenderer>().enabled;
        }

        //Modify the Font property based on the mode selected
        if (m_FontProperties.m_eFontModifyMode == LPK_NonNumericModifyMode.SET)
            modText.font = m_FontProperties.m_Font;
        else if (m_FontProperties.m_eFontModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_FontProperties.m_pFontCopyTarget != null && m_FontProperties.m_pFontCopyTarget.GetComponent<TextMesh>() != null)
                modText.font = m_FontProperties.m_pFontCopyTarget.GetComponent<TextMesh>().font;
        }

        //Modify the FontSize property based on the mode selected
        if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.SET)
            modText.fontSize = m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.ADD)
            modText.fontSize += m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.SUBTRACT)
            modText.fontSize -= m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.MULTIPLY)
            modText.fontSize *= m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.DIVIDE)
            modText.fontSize /= m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_FontSizeProperties.m_pFontSizeCopyTarget != null && m_FontSizeProperties.m_pFontSizeCopyTarget.GetComponent<TextMesh>() != null)
                modText.fontSize = m_FontSizeProperties.m_pFontSizeCopyTarget.GetComponent<TextMesh>().fontSize;
        }

        //Modify the Color property based on the mode selected
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SET)
            modText.color = m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.ADD)
            modText.color += m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SUBTRACT)
            modText.color -= m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.COPY)
        {
            if (m_VertexColorProperties.m_pVertexColorCopyTarget != null && m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<TextMesh>() != null)
                modText.color = m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<TextMesh>().color;
        }

        //Modify the Text property based on the mode selected
        if (m_TextProperties.m_eTextModifyMode == LPK_NonNumericModifyMode.SET)
            modText.text = m_TextProperties.m_sText;
        else if (m_TextProperties.m_eTextModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_TextProperties.m_pTextCopyTarget != null && m_TextProperties.m_pTextCopyTarget.GetComponent<TextMesh>() != null)
                modText.text = m_TextProperties.m_pTextCopyTarget.GetComponent<TextMesh>().text;
        }
    }

    /**
    * FUNCTION NAME: ModifyUIText
    * DESCRIPTION  : Modify text that is present in a canvas.
    * INPUTS       : _modifyGameObject - Object to modify
    * OUTPUTS      : None
    **/
    void ModifyUIText(GameObject _modifyGameObject)
    {
        Text modText = _modifyGameObject.GetComponent<Text>();

        //Modify the Visible property based on the mode selected
        if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.SET)
            modText.enabled = m_VisibleProperties.m_bVisible;
        else if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_FontProperties.m_pFontCopyTarget != null && m_FontProperties.m_pFontCopyTarget.GetComponent<Text>() != null)
                modText.enabled = m_FontProperties.m_pFontCopyTarget.GetComponent<Text>().enabled;
        }

        //Modify the Font property based on the mode selected
        if (m_FontProperties.m_eFontModifyMode == LPK_NonNumericModifyMode.SET)
            modText.font = m_FontProperties.m_Font;
        else if (m_FontProperties.m_eFontModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_FontProperties.m_pFontCopyTarget != null && m_FontProperties.m_pFontCopyTarget.GetComponent<Text>() != null)
                modText.font = m_FontProperties.m_pFontCopyTarget.GetComponent<Text>().font;
        }

        //Modify the FontSize property based on the mode selected
        if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.SET)
            modText.fontSize = m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.ADD)
            modText.fontSize += m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.SUBTRACT)
            modText.fontSize -= m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.MULTIPLY)
            modText.fontSize *= m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.DIVIDE)
            modText.fontSize /= m_FontSizeProperties.m_iFontSize;
        else if (m_FontSizeProperties.m_eFontSizeModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_FontSizeProperties.m_pFontSizeCopyTarget != null && m_FontSizeProperties.m_pFontSizeCopyTarget.GetComponent<Text>() != null)
                modText.fontSize = m_FontSizeProperties.m_pFontSizeCopyTarget.GetComponent<Text>().fontSize;
        }

        //Modify the Color property based on the mode selected
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SET)
            modText.color = m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.ADD)
            modText.color += m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SUBTRACT)
            modText.color -= m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.COPY)
        {
            if (m_VertexColorProperties.m_pVertexColorCopyTarget != null && m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<TextMesh>() != null)
                modText.color = m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<TextMesh>().color;
        }

        //Modify the Text property based on the mode selected
        if (m_TextProperties.m_eTextModifyMode == LPK_NonNumericModifyMode.SET)
            modText.text = m_TextProperties.m_sText;
        else if (m_TextProperties.m_eTextModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_TextProperties.m_pTextCopyTarget != null && m_TextProperties.m_pTextCopyTarget.GetComponent<Text>() != null)
                modText.text = m_TextProperties.m_pTextCopyTarget.GetComponent<Text>().text;
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

[CustomEditor(typeof(LPK_ModifyTextOnEvent))]
public class LPK_ModifyTextOnEventEditor : Editor
{
    SerializedProperty targetModifyObject;

    SerializedProperty visibleProperties;
    SerializedProperty fontProperties;
    SerializedProperty fontSizeProperties;
    SerializedProperty vertexColorProperties;
    SerializedProperty textProperties;

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

        visibleProperties = serializedObject.FindProperty("m_VisibleProperties");
        fontProperties = serializedObject.FindProperty("m_FontProperties");
        fontSizeProperties = serializedObject.FindProperty("m_FontSizeProperties");
        vertexColorProperties = serializedObject.FindProperty("m_VertexColorProperties");
        textProperties = serializedObject.FindProperty("m_TextProperties");

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
        LPK_ModifyTextOnEvent owner = (LPK_ModifyTextOnEvent)target;

        LPK_ModifyTextOnEvent editorOwner = owner.GetComponent<LPK_ModifyTextOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyTextOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyTextOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyTextOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(targetModifyObject, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Modifications", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(visibleProperties, true);
        EditorGUILayout.PropertyField(fontProperties, true);
        EditorGUILayout.PropertyField(fontSizeProperties, true);
        EditorGUILayout.PropertyField(vertexColorProperties, true);
        EditorGUILayout.PropertyField(textProperties, true);

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

[CustomPropertyDrawer(typeof(LPK_ModifyTextOnEvent.VisibleProperties))]
public class LPK_ModifyTextOnEvent_VisiblePropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eVisibleModifyMode = property.FindPropertyRelative("m_eVisibleModifyMode");
        SerializedProperty m_bVisible = property.FindPropertyRelative("m_bVisible");
        SerializedProperty m_pVisibleCopyTarget = property.FindPropertyRelative("m_pVisibleCopyTarget");
        SerializedProperty m_sVisibleCopyTag = property.FindPropertyRelative("m_sVisibleCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eVisibleModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eVisibleModifyMode.isExpanded, new GUIContent("Visible Properties"), true);

        if(m_eVisibleModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eVisibleModifyMode);

            if(m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_bVisible);

            if(m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pVisibleCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sVisibleCopyTag);
            }
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
        SerializedProperty m_eVisibleModifyMode = property.FindPropertyRelative("m_eVisibleModifyMode");

        if(m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eVisibleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eVisibleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eVisibleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyTextOnEvent.FontProperties))]
public class LPK_ModifyTextOnEvent_FontProperties : PropertyDrawer
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
        SerializedProperty m_eFontModifyMode = property.FindPropertyRelative("m_eFontModifyMode");
        SerializedProperty m_Font = property.FindPropertyRelative("m_Font");
        SerializedProperty m_pFontCopyTarget = property.FindPropertyRelative("m_pFontCopyTarget");
        SerializedProperty m_sFontCopyTag = property.FindPropertyRelative("m_sFontCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eFontModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eFontModifyMode.isExpanded, new GUIContent("Font Properties"), true);

        if(m_eFontModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), m_eFontModifyMode);

            if(m_eFontModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_Font);

            if(m_eFontModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_pFontCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight), m_sFontCopyTag);
            }
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
        SerializedProperty m_eFontModifyMode = property.FindPropertyRelative("m_eFontModifyMode");

        if(m_eFontModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eFontModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eFontModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eFontModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eFontModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyTextOnEvent.FontSizeProperties))]
public class LPK_ModifyTextOnEvent_FontSizeProperties : PropertyDrawer
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
        SerializedProperty m_eFontSizeModifyMode = property.FindPropertyRelative("m_eFontSizeModifyMode");
        SerializedProperty m_iFontSize = property.FindPropertyRelative("m_iFontSize");
        SerializedProperty m_pFontSizeCopyTarget = property.FindPropertyRelative("m_pFontSizeCopyTarget");
        SerializedProperty m_sFontSizeCopyTag = property.FindPropertyRelative("m_sFontSizeCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eFontSizeModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eFontSizeModifyMode.isExpanded, new GUIContent("Font Size Properties"), true);

        if(m_eFontSizeModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), m_eFontSizeModifyMode);

            if(m_eFontSizeModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_pFontSizeCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight), m_sFontSizeCopyTag);
            }
            else if(m_eFontSizeModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_iFontSize);
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
        SerializedProperty m_eFontSizeModifyMode = property.FindPropertyRelative("m_eFontSizeModifyMode");

        if(m_eFontSizeModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.COPY)
            return m_eFontSizeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if (m_eFontSizeModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.NONE)
            return m_eFontSizeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eFontSizeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyTextOnEvent.ColorProperties))]
public class LPK_ModifyTextOnEvent_ColorPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eVertexColorModifyMode = property.FindPropertyRelative("m_eVertexColorModifyMode");
        SerializedProperty m_vecColorValue = property.FindPropertyRelative("m_vecColorValue");
        SerializedProperty m_pVertexColorCopyTarget = property.FindPropertyRelative("m_pVertexColorCopyTarget");
        SerializedProperty m_sColorCopyTag = property.FindPropertyRelative("m_sColorCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eVertexColorModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eVertexColorModifyMode.isExpanded, new GUIContent("Color Properties"), true);

        if(m_eVertexColorModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eVertexColorModifyMode);

            if(m_eVertexColorModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_BasicNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pVertexColorCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sColorCopyTag);
            }
            else if(m_eVertexColorModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_BasicNumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_vecColorValue);
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
        SerializedProperty m_eVertexColorModifyMode = property.FindPropertyRelative("m_eVertexColorModifyMode");

        if(m_eVertexColorModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_BasicNumericModifyMode.COPY)
            return m_eVertexColorModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eVertexColorModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_BasicNumericModifyMode.NONE)
            return m_eVertexColorModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eVertexColorModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyTextOnEvent.TextProperties))]
public class LPK_ModifyTextOnEvent_TextPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eTextModifyMode = property.FindPropertyRelative("m_eTextModifyMode");
        SerializedProperty m_sText = property.FindPropertyRelative("m_sText");
        SerializedProperty m_pTextCopyTarget = property.FindPropertyRelative("m_pTextCopyTarget");
        SerializedProperty m_sTextCopyTag = property.FindPropertyRelative("m_sTextCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eTextModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eTextModifyMode.isExpanded, new GUIContent("Text Properties"), true);

        if(m_eTextModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eTextModifyMode);

            if(m_eTextModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_sText);

            if(m_eTextModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pTextCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sTextCopyTag);
            }
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
        SerializedProperty m_eTextModifyMode = property.FindPropertyRelative("m_eTextModifyMode");

        if(m_eTextModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eTextModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eTextModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eTextModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eTextModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

#endif  //UNITY_EDITOR

}   //LPK

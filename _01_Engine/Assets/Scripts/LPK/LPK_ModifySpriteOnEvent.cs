/***************************************************
File:           LPK_ModifySpriteOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/2/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to cause
  it to modify an object's Sprite properties upon
  receiving an event.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.UI; /* Image */
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_ModifySpriteOnEvent
* DESCRIPTION : Component used to modify the values of a sprite.
**/
public class LPK_ModifySpriteOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Tagged object(s) whose sprite properties will be modified.")]
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

        [Tooltip("Object whose visible flag will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Visible Copy Target")]
        public GameObject m_pVisibleCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy. Note this will only affect the first object with the tag found.")]
        [Rename("Visible Copy Tag")]
        [TagDropdown]
        public string m_sVisibleCopyTag;
    }

    public VisibleProperties m_VisibleProperties;

    [System.Serializable]
    public class ColorProperties
    {
        [Tooltip("How to modify color upon receiving the event.")]
        [Rename("Color Mode")]
        public LPK_BasicNumericModifyMode m_eVertexColorModifyMode;

        [Tooltip("New value to set color to be.")]
        [Rename("Color Value")]
        public Color m_vecColorValue;

        [Tooltip("Object whose color will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Color Copy Target")]
        public GameObject m_pVertexColorCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy. Note this will only affect the first object with the tag found.")]
        [Rename("Color Copy Tag")]
        [TagDropdown]
        public string m_sColorCopyTag;
    }

    public ColorProperties m_VertexColorProperties;

    [System.Serializable]
    public class SpriteProperties
        {
        [Tooltip("How to modify the sprite upon receiving the event.")]
        [Rename("Texture Mode")]
        public LPK_NonNumericModifyMode m_eSpriteModifyMode;

        [Tooltip("New value to set visible flag to be.")]
        [Rename("New Texture")]
        public Sprite m_ModifySprite;

        [Tooltip("Object whose sprite will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Sprite Copy Target")]
        public GameObject m_pSpriteCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy. Note this will only affect the first object with the tag found.")]
        [Rename("Texture Copy Tag")]
        [TagDropdown]
        public string m_SpriteCopyTag;
    }

    public SpriteProperties m_SpriteProperties;

    [System.Serializable]
    public class MaterialProperties
    {
        [Tooltip("How to modify material upon receiving the event.")]
        [Rename("Material Mode")]
        public LPK_NonNumericModifyMode m_eMaterialModifyMode;

        [Tooltip("New value to set visible flag to be.")]
        [Rename("New Material")]
        public Material m_MaterialValue;

        [Tooltip("Object whose material will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Material Copy Target")]
        public GameObject m_pMaterialCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy. Note this will only affect the first object with the tag found.")]
        [Rename("Material Copy Tag")]
        [TagDropdown]
        public string m_sMaterialCopyTag;
    }

    public MaterialProperties m_MaterialProperties;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    bool m_bHasSetup = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for sprite and color modification.
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
            if(gameObject.GetComponent<SpriteRenderer>() || gameObject.GetComponent<Image>() || gameObject.GetComponent<RawImage>())
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
    * DESCRIPTION  : Perform search for game objects to be modified.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SearchForModifyObjects()
    {
        SetCopyTargets();

        for (int i = 0; i < m_TargetModifyObjects.Length; i++)
        {
            if (m_TargetModifyObjects[i].GetComponent<SpriteRenderer>() != null)
                ModifySprite(m_TargetModifyObjects[i]);
            else if (m_TargetModifyObjects[i].GetComponent<Image>() != null)
                ModifyImage(m_TargetModifyObjects[i]);
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

        if (m_VertexColorProperties.m_pVertexColorCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_VertexColorProperties.m_sColorCopyTag))
                m_VertexColorProperties.m_pVertexColorCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_VertexColorProperties.m_sColorCopyTag);
            else if (m_VertexColorProperties.m_sColorCopyTag == null)
                m_VertexColorProperties.m_pVertexColorCopyTarget = gameObject;
        }

        if (m_SpriteProperties.m_pSpriteCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_SpriteProperties.m_SpriteCopyTag))
                    m_SpriteProperties.m_pSpriteCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_SpriteProperties.m_SpriteCopyTag);
            else if (m_SpriteProperties.m_pSpriteCopyTarget == null)
                    m_SpriteProperties.m_pSpriteCopyTarget = gameObject;
        }

        if (m_MaterialProperties.m_pMaterialCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_MaterialProperties.m_sMaterialCopyTag))
                m_MaterialProperties.m_pMaterialCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_MaterialProperties.m_sMaterialCopyTag);
            else if (m_MaterialProperties.m_pMaterialCopyTarget == null)
                m_MaterialProperties.m_pMaterialCopyTarget = gameObject;
        }
    }

    /**
    * FUNCTION NAME: ModifySprite
    * DESCRIPTION  : Modify the display of a sprite.
    * INPUTS       : modifyObject - Object to modify.
    * OUTPUTS      : None
    **/
    void ModifySprite(GameObject modifyObject)
    {
        SpriteRenderer modSprite = modifyObject.GetComponent<SpriteRenderer>();

        //Modify the visible flag property based on the mode selected
        if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.SET)
            modSprite.enabled = m_VisibleProperties.m_bVisible;
        else if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_VisibleProperties.m_pVisibleCopyTarget != null && m_VisibleProperties.m_pVisibleCopyTarget.GetComponent<SpriteRenderer>() != null)
                modSprite.enabled = m_VisibleProperties.m_pVisibleCopyTarget.GetComponent<SpriteRenderer>().enabled;
        }

        //Modify the Color property based on the mode selected
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SET)
            modSprite.color = m_VertexColorProperties.m_vecColorValue;
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.ADD)
            modSprite.color += m_VertexColorProperties.m_vecColorValue;
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SUBTRACT)
            modSprite.color -= m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.COPY)
        {
            if (m_VertexColorProperties.m_pVertexColorCopyTarget != null && m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<SpriteRenderer>() != null)
                modSprite.color = m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<SpriteRenderer>().color;
        }

        //Modify the texture property based on the mode selected
        if (m_SpriteProperties.m_eSpriteModifyMode == LPK_NonNumericModifyMode.SET)
            modSprite.sprite = m_SpriteProperties.m_ModifySprite;
        else if (m_SpriteProperties.m_eSpriteModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_SpriteProperties.m_pSpriteCopyTarget != null && m_SpriteProperties.m_pSpriteCopyTarget.GetComponent<SpriteRenderer>() != null)
                modSprite.sprite = m_SpriteProperties.m_pSpriteCopyTarget.GetComponent<SpriteRenderer>().sprite;
        }

        //Modify the material property based on the mode selected
        if (m_MaterialProperties.m_eMaterialModifyMode == LPK_NonNumericModifyMode.SET)
            modSprite.material = m_MaterialProperties.m_MaterialValue;
        else if (m_MaterialProperties.m_eMaterialModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_MaterialProperties.m_pMaterialCopyTarget != null && m_MaterialProperties.m_pMaterialCopyTarget.GetComponent<SpriteRenderer>() != null)
                modSprite.material = m_MaterialProperties.m_pMaterialCopyTarget.GetComponent<SpriteRenderer>().material;
        }
    }

    /**
    * FUNCTION NAME: ModifyImage
    * DESCRIPTION  : Modify the display of a UI image.
    * INPUTS       : modifyObject - Object to modify
    * OUTPUTS      : None
    **/
    void ModifyImage(GameObject modifyObject)
    {
        Image modSprite = modifyObject.GetComponent<Image>();

        //Modify the visible flag property based on the mode selected
        if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.SET)
            modSprite.enabled = m_VisibleProperties.m_bVisible;
        else if (m_VisibleProperties.m_eVisibleModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_VisibleProperties.m_pVisibleCopyTarget != null && m_VisibleProperties.m_pVisibleCopyTarget.GetComponent<Image>() != null)
                modSprite.enabled = m_VisibleProperties.m_pVisibleCopyTarget.GetComponent<Image>().enabled;
        }

        //Modify the Color property based on the mode selected
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SET)
            modSprite.color = m_VertexColorProperties.m_vecColorValue;
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.ADD)
            modSprite.color += m_VertexColorProperties.m_vecColorValue;
        if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.SUBTRACT)
            modSprite.color -= m_VertexColorProperties.m_vecColorValue;
        else if (m_VertexColorProperties.m_eVertexColorModifyMode == LPK_BasicNumericModifyMode.COPY)
        {
            if (m_VertexColorProperties.m_pVertexColorCopyTarget != null && m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<Image>() != null)
                modSprite.color = m_VertexColorProperties.m_pVertexColorCopyTarget.GetComponent<Image>().color;
        }

        //Modify the texture property based on the mode selected
        if (m_SpriteProperties.m_eSpriteModifyMode == LPK_NonNumericModifyMode.SET)
            modSprite.sprite = m_SpriteProperties.m_ModifySprite;
        else if (m_SpriteProperties.m_eSpriteModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_SpriteProperties.m_pSpriteCopyTarget != null && m_SpriteProperties.m_pSpriteCopyTarget.GetComponent<Image>() != null)
                modSprite.sprite = m_SpriteProperties.m_pSpriteCopyTarget.GetComponent<Image>().sprite;
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

[CustomEditor(typeof(LPK_ModifySpriteOnEvent))]
public class LPK_ModifySpriteOnEventEditor : Editor
{
    SerializedProperty targetModifyObject;
    SerializedProperty visibleProperties;
    SerializedProperty vertexColorProperties;
    SerializedProperty m_SpriteProperties;
    SerializedProperty materialProperties;

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
        vertexColorProperties = serializedObject.FindProperty("m_VertexColorProperties");
        m_SpriteProperties = serializedObject.FindProperty("m_SpriteProperties");
        materialProperties = serializedObject.FindProperty("m_MaterialProperties");
  
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
        LPK_ModifySpriteOnEvent owner = (LPK_ModifySpriteOnEvent)target;

        LPK_ModifySpriteOnEvent editorOwner = owner.GetComponent<LPK_ModifySpriteOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifySpriteOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifySpriteOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifySpriteOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(targetModifyObject, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Modifications", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(visibleProperties, true);
        EditorGUILayout.PropertyField(vertexColorProperties, true);
        EditorGUILayout.PropertyField(m_SpriteProperties, true);
        EditorGUILayout.PropertyField(materialProperties, true);

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

[CustomPropertyDrawer(typeof(LPK_ModifySpriteOnEvent.VisibleProperties))]
public class LPK_ModifySpriteOnEvent_VisiblePropertiesDrawer : PropertyDrawer
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

            if(m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_bVisible);

            if(m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pVisibleCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight), m_sVisibleCopyTag);
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

        if(m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eVisibleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eVisibleModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eVisibleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eVisibleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifySpriteOnEvent.ColorProperties))]
public class LPK_ModifySpriteOnEvent_ColorPropertiesDrawer : PropertyDrawer
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

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), m_eVertexColorModifyMode);

            if(m_eVertexColorModifyMode.enumValueIndex != (int)LPK_ModifySpriteOnEvent.LPK_BasicNumericModifyMode.COPY && m_eVertexColorModifyMode.enumValueIndex != (int)LPK_ModifySpriteOnEvent.LPK_BasicNumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_vecColorValue);

            else if(m_eVertexColorModifyMode.enumValueIndex != (int)LPK_ModifySpriteOnEvent.LPK_BasicNumericModifyMode.NONE)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_pVertexColorCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight), m_sColorCopyTag);
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
        SerializedProperty m_eVertexColorModifyMode = property.FindPropertyRelative("m_eVertexColorModifyMode");

        if(m_eVertexColorModifyMode.enumValueIndex != (int)LPK_ModifySpriteOnEvent.LPK_BasicNumericModifyMode.COPY && m_eVertexColorModifyMode.enumValueIndex != (int)LPK_ModifySpriteOnEvent.LPK_BasicNumericModifyMode.NONE)
            return m_eVertexColorModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eVertexColorModifyMode.enumValueIndex != (int) LPK_ModifySpriteOnEvent.LPK_BasicNumericModifyMode.NONE)
            return m_eVertexColorModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight; 
        else
            return m_eVertexColorModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight; 

    }
}

[CustomPropertyDrawer(typeof(LPK_ModifySpriteOnEvent.SpriteProperties))]
public class LPK_ModifySpriteOnEvent_SpritePropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eSpriteModifyMode = property.FindPropertyRelative("m_eSpriteModifyMode");
        SerializedProperty m_ModifySprite = property.FindPropertyRelative("m_ModifySprite");
        SerializedProperty m_pSpriteCopyTarget = property.FindPropertyRelative("m_pSpriteCopyTarget");
        SerializedProperty m_sSpriteCopyTag = property.FindPropertyRelative("m_sSpriteCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eSpriteModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eSpriteModifyMode.isExpanded, new GUIContent("Sprite Properties"), true);

        if(m_eSpriteModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), m_eSpriteModifyMode);

            if(m_eSpriteModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_ModifySprite);

            if(m_eSpriteModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pSpriteCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sSpriteCopyTag);
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
        SerializedProperty m_eSpriteModifyMode = property.FindPropertyRelative("m_eSpriteModifyMode");

        if(m_eSpriteModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eSpriteModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eSpriteModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eSpriteModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eSpriteModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifySpriteOnEvent.MaterialProperties))]
public class LPK_ModifySpriteOnEvent_MaterialPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eMaterialModifyMode = property.FindPropertyRelative("m_eMaterialModifyMode");
        SerializedProperty m_MaterialValue = property.FindPropertyRelative("m_MaterialValue");
        SerializedProperty m_pMaterialCopyTarget = property.FindPropertyRelative("m_pMaterialCopyTarget");
        SerializedProperty m_sMaterialCopyTag = property.FindPropertyRelative("m_sMaterialCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eMaterialModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eMaterialModifyMode.isExpanded, new GUIContent("Material Properties"), true);

        if(m_eMaterialModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eMaterialModifyMode);

            if(m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_MaterialValue);

            if(m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pMaterialCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sMaterialCopyTag);
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
        SerializedProperty m_eMaterialModifyMode = property.FindPropertyRelative("m_eMaterialModifyMode");

        if(m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eMaterialModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifySpriteOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eMaterialModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eMaterialModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

#endif  //UNITY_EDITOR

}   //LPK

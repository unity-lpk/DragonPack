/***************************************************
File:           LPK_ModifySoundOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/24/2019
Last Version:   2019.1.14

Description:
  This component can be used to change the resolution
  of the game window during runtime.  This component is
  ideally used on an options menu.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEditor;

namespace LPK
{

public class LPK_ModifySoundOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Audio Source(s) that will be modified.")]
    public AudioSource[] m_TargetAudioSources;

    [System.Serializable]
    public class MuteProperties
    {
        [Tooltip("How to modify the audio source upon receiving the event.")]
        [Rename("Mute Mode")]
        public LPK_NonNumericModifyMode m_eMuteModifyMode;

        [Tooltip("New setting to use for the mute flag.")]
        [Rename("Mute")]
        public bool m_bMute;

        [Tooltip("Audio Source whose mute setting will be copied to the recipient's property value. Only used if mode is set to copy.  Default to self if this and the tag field are left unset.")]
        [Rename("Mute Copy Target")]
        public AudioSource m_cMuteCopyTarget;

        [Tooltip("Audio Source whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will grab the first audio source with the tag found.")]
        [Rename("Mute Copy Tag")]
        public string m_sMuteCopyTag;
    }

    public MuteProperties m_MuteProperties;

    [System.Serializable]
    public class LoopProperties
    {
        [Tooltip("How to modify the font upon receiving the event.")]
        [Rename("Loop Mode")]
        public LPK_NonNumericModifyMode m_eLoopModifyMode;

        [Tooltip("New setting to use for the mute flag.")]
        [Rename("Loop")]
        public bool m_bLoop;

        [Tooltip("Audio Source whose loop setting will be copied to the recipient's property value. Only used if mode is set to copy.  Default to self if this and the tag field are left unset.")]
        [Rename("Loop Copy Target")]
        public AudioSource m_cLoopCopyTarget;

        [Tooltip("Audio Source whose loop property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will grab the first audio source with the tag found.")]
        [Rename("Loop Copy Tag")]
        public string m_sLoopCopyTag;
    }

    public LoopProperties m_LoopProperties;

    [System.Serializable]
    public class VolumeProperties
    {
        [Tooltip("How to modify the volume upon receiving the event.")]
        [Rename("Volume Mode")]
        public LPK_NumericModifyMode m_eVolumeModifyMode;

        [Tooltip("Value to use for volume modification")]
        [Rename("Volume Value")]
        public float m_flVolume;

        [Tooltip("Audio Source whose volume setting will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Volume Copy Target")]
        public AudioSource m_cVolumeCopyTarget;

        [Tooltip("Audio Source whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only grab the first audio source with the tag found.")]
        [Rename("Volume Copy Tag")]
        public string m_sVolumeCopyTag;
    }

    public VolumeProperties m_VolumeProperties;

    [System.Serializable]
    public class PitchProperties
    {
        [Tooltip("How to modify the font size upon receiving the event.")]
        [Rename("Pitch Mode")]
        public LPK_NumericModifyMode m_ePitchSizeModifyMode;

        [Tooltip("Value to use for pitch modification")]
        [Rename("Pitch Value")]
        public float m_flPitch;

        [Tooltip("Audio Source whose pitch size will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Pitch Copy Target")]
        public AudioSource m_cPitchCopyTarget;

        [Tooltip("Game Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only grab the first audio source with the tag found.")]
        [Rename("Font Size Copy Tag")]
        public string m_sPitchCopyTag;
    }

    public PitchProperties m_PitchProperties;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    [SerializeField]
    bool m_bHasSetup = false;


    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for sound modification.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if (m_EventTrigger)
            m_EventTrigger.Register(this);
    }

    /**
    * FUNCTION NAME: OnDrawGizmosSelected
    * DESCRIPTION  : Set the default audio source.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDrawGizmosSelected()
    {
        if (!m_bHasSetup)
        {   
            if(gameObject.GetComponent<AudioSource>())
                m_TargetAudioSources = new AudioSource[] { GetComponent<AudioSource>() };

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

        SetCopyTargets();

        for(int i = 0; i < m_TargetAudioSources.Length; i++)
        {
            if (m_TargetAudioSources[i] != null)
            {
                ModifyLoopSetting(m_TargetAudioSources[i]);
                ModifyMuteSetting(m_TargetAudioSources[i]);
                ModifyVolumeSetting(m_TargetAudioSources[i]);
                ModifyPitchSetting(m_TargetAudioSources[i]);
            }
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
        if (m_MuteProperties.m_cMuteCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_MuteProperties.m_sMuteCopyTag))
                m_MuteProperties.m_cMuteCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_MuteProperties.m_sMuteCopyTag).GetComponent<AudioSource>();
            else if (m_MuteProperties.m_cMuteCopyTarget == null)
                m_MuteProperties.m_cMuteCopyTarget = gameObject.GetComponent<AudioSource>();
        }

        if (m_LoopProperties.m_cLoopCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_LoopProperties.m_sLoopCopyTag))
                m_LoopProperties.m_cLoopCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_LoopProperties.m_sLoopCopyTag).GetComponent<AudioSource>();
            else if (m_LoopProperties.m_cLoopCopyTarget == null)
                m_LoopProperties.m_cLoopCopyTarget = gameObject.GetComponent<AudioSource>();
        }

        if (m_VolumeProperties.m_cVolumeCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_VolumeProperties.m_sVolumeCopyTag))
                m_VolumeProperties.m_cVolumeCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_VolumeProperties.m_sVolumeCopyTag).GetComponent<AudioSource>();
            else if (m_VolumeProperties.m_cVolumeCopyTarget == null)
                m_VolumeProperties.m_cVolumeCopyTarget = gameObject.GetComponent<AudioSource>();
        }

        if (m_PitchProperties.m_cPitchCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_PitchProperties.m_sPitchCopyTag))
                m_PitchProperties.m_cPitchCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_PitchProperties.m_sPitchCopyTag).GetComponent<AudioSource>();
            else if (m_PitchProperties.m_cPitchCopyTarget == null)
                m_PitchProperties.m_cPitchCopyTarget = gameObject.GetComponent<AudioSource>();
        }
    }

    /**
    * FUNCTION NAME: ModifyMuteSetting
    * DESCRIPTION  : Modify the mute setting of an audio source if requested.
    * INPUTS       : _source - Audio source to potentially modify.
    * OUTPUTS      : None
    **/
    void ModifyMuteSetting(AudioSource _source)
    {
        if (m_MuteProperties.m_eMuteModifyMode == LPK_NonNumericModifyMode.SET)
            _source.mute = m_MuteProperties.m_bMute;
        else if (m_MuteProperties.m_eMuteModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_MuteProperties.m_cMuteCopyTarget != null)
                _source.mute = m_MuteProperties.m_cMuteCopyTarget.mute;
        } 
    }

    /**
    * FUNCTION NAME: ModifyLoopSetting
    * DESCRIPTION  : Modify the loop setting of an audio source if requested.
    * INPUTS       : _source - Audio source to potentially modify.
    * OUTPUTS      : None
    **/
    void ModifyLoopSetting(AudioSource _source)
    {
        if (m_LoopProperties.m_eLoopModifyMode == LPK_NonNumericModifyMode.SET)
            _source.loop = m_LoopProperties.m_bLoop;
        else if (m_LoopProperties.m_eLoopModifyMode == LPK_NonNumericModifyMode.COPY)
        {
            if (m_LoopProperties.m_cLoopCopyTarget != null)
                _source.loop = m_LoopProperties.m_cLoopCopyTarget.loop;
        } 
    }

    /**
    * FUNCTION NAME: ModifyVolumeSetting
    * DESCRIPTION  : Modify the volume setting of an audio source if requested.
    * INPUTS       : _source - Audio source to potentially modify.
    * OUTPUTS      : None
    **/
    void ModifyVolumeSetting(AudioSource _source)
    {
        if (m_VolumeProperties.m_eVolumeModifyMode == LPK_NumericModifyMode.SET)
            _source.volume = m_VolumeProperties.m_flVolume;
        else if (m_VolumeProperties.m_eVolumeModifyMode == LPK_NumericModifyMode.ADD)
            _source.volume += m_VolumeProperties.m_flVolume;
        else if (m_VolumeProperties.m_eVolumeModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _source.volume -= m_VolumeProperties.m_flVolume;
        else if (m_VolumeProperties.m_eVolumeModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _source.volume *= m_VolumeProperties.m_flVolume;
        else if (m_VolumeProperties.m_eVolumeModifyMode == LPK_NumericModifyMode.DIVIDE)
            _source.volume /= m_VolumeProperties.m_flVolume;
        else if (m_VolumeProperties.m_eVolumeModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_VolumeProperties.m_cVolumeCopyTarget != null)
                _source.volume = m_VolumeProperties.m_cVolumeCopyTarget.volume;
        }
    }

    /**
    * FUNCTION NAME: ModifyPitchSetting
    * DESCRIPTION  : Modify the volume setting of an audio source if requested.
    * INPUTS       : _source - Audio source to potentially modify.
    * OUTPUTS      : None
    **/
    void ModifyPitchSetting(AudioSource _source)
    {
        if (m_PitchProperties.m_ePitchSizeModifyMode == LPK_NumericModifyMode.SET)
            _source.pitch = m_PitchProperties.m_flPitch;
        else if (m_PitchProperties.m_ePitchSizeModifyMode == LPK_NumericModifyMode.ADD)
            _source.pitch += m_PitchProperties.m_flPitch;
        else if (m_PitchProperties.m_ePitchSizeModifyMode == LPK_NumericModifyMode.SUBTRACT)
            _source.pitch -= m_PitchProperties.m_flPitch;
        else if (m_PitchProperties.m_ePitchSizeModifyMode == LPK_NumericModifyMode.MULTIPLY)
            _source.pitch *= m_PitchProperties.m_flPitch;
        else if (m_PitchProperties.m_ePitchSizeModifyMode == LPK_NumericModifyMode.DIVIDE)
            _source.pitch /= m_PitchProperties.m_flPitch;
        else if (m_PitchProperties.m_ePitchSizeModifyMode == LPK_NumericModifyMode.COPY)
        {
            if (m_PitchProperties.m_cPitchCopyTarget != null)
                _source.pitch = m_PitchProperties.m_cPitchCopyTarget.pitch;
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
        if (m_EventTrigger != null)
            m_EventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_ModifySoundOnEvent))]
public class LPK_ModifySoundOnEventEditor : Editor
{
    SerializedProperty m_TargetAudioSources;

    SerializedProperty m_MuteProperties;
    SerializedProperty m_LoopProperties;
    SerializedProperty m_VolumeProperties;
    SerializedProperty m_PitchProperties;

    SerializedProperty m_EventTrigger;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_TargetAudioSources = serializedObject.FindProperty("m_TargetAudioSources");

        m_MuteProperties = serializedObject.FindProperty("m_MuteProperties");
        m_LoopProperties = serializedObject.FindProperty("m_LoopProperties");
        m_VolumeProperties = serializedObject.FindProperty("m_VolumeProperties");
        m_PitchProperties = serializedObject.FindProperty("m_PitchProperties");

        m_EventTrigger = serializedObject.FindProperty("m_EventTrigger");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_ModifySoundOnEvent owner = (LPK_ModifySoundOnEvent)target;

        LPK_ModifySoundOnEvent editorOwner = owner.GetComponent<LPK_ModifySoundOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifySoundOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifySoundOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifySoundOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(m_TargetAudioSources, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Modifications", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(m_MuteProperties, true);
        EditorGUILayout.PropertyField(m_LoopProperties, true);
        EditorGUILayout.PropertyField(m_VolumeProperties, true);
        EditorGUILayout.PropertyField(m_PitchProperties, true);

        //Events
        EditorGUILayout.PropertyField(m_EventTrigger, true);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifySoundOnEvent.MuteProperties))]
public class LPK_ModifySoundOnEvent_MutePropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eMuteModifyMode = property.FindPropertyRelative("m_eMuteModifyMode");
        SerializedProperty m_bMute = property.FindPropertyRelative("m_bMute");
        SerializedProperty m_cMuteCopyTarget = property.FindPropertyRelative("m_cMuteCopyTarget");
        SerializedProperty m_sMuteCopyTag = property.FindPropertyRelative("m_sMuteCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eMuteModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eMuteModifyMode.isExpanded, new GUIContent("Mute Properties"), true);

        if(m_eMuteModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eMuteModifyMode);

            if(m_eMuteModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_bMute);

            if(m_eMuteModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_cMuteCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sMuteCopyTag);
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
        SerializedProperty m_eMuteModifyMode = property.FindPropertyRelative("m_eMuteModifyMode");

        if(m_eMuteModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eMuteModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eMuteModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eMuteModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eMuteModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifySoundOnEvent.LoopProperties))]
public class LPK_ModifySoundOnEvent_LoopPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eLoopModifyMode = property.FindPropertyRelative("m_eLoopModifyMode");
        SerializedProperty m_bLoop = property.FindPropertyRelative("m_bLoop");
        SerializedProperty m_cLoopCopyTarget = property.FindPropertyRelative("m_cLoopCopyTarget");
        SerializedProperty m_sLoopCopyTag = property.FindPropertyRelative("m_sLoopCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eLoopModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eLoopModifyMode.isExpanded, new GUIContent("Loop Properties"), true);

        if(m_eLoopModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eLoopModifyMode);

            if(m_eLoopModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_bLoop);

            if(m_eLoopModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_cLoopCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sLoopCopyTag);
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
        SerializedProperty m_eLoopModifyMode = property.FindPropertyRelative("m_eLoopModifyMode");

        if(m_eLoopModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eLoopModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eLoopModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eLoopModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eLoopModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifySoundOnEvent.VolumeProperties))]
public class LPK_ModifySoundOnEvent_VolumePropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eVolumeModifyMode = property.FindPropertyRelative("m_eVolumeModifyMode");
        SerializedProperty m_flVolume = property.FindPropertyRelative("m_flVolume");
        SerializedProperty m_cVolumeCopyTarget = property.FindPropertyRelative("m_cVolumeCopyTarget");
        SerializedProperty m_sVolumeCopyTag = property.FindPropertyRelative("m_sVolumeCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eVolumeModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eVolumeModifyMode.isExpanded, new GUIContent("Volume Properties"), true);

        if(m_eVolumeModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), m_eVolumeModifyMode);

            if(m_eVolumeModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_cVolumeCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight), m_sVolumeCopyTag);
            }
            else if(m_eVolumeModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_flVolume);
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
        SerializedProperty m_eVolumeModifyMode = property.FindPropertyRelative("m_eVolumeModifyMode");

        if(m_eVolumeModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.COPY)
            return m_eVolumeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if (m_eVolumeModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.NONE)
            return m_eVolumeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eVolumeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifySoundOnEvent.PitchProperties))]
public class LPK_ModifySoundOnEvent_PitchPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_ePitchSizeModifyMode = property.FindPropertyRelative("m_ePitchSizeModifyMode");
        SerializedProperty m_flPitch = property.FindPropertyRelative("m_flPitch");
        SerializedProperty m_cPitchCopyTarget = property.FindPropertyRelative("m_cPitchCopyTarget");
        SerializedProperty m_sPitchCopyTag = property.FindPropertyRelative("m_sPitchCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_ePitchSizeModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_ePitchSizeModifyMode.isExpanded, new GUIContent("Pitch Properties"), true);

        if(m_ePitchSizeModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), m_ePitchSizeModifyMode);

            if(m_ePitchSizeModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_cPitchCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight), m_sPitchCopyTag);
            }
            else if(m_ePitchSizeModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), m_flPitch);
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
        SerializedProperty m_ePitchSizeModifyMode = property.FindPropertyRelative("m_ePitchSizeModifyMode");

        if(m_ePitchSizeModifyMode.enumValueIndex == (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.COPY)
            return m_ePitchSizeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if (m_ePitchSizeModifyMode.enumValueIndex != (int)LPK_ModifyTextOnEvent.LPK_NumericModifyMode.NONE)
            return m_ePitchSizeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_ePitchSizeModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

#endif  //UNITY_EDITOR

}   //LPK

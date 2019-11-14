/***************************************************
File:           LPK_GradientColorAnimator.cs
Authors:        Christopher Onorati
Last Updated:   11/8/2019
Last Version:   2019.1.5

Description:
  This component can be used to animate color based on a 
  given gradient.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using UnityEngine.UI;   /* Image */
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_GradientColorAnimator
* DESCRIPTION : Component used to animate the color of a sprite or text.
**/
public class LPK_GradientColorAnimator : LPK_Component
{
    /************************************************************************************/

    public enum LPK_GradientAnimationPlayMode
    {
        PlAY_ONCE,
        LOOP,
        PINGPONG,
    };

    /************************************************************************************/

    [Tooltip("The gradient colors will be sampled from.")]
    [Rename("Gradient")]
    public Gradient m_Gradient;

    [System.Serializable]
    public class RendererProperties
    {
        [Tooltip("SpriteRenderer to modify the color of.  If left null, and this object has a SpriteRenderer component, assume self.")]
        [Rename("Modify Sprite Renderer")]
        public SpriteRenderer m_cRenderer;

        [Tooltip("TextMesh to modify the color of.  If left null, and this object has a TextMesh component, assume self.")]
        [Rename("Modify TextMesh")]
        public TextMesh m_cTextMesh;

        [Tooltip("UI Image to modify the color of.  If left null, and this object has an Image component, assume self.")]
        [Rename("Modify Image")]
        public Image m_cImage;

        [Tooltip("UI text to modify the color of.  If left null, and this object has a text component, assume self.")]
        [Rename("Modify Text")]
        public Text m_cText;
    }

    public RendererProperties m_RendererProperties;

    [Tooltip("What animation mode to use.")]
    [Rename("Mode")]
    public LPK_GradientAnimationPlayMode m_eMode;

    [Tooltip("If set, even if another event is received, the gradient will continue on as if nothing happened.")]
    [Rename("Never Restart")]
    public bool m_bNeverRestart = false;

    [Tooltip("How long is one animation cycle (in seconds).")]
    [Rename("Duration")]
    public float m_flDuration = 2.0f;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component to be active.")]
    public LPK_EventObject m_EventTrigger;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the gradeint color animator finishes its animation.")]
    public LPK_EventSendingInfo m_GradientColorAnimatorFinishedEvent;

    /************************************************************************************/

    //Internal active state checking.
    bool m_bActive = false;

    //Internal Timer
    float m_flTimer = 0.0f;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Begins intiial delay before animating.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger)
            m_EventTrigger.Register(this);

        if (!m_RendererProperties.m_cRenderer && GetComponent<SpriteRenderer>())
            m_RendererProperties.m_cRenderer = GetComponent<SpriteRenderer>();

        if (!m_RendererProperties.m_cTextMesh && GetComponent<TextMesh>())
            m_RendererProperties.m_cTextMesh = GetComponent<TextMesh>();

        if (!m_RendererProperties.m_cImage && GetComponent<Image>())
            m_RendererProperties.m_cImage = GetComponent<Image>();

        if(!m_RendererProperties.m_cText && GetComponent<Text>())
            m_RendererProperties.m_cText = GetComponent<Text>();
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

        if(m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        ActivateGradient();
    }

    /**
    * FUNCTION NAME: ActivateGradient
    * DESCRIPTION  : Sets the gradient animation as active.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void ActivateGradient()
    {
        if(!m_bNeverRestart)
            m_flTimer = 0.0f;

        m_bActive = true;

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Color Gradient Active");
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manages color animation.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
      if (!m_bActive)
        return;

      if (m_Gradient == null)
          return;
    
        //Increment timer
        m_flTimer += Time.deltaTime;
    
        //Reset Timer according to animation type
        if(m_flTimer >= m_flDuration)
        {
            if (m_eMode == LPK_GradientAnimationPlayMode.LOOP)
                m_flTimer = 0;
            else if (m_eMode == LPK_GradientAnimationPlayMode.PlAY_ONCE)
            { 
                m_flTimer = m_flDuration;

                //NOTENOTE:  Generally if we are a play once animation, we want to allow more color changes after the animation has stopped.
                m_bActive = false;
            }
            else if (m_eMode == LPK_GradientAnimationPlayMode.PINGPONG)
                m_flTimer = -m_flDuration;

            DispatchGradientFinishedEvent();
        }

        //Set the color
        if (m_RendererProperties.m_cRenderer != null)
            m_RendererProperties.m_cRenderer.color = m_Gradient.Evaluate(Mathf.Abs(m_flTimer) / m_flDuration);
        else if (m_RendererProperties.m_cTextMesh != null)
            m_RendererProperties.m_cTextMesh.color = m_Gradient.Evaluate(Mathf.Abs(m_flTimer) / m_flDuration);
        else if (m_RendererProperties.m_cImage != null)
            m_RendererProperties.m_cImage.color = m_Gradient.Evaluate(Mathf.Abs(m_flTimer / m_flDuration));
        else if (m_RendererProperties.m_cText != null)
            m_RendererProperties.m_cText.color = m_Gradient.Evaluate(Mathf.Abs(m_flTimer / m_flDuration));
    }

    /**
    * FUNCTION NAME: DispatchGradientFinishedEvent
    * DESCRIPTION  : Send out event when a gradient finishes an animation cycle.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchGradientFinishedEvent()
    {
        if(m_GradientColorAnimatorFinishedEvent != null && m_GradientColorAnimatorFinishedEvent.m_Event != null)
        {
            if(m_GradientColorAnimatorFinishedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_GradientColorAnimatorFinishedEvent.m_Event.Dispatch(null);
            else if(m_GradientColorAnimatorFinishedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_GradientColorAnimatorFinishedEvent.m_Event.Dispatch(gameObject);
            else if (m_GradientColorAnimatorFinishedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_GradientColorAnimatorFinishedEvent.m_Event.Dispatch(gameObject, m_GradientColorAnimatorFinishedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_GradientColorAnimatorFinishedEvent, this, "Gradient Color Animation Finished");
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

[CustomEditor(typeof(LPK_GradientColorAnimator))]
public class LPK_GradientColorAnimatorEditor : Editor
{
    SerializedProperty gradient;
    SerializedProperty renderProperties;
    SerializedProperty mode;

    SerializedProperty eventTriggers;

    SerializedProperty gradientFinishedReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        gradient = serializedObject.FindProperty("m_Gradient");
        renderProperties = serializedObject.FindProperty("m_RendererProperties");
        mode = serializedObject.FindProperty("m_eMode");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        gradientFinishedReceivers = serializedObject.FindProperty("m_GradientColorAnimatorFinishedEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_GradientColorAnimator owner = (LPK_GradientColorAnimator)target;

        LPK_GradientColorAnimator editorOwner = owner.GetComponent<LPK_GradientColorAnimator>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_GradientColorAnimator)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_GradientColorAnimator), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_GradientColorAnimator");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(gradient, true);
        owner.m_flDuration = EditorGUILayout.FloatField(new GUIContent("Duration", "How long is one animation cycle (in seconds)."), owner.m_flDuration);
        EditorGUILayout.PropertyField(mode, true);
        owner.m_bNeverRestart = EditorGUILayout.Toggle(new GUIContent("Never Restart", "If checked the gradient animation will not restart if it receives an event during its animation."), owner.m_bNeverRestart);

        //Renderers.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Renderers", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(renderProperties, true);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);
        EditorGUILayout.PropertyField(gradientFinishedReceivers, true);

        //Debug properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Properties", EditorStyles.boldLabel);

        owner.m_bPrintDebug = EditorGUILayout.Toggle(new GUIContent("Print Debug Info", "Toggle console debug messages."), owner.m_bPrintDebug);
        owner.m_sLabel = EditorGUILayout.TextField(new GUIContent("Label", "Notes for the user about this component.  This does nothing to the game or build."), owner.m_sLabel);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNTIY_EDITOR

}   //LPK

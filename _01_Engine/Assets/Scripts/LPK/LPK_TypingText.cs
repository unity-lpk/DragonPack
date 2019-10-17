/***************************************************
File:           LPK_TypingText.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This component creates a typing text effect for an animated
  display.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; /* Text. */
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_TypingText
* DESCRIPTION : Replaces the standard static textfields with dynamic typing textfields.
**/
public class LPK_TypingText : LPK_Component
{
    /************************************************************************************/

    public enum LPK_TypeType
    {
        TYPE,
        CHARACTER_SCROLL,
    }

    /************************************************************************************/

    public string m_sText;

    [Tooltip("What type of animation to play for typing text out.")]
    [Rename("Type Mode")]
    public LPK_TypeType m_eTypeMode = LPK_TypeType.TYPE;

    public float m_flSpeed = 0.15f;
    public float m_flCommaPauseTime = 0.05f;
    public float m_flPunctuationPauseTime = 0.15f;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the text has a new character typed.")]
    public LPK_EventSendingInfo m_TypingTextUpdateEvent;

    [Tooltip("Event sent when the text is finished being typed out.")]
    public LPK_EventSendingInfo m_TypingTextCompletedEvent;

    /************************************************************************************/

    //Flag to delay typing between character.
    bool m_bActive = true;

    //Hold what we have already typed.
    string m_sPreviosulyTyped;
    //Array to hold the typed text in.
    List<string> m_aCharacters = new List<string>();
    //Used in the update loop
    int m_iCounter = 0;
    //Delay used for typing.
    float m_flDelay = 0;

    //Last attempted character to type.  Used for CHARACTER_SCROLL
    char m_sLastChar = '0';

    /************************************************************************************/

    TextMesh m_cTextMesh;
    Text m_cText;

    /************************************************************************************/

    //Used to prevent notification spam.
    float m_flLastNotificationTime;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Splits up text into an indivitualized array, and set up
    *                event detection if appropriate.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
       //NOTENOTE: This is just to shut up the warning that this isn't being used outside of the editor.
       m_flLastNotificationTime = 0;


        m_cTextMesh = GetComponent<TextMesh>();
        m_cText = GetComponent<Text>();

        for (int i = 0; i < m_sText.Length; i++)
            m_aCharacters.Add(System.Convert.ToString(m_sText[i]));
    }
    
    /**
    * FUNCTION NAME: OnDrawGizmosSelected
    * DESCRIPTION  : Set the default game object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDrawGizmosSelected()
    {
        float flNow = System.DateTime.Now.Hour * 60 * 60 + (System.DateTime.Now.Minute * 60) + System.DateTime.Now.Second;

        //Display warning every five seconds.
        if (GetComponent<TextMesh>() == null && GetComponent<Text>() == null && flNow > m_flLastNotificationTime + 5.0f)
        {   
#if UNITY_EDITOR

            //This technically doesn't wrap with days but...
            m_flLastNotificationTime = flNow;

            EditorWindow window = EditorWindow.GetWindow<SceneView>();
            window.ShowNotification(new GUIContent("LPK_TypingText on game object " + name + " requires a Text or TextMesh component to work."), 3.0f);
#endif  //UNITY_EDITOR
        }
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manages selection of the typing animation.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if (!m_bActive)
            return;

        if (m_eTypeMode == LPK_TypeType.TYPE)
        {
            if (m_cTextMesh != null)
                TypeText();
            else if (m_cText != null)
                TypeUIText();
        }
        else if (m_eTypeMode == LPK_TypeType.CHARACTER_SCROLL)
        {
            if (m_cTextMesh != null)
                CharacterScroll();
            else if (m_cText != null)
                UICharacterScroll();
        }
    }

    /**
    * FUNCTION NAME: TypeText
    * DESCRIPTION  : Plays typing animation for text display.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void TypeText()
    {
        m_flDelay = m_flSpeed;

        //Extra delay for a comma.
        if (m_aCharacters[m_iCounter] == "," || m_aCharacters[m_iCounter] == ";")
            m_flDelay += m_flCommaPauseTime;

        //Extra delay for end of sentence quotations.
        else if (m_aCharacters[m_iCounter] == "." || m_aCharacters[m_iCounter] == "?" || m_aCharacters[m_iCounter] == "!" || m_aCharacters[m_iCounter] == ":")
            m_flDelay += m_flPunctuationPauseTime;

        //Notifying the owner that a letter has been typed.  Could be used for audio, for example.
        DispatchTypingTextUpdateEvent();

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Typing effect update.");

        //Inputing the new text.
        m_cTextMesh.text = m_sPreviosulyTyped + m_aCharacters[m_iCounter];
        m_sPreviosulyTyped = m_cTextMesh.text;
        m_iCounter++;

        //Marks the text as finished.
        if (m_cTextMesh.text == m_sText)
            TypingFinished();

        //Typing the next letter.
        else
        {
            m_bActive = false;
            StartCoroutine(DelayType());
        }
    }

    /**
    * FUNCTION NAME: CharacterScroll
    * DESCRIPTION  : Plays typing animation for character scroll display.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CharacterScroll()
    {
        m_flDelay = m_flSpeed;

        //Extra delay for a comma.
        if (m_aCharacters[m_iCounter].ToCharArray()[0] >= ' ' && m_aCharacters[m_iCounter].ToCharArray()[0] <= '/')
            m_sLastChar = m_aCharacters[m_iCounter].ToCharArray()[0];

        //Inputing the new text.
        m_cTextMesh.text = m_sPreviosulyTyped + m_sLastChar;

        if (m_cTextMesh.text.Length > 0 && m_cTextMesh.text[m_cTextMesh.text.Length - 1] == m_aCharacters[m_iCounter].ToCharArray()[0])
        {
            //Notifying the owner that a letter has been typed.  Could be used for audio, for example.
            DispatchTypingTextUpdateEvent();

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Typing effect update.");

            m_iCounter++;
            m_sPreviosulyTyped = m_cTextMesh.text;
            m_sLastChar = '0';
        }

        //Marks the text as finished.
        if (m_cTextMesh.text == m_sText)
            TypingFinished();

        //Typing the next letter.
        else
        {
            m_bActive = false;
            m_sLastChar++;
            StartCoroutine(DelayType());
        }
    }

    /**
    * FUNCTION NAME: TypeUIText
    * DESCRIPTION  : Plays typing animation for UI text.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void TypeUIText()
    {
        m_flDelay = m_flSpeed;

        //Extra delay for a comma.
        if (m_aCharacters[m_iCounter] == "," || m_aCharacters[m_iCounter] == ";")
            m_flDelay += m_flCommaPauseTime;

        //Extra delay for end of sentence quotations.
        else if (m_aCharacters[m_iCounter] == "." || m_aCharacters[m_iCounter] == "?" || m_aCharacters[m_iCounter] == "!" || m_aCharacters[m_iCounter] == ":")
            m_flDelay += m_flPunctuationPauseTime;

        //Notifying the owner that a letter has been typed.  Could be used for audio, for example.
        DispatchTypingTextUpdateEvent();

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Typing effect update.");

        //Inputing the new text.
        m_cText.text = m_sPreviosulyTyped + m_aCharacters[m_iCounter];
        m_sPreviosulyTyped = m_cText.text;
        m_iCounter++;

        //Marks the text as finished.
        if (m_cText.text == m_sText)
            TypingFinished();

        //Typing the next letter.
        else
        {
            m_bActive = false;
            StartCoroutine(DelayType());
        }
    }

    /**
    * FUNCTION NAME: UICharacterScroll
    * DESCRIPTION  : Plays typing animation for UI character scroll display.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void UICharacterScroll()
    {
        m_flDelay = m_flSpeed;

        //Extra delay for a comma.
        if (m_aCharacters[m_iCounter].ToCharArray()[0] >= ' ' && m_aCharacters[m_iCounter].ToCharArray()[0] <= '/')
            m_sLastChar = m_aCharacters[m_iCounter].ToCharArray()[0];

        //Inputing the new text.
        m_cText.text = m_sPreviosulyTyped + m_sLastChar;

        if (m_cText.text.Length > 0 && m_cText.text[m_cText.text.Length - 1] == m_aCharacters[m_iCounter].ToCharArray()[0])
        {
            //Notifying the owner that a letter has been typed.  Could be used for audio, for example.
            DispatchTypingTextUpdateEvent();

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Typing effect update.");

            m_iCounter++;
            m_sPreviosulyTyped = m_cText.text;
            m_sLastChar = '0';
        }

        //Marks the text as finished.
        if (m_cText.text == m_sText)
            TypingFinished();

        //Typing the next letter.
        else
        {
            m_bActive = false;
            m_sLastChar++;
            StartCoroutine(DelayType());
        }
    }

    /**
    * FUNCTION NAME: TypingFinished
    * DESCRIPTION  : Disables effect and informs all receivers that this text is finished typing.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void TypingFinished()
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Typing effect finished.");

        DispatchTypingTextFinishedEvent();

        m_bActive = false;
    }

    /**
    * FUNCTION NAME: DispatchTypingTextFinishedEvent
    * DESCRIPTION  : Dispatches event when typing text updates its string.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchTypingTextFinishedEvent()
    {
        if(m_TypingTextCompletedEvent != null && m_TypingTextCompletedEvent.m_Event != null)
        {
            if(m_TypingTextCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_TypingTextCompletedEvent.m_Event.Dispatch(null);
            else if(m_TypingTextCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_TypingTextCompletedEvent.m_Event.Dispatch(gameObject);
            else if (m_TypingTextCompletedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_TypingTextCompletedEvent.m_Event.Dispatch(gameObject, m_TypingTextCompletedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_TypingTextCompletedEvent, this, "Typing Completed");
        }
    }

    /**
    * FUNCTION NAME: DispatchTypingTextUpdateEvent
    * DESCRIPTION  : Dispatches event when typing text finishes its string.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchTypingTextUpdateEvent()
    {
        if(m_TypingTextUpdateEvent != null && m_TypingTextUpdateEvent.m_Event != null)
        {
            if(m_TypingTextUpdateEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_TypingTextUpdateEvent.m_Event.Dispatch(null);
            else if(m_TypingTextUpdateEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_TypingTextUpdateEvent.m_Event.Dispatch(gameObject);
            else if (m_TypingTextUpdateEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_TypingTextUpdateEvent.m_Event.Dispatch(gameObject, m_TypingTextUpdateEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_TypingTextUpdateEvent, this, "Typing Updated");
        }
    }

    /**
    * FUNCTION NAME: DelayTimer
    * DESCRIPTION  : Forces delay between character typing.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator DelayType()
    {
        yield return new WaitForSeconds(m_flDelay);
        m_bActive = true;
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(LPK_TypingText))]
public class LPK_TypingTextEditor : Editor
{
    SerializedProperty typeMode;

    SerializedProperty typingUpdateReceivers;
    SerializedProperty typingCompeltedReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        typeMode = serializedObject.FindProperty("m_eTypeMode");

        typingUpdateReceivers = serializedObject.FindProperty("m_TypingTextUpdateEvent");
        typingCompeltedReceivers = serializedObject.FindProperty("m_TypingTextCompletedEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_TypingText owner = (LPK_TypingText)target;

        LPK_TypingText editorOwner = owner.GetComponent<LPK_TypingText>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_TypingText)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_TypingText), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_TypingText");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_sText = EditorGUILayout.TextField(new GUIContent("Text", "Text to type out."), owner.m_sText);
        
        //NOTENOTE:  Removed this due to being a bit wonky to work with at the moment.  Uncomment to use character scrolling mode, however.
        //EditorGUILayout.PropertyField(typeMode, true);
        owner.m_flSpeed = EditorGUILayout.FloatField(new GUIContent("Character Delay", "How long to wait between each concurent character being typed."), owner.m_flSpeed);
        owner.m_flCommaPauseTime = EditorGUILayout.FloatField(new GUIContent("Comma Pause Time", "How long to pause for commas."), owner.m_flCommaPauseTime);
        owner.m_flPunctuationPauseTime = EditorGUILayout.FloatField(new GUIContent("Punctuation Pause Time", "How long to pause for periods, question marks, colons, and exclamation points."), owner.m_flPunctuationPauseTime);

        //Events
        EditorGUILayout.PropertyField(typingUpdateReceivers, true);
        EditorGUILayout.PropertyField(typingCompeltedReceivers, true);

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

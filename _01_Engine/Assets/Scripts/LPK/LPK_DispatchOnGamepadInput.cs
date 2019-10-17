/***************************************************
File:           LPK_DispatchOnGamepadInput.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component can be added to any object to cause it to 
  dispatch a LPK_GamepadInput event upon a given button being
  pressed, released or held.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : GamepadInputStatus
* DESCRIPTION : Input storage for gamepads.
**/
public class GamepadInputStatus
{
    /************************************************************************************/

    public enum LPK_ReturnInput
    {
        NONE,   //Just used for setup.
        PRESSED,
        RELEASED,
        HELD,
    }

    /************************************************************************************/

    public readonly PlayerIndex m_iID;
    public GamePadState m_prevState;
    public GamePadState m_currentState;

    /************************************************************************************/

    //Input dictionary.
    Dictionary<LPK_DispatchOnGamepadInput.LPK_ControllerButtons, LPK_ReturnInput> m_dInputList
           = new Dictionary<LPK_DispatchOnGamepadInput.LPK_ControllerButtons, LPK_ReturnInput>();

    //Dead zone
    readonly float m_flDeadZone;


    /**
    * FUNCTION NAME: Constructor
    * DESCRIPTION  : Sets ID and creates intiail dictionary of inputs.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public GamepadInputStatus(PlayerIndex _ID, float _DeadZone)
    {
        m_iID = _ID;
        m_flDeadZone = _DeadZone;
        
        //Set up each value in the dictionary up to ANY.
        for(int i = 0; i < (int)LPK_DispatchOnGamepadInput.LPK_ControllerButtons.ANY; i++)
            CreateDictionary( (LPK_DispatchOnGamepadInput.LPK_ControllerButtons)i );
    }

    /**
    * FUNCTION NAME: CreateDictionary
    * DESCRIPTION  : Creates all entries into the dictionary of inputs.
    * INPUTS       : _input - Input to add to the dictionary lookup.
    * OUTPUTS      : None
    **/
    void CreateDictionary(LPK_DispatchOnGamepadInput.LPK_ControllerButtons _input)
    {
        m_dInputList.Add(_input, LPK_ReturnInput.NONE);
    }

    /**
    * FUNCTION NAME: UpdateDictionary
    * DESCRIPTION  : Update the input state of every item in the dictionary.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void UpdateDictionary()
    {
        //NOTENOTE: This would be cleaner to do in a loop.  For now, this works, but this should be revisited and refactored later,
        //          if only for readability sake.

        //Update the buttons.
        UpdateDictionaryButton(m_prevState.Buttons.A, m_currentState.Buttons.A, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.A);
        UpdateDictionaryButton(m_prevState.Buttons.B, m_currentState.Buttons.B, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.B);
        UpdateDictionaryButton(m_prevState.Buttons.X, m_currentState.Buttons.X, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.X);
        UpdateDictionaryButton(m_prevState.Buttons.Y, m_currentState.Buttons.Y, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.Y);
        UpdateDictionaryButton(m_prevState.Buttons.Start, m_currentState.Buttons.Start, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.START);
        UpdateDictionaryButton(m_prevState.Buttons.Back, m_currentState.Buttons.Back, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.BACK);
        UpdateDictionaryButton(m_prevState.Buttons.Guide, m_currentState.Buttons.Guide, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.GUIDE);
        UpdateDictionaryButton(m_prevState.Buttons.LeftShoulder, m_currentState.Buttons.LeftShoulder, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_SHOULDER);
        UpdateDictionaryButton(m_prevState.Buttons.RightShoulder, m_currentState.Buttons.RightShoulder, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_SHOULDER);
        UpdateDictionaryButton(m_prevState.Buttons.LeftStick, m_currentState.Buttons.LeftStick, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_STICK);
        UpdateDictionaryButton(m_prevState.Buttons.RightStick, m_currentState.Buttons.RightStick, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_STICK);

        //Update the DPADs.
        UpdateDictionaryButton(m_prevState.DPad.Up, m_currentState.DPad.Up, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.DPAD_UP);
        UpdateDictionaryButton(m_prevState.DPad.Down, m_currentState.DPad.Down, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.DPAD_DOWN);
        UpdateDictionaryButton(m_prevState.DPad.Left, m_currentState.DPad.Left, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.DPAD_LEFT);
        UpdateDictionaryButton(m_prevState.DPad.Right, m_currentState.DPad.Right, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.DPAD_RIGHT);

        //Trigggers
        UpdateDictionaryTrigger(m_prevState.Triggers.Left, m_currentState.Triggers.Left, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_TRIGGER);
        UpdateDictionaryTrigger(m_prevState.Triggers.Right, m_currentState.Triggers.Right, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_TRIGGER);

        //Update left joystick.
        UpdateDictionaryJoystick(m_prevState.ThumbSticks.Left.X, m_currentState.ThumbSticks.Left.X, m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_RIGHT);
        UpdateDictionaryJoystick(m_prevState.ThumbSticks.Left.Y, m_currentState.ThumbSticks.Left.Y, m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_UP);
        UpdateDictionaryJoystickReverse(m_prevState.ThumbSticks.Left.X, m_currentState.ThumbSticks.Left.X, -m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_LEFT);
        UpdateDictionaryJoystickReverse(m_prevState.ThumbSticks.Left.Y, m_currentState.ThumbSticks.Left.Y, -m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_DOWN);

        //Update the right joystick.
        UpdateDictionaryJoystick(m_prevState.ThumbSticks.Right.X, m_currentState.ThumbSticks.Right.X, m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_RIGHT);
        UpdateDictionaryJoystick(m_prevState.ThumbSticks.Right.Y, m_currentState.ThumbSticks.Right.Y, m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_UP);
        UpdateDictionaryJoystickReverse(m_prevState.ThumbSticks.Right.X, m_currentState.ThumbSticks.Right.X, -m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_LEFT);
        UpdateDictionaryJoystickReverse(m_prevState.ThumbSticks.Right.Y, m_currentState.ThumbSticks.Right.Y, -m_flDeadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_DOWN);
    }

    /**
    * FUNCTION NAME: UpdateDictionaryButton
    * DESCRIPTION  : Update the input state of buttons.
    * INPUTS       : _prevState    - Previous state of the button on frame - 1.
    *                _currentState - Current state of the button on frame.
    *                _matchButton  - Button input to update in the dictionary.
    * OUTPUTS      : None
    **/
    void UpdateDictionaryButton(ButtonState _prevState, ButtonState _currentState, LPK_DispatchOnGamepadInput.LPK_ControllerButtons _matchButton)
    {
        if (_prevState == ButtonState.Pressed && _currentState == ButtonState.Pressed)
            m_dInputList[_matchButton] = LPK_ReturnInput.HELD;
        else if (_prevState == ButtonState.Pressed && _currentState == ButtonState.Released)
            m_dInputList[_matchButton] = LPK_ReturnInput.RELEASED;
        else if (_prevState == ButtonState.Released && _currentState == ButtonState.Pressed)
            m_dInputList[_matchButton] = LPK_ReturnInput.PRESSED;
        else
            m_dInputList[_matchButton] = LPK_ReturnInput.NONE;
    }

    /**
    * FUNCTION NAME: UpdateDictionaryTrigger
    * DESCRIPTION  : Update the input state of triggers.
    * INPUTS       : _prevState    - Previous state of the trigger on frame - 1.
    *                _currentState - Current state of the trigger on frame.
    *                _matchButton  - Trigger input to update in the dictionary.
    * OUTPUTS      : None
    **/
    void UpdateDictionaryTrigger(float _prevState, float _currentState, LPK_DispatchOnGamepadInput.LPK_ControllerButtons _matchButton)
    {
        if (_prevState >= m_flDeadZone && _currentState >= m_flDeadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.HELD;
        else if (_prevState >= m_flDeadZone && _currentState < m_flDeadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.RELEASED;
        else if (_prevState < m_flDeadZone && _currentState >= m_flDeadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.PRESSED;
        else
            m_dInputList[_matchButton] = LPK_ReturnInput.NONE;
    }

    /**
    * FUNCTION NAME: UpdateDictionaryTrigger
    * DESCRIPTION  : Update the input state of joysticks in right and up.
    * INPUTS       : _prevState    - Previous state of the joystick on frame - 1.
    *                _currentState - Current state of the joystick on frame.
    *                _deadZone     - Deadzone of the joystick to ignore input.
    *                _matchButton  - Joystick input to update in the dictionary.
    * OUTPUTS      : None
    **/
    void UpdateDictionaryJoystick(float _prevState, float _currentState, float _deadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons _matchButton)
    {
        if (_prevState >= _deadZone && _currentState >= _deadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.HELD;
        else if (_prevState >= _deadZone && _currentState < _deadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.RELEASED;
        else if (_prevState < _deadZone && _currentState >= _deadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.PRESSED;
        else
            m_dInputList[_matchButton] = LPK_ReturnInput.NONE;
    }

    /**
    * FUNCTION NAME: UpdateDictionaryTrigger
    * DESCRIPTION  : Update the input state of joysticks in left and down.
    * INPUTS       : _prevState    - Previous state of the joystick on frame - 1.
    *                _currentState - Current state of the joystick on frame.
    *                _deadZone     - Deadzone of the joystick to ignore input.
    *                _matchButton  - Joystick input to update in the dictionary.
    * OUTPUTS      : None
    **/
    void UpdateDictionaryJoystickReverse(float _prevState, float _currentState, float _deadZone, LPK_DispatchOnGamepadInput.LPK_ControllerButtons _matchButton)
    {
        if (_prevState <= _deadZone && _currentState <= _deadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.HELD;
        else if (_prevState <= _deadZone && _currentState > _deadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.RELEASED;
        else if (_prevState > _deadZone && _currentState <= _deadZone)
            m_dInputList[_matchButton] = LPK_ReturnInput.PRESSED;
        else
            m_dInputList[_matchButton] = LPK_ReturnInput.NONE;
    }

    /**
    * FUNCTION NAME: GetDictioanryValue
    * DESCRIPTION  : Get the current press value stored in the input dictionary.
    * INPUTS       : _input - Button to check value for.
    *                _mode  - Mode of input to detect.
    * OUTPUTS      : bool - Input status to report back.
    **/
    public bool GetDictioanryValue(LPK_DispatchOnGamepadInput.LPK_ControllerButtons _input, LPK_ReturnInput _mode)
    {
        //Single button check.
        if (_input != LPK_DispatchOnGamepadInput.LPK_ControllerButtons.ANY)
        {
            if (m_dInputList[_input] == _mode)
                return true;
            else
                return false;
        }

        //Any button check.
        else
        {
            for (int i = 0; i < m_dInputList.Count; i++)
            {
                if (m_dInputList[(LPK_DispatchOnGamepadInput.LPK_ControllerButtons)i] == _mode)
                    return true;
            }
        }

        return false;
    }
}

/**
* CLASS NAME  : LPK_DispatchOnGamepadInput
* DESCRIPTION : Component to sent events on gamepad input.
**/
public class LPK_DispatchOnGamepadInput : LPK_Component
{
    /************************************************************************************/

    public enum LPK_InputMode
    {
        PRESSED,
        RELEASED,
        HELD,
    };

    public enum LPK_ControllerNumber
    {
        ONE,
        TWO,
        THREE,
        FOUR,
        ANY,
    };

    public enum LPK_ControllerButtons
    {
        A,
        B,
        X,
        Y,
        START,
        BACK,
        GUIDE,
        LEFT_SHOULDER,
        RIGHT_SHOULDER,
        LEFT_STICK,
        RIGHT_STICK,
        LEFT_JOYSTICK_UP,
        LEFT_JOYSTICK_DOWN,
        LEFT_JOYSTICK_LEFT,
        LEFT_JOYSTICK_RIGHT,
        RIGHT_JOYSTICK_UP,
        RIGHT_JOYSTICK_DOWN,
        RIGHT_JOYSTICK_LEFT,
        RIGHT_JOYSTICK_RIGHT,
        LEFT_TRIGGER,
        RIGHT_TRIGGER,
        DPAD_UP,
        DPAD_DOWN,
        DPAD_LEFT,
        DPAD_RIGHT,
        ANY,
    };

    /************************************************************************************/

    [Tooltip("Which gamepad input will trigger event sending.")]
    [Rename("Gamepad Number")]
    public LPK_ControllerNumber m_eControllerNumber;

    [Tooltip("Which button on the gamepad to detect input from.")]
    [Rename("Gamepad Button")]
    public LPK_ControllerButtons m_eInputButton;

    [Tooltip("What mode should cause the event dispatch.")]
    [Rename("Input Mode")]
    public LPK_InputMode m_eInputMode = LPK_InputMode.PRESSED;

    [Tooltip("How far the trigger/joystick must be pushed in to register detection.  0 is not at all (hence not allowed), 1 is all the way.")]
    [Range(0.01f, 1.0f)]
    public float m_TriggerDetectZone = 0.5f;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when a gamepad button gives input.")]
    public LPK_EventSendingInfo m_GamepadInputEvent;

    /************************************************************************************/

    //List of gamepads to listen to.
    List<GamepadInputStatus> m_pGamepadStatuses = new List<GamepadInputStatus>();

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up which gamepad to listen for input on.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if (m_eControllerNumber == LPK_ControllerNumber.ONE)
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.One, m_TriggerDetectZone));
        else if (m_eControllerNumber == LPK_ControllerNumber.TWO)
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.Two, m_TriggerDetectZone));
        else if (m_eControllerNumber == LPK_ControllerNumber.THREE)
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.Three, m_TriggerDetectZone));
        else if (m_eControllerNumber == LPK_ControllerNumber.FOUR)
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.Four, m_TriggerDetectZone));
        else
        {
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.One, m_TriggerDetectZone));
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.Two, m_TriggerDetectZone));
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.Three, m_TriggerDetectZone));
            m_pGamepadStatuses.Add(new GamepadInputStatus(PlayerIndex.Four, m_TriggerDetectZone));
        }
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manages input detection.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        //NOTENOTE: We still want to update input even if event detection and sending is not set to be active, otherwise we will get out of sync.
        SetPrevFrameInput();
        SetCurrentFrameInput();

        for (int i = 0; i < m_pGamepadStatuses.Count; i++)
            m_pGamepadStatuses[i].UpdateDictionary();

        //Check the input based on the mode.
        for (int i = 0; i < m_pGamepadStatuses.Count; i++)
        {
            if ((int)m_pGamepadStatuses[i].m_iID != (int)m_eControllerNumber && m_eControllerNumber != LPK_ControllerNumber.ANY)
                continue;

            //Held detected with proper button.
            if (m_pGamepadStatuses[i].GetDictioanryValue(m_eInputButton, GamepadInputStatus.LPK_ReturnInput.HELD) && m_eInputMode == LPK_InputMode.HELD)
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Gamepad button HELD ");

                DispatchEvent();
            }
            //Pressed detected with proper button.
            else if (m_pGamepadStatuses[i].GetDictioanryValue(m_eInputButton, GamepadInputStatus.LPK_ReturnInput.PRESSED) && m_eInputMode == LPK_InputMode.PRESSED)
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Gamepad button PRESSED ");

                DispatchEvent();
            }
            //Released detected with propper button.
            else if (m_pGamepadStatuses[i].GetDictioanryValue(m_eInputButton, GamepadInputStatus.LPK_ReturnInput.RELEASED) && m_eInputMode == LPK_InputMode.RELEASED)
            {
                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Gamepad button RELEASED ");

                DispatchEvent();
            }
        }
    }

    /**
    * FUNCTION NAME: SetPrevFrameInput
    * DESCRIPTION  : Set the previous frame inputs.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetPrevFrameInput()
    {
        for (int i = 0; i < m_pGamepadStatuses.Count; i++)
            m_pGamepadStatuses[i].m_prevState = m_pGamepadStatuses[i].m_currentState;
    }

    /**
    * FUNCTION NAME: SetCurrentFrameInput
    * DESCRIPTION  : Set the current states of the gamepad.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SetCurrentFrameInput()
    {
        for(int i = 0; i < m_pGamepadStatuses.Count; i++)
        {
            PlayerIndex curIndex = m_pGamepadStatuses[i].m_iID;

            //Gamepad is not connected - set to a new state of everything defaulting.
            if (!GamePad.GetState(curIndex).IsConnected)
                m_pGamepadStatuses[i].m_currentState = new GamePadState();
            else
                m_pGamepadStatuses[i].m_currentState = GamePad.GetState(curIndex);
        }
    }

    /**
    * FUNCTION NAME: DispatchEvent
    * DESCRIPTION  : Dispatches the gamepad input event if conditions were met in CompareInputStates.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchEvent()
    {
        if(m_GamepadInputEvent != null && m_GamepadInputEvent.m_Event != null)
        {
            if(m_GamepadInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_GamepadInputEvent.m_Event.Dispatch(null);
            else if(m_GamepadInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_GamepadInputEvent.m_Event.Dispatch(gameObject);
            else if (m_GamepadInputEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_GamepadInputEvent.m_Event.Dispatch(gameObject, m_GamepadInputEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_GamepadInputEvent, this, "Gamepad Input");
        }    
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DispatchOnGamepadInput))]
public class LPK_DispatchOnGamepadInputEditor : Editor
{
    SerializedProperty controllerNumber;
    SerializedProperty inputButton;
    SerializedProperty inputMode;
    SerializedProperty triggerDetectZone;

    SerializedProperty gamepadInputReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        controllerNumber = serializedObject.FindProperty("m_eControllerNumber");
        inputButton = serializedObject.FindProperty("m_eInputButton");
        inputMode = serializedObject.FindProperty("m_eInputMode");
        triggerDetectZone = serializedObject.FindProperty("m_TriggerDetectZone");

        gamepadInputReceivers = serializedObject.FindProperty("m_GamepadInputEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DispatchOnGamepadInput owner = (LPK_DispatchOnGamepadInput)target;

        LPK_DispatchOnGamepadInput editorOwner = owner.GetComponent<LPK_DispatchOnGamepadInput>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DispatchOnGamepadInput)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DispatchOnGamepadInput), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DispatchOnGamepadInput");

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(controllerNumber, true);
        EditorGUILayout.PropertyField(inputButton, true);
        EditorGUILayout.PropertyField(inputMode, true);

        //TODO:  Make this if check nicer one day.
        if(owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.ANY || owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_TRIGGER ||
           owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_TRIGGER || owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_TRIGGER || 
           owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_UP || owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_DOWN ||
           owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_LEFT || owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.LEFT_JOYSTICK_RIGHT ||
           owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_UP || owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_DOWN ||
           owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_LEFT || owner.m_eInputButton == LPK_DispatchOnGamepadInput.LPK_ControllerButtons.RIGHT_JOYSTICK_RIGHT )
        EditorGUILayout.PropertyField(triggerDetectZone, true);

        //Events
        EditorGUILayout.PropertyField(gamepadInputReceivers, true);

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

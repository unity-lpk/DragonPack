/***************************************************
File:           LPK_VibrateControllerOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4

Description:
  This script is used to manage controller vibration.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XInputDotNetPure;

namespace LPK
{

/**
* CLASS NAME  : GamepadShakeStatus
* DESCRIPTION : Shake storage for gamepads.
**/
public class GamepadShakeStatus
{
    /************************************************************************************/

    public readonly PlayerIndex m_iID;

    /**
    * FUNCTION NAME: Constructor
    * DESCRIPTION  : Sets ID.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public GamepadShakeStatus(PlayerIndex _ID)
    {
        m_iID = _ID;
    }

    /**
    * FUNCTION NAME: Rumble
    * DESCRIPTION  : Rumble the controller this class is in charge of tracking.
    * INPUTS       : intensity     - Intensity of rumble.
    *                intensityMods - Modifiers to the intensity of the shake.
    * OUTPUTS      : None
    **/
    public void Rumble(float intensity, Vector2 intensityMods)
    {
        GamePad.SetVibration(m_iID, intensity * intensityMods.x, intensity * intensityMods.y);
    }
}

/**
* CLASS NAME  : LPK_VibrateControllerOnEvent
* DESCRIPTION : Manages controller vibration.
**/
public class LPK_VibrateControllerOnEvent : LPK_Component
{
    /************************************************************************************/

    public enum LPK_ControllerNumber
    {
        ONE,
        TWO,
        THREE,
        FOUR,
        ALL,
    };

    public enum LPK_ShakeType
    {
        CONSTANT,
        FADE_IN,
        FADE_OUT,
    };

    /************************************************************************************/

    [Tooltip("Which gamepad to vibrate.")]
    [Rename("Gamepad Number")]
    public LPK_ControllerNumber m_eControllerNumber;

    [Tooltip("Which mode to use when shaking the gamepad.")]
    [Rename("Shake Type")]
    public LPK_ShakeType m_eShakeType;

    public Vector2 m_vecLeftRightMods = new Vector2(1, 1);

    public float m_flCoolDown;

    public float m_flVibrateDuration = 1.5f;
    public float m_flVibrateIntensity = 1.0f;

    //NOTENOTE:  Currently this really only supports single player since there can only be one source.  If you wanted this to be
    //           more compatable with multiplayer, each controller should hold a Source object that represents its point in space, like
    //           a player, for example.  A workaround would be to add this component 4 times and set each controller and source differently.
    [System.Serializable]
    public class PointVibrationProperties
    {
        [Tooltip("Source of the vibration.  If left null, assume it is this object.")]
        [Rename("Vibrate Source")]
        public GameObject m_pVibrateSource;

        [Tooltip("Object that represents the point of receiving vibration, such as the player character.")]
        [Rename("Vibrate Receiver")]
        public GameObject m_pVibrateReceiver;

        [Tooltip("Distance from which the vibration can be felt.  Vibration strength will scale dynamically with distance.")]
        [Rename("Distance")]
        public float m_flDistance = 10.0f;

        [Tooltip("Flag to make vibration happen indefinently.  Useful for things such as a laser beam's contact point, for example.")]
        [Rename("Continual")]
        public bool m_bContinual;
    }

    public PointVibrationProperties m_PointVibrationProperties;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when a game pad starts vibrating.")]
    public LPK_EventSendingInfo m_VibrationStartEvent;

    [Tooltip("Event sent when a game object stops vibrating.")]
    public LPK_EventSendingInfo m_VibrationStopEvent;

    /************************************************************************************/

    //List of gamepads to shake.
    List<GamepadShakeStatus> m_pGamepads = new List<GamepadShakeStatus>();

    //Active state of the effect.
    bool m_bActive = false;

    //Current intenisty of the shake.
    float m_flCurIntensity;

    //Current duration of the shake.
    float m_flCurDuration;

    /************************************************************************************/

    //Emergency shutoff used to stop vibration on object destruction.
    const bool m_bShouldEmergencyShutoff = true;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for vibration starting.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        //Set a continual point shake in motion, if the user has specified.
        if (m_PointVibrationProperties.m_pVibrateReceiver != null && m_PointVibrationProperties.m_bContinual)
        {
            m_bActive = true;

            if (m_eShakeType != LPK_ShakeType.CONSTANT)
            {
                m_eShakeType = LPK_ShakeType.CONSTANT;

                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Changing shake type to be constant for a continual point shake.");
            }
        }

        if(m_EventTrigger)
            m_EventTrigger.Register(this);

        if (m_PointVibrationProperties.m_pVibrateSource == null)
            m_PointVibrationProperties.m_pVibrateSource = gameObject;

        //Set controller list.
        if (m_eControllerNumber == LPK_ControllerNumber.ONE)
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.One));
        else if (m_eControllerNumber == LPK_ControllerNumber.TWO)
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.Two));
        else if (m_eControllerNumber == LPK_ControllerNumber.THREE)
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.Three));
        else if (m_eControllerNumber == LPK_ControllerNumber.FOUR)
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.Four));
        else
        {
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.One));
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.Two));
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.Three));
            m_pGamepads.Add(new GamepadShakeStatus(PlayerIndex.Four));
        }
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manage whether or not to vibrate the controller.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if (m_bActive && m_flCurDuration >= m_flVibrateDuration && !m_PointVibrationProperties.m_bContinual)
            StopVibration();
        else if (m_bActive)
            Vibrate();
    }

    /**
    * FUNCTION NAME: Vibrate
    * DESCRIPTION  : Set vibration state across appropriate controllers.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Vibrate()
    {
        m_flCurDuration += Time.deltaTime;

        float distanceScalar = 1.0f;

        //Set distance scalar if user desires.
        if(m_PointVibrationProperties.m_pVibrateReceiver !=  null)
        {
            float distance = Vector3.Distance(m_PointVibrationProperties.m_pVibrateReceiver.transform.position,
                m_PointVibrationProperties.m_pVibrateSource.transform.position);

            distanceScalar = Mathf.Clamp( 1.0f - ( distance / m_PointVibrationProperties.m_flDistance ), 0.0f, 1.0f );
                
        }

        if (m_eShakeType == LPK_ShakeType.FADE_IN)
            m_flCurIntensity = (m_flCurDuration / m_flVibrateDuration) * m_flVibrateIntensity * distanceScalar;
        else if (m_eShakeType == LPK_ShakeType.FADE_OUT)
            m_flCurIntensity = ((m_flVibrateDuration - m_flCurDuration) / m_flVibrateDuration) * m_flVibrateIntensity * distanceScalar;
        else if (m_eShakeType == LPK_ShakeType.CONSTANT)
            m_flCurIntensity = m_flVibrateIntensity * distanceScalar;

        //Check vibration based on the controller number selected.
        for (int i = 0; i < m_pGamepads.Count; i++)
        {
            if ((int)m_pGamepads[i].m_iID != (int)m_eControllerNumber && m_eControllerNumber != LPK_ControllerNumber.ALL)
                continue;

            m_pGamepads[i].Rumble(m_flCurIntensity, m_vecLeftRightMods);
        }
    }

    /**
    * FUNCTION NAME: StopVibration
    * DESCRIPTION  : Stop vibration and send out appropriate event.  Also manages cooldown since a Corutine delay results
    *                in some unfortunate behavior.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void StopVibration()
    {
        //Stop vibration based on the controller number selected.
        for (int i = 0; i < m_pGamepads.Count; i++)
        {
            if ((int)m_pGamepads[i].m_iID != (int)m_eControllerNumber && m_eControllerNumber != LPK_ControllerNumber.ALL)
                continue;

            m_pGamepads[i].Rumble(0.0f, m_vecLeftRightMods);
        }

        m_flCurDuration += Time.deltaTime;

        //Cool down time is over.
        if (m_flCurDuration >= m_flCoolDown + m_flVibrateDuration)
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Stopping Controller Vibration");

            //Dispatch event.
            DispatchVibrationStopEvent();

            m_bActive = false;
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

        //Already active.  If the cool down is less than or equal to 0, then just go ahead and create a new shake.
        if (m_bActive && m_flCoolDown > 0 )
            return;

        m_bActive = true;
        m_flCurDuration = 0.0f;

        //Set initial intensity.
        if (m_eShakeType == LPK_ShakeType.CONSTANT || m_eShakeType == LPK_ShakeType.FADE_OUT)
            m_flCurIntensity = m_flVibrateIntensity;
        else
            m_flCurIntensity = 0.0f;

        //Dispatch event.
        DispatchVibrationStartEvent();

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Starting Controller Vibration");
    }

    /**
    * FUNCTION NAME: DispatchVibrationStartEvent
    * DESCRIPTION  : Dispatch event when controller starts vibrating.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchVibrationStartEvent()
    {
        if(m_VibrationStartEvent != null && m_VibrationStartEvent.m_Event != null)
        {
            if(m_VibrationStartEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_VibrationStartEvent.m_Event.Dispatch(null);
            else if(m_VibrationStartEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_VibrationStartEvent.m_Event.Dispatch(gameObject);
            else if (m_VibrationStartEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_VibrationStartEvent.m_Event.Dispatch(gameObject, m_VibrationStartEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_VibrationStartEvent, this, "Vibration Started");
        }  
    }

    /**
    * FUNCTION NAME: DispatchVibrationStopEvent
    * DESCRIPTION  : Dispatch event when controller stops vibrating.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchVibrationStopEvent()
    {
        if(m_VibrationStopEvent != null && m_VibrationStopEvent.m_Event != null)
        {
            if(m_VibrationStopEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_VibrationStopEvent.m_Event.Dispatch(null);
            else if(m_VibrationStopEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_VibrationStopEvent.m_Event.Dispatch(gameObject);
            else if (m_VibrationStopEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_VibrationStopEvent.m_Event.Dispatch(gameObject, m_VibrationStopEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebugDispatchingEvent(m_VibrationStartEvent, this, "Vibration Stopped");
        }  
    }

    /**
    * FUNCTION NAME: OnApplicationQuit
    * DESCRIPTION  : Stop controller vibration when game is terminated.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnApplicationQuit()
    {
        for (int i = 0; i < m_pGamepads.Count; i++)
            m_pGamepads[i].Rumble(0.0f, m_vecLeftRightMods);
    }

    /**
    * FUNCTION NAME: OnDestroy
    * DESCRIPTION  : Stop any rumble that may be happening if the object this component is on gets destroyed.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnDestroy()
    {
        if(m_bActive && m_bShouldEmergencyShutoff)
        {
            if(m_bPrintDebug)
                LPK_PrintWarning(this, "Emergency shutoff of vibration.");    

            for (int i = 0; i < m_pGamepads.Count; i++)
                m_pGamepads[i].Rumble(0.0f, m_vecLeftRightMods);  
        } 

        if(m_EventTrigger)
            m_EventTrigger.Unregister(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_VibrateControllerOnEvent))]
public class LPK_VibrateControllerOnEventEditor : Editor
{
    SerializedProperty controllerNumber;
    SerializedProperty shakeType;
    SerializedProperty pointVibrationProperties;

    SerializedProperty eventTriggers;

    SerializedProperty vibrationStartReceivers;
    SerializedProperty vibrationStopReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        controllerNumber = serializedObject.FindProperty("m_eControllerNumber");
        shakeType = serializedObject.FindProperty("m_eShakeType");
        pointVibrationProperties = serializedObject.FindProperty("m_PointVibrationProperties");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        vibrationStartReceivers = serializedObject.FindProperty("m_VibrationStartEvent");
        vibrationStopReceivers = serializedObject.FindProperty("m_VibrationStopEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_VibrateControllerOnEvent owner = (LPK_VibrateControllerOnEvent)target;

        LPK_VibrateControllerOnEvent editorOwner = owner.GetComponent<LPK_VibrateControllerOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_VibrateControllerOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_VibrateControllerOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_VibrateControllerOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(controllerNumber, true);
        EditorGUILayout.PropertyField(shakeType, true);
        owner.m_vecLeftRightMods = EditorGUILayout.Vector2Field(new GUIContent("Shake Modifiers", "Set the intensity of the shake on the left and right side of the controller.  Should be a value between 0 and 1."), owner.m_vecLeftRightMods);
        owner.m_flCoolDown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "How long to wait before allowing more vibrations to occur.  The delay starts AFTER a shake has finished."), owner.m_flCoolDown);
        owner.m_flVibrateDuration = EditorGUILayout.FloatField(new GUIContent("Vibration Duration", "Duration of the controller vibration."), owner.m_flVibrateDuration);
        owner.m_flVibrateIntensity = EditorGUILayout.FloatField(new GUIContent("Vibration Intensity", "Intensity of the controller vibration."), owner.m_flVibrateIntensity);
        EditorGUILayout.PropertyField(pointVibrationProperties, true);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);
        EditorGUILayout.PropertyField(vibrationStartReceivers, true);
        EditorGUILayout.PropertyField(vibrationStopReceivers, true);

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

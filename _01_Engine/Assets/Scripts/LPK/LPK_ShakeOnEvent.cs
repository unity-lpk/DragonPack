/***************************************************
File:           LPK_ShakeOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This component causes the specified object to shake when
  the specified event is received.

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
* CLASS NAME  : LPK_ShakeOnEvent
* DESCRIPTION : Component to enable shaking of objects.
**/
public class LPK_ShakeOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Game Object to shake when the specified event is received.")]
    [Rename("Target Shake Game Object")]
    public Transform m_pTargetShakeObject;

    public bool m_bResetTranslation = true;
    public bool m_bResetRotation = true;

    [Tooltip("How much intensity should be added per shake")]
    [Range(0, 1)]
    public float m_Intensity = 0.25f;

    [Tooltip("Set a curve for the shake.  High values are larger curves.")]
    [Range(1, 10)]
    public float m_IntensityExponent = 2.0f;
  
    public Vector3 m_vecTranslationalMagnitude = new Vector3(1, 1, 0);
    public Vector3 m_vecRotationalMagnitude = new Vector3(0, 0, 15);

    public float m_flDecayRate = 2.0f;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    // Every frame that this value is above 0, the shake target will shake. This value constantly
    // undergoes linear decay at a rate defined by DecayRate, above. This value is capped at 1.
    float m_flCurrentIntensity = 0.0f;

    bool m_bActive = false;

    Vector3 m_vecInitialPosition;
    Vector3 m_vecInitialAngles;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for object spawning.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        if(m_EventTrigger)
            m_EventTrigger.Register(this);
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

        m_bActive = true;

        // When the event comes through, if there's already some CurrentIntensity present, then the
        // object is already shaking, so there's no need to BeginShaking
        if (m_flCurrentIntensity <= 0)
            BeginShaking();

        // Add this component's defined Intensity onto the CurrentIntensity, but don't exceed 1
        m_flCurrentIntensity = Mathf.Clamp(m_flCurrentIntensity + m_Intensity, 0, 1);
    }

    /**
    * FUNCTION NAME: BeginShaking
    * DESCRIPTION  : Begins the shaking of the specified object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void BeginShaking()
    {
        m_vecInitialPosition = m_pTargetShakeObject.position;
        m_vecInitialAngles = m_pTargetShakeObject.eulerAngles;
        m_bActive = true;
    }

    /**
    * FUNCTION NAME: EndShaking
    * DESCRIPTION  : Ends the shaking of the specified object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void EndShaking()
    {
        if (m_bResetTranslation)
            m_pTargetShakeObject.position = m_vecInitialPosition;
        if (m_bResetRotation)
            m_pTargetShakeObject.eulerAngles = m_vecInitialAngles;

        m_bActive = false;
    }

    /**
    * FUNCTION NAME: FixedUpdate
    * DESCRIPTION  : Manages actual camera movement.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void FixedUpdate()
    {
        if (!m_bActive)
            return;

        if (m_bResetTranslation)
            m_pTargetShakeObject.position = m_vecInitialPosition;
        if(m_bResetRotation)
            m_pTargetShakeObject.eulerAngles = m_vecInitialAngles;

        // Shake intensity is more interesting when it's curved, so we raise the CurrentIntensity
        // to the power set by the user. A higher exponent makes for a sharper curve
        var perceivedIntensity = Mathf.Pow(m_flCurrentIntensity, m_IntensityExponent);

        // Determine the point in the ellipsoid (as described above)
        Vector3 pointOnEllipsoid = Vector3.Scale(Random.insideUnitSphere, m_vecTranslationalMagnitude);
        var r = Random.Range(0, perceivedIntensity);
        var pos = m_pTargetShakeObject.position + r * pointOnEllipsoid;

        // Determine the random angles (as described above)
        float xAngle = Random.Range(0, m_vecRotationalMagnitude.x);
        float yAngle = Random.Range(0, m_vecRotationalMagnitude.y);
        float zAngle = Random.Range(0, m_vecRotationalMagnitude.z);
        Vector3 angles =  new Vector3(xAngle, yAngle, zAngle) * m_flCurrentIntensity;

        Shake(pos, angles);

        // Reduce the CurrentIntensity by the user-defined DecayRate
        m_flCurrentIntensity -= m_flDecayRate * Time.deltaTime;
    
        // If the CurrentIntensity drops to zero (or past it), the shaking is complete
        if (m_flCurrentIntensity <= 0)
        {
            m_flCurrentIntensity = 0;
            EndShaking();
        }
    }

    /**
    * FUNCTION NAME: Shake
    * DESCRIPTION  : Set the angle and position of the object.
    * INPUTS       : _posOffset - Postion offset of the camera.
    *                _angles    - Angle offset of the cameera.
    * OUTPUTS      : None
    **/
    void Shake(Vector3 _posOffset, Vector3 _angles)
    {
        // The specified shake target is moved to the calculated offset and rotated
        // to the calculated Euler angles. All shakes are done in world space
        m_pTargetShakeObject.position = _posOffset;
        m_pTargetShakeObject.eulerAngles = _angles;
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

[CustomEditor(typeof(LPK_ShakeOnEvent))]
public class LPK_ShakeOnEventEditor : Editor
{
    SerializedProperty targetShakeObject;
    SerializedProperty intensity;
    SerializedProperty intensityExponent;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        targetShakeObject = serializedObject.FindProperty("m_pTargetShakeObject");
        intensity = serializedObject.FindProperty("m_Intensity");
        intensityExponent = serializedObject.FindProperty("m_IntensityExponent");

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
        LPK_ShakeOnEvent owner = (LPK_ShakeOnEvent)target;

        LPK_ShakeOnEvent editorOwner = owner.GetComponent<LPK_ShakeOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ShakeOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ShakeOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ShakeOnEvent");

        //Component Properties
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(targetShakeObject, true);
        owner.m_bResetTranslation = EditorGUILayout.Toggle(new GUIContent("Reset Translation", "Flag to check if the game object shaking will have its translation reset after the effect."), owner.m_bResetTranslation);
        owner.m_bResetRotation = EditorGUILayout.Toggle(new GUIContent("Reset Rotation", "Flag to check if the game object shaking will have its rotation reset after the effect."), owner.m_bResetRotation);
        EditorGUILayout.PropertyField(intensity, true);
        EditorGUILayout.PropertyField(intensityExponent, true);
        owner.m_vecTranslationalMagnitude = EditorGUILayout.Vector3Field(new GUIContent("Translational Magnitude", "The maximum distance that the object should displace itself at maximum intensity"), owner.m_vecTranslationalMagnitude);
        owner.m_vecRotationalMagnitude = EditorGUILayout.Vector3Field(new GUIContent("Rotational Magnitude", "The maximum Euler angles (in degrees) that the object should rotate itself at maximum intensity"), owner.m_vecRotationalMagnitude);
        owner.m_flDecayRate = EditorGUILayout.FloatField(new GUIContent("Decay Rate", "The rate(in intensity amount per second) at which the current shake intensity should decay."), owner.m_flDecayRate);

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

#endif  //UNITY_EDITOR

}   //LPK

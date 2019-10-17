/***************************************************
File:           LPK_SpawnOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   9/18/2019
Last Version:   2019.1.14

Description:
  This component can be attached to any object to cause
  it to spawn a game object upon receiving an event.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections;
using UnityEngine;
using UnityEditor;

namespace LPK
{

/**
* CLASS NAME  : LPK_SpawnOnEvent
* DESCRIPTION : Component to spawn a game object on events.
**/
public class LPK_SpawnOnEvent : LPK_Component
{
    /************************************************************************************/

    new public enum LPK_NonNumericModifyMode
    {
        SET,
        COPY,
    };

    /************************************************************************************/

    [Tooltip("What prefab to instantiate upon receiving the event.")]
    [Rename("Prefab")]
    public GameObject m_pPrefabToSpawn;

    [HideInInspector]
    [Tooltip("Time to delay spawn by in seconds.  Useful for respawning a dead character on the same frame it is destroyed or similar functionality.")]
    [Rename("Delay Time")]
    public float m_flDelayTime = 0.0f;

    public int m_iSpawnPerEventCount = 1;
    public int  m_iMaxTotalSpawnCount = 0;

    public float m_flCooldown = 0.0f;

    [Header("Spawn Position Properties")]

    [Tooltip("Mode for how to select the position to spawn a game object with.")]
    [Rename("Position Spawn Mode")]
    public LPK_NonNumericModifyMode m_ePositionSpawnMode;
  
    [Tooltip("Transform whose position will be the spawn location.")]
    [Rename("Target Position")]
    public Transform m_pTargetSpawnPosition;

    public Vector3 m_vecSpawnPosition;

    [Tooltip("Mode for how to select the rotation to spawn a game object with.")]
    [Rename("Rotation Spawn Mode")]
    public LPK_NonNumericModifyMode m_eRotationSpawnMode;
  
    [Tooltip("Transform whose angles will be used for the spawned game object.")]
    [Rename("Target Rotation")]
    public Transform m_pTargetSpawnRotation;

    public Vector3 m_vecSpawnRotation;

    public Vector3 m_vecRandomOffsetVariance;
    public Vector3 m_vecRandomAngleVariance;

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    [Header("Event Sending Info")]

    [Tooltip("Event sent when a game object is spawned.")]
    public LPK_EventSendingInfo m_GameObjectSpawnedEvent;

    /************************************************************************************/

    //Whether this component is waiting its cooldown
    protected bool m_bOnCooldown = false;
  
    /************************************************************************************/

    protected Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Sets up what event to listen to for object spawning.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = transform;

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

        if (m_bOnCooldown)
        {
            if (m_bPrintDebug)
                LPK_PrintDebug(this, "On Cooldown");

            return;
        }

        m_bOnCooldown = true;

        //HACKHACK: Fixes a bug where spawning and destroying an object with the same tag (for example destroing a player via tag player, and then spawning the player) on the same frame.
        //          causes both the dead player and the respawned player to be deleted.  This delays that respawned object from appearing for another frame.
        StartCoroutine(DelaySpawn());

        StartCoroutine(DelayCoolDown());
    }

    /**
    * FUNCTION NAME: DelaySpawn
    * DESCRIPTION  : Forces delay before spawning set object.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    protected IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(m_flDelayTime);
        SpawnGameObject();
    }

    /**
    * FUNCTION NAME: SpawnGameObject
    * DESCRIPTION  : Spawn the desired object.  Public so the Unity UI system can interact with this function.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public virtual void SpawnGameObject()
    {
        //Spawn the objects
        for (int i = 0; i < m_iSpawnPerEventCount; ++i)
        {
            float randX = Random.Range(-m_vecRandomOffsetVariance.x, m_vecRandomOffsetVariance.x);
            float randY = Random.Range(-m_vecRandomOffsetVariance.y, m_vecRandomOffsetVariance.y);
            float randZ = Random.Range(-m_vecRandomOffsetVariance.z, m_vecRandomOffsetVariance.z);

            Vector3 randAngles = new Vector3(Random.Range(-m_vecRandomAngleVariance.x, m_vecRandomAngleVariance.x), Random.Range(-m_vecRandomAngleVariance.y, m_vecRandomAngleVariance.y),
                                             Random.Range(-m_vecRandomAngleVariance.z, m_vecRandomAngleVariance.z));

            Vector3 spawnPosition = new Vector3();

            if (m_ePositionSpawnMode == LPK_NonNumericModifyMode.SET)
                spawnPosition = m_vecSpawnPosition;
            else if (m_pTargetSpawnPosition != null)
                spawnPosition = m_pTargetSpawnPosition.position;
            else if (m_bPrintDebug)
                LPK_PrintWarning(this, "No target set for spawn position.  Defaulting to 0, 0, 0.");

            GameObject obj = (GameObject)Instantiate(m_pPrefabToSpawn, spawnPosition + new Vector3(randX, randY, randZ), Quaternion.identity);
            Transform objTransform = obj.GetComponent<Transform>();

            if (m_eRotationSpawnMode == LPK_NonNumericModifyMode.SET)
                objTransform.eulerAngles = m_vecSpawnRotation + randAngles;
            else if (m_pTargetSpawnRotation != null)
                objTransform.eulerAngles = m_pTargetSpawnRotation.eulerAngles + randAngles;
            else if (m_bPrintDebug)
                LPK_PrintWarning(this, "No target set for spawn angles.  Defaulting to 0, 0, 0.");

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Game Object spawned");
        }

        //Dispatch spawn event
        DispatchSpawnEvent();
    }

    /**
    * FUNCTION NAME: DispatchSpawnEvent
    * DESCRIPTION  : Dispatch the spawn event.
    * INPUTS       : obj - Spawned object that may be added to receive the event.
    * OUTPUTS      : None
    **/
    protected void DispatchSpawnEvent()
    {
        if(m_GameObjectSpawnedEvent != null && m_GameObjectSpawnedEvent.m_Event != null)
        {
            if(m_GameObjectSpawnedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                m_GameObjectSpawnedEvent.m_Event.Dispatch(null);
            else if(m_GameObjectSpawnedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                m_GameObjectSpawnedEvent.m_Event.Dispatch(gameObject);
            else if (m_GameObjectSpawnedEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                m_GameObjectSpawnedEvent.m_Event.Dispatch(gameObject, m_GameObjectSpawnedEvent.m_Tags);

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Game Object Spawned event dispatched");
        }
    }

    /**
    * FUNCTION NAME: DelayCoolDown
    * DESCRIPTION  : Creates a delay between spawn waves.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    protected IEnumerator DelayCoolDown()
    {
        yield return new WaitForSeconds(m_flCooldown);
        m_bOnCooldown = false;
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

[CustomEditor(typeof(LPK_SpawnOnEvent))]
public class LPK_SpawnOnEventEditor : Editor
{
    SerializedProperty prefabToSpawn;

    SerializedProperty m_ePositionSpawnMode;
    SerializedProperty m_pTargetSpawnPosition;
    SerializedProperty m_eRotationSpawnMode;
    SerializedProperty m_pTargetSpawnRotation;

    SerializedProperty eventTriggers;

    SerializedProperty spawnObjectReceivers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        prefabToSpawn = serializedObject.FindProperty("m_pPrefabToSpawn");

        m_ePositionSpawnMode = serializedObject.FindProperty("m_ePositionSpawnMode");
        m_pTargetSpawnPosition = serializedObject.FindProperty("m_pTargetSpawnPosition");
        m_eRotationSpawnMode = serializedObject.FindProperty("m_eRotationSpawnMode");
        m_pTargetSpawnRotation = serializedObject.FindProperty("m_pTargetSpawnRotation");

        eventTriggers = serializedObject.FindProperty("m_EventTrigger");

        spawnObjectReceivers = serializedObject.FindProperty("m_GameObjectSpawnedEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_SpawnOnEvent owner = (LPK_SpawnOnEvent)target;

        LPK_SpawnOnEvent editorOwner = owner.GetComponent<LPK_SpawnOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_SpawnOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_SpawnOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_SpawnOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(prefabToSpawn, true);
        owner.m_iSpawnPerEventCount = EditorGUILayout.IntField(new GUIContent("Spawns Per Event", "How many instances of the archetype to spawn every time an event is received."), owner.m_iSpawnPerEventCount);
        owner.m_iMaxTotalSpawnCount = EditorGUILayout.IntField(new GUIContent("Max Spawns", "Total maximum number of instances this component is allowed to spawn. (0 means no limit)."), owner.m_iMaxTotalSpawnCount);
        owner.m_flCooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "Amount of time to wait (in seconds) until an event can trigger another spawn."), owner.m_flCooldown);
        
        //Spawn transform properties.
        EditorGUILayout.PropertyField(m_ePositionSpawnMode, true);

        if (m_ePositionSpawnMode.enumValueIndex == (int)LPK_SpawnOnEvent.LPK_NonNumericModifyMode.COPY)
            EditorGUILayout.PropertyField(m_pTargetSpawnPosition, true);
        else if (m_ePositionSpawnMode.enumValueIndex == (int)LPK_SpawnOnEvent.LPK_NonNumericModifyMode.SET)
            owner.m_vecSpawnPosition = EditorGUILayout.Vector3Field(new GUIContent("Spawn Position", "Position at which to spawn the game objects at."), owner.m_vecSpawnPosition);

        EditorGUILayout.PropertyField(m_eRotationSpawnMode, true);

        if (m_eRotationSpawnMode.enumValueIndex == (int)LPK_SpawnOnEvent.LPK_NonNumericModifyMode.COPY)
            EditorGUILayout.PropertyField(m_pTargetSpawnRotation, true);
        else if (m_eRotationSpawnMode.enumValueIndex == (int)LPK_SpawnOnEvent.LPK_NonNumericModifyMode.SET)
            owner.m_vecSpawnRotation = EditorGUILayout.Vector3Field(new GUIContent("Spawn Rotation", "Angle at which to spawn the game objects at."), owner.m_vecSpawnRotation);

        owner.m_vecRandomOffsetVariance = EditorGUILayout.Vector3Field(new GUIContent("Translation Offset Variance", "Random translational variance applied to the spawn position. A value of (2, 0, 0) will apply a random offset of -2 to 2 to the X value of the spawn position"), owner.m_vecRandomOffsetVariance);
        owner.m_vecRandomAngleVariance = EditorGUILayout.Vector3Field(new GUIContent("Rotational Offset Variance", "Random angular variance applied to the spawned object's angles."), owner.m_vecRandomAngleVariance);

        //Events
        EditorGUILayout.PropertyField(eventTriggers, true);
        EditorGUILayout.PropertyField(spawnObjectReceivers, true);

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

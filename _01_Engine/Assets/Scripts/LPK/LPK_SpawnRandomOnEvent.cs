/***************************************************
File:           LPK_SpawnRandomOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   9/18/2019
Last Version:   2019.1.14

Description:
  This component can be attached to any object to cause
  it to spawn a random game object from an array upon
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
* CLASS NAME  : LPK_SpawnRandomOnEvent
* DESCRIPTION : Component to spawn a random game object on events.
**/
public class LPK_SpawnRandomOnEvent : LPK_SpawnOnEvent
{
    /************************************************************************************/

    [Tooltip("Prefabs to possibly spawn upon receiving an event.  You can use the same prefab multiple times to make it more likely to be selected.")]
    public GameObject[] m_OptionsToSpawn;

    /**
    * FUNCTION NAME: SpawnGameObject
    * DESCRIPTION  : Spawn the desired object.  Public so the Unity UI system can interact with this function.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    override public void SpawnGameObject()
    {
        //Array length 0, no choices.
        if(m_OptionsToSpawn.Length < 1)
        {
            if(m_bPrintDebug)
                LPK_PrintDebug(this, "No objects set to be options for spawning.  Please set objects to choose from for spawning.");

            return;
        }

        //Spawn the objects
        for (int i = 0; i < m_iSpawnPerEventCount; ++i)
        {
            float randX = Random.Range(-m_vecRandomOffsetVariance.x, m_vecRandomOffsetVariance.x);
            float randY = Random.Range(-m_vecRandomOffsetVariance.y, m_vecRandomOffsetVariance.y);
            float randZ = Random.Range(-m_vecRandomOffsetVariance.z, m_vecRandomOffsetVariance.z);

            Vector3 randAngles = new Vector3(Random.Range(-m_vecRandomAngleVariance.x, m_vecRandomAngleVariance.x), Random.Range(-m_vecRandomAngleVariance.y, m_vecRandomAngleVariance.y),
                                             Random.Range(-m_vecRandomAngleVariance.z, m_vecRandomAngleVariance.z));

            GameObject prefabToSpawn = m_OptionsToSpawn[Random.Range(0, m_OptionsToSpawn.Length - 1)];

            //NOTENOTE:  If a null object is picked, do not count towards the spawn.  This also terminates the loop to avoid a case of infinite looping.
            if(prefabToSpawn == null)
            {
                if(m_bPrintDebug)
                    LPK_PrintError(this, "Selected null object to spawn.");

                break;
            }

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
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_SpawnRandomOnEvent))]
public class LPK_SpawnRandomOnEventEditor : Editor
{
    SerializedProperty optionsToSpawn;

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
        optionsToSpawn = serializedObject.FindProperty("m_OptionsToSpawn");

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
        LPK_SpawnRandomOnEvent owner = (LPK_SpawnRandomOnEvent)target;

        LPK_SpawnRandomOnEvent editorOwner = owner.GetComponent<LPK_SpawnRandomOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_SpawnRandomOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_SpawnRandomOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_SpawnRandomOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(optionsToSpawn, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);
        owner.m_iSpawnPerEventCount = EditorGUILayout.IntField(new GUIContent("Spawns Per Event", "How many instances of the archetype to spawn everytime an event is received."), owner.m_iSpawnPerEventCount);
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

/***************************************************
File:           LPK_DynamicPlatformerController.cs
Authors:        Christopher Onorati
Last Updated:   10/24/2019
Last Version:   2019.1.14

Description:
  This component allows for using the keyboard to move a 
  character by applying velocity to its dynamic RigidBody

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
* CLASS NAME  : LPK_DynamicPlatformerController
* DESCRIPTION : Implementation of a basic platformer character.
**/
[RequireComponent(typeof(Rigidbody2D))]
public class LPK_DynamicPlatformerController : LPK_Component
{
    /************************************************************************************/

    public enum PlatformerJumpInputType
    {
        PRESS,
        HOLD,
    };

    /************************************************************************************/

    public bool m_bCanMove = true;
    public bool m_bCanJump = true;
    public bool m_bCanMoveWhileJumping = true;
    public bool m_bCanMoveWhileAirJumping = true;

    public float m_flMoveSpeed = 2.0f;
    public Vector2 m_vecMaxSpeed = new Vector2(30, 30);

    [Tooltip("Deceleration to be applied every frame (values from 0 to 1).")]
    [Range(0, 1)]
    public float m_Deceleration = 0.2f;

    //Jump properties.

    [Tooltip("Determines how to handle jump input for the character.")]
    [Rename("Jump Input Mode")]
    public PlatformerJumpInputType m_eJumpInputType;

    public bool m_bAllowGraceJump = false;

    [Tooltip("Scalar to restrict movement while midair.  Will scale with the amount of air jumps made.")]
    [Rename("Jump Movement Deceleration")]
    public float m_flJumpMovementDeceleration = 1.0f;

    public bool m_bAirJumpDecreaseYVelocity = false;
    public bool m_bAirJumpDecreaseXVelocity = false;

    public float m_flJumpSpeed = 8.0f;

    public int m_iMaxAirJumps = 0;
    public float m_flMaxAirTime = 0.27f;

    [Tooltip("Mandatory collider to use for ground detection.  If this collider touches any non trigger, other than the character it belongs to, then the character is grounded.")]
    [Rename("Feet Collider")]
    public Collider2D m_pFeetCollider;

    //Input properties.

    public string m_sHorizontal = "Horizontal";
    public string m_sJumpButton = "Jump";

    [Header("Event Sending Info")]

    [Tooltip("Event sent when the character jumps.")]
    public LPK_EventSendingInfo m_CharacterJumpEvent;

    [Tooltip("Event sent when the character lands.")]
    public LPK_EventSendingInfo m_CharacterLandEvent;

    /************************************************************************************/
  
    //Number of mid-air jumps used
    int m_iAirJumpsUsed = 0;

    //Time the character has held the jump button down.  Used for HOLD.
    float m_flAirTime = 0.0f;

    //Flag to detect jumps status.
    bool m_bIsJumping = false;

    //Int counter used to not detect contactpoints on the feet when collision ends.
    int m_iDelayFrame = 2;

    //Flag to check if the character is grounded.
    bool m_bGrounded = true;

    /************************************************************************************/

    [HideInInspector]
    public bool m_bNoclipping = false;

    [HideInInspector]
    public float m_flNoclipRigidBodyGravityScale;

    [HideInInspector]
    public bool m_bNoclipWasTrigger;

    /************************************************************************************/

    Transform m_cTransform;
    Rigidbody2D m_cRigidBody;
    CapsuleCollider2D m_cPlayerCapsuleCollider;
    BoxCollider2D m_cPlayerBoxCollider;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Checks to ensure proper components are on the object for movement.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
        m_cRigidBody = GetComponent<Rigidbody2D>();

        m_cPlayerCapsuleCollider = GetComponent<CapsuleCollider2D>();
        m_cPlayerBoxCollider = GetComponent<BoxCollider2D>();

        if(m_cPlayerBoxCollider == null && m_cPlayerCapsuleCollider == null)
        {
            if(m_bPrintDebug)
                LPK_PrintWarning(this, "Player controller requires either a box or capsule collider for full functionality.");
        }
    }

    /**
    * FUNCTION NAME: OnUpdate
    * DESCRIPTION  : Manages player input.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Update Controller");

        /*----------MOVEMENT----------*/

        //Variable to determine the movement direction
        float moveDir = 0.0f;

        //If the left key is pressed, make the character face and move left
        if (!string.IsNullOrEmpty(m_sHorizontal) && Input.GetAxis(m_sHorizontal) < 0)
        {
            moveDir -= 1.0f;

            if (m_bAirJumpDecreaseXVelocity)
                moveDir /= Mathf.Max(1.0f, (m_iAirJumpsUsed * m_flJumpMovementDeceleration) + 1.0f);

            m_cTransform.eulerAngles = new Vector3(m_cTransform.eulerAngles.x, 180, m_cTransform.eulerAngles.z);

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move left.");
        }

        //If the right key is pressed, make the character face and move right
        if (!string.IsNullOrEmpty(m_sHorizontal) && Input.GetAxis(m_sHorizontal) > 0)
        {
            moveDir += 1.0f;

            if (m_bAirJumpDecreaseXVelocity)
                moveDir /= Mathf.Max(1.0f, (m_iAirJumpsUsed * m_flJumpMovementDeceleration) + 1.0f);

            m_cTransform.eulerAngles = new Vector3(m_cTransform.eulerAngles.x, 0, m_cTransform.eulerAngles.z);

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Move right.");
        }

        //Movement allowed check.
        if ((m_bCanMove && m_bCanMoveWhileJumping && m_iAirJumpsUsed <= 0) || (m_bCanMove && m_bCanMoveWhileJumping && m_bGrounded)
             || (m_bCanMove && m_bCanMoveWhileAirJumping && m_iAirJumpsUsed > 0) || (m_bCanMove && m_bGrounded))
        {
            //Take speed from previous frame and apply some deceleration
            float oldSpeed = m_cRigidBody.velocity.x * (1 - m_Deceleration);

            //Calculate new speed to be added this frame
            float newSpeed = moveDir * m_flMoveSpeed;

            //Set character's new velocity based on the given movemnt input (leave the Y component as is)
            m_cRigidBody.velocity = new Vector3(oldSpeed + newSpeed, m_cRigidBody.velocity.y, 0);
        }

        /*----------JUMPING----------*/

        m_iDelayFrame--;

        //If jump key is pressed
        if (m_eJumpInputType == PlatformerJumpInputType.PRESS && m_bCanJump)
            JumpInputPress();
        else if(m_bCanJump)
            JumpInputHold();
        

        /*----------SPEED CLAMP----------*/

        Vector2 velocityClamp = new Vector2(Mathf.Clamp(m_cRigidBody.velocity.x, -m_vecMaxSpeed.x, m_vecMaxSpeed.x),
                                  Mathf.Clamp(m_cRigidBody.velocity.y, -m_vecMaxSpeed.y, m_vecMaxSpeed.y));
        
        m_cRigidBody.velocity = velocityClamp;

        /*-----------GROUNDED------------*/
        if (m_pFeetCollider == null)
        {
            if (m_bPrintDebug)
                LPK_PrintError(this, "Cannot find feet object that has a 2D collider.");
        }
        else
            CheckGroundedFeet();
        
    }

    /**
    * FUNCTION NAME: JumpInputPress
    * DESCRIPTION  : Manages jump input if set to respond to PRESS.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void JumpInputPress()
    {
        if (!string.IsNullOrEmpty(m_sJumpButton) && Input.GetButtonDown(m_sJumpButton))
        {
            //If the character is grounded
            if (m_bGrounded)
            {
                //Apply upward velocity based on specified speed
                m_cRigidBody.velocity = new Vector3(m_cRigidBody.velocity.x, m_flJumpSpeed, 0);
                m_bGrounded = false;
                m_iDelayFrame = 2;

                m_bIsJumping = true;

                DispatchJumpEvent();

                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Jumping via PRESS input.");
            }

            //If character isnt grounded but air jump is enabled and available
            else if (m_iMaxAirJumps >= 1 && m_iAirJumpsUsed < m_iMaxAirJumps)
            {
                m_iAirJumpsUsed++;

                //Apply upward velocity based on specified speed
                if (m_bAirJumpDecreaseYVelocity && m_iMaxAirJumps > 0)
                    m_cRigidBody.velocity = new Vector3(m_cRigidBody.velocity.x, (m_flJumpSpeed) / m_iAirJumpsUsed, 0);
                else
                    m_cRigidBody.velocity = new Vector3(m_cRigidBody.velocity.x, m_flJumpSpeed, 0);

                m_bIsJumping = true;

                //Dispatch Jump
                DispatchJumpEvent();

                if (m_bPrintDebug)
                    LPK_PrintDebug(this, "Air jump via PRESS input.");
            }
        }
    }

    /**
    * FUNCTION NAME: JumpInputHold
    * DESCRIPTION  : Manages jump input if set to respond to HOLD.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void JumpInputHold()
    {
        //Resets data on initial button press.
        if(!string.IsNullOrEmpty(m_sJumpButton) && Input.GetButtonDown(m_sJumpButton))
        {
            if (m_iAirJumpsUsed > m_iMaxAirJumps || (!m_bGrounded && m_iMaxAirJumps < 1))
                return;

            if (!m_bGrounded)
                m_iAirJumpsUsed++;

            m_flAirTime = 0.0f;
            m_bIsJumping = true;

            //Dispatch event.
            DispatchJumpEvent();

            //Apply upward velocity based on specified speed
            if (m_bAirJumpDecreaseYVelocity && m_iMaxAirJumps > 0)
                m_cRigidBody.velocity = new Vector3(m_cRigidBody.velocity.x, (m_flJumpSpeed) / m_iAirJumpsUsed, 0);
            else
                m_cRigidBody.velocity = new Vector3(m_cRigidBody.velocity.x, m_flJumpSpeed, 0);

            if (m_bPrintDebug)
                LPK_PrintDebug(this, "Jumping via HOLD input.");
        }

        //Manages actual velocity change.
        if (!string.IsNullOrEmpty(m_sJumpButton) && Input.GetButton(m_sJumpButton) && m_flAirTime <= m_flMaxAirTime)
        {
            if (m_iAirJumpsUsed > m_iMaxAirJumps)
                return;

            m_cRigidBody.velocity = new Vector3(m_cRigidBody.velocity.x, m_flJumpSpeed, 0);

            m_flAirTime += Time.deltaTime;
        }
    }

    /**
    * FUNCTION NAME: CheckGroundedFeet
    * DESCRIPTION  : Checks grounded via feet child collision detection.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CheckGroundedFeet()
    {
        //Trigger check.
        if (m_pFeetCollider.isTrigger)
        {
            ContactFilter2D filter = new ContactFilter2D();
            Collider2D[] colliders = new Collider2D[16];

            if (m_pFeetCollider.OverlapCollider(filter, colliders) > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (colliders[i] != null && colliders[i].gameObject != gameObject && !colliders[i].isTrigger)
                    {
                        GroundCharacter();
                        return;
                    }
                }
            }

            return;
        }

        //NOTENOTE: 16 should be way more than enough.  Increase to detect more colliders but lower performance.
        ContactPoint2D[] hits = new ContactPoint2D[16];
        m_pFeetCollider.GetContacts(hits);

        //Go through each contact point to make sure that the character is **ACTUALLY** on the ground.
        foreach (var d in hits)
        {
            if (d.collider != null && d.collider.gameObject != gameObject)
            {
                GroundCharacter();
                return;
            }
        }
    }

    /**
    * FUNCTION NAME: OnCollisionExit2D
    * DESCRIPTION  : Allows a grace jump when falling off a ledge, if desired.
    * INPUTS       : col - Holds information on the collision event.
    * OUTPUTS      : None
    **/
    void OnCollisionExit2D(Collision2D col)
    {
        if (m_bAllowGraceJump)
            return;
        else
            m_bGrounded = false;
    }

    /**
    * FUNCTION NAME: GroundCharacter
    * DESCRIPTION  : Sets the character to be grounded, with an event dispatched.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    private void GroundCharacter()
    {
        DispatchLandEvent();

        //Set grounded flag and reset jumps
        m_bGrounded = true;
        m_bIsJumping = false;
        m_iAirJumpsUsed = 0;

        if (m_bPrintDebug)
            LPK_PrintDebug(this, "Character grounded.");
    }

    /**
    * FUNCTION NAME: DispatchLandEvent
    * DESCRIPTION  : Dispatches the land event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchLandEvent()
    {
        if(m_CharacterLandEvent != null)
        {
            if(m_CharacterLandEvent.m_Event != null)
            {
                if (m_CharacterLandEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                    m_CharacterLandEvent.m_Event.Dispatch(null);
                else if (m_CharacterLandEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                    m_CharacterLandEvent.m_Event.Dispatch(gameObject);
                else if (m_CharacterLandEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                    m_CharacterLandEvent.m_Event.Dispatch(gameObject, m_CharacterLandEvent.m_Tags);

                if (m_bPrintDebug)
                    LPK_PrintDebugDispatchingEvent(m_CharacterLandEvent, this, "Character Landing");
            }
        }
    }

    /**
    * FUNCTION NAME: DispatchJumpEvent
    * DESCRIPTION  : Dispatches the jump event.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DispatchJumpEvent()
    {
        if(m_CharacterJumpEvent != null)
        {
            if(m_CharacterJumpEvent.m_Event != null)
            {
                if(m_CharacterJumpEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.ALL)
                    m_CharacterJumpEvent.m_Event.Dispatch(null);
                else if(m_CharacterJumpEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.OWNER)
                    m_CharacterJumpEvent.m_Event.Dispatch(gameObject);
                else if (m_CharacterJumpEvent.m_EventSendingMode == LPK_EventSendingInfo.LPK_EventSendingMode.TAGS)
                    m_CharacterJumpEvent.m_Event.Dispatch(gameObject, m_CharacterJumpEvent.m_Tags);

                if (m_bPrintDebug)
                    LPK_PrintDebugDispatchingEvent(m_CharacterJumpEvent, this, "Character Jumping");
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DynamicPlatformerController))]
public class LPK_DynamicPlatformerControllerEditor : Editor
{
    SerializedProperty deceleration;
    SerializedProperty jumpInputType;
    SerializedProperty m_pFeetCollider;

    SerializedProperty characterJumpEvent;
    SerializedProperty characterLandEvent;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        deceleration = serializedObject.FindProperty("m_Deceleration");
        jumpInputType = serializedObject.FindProperty("m_eJumpInputType");
        m_pFeetCollider = serializedObject.FindProperty("m_pFeetCollider");

        characterJumpEvent = serializedObject.FindProperty("m_CharacterJumpEvent");
        characterLandEvent = serializedObject.FindProperty("m_CharacterLandEvent");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DynamicPlatformerController owner = (LPK_DynamicPlatformerController)target;

        LPK_DynamicPlatformerController editorOwner = owner.GetComponent<LPK_DynamicPlatformerController>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DynamicPlatformerController)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DynamicPlatformerController), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DynamicPlatformerController");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        owner.m_bCanMove = EditorGUILayout.Toggle(new GUIContent("Can Move", "Whether the character can move."), owner.m_bCanMove);
        owner.m_bCanJump = EditorGUILayout.Toggle(new GUIContent("Can Jump", "Whether the character can jump."), owner.m_bCanJump);
        owner.m_bCanMoveWhileJumping = EditorGUILayout.Toggle(new GUIContent("Can Move While Jumping", "Whether the character can jump and move at the same time."), owner.m_bCanMoveWhileJumping);
        owner.m_bCanMoveWhileAirJumping = EditorGUILayout.Toggle(new GUIContent("Can Move While Air Jumping", "Whether the character can jump while air jumping."), owner.m_bCanMoveWhileAirJumping);
        owner.m_flMoveSpeed = EditorGUILayout.FloatField(new GUIContent("Move Speed", "Velocity applied in the X axis when moving."), owner.m_flMoveSpeed);
        owner.m_vecMaxSpeed = EditorGUILayout.Vector2Field(new GUIContent("Max Speed", "Max velocity in the X and Y axis."), owner.m_vecMaxSpeed);
        EditorGUILayout.PropertyField(deceleration, true);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Jump Properties", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(jumpInputType, true);

        if(owner.m_eJumpInputType == LPK_DynamicPlatformerController.PlatformerJumpInputType.HOLD)
            owner.m_flMaxAirTime = EditorGUILayout.FloatField(new GUIContent("Jump Max Hold Time", "Total time for a press to be held to reach max height.  Only used with HOLD."), owner.m_flMaxAirTime);

        owner.m_bAllowGraceJump = EditorGUILayout.Toggle(new GUIContent("Allow Grace Jump", "Allow a single jump midair, even if midair jumps are allowed, if the player falls off a ledge.  If midair jumps are allowed, the first jump used when falling off a ledge will not count towards a midair jump."), owner.m_bAllowGraceJump);
        owner.m_flJumpMovementDeceleration = EditorGUILayout.FloatField(new GUIContent("Jump Movement Deceleration", "Scalar to restrict movement while midair.  Will scale with the amount of air jumps made."), owner.m_flJumpMovementDeceleration);
        owner.m_bAirJumpDecreaseXVelocity = EditorGUILayout.Toggle(new GUIContent("Reduce Air Jump X Velocity", "Decrease velocity of left and right movement on subsequent jumps."), owner.m_bAirJumpDecreaseXVelocity);
        owner.m_bAirJumpDecreaseYVelocity = EditorGUILayout.Toggle(new GUIContent("Reduce Air Jump Y Velocity", "Decrease velocity impact of subsequent jumps."), owner.m_bAirJumpDecreaseYVelocity);
        owner.m_flJumpSpeed = EditorGUILayout.FloatField(new GUIContent("Jump Force", "Velocity on the Y axis applied when jumping."), owner.m_flJumpSpeed);
        owner.m_iMaxAirJumps = EditorGUILayout.IntField(new GUIContent("Max Air Jumps", "Maximum number of allowed mid-air jumps.  Air jumps are reset when grounded."), owner.m_iMaxAirJumps);
        EditorGUILayout.PropertyField(m_pFeetCollider, true);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Input Properties", EditorStyles.miniBoldLabel);

        owner.m_sHorizontal = EditorGUILayout.TextField(new GUIContent("Horizontal Input", "Virtual button used for horizontal movement."), owner.m_sHorizontal);
        owner.m_sJumpButton = EditorGUILayout.TextField(new GUIContent("Jump Input", "Virtual button used for jumping."), owner.m_sJumpButton);

        //Events
        EditorGUILayout.PropertyField(characterJumpEvent, true);
        EditorGUILayout.PropertyField(characterLandEvent, true);

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
    
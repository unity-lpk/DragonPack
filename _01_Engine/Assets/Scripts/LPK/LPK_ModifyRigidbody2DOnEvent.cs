/***************************************************
File:           LPK_ModifyRigidbody2DOnEvent.cs
Authors:        Christopher Onorati
Last Updated:   10/3/2019
Last Version:   2019.1.4f1

Description:
    This script holds a component that allows modification
    to many of the properties on a Rigidbody2D.  This can be
    used to tweak those values at runtime through various different
    settings and configurations.

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
* CLASS NAME  : LPK_ModifyRigidbody2DOnEvent
* DESCRIPTION : Modify properties on Rigidbody2D components when events are received.
**/
public class LPK_ModifyRigidbody2DOnEvent : LPK_Component
{
    /************************************************************************************/

    [Tooltip("Game objects to destroy on receiving event.")]
    public GameObject[] m_ModifyGameObjects;

    /************************************************************************************/

    [System.Serializable]
    public class LinearVelocityProperies
    {
        [Tooltip("How to modify linear velocity upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NumericModifyMode m_eLinearVelocityModifyMode;

        [Tooltip("Value to use for linear velocity speed modification.")]
        [Rename("Linear Velocity Speed Value")]
        public Vector2 m_vecLinearVelocitySpeedValue;

        [Tooltip("Object whose linear velocity will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Linear Velocity Copy Target")]
        public GameObject m_pLinearVelocityCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Linear Velocity Copy Tag")]
        public string m_sLinearVelocityCopyTag;
    }

    [System.Serializable]
    public class AngularVelocityProperies
    {
        [Tooltip("How to modify angular velocity upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NumericModifyMode m_eAngularVelocityModifyMode;

        [Tooltip("Value to use for angular velocity modification.")]
        [Rename("Angular Velocity Value")]
        public float m_flAngularVelocityValue;

        [Tooltip("Object whose angular velocity will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Angular Velocity Copy Target")]
        public GameObject m_pAngularVelocityCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Angular Velocity Copy Tag")]
        public string m_sAngularVelocityCopyTag;
    }

    [System.Serializable]
    public class VelocityProperties
    {
        public LinearVelocityProperies m_LinearVelocityProperties;
        public AngularVelocityProperies m_AngularVelocityProperties;
    }

    public VelocityProperties m_VelocityProperties;

    /************************************************************************************/

    [System.Serializable]
    public class LinearDragProperies
    {
        [Tooltip("How to modify linear drag upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NumericModifyMode m_eLinearDragModifyMode;

        [Tooltip("Value to use for linear drag modification.")]
        [Rename("Linear Drag Value")]
        public float m_flLinearDragValue;

        [Tooltip("Object whose linear drag will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Linear Drag Copy Target")]
        public GameObject m_pLinearDragCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Linear Drag Copy Tag")]
        public string m_sLinearDragCopyTag;
    }

    [System.Serializable]
    public class AngularDragProperies
    {
        [Tooltip("How to modify angular drag upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NumericModifyMode m_eAngularDragModifyMode;

        [Tooltip("Value to use for angular drag modification.")]
        [Rename("Angular Drag Value")]
        public float m_flAngularDragValue;

        [Tooltip("Object whose angular drag will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Angular Drag Copy Target")]
        public GameObject m_pAngularDragCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Angular Drag Copy Tag")]
        public string m_sAngularDragCopyTag;
    }

    [System.Serializable]
    public class DragProperties
    {
        public LinearDragProperies m_LinearDragProperties;
        public AngularDragProperies m_AngularDragProperies;
    }

    public DragProperties m_DragProperties;

    /************************************************************************************/

    [System.Serializable]
    public class ConstraintsProperties
    {
        [Tooltip("How to modify constraints upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NonNumericModifyMode m_eConstraintsModifyMode;

        [Tooltip("Value to use for freezing X position modification.")]
        [Rename("Freeze X Position")]
        public bool m_bFreezePositionX;

        [Tooltip("Value to use for freezing Y position modification.")]
        [Rename("Freeze Y Position")]
        public bool m_bFreezePositionY;

        [Tooltip("Value to use for freezing Z rotation modification.")]
        [Rename("Freeze Z Rotation")]
        public bool m_bFreezeRotationZ;

        [Tooltip("Object whose constraints will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Constraints Copy Target")]
        public GameObject m_pConstraintsCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Constraints Copy Tag")]
        public string m_sConstraintsCopyTag;
    }

    public ConstraintsProperties m_ConstraintsProperties;

    /************************************************************************************/

    [System.Serializable]
    public class MassProperies
    {
        [Tooltip("How to modify mass upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NumericModifyMode m_eMassModifyMode;

        [Tooltip("Value to use for mass modification.")]
        [Rename("Mass Value")]
        public float m_flMassValue;

        [Tooltip("Object whose mass will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Mass Copy Target")]
        public GameObject m_pMassCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Mass Copy Tag")]
        public string m_sMassCopyTag;
    }

    [System.Serializable]
    public class GravityScaleProperties
    {
        [Tooltip("How to modify gravity scale upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NumericModifyMode m_eGravityScaleModifyMode;

        [Tooltip("Value to use for gravity scale modification.")]
        [Rename("Gravity Scale Value")]
        public float m_flGravityScale;

        [Tooltip("Object whose gravity scale will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Gravity Scale Copy Target")]
        public GameObject m_pGravityScaleCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Gravity Scale Copy Tag")]
        public string m_sGravityScaleCopyTag;
    }

    [System.Serializable]
    public class MaterialProperties
    {
        [Tooltip("How to modify the physics material upon receiving the event.")]
        [Rename("Modify Mode")]
        public LPK_NonNumericModifyMode m_eMaterialModifyMode;

        [Tooltip("Value to use for phyiscs material modification.")]
        [Rename("Physics Material")]
        public PhysicsMaterial2D m_Material;

        [Tooltip("Object whose physics material will be copied to the recipient's property value. Only used if mode is set to copy. Default to self if this and the tag field are left unset.")]
        [Rename("Physics Material Copy Target")]
        public GameObject m_pMaterialCopyTarget;

        [Tooltip("Object whose property value will be copied to the recipient's property value. Only used if mode is set to copy.  Note this will only affect the first object with the tag found.")]
        [Rename("Material Copy Tag")]
        public string m_sMaterialCopyTag;
    }

    [System.Serializable]
    public class MiscellaneousProperties
    {
        public MaterialProperties m_MaterialProperties;
        public GravityScaleProperties m_GravityScaleProperties;
        public MassProperies m_MassProperties;
    }

    public MiscellaneousProperties m_MiscellaneousProperties;

    /************************************************************************************/

    [Header("Event Receiving Info")]

    [Tooltip("Which event will trigger this component's action")]
    public LPK_EventObject m_EventTrigger;

    /************************************************************************************/

    //Used to assign the default game objet when the component is first added.
    bool m_bHasSetup = false;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Initalize event detection.
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
            m_ModifyGameObjects = new GameObject[] { gameObject };
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

        if(m_bPrintDebug)
            LPK_PrintDebugReceiveEvent(m_EventTrigger, this);

        SearchForModifyObjects();
    }

    /**
    * FUNCTION NAME: SearchForModifyObjects
    * DESCRIPTION  : Modifies appropriate objects.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void SearchForModifyObjects()
    {
        SetCopyTargets();    

        for (int i = 0; i < m_ModifyGameObjects.Length; i++)
        {
            if(m_ModifyGameObjects[i] != null && m_ModifyGameObjects[i].GetComponent<Rigidbody2D>())
                ModifyObject(m_ModifyGameObjects[i]);
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
        //Velocity
        if (m_VelocityProperties.m_LinearVelocityProperties.m_pLinearVelocityCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_VelocityProperties.m_LinearVelocityProperties.m_sLinearVelocityCopyTag))
                m_VelocityProperties.m_LinearVelocityProperties.m_pLinearVelocityCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_VelocityProperties.m_LinearVelocityProperties.m_sLinearVelocityCopyTag);
            else if (m_VelocityProperties.m_LinearVelocityProperties.m_pLinearVelocityCopyTarget == null)
                m_VelocityProperties.m_LinearVelocityProperties.m_pLinearVelocityCopyTarget = gameObject;
        }

        if (m_VelocityProperties.m_AngularVelocityProperties.m_pAngularVelocityCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_VelocityProperties.m_AngularVelocityProperties.m_sAngularVelocityCopyTag))
                m_VelocityProperties.m_AngularVelocityProperties.m_pAngularVelocityCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_VelocityProperties.m_AngularVelocityProperties.m_sAngularVelocityCopyTag);
            else if (m_VelocityProperties.m_AngularVelocityProperties.m_pAngularVelocityCopyTarget == null)
                m_VelocityProperties.m_AngularVelocityProperties.m_pAngularVelocityCopyTarget = gameObject;
        }

        //Drag
        if (m_DragProperties.m_LinearDragProperties.m_pLinearDragCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_DragProperties.m_LinearDragProperties.m_sLinearDragCopyTag))
                m_DragProperties.m_LinearDragProperties.m_pLinearDragCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_DragProperties.m_LinearDragProperties.m_sLinearDragCopyTag);
            else if (m_DragProperties.m_LinearDragProperties.m_pLinearDragCopyTarget == null)
                m_DragProperties.m_LinearDragProperties.m_pLinearDragCopyTarget = gameObject;
        }

        if (m_DragProperties.m_AngularDragProperies.m_pAngularDragCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_DragProperties.m_AngularDragProperies.m_sAngularDragCopyTag))
                m_DragProperties.m_AngularDragProperies.m_pAngularDragCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_DragProperties.m_AngularDragProperies.m_sAngularDragCopyTag);
            else if (m_DragProperties.m_AngularDragProperies.m_pAngularDragCopyTarget == null)
                m_DragProperties.m_AngularDragProperies.m_pAngularDragCopyTarget = gameObject;
        }

        //Constraints
        if (m_ConstraintsProperties.m_pConstraintsCopyTarget == null)
        {
            if (!string.IsNullOrEmpty(m_ConstraintsProperties.m_sConstraintsCopyTag))
                m_ConstraintsProperties.m_pConstraintsCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_ConstraintsProperties.m_sConstraintsCopyTag);
            else if (m_ConstraintsProperties.m_pConstraintsCopyTarget == null)
                m_ConstraintsProperties.m_pConstraintsCopyTarget = gameObject;
        }

        //Misc
        if (m_MiscellaneousProperties.m_MassProperties == null)
        {
            if (!string.IsNullOrEmpty(m_MiscellaneousProperties.m_MassProperties.m_sMassCopyTag))
               m_MiscellaneousProperties.m_MassProperties.m_pMassCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_MiscellaneousProperties.m_MassProperties.m_sMassCopyTag);
            else if (m_MiscellaneousProperties.m_MassProperties.m_pMassCopyTarget == null)
                m_MiscellaneousProperties.m_MassProperties.m_pMassCopyTarget = gameObject;
        }

        if (m_MiscellaneousProperties.m_GravityScaleProperties == null)
        {
            if (!string.IsNullOrEmpty(m_MiscellaneousProperties.m_GravityScaleProperties.m_sGravityScaleCopyTag))
                m_MiscellaneousProperties.m_GravityScaleProperties.m_pGravityScaleCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_MiscellaneousProperties.m_GravityScaleProperties.m_sGravityScaleCopyTag);
            else if (m_MiscellaneousProperties.m_GravityScaleProperties.m_pGravityScaleCopyTarget == null)
                m_MiscellaneousProperties.m_GravityScaleProperties.m_pGravityScaleCopyTarget = gameObject;
        }

        if (m_MiscellaneousProperties.m_MaterialProperties == null)
        {
            if (!string.IsNullOrEmpty(m_MiscellaneousProperties.m_MaterialProperties.m_sMaterialCopyTag))
                m_MiscellaneousProperties.m_MaterialProperties.m_pMaterialCopyTarget = LPK_MultiTagManager.FindGameObjectWithTag(gameObject, m_MiscellaneousProperties.m_MaterialProperties.m_sMaterialCopyTag);
            else if (m_MiscellaneousProperties.m_MaterialProperties.m_pMaterialCopyTarget == null)
                m_MiscellaneousProperties.m_MaterialProperties.m_pMaterialCopyTarget = gameObject;
        }
    }

    /**
    * FUNCTION NAME: ModifyObject
    * DESCRIPTION  : Modifies properties on Rigidbodies.
    * INPUTS       : _modifyObject - Game object to modify Rigidbody2D properties on.
    * OUTPUTS      : None
    **/
    void ModifyObject(GameObject _modifyObject)
    {
        if(_modifyObject == null || _modifyObject.GetComponent<Rigidbody2D>() == null)
            return;

        //Linear Drag.
        if(m_DragProperties.m_LinearDragProperties.m_eLinearDragModifyMode != LPK_NumericModifyMode.NONE)
        {
            if (m_DragProperties.m_LinearDragProperties.m_eLinearDragModifyMode == LPK_NumericModifyMode.SET)
                _modifyObject.GetComponent<Rigidbody2D>().drag = m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_LinearDragProperties.m_eLinearDragModifyMode == LPK_NumericModifyMode.ADD)
                _modifyObject.GetComponent<Rigidbody2D>().drag += m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_LinearDragProperties.m_eLinearDragModifyMode == LPK_NumericModifyMode.SUBTRACT)
                _modifyObject.GetComponent<Rigidbody2D>().drag -= m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_LinearDragProperties.m_eLinearDragModifyMode == LPK_NumericModifyMode.MULTIPLY)
                _modifyObject.GetComponent<Rigidbody2D>().drag *= m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_LinearDragProperties.m_eLinearDragModifyMode == LPK_NumericModifyMode.DIVIDE)
                _modifyObject.GetComponent<Rigidbody2D>().drag /= m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_LinearDragProperties.m_eLinearDragModifyMode == LPK_NumericModifyMode.COPY &&
                     m_DragProperties.m_LinearDragProperties.m_pLinearDragCopyTarget != null &&
                     m_DragProperties.m_LinearDragProperties.m_pLinearDragCopyTarget.GetComponent<Rigidbody2D>())
            { 
                _modifyObject.GetComponent<Rigidbody2D>().drag = m_DragProperties.m_LinearDragProperties.m_pLinearDragCopyTarget.GetComponent<Rigidbody2D>().drag;
            }
        }

        //Angular Drag.
        if(m_DragProperties.m_AngularDragProperies.m_eAngularDragModifyMode != LPK_NumericModifyMode.NONE)
        {
            if (m_DragProperties.m_AngularDragProperies.m_eAngularDragModifyMode == LPK_NumericModifyMode.SET)
                _modifyObject.GetComponent<Rigidbody2D>().angularDrag = m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_AngularDragProperies.m_eAngularDragModifyMode == LPK_NumericModifyMode.ADD)
                _modifyObject.GetComponent<Rigidbody2D>().angularDrag += m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_AngularDragProperies.m_eAngularDragModifyMode == LPK_NumericModifyMode.SUBTRACT)
                _modifyObject.GetComponent<Rigidbody2D>().angularDrag -= m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_AngularDragProperies.m_eAngularDragModifyMode == LPK_NumericModifyMode.MULTIPLY)
                _modifyObject.GetComponent<Rigidbody2D>().angularDrag *= m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_AngularDragProperies.m_eAngularDragModifyMode == LPK_NumericModifyMode.DIVIDE)
                _modifyObject.GetComponent<Rigidbody2D>().angularDrag /= m_DragProperties.m_LinearDragProperties.m_flLinearDragValue;
            else if (m_DragProperties.m_AngularDragProperies.m_eAngularDragModifyMode == LPK_NumericModifyMode.COPY &&
                     m_DragProperties.m_AngularDragProperies.m_pAngularDragCopyTarget != null &&
                     m_DragProperties.m_AngularDragProperies.m_pAngularDragCopyTarget.GetComponent<Rigidbody2D>())
            { 
                _modifyObject.GetComponent<Rigidbody2D>().angularDrag = m_DragProperties.m_AngularDragProperies.m_pAngularDragCopyTarget.GetComponent<Rigidbody2D>().angularDrag;
            }
        }

        //Linear velocity.
        if(m_VelocityProperties.m_LinearVelocityProperties.m_eLinearVelocityModifyMode != LPK_NumericModifyMode.NONE)
        {
            if (m_VelocityProperties.m_LinearVelocityProperties.m_eLinearVelocityModifyMode == LPK_NumericModifyMode.SET)
                _modifyObject.GetComponent<Rigidbody2D>().velocity = m_VelocityProperties.m_LinearVelocityProperties.m_vecLinearVelocitySpeedValue;
            else if (m_VelocityProperties.m_LinearVelocityProperties.m_eLinearVelocityModifyMode == LPK_NumericModifyMode.ADD)
                _modifyObject.GetComponent<Rigidbody2D>().velocity += m_VelocityProperties.m_LinearVelocityProperties.m_vecLinearVelocitySpeedValue;
            else if (m_VelocityProperties.m_LinearVelocityProperties.m_eLinearVelocityModifyMode == LPK_NumericModifyMode.SUBTRACT)
                _modifyObject.GetComponent<Rigidbody2D>().velocity -= m_VelocityProperties.m_LinearVelocityProperties.m_vecLinearVelocitySpeedValue;
            else if (m_VelocityProperties.m_LinearVelocityProperties.m_eLinearVelocityModifyMode == LPK_NumericModifyMode.MULTIPLY)
                _modifyObject.GetComponent<Rigidbody2D>().velocity *= m_VelocityProperties.m_LinearVelocityProperties.m_vecLinearVelocitySpeedValue;
            else if (m_VelocityProperties.m_LinearVelocityProperties.m_eLinearVelocityModifyMode == LPK_NumericModifyMode.DIVIDE)
                _modifyObject.GetComponent<Rigidbody2D>().velocity /= m_VelocityProperties.m_LinearVelocityProperties.m_vecLinearVelocitySpeedValue;
            else if (m_VelocityProperties.m_LinearVelocityProperties.m_eLinearVelocityModifyMode == LPK_NumericModifyMode.COPY &&
                     m_VelocityProperties.m_LinearVelocityProperties.m_pLinearVelocityCopyTarget != null &&
                     m_VelocityProperties.m_LinearVelocityProperties.m_pLinearVelocityCopyTarget.GetComponent<Rigidbody2D>())
            { 
                _modifyObject.GetComponent<Rigidbody2D>().velocity = m_VelocityProperties.m_LinearVelocityProperties.m_pLinearVelocityCopyTarget.GetComponent<Rigidbody2D>().velocity;
            }
        }

        //Angular velocity.
        if(m_VelocityProperties.m_AngularVelocityProperties.m_eAngularVelocityModifyMode != LPK_NumericModifyMode.NONE)
        {
            if (m_VelocityProperties.m_AngularVelocityProperties.m_eAngularVelocityModifyMode == LPK_NumericModifyMode.SET)
                _modifyObject.GetComponent<Rigidbody2D>().angularVelocity = m_VelocityProperties.m_AngularVelocityProperties.m_flAngularVelocityValue;
            else if (m_VelocityProperties.m_AngularVelocityProperties.m_eAngularVelocityModifyMode == LPK_NumericModifyMode.ADD)
                _modifyObject.GetComponent<Rigidbody2D>().angularVelocity += m_VelocityProperties.m_AngularVelocityProperties.m_flAngularVelocityValue;
            else if (m_VelocityProperties.m_AngularVelocityProperties.m_eAngularVelocityModifyMode == LPK_NumericModifyMode.SUBTRACT)
                _modifyObject.GetComponent<Rigidbody2D>().angularVelocity -= m_VelocityProperties.m_AngularVelocityProperties.m_flAngularVelocityValue;
            else if (m_VelocityProperties.m_AngularVelocityProperties.m_eAngularVelocityModifyMode == LPK_NumericModifyMode.MULTIPLY)
                _modifyObject.GetComponent<Rigidbody2D>().angularVelocity *= m_VelocityProperties.m_AngularVelocityProperties.m_flAngularVelocityValue;
            else if (m_VelocityProperties.m_AngularVelocityProperties.m_eAngularVelocityModifyMode == LPK_NumericModifyMode.DIVIDE)
                _modifyObject.GetComponent<Rigidbody2D>().angularVelocity /= m_VelocityProperties.m_AngularVelocityProperties.m_flAngularVelocityValue;
            else if (m_VelocityProperties.m_AngularVelocityProperties.m_eAngularVelocityModifyMode == LPK_NumericModifyMode.COPY &&
                     m_VelocityProperties.m_AngularVelocityProperties.m_pAngularVelocityCopyTarget != null &&
                     m_VelocityProperties.m_AngularVelocityProperties.m_pAngularVelocityCopyTarget.GetComponent<Rigidbody2D>())
            { 
                _modifyObject.GetComponent<Rigidbody2D>().angularVelocity = m_VelocityProperties.m_AngularVelocityProperties.m_pAngularVelocityCopyTarget.GetComponent<Rigidbody2D>().angularVelocity;
            }
        }

        //Constraints.
        if(m_ConstraintsProperties.m_eConstraintsModifyMode != LPK_NonNumericModifyMode.NONE)
        {
                if (m_ConstraintsProperties.m_eConstraintsModifyMode == LPK_NonNumericModifyMode.SET)
                {
                    RigidbodyConstraints2D constraints = RigidbodyConstraints2D.None;

                    if (m_ConstraintsProperties.m_bFreezePositionX && m_ConstraintsProperties.m_bFreezePositionY)
                        constraints = RigidbodyConstraints2D.FreezePosition;
                    else if (m_ConstraintsProperties.m_bFreezePositionX)
                        constraints = RigidbodyConstraints2D.FreezePositionX;
                    else if (m_ConstraintsProperties.m_bFreezePositionY)
                        constraints = RigidbodyConstraints2D.FreezePositionY;

                    _modifyObject.GetComponent<Rigidbody2D>().constraints = constraints;
                    _modifyObject.GetComponent<Rigidbody2D>().freezeRotation = m_ConstraintsProperties.m_bFreezeRotationZ;
                }
                else if (m_ConstraintsProperties.m_eConstraintsModifyMode == LPK_NonNumericModifyMode.COPY &&
                         m_ConstraintsProperties.m_pConstraintsCopyTarget != null && m_ConstraintsProperties.m_pConstraintsCopyTarget.GetComponent<Rigidbody2D>())
                { 
                    _modifyObject.GetComponent<Rigidbody2D>().constraints = m_ConstraintsProperties.m_pConstraintsCopyTarget.GetComponent<Rigidbody2D>().constraints;
                    _modifyObject.GetComponent<Rigidbody2D>().freezeRotation = m_ConstraintsProperties.m_pConstraintsCopyTarget.GetComponent<Rigidbody2D>().freezeRotation;
                }
        }

        //Mass
        if(m_MiscellaneousProperties.m_MassProperties.m_eMassModifyMode != LPK_NumericModifyMode.NONE)
        {
            if (m_MiscellaneousProperties.m_MassProperties.m_eMassModifyMode == LPK_NumericModifyMode.SET)
                _modifyObject.GetComponent<Rigidbody2D>().mass = m_MiscellaneousProperties.m_MassProperties.m_flMassValue;
            else if (m_MiscellaneousProperties.m_MassProperties.m_eMassModifyMode == LPK_NumericModifyMode.ADD)
                _modifyObject.GetComponent<Rigidbody2D>().mass += m_MiscellaneousProperties.m_MassProperties.m_flMassValue; 
            else if (m_MiscellaneousProperties.m_MassProperties.m_eMassModifyMode == LPK_NumericModifyMode.SUBTRACT)
                _modifyObject.GetComponent<Rigidbody2D>().mass -= m_MiscellaneousProperties.m_MassProperties.m_flMassValue; 
            else if (m_MiscellaneousProperties.m_MassProperties.m_eMassModifyMode == LPK_NumericModifyMode.MULTIPLY)
                _modifyObject.GetComponent<Rigidbody2D>().mass *= m_MiscellaneousProperties.m_MassProperties.m_flMassValue; 
            else if (m_MiscellaneousProperties.m_MassProperties.m_eMassModifyMode == LPK_NumericModifyMode.DIVIDE)
                _modifyObject.GetComponent<Rigidbody2D>().mass /= m_MiscellaneousProperties.m_MassProperties.m_flMassValue;
            else if (m_MiscellaneousProperties.m_MassProperties.m_eMassModifyMode == LPK_NumericModifyMode.COPY &&
                     m_MiscellaneousProperties.m_MassProperties.m_pMassCopyTarget != null &&
                     m_MiscellaneousProperties.m_MassProperties.m_pMassCopyTarget.GetComponent<Rigidbody2D>())
            { 
                _modifyObject.GetComponent<Rigidbody2D>().mass = m_MiscellaneousProperties.m_MassProperties.m_pMassCopyTarget.GetComponent<Rigidbody2D>().mass;
            }
        }

        //Gravity Scale
        if(m_MiscellaneousProperties.m_GravityScaleProperties.m_eGravityScaleModifyMode != LPK_NumericModifyMode.NONE)
        {
            if (m_MiscellaneousProperties.m_GravityScaleProperties.m_eGravityScaleModifyMode == LPK_NumericModifyMode.SET)
                _modifyObject.GetComponent<Rigidbody2D>().gravityScale = m_MiscellaneousProperties.m_GravityScaleProperties.m_flGravityScale;
            else if (m_MiscellaneousProperties.m_GravityScaleProperties.m_eGravityScaleModifyMode == LPK_NumericModifyMode.ADD)
                _modifyObject.GetComponent<Rigidbody2D>().gravityScale += m_MiscellaneousProperties.m_GravityScaleProperties.m_flGravityScale; 
            else if (m_MiscellaneousProperties.m_GravityScaleProperties.m_eGravityScaleModifyMode == LPK_NumericModifyMode.SUBTRACT)
                _modifyObject.GetComponent<Rigidbody2D>().gravityScale -= m_MiscellaneousProperties.m_GravityScaleProperties.m_flGravityScale;
            else if (m_MiscellaneousProperties.m_GravityScaleProperties.m_eGravityScaleModifyMode == LPK_NumericModifyMode.MULTIPLY)
                _modifyObject.GetComponent<Rigidbody2D>().gravityScale *= m_MiscellaneousProperties.m_GravityScaleProperties.m_flGravityScale; 
            else if (m_MiscellaneousProperties.m_GravityScaleProperties.m_eGravityScaleModifyMode == LPK_NumericModifyMode.DIVIDE)
                _modifyObject.GetComponent<Rigidbody2D>().gravityScale /= m_MiscellaneousProperties.m_GravityScaleProperties.m_flGravityScale;
            else if (m_MiscellaneousProperties.m_GravityScaleProperties.m_eGravityScaleModifyMode == LPK_NumericModifyMode.COPY &&
                     m_MiscellaneousProperties.m_GravityScaleProperties.m_pGravityScaleCopyTarget != null &&
                     m_MiscellaneousProperties.m_GravityScaleProperties.m_pGravityScaleCopyTarget.GetComponent<Rigidbody2D>())
            { 
                _modifyObject.GetComponent<Rigidbody2D>().gravityScale = m_MiscellaneousProperties.m_GravityScaleProperties.m_pGravityScaleCopyTarget.GetComponent<Rigidbody2D>().gravityScale;
            }
        }

        //Physics Material
        if(m_MiscellaneousProperties.m_MaterialProperties.m_eMaterialModifyMode != LPK_NonNumericModifyMode.NONE)
        {
            if (m_ConstraintsProperties.m_eConstraintsModifyMode == LPK_NonNumericModifyMode.SET)
                _modifyObject.GetComponent<Rigidbody2D>().sharedMaterial = m_MiscellaneousProperties.m_MaterialProperties.m_Material;
            else if (m_ConstraintsProperties.m_eConstraintsModifyMode == LPK_NonNumericModifyMode.COPY &&
                     m_ConstraintsProperties.m_pConstraintsCopyTarget != null && m_ConstraintsProperties.m_pConstraintsCopyTarget.GetComponent<Rigidbody2D>())
            { 
                _modifyObject.GetComponent<Rigidbody2D>().sharedMaterial = m_ConstraintsProperties.m_pConstraintsCopyTarget.GetComponent<Rigidbody2D>().sharedMaterial;
            }
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

[CustomEditor(typeof(LPK_ModifyRigidbody2DOnEvent))]
public class LPK_ModifyRigidbody2DOnEventEditor : Editor
{
    SerializedProperty modifyGameObjects;

    SerializedProperty dragProperties;
    SerializedProperty velocityProperties;
    SerializedProperty constraintsProperties;
    SerializedProperty miscellaneousProperties;

    SerializedProperty eventTriggers;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        modifyGameObjects = serializedObject.FindProperty("m_ModifyGameObjects");

        velocityProperties = serializedObject.FindProperty("m_VelocityProperties");
        dragProperties = serializedObject.FindProperty("m_DragProperties");
        constraintsProperties = serializedObject.FindProperty("m_ConstraintsProperties");
        miscellaneousProperties = serializedObject.FindProperty("m_MiscellaneousProperties");
  
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
        LPK_ModifyRigidbody2DOnEvent owner = (LPK_ModifyRigidbody2DOnEvent)target;

        LPK_ModifyRigidbody2DOnEvent editorOwner = owner.GetComponent<LPK_ModifyRigidbody2DOnEvent>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_ModifyRigidbody2DOnEvent)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_ModifyRigidbody2DOnEvent), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_ModifyRigidbody2DOnEvent");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        LPK_EditorArrayDraw.DrawArray(modifyGameObjects, LPK_EditorArrayDraw.LPK_EditorArrayDrawMode.DRAW_MODE_BUTTONS);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Modifications", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(miscellaneousProperties, true);
        EditorGUILayout.PropertyField(velocityProperties, true);
        EditorGUILayout.PropertyField(dragProperties, true);
        EditorGUILayout.PropertyField(constraintsProperties, true);

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

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.LinearVelocityProperies))]
public class LPK_ModifyRigidbody2DOnEvent_LinearVelocityProperiesDrawer : PropertyDrawer
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
        SerializedProperty m_eLinearVelocityModifyMode = property.FindPropertyRelative("m_eLinearVelocityModifyMode");
        SerializedProperty m_vecLinearVelocitySpeedValue = property.FindPropertyRelative("m_vecLinearVelocitySpeedValue");
        SerializedProperty m_pLinearVelocityCopyTarget = property.FindPropertyRelative("m_pLinearVelocityCopyTarget");
        SerializedProperty m_sLinearVelocityCopyTag = property.FindPropertyRelative("m_sLinearVelocityCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        m_eLinearVelocityModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eLinearVelocityModifyMode.isExpanded, new GUIContent("Linear Velocity Properties"), true);

        if(m_eLinearVelocityModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eLinearVelocityModifyMode);

            if(m_eLinearVelocityModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pLinearVelocityCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sLinearVelocityCopyTag);
            }

            else if(m_eLinearVelocityModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_vecLinearVelocitySpeedValue);
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
        SerializedProperty m_eLinearVelocityModifyMode = property.FindPropertyRelative("m_eLinearVelocityModifyMode");

        if (m_eLinearVelocityModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            return m_eLinearVelocityModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eLinearVelocityModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
            return m_eLinearVelocityModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eLinearVelocityModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.AngularVelocityProperies))]
public class LPK_ModifyRigidbody2DOnEvent_AngularVelocityProperiesDrawer : PropertyDrawer
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
        SerializedProperty m_eAngularVelocityModifyMode = property.FindPropertyRelative("m_eAngularVelocityModifyMode");
        SerializedProperty m_flAngularVelocityValue = property.FindPropertyRelative("m_flAngularVelocityValue");
        SerializedProperty m_pAngularVelocityCopyTarget = property.FindPropertyRelative("m_pAngularVelocityCopyTarget");
        SerializedProperty m_sAngularVelocityCopyTag = property.FindPropertyRelative("m_sAngularVelocityCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        m_eAngularVelocityModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eAngularVelocityModifyMode.isExpanded, new GUIContent("Angular Velocity Properties"), true);

        if(m_eAngularVelocityModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eAngularVelocityModifyMode);

            if(m_eAngularVelocityModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pAngularVelocityCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sAngularVelocityCopyTag);
            }

            else if(m_eAngularVelocityModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_flAngularVelocityValue);
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
        SerializedProperty m_eAngularVelocityModifyMode = property.FindPropertyRelative("m_eAngularVelocityModifyMode");

        if (m_eAngularVelocityModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            return m_eAngularVelocityModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eAngularVelocityModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
            return m_eAngularVelocityModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eAngularVelocityModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.LinearDragProperies))]
public class LPK_ModifyRigidbody2DOnEvent_LinearDragProperiesProperiesDrawer : PropertyDrawer
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
        SerializedProperty m_eLinearDragModifyMode = property.FindPropertyRelative("m_eLinearDragModifyMode");
        SerializedProperty m_flLinearDragValue = property.FindPropertyRelative("m_flLinearDragValue");
        SerializedProperty m_pLinearDragCopyTarget = property.FindPropertyRelative("m_pLinearDragCopyTarget");
        SerializedProperty m_sLinearDragCopyTag = property.FindPropertyRelative("m_sLinearDragCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        m_eLinearDragModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eLinearDragModifyMode.isExpanded, new GUIContent("Linear Drag Properties"), true);

        if(m_eLinearDragModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eLinearDragModifyMode);

            if(m_eLinearDragModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pLinearDragCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sLinearDragCopyTag);
            }

            else if(m_eLinearDragModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_flLinearDragValue);
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
        SerializedProperty m_eLinearDragModifyMode = property.FindPropertyRelative("m_eLinearDragModifyMode");

        if (m_eLinearDragModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            return m_eLinearDragModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eLinearDragModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
            return m_eLinearDragModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eLinearDragModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.AngularDragProperies))]
public class LPK_ModifyRigidbody2DOnEvent_AngularDragProperiesProperiesDrawer : PropertyDrawer
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
        SerializedProperty m_eAngularDragModifyMode = property.FindPropertyRelative("m_eAngularDragModifyMode");
        SerializedProperty m_flAngularDragValue = property.FindPropertyRelative("m_flAngularDragValue");
        SerializedProperty m_pAngularDragCopyTarget = property.FindPropertyRelative("m_pAngularDragCopyTarget");
        SerializedProperty m_sAngularDragCopyTag = property.FindPropertyRelative("m_sAngularDragCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        m_eAngularDragModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eAngularDragModifyMode.isExpanded, new GUIContent("Angular Drag Properties"), true);

        if(m_eAngularDragModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eAngularDragModifyMode);

            if(m_eAngularDragModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pAngularDragCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sAngularDragCopyTag);
            }

            else if(m_eAngularDragModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_flAngularDragValue);
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
        SerializedProperty m_eAngularDragModifyMode = property.FindPropertyRelative("m_eAngularDragModifyMode");

        if (m_eAngularDragModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            return m_eAngularDragModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eAngularDragModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
            return m_eAngularDragModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eAngularDragModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.ConstraintsProperties))]
public class LPK_ModifyRigidbody2DOnEvent_ConstraintsPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eConstraintsModifyMode = property.FindPropertyRelative("m_eConstraintsModifyMode");
        SerializedProperty m_bFreezePositionX = property.FindPropertyRelative("m_bFreezePositionX");
        SerializedProperty m_bFreezePositionY = property.FindPropertyRelative("m_bFreezePositionY");
        SerializedProperty m_bFreezeRotationZ = property.FindPropertyRelative("m_bFreezeRotationZ");
        SerializedProperty m_pConstraintsCopyTarget = property.FindPropertyRelative("m_pConstraintsCopyTarget");
        SerializedProperty m_sConstraintsCopyTag = property.FindPropertyRelative("m_sConstraintsCopyTag");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        m_eConstraintsModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eConstraintsModifyMode.isExpanded, new GUIContent("Constraints Properties"), true);

        if(m_eConstraintsModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eConstraintsModifyMode);

            if(m_eConstraintsModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.SET)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_bFreezePositionX);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_bFreezePositionY);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 4, position.width, EditorGUIUtility.singleLineHeight),m_bFreezeRotationZ);
            }

            if(m_eConstraintsModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pConstraintsCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sConstraintsCopyTag);
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
        SerializedProperty m_eConstraintsModifyMode = property.FindPropertyRelative("m_eConstraintsModifyMode");

        if(m_eConstraintsModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eConstraintsModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 5 : EditorGUIUtility.singleLineHeight;
        else if (m_eConstraintsModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eConstraintsModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eConstraintsModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.MassProperies))]
public class LPK_ModifyRigidbody2DOnEvent_MassProperiesDrawer : PropertyDrawer
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
        SerializedProperty m_eMassModifyMode = property.FindPropertyRelative("m_eMassModifyMode");
        SerializedProperty m_flMassValue = property.FindPropertyRelative("m_flMassValue");
        SerializedProperty m_pMassCopyTarget = property.FindPropertyRelative("m_pMassCopyTarget");
        SerializedProperty m_sMassCopyTag = property.FindPropertyRelative("m_sMassCopyTag");

        int indent = EditorGUI.indentLevel;

        m_eMassModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eMassModifyMode.isExpanded, new GUIContent("Mass Properties"), true);

        if(m_eMassModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eMassModifyMode);

            if(m_eMassModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pMassCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sMassCopyTag);
            }

            else if(m_eMassModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_flMassValue);
        
            EditorGUI.indentLevel--;
        }

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
        SerializedProperty m_eMassModifyMode = property.FindPropertyRelative("m_eMassModifyMode");

        if (m_eMassModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            return m_eMassModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eMassModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
            return m_eMassModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eMassModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.GravityScaleProperties))]
public class LPK_ModifyRigidbody2DOnEvent_GravityScalePropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_eGravityScaleModifyMode = property.FindPropertyRelative("m_eGravityScaleModifyMode");
        SerializedProperty m_flGravityScale = property.FindPropertyRelative("m_flGravityScale");
        SerializedProperty m_pGravityScaleCopyTarget = property.FindPropertyRelative("m_pGravityScaleCopyTarget");
        SerializedProperty m_sGravityScaleCopyTag = property.FindPropertyRelative("m_sGravityScaleCopyTag");

        int indent = EditorGUI.indentLevel;

        m_eGravityScaleModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eGravityScaleModifyMode.isExpanded, new GUIContent("Gravity Scale Properties"), true);

        if(m_eGravityScaleModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eGravityScaleModifyMode);

            if(m_eGravityScaleModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pGravityScaleCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sGravityScaleCopyTag);
            }

            else if(m_eGravityScaleModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_flGravityScale);
                    
            EditorGUI.indentLevel--;
        }

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
        SerializedProperty m_eGravityScaleModifyMode = property.FindPropertyRelative("m_eGravityScaleModifyMode");

        if (m_eGravityScaleModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.COPY)
            return m_eGravityScaleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else if(m_eGravityScaleModifyMode.enumValueIndex != (int)LPK_ModifyRigidbody2DOnEvent.LPK_NumericModifyMode.NONE)
            return m_eGravityScaleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else
            return m_eGravityScaleModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(LPK_ModifyRigidbody2DOnEvent.MaterialProperties))]
public class LPK_ModifyRigidbody2DOnEvent_MaterialPropertiesDrawer : PropertyDrawer
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
        SerializedProperty m_Material = property.FindPropertyRelative("m_Material");
        SerializedProperty m_pMaterialCopyTarget = property.FindPropertyRelative("m_pMaterialCopyTarget");
        SerializedProperty m_sMaterialCopyTag = property.FindPropertyRelative("m_sMaterialCopyTag");

        int indent = EditorGUI.indentLevel;

        m_eMaterialModifyMode.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    m_eMaterialModifyMode.isExpanded, new GUIContent("Material Properties"), true);

        if(m_eMaterialModifyMode.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),m_eMaterialModifyMode);

            if(m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.SET)
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_Material);

            if(m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.COPY)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight),m_pMaterialCopyTarget);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight),m_sMaterialCopyTag);
            }

            EditorGUI.indentLevel--;
        }

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

        if(m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.SET)
            return m_eMaterialModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
        else if (m_eMaterialModifyMode.enumValueIndex == (int)LPK_ModifyRigidbody2DOnEvent.LPK_NonNumericModifyMode.COPY)
            return m_eMaterialModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        else
            return m_eMaterialModifyMode.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    }
}

#endif //UNITY_EDITOR

}   //LPK

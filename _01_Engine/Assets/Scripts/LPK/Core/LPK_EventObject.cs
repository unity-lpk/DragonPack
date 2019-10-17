/***************************************************
File:           LPK_EventObject.cs
Authors:        Victor Cecci & Christoher Onorati
Last Updated:   10/17/19
Last Version:   2019.1.14

Description:
  Scriptable object that acts as an instance of an evnet that
  can be sent and received to activate functionality on
  components.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace LPK
{

/**
* CLASS NAME  : LPK_EventObject
* DESCRIPTION : Scriptable object that represents an instance of an event.
**/
[CreateAssetMenu(fileName = "EventObject", menuName = "LPK/EventObject")]
public class LPK_EventObject : ScriptableObject
{
    /************************************************************************************/

    //List of components that will receive the event.
    private List<LPK_Component> m_cReceivers = new List<LPK_Component>();

    /**
    * FUNCTION NAME: Dispatch.
    * DESCRIPTION  : Activates functionality on all game objects subscribed to the event.
    * INPUTS       : _activator - Game object that activated the event, used for validation.  NULL Is all objects.
    * OUTPUTS      : None
    **/
    public void Dispatch(GameObject _activator)
    {
        for(int i = m_cReceivers.Count - 1; i >= 0; i--)
        {
            m_cReceivers[i].OnEvent(_activator);
        }
    }

    /**
    * FUNCTION NAME: Dispatch.
    * DESCRIPTION  : Activates functionality on all components that are on a game object that matches a tag in the search list.
    * INPUTS       : _activator - Game object that activated the event, used for validation.  NULL Is all objects.
    *                _tags - Search list for tags that will enable event action on a component.
    * OUTPUTS      : None
    **/
    public void Dispatch(GameObject _activator, string[] _tags)
    {
        LPK_Component[] componets = FindObjectsOfType<LPK_Component>();

        for(int i = 0; i < componets.Length; i++)
        {
            if (LPK_MultiTagManager.CheckGameObjectForTags(componets[i].gameObject, _tags))
                componets[i].OnEvent(componets[i].gameObject);
        }
    }

    /**
    * FUNCTION NAME: Register.
    * DESCRIPTION  : Registers a component as listening for an event.
    * INPUTS       : _receiver - Component to add to event listening.
    * OUTPUTS      : None
    **/
    public void Register(LPK_Component _receiver)
    {
        m_cReceivers.Add(_receiver);
    }

    /**
    * FUNCTION NAME: Unregister.
    * DESCRIPTION  : Removes a component from listening for an event.
    * INPUTS       : _recever - Component to stop listening for events on.
    * OUTPUTS      : None
    **/
    public void Unregister(LPK_Component _receiver)
    {
        m_cReceivers.Remove(_receiver);
    }

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Remove any lingering event connections from a previous game session.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void OnEnable()
    {
        m_cReceivers.Clear();
    }

    /**
    * FUNCTION NAME: OnDisable
    * DESCRIPTION  : Remove any lingering event connections from a previous game session.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void OnDisable()
    {
        m_cReceivers.Clear();
    }
}

}   //LPK

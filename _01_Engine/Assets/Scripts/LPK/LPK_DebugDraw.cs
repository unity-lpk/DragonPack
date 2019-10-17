/***************************************************
File:           LPK_DebugDraw.cs
Authors:        Christopher Onorati
Last Updated:   10/9/2019
Last Version:   2019.1.4f

Description:
  Holds the parent class for all debug drawing components.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;

namespace LPK
{

/**
* CLASS NAME  : LPK_DebugBase
* DESCRIPTION : Base class for any debug drawing component.
**/
[ExecuteInEditMode]
public class LPK_DebugBase : MonoBehaviour
{
    /************************************************************************************/

    [Tooltip("Sets the debug draw to appear in the scene view.")]
    [Rename("Draw In Scene")]
    public bool m_bDrawInScene = true;

    [Tooltip("Sets the debug draw to appear in game view.")]
    [Rename("Draw In Game")]
    public bool m_bDrawInGame = true;

    [Tooltip("Sets the debug draw to appear in the built game.")]
    [Rename("Draw In Build")]
    public bool m_bDrawInBuild = true;

    [Tooltip("Whether this object's children should also be drawn.")]
    [Rename("Draw Hierarchy")]
    public bool m_bDrawHierarchy = false;

    //Label per-component.  Does not do anything, just allows notes in the inspector.
    public string m_sLabel;

    /************************************************************************************/

    protected Transform m_cTransform;

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Cache the transform.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_cTransform = GetComponent<Transform>();
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manages debug drawing.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if (!Application.isEditor && m_bDrawInBuild)
        {
            if (m_bDrawHierarchy)
                DrawRecursive(gameObject);
            else
                Draw(gameObject);

        }
        else if (Application.isEditor && (m_bDrawInScene || m_bDrawInGame))
        {
            if (m_bDrawHierarchy)
                DrawRecursive(gameObject);
            else
                Draw(gameObject);
        }
        else if(!m_bDrawInScene && !m_bDrawInGame && !m_bDrawInBuild)
        {
            if (m_bDrawHierarchy)
                UndrawRecursive(gameObject);
            else
                Undraw(gameObject);
        }

        OnUpdate();
    }

    /**
    * FUNCTION NAME: OnUpdate
    * DESCRIPTION  : Update support class to be used by parents.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    protected virtual void OnUpdate()
    {
        //Implemented in child class.
    }

    /**
    * FUNCTION NAME: DrawRecursive
    * DESCRIPTION  : Draw debug info for all children attached to the owner gameobject.
    * INPUTS       : _gameObj - Game object to find children of.
    * OUTPUTS      : None
    **/
    void DrawRecursive(GameObject _gameObj)
    {
        //Draw current object.
        Draw(_gameObj);

        for (int i = 0; i < m_cTransform.childCount; i++)
            DrawRecursive(m_cTransform.GetChild(i).gameObject);
    }

    /**
    * FUNCTION NAME: Draw
    * DESCRIPTION  : Draw debug info for the gameobject.
    * INPUTS       : _gameObj - Game object to draw debug info for.
    * OUTPUTS      : None
    **/
    protected virtual void Draw(GameObject _gameObj)
    {
        //Implemented in inhereted class.
    }

    /**
    * FUNCTION NAME: UndrawRecursive
    * DESCRIPTION  : Remove debug info for all children attached to the owner gameobject.
    * INPUTS       : _gameObj - Game object to find children of.
    * OUTPUTS      : None
    **/
    void UndrawRecursive(GameObject _gameObj)
    {
        //Undraw debug info for current object.
        Undraw(_gameObj);

        for (int i = 0; i < m_cTransform.childCount; i++)
            UndrawRecursive(m_cTransform.GetChild(i).gameObject);
    }

    /**
    * FUNCTION NAME: Undraw
    * DESCRIPTION  : Removes any debug info for the gameobject.
    * INPUTS       : obj - Game object to remove debug info for.
    * OUTPUTS      : None
    **/
    protected virtual void Undraw(GameObject obj)
    {
        //Implemented in inhereted class.
    }

    /**
    * FUNCTION NAME: OnDisable
    * DESCRIPTION  : Remnove debug info when the component is destroyed.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    protected void OnDisable()
    {
        if (m_bDrawHierarchy)
            UndrawRecursive(gameObject);
        else
            Undraw(gameObject);
    }
}

/**
* CLASS NAME  : LPK_DebugLineDrawer
* DESCRIPTION : Debugging class to draw a line in game.
**/
public class LPK_DebugLineDrawer
{
    /************************************************************************************/

    //Reference to the line renderer component.
    LineRenderer m_cLineRenderer;

    //Color for the line to draw.
    public Color m_vecLineColor;

    //Game object created by class.
    public GameObject m_pGameObject;

    /**
    * FUNCTION NAME: Constructor
    * DESCRIPTION  : Set up the line renderer and color.
    * INPUTS       : _lineColor - color of the line.
    *                _parent    - Game object to set as the parent for the line renderer that acts as debug drawing.
    * OUTPUTS      : None
    **/
    public LPK_DebugLineDrawer(Color _lineColor, GameObject _parent)
    {
        m_pGameObject = new GameObject("LPK_DebugLineObj");
        m_pGameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInBuild
                                  | HideFlags.DontSaveInEditor | HideFlags.HideInInspector
                                  | HideFlags.HideInHierarchy;

        m_cLineRenderer = m_pGameObject.AddComponent<LineRenderer>();
        m_cLineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

        m_pGameObject.GetComponent<Transform>().SetParent(_parent.transform);

        m_vecLineColor = _lineColor;
    }

    /**
    * FUNCTION NAME: DrawLineInGameView
    * DESCRIPTION  : Draw the line.
    * INPUTS       : startPos - start position of the line.
    *                endPos   - end position of the line.
    * OUTPUTS      : None
    **/
    public void DrawLineInGameView(Vector3 _startPos, Vector3 _endPos)
    {
        //Not enough memory to create the line renderer component - ABORT.
        if (m_cLineRenderer == null)
            return;

        //Set color
        m_cLineRenderer.startColor = m_vecLineColor;
        m_cLineRenderer.endColor = m_vecLineColor;

        //Set width
        m_cLineRenderer.startWidth = 0.05f;
        m_cLineRenderer.endWidth = 0.05f;

        //Set line count which is 2
        m_cLineRenderer.positionCount = 2;

        //Set the postion of both two lines
        m_cLineRenderer.SetPosition(0, _startPos);
        m_cLineRenderer.SetPosition(1, _endPos);
    }

    /**
    * FUNCTION NAME: OnDisable 
    * DESCRIPTION  : Remove the line renderer component.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void OnDisable()
    {
        if (m_cLineRenderer != null)
            Object.Destroy(m_cLineRenderer.gameObject);
    }
}

}   //LPK
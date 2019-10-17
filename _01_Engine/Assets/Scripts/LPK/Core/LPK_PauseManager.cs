/***************************************************
File:           LPK_PauseManager.cs
Authors:        Christopher Onorati
Last Updated:   8/1/2019
Last Version:   2018.3.14

Description:
  This script contains the pause manager for the LPK,
  used by ModifyPauseStateOnEvent and the dev console.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;

namespace LPK
{

/**
* CLASS NAME  : LPK_PauseManager
* DESCRIPTION : Global manager to adjust pause state of gameplay.
**/
static class LPK_PauseManager
{
    //Timescale before the game was paused.
    static float m_flPreviousTimeScale;

    /**
    * FUNCTION NAME: Pause
    * DESCRIPTION  : Pauses the scene and the actions of any LPK component.
    * INPUTS       : _newTimeScale - New time sclae to play at while paused.  Usually 0.0f.
    * OUTPUTS      : None
    **/
    public static void Pause(float _newTimeScale)
    {
        m_flPreviousTimeScale = Time.timeScale;
        Time.timeScale = _newTimeScale;
    }

    /**
    * FUNCTION NAME: Unpause
    * DESCRIPTION  : Resumes the scene and the actions of any LPK component.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static void Unpause()
    {
        Time.timeScale = m_flPreviousTimeScale;
    }
}

}  //LPK

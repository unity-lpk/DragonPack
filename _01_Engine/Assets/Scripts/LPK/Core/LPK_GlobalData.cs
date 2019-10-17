/***************************************************
File:           LPK_GlobalData.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Contains global data that has no other true place to
  be placed.  Ideally, this class is used as ==little==
  as possible.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections;
using UnityEngine;

namespace LPK
{

/**
* CLASS NAME  : LPK_GlobalData
* DESCRIPTION : Global data for use throughout the LPK.
**/
static class LPK_GlobalData
{
    /**
    * FUNCTION NAME: GetCurrentFrameRate
    * DESCRIPTION  : Calculates and returns the current framerate of the game.
    * INPUTS       : None
    * OUTPUTS      : float - current frame rate
    **/
    static public float GetCurrentFrameRate()
    {
        return 1.0f / Time.deltaTime;
    }
}

}   //LPK

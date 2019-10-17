/***************************************************
File:           LPK_DisplayObjectBase.cs
Authors:        Christopher Onorati
Last Updated:   6/10/2019
Last Version:   2018.3.14

Description:
  Base class for all display objects to inheret from.
  Display updaters call the UpdateDisplay function
  derrived from this class.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

namespace LPK
{

/**
* CLASS NAME  : LPK_DisplayObject
* DESCRIPTION : Base class for displays.
**/
public class LPK_DisplayObject :  LPK_Component
{
    /**
    * FUNCTION NAME: UpdateDisplay
    * DESCRIPTION  : Updates whatever display this manages.
    * INPUTS       : _currentVal - Current value of the display.
    *                _maxVal     - Max value of the display.
    * OUTPUTS      : None
    **/
    public virtual void UpdateDisplay(float _currentVal, float _maxVal)
    {
        //Implemented by inhereted classes.
    }
}

}   //LPK

/***************************************************
File:           LPK_Command_Clear.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Console command to exit the game.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Clear
* DESCRIPTION : Console command to clear all logged messages.
**/
public class LPK_Command_Clear : LPK_ConsoleCommand
{
    /************************************************************************************/

    //Text the parser is looking for to fire off the command.
    public override string m_sCommandText {get; protected set;}

    //Help text when listing commands.
    public override string m_sHelpText {get; protected set;}

    //Flag to detect if cheats are needed or not.
    public override bool m_bRequiresCheatsActive {get; protected set;}

    //Flag to prevent a console command from showing in the help list.
    public override bool m_bHideCommandFromHelpList {get; protected set;}

    /**
    * FUNCTION NAME: Constructor
    * DESCRIPTION  : Creats a new instance of the command.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public LPK_Command_Clear()
    {
        m_sCommandText = "clear";
        m_sHelpText = "Clear the console of all messages.";
        m_bRequiresCheatsActive = false;
        m_bHideCommandFromHelpList = false;

        AddCommandToConsole();
    }

    /**
    * FUNCTION NAME: RunCommand
    * DESCRIPTION  : Runs the console command's functionality.
    * INPUTS       : _arguments - Arguments passed to the command.
    * OUTPUTS      : None
    **/
    public override void RunCommand(string[] _arguments)
    {
        LPK_DeveloperConsole.GetConsoleTextObject().text = "";
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Clear CreateCommand()
    {
        return new LPK_Command_Clear();
    }
}

}   //LPK_CONSOLE

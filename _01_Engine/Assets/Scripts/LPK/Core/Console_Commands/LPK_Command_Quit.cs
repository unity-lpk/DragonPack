/***************************************************
File:           LPK_Command_Quit.cs
Authors:        Christopher Onorati
Last Updated:   10/1/2019
Last Version:   2018.3.14

Description:
  Console command to exit the game.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using System.Collections;

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Quit
* DESCRIPTION : Console command to quit the current game session.
**/
public class LPK_Command_Quit : LPK_ConsoleCommand
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
    public LPK_Command_Quit()
    {
        m_sCommandText = "quit";
        m_sHelpText = "Quit the current game session.";
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Quit CreateCommand()
    {
        return new LPK_Command_Quit();
    }
}

}   //LPK_CONSOLE

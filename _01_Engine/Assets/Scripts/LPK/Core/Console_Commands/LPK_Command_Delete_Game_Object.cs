/***************************************************
File:           LPK_Command_Pacifist.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Console command to destroy a single specified object
  from the scene.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Delete_Game_Object
* DESCRIPTION : Console command to delete an object from the scene.
**/
public class LPK_Command_Delete_Game_Object : LPK_ConsoleCommand
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
    public LPK_Command_Delete_Game_Object()
    {
        m_sCommandText = "delete_game_object";
        m_sHelpText = "Delete a single game object in the current scene by name.  Note there cannot be any spaces in the game object name.";
        m_bRequiresCheatsActive = true;
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
        if(_arguments == null || _arguments.Length <= 0)
        {
            LPK_DeveloperConsole.AddMessageToConsole("No argument passed.  Game object name must be specified.");
            return;
        }

        GameObject target = GameObject.Find(_arguments[0]);

        if(target == null)
        {
            LPK_DeveloperConsole.AddMessageToConsole("Cannot find game object with name given.");
            return;
        }

        if(target.tag == "MainCamera")
        {
            LPK_DeveloperConsole.AddMessageToConsole("Nice try, bud.");
            return;
        }

        GameObject.Destroy(target);
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Delete_Game_Object CreateCommand()
    {
        return new LPK_Command_Delete_Game_Object();
    }
}

}   //LPK_CONSOLE

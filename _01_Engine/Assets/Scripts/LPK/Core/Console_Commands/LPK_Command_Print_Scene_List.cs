/***************************************************
File:           LPK_Command_Print_Scene_List.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Console command to load the next scene in the project.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine.SceneManagement;
using System.IO;    /* File names. */

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Print_Scene_List
* DESCRIPTION : Console command to print a list of all scenes in the build.
**/
public class LPK_Command_Print_Scene_List : LPK_ConsoleCommand
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
    LPK_Command_Print_Scene_List()
    {
        m_sCommandText = "print_scene_list";
        m_sHelpText = "Print a list of all scenes in the build of the game.";
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

        LPK_DeveloperConsole.AddMessageToConsole("\n\nScene List:\n\n");

        for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            LPK_DeveloperConsole.AddMessageToConsole(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));

        LPK_DeveloperConsole.AddMessageToConsole("\n\n");
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Print_Scene_List CreateCommand()
    {
        return new LPK_Command_Print_Scene_List();
    }
}

}   //LPK_CONSOLE

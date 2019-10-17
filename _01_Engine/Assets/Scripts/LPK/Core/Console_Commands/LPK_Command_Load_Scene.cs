/***************************************************
File:           LPK_Command_Load_Scene.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Console command to load a scene by name.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine.SceneManagement;

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Load_Scene
* DESCRIPTION : Console command to load a scene by name.
**/
public class LPK_Command_Load_Scene : LPK_ConsoleCommand
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
    LPK_Command_Load_Scene()
    {
        m_sCommandText = "load_scene";
        m_sHelpText = "Load a scene based on the name given.  Takes 1 argument.";
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
        if(_arguments.Length <= 0)
        {
            LPK_DeveloperConsole.AddMessageToConsole("No argument given.  Please give the name of the scene to load.");
            return;
        }

        /*if(!SceneManager.GetSceneByName(_arguments[0]).IsValid())
        {
            LPK_DeveloperConsole.AddMessageToConsole("Invalid scene name given.  Please confirm the scene name was typed correctly.");
            return;
        }*/

        SceneManager.LoadScene(_arguments[0]);
        
        LPK_DeveloperConsole.AddMessageToConsole("Loaded scene: " + SceneManager.GetActiveScene().name);
        LPK_DeveloperConsole.SetConsoleActiveState(false);
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Load_Scene CreateCommand()
    {
        return new LPK_Command_Load_Scene();
    }
}

}   //LPK_CONSOLE

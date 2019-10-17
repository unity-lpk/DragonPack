/***************************************************
File:           LPK_Command_Load_Next_Scene.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Console command to load the next scene in the project.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine.SceneManagement;

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Load_Next_Scene
* DESCRIPTION : Console command to load the next scene in the build order.
**/
public class LPK_Command_Load_Next_Scene : LPK_ConsoleCommand
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
    LPK_Command_Load_Next_Scene()
    {
        m_sCommandText = "load_next_scene";
        m_sHelpText = "Load the next scene in the project.";
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentSceneIndex++;

        //Prevent overflow.
        if(currentSceneIndex >= SceneManager.sceneCountInBuildSettings)
            currentSceneIndex = 0;

        SceneManager.LoadScene(currentSceneIndex);

        //NOTENOTE:  Must be done after the scene is loaded as the name is null otherwise.
        LPK_DeveloperConsole.AddMessageToConsole("Loaded scene " + SceneManager.GetSceneByBuildIndex(currentSceneIndex).name);
        LPK_DeveloperConsole.SetConsoleActiveState(false);
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Load_Next_Scene CreateCommand()
    {
        return new LPK_Command_Load_Next_Scene();
    }
}

}   //LPK_CONSOLE

/***************************************************
File:           LPK_Command_Toggle_Cheats.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Console command to toggle cheats on and off.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Toggle_Cheats
* DESCRIPTION : Console command to toggle cheats on and off.
**/
public class LPK_Command_Toggle_Cheats : LPK_ConsoleCommand
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
    public LPK_Command_Toggle_Cheats()
    {
        m_sCommandText = "toggle_cheats";
        m_sHelpText = "Toggle cheats on and off.  All commands below require cheats active.";
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
        LPK_DeveloperConsole.SetCheatsActvieState(!LPK_DeveloperConsole.GetCheatsActiveState());

        if(LPK_DeveloperConsole.GetCheatsActiveState())
            LPK_DeveloperConsole.AddMessageToConsole("Cheats enabled.");
        else
            LPK_DeveloperConsole.AddMessageToConsole("Cheats disabeled.");
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Toggle_Cheats CreateCommand()
    {
        return new LPK_Command_Toggle_Cheats();
    }
}

}   //LPK_CONSOLE

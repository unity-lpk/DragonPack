/***************************************************
File:           LPK_Command_Noclip.cs
Authors:        Christopher Onorati
Last Updated:   5/2/2019
Last Version:   2018.3.14

Description:
  Console command to enable free movement of the player.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using LPK;  /* Access to controllers. */

namespace LPK_CONSOLE
{

/**
* CLASS NAME  : LPK_Command_Noclip
* DESCRIPTION : Console command to allow flying through scenes.
**/
public class LPK_Command_Noclip : LPK_ConsoleCommand
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

    /************************************************************************************/

    bool m_bNoclipActive = false;

    /**
    * FUNCTION NAME: Constructor
    * DESCRIPTION  : Creats a new instance of the command.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public LPK_Command_Noclip()
    {
        m_sCommandText = "noclip";
        m_sHelpText = "Enable free player movement.";
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
        m_bNoclipActive = !m_bNoclipActive;

        if(m_bNoclipActive)
            LPK_DeveloperConsole.AddMessageToConsole("Noclip mode enabled.");
        else
            LPK_DeveloperConsole.AddMessageToConsole("Noclip mode disabled.");
            
        if(m_bNoclipActive)
            ActivateNoclip();
        else
            DeactivateNoclip();
    }

    /**
    * FUNCTION NAME: ActivateNoclip
    * DESCRIPTION  : Activates noclip mode on all active player controllers.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void ActivateNoclip()
    {
        GameObject[]pPlayers = GameObject.FindObjectsOfType<GameObject>();

        for(int i = 0; i < pPlayers.Length; i++)
        {
            //Platformer controller.
            if(pPlayers[i].GetComponent<LPK_DynamicPlatformerController>() && !pPlayers[i].GetComponent<LPK_DynamicPlatformerController>().m_bNoclipping)
            {
                LPK_DynamicPlatformerController controller = pPlayers[i].GetComponent<LPK_DynamicPlatformerController>();
                controller.m_bNoclipping = true;

                controller.m_flNoclipRigidBodyGravityScale = pPlayers[i].GetComponent<Rigidbody2D>().gravityScale;
                pPlayers[i].GetComponent<Rigidbody2D>().gravityScale = 0.0f;

                if(pPlayers[i].GetComponent<Collider2D>())
                {
                    controller.m_bNoclipWasTrigger = pPlayers[i].GetComponent<Collider2D>().isTrigger;
                    pPlayers[i].GetComponent<Collider2D>().isTrigger = true;
                }
            }

            //Top down orthoganol
            else if(pPlayers[i].GetComponent<LPK_DynamicTopDownOrthogonalController>() && !pPlayers[i].GetComponent<LPK_DynamicTopDownOrthogonalController>().m_bNoclipping)
            {
                LPK_DynamicTopDownOrthogonalController controller = pPlayers[i].GetComponent<LPK_DynamicTopDownOrthogonalController>();
                controller.m_bNoclipping = true;

                controller.m_flNoclipRigidBodyGravityScale = pPlayers[i].GetComponent<Rigidbody2D>().gravityScale;
                pPlayers[i].GetComponent<Rigidbody2D>().gravityScale = 0.0f;

                if(pPlayers[i].GetComponent<Collider2D>())
                {
                    controller.m_bNoclipWasTrigger = pPlayers[i].GetComponent<Collider2D>().isTrigger;
                    pPlayers[i].GetComponent<Collider2D>().isTrigger = true;
                }
            }

            //Rotation controller
            else if(pPlayers[i].GetComponent<LPK_DynamicTopDownRotationController>() && !pPlayers[i].GetComponent<LPK_DynamicTopDownRotationController>().m_bNoclipping)
            {
                LPK_DynamicTopDownRotationController controller = pPlayers[i].GetComponent<LPK_DynamicTopDownRotationController>();
                controller.m_bNoclipping = true;

                controller.m_flNoclipRigidBodyGravityScale = pPlayers[i].GetComponent<Rigidbody2D>().gravityScale;
                pPlayers[i].GetComponent<Rigidbody2D>().gravityScale = 0.0f;

                if(pPlayers[i].GetComponent<Collider2D>())
                {
                    controller.m_bNoclipWasTrigger = pPlayers[i].GetComponent<Collider2D>().isTrigger;
                    pPlayers[i].GetComponent<Collider2D>().isTrigger = true;
                }
            }
        }
    }

    /**
    * FUNCTION NAME: DeactivateNoclip
    * DESCRIPTION  : Deactivates noclip mode on all active player controllers.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DeactivateNoclip()
    {
        GameObject[]pPlayers = GameObject.FindObjectsOfType<GameObject>();

        for(int i = 0; i < pPlayers.Length; i++)
        {
            //Platformer controller.
            if(pPlayers[i].GetComponent<LPK_DynamicPlatformerController>() && pPlayers[i].GetComponent<LPK_DynamicPlatformerController>().m_bNoclipping)
            {
                LPK_DynamicPlatformerController controller = pPlayers[i].GetComponent<LPK_DynamicPlatformerController>();
                controller.m_bNoclipping = false;

                pPlayers[i].GetComponent<Rigidbody2D>().gravityScale = controller.m_flNoclipRigidBodyGravityScale;

                if(pPlayers[i].GetComponent<Collider2D>())
                    pPlayers[i].GetComponent<Collider2D>().isTrigger = controller.m_bNoclipWasTrigger;
            }

            //Top down orthoganol
            else if(pPlayers[i].GetComponent<LPK_DynamicTopDownOrthogonalController>() && pPlayers[i].GetComponent<LPK_DynamicTopDownOrthogonalController>().m_bNoclipping)
            {
                LPK_DynamicTopDownOrthogonalController controller = pPlayers[i].GetComponent<LPK_DynamicTopDownOrthogonalController>();
                controller.m_bNoclipping = false;

                pPlayers[i].GetComponent<Rigidbody2D>().gravityScale = controller.m_flNoclipRigidBodyGravityScale;

                if(pPlayers[i].GetComponent<Collider2D>())
                    pPlayers[i].GetComponent<Collider2D>().isTrigger = controller.m_bNoclipWasTrigger;
            }

            //Rotation controller
            else if(pPlayers[i].GetComponent<LPK_DynamicTopDownRotationController>() && pPlayers[i].GetComponent<LPK_DynamicTopDownRotationController>().m_bNoclipping)
            {
                LPK_DynamicTopDownRotationController controller = pPlayers[i].GetComponent<LPK_DynamicTopDownRotationController>();
                controller.m_bNoclipping = false;

                pPlayers[i].GetComponent<Rigidbody2D>().gravityScale = controller.m_flNoclipRigidBodyGravityScale;

                if(pPlayers[i].GetComponent<Collider2D>())
                    pPlayers[i].GetComponent<Collider2D>().isTrigger = controller.m_bNoclipWasTrigger;
            }
        }
    }

    /**
    * FUNCTION NAME: CraeteCommand
    * DESCRIPTION  : Creates a new command for console usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public static LPK_Command_Noclip CreateCommand()
    {
        return new LPK_Command_Noclip();
    }
}

}   //LPK_CONSOLE

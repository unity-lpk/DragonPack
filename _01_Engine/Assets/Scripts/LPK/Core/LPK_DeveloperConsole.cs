/***************************************************
File:           LPK_DeveloperConsole.cs
Authors:        Christopher Onorati
Last Updated:   5/23/2019
Last Version:   2018.3.14

Description:
  Contains classes to run the LPK developer console.  It
  is not recommended to modify this file unless very familiar
  with Unity and the LPK. 

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using System.Collections;
using System.Collections.Generic;   /* Dictionary */
using System.Linq;                  /* Element At */
using UnityEngine;
using UnityEngine.UI;               /* Access to UI components that create the developer console. */
using LPK;                          /* Access to pause manager. */
using TMPro;                        /* TextMeshPro access. */
using UnityEditor;

namespace LPK_CONSOLE
{

#pragma warning disable CS0649  //Remove warning for unassigned privates.

/**
* CLASS NAME  : LPK_ConsoleCommand
* DESCRIPTION : Abstract class used to implement commands for the console to run.
**/
public abstract class LPK_ConsoleCommand
{
    /************************************************************************************/

    //Command that triggers functionality.
    public abstract string m_sCommandText {get; protected set;}

    //Help text.
    public abstract string m_sHelpText {get; protected set;}

    //Flag to detect if cheats are needed or not.
    public abstract bool m_bRequiresCheatsActive {get; protected set;}

    //Flag to prevent a console command from showing in the help list.
    public abstract bool m_bHideCommandFromHelpList {get; protected set;}

    /**
    * FUNCTION NAME: AddCommandToConsole
    * DESCRIPTION  : Adds commands to the console's dictionary lookup.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public void AddCommandToConsole()
    {       
        LPK_DeveloperConsole.AddCommand(m_sCommandText, this);
    }

    /**
    * FUNCTION NAME: RunCommand
    * DESCRIPTION  : Per-command implementation of what functionality should occur.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public abstract void RunCommand(string[] _arguments);
}

/**
* CLASS NAME  : LPK_DeveloperConsole
* DESCRIPTION : Core logic behind the operation of the developer console.
**/
public class LPK_DeveloperConsole : LPK_Component
{
    /************************************************************************************/
    
    //Console instance.
    public static LPK_DeveloperConsole m_ConsoleInstance;
    
    //Dictionary of commands.
    public static Dictionary<string, LPK_ConsoleCommand> m_CommandDictionary;

    [Header("UI Components")]

    [SerializeField]
    [Tooltip("Canvas that owns the developer console.")]
    Canvas m_ConsoleCanvas;

    [SerializeField]
    [Tooltip("ScrollRect that displays logging info.")]
    ScrollRect m_ScrollRect;

    [SerializeField]
    [Tooltip("Text that displays in the console.")]
    TextMeshProUGUI m_ConsoleText;

    [SerializeField]
    [Tooltip("Text that displays inside the input bar.")]
    Text m_InputText;

    [SerializeField]
    [Tooltip("UI element where input is given by the user.")]
    InputField m_InputField;

    /************************************************************************************/

    //Active state of cheats.
    static bool m_bCheatsEnabled = false;

    //Keeps a record if cheat were ever enabled, even once.
    static bool m_bWereCheatsEverEnabled = false;
    
    //Index of previous commands.
    static int m_iPreviousCommandIndex = 0;

    //Framerate before the last time the console was opened.
    static float m_flLastFramerate;

    //Previous command.
    static LinkedList<string> m_PreviousCommandText;

    /**
    * FUNCTION NAME: Awake
    * DESCRIPTION  : Ensures only one instance of the console exists at any point in time.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Awake()
    {
        //Singleton check.
        if(m_ConsoleInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_ConsoleInstance = this;
        m_CommandDictionary = new Dictionary<string, LPK_ConsoleCommand>();
        Object.DontDestroyOnLoad(gameObject);
    }

    /**
    * FUNCTION NAME: Start
    * DESCRIPTION  : Ensures console starts inactive when playing the game and initializes
    *                command list.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Start()
    {
        m_ConsoleInstance.m_ConsoleCanvas.gameObject.SetActive(false);
        CreateCommands();

        m_ConsoleInstance.m_ConsoleText.text += "\n";
        m_PreviousCommandText = new LinkedList<string>();
    }

    /**
    * FUNCTION NAME: Update
    * DESCRIPTION  : Manages input checking.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void Update()
    {
        if(!m_ConsoleInstance.m_ConsoleCanvas.gameObject.activeInHierarchy)
            m_flLastFramerate =LPK_GlobalData.GetCurrentFrameRate();

        //Toggle console.
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            SetConsoleActiveState(!m_ConsoleCanvas.gameObject.activeInHierarchy);
            StartCoroutine(m_ConsoleInstance.DelayScrollDown());       

            if(m_ConsoleCanvas.gameObject.activeInHierarchy)
            {
                LPK_PauseManager.Pause(0.0f);
                m_ConsoleInstance.m_InputField.Select();
                m_ConsoleInstance.m_InputField.ActivateInputField();
            }
            else
                LPK_PauseManager.Unpause();
        }
        
        //Run a command.
        if(m_ConsoleCanvas.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            if(!string.IsNullOrEmpty(m_InputText.text))
            {
                m_PreviousCommandText.AddFirst(m_InputText.text);
                AddMessageToConsole(m_InputText.text);
                ParseInput(m_InputText.text);
            }
            
            //Clear out the field for another command.
            m_InputField.Select();
            m_InputField.text = "";
            m_iPreviousCommandIndex = 0;

            //Activate the input field.
            m_ConsoleInstance.m_InputField.Select();
            m_ConsoleInstance.m_InputField.ActivateInputField();
        }

        //Use a previous command.
        if(m_ConsoleCanvas.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_InputField.text = m_PreviousCommandText.ElementAt(m_iPreviousCommandIndex);
            m_iPreviousCommandIndex++;

            if(m_iPreviousCommandIndex >= m_PreviousCommandText.Count)
                m_iPreviousCommandIndex = 0;
        }

        //Use a more recent command.
        if(m_ConsoleCanvas.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_iPreviousCommandIndex--;

            if(m_iPreviousCommandIndex < 0)
                m_iPreviousCommandIndex = m_PreviousCommandText.Count - 1;

            m_InputField.text = m_PreviousCommandText.ElementAt(m_iPreviousCommandIndex);
        }
    }

    /**
    * FUNCTION NAME: ParseInput
    * DESCRIPTION  : Checks input passed by user to see if a valid command exists.
    * INPUTS       : _input - Input to parse to see if a command exists for the string entry.
    * OUTPUTS      : None
    **/
    void ParseInput(string _input)
    {
        string[] splitInput = _input.Split(null);

        //Error commands or null data.
        if(splitInput.Length <= 0 || _input == null)
        {
            AddMessageToConsole("Command not recognized.");
            return;
        }

        //NOTENOTE:  Request for help command.  This is the only hard coded console command.
        if(_input == "help" || _input == "Help" || _input == "HELP")
        {
            DisplayHelpText();
            return;
        }

        if(!m_CommandDictionary.ContainsKey(splitInput[0]))
        {
            AddMessageToConsole("Command not recognized.");
            return;
        }

        if(!m_bCheatsEnabled && m_CommandDictionary[splitInput[0]].m_bRequiresCheatsActive)
        {
            AddMessageToConsole("Command requires cheats to be active.  Use command 'toggle_cheats' to do so.");
            return;
        }

        //Argument passing.
        int size = 0;

        if(splitInput.Length - 1 > 0)
            size = splitInput.Length - 1;

        string[] arguments = new string[size];

        for(int i = 0; i < size; i++)
            arguments[i] = splitInput[i + 1];

        //Run command.
        m_CommandDictionary[splitInput[0]].RunCommand(arguments);
    }

    /**
    * FUNCTION NAME: DisplayHelpText
    * DESCRIPTION  : Displays text for each console command's usage.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void DisplayHelpText()
    {
        AddMessageToConsole("\n\nList of commands below...\n\n");

        for(int i = 0; i < m_CommandDictionary.Count; i++)
        {
            if(!m_CommandDictionary.ElementAt(i).Value.m_bHideCommandFromHelpList)
                AddMessageToConsole(m_CommandDictionary.ElementAt(i).Key + " - " + m_CommandDictionary.ElementAt(i).Value.m_sHelpText);
        }

        AddMessageToConsole("\n\nEnd of command list...\n\n");
    }

    /**
    * FUNCTION NAME: CreateCommand
    * DESCRIPTION  : Creates commands that are valid for the console to use.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CreateCommands()
    {
        LPK_Command_Clear.CreateCommand();
        LPK_Command_Print_Console.CreateCommand();
        LPK_Command_Print_FPS.CreateCommand();
        LPK_Command_Print_Scene_List.CreateCommand();
        LPK_Command_Print_Current_Scene_Name.CreateCommand();
        LPK_Command_Quit.CreateCommand();
        LPK_Command_Restart.CreateCommand();
        LPK_Command_Toggle_Cheats.CreateCommand();
        LPK_Command_Delete_Game_Object.CreateCommand();
        LPK_Command_Load_Next_Scene.CreateCommand();
        LPK_Command_Load_Previous_Scene.CreateCommand();
        LPK_Command_Load_Scene.CreateCommand();
        LPK_Command_Noclip.CreateCommand();
    }

    /**
    * FUNCTION NAME: AddMessageToConsole
    * DESCRIPTION  : Adds text to the console log.
    * INPUTS       : _message - Text to add to the console.
    * OUTPUTS      : None
    **/
    public static void AddMessageToConsole(string _message)
    {
        m_ConsoleInstance.m_ConsoleText.text += _message + "\n";

        //Force scroll rect back to the bottom.
        m_ConsoleInstance.StartCoroutine(m_ConsoleInstance.DelayScrollDown());       
    }

    /**
    * FUNCTION NAME: DelayScrollDown
    * DESCRIPTION  : Forces a frame delay from scrolling the scroll rect.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    IEnumerator DelayScrollDown()
    {
        yield return new WaitForSeconds(0.0f);
        m_ConsoleInstance.m_ScrollRect.verticalNormalizedPosition = 0.0f;
    }

    /**
    * FUNCTION NAME: AddCommand
    * DESCRIPTION  : Adds a command to the developer console.
    * INPUTS       : _name    - Name of the command (text to type).
    *                _command - Command that is linked to the input text (_name).
    * OUTPUTS      : None
    **/
    public static void AddCommand(string _name, LPK_ConsoleCommand _command)
    {
        //New command to add.
        if(!m_CommandDictionary.ContainsKey(_name))
            m_CommandDictionary.Add(_name, _command);
    }

    /**
    * FUNCTION NAME: GetLastFrameRate
    * DESCRIPTION  : Get the framerate of the the game before the console was loaded.
    * INPUTS       : None
    * OUTPUTS      : float - Framerate from before the console was loaded.
    **/
    public static float GetLastFrameRate()
    {
        return m_flLastFramerate;
    }

    /**
    * FUNCTION NAME: SetCheatsActiveState
    * DESCRIPTION  : Sets the active state of console cheats.
    * INPUTS       : _state - State to set cheats.
    * OUTPUTS      : None
    **/
    public static void SetCheatsActvieState(bool _state)
    {
        m_bCheatsEnabled = _state;

        if(m_bCheatsEnabled)
            m_bWereCheatsEverEnabled = true;
    }

    /**
    * FUNCTION NAME: GetCheatsActiveState
    * DESCRIPTION  : Getter for detecting if cheats are active in the game session.
    * INPUTS       : None
    * OUTPUTS      : bool - True/False status of cheats actve.
    **/
    public static bool GetCheatsActiveState()
    {
        return m_bCheatsEnabled;
    }

    /**
    * FUNCTION NAME: CheckIfCheatsWereEverEnabled
    * DESCRIPTION  : Getter for detecting if cheats were ever used.  Not used by any LPK
    *                script, but useful for extension for systems like achivement or leaderboards.
    * INPUTS       : None
    * OUTPUTS      : bool - True/False status of cheats were ever enabled.
    **/
    public static bool CheckIfCheatsWereEverEnabled()
    {
        return m_bWereCheatsEverEnabled;
    }

    /**
    * FUNCTION NAME: SetConsoleActiveState
    * DESCRIPTION  : Set the active state of the developer console.
    * INPUTS       : _state - Active state of the console.
    * OUTPUTS      : None
    **/
    public static void SetConsoleActiveState(bool _state)
    {
        m_ConsoleInstance.m_ConsoleCanvas.gameObject.SetActive(_state);

        if(_state)
            LPK_PauseManager.Pause(0.0f);
        else
            LPK_PauseManager.Unpause();
    }

    /**
    * FUNCTION NAME: GetConsoleTextObject
    * DESCRIPTION  : Allows access to the console text object for modification by other scripts.
    * INPUTS       : None
    * OUTPUTS      : Text - Object that holds the console's log text.
    **/
    public static TextMeshProUGUI GetConsoleTextObject()
    {
        return m_ConsoleInstance.m_ConsoleText;
    }
}

#pragma warning restore CS0649

#if UNITY_EDITOR

[CustomEditor(typeof(LPK_DeveloperConsole))]
public class LPK_DeveloperConsoleEditor : Editor
{
    SerializedProperty m_ConsoleCanvas;
    SerializedProperty m_ScrollRect;
    SerializedProperty m_ConsoleText;
    SerializedProperty m_InputText;
    SerializedProperty m_InputField;

    /**
    * FUNCTION NAME: OnEnable
    * DESCRIPTION  : Save out serialized classes.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnEnable()
    {
        m_ConsoleCanvas = serializedObject.FindProperty("m_ConsoleCanvas");
        m_ScrollRect = serializedObject.FindProperty("m_ScrollRect");
        m_ConsoleText = serializedObject.FindProperty("m_ConsoleText");
        m_InputText = serializedObject.FindProperty("m_InputText");
        m_InputField = serializedObject.FindProperty("m_InputField");
    }

    /**
    * FUNCTION NAME: OnInspectorGUI
    * DESCRIPTION  : Override GUI for inspector.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    public override void OnInspectorGUI()
    {
        LPK_DeveloperConsole owner = (LPK_DeveloperConsole)target;

        LPK_DeveloperConsole editorOwner = owner.GetComponent<LPK_DeveloperConsole>();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Script");
        editorOwner = (LPK_DeveloperConsole)EditorGUILayout.ObjectField(editorOwner, typeof(LPK_DeveloperConsole), false);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        //Undo saving.
        Undo.RecordObject(owner, "Property changes on LPK_DeveloperConsole");

        //Component properties.
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Component Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_ConsoleCanvas, true);
        EditorGUILayout.PropertyField(m_ScrollRect, true);
        EditorGUILayout.PropertyField(m_ConsoleText, true);
        EditorGUILayout.PropertyField(m_InputText, true);
        EditorGUILayout.PropertyField(m_InputField, true);

        //Apply changes.
        serializedObject.ApplyModifiedProperties();
    }
}

#endif  //UNITY_EDITOR

}   //LPK_CONSOLE

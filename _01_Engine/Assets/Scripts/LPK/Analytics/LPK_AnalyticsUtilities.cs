/***************************************************
File:           LPK_AnalyticsUtilities.cs
Authors:        Christopher Onorati
Last Updated:   7/30/2019
Last Version:   2018.3.14

Description:
  Stores core logic for the analytics recording system
  the LPK uses.  Derrives from LPK_Component to take
  advantage of the event system for classes that derrive
  from LPK_AnalyticsBase below.

This script is a basic and generic implementation of its 
functionality. It is designed for educational purposes and 
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

using UnityEngine;
using System.IO; /* File IO */

namespace LPK
{

/**
* CLASS NAME  : LPK_AnalyticsBase
* DESCRIPTION : Base object for LPK analytics recording.
**/
public class LPK_AnalyticsBase : LPK_Component
{
    public string m_sFileName;
    public string m_sDirectoryPath = "Analytics";
    public bool m_bPrintFileMessages = true;

    /************************************************************************************/

    //Path to the file.
    string m_sFilePath;

    //Path to the directory.
    string m_sCombinedDriectoryPath;

    /**
    * FUNCTION NAME: OnStart
    * DESCRIPTION  : Creates all necessary files and directories needed for analytics logging.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    protected void OnStart()
    {
        if (string.IsNullOrEmpty(m_sFileName))
        {
            if (m_bPrintDebug)
                LPK_PrintError(this, "No file name given to analytics recorder.");
            return;
        }

        if (string.IsNullOrEmpty(m_sDirectoryPath))
        {
            if (m_bPrintDebug)
                LPK_PrintError(this, "No drectory path given to analytics recorder.");
            return;
        }

        m_sFilePath = Application.dataPath + "/" + m_sDirectoryPath + "/" + m_sFileName;
        m_sCombinedDriectoryPath = Application.dataPath + "/" + m_sDirectoryPath;

        CreateLogPath();
        CreateLogFile();

        if(m_bPrintFileMessages)
            PrintLogMessage("Analytics recording starting at: ", true, 3);
    }

    /**
    * FUNCTION NAME: CreateLogPath
    * DESCRIPTION  : Creates the log directory for analytics information.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CreateLogPath()
    {
        if (!Directory.Exists(m_sCombinedDriectoryPath))
            Directory.CreateDirectory(m_sCombinedDriectoryPath);

        //Emergency stop.
        if(!Directory.Exists(m_sCombinedDriectoryPath))
        {
            LPK_PrintError(this, "Attempted to create analytics directory but there is no space on hard drive!  Game is now closing.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

    }

    /**
    * FUNCTION NAME: CreateLogFile
    * DESCRIPTION  : Creates the log file for analytics information.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void CreateLogFile()
    {
        if (!File.Exists(m_sFilePath))
            File.AppendAllText(m_sFilePath, "");

        //Emergency stop.
        if (!File.Exists(m_sFilePath))
        {
            LPK_PrintError(this, "Attempted to create analytics file but there is no space on hard drive!  Game is now closing.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }

    /**
    * FUNCTION NAME: PrintLogMessage
    * DESCRIPTION  : Print log messages to analytics data file.
    * INPUTS       : _text             - Text message to add into the file.
    *                _printTime        - True/false to print time values next to message.
    *                _numberOfNewLines - Number of new lines to print at the end of the message.
    * OUTPUTS      : None
    **/
    protected void PrintLogMessage(string _text, bool _printTime = true, int _numberOfNewLines = 1)
    {
        if(!string.IsNullOrEmpty(_text))
            File.AppendAllText(m_sFilePath, _text);

        if(_printTime && (!string.IsNullOrEmpty(_text)))
        {
            File.AppendAllText(m_sFilePath, " | System Time: " + System.DateTime.Now.ToString());
            File.AppendAllText(m_sFilePath, " | Game Time: " + Time.realtimeSinceStartup.ToString());
        }

        for (int i = 0; i < _numberOfNewLines; i++)
            File.AppendAllText(m_sFilePath, "\n");
    }

    /**
    * FUNCTION NAME: OnApplicationQuit
    * DESCRIPTION  : Print end of session information.
    * INPUTS       : None
    * OUTPUTS      : None
    **/
    void OnApplicationQuit()
    {
        //Cannot write to file if the file does not exist.
        if (string.IsNullOrEmpty(m_sFileName) || string.IsNullOrEmpty(m_sDirectoryPath) || !File.Exists(m_sFilePath) || !Directory.Exists(m_sCombinedDriectoryPath))
            return;

        if (m_bPrintFileMessages)
        {
            PrintLogMessage("", true, 2);
            PrintLogMessage("Analytics recording ending at: ", true, 3);
        }
    }
}

}   //LPK

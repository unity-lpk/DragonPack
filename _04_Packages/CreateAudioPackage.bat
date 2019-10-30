@echo off
::open unity, create project, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -createProject ".\exporter" -quit
::move copy of package files into project path
    ::sound
xcopy "..\_01_Engine\Assets\Sound" ".\exporter\Assets\Sound" /S /Q /Y /K /I
::open unity, go to project, export packages, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -projectPath ".\exporter" -exportPackage "Assets" "../Unity_LPK_Audio.unitypackage" -quit
::delete temp project
rmdir ".\exporter" /Q /S
:: (:
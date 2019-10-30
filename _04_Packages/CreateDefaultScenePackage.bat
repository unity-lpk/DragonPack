@echo off
::open unity, create project, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -createProject ".\exporter" -quit
::move copy of package files into project path
    ::sound
xcopy "..\_01_Engine\Assets\Scenes" ".\exporter\Assets\Scenes" /S /Q /Y /K /I
::open unity, go to project, export packages, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -projectPath ".\exporter" -exportPackage "Assets" "../Unity_LPK_DefaultScene.unitypackage" -quit
::delete temp project
rmdir ".\exporter" /Q /S
:: (:
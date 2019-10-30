@echo off
::open unity, create project, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -createProject ".\exporter" -quit
::move copy of package files into project path
    ::plugins
xcopy "..\_01_Engine\Assets\Plugins" ".\exporter\Assets\Plugins" /S /Q /Y /K /I
    ::shaders
xcopy "..\_01_Engine\Assets\Shaders" ".\exporter\Assets\Shaders" /S /Q /Y /K /I
    ::resources
xcopy "..\_01_Engine\Assets\Resources" ".\exporter\Assets\Resources" /S /Q /Y /K /I
    ::textures
xcopy "..\_01_Engine\Assets\Textures" ".\exporter\Assets\Textures" /S /Q /Y /K /I
    ::scripts
xcopy "..\_01_Engine\Assets\Scripts" ".\exporter\Assets\Scripts" /S /Q /Y /K /I
::open unity, go to project, export packages, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -projectPath ".\exporter" -exportPackage "Assets" "../Unity_LPK_Code.unitypackage" -quit
::delete temp project
rmdir ".\exporter" /Q /S
:: (:
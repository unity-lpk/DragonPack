@echo off
::open unity, create project, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -createProject ".\exporter" -quit
::move copy of package files into project path
    ::materials
xcopy "..\_01_Engine\Assets\Materials" ".\exporter\Assets\Materials" /S /Q /Y /K /I
    ::particles
xcopy "..\_01_Engine\Assets\Particles" ".\exporter\Assets\Particles" /S /Q /Y /K /I
    ::sprites
xcopy "..\_01_Engine\Assets\Sprites" ".\exporter\Assets\Sprites" /S /Q /Y /K /I
    ::tile_palletes
xcopy "..\_01_Engine\Assets\Tile_Palletes" ".\exporter\Assets\Tile_Palletes" /S /Q /Y /K /I
    ::fonts
xcopy "..\_01_Engine\Assets\Fonts" ".\exporter\Assets\Fonts" /S /Q /Y /K /I
::open unity, go to project, export packages, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile ".\logfile.txt" -projectPath ".\exporter" -exportPackage "Assets" "../Unity_LPK_Graphics.unitypackage" -quit
::delete temp project
rmdir ".\exporter" /Q /S
:: (:
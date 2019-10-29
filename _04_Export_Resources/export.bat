@echo off
::open unity, create project, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\logfile.txt" -createProject "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter" -quit
::move copy of package files into project path
    ::plugins
xcopy "C:\Users\aeon.williams\Documents\DragonPack\_01_Engine\Assets\Plugins" "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter\Assets\Plugins" /S /Q /Y /K /I
    ::shaders
xcopy "C:\Users\aeon.williams\Documents\DragonPack\_01_Engine\Assets\Shaders" "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter\Assets\Shaders" /S /Q /Y /K /I
    ::resources
xcopy "C:\Users\aeon.williams\Documents\DragonPack\_01_Engine\Assets\Resources" "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter\Assets\Resources" /S /Q /Y /K /I
    ::textures
xcopy "C:\Users\aeon.williams\Documents\DragonPack\_01_Engine\Assets\Textures" "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter\Assets\Textures" /S /Q /Y /K /I
    ::scripts
xcopy "C:\Users\aeon.williams\Documents\DragonPack\_01_Engine\Assets\Scripts" "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter\Assets\Scripts" /S /Q /Y /K /I
::open unity, go to project, export packages, close unity
"C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Unity.exe" -batchmode -logFile "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\logfile.txt" -projectPath "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter" -exportPackage "Assets" "../test.unitypackage" -quit
::delete temp project
rmdir "C:\Users\aeon.williams\Documents\DragonPack\_04_Export_Resources\exporter" /Q /S
:: (:
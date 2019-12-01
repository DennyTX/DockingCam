
@echo off

set H=%KSPDIR%
set GAMEDIR=DockingCamKURS

echo %H%

copy /Y "%1%2" "GameData\%GAMEDIR%\Plugins"
copy /Y DockingCamera.version GameData\%GAMEDIR%

mkdir "%H%\GameData\%GAMEDIR%"
xcopy  /E /y GameData\%GAMEDIR% "%H%\GameData\%GAMEDIR%"



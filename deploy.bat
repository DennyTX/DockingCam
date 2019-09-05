
@echo off

set H=R:\KSP_1.7.3_dev
set GAMEDIR=DockingCamKURS

echo %H%

copy /Y "%1%2" "GameData\%GAMEDIR%\Plugins"
copy /Y DockingCamera.version GameData\%GAMEDIR%

mkdir "%H%\GameData\%GAMEDIR%"
xcopy  /E /y GameData\%GAMEDIR% "%H%\GameData\%GAMEDIR%"



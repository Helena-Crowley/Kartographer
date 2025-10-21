@echo off
REM Move up from Unity/Kartographer to the Unity project root
pushd "%~dp0"

REM Go back to Unity project folder (the current folder)
SET PROJECT_PATH=%CD%

REM Path to Unity executable (relative or from PATH)
SET UNITY_EXE=C:\Program Files\Unity\Hub\Editor\6000.2.6f2\Editor\Unity.exe

REM Run Unity batch mode
"%UNITY_EXE%" -batchmode -quit -projectPath "%PROJECT_PATH%" -executeMethod VersionIncrementer.IncrementBuildNumber -logFile -

echo Version increment process finished.
popd
exit /b 0

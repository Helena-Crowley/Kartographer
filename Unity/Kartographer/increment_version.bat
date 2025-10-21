@echo off
REM No quotes here
SET UNITY_EXE=C:\Program Files\Unity\Hub\Editor\6000.2.6f2\Editor\Unity.exe
SET PROJECT_PATH=%CD%

REM Quote it when calling
"%UNITY_EXE%" -batchmode -quit -projectPath "%PROJECT_PATH%" -executeMethod VersionIncrementer.IncrementBuildNumber -logFile -

echo Unity finished running.
pause


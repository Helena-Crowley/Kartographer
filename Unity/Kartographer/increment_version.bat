@echo off
SET UNITY_EXE="C:\Program Files\Unity\Hub\Editor\6000.2.6f2\Editor\Unity.exe"
SET PROJECT_PATH=%CD%
%UNITY_EXE% -batchmode -quit -projectPath "%PROJECT_PATH%" -executeMethod VersionIncrementer.IncrementBuildNumber

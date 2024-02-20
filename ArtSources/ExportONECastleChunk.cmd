@echo off

cd ExportScripts
call config.cmd

set object_name=%1

if "%object_name%"=="" (
    echo Error: Please provide an object name as a command line argument.
    exit /b 1
)

call export_fbx_object.cmd ../CastleChunks/CastleChunks.blend %object_name% CastleGenerator/T1.Omino/FBX/CastleChunks/%object_name%.fbx

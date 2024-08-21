@echo off

cd ExportScripts
call config.cmd

call export_fbx_object.cmd ../Room/bedroom.blend bedroom Bedroom/bedroom.fbx
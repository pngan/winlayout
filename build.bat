@echo off
cd winLayout
dotnet build
dotnet publish -c Release
copy bin\Release\net7.0\win-x64\publish\winLayout.exe ..
cd ..\winlayout-ui\winlayout-ui
dotnet build
dotnet publish -c Release /p:IncludeNativeLibrariesForSelfExtract=true
copy bin\Release\net7.0-windows\win-x64\publish\winlayout-ui.exe ..\..

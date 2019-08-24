# winlayout

A utility to save and restore window positions on a Windows Desktop. 

## Usage

With the laptop running on the docking station, and application windows in their desired locations, run the command:

`winlayout save`

Then the next time the laptop is plugged into the docking station, the window locations of the running applications can be restored by the command:

`winlayout restore`

For convenience, make a windows short cut to the file winlayout.exe, and in the properties of the short, set the short cut keys. That way the save and restore can be activated by keyboard shortcuts.

## Building the .exe

Install `dotnet core 3 SDK` onto the developer machine.
In the folder, in a cmd shell, run the commands

`cd <folder containing winlayout.csproj>`

`dotnet build`

`dotnet publish -r win10-x64 -c Release /p:PublishSingleFile=true`




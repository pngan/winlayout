# winlayout

A utility to save and restore window positions on a Windows Desktop. 

## Usage

With the laptop running on the docking station, and application windows in their desired locations, run the command:

`winlayout save`

Then the next time the laptop is plugged into the docking station, the window locations of the running applications can be restored by the command:

`winlayout restore`

For convenience, make a windows short cut to the file winlayout.exe (with the desired parameter) and pin to the task bar for easy access.

## Installation

Copy the file `winLayout.exe` onto your computur.

## Building the .exe

Install `.Net 5 SDK` onto the developer machine.
In the folder, in a cmd shell, run the commands

`cd <folder containing winlayout.csproj>`

`dotnet build`

`dotnet publish -r win10-x64 -c Release /p:PublishSingleFile=true`

## Tips

If some windows do not change position when a restore is performed, it might be that these windows are running under Administrator privilege. In this case, run `winlayout restore` as Administrator and they will move. Visual Studio is commonly affected in this way.


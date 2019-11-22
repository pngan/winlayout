dotnet build
dotnet publish -r win10-x64 -c Release /p:PublishSingleFile=true
copy bin\Release\netcoreapp3.0\win10-x64\publish\winLayout.exe ..
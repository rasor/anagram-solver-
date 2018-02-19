
REM https://stackoverflow.com/questions/44074121/build-net-core-console-application-to-output-an-exe/44074296
dotnet publish -c Release -r win10-x64
start .\bin\Release\netcoreapp2.0\win10-x64

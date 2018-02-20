REM ..\Tools\nuget pack ..\src\MyIP.Client\MyIP.Client.csproj -Build -Prop Configuration=Release;
dotnet pack ..\src\MyIP.Client\MyIP.Client.csproj -c Release /p:NuspecFile=MyIP.Client.nuspec
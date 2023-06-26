REM Publishes the application and its dependencies to a folder for deployment to a hosting system.
dotnet publish ../src/service/PViMS.API/PViMS.API.csproj -o api -v normal
PAUSE
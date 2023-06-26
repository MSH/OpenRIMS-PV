REM Clear existing folder
del app\*.* /s /q

REM Recopy config file
XCOPY web.config app

REM Builds an angular application and publishes it to the dist folder
cd ..\src\web\PViMS.Spa
node --max-old-space-size=8192 node_modules/@angular/cli/bin/ng build --configuration=production --verbose=true

REM Copy dist folder into build
XCOPY dist\*.* ..\..\..\build\app\*.* /s /y
PAUSE
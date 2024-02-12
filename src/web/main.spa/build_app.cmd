REM Builds an angular application and publishes it to the dist folder
node --max-old-space-size=8192 node_modules/@angular/cli/bin/ng build --configuration=production --verbose=true
PAUSE
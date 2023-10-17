rem @echo off
set folderchoice=
set /p folderchoice=Enter plugin name: 
mkdir %folderchoice%
cd %folderchoice%
mkdir dialogs
mkdir views
mkdir styles
mkdir controllers
@echo { > package.manifest
@echo    propertyEditors: [{ >> package.manifest
@echo      alias: "%folderchoice%", >> package.manifest
@echo      name: "%folderchoice%", >> package.manifest
@echo      editor: { >> package.manifest
@echo        view: "~/App_Plugins/%folderchoice%/views/default.html", >> package.manifest
@echo        hideLabel: false >> package.manifest
@echo      } >> package.manifest
@echo   }], >> package.manifest
@echo   javascript: [ >> package.manifest
@echo     "~/App_Plugins/%folderchoice%/controllers/%folderchoice%.controller.js" >> package.manifest
@echo   ], >> package.manifest
@echo   css: [ >> package.manifest
@echo		"~/App_Plugins/%folderchoice%/styles/styles.css" >> package.manifest
@echo 	]  >> package.manifest
@echo } >> package.manifest

cd .\views\
@echo ^<div class="LukicPropertyEditors" ng-controller="%folderchoice%.Controller"^> > default.html
@echo ^<!-- HTML --^> >> default.html
@echo ^</div^> >> default.html
cd..

cd controllers
@echo angular.module("umbraco").controller("%folderchoice%.Controller", function ($scope, $http, editorState, dialogService, notificationsService) { > %folderchoice%.controller.js
@echo // %folderchoice% Controller >> %folderchoice%.controller.js
@echo }); >> %folderchoice%.controller.js
cd..

cd styles
@echo .body {} > styles.css
cd ..
 
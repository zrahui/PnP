#Remove-SPONavigationNode
*Topic automatically generated on: 2015-04-29*

Removes a menu item from either the quicklaunch or top navigation
##Syntax
```powershell
Remove-SPONavigationNode -Location <NavigationType> -Title <String> [-Header <String>] [-Force [<SwitchParameter>]] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Header|String|False|
Location|NavigationType|True|
Title|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: 29E741B72A46B07F532B080ED20E6607 -->
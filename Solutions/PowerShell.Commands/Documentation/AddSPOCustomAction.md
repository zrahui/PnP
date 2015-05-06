#Add-SPOCustomAction
*Topic automatically generated on: 2015-04-29*

Adds a custom action to a web
##Syntax
```powershell
Add-SPOCustomAction -Title <String> -Description <String> -Group <String> -Location <String> -Sequence <Int32> -Url <String> [-Rights <List`1>] [-Scope <CustomActionScope>] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Description|String|True|
Group|String|True|
Location|String|True|
Rights|List`1|False|
Scope|CustomActionScope|False|
Sequence|Int32|True|
Title|String|True|
Url|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: BBFCCFAAA4B149B035D180DB382C3BBD -->
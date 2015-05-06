#Add-SPOJavaScriptBlock
*Topic automatically generated on: 2015-04-29*

Adds a link to a JavaScript snippet/block to a web or site collection
##Syntax
```powershell
Add-SPOJavaScriptBlock -Name <String> -Script <String> [-Sequence <Int32>] [-Scope <CustomActionScope>] [-Web <WebPipeBind>]```
&nbsp;

##Detailed Description
Specify a scope as 'Site' to add the custom action to all sites in a site collection.

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Name|String|True|
Scope|CustomActionScope|False|
Script|String|True|
Sequence|Int32|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: 5BE6F2CC2BFD25AFB6B0308F4FFB4E7A -->
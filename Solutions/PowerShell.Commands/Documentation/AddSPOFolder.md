#Add-SPOFolder
*Topic automatically generated on: 2015-04-29*

Creates a folder within a parent folder
##Syntax
```powershell
Add-SPOFolder -Name <String> -Folder <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Folder|String|True|The parent folder in the site
Name|String|True|The folder name
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:> Add-SPOFolder -Name NewFolder -Folder _catalogs/masterpage/newfolder

<!-- Ref: C30D43360D9C5333A858B12F1C4ADC0D -->
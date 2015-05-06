#Get-SPOFile
*Topic automatically generated on: 2015-05-04*

Downloads a file.
##Syntax
```powershell
Get-SPOFile -AsString [<SwitchParameter>] [-Web <WebPipeBind>] -ServerRelativeUrl <String>```
&nbsp;

```powershell
Get-SPOFile [-Path <String>] [-Filename <String>] [-Web <WebPipeBind>] -ServerRelativeUrl <String>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AsString|SwitchParameter|True|
Filename|String|False|
Path|String|False|
ServerRelativeUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:> Get-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor
Downloads the file and saves it to the current folder

###Example 2
    
PS:> Get-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor -Path c:\temp -FileName company.spcolor
Downloads the file and saves it to c:\temp\company.spcolor

###Example 3
    
PS:> Get-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor -AsString
Downloads the file and outputs its contents to the console
<!-- Ref: 82583DE5570ADD78040D924F662E437E -->
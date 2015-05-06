#Add-SPOTaxonomyField
*Topic automatically generated on: 2015-04-29*

Adds a taxonomy field to a list or as a site column.
##Syntax
```powershell
Add-SPOTaxonomyField [-List <ListPipeBind>] -DisplayName <String> -InternalName <String> -TermSetPath <String> [-TermPathDelimiter <String>] [-Group <String>] [-Id <GuidPipeBind>] [-AddToDefaultView [<SwitchParameter>]] [-MultiValue [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-FieldOptions <AddFieldOptions>] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AddToDefaultView|SwitchParameter|False|
DisplayName|String|True|
FieldOptions|AddFieldOptions|False|
Group|String|False|
Id|GuidPipeBind|False|
InternalName|String|True|
List|ListPipeBind|False|
MultiValue|SwitchParameter|False|
Required|SwitchParameter|False|
TermPathDelimiter|String|False|
TermSetPath|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: 67877B591ED1668F2BFADBE98AF418A4 -->
#Uninstall-SPOSolution
*Topic automatically generated on: 2015-04-29*

Uninstalls a sandboxed solution from a site collection
##Syntax
```powershell
Uninstall-SPOSolution -PackageId <GuidPipeBind> -PackageName <String> [-MajorVersion <Int32>] [-MinorVersion <Int32>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
MajorVersion|Int32|False|Optional major version of the solution, defaults to 1
MinorVersion|Int32|False|Optional minor version of the solution, defaults to 0
PackageId|GuidPipeBind|True|ID of the solution, from the solution manifest
PackageName|String|True|Filename of the WSP file to uninstall
<!-- Ref: C583A84540363A809C04EEE05A00BDDB -->
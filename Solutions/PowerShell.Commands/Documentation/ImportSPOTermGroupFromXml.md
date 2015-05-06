#Import-SPOTermGroupFromXml
*Topic automatically generated on: 2015-05-04*

Imports a taxonomy TermGroup from either the input or from an XML file.
##Syntax
```powershell
Import-SPOTermGroupFromXml [-Xml <String>]```
&nbsp;

```powershell
Import-SPOTermGroupFromXml [-Path <String>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Path|String|False|The XML File to import the data from
Xml|String|False|The XML to process
##Examples

###Example 1
    PS:> Import-SPOTermGroupFromXml -Path input.xml
Imports the XML file specified by the path.

###Example 2
    PS:> Import-SPOTermGroupFromXml -Xml $xml
Imports the XML based termgroup information located in the $xml variable
<!-- Ref: 5DAE1B1A0566CB2DE927F2A4AE8B01A1 -->
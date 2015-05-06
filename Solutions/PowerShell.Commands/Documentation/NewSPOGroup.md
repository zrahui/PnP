#New-SPOGroup
*Topic automatically generated on: 2015-04-29*

Adds a user to the build-in Site User Info List and returns a user object
##Syntax
```powershell
New-SPOGroup -Title <String> [-Description <String>] [-Owner <String>] [-AllowRequestToJoinLeave [<SwitchParameter>]] [-AutoAcceptRequestToJoinLeave [<SwitchParameter>]] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AllowRequestToJoinLeave|SwitchParameter|False|
AutoAcceptRequestToJoinLeave|SwitchParameter|False|
Description|String|False|
Owner|String|False|
Title|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:> New-SPOUser -LogonName user@company.com


<!-- Ref: 7BCB2FB9C84580F9542F731CF7087F63 -->
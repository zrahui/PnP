#Add-SPOFile
*Topic automatically generated on: 2015-04-29*

Uploads a file to Web
##Syntax
```powershell
Add-SPOFile -Path <String> -Folder <String> [-Checkout [<SwitchParameter>]] [-Approve [<SwitchParameter>]] [-ApproveComment <String>] [-Publish [<SwitchParameter>]] [-PublishComment <String>] [-UseWebDav [<SwitchParameter>]] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Approve|SwitchParameter|False|Will auto approve the uploaded file.
ApproveComment|String|False|The comment added to the approval.
Checkout|SwitchParameter|False|If versioning is enabled, this will check out the file first if it exists, upload the file, then check it in again.
Folder|String|True|The destination folder in the site
Path|String|True|The local file path.
Publish|SwitchParameter|False|Will auto publish the file.
PublishComment|String|False|The comment added to the publish action.
UseWebDav|SwitchParameter|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:> Add-SPOFile -Path c:\temp\company.master -Url /sites/

<!-- Ref: 5760EA695501C4F78DBF9CECE83BC038 -->
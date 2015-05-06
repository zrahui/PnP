﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Remove, "SPOUserFromGroup")]
    [CmdletHelp("Removes a user from a group", Category = "User and group management")]
    [CmdletExample(Code = @"
PS:> Remove-SPOUserFromGroup -LoginName user@company.com -GroupName 'Marketing Site Members'
")]
    public class RemoveUserFromGroup : SPOWebCmdlet
    {

        [Parameter(Mandatory = true, HelpMessage = "A valid logon name of a user")]
        [Alias("LogonName")]
        public string LoginName = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "A valid group name")]
        public string GroupName = string.Empty;

        protected override void ExecuteCmdlet()
        {
            SelectedWeb.RemoveUserFromGroup(GroupName, LoginName);
        }
    }
}

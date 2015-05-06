﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Add, "SPOUserToGroup")]
    [CmdletHelp("Adds a user to a group", Category = "User and group management")]
    [CmdletExample(Code = @"
    PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 'Marketing Site Members'
    ")]
    [CmdletExample(Code = @"
    PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 5
    ", Remarks = "Add the specified user to the group with Id 5")]
    public class AddUserToGroup : SPOWebCmdlet
    {

        [Parameter(Mandatory = true, HelpMessage = "The login name of the user")]
        [Alias("LogonName")]
        public string LoginName;

        [Parameter(Mandatory = true, HelpMessage = "The group id, group name or group object to add the user to.", ValueFromPipeline = true)]
        public GroupPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Id != -1)
            {
                SelectedWeb.AddUserToGroup(Identity.Id, LoginName);
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                SelectedWeb.AddUserToGroup(Identity.Name, LoginName);
            }
            else if (Identity.Group != null)
            {
                SelectedWeb.AddUserToGroup(Identity.Group, LoginName);
            }
        }
    }
}

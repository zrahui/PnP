﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOSubWebs")]
    [CmdletHelp("Returns the subwebs", Category = "Webs")]
    public class GetSubWebs : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline=true, Position=0)]
        public WebPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var webs = ClientContext.LoadQuery(SelectedWeb.Webs);
            ClientContext.ExecuteQueryRetry();
            WriteObject(webs, true);

        }

    }
}

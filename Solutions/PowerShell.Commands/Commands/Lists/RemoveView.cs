﻿using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOView", SupportsShouldProcess = true)]
    [CmdletHelp("Deletes a view from a list", Category = "Lists")]
    public class RemoveView : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Title of the list.")]
        public ViewPipeBind Identity = new ViewPipeBind();

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 1, HelpMessage = "The ID or Url of the list.")]
        public ListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (List != null)
            {
                var list = SelectedWeb.GetList(List);

                if (list != null)
                {
                    View view = null;
                    if (Identity != null)
                    {
                        if (Identity.Id != Guid.Empty)
                        {
                            view = list.GetViewById(Identity.Id);
                        }
                        else if (!string.IsNullOrEmpty(Identity.Title))
                        {
                            view = list.GetViewByName(Identity.Title);
                        }
                        else if (Identity.View != null)
                        {
                            view = Identity.View;
                        }
                        if (view != null)
                        {
                            if (Force || ShouldContinue(string.Format(Properties.Resources.RemoveView0, view.Title), Properties.Resources.Confirm))
                            {
                                view.DeleteObject();
                                ClientContext.ExecuteQueryRetry();
                            }
                        }
                    }
                }
            }
        }
    }

}

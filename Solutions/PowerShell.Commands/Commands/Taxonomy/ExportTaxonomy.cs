﻿using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using File = System.IO.File;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Export, "SPOTaxonomy", SupportsShouldProcess = true)]
    [CmdletHelp("Exports a taxonomy to either the output or to a file.", Category = "Taxonomy")]
    [CmdletExample(Code = @"PS:> Export-SPOTaxonomy", Remarks = "Exports the full taxonomy to the standard output")]
    [CmdletExample(Code = @"PS:> Export-SPOTaxonomy -Path c:\output.txt", Remarks = "Exports the full taxonomy the file output.txt")]
    [CmdletExample(Code = @"PS:> Export-SPOTaxonomy -Path c:\output.txt -TermSet f6f43025-7242-4f7a-b739-41fa32847254 ", Remarks = "Exports the term set with the specified id")]
    public class ExportTaxonomy : SPOCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "TermSet", HelpMessage = "If specified, will export the specified termset only")]
        public GuidPipeBind TermSetId = new GuidPipeBind();

        [Parameter(Mandatory = false, HelpMessage = "If specified will include the ids of the taxonomy items in the output. Format: <label>;#<guid>")]
        public SwitchParameter IncludeID = false;

        [Parameter(Mandatory = false, HelpMessage = "File to export the data to.")]
        public string Path;

        [Parameter(Mandatory = false, ParameterSetName = "TermSet")]
        public string TermStoreName;

        [Parameter(Mandatory = false, HelpMessage = "Overwrites the output file if it exists.")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false)]
        public string Delimiter = "|";


        protected override void ExecuteCmdlet()
        {
            List<string> exportedTerms;
            if (ParameterSetName == "TermSet")
            {
                if (Delimiter != "|" && Delimiter == ";#")
                {
                    throw new Exception("Restricted delimiter specified");
                }
                if (!string.IsNullOrEmpty(TermStoreName))
                {
                    var taxSession = TaxonomySession.GetTaxonomySession(ClientContext);
                    var termStore = taxSession.TermStores.GetByName(TermStoreName);
                    exportedTerms = ClientContext.Site.ExportTermSet(TermSetId.Id, IncludeID, termStore, Delimiter);
                }
                else
                {
                    exportedTerms = ClientContext.Site.ExportTermSet(TermSetId.Id, IncludeID, Delimiter);
                }
            }
            else
            {
                exportedTerms = ClientContext.Site.ExportAllTerms(IncludeID, Delimiter);
            }

            if (Path == null)
            {
                WriteObject(exportedTerms);
            }
            else
            {
                if (!System.IO.Path.IsPathRooted(Path))
                {
                    Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
                }

                if (File.Exists(Path))
                {
                    if (Force || ShouldContinue(string.Format(Resources.File0ExistsOverwrite, Path), Resources.Confirm))
                    {
                        File.WriteAllLines(Path, exportedTerms);
                    }
                }
                else
                {
                    File.WriteAllLines(Path, exportedTerms);
                }
            }
        }

    }
}

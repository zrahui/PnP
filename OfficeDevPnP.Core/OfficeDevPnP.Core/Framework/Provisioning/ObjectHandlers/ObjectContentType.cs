﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectContentType : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Content Types"; }
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_ContentTypes);

            // if this is a sub site then we're not provisioning content types. Technically this can be done but it's not a recommended practice
            if (web.IsSubSite())
            {
                return;
            }

            web.Context.Load(web.ContentTypes, ct => ct.Include(c => c.StringId));
            web.Context.ExecuteQueryRetry();
            var existingCTs = web.ContentTypes.ToList();

            foreach (var ct in template.ContentTypes.OrderBy(ct => ct.Id)) // ordering to handle references to parent content types that can be in the same template
            {
                var existingCT = existingCTs.FirstOrDefault(c => c.StringId.Equals(ct.Id, StringComparison.OrdinalIgnoreCase));
                if (existingCT == null)
                {
                    var newCT = CreateContentType(web, ct);
                    if (newCT != null)
                    {
                        existingCTs.Add(newCT);
                    }

                }
                else
                {
                    if (ct.Overwrite)
                    {
                        existingCT.DeleteObject();
                        web.Context.ExecuteQueryRetry();
                        var newCT = CreateContentType(web, ct);
                        if (newCT != null)
                        {
                            existingCTs.Add(newCT);
                        }
                    }
                }
            }

        }

        private static Microsoft.SharePoint.Client.ContentType CreateContentType(Web web, ContentType ct)
        {
            var name = ct.Name.ToParsedString();
            var description = ct.Description.ToParsedString();
            var id = ct.Id.ToParsedString();
            var group = ct.Group.ToParsedString();

            var createdCT = web.CreateContentType(name, description, id, group);
            foreach (var fieldRef in ct.FieldRefs)
            {
                var field = web.Fields.GetById(fieldRef.Id);
                web.AddFieldToContentType(createdCT, field, fieldRef.Required, fieldRef.Hidden);
            }

            createdCT.ReadOnly = ct.ReadOnly;
            createdCT.Hidden = ct.Hidden;
            createdCT.Sealed = ct.Sealed;
            if (!string.IsNullOrEmpty(ct.DocumentTemplate))
            {
                createdCT.DocumentTemplate = ct.DocumentTemplate;
            }

            web.Context.Load(createdCT);
            web.Context.ExecuteQueryRetry();

            return createdCT;
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // if this is a sub site then we're not creating content type entities. 
            if (web.IsSubSite())
            {
                return template;
            }

            var cts = web.ContentTypes;
            web.Context.Load(cts, ctCollection => ctCollection.IncludeWithDefaultProperties(ct => ct.FieldLinks));
            web.Context.ExecuteQueryRetry();

            foreach (var ct in cts)
            {
                if (!BuiltInContentTypeId.Contains(ct.StringId))
                {
                    //   template.ContentTypes.Add(new ContentType() { SchemaXml = ct.SchemaXml });
                    template.ContentTypes.Add(new ContentType
                        (ct.StringId,
                        ct.Name,
                        ct.Description,
                        ct.Group,
                        ct.Sealed,
                        ct.Hidden,
                        ct.ReadOnly,
                        ct.DocumentTemplate,
                        false,
                            (from fieldLink in ct.FieldLinks.AsEnumerable<FieldLink>()
                             select new FieldRef(fieldLink.Name)
                             {
                                 Id = fieldLink.Id,
                                 Hidden = fieldLink.Hidden,
                                 Required = fieldLink.Required,
                             })
                        ));
                }
            }

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            foreach (var ct in baseTemplate.ContentTypes)
            {
                var index = template.ContentTypes.FindIndex(f => f.Id.Equals(ct.Id, StringComparison.OrdinalIgnoreCase));
                if (index > -1)
                {
                    template.ContentTypes.RemoveAt(index);
                }

            }

            return template;
        }
    }
}

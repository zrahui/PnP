﻿using Provisioning.Common.Data.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class TemplateResultResponse
    {
        #region Public Members
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "templates")]
        public ICollection<Template> Templates
        {
            get;
            set;
        }
        #endregion
    }
}
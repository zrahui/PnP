﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class PlayWithUsersViewModel
    {
        [Required(ErrorMessage = "UPN is a required field!")]
        [DisplayName("User Principal Name")]
        public String UserPrincipalName { get; set; }

        [Required(ErrorMessage = "Mail To is a required field!")]
        [DisplayName("Mail To")]
        public String MailSendTo { get; set; }

        [Required(ErrorMessage = "Mail To Description is a required field!")]
        [DisplayName("Mail To Description")]
        public String MailSendToDescription { get; set; }
    }
}
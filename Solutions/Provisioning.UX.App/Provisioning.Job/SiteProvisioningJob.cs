﻿using Microsoft.SharePoint.Client;
using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Data;
using Provisioning.Common.Data.SiteRequests;
using Provisioning.Common.Data.Templates;
using Provisioning.Common.Mail;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Job
{
    public class SiteProvisioningJob
    {
        #region Instance Members
        ISiteRequestFactory _requestFactory;
        IConfigurationFactory _configFactory;
        ISiteTemplateFactory _siteTemplateFactory;
        IAppSettingsManager _appManager;
        AppSettings _settings;
        
        #endregion

        #region Constructors
        public SiteProvisioningJob()
        {
            this._configFactory = ConfigurationFactory.GetInstance();
            this._appManager = _configFactory.GetAppSetingsManager();
            this._settings = _appManager.GetAppSettings();
            this._requestFactory = SiteRequestFactory.GetInstance();
            this._siteTemplateFactory = SiteTemplateFactory.GetInstance();
        }
        #endregion

        public void ProcessSiteRequests()
        {
            #region Process Approved Requests
            // Begin processing of approved requests
            Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "Beginning Processing the site request repository");
            var _siteManager = _requestFactory.GetSiteRequestManager();
            var _requests = _siteManager.GetApprovedRequests();
            Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "There are {0} site requests pending in the repository.", _requests.Count);
            if(_requests.Count > 0)
            {
                this.ProvisionSites(_requests);
            }
            else
            {
               Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "There are no site requests pending in the repository");
            }
            // End processing of approved requests
            #endregion

            #region Process Failed or Incomplete Requests
            // Begin processing of failed requests (retry all that are not in approved or complete status)
            Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "Beginning processing of the site request repository for failed or incomplete requests");
            _siteManager = _requestFactory.GetSiteRequestManager();
            _requests = _siteManager.GetIncompleteRequests();
            Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "There are {0} failed site requests pending in the repository.", _requests.Count);
            if (_requests.Count > 0)
            {
                this.ProvisionSites(_requests);
            }
            else
            {
                Log.Info("Provisioning.Job.SiteProvisioningJob.ProcessSiteRequests", "There are no failed site requests pending in the repository");
            }
            // End processing of failed requests
            #endregion
        }

        /// <summary>
        /// Member to handle provisioning sites
        /// </summary>
        /// <param name="siteRequests">The site request</param>
        public void ProvisionSites(ICollection<SiteInformation> siteRequests)
        {
            var _tm = this._siteTemplateFactory.GetManager();
            var _requestManager = this._requestFactory.GetSiteRequestManager();

            foreach (var siteRequest in siteRequests)
            {
                try 
                {
                    // ****************************************************
                    // Step 1 - Get Template                   
                    // ****************************************************
                    var _template = _tm.GetTemplateByName(siteRequest.Template);              
                    if (_template == null)
                    {   
                        //NO TEMPLATE FOUND THAT MATCHES WE CANNOT PROVISION A SITE
                        var _message = string.Format("Template: {0} was not found for site {1}. Ensure that the template file exits.", siteRequest.Template, siteRequest.Url);
                        Log.Error("Provisioning.Job.SiteProvisioningJob.ProvisionSites", _message );
                        throw new ConfigurationErrorsException(_message);
                    }
                    
                    // ****************************************************
                    // Step 2 - Update request status                    
                    // ****************************************************
                    var _provisioningTemplate = _tm.GetProvisioningTemplate(_template.ProvisioningTemplate);

                    //NO TEMPLATE FOUND THAT MATCHES WE CANNOT PROVISION A SITE
                    if (_template == null)
                    {
                        Log.Warning("Provisioning.Job.SiteProvisioningJob.ProvisionSites", "Template {0} was not found for Site Url {1}.", siteRequest.Template, siteRequest.Url);
                    }

                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Processing);
                   
                    // ****************************************************
                    // Step 3 - Create the site                    
                    // ****************************************************
                    SiteProvisioningManager _siteProvisioningManager = new SiteProvisioningManager(siteRequest, _template);
                    Log.Info("Provisioning.Job.SiteProvisioningJob.ProvisionSites", "Provisioning Site Request for Site Url {0}.", siteRequest.Url);
                    _siteProvisioningManager.CreateSiteCollection(siteRequest, _template);

                    // FOR SUBSITE PROVISIONING TESTING ONLY
                    //_siteProvisioningManager.CreateSubSite(siteRequest, _template);

                    // ****************************************************
                    // Step 4 - Apply provisioning template                    
                    // ****************************************************
                    Log.Info("Provisioning.Job.SiteProvisioningJob.ProvisionSites", "Applying Provisioning Template for Site Url {0}.", siteRequest.Url);
                    _siteProvisioningManager.ApplyProvisioningTemplate(_provisioningTemplate, siteRequest, _template);
                    
                    // ****************************************************
                    // Step 5 - Update request access email                    
                    // ****************************************************
                    Log.Info("Provisioning.Job.SiteProvisioningJob.ProvisionSites", "Updating Request Access Email Address for Site Url {0}.", siteRequest.Url);
                    _siteProvisioningManager.UpdateRequestAccessEmail(siteRequest);

                    // ****************************************************
                    // Step 6 - Update site description                   
                    // ****************************************************
                    Log.Info("Provisioning.Job.SiteProvisioningJob.ProvisionSites", "Updating site description for Site Url {0}.", siteRequest.Url);
                    _siteProvisioningManager.UpdateSiteDescription(siteRequest);

                    // ****************************************************
                    // Step 7 - Send success email                    
                    // ****************************************************
                    Log.Info("Provisioning.Job.SiteProvisioningJob.ProvisionSites", "Sending Success Email for Site Url {0}.", siteRequest.Url);
                    this.SendSuccessEmail(siteRequest);
                    
                    // ****************************************************
                    // Step 8 - Set status to complete                    
                    // ****************************************************
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Complete, "");

                }
                catch(ProvisioningTemplateException _pte)
                {
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.CompleteWithErrors, _pte.Message);
                }
                catch(Exception _ex)
                {
                    Log.Error("Provisioning.Job.SiteProvisioningJob.ProvisionSites", _ex.ToString());
                    _requestManager.UpdateRequestStatus(siteRequest.Url, SiteRequestStatus.Exception, _ex.Message);
                  this.SendFailureEmail(siteRequest, _ex.Message, true);
                }
            }
        }

        protected string CheckReservedNamespaces(SiteInformation siteRequest)
        {
            string returnValue = string.Empty;
            string siteUri = siteRequest.Url;
            //   string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            //   string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

            siteUri = siteUri.Replace("https://cocacola.sharepoint.com/teams", "https://teams.coca-cola.com/sites");
            siteUri = siteUri.Replace("https://cocacola.sharepoint.com/sites", "https://partner.coca-cola.com/sites");


            using (var ctx = new ClientContext("https://teams.coca-cola.com/sites/MTMigration"))
            {
                ctx.AuthenticationMode = ClientAuthenticationMode.Default;

                System.Net.NetworkCredential cred = new System.Net.NetworkCredential("na\\X30965", "Friday$123456789");
                ctx.Credentials = cred;
                List oList = ctx.Web.Lists.GetByTitle("SiteInventory");

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><And><Eq><FieldRef Name='Title' /><Value Type='Text'>" + siteUri.Trim() + "</Value></Eq><Eq><FieldRef Name='MigrateSite' /><Value Type='Boolean'>1</Value></Eq></And></Where></Query><RowLimit>1</RowLimit></View>";

                ListItemCollection collListItem = oList.GetItems(camlQuery);
                ctx.Load(collListItem);
                ctx.Load(oList);
                ctx.ExecuteQuery();
                if (collListItem.Count > 0)
                {
                    returnValue = siteUri;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Sends a Notification that the Site was created
        /// </summary>
        /// <param name="info"></param>
        protected void SendSuccessEmail(SiteInformation info)
        {
            //TODO CLEAN UP EMAILS
            try
            { 
                StringBuilder _admins = new StringBuilder();
                SuccessEmailMessage _message = new SuccessEmailMessage();
                _message.SiteUrl = info.Url;
                _message.SiteOwner = info.SiteOwner.Name;
                _message.Subject = "Notification: Your new SharePoint site is ready";

                _message.To.Add(info.SiteOwner.Name);
                foreach (var admin in info.AdditionalAdministrators)
                {
                    _message.Cc.Add(admin.Name);
                    _admins.Append(admin.Name);
                    _admins.Append(" ");
                }
                _message.SiteAdmin = _admins.ToString();
                EmailHelper.SendNewSiteSuccessEmail(_message);
            }
            catch(Exception ex)
            {
                Log.Error("Provisioning.Job.SiteProvisioningJob.SendSuccessEmail",
                    "There was an error sending email. The Error Message: {0}, Exception: {1}", 
                     ex.Message,
                     ex);
         
            }
        }

        /// <summary>
        /// Sends an Failure Email Notification
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        protected void SendFailureEmail(SiteInformation info, string errorMessage, bool sendToAdmin)
        {
            try
            {
                StringBuilder _admins = new StringBuilder();
                FailureEmailMessage _message = new FailureEmailMessage();
                _message.SiteUrl = info.Url;
                _message.SiteOwner = info.SiteOwner.Name;
                _message.Subject = "Alert: Your new SharePoint site request had a problem.";
                _message.ErrorMessage = errorMessage;
                if (sendToAdmin)
                {
                    _message.To.Add(info.SiteOwner.Name);
                }
                if (!string.IsNullOrEmpty(this._settings.SupportEmailNotification))
                {
                    string[] supportAdmins = this._settings.SupportEmailNotification.Split(';');
                    foreach (var supportAdmin in supportAdmins)
                    {
                        _message.To.Add(supportAdmin);

                    }
                }
                foreach (var admin in info.AdditionalAdministrators)
                {
                    if (sendToAdmin)
                    {
                        _message.Cc.Add(admin.Name);
                    }
                    _admins.Append(admin.Name);
                    _admins.Append(" ");
                }
                _message.SiteAdmin = _admins.ToString();
                EmailHelper.SendFailEmail(_message);
            }
            catch (Exception ex)
            {
                Log.Error("Provisioning.Job.SiteProvisioningJob.SendSuccessEmail",
                    "There was an error sending email. The Error Message: {0}, Exception: {1}",
                     ex.Message,
                     ex);
            }

        }

    }
}

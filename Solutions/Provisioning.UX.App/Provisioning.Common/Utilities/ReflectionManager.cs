﻿using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Data.SiteUrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Utilities
{
    public class ReflectionManager
    {
        private const string CONNECTIONSTRING_KEY = "ConnectionString";
        private const string CONTAINERSTRING_KEY = "Container";
        private ConfigManager _configManager = new ConfigManager();

        /// <summary>
        /// Returns Connectors
        /// </summary>
        /// <returns></returns>
        public FileConnectorBase GetProvisioningConnector(string moduleKey)
        {
            var _module = _configManager.GetModuleByName(moduleKey);
            var _managerTypeString = _module.ModuleType;
  
            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (FileConnectorBase)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                string connectionString;
                if (String.IsNullOrEmpty(_module.ConnectionString))
                {
                    connectionString = 
                        System.Configuration.ConfigurationManager.AppSettings.Get(moduleKey + "_connectionString");
                }
                else
                {
                    connectionString =  _module.ConnectionString;
                }
                instance.AddParameter(CONNECTIONSTRING_KEY, connectionString);
                instance.AddParameter(CONTAINERSTRING_KEY, _module.Container);

                if (instance.GetType() == typeof(SharePointConnector))
                {
                    instance.AddParameter(SharePointConnector.CLIENTCONTEXT,
                        new AppOnlyAuthenticationSite()
                        {
                            SiteUrl = connectionString
                        }.GetAuthenticatedContext());
                }

                Log.Info("ReflectionManager", "GetProvisioningConnector:key = {0}, provider = {1}, connectionString = {2}, container = {3}",
                    moduleKey,
                    _managerTypeString,
                    instance.Parameters[CONNECTIONSTRING_KEY],
                    instance.Parameters[CONTAINERSTRING_KEY]);
                return instance;
            }
            catch (Exception _ex)
            {
                Log.Error("ReflectionManager", PCResources.FileConnectorBase_Exception, _ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <returns></returns>
        public TemplateProviderBase GetTemplateProvider(string moduleKey)
        {
            var _module = _configManager.GetModuleByName(moduleKey);
            var _managerTypeString = _module.ModuleType;

            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var providerInstance = (TemplateProviderBase)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                var connectorInstance = this.GetProvisioningConnector(ModuleKeys.PROVISIONINGCONNECTORS_KEY);

                if (String.IsNullOrEmpty(_module.ConnectionString))
                {
                    connectorInstance.AddParameter(CONNECTIONSTRING_KEY,
                        System.Configuration.ConfigurationManager.AppSettings.Get(ModuleKeys.PROVISIONINGPROVIDER_KEY + "_connectionString"));
                }
                else
                {
                    connectorInstance.AddParameter(CONNECTIONSTRING_KEY, _module.ConnectionString);
                }
           
                connectorInstance.AddParameter(CONTAINERSTRING_KEY, _module.Container);

               Log.Info("ReflectionManager", "GetTemplateProvider: provider = {0}, connectionString = {1}, container = {2}",
                   _managerTypeString,
                    connectorInstance.Parameters[CONNECTIONSTRING_KEY],
                    connectorInstance.Parameters[CONTAINERSTRING_KEY]);


                providerInstance.Connector = connectorInstance;
                return providerInstance;
            }
            catch (Exception _ex)
            {
                Log.Error("ReflectionManager", PCResources.FileConnectorBase_Exception, _ex);
                throw;
            }
        }


        public ISiteUrlProvider GetSiteUrlProvider(string moduleKey)
        {
            var _module = _configManager.GetModuleByName(moduleKey);
            if(_module == null)
            {
                return null; // no SiteUrlProvider
            }

            var _managerTypeString = _module.ModuleType;

            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                ISiteUrlProvider instance = (ISiteUrlProvider)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                
                Log.Info("ReflectionManager", "GetSiteUrlProvider: key = {0}, provider = {1}",
                    moduleKey,
                    _managerTypeString);
                return instance;
            }
            catch (Exception _ex)
            {
                Log.Error("ReflectionManager", PCResources.FileConnectorBase_Exception, _ex);
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Configuration;
using System.ServiceModel.Description;

/// <summary>
/// Summary description for xrmConnection
/// </summary>
public static class crmConnection
{
    public static IOrganizationService getService()
    {
        IOrganizationService _service;
        OrganizationServiceContext _servicecontext;
        Uri OrganizationUri = new Uri(string.Format(ConfigurationManager.AppSettings["urlCRM"].ToString()));

        var Credentials = new ClientCredentials();
        Credentials.UserName.UserName = ConfigurationManager.AppSettings["DOM"].ToString() + "\\" + ConfigurationManager.AppSettings["UID"].ToString();
        Credentials.UserName.Password = ConfigurationManager.AppSettings["PID"].ToString();

        //ClientCredentials Credentials = new ClientCredentials();
        //Credentials.Windows.ClientCredential.UserName = @"GCG\Administrator";//@ConfigurationManager.AppSettings["DOM"].ToString()+"\\"+ ConfigurationManager.AppSettings["UID"].ToString();
        //Credentials.Windows.ClientCredential.Password = "Passw0rd";//ConfigurationManager.AppSettings["PID"].ToString();
        //Credentials.Windows.ClientCredential.Domain = ConfigurationManager.AppSettings["DOM"].ToString();

        OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(OrganizationUri, null, Credentials, null);

        _serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());

        _service = (IOrganizationService)_serviceProxy;
        _servicecontext = new OrganizationServiceContext(_service);

        return _service;
    }

}
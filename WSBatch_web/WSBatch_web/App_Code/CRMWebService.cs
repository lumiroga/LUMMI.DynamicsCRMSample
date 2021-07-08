using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Xml;
using Microsoft.Xrm.Sdk;

[WebService(Namespace = "http://www.company.com")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]


/// <summary>
/// Summary description for CRMWebService
/// </summary>
public class CRMWebService : WebService
{
	public CRMWebService()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    
    [WebMethod]
        public XmlDocument actualizar(string xmlCRM) {
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.LoadXml(xmlCRM);
            procToCRM pCRM = new procToCRM();
            pCRM.procesaACRM(xmlDoc);
            xmlDoc = XmlResponses.armaXml();
        }
        catch (System.ServiceModel.FaultException<OrganizationServiceFault> ex)
        {

            xmlDoc = XmlResponses.armaXmlError(ex.Detail.Message,ex.Detail.ErrorCode);
            CRMErrores.enviarErrores("CRM-Portaf " + ex.Detail.Message, xmlCRM);

        }
        catch (System.TimeoutException ex)
        {
            xmlDoc = XmlResponses.armaXmlError(ex.Message);
            CRMErrores.enviarErrores("CRM-Portaf " + ex.Message, xmlCRM);
        }
        catch (System.Exception ex)
        {
            string error = ex.Message;
            int code = 0;

            // Display the details of the inner exception.
            if (ex.InnerException != null)
            {
                error+="\n"+ex.InnerException.Message;

                System.ServiceModel.FaultException<OrganizationServiceFault> fe = ex.InnerException
                    as System.ServiceModel.FaultException<OrganizationServiceFault>;
                if (fe != null)
                {
    
                    code = fe.Detail.ErrorCode;
                    error+="\n"+fe.Detail.Message;
                }
            }

            xmlDoc = XmlResponses.armaXmlError(error,code);
            CRMErrores.enviarErrores("CRM-Portaf " + error, xmlCRM);
        }

        return xmlDoc;
    }
}
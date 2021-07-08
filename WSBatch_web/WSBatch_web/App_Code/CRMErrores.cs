using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

    public static class CRMErrores
    {
        public static void enviarErrores(string msg, string xml)
        {
            Entity error = new Entity("sim_errorservicioweb");
            IOrganizationService srv = crmConnection.getService();

            if (!error.Contains("sim_error"))
            {
                error.Attributes.Add("sim_error", msg);
            }
            else
            {
                error.Attributes["sim_error"] = msg;
            }

            if (!error.Contains("sim_xml"))
            {
                error.Attributes.Add("sim_xml", xml);
            }
            else
            {
                error.Attributes["sim_xml"] = xml;
            }

         

            srv.Create(error);
        }
    }


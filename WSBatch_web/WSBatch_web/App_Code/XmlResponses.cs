using System;
using System.Collections.Generic;

using System.Web;
using System.Xml;
using System.Net;
using System.Configuration;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for XmlResponses
/// </summary>
public static class XmlResponses
{
    public static XmlDocument armaXml()
    {
        string xmlStr = "<?xml version='1.0' encoding='UTF-8'?><resultado>";
        XmlDocument xmlDoc = new XmlDocument();
        xmlStr += creaNodo("tipo", "P");
        xmlStr += "</resultado>";
        xmlDoc.LoadXml(xmlStr);
        return xmlDoc;
    }
    public static XmlDocument armaXmlError(string _mensaje,int errorcode =0)
    {
        string error_code = decodeError(errorcode);
        string xmlStr = "<?xml version='1.0' encoding='UTF-8'?><resultado>";
        XmlDocument xmlDoc = new XmlDocument();
        xmlStr += creaNodo("tipo", "E");
        xmlStr += "<error>";
        xmlStr += creaNodo("cod", error_code);
        xmlStr += creaNodo("desc", _mensaje);
        xmlStr += "</error>";
        xmlStr += "</resultado>";
        xmlDoc.LoadXml(xmlStr);
        return xmlDoc;

    }
    private static string creaNodo(string _nombre, string _value)
    {
        StringBuilder strB = new StringBuilder();
        strB.Append("<");
        strB.Append(_nombre);
        strB.Append(">");
        strB.Append(_value);
        strB.Append("</");
        strB.Append(_nombre);
        strB.Append(">");
        return strB.ToString();
    }
    private static string decodeError(int errorcode)
    {
      return errorcode.ToString("X");
    }
}
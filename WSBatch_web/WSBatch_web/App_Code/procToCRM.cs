using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

/// <summary>
/// Summary description for procToCRM
/// </summary>
public class procToCRM
{
    //private const string CONDITIONTAG = "Condition";
    //private const string CONDITIONSTAG = "Conditions";
    //private const string LOGICALATTR = "logicalop";
    //private const string CONDATTR = "condattribute";
    //private const string CONDOP = "condoperator";
    private const string CONDVALUE = "conddatavalue";
    private const string ENTITYTAG = "Incidente";
    private const string ENT_RELATION = "TipoCliente";
    private const string IDCLIENTE = "IdCliente";
    //private const string DODELETE = "dodelete";
    //private const string DOACTIVE = "doactive";
    //private const string DOINACTIVE = "doinactive";
    private const string ATTRIBUTESTAG = "attribute";
    private const string TYPE = "type";
    private const string ATTRNAME = "name";
    private const string ATTRENTREL = "entityRel";
    private const string ATTRREL = "relation";
    private const string ID = "Id";
    private const string INCIDENTE = "incident";
    private const string IDPORTAF = "sim_idportaf";
    private const string SUBSERVICIO = "sim_subservicio";
    private const string SOLICITUD = "sim_solicitud";
    private const string ORIGEN = "sim_origen";
    private const string ORIGEN_INCIDENTE = "sim_origenincidente";
    

    IOrganizationService srv;
    private string[,] atributos;
    private string[] tipos;
    public procToCRM()
    {
        srv = crmConnection.getService();
    }

    public string procesaACRM(XmlDocument _xmlCRM)
    {
        Entity registro = new Entity();
        XmlNode root = _xmlCRM.GetElementsByTagName(ENTITYTAG).Item(0);

        //string optr = getOperator(_xmlCRM);
        string id_crm = getIdEntidad(_xmlCRM,ID);
        string entidad_relacion = getAttrEntity(root, ENTITYTAG, ENT_RELATION);

        string campo_cliente = string.Empty, campo_incidente = string.Empty, id_cliente = root.Attributes[IDCLIENTE].Value.ToString();

 
        if (entidad_relacion == "contact")
        {
            campo_cliente = "employeeid";
            campo_incidente = "sim_contacto";
        }
        else
        {
            campo_cliente = "accountnumber";
            campo_incidente = "sim_unidad";
        }

            getAttributes(_xmlCRM);
           // if (optr == "*") //Inserta nuevo
            //{
                
            //}
           // else //Update
            //{
            EntityCollection results = bringEntityUpd(INCIDENTE, IDPORTAF, id_crm);

            agregarRelacionCliente(ref registro, entidad_relacion, campo_cliente, campo_incidente,id_cliente);

            registro.Attributes[ORIGEN] = new OptionSetValue(2);

             registro.Attributes[ORIGEN_INCIDENTE] = new OptionSetValue(1);

             registro.Attributes[IDPORTAF] = id_crm;

            registro["sim_fechaasignacionnivel1"] = DateTime.Now;

                bool existe = false;
                for (int unit = 0; unit < results.Entities.Count; unit++)
                {
                    registro = results.Entities[unit];
                    UpdAttributes(ref registro);

                    srv.Update(registro);
                    existe = true;
                }
                if(!existe)
                {
                    registro.LogicalName = INCIDENTE;
                    UpdAttributes(ref registro);
                    ColumnSet cls = new ColumnSet(new string[]{SUBSERVICIO});
                    Entity solicitud = srv.Retrieve(SOLICITUD, ((EntityReference)registro.Attributes[SOLICITUD]).Id, cls);
                    
                    if(solicitud.Contains(SUBSERVICIO))
                    registro.Attributes[SUBSERVICIO] = solicitud[SUBSERVICIO];


                    srv.Create(registro);
                }
            //}
 

        return "";
    }
    private EntityCollection bringEntityUpd( string _entityName, string campo,string id_crm)
    {
        QueryExpression query = new QueryExpression(_entityName);
        query.Criteria.AddCondition(new ConditionExpression(campo,ConditionOperator.Equal,id_crm));
       
        if (atributos != null)
        {
            string[] atrb = new string[atributos.Length / 4];
            for (int i = 0; i < (atributos.Length / 4); i++)
            {
                atrb[i] = atributos[i, 0];
            }
            ColumnSet cols = new ColumnSet(atrb);
            query.ColumnSet = cols;
        }
        return srv.RetrieveMultiple(query);
    }
   private string getIdEntidad(XmlDocument xml, string id)
    {
        return xml.GetElementsByTagName(id)[0].InnerXml;
    }
    
    private string getAttrEntity(XmlNode _xmldoc, string _tagName, string _attrName)
    {
        XmlNode node = _xmldoc;
      
            return node.Attributes[_attrName].Value.ToLower();
 
    }
    private LogicalOperator transOperator(string _op)
    {
        if (_op.ToLower() == "and")
        {
            return LogicalOperator.And;
        }
        else
        {
            return LogicalOperator.Or;
        }
    }
    private void agregarRelacionCliente( ref Entity incidente, string nombreEntidadCliente, string campoCliente, string campoBusquedaIncidente,string valor)
    {
        Guid idCliente = getIdrelated(nombreEntidadCliente, campoCliente, valor);

        incidente[campoBusquedaIncidente] = new EntityReference(nombreEntidadCliente, idCliente);
        incidente["customerid"] = new EntityReference(nombreEntidadCliente, idCliente);
    }

    private void getAttributes(XmlDocument _xmldoc)
    {
        XmlNodeList nodeList = _xmldoc.GetElementsByTagName(ATTRIBUTESTAG);

        atributos = new string[nodeList.Count, 4];
        tipos = new string[nodeList.Count];

        for (int i = 0; i < nodeList.Count; i++)
        {
            atributos[i, 0] = nodeList[i].Attributes[ATTRNAME].Value;
            atributos[i, 1] = nodeList[i].ChildNodes[0] == null ? " " : nodeList[i].ChildNodes[0].InnerText;
            XmlAttribute attr = nodeList[i].Attributes[ATTRENTREL];
            XmlAttribute tp = nodeList[i].Attributes[TYPE] ;

            if(tp!=null)
            {
                tipos[i] = tp.Value;
            }
            else 
            {
                tipos[i] = " ";
            }

            if (attr != null)
            {
                atributos[i, 2] = attr.Value;
            }
            else
            {
                atributos[i, 2] = "";
            }
            attr = nodeList[i].Attributes[ATTRREL];
            if (attr != null)
            {
                atributos[i, 3] = attr.Value;
            }
            else
            {
                atributos[i, 3] = "";
            }
        }
    }
    /*private void actInactEntity(Entity _registro, string _active, string _inactive)
    {
        OptionSetValue state = new OptionSetValue();
        OptionSetValue status = new OptionSetValue();
        if (_active == "true")
        {
            state.Value = 0; // active
            status.Value = 100000002; // default status for the state
        }
        else
        {
            if (_inactive == "true")
            {
                state.Value = 1; // inactive
                status.Value = 100000002; // default status for the state
            }
        }

        EntityReference moniker = new EntityReference();
        moniker.LogicalName = _registro.LogicalName;
        moniker.Id = _registro.Id;

        OrganizationRequest request = new OrganizationRequest() { RequestName = "SetState" };
        request["EntityMoniker"] = moniker;
        request["State"] = state;
        request["Status"] = status;
        srv.Execute(request);
    }*/
    private void UpdAttributes(ref Entity _registro)
    {
        for (int i = 0; i < (atributos.Length / 4); i++)
        {
            string attrName = atributos[i, 0];
            string values = atributos[i, 1];
            if (_registro.Attributes.Contains(attrName))
            {
                _registro.Attributes.Remove(attrName);
            }
            if (values.Trim() != "")
            {
                RetrieveAttributeResponse DataAttr = ObtenDato(attrName, _registro.LogicalName);
                if (atributos[i, 2].Trim() != "" && atributos[i, 3].Trim() != "")
                {
                    Guid idRelated = getIdrelated(atributos[i, 2].ToLower().Trim(), atributos[i, 3].ToLower().Trim(), values);
                    EntityReference refEntity = new EntityReference(atributos[i, 2].ToLower().Trim(), idRelated);
                    _registro.Attributes.Add(attrName, refEntity);
                }
                else
                {
                    switch (DataAttr.AttributeMetadata.AttributeType.Value)
                
                    {
                       /* case " ":
                        case "string":
                        _registro.Attributes.Add(attrName, values);
                            break;

                        case "opt":
                            OptionSetValue pick = new OptionSetValue();
                            int pintVal;
                            int.TryParse(values, out pintVal);
                            pick.Value = pintVal;
                            _registro.Attributes.Add(attrName, pick);
                            break;

                        case "int":
                            int intVal;
                            int.TryParse(values, out intVal);
                            _registro.Attributes.Add(attrName, intVal);
                            break;

                        case "bit":
                            Boolean bval;
                            Boolean.TryParse(values, out bval);
                            _registro.Attributes.Add(attrName, bval);
                            break;
                        case "float":
                            Decimal decVal;
                            Decimal.TryParse(values, out decVal);
                            _registro.Attributes.Add(attrName, decVal);
                            break;
                        case "din":
                            Money mVal = new Money();
                            Decimal mdecVal;
                            Decimal.TryParse(values, out mdecVal);
                            mVal.Value = mdecVal;
                            _registro.Attributes.Add(attrName, mVal);
                            break;
                        case "fech":
                            DateTime dtval;
                            DateTime.TryParse(values, out dtval);
                            _registro.Attributes.Add(attrName, dtval);
                            break;
                        default:
                            _registro.Attributes.Add(attrName, values);
                            break;*/
                        #region Parsea Dato
                        case AttributeTypeCode.Boolean:
                            Boolean bval;
                            Boolean.TryParse(values, out bval);
                            _registro.Attributes.Add(attrName, bval);
                            break;
                        case AttributeTypeCode.DateTime:
                            DateTime dtval;
                            DateTime.TryParse(values, out dtval);
                            _registro.Attributes.Add(attrName, dtval);
                            break;
                        case AttributeTypeCode.Decimal:
                            Decimal decVal;
                            Decimal.TryParse(values, out decVal);
                            _registro.Attributes.Add(attrName, decVal);
                            break;
                        case AttributeTypeCode.Double:
                            Double dbVal;
                            Double.TryParse(values, out dbVal);
                            _registro.Attributes.Add(attrName, dbVal);
                            break;
                        case AttributeTypeCode.Integer:
                            int intVal;
                            int.TryParse(values, out intVal);
                            _registro.Attributes.Add(attrName, intVal);
                            break;
                        case AttributeTypeCode.Money:
                            Money mVal = new Money();
                            Decimal mdecVal;
                            Decimal.TryParse(values, out mdecVal);
                            mVal.Value = mdecVal;
                            _registro.Attributes.Add(attrName, mVal);
                            break;
                        case AttributeTypeCode.Picklist:
                            OptionSetValue pick = new OptionSetValue();
                            int pintVal;
                            int.TryParse(values, out pintVal);
                            pick.Value = pintVal;
                            _registro.Attributes.Add(attrName, pick);
                            break;
                        case AttributeTypeCode.String:
                            _registro.Attributes.Add(attrName, values);
                            break;
                        case AttributeTypeCode.Memo:
                            _registro.Attributes.Add(attrName, values);
                            break;
                        #endregion
                    }
                }
            }
        }
    }
    private Guid getIdrelated(string entityName, string attrRelated, object value)
    {
        string st=string.Empty;
        ConditionExpression cond = new ConditionExpression(attrRelated, ConditionOperator.Equal, value);
        QueryExpression query = new QueryExpression(entityName);
        query.Criteria.AddCondition(cond);


        EntityCollection results = srv.RetrieveMultiple(query);


        if (results.Entities.Count > 0)
        {
            Entity reg = results.Entities[0];
            return reg.Id;
        }
        else
        {
            return Guid.Empty;
        }
    }
    private RetrieveAttributeResponse ObtenDato(string attributeName, string entidad)
    {
        RetrieveAttributeRequest req = new RetrieveAttributeRequest();
        req.EntityLogicalName = entidad;
        req.LogicalName = attributeName;
        req.RetrieveAsIfPublished = true;

        RetrieveAttributeResponse attResp = new RetrieveAttributeResponse();
        attResp = (RetrieveAttributeResponse)srv.Execute(req);
        return attResp;
    }
}
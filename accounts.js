function copiarDireccion()
{
	if(!Xrm.Page.getAttribute("ednr_copiardireccion").getValue()) return;
	var dir_fiscal =
	{
	calle: Xrm.Page.getAttribute("address1_line1"),
	numext: Xrm.Page.getAttribute("address1_line2"),
	numint: Xrm.Page.getAttribute("address1_line3"),
	colonia  : Xrm.Page.getAttribute("address1_county"),
	cp: Xrm.Page.getAttribute("address1_postalcode")
	};
	
	
	var dir_comer=
	{
	calle: Xrm.Page.getAttribute("address2_line1"),
	numext: Xrm.Page.getAttribute("address2_line2"),
	numint: Xrm.Page.getAttribute("address2_line3"),
	colonia  : Xrm.Page.getAttribute("ednr_coloniacomercial"),
	cp:Xrm.Page.getAttribute("ednr_codigopostalcomercial")
	};
	
	dir_comer.calle.setValue(dir_fiscal.calle.getValue());
	dir_comer.numext.setValue(dir_fiscal.numext.getValue());
	dir_comer.numint.setValue(dir_fiscal.numint.getValue());
	dir_comer.colonia.setValue(dir_fiscal.colonia.getValue());
	
	direccion_buscar(dir_fiscal.cp.getValue());
	
}

/////////////////////
//
//@autor: LMR
//Entidad: n/a (funciones generales)
//Version 1.0
//Llena el valor de un campo tipo lookup.
//Funciones generales
/////////////////////
function set_lookup(campo,id,name,entity)
{
	var v_campo = Xrm.Page.getAttribute(campo);
	var o_campo = [] ; 
	o_campo.push({id:id,name:name,entityType:entity});
	
	v_campo.setValue(o_campo);
}
function setDelEst()
{
	var v_cp = Xrm.Page.getAttribute("ednr_codigopostalcomercial");
	if(v_cp.getValue()==null) return;
	
	direccion(v_cp.getValue());
	
}
function direccion_buscar(codigopostal)
{
	if(codigopostal != null)
{
var oDataEndpointUrl = "http://"+window.location.host+"/CRM/XRMServices/2011/OrganizationData.svc/ednr_codigopostalSet?$filter=ednr_name+eq+'"+codigopostal+"'";

   var service = GetRequestObject();
     if (service != null)
    {

        service.open("GET", oDataEndpointUrl, false);
        service.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
        service.setRequestHeader("Accept", "application/json, text/javascript, */*");
        service.send(null);
     requestResults = JSON.parse(  service.responseText );

   if (requestResults != null)
        {
         var codigo=requestResults.d; 
         
			var estado = codigo.results[0].ednr_Estadodelarepublica;
			var codigoID = codigo.results[0].ednr_codigopostalId;
			var codigoNombre = codigo.results[0].ednr_name;
			
			if(estado ==undefined ) 
			{
				alert("Codigo postal inexistente");
				return;
			}
			
			var del = codigo.results[0].ednr_Municipiodelegacion;
			//Id,LogicalName,Name
			set_lookup("ednr_municipiodelegacioncomercial",del.Id,del.Name,del.LogicalName);
			set_lookup("ednr_estadodelarepublicacomercial",estado.Id,estado.Name,estado.LogicalName);
			set_lookup("ednr_codigopostalcomercial",codigoID,codigoNombre,"ednr_codigopostal");
			      
         }
      }
  }
  
}
function direccion(codigopostal)
{
if(codigopostal != null)
{
var oDataEndpointUrl = "http://"+window.location.host+"/CRM/XRMServices/2011/OrganizationData.svc/ednr_codigopostalSet(guid'" + codigopostal[0].id+ "')";

   var service = GetRequestObject();
     if (service != null)
    {

        service.open("GET", oDataEndpointUrl, false);
        service.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
        service.setRequestHeader("Accept", "application/json, text/javascript, */*");
        service.send(null);
     requestResults = JSON.parse(  service.responseText );

   if (requestResults != null)
        {
         var codigo=requestResults.d; 
          if(GuidsAreEqual(codigopostal[0].id,codigo.ednr_codigopostalId))
            {
			var estado = codigo.ednr_Estadodelarepublica;
			var del = codigo.ednr_Municipiodelegacion;
			//Id,LogicalName,Name
			set_lookup("ednr_municipiodelegacioncomercial",del.Id,del.Name,del.LogicalName);
			set_lookup("ednr_estadodelarepublicacomercial",estado.Id,estado.Name,estado.LogicalName);
			}       
         }
      }
  }
}
function GuidsAreEqual(guid1, guid2)
{
    var isEqual = false;

    if (guid1 == null || guid2 == null)
    {
        isEqual = false;
    }
    else
    {
        isEqual = guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase();
    }
    return isEqual;
}


function GetRequestObject() 
{
    if (window.XMLHttpRequest) 
    {
        return new window.XMLHttpRequest;
    }
    else 
    {
        try 
        {
            return new ActiveXObject("MSXML2.XMLHTTP.3.0");
        }
        catch (ex) 
        {
             return null;
        }
    }
}

//Autor: LMR
//Entidad: Establecimiento 
//JS#1.0
//Alerta sobre la no existencia de un valor de numero exterior
//onsave
function alertaCalles(e)
{
	var regreso = "";
	if(Xrm.Page.getAttribute("address1_line2").getValue() ==null ) regreso+="Número exterior fiscal vacío\n";
	
	if(Xrm.Page.getAttribute("address2_line2").getValue() ==null ) regreso+="Número exterior comercial vacío\n";
	
	if(regreso=="") return;
	
	if(!confirm(regreso+"¿Desea continuar?"))
	{
		e.getEventArgs().preventDefault();
		return false;
	}
	return true;
}
//Autor: LMR
//Entidad: Establecimiento 
//JS#1.0
//Hace obligatorio RFC dependiendo del estatus de establecimiento
//onchange estatus, onload
function validarRFC()
{
	var rfc = Xrm.Page.getAttribute("ednr_rfc");
	if(Xrm.Page.getAttribute("ednr_establecimientoafiliado").getValue())
			rfc.setRequiredLevel("required");
	else
		rfc.setRequiredLevel("none");
	
}

// 3.0 Establecimiento

//Autor: LCM
//Entidad: Establecimiento 
//JS#3.1
//setear el campo estado con valor Mexico
//onload del lead

function setearPais(){
	var estado = Xrm.Page.getAttribute("address2_country"); //require level
		estado.setValue("México");
		estado.setSubmitMode("always");
}

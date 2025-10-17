#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.DataObjects
{
	/// <summary>
	/// Class MPIPHP.DataObjects.doOrganization:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrganization : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doOrganization() : base()
         {
         }
         public int org_id { get; set; }
         public string mpi_org_id { get; set; }
         public string org_name { get; set; }
         public string phone_no { get; set; }
         public string fax_no { get; set; }
         public string email_address { get; set; }
         public int org_type_id { get; set; }
         public string org_type_description { get; set; }
         public string org_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string routing_number { get; set; }
         public string federal_id { get; set; }
         public string status_value { get; set; }
         public int payment_type_id { get; set; }
         public string payment_type_description { get; set; }
         public string payment_type_value { get; set; }
         public string care_of { get; set; }
         public string aba_swift_bank_code { get; set; }
    }
    [Serializable]
    public enum enmOrganization
    {
         org_id ,
         mpi_org_id ,
         org_name ,
         phone_no ,
         fax_no ,
         email_address ,
         org_type_id ,
         org_type_description ,
         org_type_value ,
         status_id ,
         status_description ,
         routing_number ,
         federal_id ,
         status_value ,
         payment_type_id ,
         payment_type_description ,
         payment_type_value ,
         care_of ,
         aba_swift_bank_code ,
    }
}


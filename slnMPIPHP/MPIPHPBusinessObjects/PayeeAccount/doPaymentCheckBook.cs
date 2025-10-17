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
	/// Class MPIPHP.DataObjects.doPaymentCheckBook:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentCheckBook : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentCheckBook() : base()
         {
         }
         public int check_book_id { get; set; }
         public int check_type_id { get; set; }
         public string check_type_description { get; set; }
         public string check_type_value { get; set; }
         public DateTime effective_date { get; set; }
         public int first_check_number { get; set; }
         public int max_check_number { get; set; }
         public int last_check_number { get; set; }
    }
    [Serializable]
    public enum enmPaymentCheckBook
    {
         check_book_id ,
         check_type_id ,
         check_type_description ,
         check_type_value ,
         effective_date ,
         first_check_number ,
         max_check_number ,
         last_check_number ,
    }
}


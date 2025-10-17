#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using System.Data;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPersonContact:
	/// Inherited from doPersonContact, the class is used to customize the database object doPersonContact.
	/// </summary>
    [Serializable]
	public class cdoPersonContact : doPersonContact
	{
		public cdoPersonContact() : base()
		{

		}

        public string istrCompleteZipCode
        {
            get
            {
                string lstrZip = this.addr_zip_code;
                if (!string.IsNullOrEmpty(addr_zip_4_code))
                {
                    lstrZip += "-" + addr_zip_4_code;
                }
                return lstrZip;
            }
        }
    } 
} 

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPersonAddressHistory:
	/// Inherited from doPersonAddressHistory, the class is used to customize the database object doPersonAddressHistory.
	/// </summary>
    [Serializable]
	public class cdoPersonAddressHistory : doPersonAddressHistory 
	{
        public string istrPhysicalAddressType { get; set; }
        public string istrMailingAddressType { get; set; }

		public cdoPersonAddressHistory() : base()
		{
		}

        public string istrfull_address
        {
            get
            {
                StringBuilder lstrbAddress = new StringBuilder();
                lstrbAddress.Append(this.addr_line_1 + ",");
                if (!string.IsNullOrEmpty(this.addr_line_2))
                    lstrbAddress.Append(this.addr_line_2 + ",");
                if (!string.IsNullOrEmpty(this.addr_city))
                    lstrbAddress.Append(this.addr_city + ",");
                if (!string.IsNullOrEmpty(this.addr_state_description) && this.addr_state_description != busConstant.OTHER)
                    lstrbAddress.Append(this.addr_state_description + ",");
                if (!string.IsNullOrEmpty(this.foreign_province))
                    lstrbAddress.Append(this.foreign_province + ",");
                if (!string.IsNullOrEmpty(this.addr_country_description))
                    lstrbAddress.Append(this.addr_country_description + ",");
                if (!string.IsNullOrEmpty(this.addr_zip_code))
                    lstrbAddress.Append(this.addr_zip_code + ",");
                if (!string.IsNullOrEmpty(this.addr_zip_4_code))
                    lstrbAddress.Append(this.addr_zip_4_code + ",");
                if (!string.IsNullOrEmpty(this.foreign_postal_code))
                    lstrbAddress.Append(this.foreign_postal_code + ",");
                return lstrbAddress.ToString().Remove(lstrbAddress.Length - 1);
            }
        }

        public string istrSecuredFlag
        {
            get
            {
                if (this.secured_flag == busConstant.Flag_Yes)
                    return busConstant.YES;
                else
                    return busConstant.NO;
            }
        }

    } 
} 

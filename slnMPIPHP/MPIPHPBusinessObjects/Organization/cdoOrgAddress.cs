#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoOrgAddress:
	/// Inherited from doOrgAddress, the class is used to customize the database object doOrgAddress.
	/// </summary>
    [Serializable]
	public class cdoOrgAddress : doOrgAddress
	{
		public cdoOrgAddress() : base()
		{
		}
        public string istrOrgName { get; set; }
        public string istrContactName { get; set; }

        public string istrFullAddress
        {
            get
            {
                StringBuilder lstrbAddress = new StringBuilder();
                lstrbAddress.Append(this.addr_line_1 + ",");
                if (!string.IsNullOrEmpty(this.addr_line_2))
                    lstrbAddress.Append(this.addr_line_2 + ",");
                if (!string.IsNullOrEmpty(this.city))
                    lstrbAddress.Append(this.city + ",");
                if (!string.IsNullOrEmpty(this.state_description))
                    lstrbAddress.Append(this.state_description + ",");
                if (!string.IsNullOrEmpty(this.foreign_province))
                    lstrbAddress.Append(this.foreign_province + ",");
                if (!string.IsNullOrEmpty(this.country_description))
                    lstrbAddress.Append(this.country_description + ",");
                if (!string.IsNullOrEmpty(this.zip_code))
                    lstrbAddress.Append(this.zip_code + ",");
                if (!string.IsNullOrEmpty(this.zip_4_code))
                    lstrbAddress.Append(this.zip_4_code + ",");
                if (!string.IsNullOrEmpty(this.foreign_postal_code))
                    lstrbAddress.Append(this.foreign_postal_code + ",");
                return lstrbAddress.ToString().Remove(lstrbAddress.Length - 1);
            }
        }
    } 
} 

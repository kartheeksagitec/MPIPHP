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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountPaymentItemType:
	/// Inherited from doPayeeAccountPaymentItemType, the class is used to customize the database object doPayeeAccountPaymentItemType.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountPaymentItemType : doPayeeAccountPaymentItemType
	{
        public DateTime end_date_no_null
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return end_date;
            }
        }

		public cdoPayeeAccountPaymentItemType() : base()
		{
		}
    } 
} 

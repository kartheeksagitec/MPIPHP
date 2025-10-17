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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountRolloverItemDetail:
	/// Inherited from doPayeeAccountRolloverItemDetail, the class is used to customize the database object doPayeeAccountRolloverItemDetail.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountRolloverItemDetail : doPayeeAccountRolloverItemDetail
	{
		public cdoPayeeAccountRolloverItemDetail() : base()
		{
		}
    } 
} 

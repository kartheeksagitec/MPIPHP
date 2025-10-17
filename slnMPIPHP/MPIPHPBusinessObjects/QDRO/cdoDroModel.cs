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
	/// Class MPIPHP.CustomDataObjects.cdoDroModel:
	/// Inherited from doDroModel, the class is used to customize the database object doDroModel.
	/// </summary>
    [Serializable]
	public class cdoDroModel : doDroModel
	{
		public cdoDroModel() : base()
		{
		}
    } 
} 

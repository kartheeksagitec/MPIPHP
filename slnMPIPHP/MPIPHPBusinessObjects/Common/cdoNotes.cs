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
	/// Class MPIPHP.CustomDataObjects.cdoNotes:
	/// Inherited from doNotes, the class is used to customize the database object doNotes.
	/// </summary>
    [Serializable]
	public class cdoNotes : doNotes
	{
		public cdoNotes() : base()
		{
		}

        public string istrSource { get; set; }
       
    } 
} 

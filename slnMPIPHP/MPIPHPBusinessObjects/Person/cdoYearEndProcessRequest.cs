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
	/// Class MPIPHP.CustomDataObjects.cdoYearEndProcessRequest:
	/// Inherited from doYearEndProcessRequest, the class is used to customize the database object doYearEndProcessRequest.
	/// </summary>
    [Serializable]
	public class cdoYearEndProcessRequest : doYearEndProcessRequest
	{
		public cdoYearEndProcessRequest() : base()
		{
		}

        DateTime RerunDate = new DateTime(DateTime.Now.Year, 10, 15);
            //"10/15/" + DateTime.Now.Year;
        public DateTime istrRerunDate
        {
            get
            {

                return RerunDate;
            }
            set
            {
                RerunDate = value;
            }
        }
    } 
} 

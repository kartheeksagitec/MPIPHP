#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP
{
	/// <summary>
	/// Class MPIPHP.busPirEffortsHours:
	/// Inherited from busPirEffortsHoursGen, the class is used to customize the business object busPirEffortsHoursGen.
	/// </summary>
	[Serializable]
	public class busPirEffortsHours : busPirEffortsHoursGen
	{
        public string istrUser
        {
            get;
            set;
        }
	}
}

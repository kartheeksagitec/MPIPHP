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
	/// Class MPIPHP.CustomDataObjects.cdoDroBenefitDetails:
	/// Inherited from doDroBenefitDetails, the class is used to customize the database object doDroBenefitDetails.
	/// </summary>
    [Serializable]
	public class cdoDroBenefitDetails : doDroBenefitDetails
	{
		public cdoDroBenefitDetails() : base()
		{
		}

        #region Properties

        //Properties required for the corresponding names of DRO Model and Plan in the Cdo
        public string istrDroModelName{get;set;}
        public string istrPlanName { get; set; }
        public string istrPlanCode { get; set; }
        public string istrBenefitOptionValue { get; set; }
        public string istrBenefitOptionDescription { get; set; }

        #endregion 
    } 
} 

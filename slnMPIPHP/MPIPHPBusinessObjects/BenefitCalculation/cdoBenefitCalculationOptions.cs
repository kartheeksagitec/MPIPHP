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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitCalculationOptions:
	/// Inherited from doBenefitCalculationOptions, the class is used to customize the database object doBenefitCalculationOptions.
	/// </summary>
    [Serializable]
	public class cdoBenefitCalculationOptions : doBenefitCalculationOptions
	{
		public cdoBenefitCalculationOptions() : base()
		{
		}

        public string istrBenefitOptionDescription { get; set; }        
        public decimal idecSurvivorBenefitAmount { get; set; }

        //10 Percent
        public decimal idecRemainingAmountToBePaid
        {
            get;set;
        }

} 
} 

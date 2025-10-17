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

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPayment1099r:
	/// Inherited from busPayment1099rGen, the class is used to customize the business object busPayment1099rGen.
	/// </summary>
	[Serializable]
	public class busPayment1099r : busPayment1099rGen
	{
        /// <summary>
        /// Method to create a temp table and store data for annual batch
        /// </summary>
        /// <param name="aintTaxYear">Tax Year</param>
        public void CreateTemp1099rTableWithData(int aintTaxYear)
        {
            DBFunction.DBNonQuery("cdoPayment1099r.CreateTableFor1099r", new object[1] { aintTaxYear },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        public void CreateTempCorrected1099rTableWithData(int aintTaxYear, DateTime idtRefferenceDate)
        {
            DBFunction.DBNonQuery("cdoPayment1099r.CreateTempTblCorrected1099r", new object[2] { aintTaxYear,idtRefferenceDate },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        /// <summary>
        /// Method to drop the temp table for 1099r
        /// </summary>
        public void DropTemp1099rTable()
        {
            DBFunction.DBNonQuery("cdoPayment1099r.DropTableFor1099r", new object[0] { },
                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        public void DeleteNegativeRecords(int aintTaxYear)
        {
            DBFunction.DBNonQuery("cdoPayment1099r.DeleteNeg1099r", new object[1] { aintTaxYear },
                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
	}
}

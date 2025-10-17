#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busPaymentHistoryDistributionGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentHistoryDistribution and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentHistoryDistributionGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busPaymentHistoryDistributionGen
        /// </summary>
		public busPaymentHistoryDistributionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentHistoryDistributionGen.
        /// </summary>
		public cdoPaymentHistoryDistribution icdoPaymentHistoryDistribution { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPaymentHistoryDistributionStatusHistory. 
        /// </summary>
		public Collection<busPaymentHistoryDistributionStatusHistory> iclbPaymentHistoryDistributionStatusHistory { get; set; }



        /// <summary>
        ///  MPIPHP.busPaymentHistoryDistributionGen.FindPaymentHistoryDistribution():
        /// Finds a particular record from cdoPaymentHistoryDistribution with its primary key. 
        /// </summary>
        /// <param name="aintPaymentHistoryDistributionId">A primary key value of type int of cdoPaymentHistoryDistribution on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentHistoryDistribution(int aintPaymentHistoryDistributionId)
		{
			bool lblnResult = false;
			if (icdoPaymentHistoryDistribution == null)
			{
				icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution();
			}
			if (icdoPaymentHistoryDistribution.SelectRow(new object[1] { aintPaymentHistoryDistributionId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busPaymentHistoryDistributionGen.LoadPaymentHistoryDistributionStatusHistorys():
        /// Loads Collection object iclbPaymentHistoryDistributionStatusHistory of type busPaymentHistoryDistributionStatusHistory.
        /// </summary>
		public virtual void LoadPaymentHistoryDistributionStatusHistorys()
		{
			DataTable ldtbList = Select<cdoPaymentHistoryDistributionStatusHistory>(
                new string[1] { enmPaymentHistoryDistributionStatusHistory.payment_history_distribution_id.ToString() },
				new object[1] { icdoPaymentHistoryDistribution.payment_history_distribution_id }, null, enmPaymentHistoryDistributionStatusHistory.payment_history_distribution_id.ToString());
			iclbPaymentHistoryDistributionStatusHistory = GetCollection<busPaymentHistoryDistributionStatusHistory>(ldtbList, "icdoPaymentHistoryDistributionStatusHistory");
		}

	}
}

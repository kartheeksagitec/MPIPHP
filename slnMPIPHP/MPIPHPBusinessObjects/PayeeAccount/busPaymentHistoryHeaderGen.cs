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
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPaymentHistoryHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentHistoryHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentHistoryHeaderGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentHistoryHeaderGen
        /// </summary>
		public busPaymentHistoryHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentHistoryHeaderGen.
        /// </summary>
		public cdoPaymentHistoryHeader icdoPaymentHistoryHeader { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPaymentHistoryDetail. 
        /// </summary>
		public Collection<busPaymentHistoryDetail> iclbPaymentHistoryDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPaymentHistoryDistribution. 
        /// </summary>
		public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDistribution { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPaymentHistoryDistributionStatusHistory. 
        /// </summary>
		public Collection<busPaymentHistoryDistributionStatusHistory> iclbPaymentHistoryDistributionStatusHistory { get; set; }



        /// <summary>
        /// MPIPHP.busPaymentHistoryHeaderGen.FindPaymentHistoryHeader():
        /// Finds a particular record from cdoPaymentHistoryHeader with its primary key. 
        /// </summary>
        /// <param name="aintPaymentHistoryHeaderId">A primary key value of type int of cdoPaymentHistoryHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentHistoryHeader(int aintPaymentHistoryHeaderId)
		{
			bool lblnResult = false;
			if (icdoPaymentHistoryHeader == null)
			{
				icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader();
			}
			if (icdoPaymentHistoryHeader.SelectRow(new object[1] { aintPaymentHistoryHeaderId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPaymentHistoryHeaderGen.LoadPaymentHistoryDetails():
        /// Loads Collection object iclbPaymentHistoryDetail of type busPaymentHistoryDetail.
        /// </summary>
		public virtual void LoadPaymentHistoryDetails()
		{
            DataTable ldtbList = busBase.Select("cdoPaymentHistoryHeader.LoadPaymentHistoryDetail", new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
                
                
                //Select<cdoPaymentHistoryDetail>(
                //new string[1] { enmPaymentHistoryDetail.payment_history_header_id.ToString() },
                //new object[1] { icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
			iclbPaymentHistoryDetail = GetCollection<busPaymentHistoryDetail>(ldtbList, "icdoPaymentHistoryDetail");
		}

        /// <summary>
        /// MPIPHP.busPaymentHistoryHeaderGen.LoadPaymentHistoryDistributions():
        /// Loads Collection object iclbPaymentHistoryDistribution of type busPaymentHistoryDistribution.
        /// </summary>
		public virtual void LoadPaymentHistoryDistributions()
		{
			DataTable ldtbList = Select<cdoPaymentHistoryDistribution>(
				new string[1] { enmPaymentHistoryDistribution.payment_history_header_id.ToString() },
				new object[1] { icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
			iclbPaymentHistoryDistribution = GetCollection<busPaymentHistoryDistribution>(ldtbList, "icdoPaymentHistoryDistribution");
		}

        /// <summary>
        /// MPIPHP.busPaymentHistoryHeaderGen.LoadPaymentHistoryDistributionStatusHistorys():
        /// Loads Collection object iclbPaymentHistoryDistributionStatusHistory of type busPaymentHistoryDistributionStatusHistory.
        /// </summary>
		public virtual void LoadPaymentHistoryDistributionStatusHistorys()
		{
			DataTable ldtbList = Select<cdoPaymentHistoryDistributionStatusHistory>(
				new string[1] { enmPaymentHistoryDistributionStatusHistory.payment_history_header_id.ToString() },
				new object[1] { icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
			iclbPaymentHistoryDistributionStatusHistory = GetCollection<busPaymentHistoryDistributionStatusHistory>(ldtbList, "icdoPaymentHistoryDistributionStatusHistory");
		}

	}
}

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
using Sagitec.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
#endregion
namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// partial class NeoSpin.BusinessObjects.busOnetimeRetireePaymentContract
    /// </summary>	
	public partial class busOnetimeRetireePaymentContract 
	{
		
        /// <summary>
        /// Gets or sets the main-table object contained in busOnetimeRetireePaymentContract.
        /// </summary>
		public doOnetimeRetireePaymentContract icdoOnetimeRetireePaymentContract { get; set; }


		public Collection<busOnetimeRetireePaymentContract> iclbOnetimeRetireePaymentContract { get; set; }


        public virtual void LoadOnetimeRetireePaymentContracts(DataTable adtbSearchResult)
        { 
            iclbOnetimeRetireePaymentContract = GetCollection<busOnetimeRetireePaymentContract>(adtbSearchResult, "icdoOnetimeRetireePaymentContract");
        }


        public virtual bool FindOnetimeRetireePaymentContract(int aintOnetimeRetireePaymentContractId)
        {
            bool lblnResult = false;
            if (icdoOnetimeRetireePaymentContract == null)
            {
                icdoOnetimeRetireePaymentContract = new doOnetimeRetireePaymentContract();
            }
            if (icdoOnetimeRetireePaymentContract.SelectRow(new object[1] { aintOnetimeRetireePaymentContractId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

    }
	
}

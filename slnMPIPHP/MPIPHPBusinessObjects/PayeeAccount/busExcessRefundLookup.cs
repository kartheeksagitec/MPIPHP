#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busExcessRefundLookup:
	/// Inherited from busExcessRefundLookupGen, this class is used to customize the lookup business object busExcessRefundLookupGen. 
	/// </summary>
	[Serializable]
	public class busExcessRefundLookup : busExcessRefundLookupGen
    {
        #region Load Other Objects
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);
            busExcessRefund lbusExcessRefund = (busExcessRefund)aobjBus;
            lbusExcessRefund.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
            lbusExcessRefund.ibusPayee.icdoPerson.mpi_person_id = adtrRow[enmPerson.mpi_person_id.ToString()].ToString();
            lbusExcessRefund.ibusPayee.istrFullName = adtrRow["PayeeName"].ToString();
        }
        #endregion
    }
}

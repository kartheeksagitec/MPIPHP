#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;

using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busIapAnnunityAdjustmentMultiplier:
    /// Inherited from busIapAnnunityAdjustmentMultiplierGen, the class is used to customize the business object busIapAnnunityAdjustmentMultiplierGen.
    /// </summary>
    [Serializable]
    public class busIapAnnunityAdjustmentMultiplier : busIapAnnunityAdjustmentMultiplierGen
    {

        public Collection<busIapAnnunityAdjustmentMultiplier> iclbIapAnnunityAdjustmentMultiplier { get; set; }

        public override void BeforePersistChanges()
        {
           
            base.BeforePersistChanges();

            if (this.iobjPassInfo.ienmPageMode == utlPageMode.Update)
            {
                updateIAPAdjustmentMultiplier();

            }
        }
        public override long iintPrimaryKey
        {
            get
            {
                if (iobjPassInfo?.idictParams != null && iobjPassInfo.istrSenderID == "btnSave")
                {
                    return icdoIapAnnunityAdjustmentMultiplier.iap_annunity_adjustment_multiplier_id;
                }
                else return icdoIapAnnunityAdjustmentMultiplier.iintAPrimarKey;
            }
        }
        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            base.AddToResponse(aobjResponseData);
            aobjResponseData.ConcurrentKeysData[utlConstants.istrPrimaryKey] = Convert.ToString(icdoIapAnnunityAdjustmentMultiplier.iintAPrimarKey > 0 ? icdoIapAnnunityAdjustmentMultiplier.iintAPrimarKey : iintPrimaryKey);

        }

        //public override void AfterPersistChanges()
        //{
        //    base.AfterPersistChanges();
        //}

        public bool CheckIfManager()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                return true;
            }
            return false;
        }


        public override void BeforeValidate(Sagitec.Common.utlPageMode aenmPageMode)
        {
    
            base.BeforeValidate(aenmPageMode);
        }

        public void updateIAPAdjustmentMultiplier()
        {
            DataTable ldtblAnnunityAdjustmentMultiplier = busIapAnnunityAdjustmentMultiplier.Select("cdoIapAnnunityAdjustmentMultiplier.GetIAPAnnunityAdjustmentMultiplier", new object[1] { icdoIapAnnunityAdjustmentMultiplier.iap_annunity_adjustment_multiplier_id });
            iclbIapAnnunityAdjustmentMultiplier = GetCollection<busIapAnnunityAdjustmentMultiplier>(ldtblAnnunityAdjustmentMultiplier, "icdoIapAnnunityAdjustmentMultiplier");

            foreach (var lIAPAnnunityAdjustmentMultiplier in iclbIapAnnunityAdjustmentMultiplier)
            {
                lIAPAnnunityAdjustmentMultiplier.icdoIapAnnunityAdjustmentMultiplier.end_date = DateTime.Now;
                lIAPAnnunityAdjustmentMultiplier.icdoIapAnnunityAdjustmentMultiplier.Update();
            }
        }
     
    }
}

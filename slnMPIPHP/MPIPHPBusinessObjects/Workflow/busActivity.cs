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
using System.Xml;
using MPIPHP.Common;
using System.IO;
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busActivity:
    /// Inherited from busActivityGen, the class is used to customize the business object busActivityGen.
    /// </summary>
    [Serializable]
    public class busActivity : busActivityGen
    {
        public override bool FindActivity(int Aintactivityid)
        {
            bool lblnResult = false;
            lblnResult = base.FindActivity(Aintactivityid);
            if (icdoActivity != null && lblnResult)
            {
                GetDetails();
            }

            return lblnResult;
        }

        public void GetDetails()
        {
            if (ibusProcess == null)
                LoadProcess();

            GetDetails(ibusProcess.icdoProcess.name);
        }

        public void GetDetails(string astrProcessName)
        {
            utlProcessMaintainance.utlActivity lobjActivity = iobjPassInfo.isrvMetaDataCache.GetAcvtivityDetails(astrProcessName, icdoActivity.name);
            if (lobjActivity != null)
            {
                utlProcessMaintainance.utlForm lobjNewForm = null;
                utlProcessMaintainance.utlForm lobjUpdateForm = null;
                foreach (var item in lobjActivity.iarrItems)
                {
                    if (item is utlProcessMaintainance.utlForm)
                    {
                        utlProcessMaintainance.utlForm lobjTempForm = (utlProcessMaintainance.utlForm)item;
                        if (lobjTempForm.ienmPageMode == utlPageMode.New)
                            lobjNewForm = lobjTempForm;
                        else
                            lobjUpdateForm = lobjTempForm;
                    }
                }
                if (lobjNewForm != null)
                {
                    icdoActivity.new_mode_screen_name = lobjNewForm.istrFormName;
                    icdoActivity.new_mode_focus_control_id = lobjNewForm.istrFocusControlId;
                }
                if (lobjUpdateForm != null)
                {
                    icdoActivity.update_mode_screen_name = lobjUpdateForm.istrFormName;
                    icdoActivity.update_mode_focus_control_id = lobjUpdateForm.istrFocusControlId;
                }
                icdoActivity.istrReferenceID = lobjActivity.istrReferenceID;
                icdoActivity.iblnFirstActivityFlag = lobjActivity.iblnFirstActivityFlag;
                icdoActivity.iblnLastActivityFlag = lobjActivity.iblnLastActivityFlag;
            }
        }
    }
}

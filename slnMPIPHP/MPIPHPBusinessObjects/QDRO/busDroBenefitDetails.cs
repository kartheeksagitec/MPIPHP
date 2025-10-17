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
using Sagitec.DataObjects;
using System.Text.RegularExpressions;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busDroBenefitDetails:
	/// Inherited from busDroBenefitDetailsGen, the class is used to customize the business object busDroBenefitDetailsGen.
	/// </summary>
	[Serializable]
	public class busDroBenefitDetails : busDroBenefitDetailsGen
	{
        public busQdroApplication iobjbusQdroApplication { get; set; }

        public string istrSubPlan { get; set; }
        public string istrSubPlanDescription { get; set; } 

        public bool CheckNumeric()
        {
            bool lblnValidPercentage = false;
            Regex lrexGex = new Regex("^[0-9,.]*$");
            if (!lrexGex.IsMatch(this.icdoDroBenefitDetails.benefit_perc.ToString()) || !lrexGex.IsMatch(this.icdoDroBenefitDetails.benefit_amt.ToString()))
            {
                lblnValidPercentage = true;
            }
            return lblnValidPercentage;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);
        }
        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            int lint = this.iarrChangeLog.Count;
        }
	}
}

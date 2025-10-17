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
	/// Class MPIPHP.BusinessObjects.busAttorney:
	/// Inherited from busAttorneyGen, the class is used to customize the business object busAttorneyGen.
	/// </summary>
    [Serializable]
    public class busAttorney : busAttorneyGen
    {
        public busQdroApplication iobjbusQdroApplication { get; set; }

        public bool IsEffectiveEndDateNull()
        {
            if (this.icdoAttorney.date_eff_to == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }
    }
}

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
    /// Class MPIPHP.BusinessObjects.busOrganizationLookup:
    /// Inherited from busOrganizationLookupGen, this class is used to customize the lookup business object busOrganizationLookupGen. 
    /// </summary>
    [Serializable]
    public class busOrganizationLookup : busOrganizationLookupGen
    {        
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErros = new ArrayList();
            if (ahstParam["astr_org_type"].ToString().Length == 0)
            {
                larrErros.Add(AddError(3001, ""));
            }
            return larrErros;

        }

        public override void LoadOrganizations(DataTable adtbSearchResult)
        {
            base.LoadOrganizations(adtbSearchResult);
            if (iobjPassInfo.idictParams["FormName"].ToString() != busConstant.Organization_Lookup_Mail_Return)
            {
           
                foreach (busOrganization lbusOrg in this.iclbOrganization)
                {
                    DataTable ldtbOrgAddress = busOrgAddress.Select("cdoOrgAddress.GetFullAddress", new object[1] { lbusOrg.icdoOrganization.org_id });
                    if (ldtbOrgAddress.Rows.Count > 0)
                    {
                        DataRow ldtr = ldtbOrgAddress.Rows[0];
                        lbusOrg.ibusOrgAddress = new busOrgAddress { icdoOrgAddress = new cdoOrgAddress() };
                        lbusOrg.ibusOrgAddress.icdoOrgAddress.LoadData(ldtr);
                    }
                }
            }
        }
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            
            base.LoadOtherObjects(adtrRow, aobjBus);
            if (iobjPassInfo.idictParams["FormName"].ToString() == busConstant.Organization_Lookup_Mail_Return)
            {
                busOrganization lbusOrganization;
                lbusOrganization = (aobjBus as busOrganization);
                lbusOrganization.ibusOrgAddress = new busOrgAddress() { icdoOrgAddress = new cdoOrgAddress() };
                lbusOrganization.ibusOrgAddress.icdoOrgAddress.LoadData(adtrRow);
            }
        }

    }
}

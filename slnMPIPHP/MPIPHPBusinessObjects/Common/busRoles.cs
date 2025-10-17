#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busRoles : busMPIPHPBase
    {
		public busRoles()
		{
		}

        public string istrSecurityValue { get; set; }

        public cdoRoles icdoRoles { get; set; }
        public string istrResourceId { get; set; }
        public Collection<busSecurity> iclbSecurityByRoles { get; set; }

		public bool FindRole(int aintRoleId)
		{
			bool lblnResult = false;
			if (icdoRoles == null)
			{
				icdoRoles = new cdoRoles();
			}
			if (icdoRoles.SelectRow(new object[1] { aintRoleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        public override long iintPrimaryKey
        {
            get
            {
                return icdoRoles.iintPrimaryKey;
            }
        }

        public busRoles FindRole(string astrRoleDescription)
        {
            busRoles lbusRoles = null;
            DataTable ldtbRoleByDescription = Select("cdoRoles.RoleByRoleDescription", new object[] { astrRoleDescription });
            if (ldtbRoleByDescription != null && ldtbRoleByDescription.Rows.Count > 0)
            {
                Collection<busRoles> lclbRoles = GetCollection<busRoles>(ldtbRoleByDescription, "icdoRoles");
                if (lclbRoles != null && lclbRoles.Count > 0)
                {
                    lbusRoles = lclbRoles[0];
                }
            }

            return lbusRoles;
        }

        public void LoadSecurity()
        {
            DataTable ldtbList =
                Select("cdoSecurity.ByRole", new object[1] { icdoRoles.role_id });
            iclbSecurityByRoles = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
        }

        public void LoadSecurityForDelete()
        {
            DataTable ldtbList =
                Select("cdoSecurity.ByRoleForAllSecurities", new object[1] { icdoRoles.role_id });
            iclbSecurityByRoles = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
        }

		protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
		{
			if (aobjBus is busSecurity)
			{
				busSecurity lobjSec = (busSecurity)aobjBus;
				lobjSec.ibusResources = new busResources();
				lobjSec.ibusResources.icdoResources = new cdoResources();
				sqlFunction.LoadQueryResult(lobjSec.ibusResources.icdoResources, adtrRow);
			}
		}

        public bool IsRoleAssigned()
        {
            DataTable ldtbList =
                Select("cdoSecurity.CheckAssignedRoles", new object[1] { icdoRoles.role_id });
            iclbSecurityByRoles = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
            if (ldtbList.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AfterPersistChanges()
        {
            if ((iclbSecurityByRoles == null) || (iclbSecurityByRoles.Count == 0))
            {
                LoadSecurity();

            }

        }
	}
}

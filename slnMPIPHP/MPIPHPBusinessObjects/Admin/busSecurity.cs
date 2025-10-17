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
	/// Class MPIPHP.BusinessObjects.busSecurity:
	/// Inherited from busSecurityGen, the class is used to customize the business object busSecurityGen.
	/// </summary>
    [Serializable]
    public class busSecurity : busSecurityGen
    {
        public busSecurity()
        {

        }

        public busResources ibusResources { get; set; }

        public busRoles ibusRoles { get; set; }

        public cdoSecurity icdoSecurity { get; set; }

        public string istrSecurityRoles { get; set; }

        public string istrSecurityLevel { get; set; }

        public busCustomSecurity ibusCustomSecurity { get; set; }


        public bool FindSecurity(int aintResourceId, int aintRoleId)
        {
            bool lblnResult = false;
            if (icdoSecurity == null)
            {
                icdoSecurity = new cdoSecurity();
            }
            if (icdoSecurity.SelectRow(new string[2] { "resource_id","role_id" },
                  new object[2] { aintResourceId, aintRoleId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadResource()
        {
            if (ibusResources == null)
            {
                ibusResources = new busResources();
            }
            ibusResources.FindResource(icdoSecurity.resource_id);
        }

        public void LoadRole()
        {
            if (ibusRoles == null)
            {
                ibusRoles = new busRoles();
            }
            ibusRoles.FindRole(icdoSecurity.role_id);
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
    }
}

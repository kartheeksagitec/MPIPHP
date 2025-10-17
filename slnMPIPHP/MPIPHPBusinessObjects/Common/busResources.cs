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
using Sagitec.DataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
    public class busResources : busMPIPHPBase
    {
        public busResources()
        {

            icdoResources = new cdoResources();
            iobjMainCDO = icdoResources;
        }

        #region [Fields]

        DataTable idtrResourceRoleDescription;
        public cdoResources icdoResources { get; set; }
        public Collection<busSecurity> iclbSecurity { get; set; }
        public Collection<busResourcesScreen> iclbResourcesScreen { get; set; }
        public Collection<busUser> iclbUser { get; set; }
        public Collection<busCustomSecurity> iclbCustomSecurity { get; set; }

        public int intRoleId { get; set; }

        public utlCollection<cdoRoles> iutlUserRoles;
        //get the unassigned roles count 
        public int iintUnassignedRolesCount
        {
            get
            {
                if (iutlUserRoles != null)
                    return iutlUserRoles.Count;
                else
                    return 0;
            }
        }
        #endregion

        #region [Public Methods]

        public bool FindResource(int aintResourceId)
        {
            bool lblnResult = false;
            if (icdoResources == null)
            {
                icdoResources = new cdoResources();
            }
            if (icdoResources.SelectRow(new object[1] { aintResourceId }))
            {
                lblnResult = true;
            }
            LoadResourcesScreen();
            return lblnResult;
        }

        /// <summary>
        /// add new role to the user. If role_id already exists 
        /// then no record will be inserted in database.
        /// </summary>
        /// <param name="astrUserRoleId"></param>
        /// <returns></returns>
        public ArrayList AddRoleforResource(int aintRoleId)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;

            if (aintRoleId == 0)
            {
                lobjError = new utlError();
                lobjError = AddError(5099, "");
               // lobjError.istrErrorMessage = "Role is required.";
                larrResult.Add(lobjError);
                return larrResult;
            }
            busSecurity lbusSecurity = new busSecurity();
            if (lbusSecurity.FindSecurity(icdoResources.resource_id, aintRoleId))
            {
                if (lbusSecurity.icdoSecurity.security_value == 0)
                {
                    //First delete the record and then insert again with proper security value
                    lbusSecurity.icdoSecurity.Delete();

                    lbusSecurity.icdoSecurity.security_value = 1;
                    lbusSecurity.icdoSecurity.Insert();
                }
            }
            LoadSecurity();
            return larrResult;
        }

        public bool IsResourceAssigned()
        {
            DataTable ldtbList =
                Select("cdoSecurity.CheckAssignedResource", new object[1] { icdoResources.resource_id });
            iclbSecurity = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
            busSecurity lbusSecurity = new busSecurity();

            if (ldtbList.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void BeforePersistChanges()
        {
            busSecurity lbusSecurity = new busSecurity();
            cdoSecurity lcdoSecurity = new cdoSecurity();

            for (int i = 0; i < iarrChangeLog.Count; i++)
            {
                if (iarrChangeLog[i] is cdoSecurity)
                {
                    lcdoSecurity = (cdoSecurity)iarrChangeLog[i];
                    if (lbusSecurity.FindSecurity(lcdoSecurity.resource_id, lcdoSecurity.role_id))
                    {

                        lbusSecurity.icdoSecurity.Delete();
                        lcdoSecurity.Insert();
                    }
                }
                base.BeforePersistChanges();
            }
        }

        #endregion

        #region [Load Methods]

        public void LoadResourcesScreen()
        {
            ArrayList larrResourceScreens =
                iobjPassInfo.isrvMetaDataCache.GetScreensHavingResource(icdoResources.resource_id.ToString());
            Collection<busResourcesScreen> ltemp = new Collection<busResourcesScreen>();
            foreach (string lstrScreenElement in larrResourceScreens)
            {
                busResourcesScreen ltmpResourceScreen = new busResourcesScreen();
                string[] lstrSplit = lstrScreenElement.Split(new char[1] { '=' });
                if (lstrSplit[0] != null)
                {
                    ltmpResourceScreen.istrResourceFileName = lstrSplit[0].ToString();
                }
                if (lstrSplit[1] != null)
                {
                    ltmpResourceScreen.istrResourceElement = lstrSplit[1].ToString();
                }
                if (lstrSplit[2] != null)
                {
                    ltmpResourceScreen.istrResourceID = lstrSplit[2].ToString();
                }
                ltemp.Add(ltmpResourceScreen);
            }
            iclbResourcesScreen = ltemp;
        }

        public void LoadSecurity()
        {
            DataTable ldtbList =
                Select("cdoSecurity.ByResource", new object[1] { icdoResources.resource_id });
            iclbSecurity = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
        }

        public void LoadSecurityByResource()
        {
            DataTable ldtbList =
                Select("cdoSecurity.LoadSecurityByResource", new object[1] { icdoResources.resource_id });
            iclbSecurity = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
        }

        public void LoadUsers()
        {
            idtrResourceRoleDescription = Select("cdoSecurity.LoadResourceRoleDescription",
                                            new object[1] { icdoResources.resource_id });

            DataTable ldtbList =
                Select("cdoSecurity.LoadUserDetails", new object[1] { icdoResources.resource_id });
            iclbUser = GetCollection<busUser>(ldtbList, "icdoUser");

        }

        public void LoadCustomSecurityDetails()
        {
            DataTable ldtbList =
               Select("cdoSecurity.LoadCustomSecurityUserDetails", new object[1] { icdoResources.resource_id });
            iclbCustomSecurity = GetCollection<busCustomSecurity>(ldtbList, "icdoCustomSecurity");
        }

        public void LoadCustomSecurity()
        {
            DataTable idtCustomSecurity = Select("cdoSecurity.LoadCustomSecurity", new object[1] { icdoResources.resource_id });
            iclbCustomSecurity = GetCollection<busCustomSecurity>
                                            (idtCustomSecurity, "icdoCustomSecurity");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busUser)
            {
                busUser lobjUser = (busUser)aobjBus;
                lobjUser.icdoRoles = new cdoRoles();
                //fill user details grid
                StringBuilder lstrbResourceRoleDescription = new StringBuilder();
                string lstrFilter = "security_value = " + adtrRow["security_value"].ToString();
                DataRow[] ldrSelectedRows = idtrResourceRoleDescription.Select(lstrFilter);
                foreach (DataRow dr in ldrSelectedRows)
                {
                    lstrbResourceRoleDescription.Append(dr["role_description"] + ",");
                    lobjUser.icdoRoles.role_description = lstrbResourceRoleDescription.ToString().Remove(lstrbResourceRoleDescription.Length - 1);
                    lobjUser.icdoSecurity = new cdoSecurity();
                    lobjUser.icdoSecurity.security_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(11, adtrRow["security_value"].ToString());
                }
            }
            if (aobjBus is busCustomSecurity)
            {
                busCustomSecurity lobjCustomSecurity = (busCustomSecurity)aobjBus;
                lobjCustomSecurity.ibusUser = new busUser();
                lobjCustomSecurity.ibusUser.icdoUser = new cdoUser();
                lobjCustomSecurity.ibusUser.icdoUser.LoadData(adtrRow);
            }
            if (aobjBus is busSecurity)
            {
                busSecurity lobjSec = (busSecurity)aobjBus;
                lobjSec.ibusRoles = new busRoles();
                lobjSec.ibusRoles.icdoRoles = new cdoRoles();
                sqlFunction.LoadQueryResult(lobjSec.ibusRoles.icdoRoles, adtrRow);
            }
        }

        //load roles not assinged to resource
        public utlCollection<cdoRoles> LoadUnassignedRolesByResource()
        {
            DataTable ldtbList = Select("cdoRoles.ListOfUnassignedRolesByResource", new object[1] { icdoResources.resource_id });
            iutlUserRoles = doBase.GetCollection<cdoRoles>(ldtbList);
            return iutlUserRoles;
        }

        #endregion


    }
}

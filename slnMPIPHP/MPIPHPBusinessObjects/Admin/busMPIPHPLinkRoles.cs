#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using MPIPHP.BusinessObjects;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;

using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
using System.Collections.Generic;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busPERSLinkRoles : busRoles
    {
        public busPERSLinkRoles()
        {
            icdoRoles = new cdoRoles();
            iobjMainCDO = icdoRoles;
        }

        #region [Fields]
        //Here the UserSerialID is purposely declared as string varible 
        //It is then converted to int in the AddUser() method
        public string istrUserSerialId { get; set; }
        ////Property used to bind the resource_id to retrive textbox
        public int iintResourceID { get; set; }
        public Collection<busUser> iclbUsers { get; set; }
        public Collection<busUserRoles> iclbUserRoles { get; set; }
        public string istrQueueResources { get; set; }

        #endregion

        #region [Public Methods]

        /// <summary>
        /// add new user to the role. If user_serial_id already exists 
        /// then no record will be inserted in database.
        /// </summary>
        /// <param name="userSerialId"></param>
        /// <returns></returns>
        public ArrayList AddUser(string astrUserSerialId)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;

            int iintUserSerialId = 0;
            if (astrUserSerialId != null && astrUserSerialId.Length > 0)
                iintUserSerialId = int.Parse(astrUserSerialId);

            if (iintUserSerialId == 0)
            {
                lobjError = new utlError();
                lobjError = AddError(5096, "");
               // lobjError.istrErrorMessage = "Please enter valid user serial Id.";
                larrResult.Add(lobjError);
            }

            if (iintUserSerialId != 0)
            {
                busUser lobjUser = new busUser();

                //verify if user serial id exists          
                if (!lobjUser.FindUser(iintUserSerialId))
                {
                    lobjError = new utlError();
                    lobjError = AddError(5095, "");
                    //lobjError.istrErrorMessage = "User does not exists.";
                    larrResult.Add(lobjError);
                }

                if (lobjUser.FindUser(iintUserSerialId))
                {
                    busUserRoles lobjUserRoles = new busUserRoles();
                    //verify if role is assigned to the user
                    if (!lobjUserRoles.FindUserRoles(iintUserSerialId, icdoRoles.role_id))
                    {
                        busUserRoles lbusUserRole = new busUserRoles();
                        lbusUserRole.icdoUserRoles = new cdoUserRoles();
                        lbusUserRole.icdoUserRoles.role_id = icdoRoles.role_id;
                        lbusUserRole.icdoUserRoles.user_serial_id = iintUserSerialId;
                        if (lobjUser.icdoUser.begin_date.ToString() != string.Empty)
                        {
                            lbusUserRole.icdoUserRoles.effective_start_date =
                                Convert.ToDateTime(lobjUser.icdoUser.begin_date);
                        }
                        if (lobjUser.icdoUser.end_date.ToString() != string.Empty)
                        {
                            lbusUserRole.icdoUserRoles.effective_end_date =
                                Convert.ToDateTime(lobjUser.icdoUser.end_date);
                        }

                        lbusUserRole.icdoUserRoles.Insert();
                        larrResult.Add(this);
                    }
                    else
                    {
                        lobjError = new utlError();
                        lobjError = AddError(5097, "");
                        //lobjError.istrErrorMessage = "User already exists.";
                        larrResult.Add(lobjError);
                    }
                }
            }
            return larrResult;
        }

        /// <summary>
        /// add new resource to the role. If resource_id already exists 
        /// then no record will be inserted in database.
        /// </summary>
        /// <param name="astrResourceId"></param>
        /// <returns></returns>
        public ArrayList AddResource(string astrResourceId)
        {
            if(astrResourceId.IsNull())
            {
                astrResourceId = string.Empty;
            }
            int lintResourceId = 0;
            if (astrResourceId.Length > 0)
                lintResourceId = int.Parse(astrResourceId);

            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;

            if (lintResourceId == 0)
            {
                lobjError = new utlError();
                lobjError = AddError(5098, "");
               // lobjError.istrErrorMessage = "Resource is required.";
                larrResult.Add(lobjError);
                return larrResult;
            }
            busSecurity lbusSecurity = new busSecurity();

            if (lbusSecurity.FindSecurity(lintResourceId, icdoRoles.role_id))
            {
                //First delete the record and then insert again with proper security value
                lbusSecurity.icdoSecurity.Delete();
            }

            lbusSecurity.icdoSecurity.security_value = 1;
            lbusSecurity.icdoSecurity.created_by = iobjPassInfo.istrUserID;
            lbusSecurity.icdoSecurity.created_date = DateTime.Now;
            lbusSecurity.icdoSecurity.modified_by = iobjPassInfo.istrUserID;
            lbusSecurity.icdoSecurity.modified_date = DateTime.Now;
            lbusSecurity.icdoSecurity.resource_id = lintResourceId;
            lbusSecurity.icdoSecurity.role_id = icdoRoles.role_id;

            lbusSecurity.icdoSecurity.Insert();

            LoadSecurity();
            return larrResult;
        }

        #endregion

        #region [Load Methods]
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);
            if (aobjBus is busUserRoles)
            {
                busUserRoles lobjUserRoles = (busUserRoles)aobjBus;
                lobjUserRoles.ibusUser = new busUser();
                lobjUserRoles.ibusUser.icdoUser = new cdoUser();
                lobjUserRoles.ibusUser.icdoUser.LoadData(adtrRow);

                lobjUserRoles.icdoUserRoles.role_id = icdoRoles.role_id;
            }
        }

        public void LoadUsers()
        {
            DataTable ldtbList = Select("cdoUser.ListOfUsersByRole", new object[1] { icdoRoles.role_id });
            iclbUsers = GetCollection<busUser>(ldtbList, "icdoUser");
        }

        //load resources not assigned to roles
        public Collection<cdoResources> LoadUnassignedRolesByUser()
        {
            DataTable ldtbList;

            if (!string.IsNullOrEmpty(istrQueueResources))
            {
                Dictionary<string, string> ldictParams = new Dictionary<string, string>();
                ldictParams.Add("@ROLE_ID", icdoRoles.role_id.ToString());
                ldictParams.Add("@RESOURCE_ID", istrQueueResources);

                string lstrQuery = iobjPassInfo.isrvMetaDataCache.GetFormattedQuery("cdoResources.UnassignedResourcesForQueue", ldictParams);
                ldtbList = DBFunction.DBSelect(lstrQuery, new Collection<IDbDataParameter>(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                ldtbList = Select("cdoResources.UnassignedResourcesByRoles", new object[1] { icdoRoles.role_id });
            }

            return doBase.GetCollection<cdoResources>(ldtbList);
        }
        #endregion
    }

}

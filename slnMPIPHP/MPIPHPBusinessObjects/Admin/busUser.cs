#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using MPIPHP.BusinessObjects;

using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class BusinessUserRole : busMPIPHPBase
    {
        public string role_value { get; set; }
        public string role_desc { get; set; }
        public override int Delete()
        {
            return 1;
        }

    }
    [Serializable]
    public class busUser : busUserGen
    {
        #region [Constructor(s)]

        public busUser()
        {
        }

        #endregion

        #region [Properties]
        /// <summary>
        /// Communication Secure Message Conversations
        /// </summary>
      
        private DataTable idtroleDescription;
        public utlCollection<BusinessUserRole> iclbBusinessUserRoles { get; set; }
        public utlCollection<cdoRoles> iutlUserRoles;
        //public Collection<busUserRoles> iclbUserRoles { get; set; }

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

        //Here the UserRoleId is purposely declared as string varible 
        //It is then converted to int in the AddRole() method
        public string istrUserRoleId { get; set; }

        private bool _iblnEnableRevokeCustomSecurityButton = false;
        public bool iblnEnableRevokeCustomSecurityButton
        {
            get { return _iblnEnableRevokeCustomSecurityButton; }
            set { _iblnEnableRevokeCustomSecurityButton = value; }
        }
        public Collection<busUser> iclbUser { get; set; }
        public Collection<busUserRoles> iclbUserRoles { get; set; }
        public Collection<busSecurity> iclbSecurity { get; set; }
        public cdoSecurity icdoSecurity { get; set; }
        /// <summary>
        /// This boolean will be used by the simple user creation maintenance page
        /// </summary>
        public bool iblnSimpleUserCreationMode { get; set; }

        /// <summary>
        /// Set role description on the ResourceMaintenance Screen on User Details tab
        /// </summary>
        public cdoRoles icdoRoles { get; set; }

        #endregion Fields

        #region [Public Methods]

        public void LoadUserRolesByEffectiveDate(DateTime adtStartDate)
        {
            DataTable ldtbList = Select("cdoUserRoles.ByEffectiveDate", new object[] { icdoUser.user_serial_id, adtStartDate });
            if (ldtbList != null && ldtbList.Rows.Count > 0)
                iclbUserRoles = GetCollection<busUserRoles>(ldtbList, "icdoUserRoles");
            else
                iclbUserRoles = new Collection<busUserRoles>();
        }

        /// <summary>
        /// This method will get the rog contact for batch certification.
        /// </summary>
        /// <returns></returns>
        public static Collection<busUser> GetUsersForSendingActiveEmployerWithNoActiveEmployees()
        {
            Collection<busUser> lclbUsers = new Collection<busUser>();
            DataTable ldtbUsers = busBase.Select("cdoUser.GetUsersForActiveEmployerWithNoActiveEmployeeNotification",
                      new object[] { });
            if (ldtbUsers != null && ldtbUsers.Rows.Count > 0)
            {
                busBase lbusBase = new busBase();
                lclbUsers = lbusBase.GetCollection<busUser>(ldtbUsers, "icdoUser");
            }
            return lclbUsers;
        }

        /// <summary>
        /// Returns true if the user marks himself as unavailable, false o.w.
        /// </summary>
        /// <param name="aintUserSerialID">User Serial ID.</param>
        /// <returns>True if the user is unavailable false o.w.</returns>
        public bool IsUserUnavailable(int aintUserSerialID)
        {
            if (!FindUser(aintUserSerialID))
            {
                return true;
            }


            //if (this.icdoUser.unavailable_ind.IsNotNullOrEmpty() && this.icdoUser.unavailable_ind.Equals(WorkflowConstants.FlagYes))
            //{
            //    return true;
            //}

            return false;
        }

        public bool EndDateBeforeBeginDate()
        {
            if ((icdoUser.end_date != DateTime.MinValue) && (icdoUser.end_date < icdoUser.begin_date))
            {
                return true;
            }
            return false;
        }

        /* To Check whether User Id is already exists*/
        public bool CheckDuplicateUserID()
        {
            if (iblnSimpleUserCreationMode && icdoUser.user_serial_id == 0)
            {
                icdoUser.user_id = icdoUser.email_address.Substring(0, icdoUser.email_address.IndexOf("@"));
            }
            if (icdoUser.user_serial_id == 0)
            {
                DataTable ldtbList = Select<cdoUser>(new string[1] { "user_id" },
                                                     new object[1] { icdoUser.user_id }, null, null);
                iclbUser = GetCollection<busUser>(ldtbList, "icdoUser");
                string lstrUserID = icdoUser.user_id;
                foreach (busUser lobjUser in iclbUser)
                {
                    if (lobjUser.icdoUser.user_id.Trim().ToLower() == lstrUserID.Trim().ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ValidateEmail()
        {
            if (!String.IsNullOrEmpty(icdoUser.email_address))
            {
                return true;//busGlobalFunctions.IsValidEmail(_icdoUser.email_address);
            }
            return true;
        }
        /// <summary>
        /// Revoke custom security
        /// </summary>
        public void DeleteCustomSecurity()
        {
            DBFunction.DBNonQuery("cdoCustomSecurity.DeleteRecords",
                              new object[1] { icdoUser.user_serial_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

        }

        /// <summary>
        /// add new role to the user. If role_id already exists 
        /// then no record will be inserted in database.
        /// </summary>
        /// <param name="astrUserRoleId"></param>
        /// <returns></returns>
        public ArrayList AddRole(string astrUserRoleId)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;

            int lintUserRoleId = 0;
            if (astrUserRoleId != null && astrUserRoleId.Length > 0)
                lintUserRoleId = int.Parse(astrUserRoleId);

            if (lintUserRoleId == 0)
            {
                lobjError = new utlError();
                lobjError = AddError(5099, "");
              //  lobjError.istrErrorMessage = "Role  is required.";
                larrResult.Add(lobjError);
                return larrResult;
            }
            //verify if role is assigned to the user
            busUserRoles lobjUserRoles = new busUserRoles();
            if (!lobjUserRoles.FindUserRoles(icdoUser.user_serial_id, lintUserRoleId))
            {
                busUserRoles lbusUserRoles = new busUserRoles();
                lbusUserRoles.icdoUserRoles = new cdoUserRoles();
                lbusUserRoles.icdoUserRoles.role_id = lintUserRoleId;
                lbusUserRoles.icdoUserRoles.user_serial_id = icdoUser.user_serial_id;
                lbusUserRoles.icdoUserRoles.effective_start_date = DateTime.Parse(System.DateTime.Now.ToShortDateString());
                lbusUserRoles.icdoUserRoles.Insert();

                larrResult.Add(this);
            }
            return larrResult;
        }

        public bool ValidateAgainstADS()
        {
            utlUserInfo lobjUserInfo = null;
            if (iblnSimpleUserCreationMode && icdoUser.user_serial_id == 0)
            {
                icdoUser.user_id = icdoUser.email_address.Substring(0, icdoUser.email_address.IndexOf("@"));
            }
            lobjUserInfo = iobjPassInfo.isrvDBCache.CheckUserInAD(icdoUser.user_id);
            if (lobjUserInfo.iblnAuthenticated)
            {
                if (iblnSimpleUserCreationMode)
                {
                    if (string.IsNullOrWhiteSpace(lobjUserInfo.istrFirstName) && string.IsNullOrWhiteSpace(lobjUserInfo.istrLastName))
                    {
                        icdoUser.user_status_value = "I";
                        icdoUser.end_date = DateTime.Now;
                    }
                }
                if (lobjUserInfo.istrLastName != "")
                    icdoUser.last_name = lobjUserInfo.istrLastName;

                if (lobjUserInfo.istrFirstName != "")
                    icdoUser.first_name = lobjUserInfo.istrFirstName;
            }
            else
            {
                if (iblnSimpleUserCreationMode)
                {
                    icdoUser.user_status_value = "I";
                    icdoUser.end_date = DateTime.Now;
                }
            }
            return lobjUserInfo.iblnAuthenticated;
        }

        /// <summary>
        /// MPIPHP.BusinessObjects.busPersonGen.FindPerson():
        /// Finds a particular record from cdoPerson with its MPIPHP id. 
        /// </summary>
        /// <param name="astrMPIPHPId">MPIPHP id for the person.</param>
        /// <returns>true if found otherwise false</returns>
        public bool FindUser(string astrUserId)
        {
            DataTable ldtbPerson = busBase.Select<cdoUser>(new string[1] { enmUser.user_id.ToString() },
                new object[1] { astrUserId }, null, null);

            if (ldtbPerson.Rows.Count > 0)
            {
                this.icdoUser = new cdoUser();
                this.icdoUser.LoadData(ldtbPerson.Rows[0]);
                return true;
            }

            return false;
        }


        #region [Load Methods]

        public void LoadUserRoles()
        {
            DataTable ldtbList = Select("cdoUserRoles.ByUser", new object[1] { icdoUser.user_serial_id });
            iclbUserRoles = GetCollection<busUserRoles>(ldtbList, "icdoUserRoles");
        }

        public bool IsMemberActiveInRole(int aintRoleID, DateTime adtEffectiveDate)
        {
            if (iclbUserRoles == null)
                LoadUserRoles();

            busUserRoles lbusUserRole = iclbUserRoles.FirstOrDefault(i => i.icdoUserRoles.role_id == aintRoleID);
            if (lbusUserRole != null)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lbusUserRole.icdoUserRoles.effective_start_date, lbusUserRole.icdoUserRoles.effective_end_date))
                {
                    return true;
                }
            }

            return false;
        }

        //load roles not assigned to the user 
        public utlCollection<cdoRoles> LoadUnassignedRolesByUser()
        {
            DataTable ldtbList = Select("cdoRoles.ListOfUnassignedRolesByUser", new object[1] { icdoUser.user_serial_id });
            iutlUserRoles = doBase.GetCollection<cdoRoles>(ldtbList);
            return iutlUserRoles;
        }

        public void LoadSecurity()
        {
            //This information is used to populate "Roles" column  in the security tab.
            //This datatable is to be populated before we we retrieve the security information 
            idtroleDescription = Select("cdoSecurity.LoadRoleDescription", new object[1] { icdoUser.user_serial_id });

            DataTable ldtbList =
                Select("cdoSecurity.ByUser", new object[1] { icdoUser.user_serial_id });
            iclbSecurity = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
        }

        #endregion


        #endregion

        #region [Overriden Methods]

        public override void BeforePersistChanges()
        {
            if (iblnSimpleUserCreationMode)
            {
                icdoUser.log_activity_flag = busConstant.FLAG_NO;
                icdoUser.begin_date = DateTime.Now;
                icdoUser.user_type_id = 9;
                icdoUser.user_type_value = "I";
                icdoUser.user_status_id = 10;
                icdoUser.user_status_value = "A";
            }
            //Fire this to obtain latest name info from active directory
            ValidateAgainstADS();
            /*
            if (icdoUser.email_address == null)
            {
                icdoUser.email_address = icdoUser.user_id + "@nd.gov";
            }*/

            //insert, update and delete records from sgs_custom_security table
            InsertandDeleteRecord();
        }

        public override void AfterPersistChanges()
        {
            if (iblnSimpleUserCreationMode && icdoUser.user_status_value == "I")
            {
                icdoUser.Delete();
                return;
            }
            LoadCustomAccessPolicys();
            InsertValuesInCustomAccessPolicy();

            //busPersonalize lbusPersonalize = new busPersonalize();
            //lbusPersonalize.icdoPersonalize = new cdoPersonalize();

            cdoUserSecurityHistory lcdoUserSecurityHistory = new cdoUserSecurityHistory();
            cdoCustomSecurity lcdocustomSecurity = new cdoCustomSecurity();
            cdoSecurity lcdoSecurity = new cdoSecurity();
            busRoles lbusRoles = new busRoles();

            LoadSecurity();
            LoadCustomAccessPolicys();

            //Inserting in SGS_UserSecurityHistory
            for (int i = 0; i < iarrChangeLog.Count; i++)
            {
                if (iarrChangeLog[i] is cdoCustomSecurity)
                {
                    lcdocustomSecurity = (cdoCustomSecurity)iarrChangeLog[i];

                    lcdoUserSecurityHistory.user_serial_id = icdoUser.user_serial_id;
                    lcdoUserSecurityHistory.resource_id = lcdocustomSecurity.resource_id;
                    lcdoUserSecurityHistory.old_security_level = lcdocustomSecurity.old_security_value.ToString();
                    lcdoUserSecurityHistory.custom_security_level_id = lcdocustomSecurity.custom_security_level_id;
                    lcdoUserSecurityHistory.custom_security_level_value = lcdocustomSecurity.custom_security_level_value;
                    lcdoUserSecurityHistory.Insert();
                }
            }
            //Log is changed for cdoCustomSecurity as it was giving error
            //when trying to update the customized value other than default value.
            iarrChangeLog.Clear();

            if (iblnSimpleUserCreationMode)
            {

                if (iclbBusinessUserRoles != null || iclbBusinessUserRoles.Count != 0)
                {
                    foreach (BusinessUserRole lcdoBusinessUserRole in iclbBusinessUserRoles)
                    {
                        busRoles lbusSimpleRoles = new busRoles();
                        busCode lbusRoleCode = new busCode();
                        cdoCodeValue lcdoCodeValue = lbusRoleCode.GetCodeValue(busConstant.Role.CODE_ID_BUSINESS_USER_ROLES, lcdoBusinessUserRole.role_value);
                        if (lcdoCodeValue != null && string.IsNullOrWhiteSpace(lcdoCodeValue.data1) == false)
                        {
                            lbusSimpleRoles = lbusSimpleRoles.FindRole(lcdoCodeValue.data1);
                            if (lbusSimpleRoles != null)
                            {
                                AddRole(lbusSimpleRoles.icdoRoles.role_id.ToString());
                            }
                        }
                    }
                    LoadUserRoles();
                }

            }

            base.AfterPersistChanges();
        }

        //validation to check whether end date and begin date combination
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busSecurity)
            {
                StringBuilder lstrbRoleDesc = new StringBuilder();
                string lstrFilter = "resource_id = " + adtrRow["resource_id"].ToString();
                DataRow[] ldrSelectedRows = idtroleDescription.Select(lstrFilter);
                busSecurity lobjbusSecurity = (busSecurity)aobjBus;

                lobjbusSecurity.ibusResources = new busResources();
                lobjbusSecurity.ibusResources.icdoResources = new cdoResources();
                lobjbusSecurity.ibusResources.icdoResources.LoadData(adtrRow);

                //Get default security level
                lobjbusSecurity.icdoSecurity.security_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(11, adtrRow["security_value"].ToString());

                if (!adtrRow["security_value"].Equals(0))
                {
                    foreach (DataRow dtrow in ldrSelectedRows)
                    {
                        lstrbRoleDesc.Append(dtrow["role_description"] + ",");
                    }
                    if (lstrbRoleDesc.Length > 0)
                        lobjbusSecurity.istrSecurityRoles = lstrbRoleDesc.ToString().Remove(lstrbRoleDesc.Length - 1);
                }

                //Load Custom Security bus object
                lobjbusSecurity.ibusCustomSecurity = new busCustomSecurity();
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity = new cdoCustomSecurity();
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity.custom_security_id = Convert.ToInt32(adtrRow["custom_security_id"]);
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity.user_serial_id = icdoUser.user_serial_id;
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity.resource_id = Convert.ToInt32(adtrRow["resource_id"]);
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity.old_security_value = Convert.ToInt32(adtrRow["security_value"]);
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity.custom_security_level_id = Convert.ToInt32(adtrRow["security_id"]);
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity.custom_security_level_value = adtrRow["custom_security_value"].ToString();
                lobjbusSecurity.ibusCustomSecurity.icdoCustomSecurity.update_seq = Convert.ToInt32(adtrRow["update_seq"]);

                //This value is used to display "*" against the customized security for a resource
                if (adtrRow["ModifiedSecurity"].ToString() == "True")
                {
                    lobjbusSecurity.istrSecurityLevel = "*";
                    iblnEnableRevokeCustomSecurityButton = true;
                }
            }
            else if (aobjBus is busUserRoles)
            {
                busUserRoles lobjbusUserRoles = (busUserRoles)aobjBus;
                lobjbusUserRoles.icdoRoles = new cdoRoles();
                lobjbusUserRoles.icdoRoles.role_description = adtrRow["role_description"].ToString();
            }
        }

        #endregion

        #region [Private Methods]

        /// <summary>
        /// Insert and delete records in sgs_custom_access_policy table
        /// </summary>
        private void InsertValuesInCustomAccessPolicy()
        {
            if ((iclbCustomAccessPolicy.Count == 0) && (icdoUser.access_type_value == busConstant.Security.ACCESS_TYPE_VALUE))
            {
                cdoCustomAccessPolicy lcdoAccesspolicy = new cdoCustomAccessPolicy();
                lcdoAccesspolicy.user_serial_id = icdoUser.user_serial_id;
                lcdoAccesspolicy.weekday_value = busConstant.Security.MONDAY;
                lcdoAccesspolicy.from_time = 1;
                lcdoAccesspolicy.to_time = 2;
                lcdoAccesspolicy.Insert();

                lcdoAccesspolicy.weekday_value = busConstant.Security.TUESDAY;
                lcdoAccesspolicy.from_time = 1;
                lcdoAccesspolicy.to_time = 1;
                lcdoAccesspolicy.Insert();

                lcdoAccesspolicy.weekday_value = busConstant.Security.WEDNESDAY;
                lcdoAccesspolicy.from_time = 1;
                lcdoAccesspolicy.to_time = 1;
                lcdoAccesspolicy.Insert();

                lcdoAccesspolicy.weekday_value = busConstant.Security.THRUSDAY;
                lcdoAccesspolicy.from_time = 1;
                lcdoAccesspolicy.to_time = 1;
                lcdoAccesspolicy.Insert();

                lcdoAccesspolicy.weekday_value = busConstant.Security.FRIDAY;
                lcdoAccesspolicy.from_time = 1;
                lcdoAccesspolicy.to_time = 1;
                lcdoAccesspolicy.Insert();

                lcdoAccesspolicy.weekday_value = busConstant.Security.SATURDAY;
                lcdoAccesspolicy.from_time = 1;
                lcdoAccesspolicy.to_time = 1;
                lcdoAccesspolicy.Insert();

                lcdoAccesspolicy.weekday_value = busConstant.Security.SUNDAY;
                lcdoAccesspolicy.from_time = 1;
                lcdoAccesspolicy.to_time = 1;
                lcdoAccesspolicy.Insert();

                LoadCustomAccessPolicys();
            }
            if ((iclbCustomAccessPolicy.Count > 0) && (icdoUser.access_type_value != busConstant.Security.ACCESS_TYPE_VALUE))
            {
                DBFunction.DBNonQuery("cdoCustomAccessPolicy.DeleteRecordsIFNotCAH",
                               new object[1] { icdoUser.user_serial_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
        }


        public bool FindUserByUserName(string astrUserName)
        {
            if (icdoUser == null)
            {
                icdoUser = new cdoUser();
            }
            DataTable ldtbUser = Select<cdoUser>(new string[1] { "user_id" },
                  new object[1] { astrUserName }, null, null);
            if (ldtbUser.Rows.Count > 0)
            {
                icdoUser.LoadData(ldtbUser.Rows[0]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Insert and delete records from sgs_custom_security
        /// 
        /// </summary>
        private void InsertandDeleteRecord()
        {
            cdoCustomSecurity lcdoCustomSecurity = null;
            for (int i = iarrChangeLog.Count - 1; i >= 0; i--)
            {
                if (iarrChangeLog[i] is cdoCustomSecurity)
                {
                    lcdoCustomSecurity = (cdoCustomSecurity)iarrChangeLog[i];
                    // insert value in sgs_custom_security for first time
                    if ((lcdoCustomSecurity.custom_security_id).Equals(0))
                    {
                        lcdoCustomSecurity.Insert();
                    }
                    else
                    {
                        // if old and custom security levels are same then delete the record
                        if (lcdoCustomSecurity.custom_security_level_value == lcdoCustomSecurity.old_security_value.ToString())
                        {
                            lcdoCustomSecurity.Delete();
                            lcdoCustomSecurity.ienuObjectState = ObjectState.Select;
                            iarrChangeLog.RemoveAt(i);
                        }
                        else
                        {
                            lcdoCustomSecurity.Update();
                        }
                    }
                }
            }

        }

        public static utlUserInfo ValidateUser(string astrUserId, string astrPassword)
        {
            //Create this object and populate it as it goes
            utlUserInfo lobjUtlUserInfo = new utlUserInfo();
            lobjUtlUserInfo.istrUserId = astrUserId;

            //Populate default values
            lobjUtlUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.NotYetSignedIn;

            DataTable ldtbList = Select<cdoUser>(new string[1] { "user_id" },
                                    new object[1] { astrUserId }, null, null);
            cdoUser lobjUser = null;
            if (ldtbList.Rows.Count == 0)
            {
                //Due to no matches
                lobjUtlUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.InvalidUser;
                lobjUtlUserInfo.istrMessage = "Invalid userid or Password";
                lobjUtlUserInfo.istrMessageInternal = "Invalid UserID or Password";
                return lobjUtlUserInfo;
            }
            else if (ldtbList.Rows.Count > 1)
            {
                //Due to multiple matches
                lobjUtlUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.InvalidUser;
                lobjUtlUserInfo.istrMessage = "Invalid user. ";
                lobjUtlUserInfo.istrMessageInternal = "Invalid user - there are multiple user records for this userid";
                return lobjUtlUserInfo;
            }
            else
            {
                lobjUser = new cdoUser();
                lobjUser.LoadData(ldtbList.Rows[0]);
                lobjUser.GetUserPasswordDisplay();
            }


            //Assign the proper value to the user info object            
            lobjUtlUserInfo.iintUserSerialId = lobjUser.user_serial_id;

            //Two possible value are Active and Inactive
            if (lobjUser.user_status_value != "A")
            {
                lobjUtlUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.InvalidUser;
                lobjUtlUserInfo.istrMessage = "User is not Active!";
                lobjUtlUserInfo.istrMessageInternal = "Invalid user - user status is not active";
                return lobjUtlUserInfo;
            }

            lobjUtlUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.ValidUserInitial;


            //Now compare the pw in encrypted state -- the db value is also encypted
            //string lstrPwEnc = HelperFunction.SagitecEncrypt("", astrPassword);
            //if (lobjUser.user_password_display.Trim() == astrPassword.Trim())
            {
                lobjUtlUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.ValidUserFinal;
                lobjUtlUserInfo.istrFirstName = lobjUser.last_name + ", " + lobjUser.first_name;
                lobjUtlUserInfo.istrMessage = "";
                lobjUtlUserInfo.istrMessageInternal = "";
                return lobjUtlUserInfo;
            }

            //there is a mis-match in pw - Update login attempts
            //the status remains the same as before
            //lobjUtlUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.InvalidUser;
            //lobjUtlUserInfo.istrMessage = "Invalid entry. Please try again";
            //lobjUtlUserInfo.istrMessageInternal = "Invalid UserID or Password.";
            //return lobjUtlUserInfo;
        }

        #endregion

        /// <summary>
        /// Load User Roles by Effective Date
        /// </summary>
        /// <param name="adtStartDate">Start Date</param>

    }
}

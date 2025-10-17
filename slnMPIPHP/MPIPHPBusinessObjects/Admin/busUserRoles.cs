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


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busUserRoles : busMPIPHPBase
    {
		public busUserRoles()
		{
		}

        public cdoUserRoles icdoUserRoles { get; set; }
        public cdoRoles icdoRoles { get; set; }
        public busUser ibusUser { set; get; }

        public override long iintPrimaryKey
        {
            get
            {
                return icdoUserRoles.user_serial_id;
            }
        }


        public bool FindUserRoles(int aintUserSerialId, int aintRoleId)
		{
			bool lblnResult = false;
			if (icdoUserRoles == null)
			{
				icdoUserRoles = new cdoUserRoles();
			}
            
            if (icdoUserRoles.SelectRow(new object[2] { aintUserSerialId, aintRoleId }) && (icdoUserRoles.effective_end_date == DateTime.MinValue || (icdoUserRoles.effective_end_date != DateTime.MinValue && icdoUserRoles.effective_end_date > DateTime.Now)))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadUser()
		{
			if (ibusUser == null)
			{
				ibusUser = new busUser();
			}
			ibusUser.FindUser(icdoUserRoles.user_serial_id);
		}

        public void LoadRoles()
        {
            if (icdoRoles == null)
            {
                icdoRoles = new cdoRoles();
                icdoRoles.SelectRow(new object[1] { icdoUserRoles.role_id });
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadRoles();
        }
        ////checking the user status 
        //public bool CheckStatus()
        //{
        //    DataTable ldtbList = busMPIPHPBase.Select("cdoUserRoles.GetUserWithActiveStatus", new object[1] { _icdoUserRoles.user_serial_id });
        //    if (ldtbList.Rows.Count >= 1)
        //    {
        //        return true;
        //    }
        //    return false;

        //}     

        /// <summary>
        /// Returns the collection of the active assigned users for the role passed as an argument.
        /// </summary>
        /// <param name="aintRoleId">Role id.</param>
        /// <returns>Collection of the active users who belongs to this role.</returns>
        public Collection<busUserRoles> GetActiveUsersByRoleId(int aintRoleId)
        {
            DataTable ldtbActiveUsersByRole = busBase.Select("cdoUserRoles.GetActiveUsersByRoleId", new object[1] { aintRoleId });
            Collection<busUserRoles> lclbActiveUsersForRole = GetCollection<busUserRoles>(ldtbActiveUsersByRole, "icdoUserRoles");

            return lclbActiveUsersForRole;
        }

        /// <summary>
        /// Returns the collection of the roles for the user serial id passed as an argument.
        /// </summary>
        /// <param name="aintUserSerialId">User Serial id.</param>
        /// <returns>Collection of the active users who belongs to this role.</returns>
        public Collection<busUserRoles> GetRolesByUserSerialId(int aintUserSerialId)
        {
            DataTable ldtbRolesByUser = busBase.Select("cdoUserRoles.GetRolesByUserSerialId", new object[1] { aintUserSerialId });
            Collection<busUserRoles> lclbRolesByUser = GetCollection<busUserRoles>(ldtbRolesByUser, "icdoUserRoles");

            return lclbRolesByUser;
        }


	}
}

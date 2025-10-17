using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MPIPHP.CustomDataObjects;
using System.Data;
using System.Collections;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busReassignWorkDetail : busMPIPHPBase
    {
        public busActivityInstance ibusSolutionActivityInstance { get; set; }
        public string istrReassignedUser { get; set; }
        public Collection<cdoUser> iclcUsersByRoles { get; set; }

        public void LoadReassigmentDetail(int aintActivityInstanceID)
        {
            if (ibusSolutionActivityInstance == null)
                ibusSolutionActivityInstance = new busActivityInstance();

            if (ibusSolutionActivityInstance.FindActivityInstance(aintActivityInstanceID))
            {
                ibusSolutionActivityInstance.LoadActivity();
                ibusSolutionActivityInstance.ibusActivity.LoadRoles();
                ibusSolutionActivityInstance.LoadProcessInstance();
                ibusSolutionActivityInstance.ibusProcessInstance.ibusProcess = ibusSolutionActivityInstance.ibusActivity.ibusProcess;
                //ibusSolutionActivityInstance.ibusProcessInstance.LoadOrganization();
                ibusSolutionActivityInstance.ibusProcessInstance.LoadPerson();
            }
        }

        public Collection<cdoUser> LoadUserByActivityRole()
        {
            DataTable ldtpUsersList = busMPIPHPBase.Select("cdoActivityInstance.LoadUsersByActivityRole",
                new object[1] { ibusSolutionActivityInstance.ibusActivity.icdoActivity.activity_id });
            iclcUsersByRoles = cdoUser.GetCollection<cdoUser>(ldtpUsersList);
            return iclcUsersByRoles;
        }

        public ArrayList btnReassign_Click()
        {
            ArrayList larrList = new ArrayList();
            ibusSolutionActivityInstance.icdoActivityInstance.checked_out_user = istrReassignedUser;
            ibusSolutionActivityInstance.icdoActivityInstance.iblnNeedHistory = true;
            ibusSolutionActivityInstance.icdoActivityInstance.Update();
            iintMessageID = 8;            
            LoadReassigmentDetail(ibusSolutionActivityInstance.icdoActivityInstance.activity_instance_id);
            larrList.Add(this);
            return larrList;
        }
    }
}

#region Using directives

using NeoBase.BPMDataObjects;
using NeoBase.Common;
using Sagitec.Bpm;
using Sagitec.Common;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
#endregion

namespace NeoBase.BPM
{
    [Serializable]
    public class busNeobaseBpmActivity : busBpmActivity
    {
        #region [Properties]
        /// <summary>
        /// String to hold rejection reasons.
        /// </summary>
        public string istrRejectionReason { get; set; }

        public string istrExceptionReason { get; set; }

        public string istrMissingInformation { get; set; }


        /// <summary>
        /// Collection to hold rejection reasons.
        /// </summary>
        public Collection<busBpmActivityRejection> iclbBpmActivityRejection { get; set; }

        public Collection<busBpmActivityRejection> iclbBpmActivityException { get; set; }


        /// <summary>
        /// Collection to hold missing information.
        /// </summary>
        public Collection<busBpmActivityRejection> iclbBpmActivityMissingInformation { get; set; }

        /// <summary>
        /// Property to hold if activity is the Approve activity.
        /// </summary>
        public bool iblnIsApprovalActivity { get; set; }

        #endregion [Properties]


        /// <summary>
        /// Collection for Activity Checklist.
        /// </summary>

        public busNeobaseBpmActivity()
            : base()
        {
            iclbBpmActivityChecklist = new Collection<busBpmActivityChecklist>();
        }
      

        /// <summary>
        /// Changes for 6.0.7.1 framework
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public busBpmActvChklGroup GetGroupData(string id)
        {
            int groupid = 0;
            var lbusBpmActivityChecklistGroup = new busBpmActvChklGroup();
            int.TryParse(id, out groupid);
            lbusBpmActivityChecklistGroup.FindByPrimaryKey(groupid);
            if (!int.TryParse(id, out groupid))

                lbusBpmActivityChecklistGroup.icdoBpmActvChklGroup.group_name = id;
            return lbusBpmActivityChecklistGroup;
        }
        /// <summary>
        /// LoadUserRoles
        /// </summary>
        //public new void LoadBpmActivityChecklist()
        //{
        //    DataTable ldtbList = Select("entBpmActivityChecklist.ActiveActivityChecklist", new object[] { icdoBpmActivity.activity_id });
        //    iclbBpmActivityChecklist = GetCollection<busBpmActivityChecklist>(ldtbList, "icdoBpmActivityChecklist");
        //}
        #region [Public Methods]
        /// <summary>
        /// LoadUserRoles
        /// </summary>
        //public new void LoadBpmActivityChecklist()
        //{
        //    DataTable ldtbList = Select("entBpmActivityChecklist.ActiveActivityChecklist", new object[] { icdoBpmActivity.activity_id });
        //    iclbBpmActivityChecklist = GetCollection<busBpmActivityChecklist>(ldtbList, "icdoBpmActivityChecklist");
        //}
        /// <summary>
        /// To load BPM activity reasons
        /// </summary>
        public void LoadBPMActivityReasons()
        {
            DataTable ldtbList = Select<doBpmActivityRejection>(
                new string[1] { enmBpmActivity.activity_id.ToString() },
                new object[1] { icdoBpmActivity.activity_id }, null, null);

            iclbBpmActivityRejection = GetCollection<busBpmActivityRejection>(ldtbList, "icdoBpmActivityRejection");
        }
        /// <summary>
        /// To load BPM activity rejection reasons 
        /// </summary>
        public void LoadBPMActivityRejectionReasons()
        {
            iclbBpmActivityRejection = GetBPMActivityReasonsByReasonType("R");
        }

        /// <summary>
        /// To load BPM activity exceptions reasons
        /// </summary>
        public void LoadBPMActivityExceptionReasons()
        {
            iclbBpmActivityException = GetBPMActivityReasonsByReasonType("E");
        }

        /// <summary>
        /// To load BPM activity missing information
        /// </summary>
        public void LoadBPMActivityMissingInformation()
        {
            iclbBpmActivityMissingInformation = GetBPMActivityReasonsByReasonType("M");
        }
        /// <summary>
        /// this methods returns activity reasons by reason type
        /// </summary>
        /// <param name="astrReasonType">reason type can be Rejection / Exception / missing information</param>
        /// <returns></returns>
        public Collection<busBpmActivityRejection> GetBPMActivityReasonsByReasonType(string astrReasonType)
        {
            DataTable ldtbList = Select("entBpmActivityRejection.GetRejectionsByType", new object[2] { this.icdoBpmActivity.activity_id, astrReasonType });
            return GetCollection<busBpmActivityRejection>(ldtbList, "icdoBpmActivityRejection");
        }

        /// <summary>
        /// To delete BPM activity rejection reasons
        /// </summary>
        /// <param name = "aarrSelectedObjects" ></ param >
        /// < returns ></ returns >
        //public ArrayList DeleteBPMActivityRejectionReason(ArrayList aarrSelectedObjects)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    foreach (busBpmActivityRejection lbusBpmActivityRejection in aarrSelectedObjects)
        //    {
        //        lbusBpmActivityRejection.Delete();
        //    }

        //    LoadBPMActivityRejectionReasons();
        //    larrResult.Add(this);
        //    return larrResult;
        //}
        /// <summary>
        /// To add BPM activity rejection reasons
        /// </summary>
        /// <returns></returns>
        public ArrayList AddBPMActivityRejectionReason()
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = null;

            if (istrRejectionReason.IsNullOrEmpty())
            {
                //Rejection reason not selected.
                lutlError = AddError(busNeoBaseConstants.Message.MessageID_20007037, String.Empty);
                larrResult.Add(lutlError);
            }
            else
            {
                //Rejection reason already added.
                int lintCnt = iclbBpmActivityRejection.Where(lBpmActivityRejection => lBpmActivityRejection.icdoBpmActivityRejection.rejection_reason_value == istrRejectionReason).Count();

                if (lintCnt > 0)
                {
                    lutlError = AddError(busNeoBaseConstants.Message.MessageID_20007038, String.Empty);
                    larrResult.Add(lutlError);
                }
            }

            if (larrResult.Count == 0)
            {
                busBpmActivityRejection lbusBpmActivityRejection = new busBpmActivityRejection();
                lbusBpmActivityRejection.icdoBpmActivityRejection = new doBpmActivityRejection();
                lbusBpmActivityRejection.icdoBpmActivityRejection.activity_id = this.icdoBpmActivity.activity_id;
                lbusBpmActivityRejection.icdoBpmActivityRejection.rejection_reason_value = istrRejectionReason;
                lbusBpmActivityRejection.icdoBpmActivityRejection.Insert();

                iclbBpmActivityRejection.Add(lbusBpmActivityRejection);
                larrResult.Add(this);
            }

            return larrResult;
        }
        /// <summary>
        /// To add bpm activity exception reason
        /// </summary>
        /// <returns></returns>
        public ArrayList AddBPMActivityExceptionReason()
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = null;

            if (istrExceptionReason.IsNullOrEmpty())
            {
                //Rejection reason not selected.
                lutlError = AddError(busNeoBaseConstants.Message.MessageID_20007040, String.Empty);
                larrResult.Add(lutlError);
            }
            else
            {
                //Rejection reason already added.
                int lintCnt = iclbBpmActivityException.Where(lBpmActivityException => lBpmActivityException.icdoBpmActivityRejection.rejection_reason_value == istrExceptionReason).Count();

                if (lintCnt > 0)
                {
                    lutlError = AddError(busNeoBaseConstants.Message.MessageID_20007041, String.Empty);
                    larrResult.Add(lutlError);
                }
            }

            if (larrResult.Count == 0)
            {
                busBpmActivityRejection lbusBpmActivityRejection = new busBpmActivityRejection();
                lbusBpmActivityRejection.icdoBpmActivityRejection = new doBpmActivityRejection();
                lbusBpmActivityRejection.icdoBpmActivityRejection.activity_id = this.icdoBpmActivity.activity_id;
                lbusBpmActivityRejection.icdoBpmActivityRejection.rejection_reason_value = istrExceptionReason;
                lbusBpmActivityRejection.icdoBpmActivityRejection.Insert();

                iclbBpmActivityException.Add(lbusBpmActivityRejection);
                larrResult.Add(this);
            }

            return larrResult;
        }

        /// <summary>
        /// To add BPM activity missing information
        /// </summary>
        /// <returns></returns>
        public ArrayList AddBPMActivityMissingInformation()
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = null;

            if (istrMissingInformation.IsNullOrEmpty())
            {
                //Rejection reason not selected.  REVISIT THIS MJS 20007037?
                lutlError = AddError(busNeoBaseConstants.Message.MessageID_20007042, String.Empty);
                larrResult.Add(lutlError);
            }
            else
            {
                //Rejection reason already added.
                int lintCnt = iclbBpmActivityMissingInformation.Where(lBpmActivityMissingInformation => lBpmActivityMissingInformation.icdoBpmActivityRejection.rejection_reason_value == istrMissingInformation).Count();

                if (lintCnt > 0)
                {
                    // REVISIT THIS MJS 20007038?
                    lutlError = AddError(busNeoBaseConstants.Message.MessageID_20007043, String.Empty);
                    larrResult.Add(lutlError);
                }
            }

            if (larrResult.Count == 0)
            {
                busBpmActivityRejection lbusBpmActivityMissingInformation = new busBpmActivityRejection();
                lbusBpmActivityMissingInformation.icdoBpmActivityRejection = new doBpmActivityRejection();
                lbusBpmActivityMissingInformation.icdoBpmActivityRejection.activity_id = this.icdoBpmActivity.activity_id;
                lbusBpmActivityMissingInformation.icdoBpmActivityRejection.rejection_reason_value = istrMissingInformation;
                lbusBpmActivityMissingInformation.icdoBpmActivityRejection.Insert();

                iclbBpmActivityMissingInformation.Add(lbusBpmActivityMissingInformation);
                larrResult.Add(this);
            }

            return larrResult;
        }
        /// <summary>
        /// To Load BPM activity based on activity id
        /// </summary>
        /// <param name="aintActivityId"></param>
        /// <returns></returns>
        public bool FindSolBpmActivity(int aintActivityId)
        {
            bool lblnResult = false;
            if (icdoBpmActivity == null)
            {
                icdoBpmActivity = new doBpmActivity();
            }
            if (icdoBpmActivity.SelectRow(new object[1] { aintActivityId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public bool FindBPMActivity(int aintActivityId)
        {
            if (FindSolBpmActivity(aintActivityId))
            {
                LoadBpmProcess();
                LoadBPMActivityRejectionReasons();
                LoadBPMActivityExceptionReasons();
                LoadBPMActivityMissingInformation();
                //LoadBPMActivityUserExclusions();

                if (this.icdoBpmActivity.activity_type_value == busNeoBaseConstants.BPM.BPM_USER_TASK)
                {
                    busBpmCase lbusBpmCase = busBpmCase.GetBpmCase(this.ibusBpmProcess.icdoBpmProcess.case_id);
                    busBpmProcess lbusBpmProcess = lbusBpmCase.iclbBpmProcess.Where(lProcess => lProcess.icdoBpmProcess.process_id == this.ibusBpmProcess.icdoBpmProcess.process_id).FirstOrDefault();

                    if (lbusBpmProcess != null)
                    {
                        busBpmActivity lbusBpmActivity = lbusBpmProcess.iclbBpmActivity.Where(lActivity => lActivity.icdoBpmActivity.activity_id
                                                            == aintActivityId).FirstOrDefault();

                        utlBPMTask lutlBPMTask = (utlBPMTask)lbusBpmActivity.GetBpmActivityDetails();
                        this.iblnIsApprovalActivity = lutlBPMTask.iblnIsApprovalActivity;
                    }
                }
                return true;
            }

            return false;
        }
        #endregion
    }
}

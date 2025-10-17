#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using MPIPHP.CustomDataObjects;
using MPIPHP.Common;

#endregion

namespace MPIPHP.BusinessTier
{
    public class srvAdmin : srvMPIPHP
    {
        public srvAdmin()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public busCode FindCode(int aintCodeId)
        {
            busCode lobjCode = new busCode();
            if (lobjCode.FindCode(aintCodeId))
            {
                lobjCode.LoadCodeValues();
            }
            return lobjCode;
        }


        public busCode NewCode()
        {
            busCode lobjCode = new busCode();
            lobjCode.icdoCode = new cdoCode();
            return lobjCode;
        }

        public busCodeLookup LoadCodes(DataTable adtbSearchResult)
        {
            busCodeLookup lobjCodeLookup = new busCodeLookup();
            lobjCodeLookup.LoadCodes(adtbSearchResult);
            return lobjCodeLookup;
        }

        public busCodeValue FindCodeValue(int aintCodeValueId)
        {
            busCodeValue lobjCodeValue = new busCodeValue();
            if (lobjCodeValue.FindCodeValue(aintCodeValueId))
            {
                lobjCodeValue.LoadCode();
            }
            return lobjCodeValue;
        }

        public busCodeValue NewCodeValue(int aintCodeId)
        {
            busCodeValue lobjCodeValue = new busCodeValue();
            lobjCodeValue.icdoCodeValue = new cdoCodeValue();
            lobjCodeValue.icdoCodeValue.code_id = aintCodeId;
            lobjCodeValue.LoadCode();
            return lobjCodeValue;
        }

        public busMessages FindMessage(int aintMessageId)
        {
            busMessages lobjMessage = new busMessages();
            if (lobjMessage.FindMessage(aintMessageId))
            {
            }
            return lobjMessage;
        }

        public busMessages NewMessage()
        {
            busMessages lobjMessages = new busMessages();
            lobjMessages.icdoMessages = new cdoMessages();            
            return lobjMessages;
        }

        public busMessagesLookup LoadMessages(DataTable adtbSearchResult)
        {
            busMessagesLookup lobjMessageLookup = new busMessagesLookup();
            lobjMessageLookup.LoadMessages(adtbSearchResult);
            return lobjMessageLookup;
        }

        public busUser NewUser(int aintUserSerialId)
        {
            busUser lobjUser = new busUser();
            lobjUser.icdoUser = new cdoUser();
            lobjUser.iclbUserRoles = new Collection<busUserRoles>();
            lobjUser.iclbSecurity = new Collection<busSecurity>();
            lobjUser.iclbCustomAccessPolicy = new Collection<busCustomAccessPolicy>();
            lobjUser.icdoUser.user_serial_id = aintUserSerialId;
            return lobjUser;
        }

        public busUser FindUser(int aintUserSerialId)
        {
            busUser lobjUser = new busUser();
            if (lobjUser.FindUser(aintUserSerialId))
            {
                lobjUser.LoadUserRoles();
                lobjUser.LoadSecurity();
                lobjUser.LoadCustomAccessPolicys();
                //lobjUser.LoadUnassignedRolesByUser();
            }
            return lobjUser;
        }

        public busUserLookup LoadUser(DataTable adtbSearchResult)
        {
            busUserLookup lobjUserLookup = new busUserLookup();
            lobjUserLookup.LoadUsers(adtbSearchResult);
            return lobjUserLookup;
        }

        public busUserRoles FindUserRoles(int aintUserSerialId, int aintRoleId)
        {
            busUserRoles lobjUserRoles = new busUserRoles();
            if (lobjUserRoles.FindUserRoles(aintUserSerialId, aintRoleId))
            {
                lobjUserRoles.LoadUser();
                lobjUserRoles.LoadRoles();
            }
            return lobjUserRoles;
        }

        public busUserRoles NewUserRoles(int aintUserSerialId)
        {
            busUserRoles lobjUserRoles = new busUserRoles();
            lobjUserRoles.icdoUserRoles = new cdoUserRoles();
            lobjUserRoles.icdoUserRoles.user_serial_id = aintUserSerialId;
            lobjUserRoles.LoadUser();
            return lobjUserRoles;
        }

        public busResources FindResource(int aintResourceId)
        {
            busResources lobjResource = new busResources();
            if (lobjResource.FindResource(aintResourceId))
            {
                lobjResource.LoadSecurity();
                lobjResource.LoadUsers();
                lobjResource.LoadCustomSecurityDetails();
                //load roles not assigned
                lobjResource.LoadUnassignedRolesByResource();
            }
            return lobjResource;
        }

        public busResourcesLookup LoadResources(DataTable adtbSearchResult)
        {
            busResourcesLookup lobjResourceLookup = new busResourcesLookup();
            lobjResourceLookup.LoadResources(adtbSearchResult);
            return lobjResourceLookup;
        }

        public busPERSLinkRoles FindRole(int aintRoleId)
        {
            busPERSLinkRoles lobjRole = new busPERSLinkRoles();
            if (aintRoleId == 0)
            {
                lobjRole.icdoRoles = new cdoRoles();
                lobjRole.iclbSecurityByRoles = new Collection<busSecurity>();
                lobjRole.iclbUsers = new Collection<busUser>();
            }
            if (lobjRole.FindRole(aintRoleId))
            {
                lobjRole.LoadSecurity();
                lobjRole.LoadUsers();
            }
            return lobjRole;
        }
        
        public busRolesLookup LoadRoles(DataTable adtbSearchResult)
        {
            busRolesLookup lobjRoleLookup = new busRolesLookup();
            lobjRoleLookup.LoadRoles(adtbSearchResult);
            return lobjRoleLookup;
        }

        public busSecurity FindSecurity(int aintResourceId, int aintRoleId)
        {
            busSecurity lobjSecurity = new busSecurity();
            if (lobjSecurity.FindSecurity(aintResourceId, aintRoleId))
            {
                lobjSecurity.LoadResource();
                lobjSecurity.LoadRole();
            }
            return lobjSecurity;
        }

        public busSystemManagement FindSystemManagement()
        {
            busSystemManagement lobjSystemManagement = new busSystemManagement();
            if (lobjSystemManagement.FindSystemManagement())
            {
            }
            return lobjSystemManagement;
        }

        public busSystemPaths FindPath(int aintPathId)
        {
            busSystemPaths lobjSystemPath = new busSystemPaths();
            if (lobjSystemPath.FindPath(aintPathId))
            {
            }
            return lobjSystemPath;
        }

        public busSystemPathsLookup LoadPaths(DataTable adtbSearchResult)
        {
            busSystemPathsLookup lobjSystemPathsLookup = new busSystemPathsLookup();
            lobjSystemPathsLookup.LoadSystemPaths(adtbSearchResult);
            return lobjSystemPathsLookup;
        }

        public busBatchSchedule FindBatchSchedule(int aintBatchScheduleId)
        {
            busBatchSchedule lobjBatchSchedule = new busBatchSchedule();
            if (lobjBatchSchedule.FindBatchSchedule(aintBatchScheduleId))
            {
            }
            return lobjBatchSchedule;
        }

        public busBatchScheduleLookup LoadBatchSchedules(DataTable adtbSearchResult)
        {
            busBatchScheduleLookup lobjBatchScheduleLookup = new busBatchScheduleLookup();
            lobjBatchScheduleLookup.LoadBatchSchedule(adtbSearchResult);
            return lobjBatchScheduleLookup;
        }

        public busProcessLogLookup LoadProcessLog(DataTable adtbSearchResult)
        {
            busProcessLogLookup lobjProcessLogLookup = new busProcessLogLookup();
            lobjProcessLogLookup.LoadProcessLog(adtbSearchResult);
            return lobjProcessLogLookup;
        }

        public busFile FindFile(int aintFileId)
        {
            busFile lobjFile = new busFile();
            if (lobjFile.FindFile(aintFileId))
            {
            }
            return lobjFile;
        }

        public busFileLookup LoadFiles(DataTable adtbSearchResult)
        {
            busFileLookup lobjFileLookup = new busFileLookup();
            lobjFileLookup.LoadFiles(adtbSearchResult);
            return lobjFileLookup;
        }

        public busFileLayout FinFileLayout(int aintFileId, string astrTransactionCode)
        {
            busFileLayout lobjFileLayout = new busFileLayout();
            lobjFileLayout.FindFileLayout(aintFileId, astrTransactionCode);
            return lobjFileLayout;
        }

        public busFileHdr FindFileHdr(int aintFileHdrId)
        {
            busFileHdr lobjFileHdr = new busFileHdr();
            if (lobjFileHdr.FindFileHdr(aintFileHdrId))
            {
                lobjFileHdr.LoadFile();
                lobjFileHdr.LoadStatusSummary();
            }
            return lobjFileHdr;
        }

        public busFileHdrLookup LoadFileHdrs(DataTable adtbSearchResult)
        {
            busFileHdrLookup lobjFileHdrLookup = new busFileHdrLookup();
            lobjFileHdrLookup.LoadFileHdrs(adtbSearchResult);
            return lobjFileHdrLookup;
        }

        public busFileDtl FindFileDtl(int aintFileDtlId)
        {
            busFileDtl lobjFileDtl = new busFileDtl();
            if (lobjFileDtl.FindFileDtl(aintFileDtlId))
            {
                lobjFileDtl.LoadFileHdr();
                lobjFileDtl.LoadFileDtlErrors();
            }
            return lobjFileDtl;
        }

        public busFileDtlLookup LoadFileDtls(DataTable adtbSearchResult)
        {
            busFileDtlLookup lobjFileDtlLookup = new busFileDtlLookup();
            lobjFileDtlLookup.LoadFileDtls(adtbSearchResult);
            return lobjFileDtlLookup;
        }

        public busUser ChangeUserPassword()
        {
            busUser lobjUser = new busUser();
            if (lobjUser.FindUser(utlPassInfo.iobjPassInfo.iintUserSerialID))
            {
            }
            return lobjUser;
        }


		public busSecurity FindSecurity(int aintresourceid)
		{
			busSecurity lobjSecurity = new busSecurity();
			if (lobjSecurity.FindSecurity(aintresourceid))
			{
				lobjSecurity.LoadCustomSecuritys();
				lobjSecurity.LoadResourcess();
			}

			return lobjSecurity;
		}

		public busCustomSecurity FindCustomSecurity(int aintcustomsecurityid)
		{
			busCustomSecurity lobjCustomSecurity = new busCustomSecurity();
			if (lobjCustomSecurity.FindCustomSecurity(aintcustomsecurityid))
			{
			}

			return lobjCustomSecurity;
		}

        //public busUser FindUser(int aintuserserialid)
        //{
        //    busUser lobjUser = new busUser();
        //    if (lobjUser.FindUser(aintuserserialid))
        //    {
        //        lobjUser.LoadCustomSecuritys();
        //        lobjUser.LoadUserRoless();
        //    }

        //    return lobjUser;
        //}

		public busCustomAccessPolicy FindCustomAccessPolicy(int aintcustomaccesspolicyid)
		{
			busCustomAccessPolicy lobjCustomAccessPolicy = new busCustomAccessPolicy();
			if (lobjCustomAccessPolicy.FindCustomAccessPolicy(aintcustomaccesspolicyid))
			{
			}

			return lobjCustomAccessPolicy;
		}

        //public busResources FindResources(int aintresourceid)
        //{
        //    busResources lobjResources = new busResources();
        //    if (lobjResources.FindResources(aintresourceid))
        //    {
        //        lobjResources.LoadSecurity();
        //        lobjResources.LoadUsers();
        //        lobjResources.LoadCustomSecurityDetails();
        //        //load roles not assigned
        //        lobjResources.LoadUnassignedRolesByResource();
        //    }

        //    return lobjResources;
        //}

		public busUserSecurityHistory FindUserSecurityHistory(int aintsgsusersecurityhistoryid)
		{
			busUserSecurityHistory lobjUserSecurityHistory = new busUserSecurityHistory();
			if (lobjUserSecurityHistory.FindUserSecurityHistory(aintsgsusersecurityhistoryid))
			{
			}

			return lobjUserSecurityHistory;
		}



        public busJobHeader FindJobHeader(int Aintjobheaderid)
        {
            busJobHeader lobjJobHeader = new busJobHeader();
            if (lobjJobHeader.FindJobHeader(Aintjobheaderid))
            {
                lobjJobHeader.LoadJobDetail(true);
                lobjJobHeader.LoadJobDetailStepInfo();
                lobjJobHeader.LoadJobSchedule();
                lobjJobHeader.LoadErrors();
                lobjJobHeader.LoadProcessLogs();
            }
            return lobjJobHeader;
        }

        public busJobHeader NewJobHeader()
        {
            busJobHeader lobjJobHeader = new busJobHeader();
            lobjJobHeader.icdoJobHeader = new cdoJobHeader();
            lobjJobHeader.icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_REVIEW;
            lobjJobHeader.icdoJobHeader.status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(BatchHelper.CODE_ID_FOR_JOB_HEADER_STATUS, lobjJobHeader.icdoJobHeader.status_value);

            lobjJobHeader.EvaluateInitialLoadRules();
            return lobjJobHeader;
        }

        public busJobDetail NewJobDetail(int aintJobHeaderId)
        {
            busJobDetail lobjJobDetail = new busJobDetail();
            lobjJobDetail.icdoJobDetail = new cdoJobDetail();
            lobjJobDetail.icdoJobDetail.job_header_id = aintJobHeaderId;
            lobjJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_REVIEW;
            lobjJobDetail.icdoJobDetail.status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(BatchHelper.CODE_ID_FOR_JOB_DETAIL_STATUS, lobjJobDetail.icdoJobDetail.status_value);

            lobjJobDetail.LoadJobHeader();
            return lobjJobDetail;
        }

        public busJobScheduleDetail NewJobScheduleDetail(int aintJobScheduleId)
        {
            busJobScheduleDetail lobjJobScheduleDetail = new busJobScheduleDetail();
            lobjJobScheduleDetail.icdoJobScheduleDetail = new cdoJobScheduleDetail();
            lobjJobScheduleDetail.icdoJobScheduleDetail.job_schedule_id = aintJobScheduleId;
            lobjJobScheduleDetail.icdoJobScheduleDetail.status_value = BatchHelper.JOB_SCHEDULE_DETAIL_STATUS_REVIEW;
            lobjJobScheduleDetail.icdoJobScheduleDetail.status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(BatchHelper.CODE_ID_FOR_JOB_SCHEDULE_DETAIL_STATUS, lobjJobScheduleDetail.icdoJobScheduleDetail.status_value);

            lobjJobScheduleDetail.LoadJobSchedule();
            return lobjJobScheduleDetail;
        }

        public busJobDetail FindJobDetail(int Aintjobdetailid)
        {
            busJobDetail lobjJobDetail = new busJobDetail();
            if (lobjJobDetail.FindJobDetail(Aintjobdetailid))
            {
                lobjJobDetail.LoadJobHeader();
                lobjJobDetail.LoadStepInfo();
                lobjJobDetail.LoadJobParameters();
                lobjJobDetail.LoadErrors();

            }

            return lobjJobDetail;
        }

        public busJobParameters FindJobParameters(int Aintjobparametersid)
        {
            busJobParameters lobjJobParameters = new busJobParameters();
            if (lobjJobParameters.FindJobParameters(Aintjobparametersid))
            {
            }
            return lobjJobParameters;
        }

        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            switch (astrFormName)
            {
                case busConstant.MPIPHPBatch.FORM_JOB_SCHEDULE_LOOKUP:
                    busJobScheduleLookup lbusJobScheduleLookup = new busJobScheduleLookup();
                    return lbusJobScheduleLookup.ValidateNew(ahstParam);
            }
            return base.ValidateNew(astrFormName, ahstParam);
        }
        public busJobSchedule NewJobSchedule(string astrFreqTypeValue)
        {
            busJobSchedule lobjJobSchedule = new busJobSchedule();
            lobjJobSchedule.icdoJobSchedule = new cdoJobSchedule();
            lobjJobSchedule.iclbJobScheduleDetail = new Collection<busJobScheduleDetail>();

            lobjJobSchedule.icdoJobSchedule.frequency_type_value = astrFreqTypeValue;
            lobjJobSchedule.icdoJobSchedule.frequency_type_description =
                                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(BatchHelper.CODE_ID_FOR_JOB_SCHEDULE_FREQUENCY_TYPE, lobjJobSchedule.icdoJobSchedule.frequency_type_value);

            // Set default values for the fields.
            lobjJobSchedule.icdoJobSchedule.lstrEndDatePresent = BatchHelper.JOB_SCHEDULE_NO_END_DATE_PRESENT;
            lobjJobSchedule.icdoJobSchedule.lstrSubdayFrequency = BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_ONCE;

            lobjJobSchedule.icdoJobSchedule.status_value = BatchHelper.JOB_SCHEDULE_HEADER_STATUS_REVIEW;
            lobjJobSchedule.icdoJobSchedule.status_description =
                                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(BatchHelper.CODE_ID_FOR_JOB_SCHEDULE_STATUS, lobjJobSchedule.icdoJobSchedule.status_value);

            lobjJobSchedule.icdoJobSchedule.start_date = DateTime.Now;
            lobjJobSchedule.EvaluateInitialLoadRules();
            return lobjJobSchedule;
        }

        public busJobSchedule FindJobSchedule(int Aintjobscheduleid)
        {
            busJobSchedule lobjJobSchedule = new busJobSchedule();

            if (lobjJobSchedule.FindJobSchedule(Aintjobscheduleid))
            {
                lobjJobSchedule.PrepareUIControlsBasedOnData();
                lobjJobSchedule.LoadJobScheduleDetails(true);
                lobjJobSchedule.LoadJobScheduleDetailStepInfo();
                lobjJobSchedule.LoadErrors();
                lobjJobSchedule.LoadJobHeaders();

                //To display child grid - start
                lobjJobSchedule.iclbJobScheduleParametersChildGrid = new Collection<busJobScheduleParams>();
                //To load all child from all parent
                foreach (busJobScheduleDetail lbusJobScheduleDetail in lobjJobSchedule.iclbJobScheduleDetail)
                {
                    foreach (busJobScheduleParams lbusJobScheduleParams in lbusJobScheduleDetail.iclbJobScheduleParameters ?? new Collection<busJobScheduleParams>())
                    {
                        lobjJobSchedule.iclbJobScheduleParametersChildGrid.Add(lbusJobScheduleParams);
                    }
                }
                //To load all child from first parent
                //foreach (busJobScheduleParams lbusJobScheduleParams in lobjJobSchedule.iclbJobScheduleDetail.FirstOrDefault().iclbJobScheduleParameters ?? new Collection<busJobScheduleParams>())
                //{
                //    lobjJobSchedule.iclbJobScheduleParametersChildGrid.Add(lbusJobScheduleParams);
                //}
                //To display child grid - end
            }
            return lobjJobSchedule;
        }

        public busJobScheduleDetail FindJobScheduleDetail(int Aintjobscheduledetailid)
        {
            busJobScheduleDetail lobjJobScheduleDetail = new busJobScheduleDetail();
            if (lobjJobScheduleDetail.FindJobScheduleDetail(Aintjobscheduledetailid))
            {
                lobjJobScheduleDetail.LoadJobSchedule();
                lobjJobScheduleDetail.LoadStepInfo();
                lobjJobScheduleDetail.LoadJobParameters();
                lobjJobScheduleDetail.GetStepsForJobScheduleId();
                lobjJobScheduleDetail.LoadErrors();
            }

            return lobjJobScheduleDetail;
        }

        public busJobScheduleParams FindJobScheduleParams(int Aintjobscheduleparamsid)
        {
            busJobScheduleParams lobjJobScheduleParams = new busJobScheduleParams();
            if (lobjJobScheduleParams.FindJobScheduleParams(Aintjobscheduleparamsid))
            {
            }

            return lobjJobScheduleParams;
        }

        public busJobHeaderLookup LoadJobHeaders(DataTable adtbSearchResult)
        {
            busJobHeaderLookup lobjJobHeaderLookup = new busJobHeaderLookup();
            lobjJobHeaderLookup.LoadJobHeaders(adtbSearchResult);
            return lobjJobHeaderLookup;
        }

        public busJobScheduleLookup LoadJobSchedules(DataTable adtbSearchResult)
        {
            busJobScheduleLookup lobjJobScheduleLookup = new busJobScheduleLookup();
            lobjJobScheduleLookup.LoadJobSchedules(adtbSearchResult);
            return lobjJobScheduleLookup;
        }

		public busBatchNotification FindBatchNotification(int aintbatchnotificationid)
		{
			busBatchNotification lobjBatchNotification = new busBatchNotification();
			if (lobjBatchNotification.FindBatchNotification(aintbatchnotificationid))
			{
			}

			return lobjBatchNotification;
		}

		public busPlan FindPlan(int aintplanid)
		{
			busPlan lobjPlan = new busPlan();
			if (lobjPlan.FindPlan(aintplanid))
			{
			}

			return lobjPlan;
		}
        public busUploadFile NewUpload()
        {
            busUploadFile lbusUploadFile = new busUploadFile();
            return lbusUploadFile;
        }
    }
}

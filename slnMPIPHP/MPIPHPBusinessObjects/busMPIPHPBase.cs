using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.BusinessTier;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using Sagitec.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busMPIPHPBase : busBase
    {
        static busMPIPHPBase()
        {
            try
            {
                //FM upgrade: 6.0.0.31 build error changes
                //iutlServerDetail = new utlServerDetail();

                //Server IP address is set-up once
                string lstrDns = Dns.GetHostName();
                //FM upgrade: 6.0.0.31 build error changes
                //iutlServerDetail.istrIPAddress = lstrDns;

                System.IO.FileInfo lfi = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8));
                if (lfi != null)
                {
                    //FM upgrade: 6.0.0.31 build error changes
                    //iutlServerDetail.istrReleaseDate = lfi.LastWriteTime.ToString();
                }
            }
            catch
            {
                //Intentionally eating exception because this code is causing exception while completing the WF activity at Dns.GetHostName();
            }
        }

        #region [Members]

        //FM upgrade: 6.0.0.31 build error changes
        //public static utlServerDetail iutlServerDetail;
        /// <summary>
        /// Error Message to be used in the screens. Override default message on button click.
        /// </summary>
        public string istrErrorMessage { get; set; }

        /// <summary>
        /// Indicates if Melissa Data Suggestions are not be retreived
        /// </summary>
        public bool iblnSuppressMelissaSuggestions { get; set; }

        public static bool iblnSurvivorNotExist { get; set; }

        /// <summary>
        /// This property should be overridden in child object 
        /// to indicate its relationship with an organization.
        /// 
        /// This property can be used by system blocks (Correspondence, ECM, IVR) to interrogate 
        /// organization information from any functional block object
        /// As of now it is used by ECM block to fetch list of documents for LOB screens which belong to organization block.
        /// E.g.: busOrgAddress can describe its relationship with its org as
        ///     this.iintOrgID = this.icdoOrgAddress.org_id
        /// </summary>

        public virtual int iintOrgID
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// This property should be overridden in child object 
        /// to indicate its relationship with a person.
        /// 
        /// This property can be used by system blocks (Correspondence, ECM, IVR) to interrogate 
        /// person information from any functional block object
        /// As of now it is used by ECM block to fetch list of documents for LOB screens which belong to person block.
        /// E.g.: busPersonAddress can set iintOrgID=lbusPersonAddress.icdoPersonAddress.org_id
        ///     this.iintPersonID = this.icdoPersonAddress.person_id
        /// </summary>
        public virtual int iintPersonID
        {
            get
            {
                return 0;
            }

        }

        #endregion

        #region Public Methods

        public virtual void SetCorrespondenceFields()
        {
        }

        public static void LoadStandardConstantBookmarks(utlCorresPondenceInfo aobjCorrInfo)
        {
            utlBookmarkFieldInfo lobjField;
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;

            busUser lbusSenderInfo = new busUser();
            if (lbusSenderInfo.FindUser(lobjPassInfo.iintUserSerialID))
            {
                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdSenderFirstName";
                lobjField.istrValue = lbusSenderInfo.icdoUser.first_name.ToUpper();
                lobjField.istrValue = lobjField.istrValue.ToProperCase();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdSenderLastName";
                lobjField.istrValue = lbusSenderInfo.icdoUser.last_name.ToUpper();
                lobjField.istrValue = lobjField.istrValue.ToProperCase();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdLoggedInUserFullName";
                lobjField.istrValue = lbusSenderInfo.icdoUser.User_Name.ToUpper();
                lobjField.istrValue = lobjField.istrValue.ToProperCase();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdLoggedInUserEmailID";
                if (lbusSenderInfo.icdoUser.email_address.IsNotNullOrEmpty())
                    lobjField.istrValue = lbusSenderInfo.icdoUser.email_address.ToLower();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                string istrFirstinitial = string.Empty;
                string istrSecondInitial = string.Empty;
                string istrLoggedInUserInitials = string.Empty;
                if (lbusSenderInfo.icdoUser.User_Name.Contains(" ") || lbusSenderInfo.icdoUser.User_Name.Contains("."))
                {
                    string[] split = lbusSenderInfo.icdoUser.User_Name.Split(new Char[] { ' ', '.' });
                    if (split.Length > 0 && !string.IsNullOrEmpty(split[0]))
                        istrFirstinitial = split[0].Substring(0, 1);
                    if (split.Length > 1 && !string.IsNullOrEmpty(split[1]))
                        istrSecondInitial = split[1].Substring(0, 1);

                    istrLoggedInUserInitials = istrFirstinitial.ToLower() + istrSecondInitial.ToLower();
                }
                else
                {
                    if (!string.IsNullOrEmpty(lbusSenderInfo.icdoUser.User_Name))
                    {
                        istrLoggedInUserInitials = lbusSenderInfo.icdoUser.User_Name.Substring(0, 1).ToLower();
                    }
                }
                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdLoggedInUserInitials";
                lobjField.istrValue = istrLoggedInUserInitials.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdLoggedInUserInitialsInLowerCase";
                lobjField.istrValue = istrLoggedInUserInitials.ToLower();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
            }

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdCurrentDate";
            lobjField.istrValue = DateTime.Now.ToShortDateString();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdCurrentDay";
            lobjField.istrValue = DateTime.Now.DayOfWeek.ToString();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdCurrentTime";
            lobjField.istrValue = DateTime.Now.TimeOfDay.ToString();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
        }

        public utlWhereClause GetWhereClause(object aobjValue1, object aobjValue2,
           string astrFieldName, string astrDataType, string astrOperator, string astrCondition, string astrQueryId)
        {
            utlWhereClause lobjWhereClause = new utlWhereClause();
            lobjWhereClause.iobjValue1 = aobjValue1;
            lobjWhereClause.iobjValue2 = aobjValue2;
            lobjWhereClause.istrQueryId = astrQueryId;
            lobjWhereClause.istrFieldName = astrFieldName;
            lobjWhereClause.istrDataType = astrDataType;
            lobjWhereClause.istrOperator = astrOperator;
            lobjWhereClause.istrCondition = astrCondition;
            return lobjWhereClause;
        }

        public string EncryptPassword(string astrPassword)
        {
            return HelperFunction.SagitecEncryptAES(astrPassword);
        }

        public string DecryptPassword(string astrPassword)
        {
            return HelperFunction.SagitecDecryptAES(astrPassword);
        }

        public static string DeriveMimeTypeFromFileName(string astrFileName)
        {
            string lstrMimeType = "application/octet-stream";
            string[] larrDotSplit = astrFileName.Split(".");
            if (larrDotSplit != null && larrDotSplit.Length > 0)
            {
                string lstrFileExtension = larrDotSplit[larrDotSplit.Length - 1];
                switch (lstrFileExtension.ToLower())
                {
                    case "pdf":
                        lstrMimeType = "application/pdf";
                        break;
                    case "doc":
                    case "docx":
                        lstrMimeType = "application/msword";
                        break;
                    case "html":
                        lstrMimeType = "application/iexplore";
                        break;
                }
            }
            return lstrMimeType;
        }

        public static utlCorresPondenceInfo SetCorrespondence(
            string astrTemplateName, string astrUserID, int aintUserSerialID, ArrayList aarrResult, Hashtable ahtbQueryBkmarks)
        {
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            lobjPassInfo.istrUserID = astrUserID;
            lobjPassInfo.iintUserSerialID = aintUserSerialID;
            //			iobjPassInfo.BeginTransaction();
            cdoCorTemplates lobjCorTemplate = new cdoCorTemplates();
            lobjCorTemplate.LoadByTemplateName(astrTemplateName);

            utlCorresPondenceInfo lobjCorresPondenceInfo =
                lobjPassInfo.isrvMetaDataCache.GetCorresPondenceInfo(astrTemplateName);

            lobjCorresPondenceInfo.istrContactRole = lobjCorTemplate.contact_role_value;

            lobjCorresPondenceInfo.istrTemplatePath = lobjPassInfo.isrvDBCache.GetPathInfo("CorrTmpl");
            lobjCorresPondenceInfo.istrGeneratePath = lobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");

            //Set the auto print flag and printer name from Cor template
            lobjCorresPondenceInfo.istrAutoPrintFlag = lobjCorTemplate.auto_print_flag;
            lobjCorresPondenceInfo.istrPrinterName = lobjCorTemplate.printer_name_description;

            cdoCorTracking lcdoCorTracking = new cdoCorTracking();
            lcdoCorTracking.template_id = lobjCorTemplate.template_id;
            lcdoCorTracking.cor_status_value = "GENR";
            lcdoCorTracking.generated_date = DateTime.Now;
            lcdoCorTracking.created_by = astrUserID;
            lcdoCorTracking.modified_by = astrUserID;
            lcdoCorTracking.comments = "";

            busOrganization lobjOrganization = null;
            busPerson lobjPerson = null;
            GetMPIPHPValues(lobjCorresPondenceInfo, aarrResult, ahtbQueryBkmarks, lcdoCorTracking, lobjOrganization, lobjPerson);

            lcdoCorTracking.person_id = lobjCorresPondenceInfo.iintPersonID;
            lcdoCorTracking.org_contact_id = lobjCorresPondenceInfo.iintOrgContactID;

            //Populate plan for the relevant tracking records
            string lstrValue = busBase.GetBookMarkValue("PlanId", lobjCorresPondenceInfo.icolBookmarkFieldInfo);
            lcdoCorTracking.plan_id = 0;
            if (string.IsNullOrEmpty(lstrValue))
            {
                lcdoCorTracking.plan_id = 0;
            }
            else
            {
                lcdoCorTracking.plan_id = Convert.ToInt32(lstrValue);
            }
            lcdoCorTracking.Insert();
            lobjCorresPondenceInfo.iintCorrespondenceTrackingId = lcdoCorTracking.tracking_id;

            //Generate the file name
            string strSlNo = lobjCorresPondenceInfo.iintCorrespondenceTrackingId.ToString().PadLeft(10, '0');
            lobjCorresPondenceInfo.istrGeneratedFileName = lobjCorresPondenceInfo.istrTemplateName + "-" + strSlNo + ".docx";
            LoadStandardConstantBookmarks(lobjCorresPondenceInfo);
            LoadLastBookmarks(lobjCorresPondenceInfo);
            return lobjCorresPondenceInfo;
        }

        #endregion

        #region Private Methods

        private static void GetMPIPHPValues(
            utlCorresPondenceInfo aobjCorrInfo, ArrayList aarrResult, Hashtable ahstQueryBookMks, cdoCorTracking acdoCorTracking, busOrganization abusOrganization, busPerson abusPerson)
        {
            busBase lobjCorr = null;
            ArrayList larrCorr = aarrResult;
            //Target Business Object Fix
            if ((aarrResult != null) && (aarrResult.Count > 0) && (aobjCorrInfo.ihstFormInfo != null) && (aobjCorrInfo.ihstFormInfo.Count > 0))
            {
                string lstrCurrentObjectName = aarrResult[0].GetType().Name;
                if (lstrCurrentObjectName != aobjCorrInfo.istrObjectID)
                {
                    string lstrPropertyName = (string)aobjCorrInfo.ihstFormInfo[ahstQueryBookMks["sfwCallingForm"]];
                    if (lstrPropertyName == null)
                        throw new Exception(lstrCurrentObjectName + " Calling Form is Not Set !");
                    lobjCorr = (busBase)HelperFunction.GetValue(lstrCurrentObjectName, lstrPropertyName, aarrResult[0], ReturnType.Object);
                    larrCorr = new ArrayList();
                    larrCorr.Add(lobjCorr);
                }
            }
            busBase lobjBus = (busBase)HelperFunction.GetObjectFromResult(larrCorr[0], aobjCorrInfo.istrObjectID);
         
           
            if (lobjBus == null)
            {
                throw new Exception("Unable to find correspondence object " + aobjCorrInfo.istrObjectID);
            }
            //Ticket#107637
            iblnSurvivorNotExist = false;
            if ((aobjCorrInfo.istrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE
                    || aobjCorrInfo.istrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700
                    || aobjCorrInfo.istrTemplateName == busConstant.Disability_Benefit_Election_Packet_L52
                    || aobjCorrInfo.istrTemplateName == busConstant.Disability_Benefit_Election_Packet_L600
                    || aobjCorrInfo.istrTemplateName == busConstant.Disability_Benefit_Election_Packet_L161
                    || aobjCorrInfo.istrTemplateName == busConstant.Disability_Benefit_Election_Packet_L666
                    || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L52
               || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L161 || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L600
               || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L666 || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L700
               || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI 
               || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Application_Packet_MPI
               || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Application_Packet_L161
               || aobjCorrInfo.istrTemplateName == busConstant.Retirement_Application_Packet_L52_600_666_7000) && (lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.calculation_type_value == "ESTI" && (lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.istrSurvivorMPID.IsNullOrEmpty())
            {
                iblnSurvivorNotExist = true;

            }
            //Ticket#132783
            else if (aobjCorrInfo.istrTemplateName == busConstant.IAP_Withdrawal_Packet && (lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.istrSurvivorMPID.IsNullOrEmpty())
            {
                iblnSurvivorNotExist = true;

            }

            #region Set plan id for Retr-0033 and PAYEE-0035

            if (aobjCorrInfo.istrTemplateName == busConstant.LUMP_SUM_DISTRIBUTION_ELECTION_NON_SPOUCE || aobjCorrInfo.istrTemplateName == busConstant.LUMP_SUM_DISTRIBUTION_ELECTION)
            {
                (lobjBus as busBenefitCalculationHeader).istrPlan = (string)ahstQueryBookMks["Plan"];
                if (ahstQueryBookMks["Plan"].ToString().Contains("MPIPP"))
                {
                    if ((lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                    {
                        if ((lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.MPIPP;
                        }
                        else if ((lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_161;
                        }
                        else if ((lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_600;
                        }
                        else if ((lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_666;
                        }
                        else if ((lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_52;
                        }
                        else if ((lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.LOCAL_700;
                        }
                    }
                }
                else if (ahstQueryBookMks["Plan"].ToString().Contains(busConstant.IAP) ||
                    ahstQueryBookMks["Plan"].ToString().Contains("161") || ahstQueryBookMks["Plan"].ToString().Contains("52"))
                {
                    if ((lobjBus as busBenefitCalculationHeader).iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        (lobjBus as busBenefitCalculationHeader).icdoBenefitCalculationHeader.iintPlanId = busConstant.IAP_PLAN_ID;
                    }
                }
            }

            #endregion

            #region set plan id for DRO-0033

            if (aobjCorrInfo.istrTemplateName == busConstant.DRO_LUMP_SUM_DISTRIBUTION_ELECTION)
            {
                (lobjBus as busQdroCalculationHeader).istrPlan = (string)ahstQueryBookMks["Plan"];
                if (ahstQueryBookMks["Plan"].ToString().Contains("MPIPP"))
                {
                    if ((lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                    {
                        if ((lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.MPIPP;
                        }
                        else if ((lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_161;
                        }
                        else if ((lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_600;
                        }
                        else if ((lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_666;
                        }
                        else if ((lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.Local_52;
                        }
                        else if ((lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                        {
                            ahstQueryBookMks["Plan"] = "2:" + busConstant.LOCAL_700;
                        }
                    }
                }
                else if (ahstQueryBookMks["Plan"].ToString().Contains(busConstant.IAP) ||
                    ahstQueryBookMks["Plan"].ToString().Contains("161") || ahstQueryBookMks["Plan"].ToString().Contains("52"))
                {
                    if ((lobjBus as busQdroCalculationHeader).iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        (lobjBus as busQdroCalculationHeader).icdoQdroCalculationHeader.iintPlanId = busConstant.IAP_PLAN_ID;
                    }
                }
            }

            #endregion

            lobjBus.LoadCorresProperties(aobjCorrInfo.istrTemplateName);
            if (aobjCorrInfo.istrTemplateName == busConstant.POA_REQUIREMENTS)
            {
                string lstrCurrentObjectName = aarrResult[0].GetType().Name;
                if (lstrCurrentObjectName == aobjCorrInfo.istrObjectID)
                {
                    if (lobjBus is busPerson)
                    {
                        string lstrPropertyName = (string)ahstQueryBookMks["Contact_Type"];
                        if (lstrPropertyName.IsNotNullOrEmpty())
                        {
                            if (lstrPropertyName.Contains("Conservatorship"))
                                (lobjBus as busPerson).istrIsConservator = "Y";
                            else if (lstrPropertyName.Contains("Guardianship"))
                                (lobjBus as busPerson).istrIsGuardian = "Y";
                            else
                                (lobjBus as busPerson).istrIsPOA = "Y";
                        }
                    }
                }
            }
          
            if (aobjCorrInfo.istrTemplateName == busConstant.DISABILITY_CONVERSION_COVER_LETTER)
            {
                string lstrCurrentObjectName = aarrResult[0].GetType().Name;
                if (lstrCurrentObjectName == aobjCorrInfo.istrObjectID)
                {
                    if (lobjBus is busDisabilityApplication)
                    {
                        string lstrPropertyName = (string)ahstQueryBookMks["CuttOffDate"];
                        if (lstrPropertyName.IsNotNullOrEmpty())
                        {
                            int lstrlen = lstrPropertyName.Length;
                            string lstrDueDate = string.Empty;
                            lstrPropertyName = lstrPropertyName.Substring(0, lstrlen - lstrPropertyName.IndexOf(':') - 1);
                            DateTime ldtDueDate = Convert.ToDateTime(lstrPropertyName);
                            lstrDueDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDueDate);
                            ahstQueryBookMks["CuttOffDate"] = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDueDate) + ":" + busGlobalFunctions.ConvertDateIntoDifFormat(ldtDueDate);

                        }
                        string lstrPropertyName1 = (string)ahstQueryBookMks["NextBenPaytDate"];
                        if (lstrPropertyName1.IsNotNullOrEmpty())
                        {
                            int lstrlen = lstrPropertyName1.Length;
                            string lstrDueDate = string.Empty;
                            lstrPropertyName1 = lstrPropertyName1.Substring(0, lstrlen - lstrPropertyName1.IndexOf(':') - 1);
                            DateTime ldtDueDate = Convert.ToDateTime(lstrPropertyName1);
                            lstrDueDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDueDate);
                            ahstQueryBookMks["NextBenPaytDate"] = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDueDate) + ":" + busGlobalFunctions.ConvertDateIntoDifFormat(ldtDueDate);
                        }
                    }
                }
            }

            if (aobjCorrInfo.istrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_700)
            {
                string lstrCurrentObjName = aarrResult[0].GetType().Name;
                if (lobjBus is busBenefitCalculationRetirement)
                {
                    string lstrPropertyName = (string)ahstQueryBookMks["LevIncRelVal"];
                    if (lstrPropertyName == "")
                        ahstQueryBookMks["LevIncRelVal"] = "N/A:N/A";

                    string lstrPropertNameAmt = (string)ahstQueryBookMks["LevIncAmt"];
                    if (lstrPropertNameAmt == "")
                        ahstQueryBookMks["LevIncAmt"] = "N/A:N/A";
                }
            }
            if (aobjCorrInfo.istrTemplateName == busConstant.NOTICE_OF_APPEARANCE_AND_RESPONSE_OF_EMPLOYEE_BENEFIT_PLAN_JOINDER)
            {
                string lstrCurrentObjName = aarrResult[0].GetType().Name;
                if (lobjBus is busQdroApplication)
                {
                    string lstrPropertyName = (string)ahstQueryBookMks["Petitioner"];
                    if (lstrPropertyName == "Participant:Participant")
                    {
                        ahstQueryBookMks["Petitioner"] = (lobjBus as busQdroApplication).ibusParticipant.icdoPerson.istrFullName.ToUpper() + ":" + (lobjBus as busQdroApplication).ibusParticipant.icdoPerson.istrFullName.ToUpper();
                        (lobjBus as busQdroApplication).istrRespondent = (lobjBus as busQdroApplication).ibusAlternatePayee.icdoPerson.istrFullName;
                    }
                    else if (lstrPropertyName == "Alternate Payee:Alternate Payee" || lstrPropertyName == "")
                    {
                        ahstQueryBookMks["Petitioner"] = (lobjBus as busQdroApplication).ibusAlternatePayee.icdoPerson.istrFullName.ToUpper() + ":" + (lobjBus as busQdroApplication).ibusAlternatePayee.icdoPerson.istrFullName.ToUpper();
                        (lobjBus as busQdroApplication).istrRespondent = (lobjBus as busQdroApplication).ibusParticipant.icdoPerson.istrFullName;
                    }
                    // pir-692
                    string lstrBookmarkValue = (string)(ahstQueryBookMks["Claimant"]);
                    if (!string.IsNullOrEmpty(lstrBookmarkValue))
                    {
                        ahstQueryBookMks["Claimant"] = lstrBookmarkValue.ToUpper();
                    }
                }
            }
         if (aobjCorrInfo.istrTemplateName == busConstant.SUBPOENA_RESPONSE_CERTIFICATION_OF_RECORDS)
            {
                string lstrCurrentObjectName = aarrResult[0].GetType().Name;
                if (lstrCurrentObjectName == aobjCorrInfo.istrObjectID)
                {
                    if (lobjBus is busPersonOverview)
                    {
                        string lstrPropertyName = (string)ahstQueryBookMks["CUR_DATE"];
                        if (lstrPropertyName.IsNotNullOrEmpty())
                        {
                            int lstrlen = lstrPropertyName.Length;

                            string CurrDate = string.Empty;
                            string lstrDay = string.Empty;
                            lstrPropertyName = lstrPropertyName.Substring(0, lstrlen - lstrPropertyName.IndexOf(':') - 1);
                            DateTime ldtCurrDate = Convert.ToDateTime(lstrPropertyName);
                            lstrDay = busGlobalFunctions.AddOrdinal(ldtCurrDate.Day);
                            CurrDate = lstrDay + " day of " + String.Format("{0:MMMM}", ldtCurrDate) + "," + ldtCurrDate.Year;
                            ahstQueryBookMks["CUR_DATE"] = CurrDate + ":" + CurrDate;

                        }
                    }
                }
            }
            //#126479
            if (aobjCorrInfo.istrTemplateName == busConstant.MISSING_DOCUMENT_REQUEST)
            {
                if (utlPassInfo.iobjPassInfo.istrFormName == busConstant.PAYEE_ACCOUNT_MAINTENANCE)
                {
                    busPayeeAccount lbusPayeeAccount = (busPayeeAccount)aarrResult[0];
                    busPerson lbusPerson = ((busPerson)lobjBus);
                    lbusPerson.istrBenefitTypeDescription = lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_description;
                }
                else if (utlPassInfo.iobjPassInfo.istrFormName == "wfmBenefitCalculationPreRetirementDeathMaintenance" ||
                         utlPassInfo.iobjPassInfo.istrFormName == "wfmBenefitCalculationRetirementMaintenance" ||
                         utlPassInfo.iobjPassInfo.istrFormName == "wfmDisabiltyBenefitCalculationMaintenance" ||
                         utlPassInfo.iobjPassInfo.istrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance" ||
                         utlPassInfo.iobjPassInfo.istrFormName == "wfmQDROCalculationMaintenance" ||
                         utlPassInfo.iobjPassInfo.istrFormName == "wfmBenefitCalculationWithdrawalMaintenance")
                {
                    busBenefitCalculationHeader lbusBenefitCalculationHeader = (busBenefitCalculationHeader)aarrResult[0];
                    busPerson lbusPerson = ((busPerson)lobjBus);
                    lbusPerson.istrBenefitTypeDescription = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_type_description;
                }
            }
            //This Block will load the objects that are needed only in correpondence.

            //FM upgrade: 6.0.0.29 changes
            //lobjBus = (busBase)busBase.GetStandardValues(aobjCorrInfo, larrCorr, ahstQueryBookMks);
            lobjBus = GetCorrespondenceObject(aobjCorrInfo, lobjBus, ahstQueryBookMks);
            lobjBus.LoadBookmarkValues(aobjCorrInfo, ahstQueryBookMks);

            //Ticket : 58008 and Ticket:59005
            //Ticket#71960
            if (aobjCorrInfo.istrTemplateName == busConstant.PENSION_INCOME_VERIFICATION || aobjCorrInfo.istrTemplateName == busConstant.IAP_ANNUITY_QUOTE_CONFIRMATION_OF_ANNUITY_PAYMENT_TO_PAYEE || aobjCorrInfo.istrTemplateName == busConstant.RETROACTIVE_BENEFIT_INCREASE_NOTICE || aobjCorrInfo.istrTemplateName == busConstant.IAP_PAYBACK_ANNUAL_BATCH_LETTER || aobjCorrInfo.istrTemplateName ==busConstant.ONETIME_PENSION_PAYMENT_LETTER)
            {
                if (lobjBus is busPayeeAccount)
                {
                    busPayeeAccount lbusPayeeAccount = (busPayeeAccount)lobjBus;
                    //Ticket#14060
                    if(lbusPayeeAccount.icdoPayeeAccount.org_id > 0)
                    {

                        if(aobjCorrInfo.istrTemplateName == busConstant.IAP_ANNUITY_QUOTE_CONFIRMATION_OF_ANNUITY_PAYMENT_TO_PAYEE)
                        {
                            LoadStandardOrganizationBookMarks(aobjCorrInfo, lbusPayeeAccount);
                            return;
                        }
                    }
                    else
                    {
                        abusPerson = (busPerson)lbusPayeeAccount.ibusPayee.GetCorPerson();

                    }
                   
                }
                
            }
            if (aobjCorrInfo.istrTemplateName == busConstant.RE_EMPLOYMENT_GENERAL_INFORMATION)
            {
                if (utlPassInfo.iobjPassInfo.istrFormName == busConstant.PERSON_MAINTENANCE)
                {                  
                    LoadPersonBookMarksFor_COR_PER_0012(abusPerson, aobjCorrInfo);
                }
            }
            if (aobjCorrInfo.istrTemplateName == busConstant.RE_EMPLOYMENT_NOTIFICATION_FORM)
            {
                if (lobjBus is busPayeeAccount)
                {
                    busPayeeAccount lbusPayeeAccount = (busPayeeAccount)lobjBus;
                    if (lbusPayeeAccount.icdoPayeeAccount.person_id > 0)
                    {
                        lbusPayeeAccount.ibusPayee.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                        abusPerson = (busPerson)lbusPayeeAccount.ibusPayee.GetCorPerson();
                        LoadStandardPersonBookMarks(abusPerson, aobjCorrInfo);
                        return;
                    }
                }
            }
            else
            {
                abusPerson = (busPerson)lobjBus.GetCorPerson();
            }
          if (abusPerson != null)
                LoadStandardPersonBookMarks(abusPerson, aobjCorrInfo);
            return;
        }
        private static void LoadPersonBookMarksFor_COR_PER_0012(busPerson aobjPerson, utlCorresPondenceInfo aobjCorrInfo)
        {
            utlBookmarkFieldInfo lobjField;
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdEndOfReemploymentDate";
            lobjField.istrValue = "{END_OF_RE-EMPLOYMENT_DATE}";
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);           
        }

        private static void LoadStandardPersonBookMarks(busPerson aobjPerson, utlCorresPondenceInfo aobjCorrInfo)
        {
            aobjCorrInfo.iintPersonID = aobjPerson.icdoPerson.person_id;

            utlBookmarkFieldInfo lobjField;

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdTitle";
            lobjField.istrValue = aobjPerson.icdoPerson.name_prefix_description ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrParticipantMPID";
            lobjField.istrValue = aobjPerson.icdoPerson.mpi_person_id.ToString();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFirstName";
            lobjField.istrValue = aobjPerson.icdoPerson.first_name ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrMidInitial";
            lobjField.istrValue = aobjPerson.icdoPerson.middle_name ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrLastName";
            lobjField.istrValue = aobjPerson.icdoPerson.last_name ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullName";
            lobjField.istrValue = aobjPerson.icdoPerson.istrFullName ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullNameInProperCase";
            lobjField.istrValue = aobjPerson.icdoPerson.istrFullName ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            if (lobjField.istrValue.Contains(","))
            {
                string lstrSuffix = lobjField.istrValue.Substring(lobjField.istrValue.LastIndexOf(",") + 1);
                if (lstrSuffix.IsNotNullOrEmpty())
                {
                    lobjField.istrValue = lobjField.istrValue.Substring(0, lobjField.istrValue.LastIndexOf(",")) + lstrSuffix.ToUpper();
                }
            }
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrSpouseFullName";
            DataTable ldtbSpouseFullName = Select("cdoRelationship.GetSpouseName", new object[1] { aobjPerson.icdoPerson.person_id });
                      
            
            lobjField.istrValue = string.Empty;
            //Ticket#107637
            if (!iblnSurvivorNotExist)
            {
                if (ldtbSpouseFullName.Rows.Count > 0)
                {
                    lobjField.istrValue = ldtbSpouseFullName.Rows[0][0].ToString();
                }
            }
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrSpouseFullNameInCaps";
            DataTable ldtbSpouseFullNameInCaps = Select("cdoRelationship.GetSpouseName", new object[1] { aobjPerson.icdoPerson.person_id });
            lobjField.istrValue = string.Empty;
            if (ldtbSpouseFullName.Rows.Count > 0)
            {
                //Ticket#107637
                if (!iblnSurvivorNotExist)
                {
                    lobjField.istrValue = ldtbSpouseFullName.Rows[0][0].ToString();
                }
            }
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
                        

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdSpouseDateOfBirth";
            lobjField.istrValue = string.Empty;
            //Ticket#107637
            if (!iblnSurvivorNotExist)
            {
                if (ldtbSpouseFullName.Rows.Count > 0 && ldtbSpouseFullName.Rows[0][1] != null && !string.IsNullOrEmpty(Convert.ToString(ldtbSpouseFullName.Rows[0][1])))
                {
                    DateTime ldtSpouseDOB = Convert.ToDateTime(ldtbSpouseFullName.Rows[0][1].ToString());
                    if (ldtSpouseDOB != null)
                    {
                        lobjField.istrValue = ldtSpouseDOB.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                }
            }
            lobjField.istrValue = lobjField.istrValue.ToProperCase();

            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdSpouseNamePrefix";
            //Ticket#107637
            if (!iblnSurvivorNotExist)
            {
                if (ldtbSpouseFullName.Rows.Count > 0)
                {
                    lobjField.istrValue = ldtbSpouseFullName.Rows[0]["SPOUSENAMEPREFIX"].ToString();
                }
            }
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdSpouseGender";
            if (!iblnSurvivorNotExist)
            {
                if (ldtbSpouseFullName.Rows.Count > 0)
                {
                    lobjField.istrValue = ldtbSpouseFullName.Rows[0]["SpouseGender"].ToString();
                }
            }
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            if (aobjPerson.iclbPersonAddress != null && aobjPerson.ibusPersonAddressForCorr != null)
            {
                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorStreet1";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1 ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrFUllAddress";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.istrfull_address ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorStreet2";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2 ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorCity";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_city ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorState";
                if (!string.IsNullOrEmpty(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value))
                {
                    int lintCountryValue = Convert.ToInt32(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value);

                    if (lintCountryValue == busConstant.USA || lintCountryValue == busConstant.AUSTRALIA || lintCountryValue == busConstant.CANADA ||
                        lintCountryValue == busConstant.MEXICO || lintCountryValue == busConstant.NewZealand)
                    {
                        lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_description ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToUpper();
                    }
                    else
                    {
                        lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.foreign_province ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToUpper();
                    }
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrStatePropCase";
                if (!string.IsNullOrEmpty(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value))
                {
                    int lintCountryValue = Convert.ToInt32(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value);

                    if (lintCountryValue == busConstant.USA || lintCountryValue == busConstant.AUSTRALIA || lintCountryValue == busConstant.CANADA ||
                        lintCountryValue == busConstant.MEXICO || lintCountryValue == busConstant.NewZealand)
                    {
                        lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_description ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToProperCase();
                    }
                    else
                    {
                        lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.foreign_province ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToProperCase();
                    }
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrStateValue";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCounty";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.county ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCountry";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCountryDesc";
                lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorZip";
                if (!string.IsNullOrEmpty(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value))
                {
                    int lintCountryValue = Convert.ToInt32(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value);

                    if (lintCountryValue == busConstant.USA)
                    {
                        lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_code ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToUpper();
                        if ((aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_4_code != null) &&
                            (aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_4_code.Trim() != String.Empty))
                        {
                            lobjField.istrValue += "-" + aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_4_code ?? String.Empty;
                            lobjField.istrValue = lobjField.istrValue.ToUpper();
                        }
                    }
                    else
                    {
                        lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.foreign_postal_code ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToUpper();
                    }
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdDomesticStateInternationalCountry";

                if (Convert.ToInt32(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                {
                    lobjField.istrValue = (aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_city + " " + aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value + "  " +
                                           aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_code) ?? String.Empty;

                    if ((aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_4_code != null) &&
                           (aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_4_code.Trim() != String.Empty))
                    {
                        lobjField.istrValue += "-" + aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_4_code ?? String.Empty;
                    }
                    lobjField.istrValue = lobjField.istrValue.ToUpper();
                }
                else
                {
                    //PIR 1002
                    lobjField.istrValue = aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.istrForeignProvince + " " + aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_city + " " + aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.foreign_postal_code ?? String.Empty;
                    lobjField.istrValue = lobjField.istrValue.ToUpper();
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdIsUSA";

                if (Convert.ToInt32(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                {
                    lobjField.istrValue = "1";
                }
                else
                {
                    lobjField.istrValue = "0";
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
            }

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrSSN";
            lobjField.istrValue = HelperFunction.FormatData(aobjPerson.icdoPerson.istrSSNNonEncrypted, "{0:000-##-####}");
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrLastFourDigitsOfSSN";
            lobjField.istrValue = aobjPerson.icdoPerson.istrLast4DigitsofSSN ?? String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrDateOfBirth";
            if (aobjPerson.icdoPerson.idtDateofBirth != DateTime.MinValue)
            {
                lobjField.istrValue = aobjPerson.icdoPerson.idtDateofBirth.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
            }
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            //HS23 MPID, DOB, Email1, and CurDate is added.
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrMPID";
            lobjField.istrValue = aobjPerson.icdoPerson.mpi_person_id.ToString();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrDOB";
            if (aobjPerson.icdoPerson.idtDateofBirth != DateTime.MinValue)
            {
                lobjField.istrValue = aobjPerson.icdoPerson.idtDateofBirth.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
            }
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrEmail1";
            lobjField.istrValue = aobjPerson.icdoPerson.email_address_1 ?? String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdCurDate";
            DateTime ldtCurrentDate = System.DateTime.Now;
            lobjField.istrValue = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOMAHAOrgName";
            DataTable ldtbOmahaOrgAddress = busOrgAddress.Select("cdoOrgAddress.GetOMahaFullAddress", new object[0] { });
            lobjField.istrValue = string.Empty;
            if (ldtbOmahaOrgAddress.Rows.Count > 0)
            {
                //Ticket#107637
                
                    lobjField.istrValue = ldtbOmahaOrgAddress.Rows[0]["org_name"].ToString();
                
            }
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOMAHAOrgAddress1";
            if (ldtbOmahaOrgAddress.Rows.Count > 0)
            {
               lobjField.istrValue = lobjField.istrValue = ldtbOmahaOrgAddress.Rows[0]["ADDR_LINE_1"].ToString();
            }
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOMAHAOrgAddress2";
            if (ldtbOmahaOrgAddress.Rows.Count > 0)
            {
                lobjField.istrValue = lobjField.istrValue = ldtbOmahaOrgAddress.Rows[0]["ADDR_LINE_2"].ToString();
            }
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdstdOMAHAOrgCityStateZip";


            lobjField.istrValue = (ldtbOmahaOrgAddress.Rows[0]["CITY"].ToString() + " " + ldtbOmahaOrgAddress.Rows[0]["STATE_VALUE"].ToString() + "  " +
                                   //aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_code);
                                   ldtbOmahaOrgAddress.Rows[0]["ZIP_CODE"].ToString());
                lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            string stdMbrWithholdingStateCode = "";
            string stdMbrWithholdingStateName = "";
            string stdMbrStateTaxWitholdingType = "";
            if (Convert.ToInt32(aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
            {
                DataTable dtStateTax = busBase.Select("cdoPayeeAccountTaxWithholding.GetStateTaxWithHoldingRequiredInfo", new object[1] { aobjPerson.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value });
                if (dtStateTax.Rows.Count > 0)
                {
                    stdMbrWithholdingStateCode = dtStateTax.Rows[0]["WITHHOLDING_STATE_CODE"].ToString();
                    stdMbrWithholdingStateName = dtStateTax.Rows[0]["WITHHOLDING_STATE_NAME"].ToString();
                    stdMbrStateTaxWitholdingType = dtStateTax.Rows[0]["STATE_TAX_WITHHOLDING_TYPE"].ToString();
                }
            }
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrWithholdingStateCode";
            lobjField.istrValue = stdMbrWithholdingStateCode;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrWithholdingStateName";
            lobjField.istrValue = stdMbrWithholdingStateName;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrStateTaxWitholdingType";
            lobjField.istrValue = stdMbrStateTaxWitholdingType;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
        }
        //Ticket#14060
        private static void LoadStandardOrganizationBookMarks(utlCorresPondenceInfo aobjCorrInfo, busPayeeAccount lbusPayeeAccount)
        {
            lbusPayeeAccount.ibusOrganization.LoadOrgAddresss();
            utlBookmarkFieldInfo lobjField;
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullName";
            lobjField.istrValue = lbusPayeeAccount.ibusOrganization.icdoOrganization.org_name ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullNameInProperCase";
            lobjField.istrValue = lbusPayeeAccount.ibusOrganization.icdoOrganization.org_name ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToProperCase();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
            if (lbusPayeeAccount.ibusOrganization.iclbOrgAddress != null)
            {
                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorStreet1";
                lobjField.istrValue = lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.addr_line_1).FirstOrDefault() ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorStreet2";
                lobjField.istrValue = lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.addr_line_2).FirstOrDefault() ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorCity";
                lobjField.istrValue = lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.city).FirstOrDefault() ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCountryDesc";
                lobjField.istrValue = lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.country_description).FirstOrDefault() ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdMbrAdrCorState";
                if (!string.IsNullOrEmpty(lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.country_value).FirstOrDefault()))
                {
                    int lintCountryValue = Convert.ToInt32(lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.country_value).FirstOrDefault());

                    if (lintCountryValue == busConstant.USA || lintCountryValue == busConstant.AUSTRALIA || lintCountryValue == busConstant.CANADA ||
                        lintCountryValue == busConstant.MEXICO || lintCountryValue == busConstant.NewZealand)
                    {
                        lobjField.istrValue = lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.state_value).FirstOrDefault() ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToUpper();
                    }
                    else
                    {
                        lobjField.istrValue = lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.foreign_province).FirstOrDefault() ?? String.Empty;
                        lobjField.istrValue = lobjField.istrValue.ToUpper();
                    }
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdDomesticStateInternationalCountry";

                if (Convert.ToInt32(lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.country_value).FirstOrDefault()) == busConstant.USA)
                {
                    lobjField.istrValue = (lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.city).FirstOrDefault()) + " " + lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.state_value).FirstOrDefault() + "  " +
                                           lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.zip_code).FirstOrDefault() ?? String.Empty;

                    if ((lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.zip_4_code).FirstOrDefault() != null) &&
                           (lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.zip_4_code).FirstOrDefault().Trim() != String.Empty))
                    {
                        lobjField.istrValue += "-" + lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.zip_4_code).FirstOrDefault() ?? String.Empty;
                    }
                    lobjField.istrValue = lobjField.istrValue.ToUpper();
                }
                else
                {

                    lobjField.istrValue = lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.foreign_province).FirstOrDefault() + " " + lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.city).FirstOrDefault() + " " + lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.foreign_postal_code).FirstOrDefault() ?? String.Empty;
                    lobjField.istrValue = lobjField.istrValue.ToUpper();
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdIsUSA";

                if (Convert.ToInt32(lbusPayeeAccount.ibusOrganization.iclbOrgAddress.Where(i => i.icdoOrgAddress.org_id == lbusPayeeAccount.icdoPayeeAccount.org_id).Select(y => y.icdoOrgAddress.country_value).FirstOrDefault()) == busConstant.USA)
                {
                    lobjField.istrValue = "1";
                }
                else
                {
                    lobjField.istrValue = "0";
                }
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            }
        }

        private static void LoadLastBookmarks(utlCorresPondenceInfo aobjCorrInfo)
        {
            utlBookmarkFieldInfo lobjField;
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdTrackingNo";
            lobjField.istrDataType = "string";
            lobjField.istrValue = "*" + aobjCorrInfo.iintCorrespondenceTrackingId.ToString() + "*";  //.PadLeft(10, '0'); (Removing the padding since MPI doesn't want zeros)
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
        }

        #endregion

        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            base.AddToResponse(aobjResponseData);
            utlObjectData HiddenControlList = (utlObjectData)aobjResponseData.HeaderData["ControlList"]["HiddenControls"];
            utlObjectData ReadOnlyControlList = (utlObjectData)aobjResponseData.HeaderData["ControlList"]["ReadOnlyControls"];
            utlThreadStatic lutlThreadStatic = (utlThreadStatic)Sagitec.Common.utlThreadStatic.iutlThreadStatic.Value;
            XmlFormObject lFormObject = lutlThreadStatic?.iobjFormXml;

            object lstrActivityType = string.Empty;
            int lintFormResource = Convert.ToInt32(lFormObject.idictAttributes["sfwResource"]);
            int lintFormSecurityLevel = 0;
            srvHelper.HasAccess(lintFormResource, out lintFormSecurityLevel);

            utlDataControl ludcCorrButton = lFormObject?.icolutlDataControl?.FirstOrDefault(c => c.istrMethodName == "btnCorrespondence_Click");
            if (HiddenControlList.IsNotNull() && ludcCorrButton.IsNotNull())
            {
                if (ludcCorrButton.IsNotNull() && (ludcCorrButton.istrVisibleRule == null || ludcCorrButton.istrVisibleRule == ""))
                {
                    DataTable ldtbCorrespondence = Select("entCorTemplates.Templates", new object[] { "%" + lFormObject.istrFileName + ";%" });

                    int lintControlSecurityLevel = 0;
                    int ControlResource = Convert.ToInt32(ludcCorrButton.iintResourceID);
                    if (ControlResource == 0)
                    {
                        ControlResource = lintFormResource;
                    }
                    
                    srvHelper.HasAccess(ControlResource, out lintControlSecurityLevel);


                    

                    if (lintFormSecurityLevel >= lintControlSecurityLevel && lintFormSecurityLevel > 1 && lintControlSecurityLevel > 1)
                    {
                        if (HiddenControlList.ContainsKey(ludcCorrButton.istrControlID))
                            HiddenControlList.Remove(ludcCorrButton.istrControlID);
                    }
                    else
                    {
                        if (!HiddenControlList.ContainsKey(ludcCorrButton.istrControlID))
                           HiddenControlList.Add(ludcCorrButton.istrControlID, null);
                    }

                    if (lintFormSecurityLevel > 1 && lintControlSecurityLevel > 1)
                    {
                        if (HiddenControlList.ContainsKey(ludcCorrButton.istrControlID))
                            HiddenControlList.Remove(ludcCorrButton.istrControlID);
                    }

                    if (lintControlSecurityLevel == 0)
                    {
                        if (!HiddenControlList.ContainsKey(ludcCorrButton.istrControlID))
                            HiddenControlList.Add(ludcCorrButton.istrControlID,"ControlSecurityLevel");
                    }

                    if (ldtbCorrespondence.Rows.Count == 0)
                    {
                        if (!HiddenControlList.ContainsKey(ludcCorrButton.istrControlID))
                            HiddenControlList.Add(ludcCorrButton.istrControlID, null);
                    }
                }
            }
            
            if (lintFormSecurityLevel == 1)
            {
                List<utlDataControl> lutlNewOpenDataControls = lFormObject.icolutlDataControl.Where(c => (c.iintResourceID == lintFormResource || c.iintResourceID == 0) &&
                                                                (c.istrMethodName == "btnNew_Click" || c.istrMethodName == "btnOpenDetail_Click")).ToList();
                List<utlDataControl> lutlGridDataControls = lFormObject.icolutlDataControl.Where(c => 
                                                                (c.istrMethodName == "btnGridViewAdd_Click" || c.istrMethodName == "btnGridViewUpdate_Click" || c.istrMethodName == "btnGridViewDelete_Click")).ToList();


                foreach (utlDataControl lutlDataControl in lutlNewOpenDataControls)
                {
                    if (!HiddenControlList.ContainsKey(lutlDataControl.istrControlID))
                    {
                        HiddenControlList.Add(lutlDataControl.istrControlID, null);
                        
                    }
                }
                List<utlDataControl> lutlTextBoxDrodownDataControls = lFormObject.icolutlDataControl.Where(c => (c.ienmControlType == utlControlType.sfwTextBox || c.ienmControlType == utlControlType.sfwDropDownList || c.ienmControlType == utlControlType.sfwCascadingDropDownList)).ToList();

                foreach (utlDataControl lutlDataControl in lutlTextBoxDrodownDataControls)
                {
                    if(lutlGridDataControls.Count() > 0 || iobjPassInfo.istrFormName == "wfmTaxWithholdingCalculatorMaintenance")
                    {
                        if (HiddenControlList.ContainsKey(lutlDataControl.istrControlID))
                        {
                            HiddenControlList.Remove(lutlDataControl.istrControlID);
                            ilstNonEditableControls.Remove(lutlDataControl.istrControlID);
                        }
                        if (ReadOnlyControlList.ContainsKey(lutlDataControl.istrControlID))
                        {
                            ReadOnlyControlList.Remove(lutlDataControl.istrControlID);
                            ilstNonEditableControls.Remove(lutlDataControl.istrControlID);
                        }
                    }
                    
                }

                if (iobjPassInfo.istrFormName == "wfmWorkflowMaintenance")
                {
                    if (ReadOnlyControlList.ContainsKey("grvProcessInstanceChecklist"))
                    {
                        ReadOnlyControlList.Remove("grvProcessInstanceChecklist");
                        ilstNonEditableControls.Remove("grvProcessInstanceChecklist");
                    }
                }
            }
            
            if (aobjResponseData.ConcurrentOtherData.TryGetValue("ActivityInstanceType", out lstrActivityType))
            {
                if ("BPM".Equals(Convert.ToString(lstrActivityType)))
                {
                    aobjResponseData.ConcurrentOtherData["ShowActivityInstanceDetails"] = true;
                    busSolBpmActivityInstance lbusBaseActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                    if (lbusBaseActivityInstance != null)
                    {
                        aobjResponseData.ConcurrentOtherData["ProcessName"] = lbusBaseActivityInstance?.ibusBpmProcessInstance?.ibusBpmProcess?.icdoBpmProcess?.description;
                        aobjResponseData.ConcurrentOtherData["ActivityName"] = lbusBaseActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name;
                        aobjResponseData.ConcurrentOtherData["ProcessInstanceId"] = lbusBaseActivityInstance?.ibusBpmProcessInstance?.icdoBpmProcessInstance?.process_instance_id;
                        aobjResponseData.ConcurrentOtherData["ActivityDetailsNavParams"] = HelperFunction.EncryptString($"aintactivityinstanceid=#{lbusBaseActivityInstance.icdoBpmActivityInstance.activity_instance_id}", utlConstants.istrMenuNavParamKey, utlConstants.istrFormNameNavParamKey);
                    }
                }             
            }
            //Fw upgrade: PIR ID : 34590: LOB - User Specific - My Task - Activity hyperlink is not working
            if (iobjPassInfo.istrFormName == "wfmWorkflowCenterLeftMaintenance")
            {
                if (ReadOnlyControlList.ContainsKey("btnCLOpen1"))
                {
                    ReadOnlyControlList.Remove("btnCLOpen1");
                }
            }
        }
    }
}

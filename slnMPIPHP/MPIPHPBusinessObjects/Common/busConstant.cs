using DocumentFormat.OpenXml.Spreadsheet;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Text;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public static class busConstant
    {
         
        #region Common

        public const int ZERO_INT = 0;
        public const string ZERO_STRING = "0";
        public const decimal ZERO_DECIMAL = 0.0m;
        public const decimal INFINITY_DECIMAL = 9999.9999M;
        public const string FLAG_YES = "Y";
        public const string FLAG_NO = "N";
        public const string MPIPHP_BATCH_USER = "MPIPHP Batch";
        public const string MPIPHPServiceUser = "MPIPHP Service";
        public const string BUSINESS_TIER_URL = "BusinessTierUrl";
        public const string LEGACY_DATABASE = "Legacy";
        public const string STATUS_REVIEW = "REVW";
        public const string STATUS_VALID = "VALD";
        public const string STATUS_ACTIVE = "A";
        public const string STATUS_INACTIVE = "I";
        public const string STATUS_INAC = "INAC";
        public const string YES = "Yes";
        public const string NO = "No";
        public const bool BOOL_TRUE = true;
        public const bool BOOL_FALSE = false;
        public const int ERROR_STATUS_ID = 77;
        public const string YES_CAPS = "YES";
        public const string NO_CAPS = "NO";

        public const int SUSPENDIBLE_MONTH_ID = 801;
        public const int SUSPENDIBLE_MONTH_STATUS_ID = 7042;
        public const string SUSPENDIBLE_MONTH_STATUS_PENDING = "PEND";
        public const string SUSPENDIBLE_MONTH_STATUS_REVIEW = "REVW";
        public const string SUSPENDIBLE_MONTH_STATUS_APPROVED = "APVD";
        public const string SUSPENDIBLE_MONTH_STATUS_SUSPENDED = "SPND";
        public const string SUSPENDIBLE_MONTH_STATUS_PROCESSED = "PRCD";
        public const string SUSPENDIBLE_MONTH_STATUS_PENDING_DESC = "Pending";

        public const string J100 = "J100";
        public const string JA66 = "JA66";
        public const string JP50 = "JP50";
        public const string JPOP = "JPOP";
        public const string JS66 = "JS66";
        public const string JS75 = "JS75";
        public const string JSAA = "JSAA";
        public const string QJSA = "QJSA";
        public const string QJ50 = "QJ50";

        public const string PENSION = "PENSION";
        public const string IAP = "IAP";
        public const string MPIPP = "MPIPP";
        public const string Local_600 = "Local600";
        public const string Local_666 = "Local666";
        public const string Local_52 = "Local52";
        public const string Local_161 = "Local161";                
        public const string LOCAL_700 = "Local700";
        public const string LIFE = "LIFE";
        public const string UVHP = "UVHP";
        public const string EE = "EE";
        public const string EE_UVHP = "EE & UVHP";
        public const string L52_SPL_ACC = "L52_SPL_ACC";
        public const string L161_SPL_ACC = "L161_SPL_ACC";
        public const string LIFE_PLAN = "Health Plan";
        public const string LOCAL_52_SPECIAL_ACCOUNT = "Local-52 Special Account";
        public const string LOCAL1_161_SPECIAL_ACCOUNT = "Local-161 Special Account";
        public const string MPIPPUVHP = "MPIPP (UVHP) ";
        public const string MPIPPEE = "MPIPP (EE) ";
        public const string IAPLOCAL52 = "IAP (Local 52 Special Account)";
        public const string IAPLOCAL161 = "IAP (Local-161 Special Account)";



        public const string MARITAL_STATUS_MARRIED = "M";
        public const string MARITAL_STATUS_DIVORCED = "D";
        public const string MARITAL_STATUS_SINGLE = "S";
        public const string CodeValueAll = "All";
        public const string Flag_Yes = "Y";
        public const string PLAN_NAME = "plan_name";
        public const string PLAN_CODE = "plan_code";
        public const string LOCAL_700_PENSION_PLAN = "Local 700 Pension Plan";
        public const int LOCAL_700_PLAN_ID = 6;
        public const int IAP_PLAN_ID = 1;
        public const int MPIPP_PLAN_ID = 2;
        public const int LOCAL_52_PLAN_ID = 7;
        public const int LOCAL_600_PLAN_ID = 3;
        public const int LOCAL_666_PLAN_ID = 4;
        public const int LIFE_PLAN_ID = 9;
        public const int LOCAL_161_PLAN_ID = 8;
        public const string CANCELLATION_REASON_DECEASED = "DCSD";
        public const string CANCELLATION_REASON_OTHER = "OTHR";
        public const int CANCELLATION_REASON_ID = 6036;
        public const string IAP_PLAN = "Individual Account Plan";
        public const string PENSION_PLAN = "Motion Picture Industry Pension Plan";
        public const string PERSON_ACCOUNT_STATUS_DECEASED = "DCSD";
        public const string PERSON_ACCOUNT_STATUS_ACTIVE = "ACTV";
        public const string PERSON_ACCOUNT_STATUS_INACTIVE = "INAC";
        public const string PERSON_ACCOUNT_STATUS_RETIRED = "RETR";
        public const string BUS_DRO_BENIFIT_DETAILS = "busDroBenefitDetails";
        public const int PERSON_ACCOUNT_STATUS_ID = 6035;

        public const string HEALTH_ELIGIBLE_FLAG_YES = "Y";
        public const string HEALTH_ELIGIBLE_FLAG_NO = "N";

        public const int BENEFICIARY_TYPE_ID = 6001;
        public const string BENEFICIARY_TYPE_PRIMARY = "PRIM";
        public const string BENEFICIARY_TYPE_CONTINGENT = "CONT";


        public const string PAYMENT_REIMBURSEMENT_STATUS_PENDING = "PEND";

        public const string RULE_3 = "R3";

        public const int YEAR_END_PROCESS_NAME_ID = 7055;

        public const string YEAR_END_PROCESS_REQUEST_LOOKUP = "wfmYearEndProcessRequestLookup";
        public const string YEAR_END_PROCESS_REQUEST_PENDING = "PEND";
        public const string YEAR_END_PROCESS_REQUEST_COMPLETED = "CMPL";
        public const string YEAR_END_PROCESS_REQUEST_FAILED = "FALD";
        public const string ANNUAL_1099R = "A99R";
        public const string ANNUAL_STATEMENT_AND_PENSION_ACTUARY_DATA = "ASPA";
        public const string CORRECTED_1099R = "C99R";
        public const string RETIREE_HEALTH_ELIGIBILITY = "REHE";
        public const int OCTOBER = 10;
        public const int FIFTEEN = 15;

        public const string UPDATESEQ = "UPDATE_SEQ";
        public const string MODIFIEDDATE = "MODIFIED_DATE";

        public const string DELIMITERTAB = "\t";

        public const string Death_Report_Start_Date = "06/30/2018";

        //EmergencyOneTimePayment - 03/17/2020
        public const int COVID_OPTION_CODE_ID = 7096;
        public const string COVID_IAP_ONLY_OPTION = "OPT1";
        public const string COVID_L52_SPL_AC_ONLY_OPTION = "OPT2";
        public const string COVID_L161_SPL_AC_ONLY_OPTION = "OPT3";
        public const string COVID_L52_L161_SPL_AC_ONLY_OPTION = "OPT4";
        public const string COVID_ALL_OPTION = "OPT5";

        #endregion

        #region State Code Values
        public const string CALIFORNIA = "CA";
        public const string GEORGIA = "GA";
        public const string OREGON = "OR";
        public const string NORTH_CAROLINA = "NC";
        public const string VERGINIA = "VA";

        #endregion

        #region DB Operators

        public const string DBOperatorEquals = "=";
        public const string DBOperatorNotEquals = "!=";
        public const string DBOperatorGreaterThanEquals = ">=";
        public const string DBOperatorLessThanEquals = "<=";
        public const string DBOperatorLike = "like";

        #endregion

        public abstract class Role
        {
            public const int CODE_ID_BUSINESS_USER_ROLES = 76;
            public const int MANAGER_ROLE = 97;
            //Ticket#76557 
            public const int STAFF_2 = 98;

            public const string SUPER_ADMIN_ROLE = "SCOUT - Admin";
            public const string RESTRICTED_ROLE = "SCOUT - Read";
            public const int LOGGED_PIR = 303;
            //Ticket : 55015
            public const int VIP_MANAGER_ROLE = 502;

        }

        public abstract class Organization
        {
            public const string OrgPaymentTypeBank = "BANK";
            public const string OrgPaymentTypeEstate = "EST";
            public const string OrgPaymentTypeMPITrustFund = "MTFA";
            public const string OrgPaymentTypeRolloverOrg = "RLIT";
            public const string OrgPaymentTypeTrust = "TRST";
            public const string OrgPaymentTypeVendor = "VEND";

        }

        public abstract class File
        {
            public const int ORG_RATE_FILE = 48;
            public const int BANK_MASTER = 49;
            public const int BENEFIT_ENROLL_FILE = 60;
            public const int SMALL_WORLD_FILE_ID = 1001;

            //For Check Inbound File
            public const int CHECK_RECONCILIATION_SERVICE_FILE_ID = 1002;
            public const int ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE = 1006;

            public const int RECORD_CODE_TWO = 2;
            public const string RECORD_CODE_ONE = "01";
            public const string PAID_CHECK_CODE = "01";
            public const string PAID_CHECK = "1"; // "11"; NEW US BANK CODE
            public const string Outstanding_Issue = "2"; //  "10"; NEW US BANK CODE
            public const string Stale_Dated_Check = "5"; // "14"; Cancel is considered stale in US Bank
            public const string RECONCILED_CHECK = "3"; // US BANK ALSO USE RECONCILED AS PAID NO ISSUE STATUS

            public const string RETURN_CODE_R14 = "R14";
            public const string RETURN_CODE_R15 = "R15";
        }

        public abstract class BatchPersonEnroll
        {
            public const string FILE_STATUS_PROCESSED_WITH_WARNINGS = "PRCW";
        }

        public abstract class BatchNotification
        {
            public const string STATUS_ACTIVE = "ACTV";
            public const string STATUS_INACTIVE = "INAC";
        }


        public abstract class MPIPHPBatch
        {
            public const string FORM_FIELD_FREQUENCY_TYPE_DROP_DOWN_VALUE = "freq_type_value";
            public const string FORM_JOB_SCHEDULE_LOOKUP = "wfmJobScheduleLookup";
            public const int ERROR_ID_SELECT_FREQUENCY_TYPE = 5000;
            public const string ERROR_MESSAGE_SELECT_FREQUENCY_TYPE = "Please select a frequency type to create a new batch schedule";

            public const string BATCH_USER_ID_FORMAT = "[BATCH_{0}_{1}]";
            public const string MESSAGE_FORMAT_UNCAUGHT_EXCEPTION = "Uncaught exception in Schedule Instance: {0}. {1}";
            public const string MESSAGE_TYPE_SUMMARY = "SUMM";
            public const string MESSAGE_TYPE_ERROR = "ERRO";
            public const string MESSAGE_TYPE_INFORMATION = "INFO";

            public const string ERROR_TYPE_ERROR = "E";
            public const string ERROR_TYPE_INFORMATION = "I";
            public const string ERROR_TYPE_WARNING = "W";

            public const string JOB_STATUS_ON_SCHEDULE = "ONSE";
            public const string JOB_STATUS_ON_DEMAND = "ONDE";
            public const string JOB_STATUS_AD_HOC = "ADHC";

            public const int STEP_RECEIVE_RATE_FILE = 38;
            public const int STEP_UPLOAD_RATE_FILE = 37;
            public const int STEP_POST_RATE_FILE_DATA = 39;
            public const int STEP_RECEIVE_BANK_MASTER_FILE = 41;
            public const int STEP_UPLOAD_BANK_MASTER_FILE = 40;
            public const int STEP_POST_BANK_MASTER_DATA = 42;
            public const int NOTIFICATION_BATCH = 43;
            public const int BENEFIT_APPLICATION_BATCH = 2;
            public const int CANCEL_WITHDRAWAL_APPL = 3;
            public const int YEAR_END_SNAPSHOT = 12;
            public const int DEATH_REPORT_BATCH = 4;
            public const int PRENOTIFICAITON_BREAK_IN_SERVICE_BATCH = 5;
            public const int BREAK_IN_SERVICE_NOTIFICATION_BATCH = 6;
            public const int MINIMUM_DISTRIBUTION_BATCH = 10;
            public const int RE_EVALUATION_OF_MINIMUM_DISTRIBUTION_BATCH =35 ;
            public const int BENEFIT_ADJUSTMENT_BATCH = 36;
            public const int ACTIVE_RETIREE_INCREASE_BATCH = 42;
            public const int RETIREE_INCREASE_ROLLOVER_BATCH = 75;
            public const int ANNUAL_INTEREST_POSTING_BATCH = 11;
            public const int ACTIVE_PART_OUTBOUND_FILE = 13;
            public const int LATE_IAP_ALLOCATION_BATCH = 14;
            public const int YEAREND_IAP_ALLOCATION_BATCH = 15;
            public const int YEAREND_IAP_ALLOCATION_POSTING_BATCH = 16;
            public const int CONVERSION_INTEREST_POSTING_BATCH = 99;
            public const int BatchPreNoteACH_Bound = 30;
            public const int BatchUpdateTaxAmount = 33;
            public const int BatchMonthlyPayment = 34;
            public const int BatchAdhocPayment = 40;
            public const int BatchWeaklyIAPPayment = 38;
            public const int SSA_DISABILITY_RE_CERTIFICATION_BATCH = 20;
            public const int RETIREMENT_AFFIDAVIT_BATCH = 37;
            public const int VERIFICATION_OF_HOURS_BATCH = 39;
            public const int IAP_PAYMENT_ADJUSTMENT_BATCH = 44;
            public const int REEMPLOYED_BATCH = 45;
            public const int PAYEE_ERROR_BATCH = 47;
            public const int VENDOR_PAYMENT_BATCH = 48;
            public const int REEVALUATION_OF_REEMPLOYED_BATCH = 49;
            public const int INTEGERATION_REEMPLOYMENT_BATCH = 54;
            public const int RESUME_BENEFITS_BATCH = 55;
            public const int ACH_STATUS_UPDATE_BATCH = 56;
            public const int ANNUAL_STATEMENT_DATA_EXTRACTION_BATCH = 57;
            public const int GENERATE_ANNUAL_STATEMENT_BATCH = 61;
            public const int HEALTH_ELIGIBILITY_ACTUARY_BATCH = 58;
            public const int HEALTH_ELIGIBILITY_ACTUARY_OUTBOUND_FILE = 59;
            public const int GENERATE_PENSION_ACTUARY_FILE = 60;
            public const int EDD_OUTBOUND_FILE = 62;
            public const int RECLAMATION_OUTBOUND_FILE = 63;
            public const int RETIREE_INCREASE_CORRESPONDENCE_BATCH = 70;
            public const int RECALCULATE_RETIREMENT_BENEFIT_BATCH = 76;
            public const int EGWP_PARTICIPANTS_BATCH = 78; // ChangeID: 59596
            public const int REPORT_5500_BATCH = 80; //ID_59363
            public const int PENSION_VERIFICATION_HISTORY_BATCH = 84;
            public const int IAP_RECALCULATE_FILE_CLEANUP_BATCH = 85;
            public const int IAP_REQUIRED_MINIMUM_DISTRIBUTION_BATCH = 86;
            public const int IAP_HARDSHIP_PAYBACK_BATCH = 89;
            public const int APPROVE_10_PERCENT_INCREASE_PAYEE_ACCOUNT = 90;
            public const int RETIREMENT_WORKSHOP = 91;
            public const int LAST_ONE_YEAR_DEATH_NOTIFICATION_REPORT = 92;
            public const int PENSION_VERIFICATION_SUSPEND_BATCH = 93; //WI 19555 - PBV Phase-1 Suspension Batch
            public const int STATE_TAX_UPDATE_BATCH = 94;
            public const int ONE_TIME_PENSION_PAYMENT_CORRESPONDENCE_BATCH = 96;


            public const int ONETIME_PAYMENT_BATCH = 93;   //JOB SCHEDULE ID
            public const int ONETIME_PAYMENT_BATCH_STEP_NO = 95;   //JOB SCHEDULE STEP NO

            //PIR: 1040
            public const int UPDATE_CHECK_STATUS_BATCH = 82;

            public const int RETIRE_HEALTH_ELIGIBILITY_REPORT_BATCH = 83;
            public const int THIRTY_DAY_RETIRE_HEALTH_ELIGIBILITY_REPORT_BATCH = 87;
            //Ticket#71531
            public const int EE_UVHP_STATEMENT_BATCH = 88;

            public const int ACTIVE_RETIREE_INCREASE_BATCH_SCHEDULE_ID = 43;
            public const int RETIREE_INCREASE_CORRESPONDENCE_BATCH_SCHEDULE_ID = 66;

            public const int CHECK_RECONCILIATION_SERVICE_OUTBOUND_FILE_BATCH = 72;
            public const int NEW_PARTICIPANT_BATCH = 74;


            public const int BatchAnnual1099r = 64;
            public const int BatchGenrate1099r = 68;
            public const int BatchCorrected1099r = 69;
            public const int BatchHealthAddrUpdateBatch = 71;
            // For Inbound File
            public const int POST_SMALL_WORLD_INBOUND_FILE = 17;
            public const int RECEIVE_SMALL_WORLD_FILE = 18;
            public const int UPLOAD_SMALL_WORLD_FILE = 19;

            public const int POST_CHECK_RECONCILIATION_SERVICE = 50;
            public const int RECEIVE_CHECK_RECONCILIATION_SERVICE = 51;
            public const int UPLOAD_CHECK_RECONCILIATION_SERVICE = 52;

            public const int RETIREE_HEALTH_ELIGIBILITY_BATCH = 7;
            public const int PENSION_ELIGIBILITY_BATCH = 8;
            public const int ENCRYPT_SSN_BATCH = 9;

            public const int POST_ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE = 67;
            public const int RECEIVE_ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE = 65;
            public const int UPLOAD_ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE = 66;

            public const int COMPLIANCE_REVIEW_POLITICAL_SUBDIVISION_EMPL = 44;
            public const int COMPLIANCE_REVIEW_TPA_BP = 45;
            public const int COMPLIANCE_REVIEW_SCHOOL_DIVISION_EMPL = 46;
            public const int COMPLIANCE_REVIEW_STATE_EMPL = 47;

            public const int PERSON_BATCH_ENROLL_UPLOAD = 48;
            public const int PERSON_BATCH_ENROLL_RECEIVE = 49;
            public const int PERSON_BATCH_ENROLL_POST = 50;
            public const int STEP_PERFORM_PERSON_ENROLLMENT = 51;

            public const int PERSON_BATCH_CORRECTION_RECEIVE = 52;
            public const int PERSON_BATCH_CORRECTION_UPLOAD = 53;
            public const int PERSON_BATCH_CORRECTION_POST = 54;
            public const int PERSON_BATCH_CORRECTION_PERFORM = 55;

            public const int ERROR_DUPLICATE_BATCH_RECORD = 91011;
            public const int ERROR_DUPLICATE_ENROLLMENT = 7139;

            public const int STEP_BATCH_RECEIVE_DC_PLAN_FILE = 56;
            public const int STEP_BATCH_UPLOAD_DC_PLAN_FILE = 57;
            public const int STEP_BATCH_POST_DC_PLAN_FILE = 58;

            public const int STEP_BATCH_RECEIVE_LTCG_PLAN_FILE = 68;
            public const int STEP_BATCH_UPLOAD_LTCG_PLAN_FILE = 69;
            public const int STEP_BATCH_POST_LTCG_PLAN_FILE = 70;

            public const int STEP_BATCH_RECEIVE_MNLI_PLAN_FILE = 76;
            public const int STEP_BATCH_UPLOAD_MNLI_PLAN_FILE = 77;
            public const int STEP_BATCH_POST_MNLI_PLAN_FILE = 78;

            public const int STEP_BATCH_RECEIVE_GENWORTH_PLAN_FILE = 79;
            public const int STEP_BATCH_UPLOAD_GENWORTH_PLAN_FILE = 80;
            public const int STEP_BATCH_POST_GENWORTH_PLAN_FILE = 81;

            public const int STEP_BATCH_RECEIVE_UNUM_PLAN_FILE = 82;
            public const int STEP_BATCH_UPLOAD_UNUM_PLAN_FILE = 83;
            public const int STEP_BATCH_POST_UNUM_PLAN_FILE = 84;
            public const int STEP_BATCH_MASS_COMMUNICATION = 2600;

            public const int STEP_BATCH_RECEIVE_DHRM_LOG_FILE = 9801;
            public const int STEP_BATCH_UPLOAD_DHRM_LOG_FILE = 9802;
            public const int STEP_BATCH_POST_DHRM_LOG_FILE = 9803;

            public const int STEP_BATCH_RECEIVE_EMPLOYER_RPT_FILE = 9805;
            public const int STEP_BATCH_UPLOAD_EMPLOYER_RPT_FILE = 9806;
            public const int STEP_BATCH_POST_EMPLOYER_RPT_FILE = 9807;
            public const string BATCH_NOTIFICATION = "Batch Notification";

            public const int EMDEON_PAYEE_NAME_LENGTH = 39; //PIR 1077

            //Workflow related batches
            public const int WORKFLOW_ASSIGN_OR_UNASSIGN_BACKUP_USER = 3000;

            public const int STEP_LONG_STEP_TO_TEST_CANCEL_JOB_OPERATION = 9998;

            public const int STEP_PROCESS_FORM_REQUEST = 85;

            public const int PERFORM_MASS_CHANGE_UPDATE = 6000;

            public const int SEND_FORM_160A = 150;

            public const int STEP_PERFORM_ADDRESS_CORRECTION = 86;
            public const string STEP_NAME_PERFORM_ADDRESS_CORRECTION = "PERFORM_ADDRESS_CORRECTION";
            public const string STEP_NAME_PERFORM_PERSON_ADDRESS_CORRECTION = "PERFORM_PERSON_ADDRESS_CORRECTION";
            public const string STEP_NAME_PERFORM_ORG_ADDRESS_CORRECTION = "PERFORM_ORG_ADDRESS_CORRECTION";
            public const string STEP_NAME_PERFORM_PERSON_REQUEST_ADDRESS_CORRECTION = "PERFORM_PERSON_REQUEST_ADDRESS_CORRECTION";

            public const int AUDIT_LOG_PURGING_BATCH = 7000;

            public const string REPORT_PATH_DEFINITION = "BatchRptDF";
            public const string REPORT_PATH_GENERATED = "BatchRptGN";
            public const string REPORT_PATH_PREBIS = "PREBIS";  //PIR 978

            public const string GENERATED_SSADISABILITY_REPORT_PATH = "RPTSSA";

            public const string GENERATED_PENSION_ELIGIBILITY_REPORT_PATH = "RPTPEN";
            public const string GENERATED_APPROVE_10_PERCENT_INCREASE_REPORT_PATH = "RPT10PRCNT";
            public const string GENERATE_STATE_TAX_UPDATE_BATCH_PATH = "RPTSTUPB"; 
            //PIR-868
            public const string GENERATED_MINIMUMDISTRIBUTION_REPORT_PATH = "RPTMD";

            public const string GENERATED_RITIREE_HEALTH_ELIGIBILITY_REPORT_PATH = "RPTRHER";//LA Sunset - RITIREE HEALTH ELIGIBILITY
            public const string GENERATED_RITIREE_HEALTH_ELIGIBILITY_REPORT_PATH_30_DAY = "RPTRHER30"; 
            public const string GENERATED_BIS_REPORT_PATH = "RPTBIS";
            //RequestID: 63892
            public const string GENERATED_PENSION_BENEFIT_VERIFICATION_REPORT_PATH = "RPTPBV";

            public const string GENERATED_RECALCULATE_IAP_ALLOCATION = "RPTPRECL";

            public const string GENERATED_IAPREQUIREDMINIMUMDISTRIBUTION_REPORT_PATH = "RPTIAPRMD";

            public const string GENERATED_UVHP_EE_REFUND_REPORT_PATH = "RPTPUVHPEE";

           public const string INPUT_MPI_RETIREMENT_WORKSHOP = "RPTPRTWKS";



            public const string GENERATED_TRIAL_REPORT_PATH = "RPTTRI";
            public const string GENERATED_FINAL_REPORT_PATH = "RPTFIN";
            public const string GENERATED_ANNUAL_REPORT_PATH = "RPTANN";
            public const string GENERATED_ANNUAL_REPORT_ESTATEMENT_PATH = "RPTANE";
            public const string GENERATED_ANNUAL_REPORT_FOREIGN_ADDRESS_PATH = "RPTANF";
            public const string GENERATED_ANNUAL_REPORT_DOMESTIC_ADDRESS_PATH = "RPTAND";
            public const string GENERATED_ANNUAL_REPORT_BAD_ADDRESS_PATH = "RPTANB";
            public const string GENERATED_PAYMENT_DIRECTIVE_PATH = "RPTPDS"; //LA Sunset - Payment Directives
            public const string GENERATED_REPORT_REEVALUATION_OF_MD_PATH = "RPTREMD";

            //Annual Statement Report Changes PIR 960
            public const string GENERATED_ANNUAL_REPORT_FOREIGN_CORRECTED_ADDRESS_PATH = "RPTACF";
            public const string GENERATED_ANNUAL_REPORT_DOMESTIC_CORRECTED_ADDRESS_PATH = "RPTACD";
            public const string GENERATED_ANNUAL_REPORT_BAD_CORRECTED_ADDRESS_PATH = "RPTACB";


            //ROhan RE-MD PIR 815
            public const string GENERATED_REPORT_REEVALUATION_OF_MD_IAP = "RTRMDI";
            public const string GENERATED_REPORT_REEVALUATION_OF_MD_Pension = "RTRMDP";
            public const string REPORT_REEVALUATION_OF_MD = "rptReEvaluationofMDBatchReport";

            public const string ANNUAL_EXTRACTION_PATH_GENERATED = "DataExtrFl";

            public const string REPORT_ACTIVE_DEATH_OUTBOUND = "rptDeathReport";
            public const string REPORT_ACTIVE_DEATH_OUTBOUNDWithSSN = "rptDeathReportwithSSN";

            public const string IAPAnnualAllocationSummaryReport = "rptIAPAnnualAllocationSummary";
            public const string IAPAnnualDetailVsSummaryReport = "rptIAPAnnualDetailVsSummary";
            public const string IAPAnnualFinancialReport = "rptIAPAnnualFinancial";
            public const string IAPAnnualOverlimitReport = "rptIAPAnnualOverlimit";
            public const string IAPAnnualPresentationReport = "rptIAPAnnualPresentation";

            public const bool NOTAPPLICABLE = true;

            public const int SMALL_WORLD_OUTBOUND_PAYEE_FILE = 73;
            public const string HEALTH_OUTBOUND_FILE_PATH = "HAOOUT";

            public const int ANNUAL_BENEFIT_SUMMARY_CORRESPONDENCE_BATCH = 77;//PIR 1003

            public const int PARTICIPANT_SUMMARY_BATCH = 81;
            
            //VIP status history
            public const int VIP_STATUS_HISTORY_BATCH = 79 ;           

        }


        #region User&SecurityConstants
        public abstract class Security
        {
            public const string ACCESS_TYPE_VALUE = "CAH";
            public const string MONDAY = "Mon";
            public const string TUESDAY = "Tue";
            public const string WEDNESDAY = "Wed";
            public const string THRUSDAY = "Thur";
            public const string FRIDAY = "Fri";
            public const string SATURDAY = "Sat";
            public const string SUNDAY = "Sun";

        }

        #endregion User&SecurityConstants

        #region QRDO
        public const string BENEFIT_INFORMATION_GRID = "grvDroBenefitDetails";
        public const string QRDO_LOOKUP = "wfmQDROApplicationLookup";
        public const string QRDO_MAINTAINENCE = "wfmQDROApplicationMaintenance";
        public const string FIELD_DESCRIPTION = "description";
        public const string DRO_CALCULATION_LOOKUP = "wfmDROCalculationLookup";
        public const string DRO_CALCULATION_MAINTENANCE = "wfmQDROCalculationMaintenance";
        public const int DEFAULT_QDRO_PERCENT = 50;
        public const int DRO_MODEL_ID = 6004;
        public const string DRO_MODEL_VALUE_CHILD_SUPPORT = "CSUP";
        public const string DRO_MODEL_VALUE_SPOUSAL_SUPPORT = "SSUP";
        public const string DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA = "STRF";
        #endregion 

        #region Death Notification

        public const string NOTIFICATION_STATUS_IN_PROGRESS = "PROG";
        public const string NOTIFICATION_STATUS_CERTIFIED = "CRTF";
        public const string POTENTIAL_DEATH_CERTIFICATION = "PCRTF";
        public const string NOTIFICATION_STATUS_INCORRECTLY_REPORTED = "INRP";
        public const string NOTIFICATION_STATUS_NOT_DECEASED = "NDCS";
        public const string DEATH_NOTIFICATION_LOOKUP = "wfmDeathNotificationLookup";

        public const string NEW_REPORT_DEATH_REPORT = "Newly Reported Death";
        public const string INC_REPORT_DEATH_REPORT = "Incorrectly Reported Death";
        public const string DEATH_MATCH_BATCH = "DEATH_MATCH_BATCH";
        #endregion

        #region Workflow

        public const string ResumeActionAllDocuments = "ALLD";
        public const string ResumeActionAnyDocument = "ANYD";

        public const string ActivityStatusInitiated = "UNPC";
        public const string ActivityStatusInProcess = "INPC";
        public const string ActivityStatusProcessed = "PROC";
        public const string ActivityStatusReleased = "RELE";
        public const string ActivityStatusSuspended = "SUSP";
        public const string ActivityStatusResumed = "RESU";
        public const string ActivityStatusCancelled = "CANC";
        public const string ActivityStatusReturned = "RETU";
        public const string ActivityStatusReturnedToAudit = "REAU";

        public const int Map_Process_Death_Match = 266;
        public const int Map_Update_SSN_ToPerson_Record = 277;
        public const int Map_Split_Person_Record = 278;
        public const int Map_Merge_Person_Record = 279;
        public const int Map_Resolve_Incoming_Mail = 337;

        ////Workflow Instance Process Stats
        public const string WorkflowProcessStatus_UnProcessed = "PSNP";
        public const string WorkflowProcessStatus_Processed = "PSP";
        public const string WorkflowProcessStatus_Ignored = "IGNO";

        ////Workflow Source
        public const string WorkflowProcessSource_Online = "ONLI";
        public const string Return_From_Audit_Flag_Yes = "Y";
        public const string Return_From_Audit_Flag_No = "N";

        ////Code IDs
        //public const int ImageDoc_Category_Code_ID = 603;
        //public const int FileNet_Document_Type_Code_ID = 604;
        public const int Payee_Account_Status_ID = 7027;
        public const int Payee_Account_Suspension_Reason_id = 7008;
        public const int Payee_Account_Terminated_Status_Reason_id = 7009;
        public const int PAYEE_ACCOUNT_SUBSYSTEM_ID = 7043;

        //Ticket#69506
        public const string PAYEE_REVIEW_STATUS_REASON_VALUE = "NACH";//New ACH Information.


        public const string SUBSYSTEM_TYPE_BENEFIT_PAYMENT = "PMNT";
        public const string SUBSYSTEM_TYPE_REIMBURSEMENTS = "REIM";
        public const string SUSSYSTEM_TYPE_PAYMENT_CANCEL = "PYCN";
        public const string SUSSYSTEM_TYPE_PAYMENT_RECLAIMED = "RCMD";

        public const int STATE_TAX_PROVIDER_ORG_ID = 114;
        public const int FED_TAX_PROVIDER_ORG_ID = 113;

    
        public const string ProcessType_Person = "PERS";
        public const string ProcessType_Org = "ORGN";

        public const string MyBasketFilter_WorkPool = "WKPO";
        public const string MyBasketFilter_WorkAssigned = "ASWO";
        public const string MyBasketFilter_CompletedWork = "COWO";
        public const string MyBasketFilter_SuspendedWork = "SUWO";


        public const string RETIREMENT_ESTIMATE_ACTIVITY_NAME= "Create/Update Benefit Calculation Estimate";
        public const string WITHDRAWAL_ESTIMATE_ACTIVITY_NAME = "Enter/Update Withdrawal Calculation Estimate";
        public const string PRERETIREMENT_DEATH_ESTIMATE_ACTIVITY_NAME= "Create/Update Pre-Retirement Death Benefit Estimate";
        public const string DISABILITY_ESTIMATE_ACTIVITY_NAME = "Create/Update Disability Estimate";
        public const string RETIREMENT_WORKFLOW_NAME = "nfmProcessRetirementApplication" ;
        public const string WITHDRAWAL_WORKFLOW_NAME = "nfmProcessWithdrawalApplication" ;
        public const string DEATH_NOTIFICATION_WORKFLOW_NAME = "nfmProcessDeathNotification";        
        public const string PRERETIREMENT_DEATH_WORKFLOW_NAME = "nfmProcessPreRetirementDeathApplication";
        public const string DISABILITY_WORKFLOW_NAME = "nfmProcessDisabilityApplication";
        public const string QDRO_WORKFLOW_NAME = "nfmDroApplicationWorkFlow";
        public const string POSTRETIREMENT_DEATH_WORKFLOW_NAME = "nfmProcessPostRetirementDeath";
        public const string POSTRETIREMENT_DEATH_PAYSURVIVOR_WORKFLOW_NAME = "nfmProcessPostRetirementDeathPaySurvivor";
        public const string POSTRETIREMENT_DEATH_PAYSURVIVOR_BOB_WORKFLOW_NAME = "nfmProcessPostRetirementDeathPaySurvivorBOB";
        public const string RE_CALCULATE_BENEFIT_WORKFLOW_NAME = "nfmProcessReCalculateBenefit";
        public const string POP_UP_PAYEE_ACCOUNT_WORKFLOW_NAME = "nfmProcessPopUpPayeeAccount";
        public const string UPDATE_PAYEE_ACCOUNT = "nfmProcessPayeeAccountForVerificationOfHoursBatch";
        public const string PROCESS_PAYEE_ACCOUNT = "nfmProcessPayeeAccount";
        public const string PROCESS_STOPREVERSAL_OR_OVERPAYMENT = "nfmProcessStopReversalOrOverpayment";
        public const string PROCESS_OVERPAYMENT_WORKFLOW = "nfmProcessOverpaymentWorkflow";
        public const string PROCESS_REEMPLOYMENT = "nfmProcessReemployment";
        public const string PROCESS_STOP_REISSUE_OR_RECLAMATION = "nfmProcessStopReissueorReclamation";
        public const string PROCESS_SSN_MERGE = "nfmProcessSSNMerge";
        public const string PROCESS_EARLY_TO_DISABILITY = "nfmProcessConversionOfEarlyRetirementToDisabilityRetirement";
        //PIR 258
        public const string SSN_MERGE_IAP_RECALCULATION = "nfmProcessSSNMergeIAPRecalculation";
       
        public const int RETIREMENT_WORKFLOW_PROCESS_ID = 5;
        public const int WITHDRAWAL_WORKFLOW_PROCESS_ID = 10;       
        public const int PRERETIREMENT_DEATH_WORKFLOW_PROCESS_ID = 11;
        public const int DISABILITY_WORKFLOW_PROCESS_ID = 9;
        public const int QDRO_WORKFLOW_PROCESS_ID = 1;
        public const int REEMPLOYMENT_WORKFLOW_PROCESS_ID = 22;
        public const int SSN_MERGE_WORKFLOW_PROCESS_ID = 23;
        public const int Payee_Account_WORKFLOW_PROCESS_ID = 1707;

        public const string QDRO_ESTIMATE_ACTIVITY_NAME = "Create QDRO Estimate";
        public const string QDRO_APPLICATION_ACTIVITY_NAME = "Create /Update QDRO Application";

        # endregion

        #region Constants Related to Person
        public const string PERSON_LOOKUP = "wfmPersonLookup";
        public const string PERMANENT_ADDRESS = "PERM";
        public const string TEMPRORARY_ADDRESS = "TEMP";
        public const string PHYSICAL_AND_MAILING_ADDRESS = "PYAM";
        public const string MAILING_ADDRESS = "MAIL";
        public const string PHYSICAL_ADDRESS = "PYSL";
        public const string PENSION_ADDRESS = "PENC";
        public const string HEALTH_ADDRESS = "HEAC";
        public const string PENSION_HEALTH_ADDRESS = "PEHE";
        public const int PIR_STATUS_ID = 40;
        public const string PERSON_MAINTENANCE = "wfmPersonMaintenance";
        public const string BENEFIT_LOOKUP = "wfmBenefitApplicationLookup";
        public const string PERSON_ACCOUNT_BENEFICIARY_GRID =  "grvPersonAccountBeneficiary";
        public const string DISABILITY_HISTORY_GRID = "grvDisabilityBenefitHistory";
        public const string BENEFICIARY_MAINTENANCE = "wfmBeneficiaryMaintenance";
        public const string PARTICIPANT_BENEFICIARY_MAINTENANCE = "wfmParticipantBeneficiaryMaintenance";
        public const string SSN_MERGE_DEMOGRAPHIC_MAINTENANCE = "wfmSSNMergeDemograhicsAndBenefitsMaintenance";
        public const string SSN_MERGE_LOOKUP = "wfmSSNMergeLookup";
        public const string Person_Lookup_Mail_Return = "wfmPersonforMailReturnLookup";
        public const string Organization_Lookup_Mail_Return = "wfmOrganizationforMailReturnLookup";
        public const string SSN_MERGE_HISTORY_LOOKUP = "wfmSSNMergeHistoryLookup";
        public const string PERSON_DEPENDENT_MAINTENANCE = "wfmPersonDependentMaintenance";
        public const string PERSON_CONTACT_MAINTENANCE = "wfmPersonContactMaintenance";
        public const string BENEFICIARY_LOOKUP = "wfmBeneficiaryLookup";
        public const string MPI_ID = "MPI_PERSON_ID";
        public const string mpi_per_id = "mpi_person_id";
        public const string sp_mpi_per_id = "SP.MPI_PERSON_ID";
        public const string OTHER = "Other";
        public const string CONTACT_ALTERNATE = "ALTR";
        public const string PLAN_GRID = "grvPlan";
        public const string PENSION_CONTACT_DESC = "Pension";
        public const string HEALTH_CONTACT_DESC = "Health";
        public const string COURT_CONTACT_VAL = "CORT";
        public const string Gaurdian_CONTACT_VAL = "GRDN";
        public const string PowOfAttr_CONTACT_VAL = "POAP";
        public const string Attorney_CONTACT_VAL = "ATRN";
        public const string Conservator_CONTACT_VAL = "COAP";
        public const string Petitioner_CONTACT_VAL = "PETR";
        public const string Respondent_CONTACT_VAL = "RESP";
        public const string MALE = "M";
        public const string FEMALE = "F";
        public const string UNKNOWN = "U";
        public const string MR = "MR";
        public const string MRS = "MRS";
        public const string MISS = "MISS";
        public const string MAIL = "MAIL";
        public const string EMAL = "EMAL";
        public const string SSN = "ssn";
        public const string SP_SSN = "sp.ssn";
        public const string OLD_SSN = "old_ssn";
        public const string NEW_SSN = "new_ssn";
        public const string SP_PREFIX = "sp.";
        public const string PERSON_TYPE_PARTICIPANT = "PART";
        public const string PERSON_TYPE_SURVIVOR = "SURV";
        public const string PERSON_TYPE_ALTERNATE_PAYEE = "ALTP";

        public const string PIR_STATUS_In_Production = "CLOS";
        public const string PIR_STATUS_Cancelled = "CNCL";
        public const string PIR_STATUS_Confirmed = "CNFM";
        public const string PIR_STATUS_Deploy_to_Systems_Test = "DEPL";
        public const string PIR_STATUS_Deploy_to_Production = "DPRD";
        public const string PIR_STATUS_Deploy_to_UAT = "DUAT";
        public const string PIR_STATUS_Deploy_to_UAT_Pending = "DUTP";
        public const string PIR_STATUS_Logged = "LOGD";
        public const string PIR_STATUS_Rejected = "RJCT";
        public const string PIR_STATUS_Rejected_ON_HOLD = "RJOH";
        public const string PIR_STATUS_Rejected_Move_to_Change_Mgmt = "RMSM";
        public const string PIR_STATUS_Reported = "RPRR";
        public const string PIR_STATUS_Ready_for_System_Test = "RSYS";
        public const string PIR_STATUS_Re_Test = "RTST";
        public const string PIR_STATUS_Work_in_progress = "WIPR";

      

        #endregion

        #region Country Code Values
        public const int USA = 0001;
        public const int AUSTRALIA=0011;
        public const int CANADA = 0036;
        public const int MEXICO=0133;
        public const int NewZealand = 0147;
        public const int OTHER_PROVINCE = 9999;
        #endregion

        #region BenefitApplication

        #region Vesting Rules
        public const string RULE_1 = "R1";
        public const string RULE_2 = "R2";
        public const string RULE_3A = "R3A";
        public const string RULE_3B = "R3B";
        public const string RULE_4 = "R4";
        public const string RULE_5 = "R5";
        public const string RULE_6 = "R6";

        public const string BIS_PARTICIPANT = "BIS-PARTICIPANT";
        public const string FORFIETURE = ",FORFEITURE AT THE END OF THE YEAR";
        public const string FORFEITURE_COMMENT = "FORFEITURE";
        public const string VESTED_COMMENT= "VESTED-";
        public const string VESTED_ON_MERGER = "MPI_VESTED_ON_MERGER_WITH";
        #endregion

        public const int DRO_APPLICATION_STATUS_CODE_ID = 6005;
        public const int BENEFIT_APPLICATION_STATUS_CODE_ID = 1503;
        public const string BENEFIT_APPLICATION_STATUS_PENDING = "PEND";
        public const string BENEFIT_APPLICATION_STATUS_APPROVED = "APPR";
        public const string BENEFIT_APPLICATION_STATUS_CANCELLED = "CANC";
        public const string BENEFIT_APPLICATION_STATUS_DENIED = "DEND";
        public const string BENEFIT_APPLICATION_STATUS_INCOMPLETE = "INCP";
        public const string BENEFIT_APPLICATION_STATUS_PENDING_DESC = "Pending";
        public const string BENEFIT_CANCEL_OTHER = "OTHR";
        public const string BENEFIT_BATCH_CANCEL_AUTO = "Application expired. Auto Cancellation.";



        public const int BENEFIT_TYPE_CODE_ID = 1502;
        public const string BENEFIT_TYPE_RETIREMENT = "RTMT";
        public const string BENEFIT_TYPE_WITHDRAWAL = "WDRL";
        public const string BENEFIT_TYPE_DISABILITY = "DSBL";
        public const string BENEFIT_TYPE_DEATH_PRE_RETIREMENT = "DDPR";
        public const string BENEFIT_TYPE_DEATH_POST_RETIREMENT = "DDPT";
        public const string BENEFIT_TYPE_RETIREMENT_DESC = "Retirement";
        public const string BENEFIT_TYPE_WITHDRAWAL_DESC = "Withdrawal";
        public const string BENEFIT_TYPE_DISABILITY_DESC = "Disability";
        public const string BENEFIT_TYPE_QDRO = "QDRO";
        public const string BENEFIT_TYPE_DEATH_PRE_RETIREMENT_DESC = "Death(Pre-Retirement)";
         public const string BENEFIT_TYPE_DEATH_POST_RETIREMENT_DESC = "Death(Post-Retirement)";


        public const int RETIREMENT_TYPE_CODE_ID = 1501;
        public const string RETIREMENT_TYPE_NORMAL = "NORM";
        public const string RETIREMENT_TYPE_LATE = "LATE";
        public const string RETIREMENT_TYPE_REDUCED_EARLY = "REDE";
        public const string RETIREMENT_TYPE_SPL_REDUCED_EARLY = "SPLR";
        public const string RETIREMENT_TYPE_UNREDUCED_EARLY = "URED";
        public const string RETIREMENT_TYPE_MINIMUM_DISTRIBUTION = "MIND";
        public const string RETIREMENT_TYPE_NORMAL_DESC = "Normal Retirement";
        public const string PLAN_BENEFIT_ID = "plan_benefit_id";
        public const string BENEFIT_TYPE_VALUE = "benefit_type_value";
        public const string DISABILITY_TYPE_TERMINAL = "TRMD";
        public const string DISABILITY_TYPE_SSA = "SSAD";

        public const string PERSON_BRIDGED_HOURS_GRID = "grvPersonBridgeHours";
        public const string BENEFIT_APPLICATION_DETAIL_GRID = "grvBenefitApplicationDetail";
        public const string RETIREMENT_APPLICATION_MAINTAINENCE = "wfmRetirementApplicationMaintenance";
        public const string RETIREMENT_WIZARD = "wfmRetirementMaintenance";
        public const string WITHDRAWAL_APPLICATION_MAINTAINENCE = "wfmWithdrawalApplicationMaintenance";
        public const string DISABILITY_APPLICATION_MAINTAINENCE = "wfmDisabilityApplicationMaintenance";
        public const string BUS_BENEFIT_APPLICATION_DETAIL = "busBenefitApplicationDetail";
        public const string BUS_DISABILITY_BENEFIT_HISTORY = "busDisabilityBenefitHistory";
       
        public const string DEATH_PRE_RETIREMENT_MAINTANENCE = "wfmDeathPreRetirementMaintenance";
        public const string BENEFIT_CALCULATION_RETIREMENT_MAINTENANCE = "wfmBenefitCalculationRetirementMaintenance";
        public const string BENEFIT_CALCULATION_WITHDRAWL_MAINTENANCE = "wfmBenefitCalculationWithdrawalMaintenance";
        public const string BENEFIT_CALCULATION_PRE_RETIREMENT_MAINTENANCE = "wfmBenefitCalculationPreRetirementDeathMaintenance";
        public const string BUS_PERSON_BRIDGE_HOURS = "busPersonBridgeHours";

        public const string Joint_Survivor = "JS";
        public const string LEVEL_INCOME = "LI";
        public const string TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY = "10LA";
        public const string TEN_YEARS_TERM_CERTAIN = "10TC";
        public const string TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY = "2YLA";
        public const string THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY = "3YLA";
        public const string JOINT_100_PERCENT_SURVIVOR_ANNUITY = "J100";
        public const string JOINT_50_PERCENT_SURVIVOR_ANNUITY = "QJ50";
        public const string JOINT_75_PERCENT_SURVIVOR_ANNUITY = "JS75";
        public const string JOINT_75_PERCENT_POPUP_ANNUITY = "JP75";
        public const string JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY = "JA66";
        public const string JOINT_100_PERCENT_POPUP_ANNUITY = "JPOP";
        public const string JOINT_50_PERCENT_POPUP_ANNUITY = "JP50";
        public const string FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY = "5YLA";
        public const string LIFE_ANNUTIY = "LIFE";
        public const string LUMP_SUM = "LUMP";
        public const string LUMP_SUM_DESCRIPTION = "Lump Sum";
        public const string LIFE_ANNUTIY_DESCRIPTION = "Life Annuity";
        public const string SPOUSAL_CHECKBOX_ID = "chkSpousalConsent";
        public const string SPOUSAL_CHECKBOX_OBJECTFIELD = "icdoBenefitApplicationDetail.spousal_consent_flag";
        public const int TEN_YEAR_CERTAIN_MONTHS = 120;
        public const int FIVE_YEAR_CERTAIN_MONTHS = 60;
        public const int TWO_YEAR_CERTAIN_MONTHS = 24;
        public const int THREE_YEAR_CERTAIN_MONTHS = 36;
        

        public const int BENEFIT_OPTION_CODE_ID = 1504;
        public const string MERGER_DATE_STRING = "12/31/2003";
        public const int RETIREMENT_NORMAL_AGE = 65;
        public const int LOCAL_161_RETIREMENT_NORMAL_AGE = 60;
        public const int LOCAL_666_RETIREMENT_NORMAL_AGE = 65;
        public const int LOCAL_52_RETIREMENT_NORMAL_AGE = 65;
        public const int LOCAL_600_RETIREMENT_NORMAL_AGE = 65;
        public const int LOCAL_700_RETIREMENT_NORMAL_AGE = 62;
        public const decimal MIN_HOURS_FOR_VESTED_YEAR = 400;
        public const double LOCAL52_SPC_ACC_WDRL_DATE = 59.5;
        public const decimal LOCAL161_SPC_ACC_WDRL_DATE = 57;
        public const string DRO_APPROVED = "APRD";
        public const string DRO_CANCELLED = "CNLD";
        public const string DRO_QUALIFIED = "QLFD";
        public const string BENEFIT_APPL_APPROVED="APPR";
        public const string BENEFIT_APPL_CANCELLED = "CANC";
        public const string SURVIVOR_TYPE_PERSON = "PRSN";
        public const string SURVIVOR_TYPE_ORG = "ORGN";
        public const string SURVIVOR_TYPE_PER= "PER";
        public const string SURVIVOR_TYPE_ORGN = "ORG";
        public const string BRIDGED_SERVICE = "BRIDGED SERVICE ";
        public const string CONTRIBUTION_TYPE_EE = "EE";
        public const string CONTRIBUTION_TYPE_UVHP = "UVHP";
        public const string PERSON = "Person";
        public const string ORGANIZATION = "Organization";


        public const string ORGANIZATION_STATUS_ACTIVE = "A";
        public const string ORGANIZATION_STATUS_INACTIVE = "I";

        public const string L52_RULE_1 = "R1";
        public const string L52_RULE_2 = "R2";
        public const string L52_RULE_3 = "R3";
        public const string L52_RULE_4 = "R4";
        public const string L52_RULE_5 = "R5";
        public const string L52_RULE_6 = "R6";

        public const int RETIREMENT_APPLICATION_DISABILITY_CONVERSION = 107;
        #endregion BenefitApplication

        # region Notes Related
        public const int Form_ID = 6028;
        public const string QRDO_MAINTAINENCE_FORM = "QDRO";
        public const string RETIREMENT_APPLICATION_MAINTAINENCE_FORM = "RETR";
        public const string WITHDRAWL_APPLICATION_MAINTAINENCE_FORM = "WDRL";
        public const string WITHDRAWL_CALCULATION_MAINTAINENCE_FORM = "WDRC";
        public const string DISABILITY_APPLICATION_MAINTAINENCE_FORM = "DSBL";
        public const string DISABILITY_CALCULATION_MAINTAINENCE_FORM = "DSBC";
        public const string DEATH_PRE_RETIREMENT_MAINTANENCE_FORM = "DDPR";
        public const string DEATH_NOTIFICATION_MAINTANENCE_FORM = "DTHN";
        public const string PERSON_MAINTAINENCE_FORM = "PERS";
        public const string WF_CENTRELEFT = "WFCL";
        public const string WF_MANTAINENCE_FORM = "WFMN";
        public const string ORG_MAINTAINENCE_FORM = "ORGM";
        public const string PERSON_OVERVIEW_MAINTAINANCE_FORM = "PROV";
        public const string PERSON_ADDRESS_MAINTAINANCE_FORM = "PERA";
        public const string HARDSHIP_2025_IAP_WITHDRAWAL = "Hardship 2025 IAP Withdrawal";
        #endregion

        #region Organization
        public const string ORG_LOOKUP = "wfmOrganizationLookup";
        #endregion

        #region Correspondence
        public const string CORRSTATUS_READY_FOR_IMAGING = "REIM";
        public const string CORRSTATUS_IMAGED = "IMG";
        public const string CORRSTATUS_PRINTED = "PRNT";
        public const string CORRSTATUS_GENERATED = "GENR";

        /* Removed Not being used anywhere.
        public const string CORRSTATUS_PURGED = "PURG";
        public const string CORRSTATUS_ARCHIVED = "ARCH";
        */

        public const string RETIREMENT_APPLICATION_NORMAL_EARLY = "RETR-0001";
        public const string RETIREMENT_APPLICATION_MD_TO_LATE = "RETR-0002";
        public const string RETIREMENT_APPLICATION_LOCALS_52_600_666_700 = "RETR-0003";
        public const string RETIREMENT_APPLICATION_LOCAL_161 = "RETR-0004";
        public const string RE_EMPLOYMENT_RULES_ACKNOWLEDGEMENT = "RETR-0005";
        public const string RE_EMPLOYMENT_NOTIFICATION_FORM = "RETR-0006";
        public const string MEDICARE_COORDINATION_ACKNOWLEDGEMENT = "RETR-0007";
        public const string RETIREMENT_COUNSELING_CHECKLIST = "RETR-0008";
        public const string RETIREMENT_COUNSELING_SUMMARY_OF_PENDING_ITEMS = "RETR-0009";
        public const string RETIREMENT_APPLICATION_REQUEST_FORM = "RETR-0010";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION = "RETR-0011";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_SURVIVING_SPOUSE_PENSION = "RETR-0012";
        public const string ACTIVE_DEATH_PENSION_BENEFIT_ELECTION_COVER_LETTER_DEFAULTED_ANNUITY = "RETR-0013";
        public const string RETIREE_DEATH_REQUESTING_DEATH_CERTIFICATE_ONLY = "RETR-0014";
        public const string RETIREE_DEATH_LETTER_TO_SURVIVING_SPOUSE_TO_START_ANNUITY = "RETR-0015";
        public const string RE_EMPLOYMENT_APPROACHING_UNREDUCED_LIMIT = "RETR-0017";
        public const string WORK_HISTORY_REQUEST = "RETR-0018";
        public const string MISSING_DOCUMENT_REQUEST = "RETR-0020";
        public const string RETIREMENT_APPLICATION_RETIREE_HEALTH = "RETR-0021";
        public const string RETIREMENT_BENEFIT_ESTIMATE_SUMMARY = "RETR-0022";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_NORMAL_EARLY = "RETR-0023";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_700 = "RETR-0024";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_666 = "RETR-0025";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_600 = "RETR-0026";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_52 = "RETR-0027";
        public const string SPECIAL_TAX_NOTICE_402_F = "RETR-0028";
        public const string PAYROLL_CALENDAR = "RETR-0029";
        public const string PHYSICIANS_CERTIFICATION_OF_INCAPACITY = "RETR-0030";
        public const string RETIREMENT_APPLICATION_DISABILITY_CONVERSION_CORR = "RETR-0031";
        public const string DIRECT_DEPOSIT_AUTHORIZATION = "RETR-0032";
        public const string LUMP_SUM_DISTRIBUTION_ELECTION = "RETR-0033";
        public const string DRO_LUMP_SUM_DISTRIBUTION_ELECTION = "DRO-0033";
        public const string LUMP_SUM_DISTRIBUTION_ELECTION_NON_SPOUCE = "PAYEE-0035";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_161 = "RETR-0034";
        public const string EMPLOYEE_CONTRIBUTION_BALANCE = "RETR-0035";
        public const string NON_RESIDENT_ALIEN_STATUS = "RETR-0036";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_SURVIVING_SPOUSE_IAP = "RETR-0037";
        public const string GENERAL_BENEFIT_ESTIMATE = "RETR-0038";
        public const string GENERAL_BENEFIT_ESTIMATE_52 = "RETR-0520";  //rid 78456
        public const string GENERAL_BENEFIT_ESTIMATE_700 = "RETR-0700"; //rid 78456
        public const string GENERAL_BENEFIT_ESTIMATE_666 = "RETR-0666"; //rid 78456
        public const string GENERAL_BENEFIT_ESTIMATE_600 = "RETR-0600"; //rid 78456
        public const string GENERAL_BENEFIT_ESTIMATE_161 = "RETR-0161"; //rid 78456
        public const string ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_PENSION_AND_IAP = "RETR-0039";
        public const string ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_PENSION_ONLY = "RETR-0040";
        public const string CANCELLATION_NOTIFICATION = "RETR-0041";
        //TICKET#69388
        public const string RETIREMENT_CANCELLATION_FORM = "RETR-0052";
        public const string IAP_ANNUITY_QUOTE_CONFIRMATION_OF_ANNUITY_PAYMENT_TO_PAYEE = "RETR-0042";
        public const string IAP_ONLY_MD_PACKAGE_COVER_LETTER = "RETR-0043";
        public const string RETIREMENT_PACKAGE_COVER_LETTER = "RETR-0044";
        public const string TRYING_TO_LOCATE_BENEFICIARY = "RETR-0045";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_SURVIVING_SPOUSE_BOTH = "RETR-0046";
        public const string ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_IAP_ONLY = "RETR-0047";
        public const string CONVERT_MD_TO_LATE_RETIREMENT_APPLICATION_COVER_LETTER = "RETR-0048";
        public const string RETIREMENT_APPLICATION_COVER_LETTER = "RETR-0049";
        public const string CANCELLATION_NOTIFICATION_WITHDRAWAL = "RETR-0050";

        public const string RETIREE_HEALTH_PACKET = "RETR-0059";

        public const string RETIREE_HEALTH_PLAN_ANNUAL_WORK_HISTORY_SUMMARY = "RETR-0069";

        public const string RETIREMENT_APPLICATION_TERMINAL_ILLNESS = "DIS-0002"; 
        public const string RETIREMENT_APPLICATION_DISABILITY = "DIS-0003"; 
        public const string PHYSICIANS_STATEMENT_TERMINALLY_ILL = "DIS-0004";
        public const string BENEFIT_OPTION_DESCRIPTION = "DIS-0005";
        public const string BENEFIT_OPTION_DESCRIPTION_IAP = "DIS-0006";
        public const string DISABILITY_AWARD_LETTER = "DIS-0007";
        public const string DISABILITY_CONVERSION_COVER_LETTER = "DIS-0008";
        public const string DISABILITY_CONVERSION_DENIAL_LETTER = "DIS-0009";
        public const string DISABILITY_PENSION_APPLICATION_COVER = "DIS-0010";
        public const string DISABILITY_PENSION_REQUIREMENTS = "DIS-0011";
        public const string DISABILITY_RETIREMENT_CONVERSION_CONFIRMATION_LETTER = "DIS-0012";
        public const string IAP_DISABILITY_BENEFIT_APPLICATION = "DIS-0013";
        public const string IAP_DISABILITY_BENEFIT_OPTION_FORM = "DIS-0014";
        public const string IAP_ONLY_DISABILITY_PACKAGE_COVER_LETTER = "DIS-0015";
        public const string MEDICARE_COORDINATION_INFORMATION = "DIS-0016";
        public const string DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE = "DIS-0028";

        //PIR-827
        //public const string PROOF_OF_SSA_CONTINUOUS_DISABILITY_ALTERNATE_PAYEE = "DIS-0017";
        //public const string PROOF_OF_SSA_CONTINUOUS_DISABILITY = "DIS-0018";       
        public const string PROOF_OF_SSA_CONTINUOUS_DISABILITY = "DIS-0017";
        
        

        public const string TERMINALLY_ILL_COVER_LETTER = "DIS-0019";
        public const string WAIVER_OF_DISABILITY_INTEREST_ALTERNATE_PAYEE = "DIS-0020";
        public const string DISABILITY_RETIREMENT_PACKAGE_COVER_LETTER = "DIS-0021";

   

        public const string RETIREMENT_APPLICATION_IAP_WITHDRAWAL = "WIDRWL-0001";
        public const string RETIREMENT_APPLICATION_LOCAL_161_WITHDRAWAL = "WIDRWL-0002";
        public const string RETIREMENT_APPLICATION_EE_UVHP_WITHDRAWAL = "WIDRWL-0003";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_L161_SPECIAL_ACCOUNT = "WIDRWL-0004"; 
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_IAP_WITHDRAWAL = "WIDRWL-0005";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_EE_CONTRIBUTIONS_UVHP_WITHDRAWAL = "WIDRWL-0006";
        public const string IAP_EARLY_WITHDRAWAL_COVER_LETTER = "WIDRWL-0007";
        public const string EE_CONTRIBUTIONS_AND_UVHP_REFUND_COVER_LETTER = "WIDRWL-0008";
        //TICKET 68160
        public const string EE_CONTRIBUTIONS_AND_UVHP_WITHDRAWAL_REFUND_COVER_LETTER = "WIDRWL-0010";


        public const string AUTORIZATION_FOR_RELEASE_OF_PENSION_AND_IAP_INFORMATION = "DRO-0001";
        public const string QDRO_STATUS_PENDING = "DRO-0002";
        public const string QDRO_ALTERNATE_PAYEE_PENSION_PACKAGE_COVER_LETTER = "DRO-0003";
        public const string QDRO_APPROVED = "DRO-0004";
        public const string JUDGMENT_ADVERSE_INTEREST = "DRO-0005";
        public const string JOINDER_COVER_LETTER_TO_COURT = "DRO-0007";
        public const string NOTICE_OF_APPEARANCE_AND_RESPONSE_OF_EMPLOYEE_BENEFIT_PLAN_JOINDER = "DRO-0008";
        public const string JOINDER_NOTIFICATION_TO_PARTICIPANT = "DRO-0009";
        public const string _2009_MODEL_QDRO_NOT_IN_PAY_STATUS = "DRO-0010";
        public const string FAQ_1 = "DRO-0011";
        public const string MODEL_QDRO_RETIREE_JUSTIFIED = "DRO-0012";
        public const string QDRO_ESTIMATE = "DRO-00141";
        public const string QDRO_PENSION_BENEFIT_ELECTION_FORM_ALTERNATE_PAYEE_RETIREMENT_BENEFIT_ELECTION_FORM_QDRO = "DRO-0017";
        public const string QDRO_ALTERNATE_PAYEE_IAP_PACKAGE_COVER_LETTER = "DRO-0018";
        public const string QDRO_UVHP_PACKAGE_COVER_LETTER = "DRO-0019";
        public const string QDRO_FOR_SUPPORT = "DRO-0020";
        public const string QDRO_STATUS_PENDING_90_DAYS = "DRO-0022";
        public const string QDRO_BENEFIT_ELECTION_PACKET = "DRO-0034";
        public const string QDRO_IAP_SPECIAL_ACCOUNT_PACKET = "DRO-0035";


        public const string CHANGE_OF_ADDRESS_FORM = "PER-0001"; 
        public const string TAX_WITHHOLDING_FORM = "PER-0002"; 
        public const string NEW_PARTICIPANT_LETTER = "PER-0003"; 
        public const string MINIMUM_DISTRIBUTION_COVER_LETTER = "PER-0006";
        //rid 118418
        public const string AGE_72_RMD_ELECTION_FORM = "PER-0016"; 
        public const string MINIMUM_DISTRIBUTION_COVER_LETTER_REVISED = "PER-0006R";
        public const string RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION_REVISED = "RETR-0011R";

        public const string WITHHOLDING_ORDER_IRS_OR_FTB = "PER-0007"; 
        public const string NOT_AT_ADDRESS = "PER-0008"; 
        public const string SUSPENSION_OF_BENEFITS_NOTIFICATION = "PER-0009"; 
        //TICKET 68159
   //     public const string PENDING_BALANCE_BENEFIT_POTENTIAL_BENEFIT_NOTIFICATION = "PER-0010";
        public const string POA_REQUIREMENTS = "PER-0011";
        public const string RE_EMPLOYMENT_GENERAL_INFORMATION = "PER-0012";
        public const string SUBPOENA_RESPONSE_NO_RECORDS = "PER-0013";
        public const string SUBPOENA_RESPONSE_CERTIFICATION_OF_RECORDS = "PER-0014";
        public const string PENSION_AND_IAP_VERIFICATION = "PER-0015";

        public const string AUTHORIZATION_FOR_RELEASE_OF_PP_INFO = "PERO-0001"; 
        public const string NON_VESTED_BREAK_IN_SERVICE_NOTIFICATION = "PERO-0002"; 
        public const string AUTHORIZATION_FOR_RELEASE_OF_HEALTH_INFORMATION = "PERO-0003"; 
        public const string NOTIFICATION_OF_PENSION_ELIGIBILITY = "PERO-0004"; 
        public const string ONE_YEAR_BREAK_NOTIFICATION = "PERO-0005"; 
        public const string BREAK_IN_SERVICE_NOTIFICATION = "PERO-0006";
        public const string STATE_TAX_ADDRESS_CHANGE = "PAYEE-0053";

        public const string BENEFICIARY_FORM = "BENF-0001";
        public const string ACTIVE_DEATH_NON_SPOUSE_BENE = "BENF-0002";
       

        public const string REEMPLOYMENT_NOTIFICATION_FORM = "PAYEE-0001";
        public const string SSA_DISABILITY_STOP_OVERPAYMENT = "PAYEE-0002";
        public const string REEMPLOYMENT_WITHIN_TWO_MONTHS_OF_RETIREMENT = "PAYEE-0003";    
        public const string RETIREE_DEATH_NOTIFICATION_OF_NO_CONTINUING_BENEFITS_AND_ACH_REVERSALS_OF_OP = "PAYEE-0004";
        public const string CONFIRMATION_LETTER = "PAYEE-0005";
        public const string IAP_OUTSTANDING_CHECKS = "PAYEE-0006"; 
        public const string NOTIFICATION_OF_ANNUITY_PURCHASE_TO_INSURANCE_AGENCY = "PAYEE-0007";
        public const string NOTIFICATION_OF_HOURS_LETTER_TO_PAYEE = "PAYEE-0008";
        public const string NOTIFICATION_OF_IAP_PAYMENTS_TO_PAYEE = "PAYEE-0009";
        public const string REQUEST_OF_ANNUITY_QUOTE_OR_ESTIMATE_TO_INSURANCE_AGENCY = "PAYEE-0010";
        public const string RETIREMENT_AFFIDAVIT_COVER_LETTER = "PAYEE-0011";
        public const string THIRTY_DAY_FINAL_NOTICE_IAP = "PAYEE-0012";
        public const string CONFIRMATION_OF_ANNUITY_PAYMENT_TO_PAYEE = "PAYEE-0013";
        public const string RETIREMENT_AFFIDAVIT = "PAYEE-0014";
        public const string ROLLOVER_VERIFICATION_FORM = "PAYEE-0015";
        public const string VERIFICATION_OF_HOURS_LETTER_TO_EMPLOYER = "PAYEE-0016";
        public const string IRS_LEVY = "PAYEE-0017";
        public const string OUTSTANDING_PAYMENT_LETTER = "PAYEE-0018";
        public const string OVERPAYMENT_REIMBURSEMENT_REQUEST = "PAYEE-0019";
        public const string PENSION_ADJUSTMENT_NOTIFICATION = "PAYEE-0020";
        public const string STOP_WORKING_POST_RE_EMPLOYMENT_NOTIFICATION_FORM = "PAYEE-0021";
        public const string REPAYMENT_SCHEDULE = "PAYEE-0022";
        public const string RETIREE_INCREASE = "PAYEE-0023";
        public const string SSA_DISABILITY_BENEFITS_STOPS_ALTERNATE_PAYEE = "PAYEE-0024";
        public const string NOTIFICATION_OF_CHANGE_ACH = "PAYEE-0025";
        public const string OVERPAYMENT_REPAID_CONFIRMATION = "PAYEE-0026";
        public const string PENSION_INCOME_VERIFICATION = "PAYEE-0027";
        public const string RE_EMPLOYMENT_OVERPAYMENT_NOTICE_RE_PAYMENT_LETTER = "PAYEE-0028";
        public const string LOCAL_PLAN_RE_EMPLOYMENT = "PAYEE-0029";
        public const string IAP_RETIREMENT_CANCELLATION_NOTICE = "PAYEE-0030";
        public const string IAP_REPAYMENT_AUTHORIZATION_LETTER = "PAYEE-0031";
        public const string IAP_REPAYMENT_AUTHORIZATION_FORM = "PAYEE-0032";
        public const string ACH_REVERSAL_REQUEST = "PAYEE-0033";
        public const string LUMP_SUM_DISTRIBUTION_ELECTION_FROM_PAYEE = "PAYEE-0034";
        public const string QDRO_BENEFIT_CONFIRMATION = "PAYEE-0050";
        public const string IAP_QDRO_PAYMENT_NOTIFICATION = "PAYEE-0051";
        public const string IAP_ANNUITY_OPTION_ELECTION_CONFIRMATION = "PAYEE-0052";
        public const string IAP_HARDSHIP_PAYMENT_CONFIRMATION = "PAYEE-0054";       //HS23

        public const string REJECTION_OF_STATE_TAX = "PAYEE-0055";

        public const string CONFIRMATION_STATE_CHANGE_LETTER = "PAYEE-0056";

        public const string ONETIME_PENSION_PAYMENT_LETTER = "PAYEE-0057";

        public const string ANNUAL_BENEFIT_SUMMARY_LETTER = "PERO-0009"; //PIR 1003


        public const string RETIREMENT_APPLICATION_CANCELLATION_NOTICE = "PERO-0011";

        public const string DISABILITY_PENSION_BENEFIT_CONFIRMATION = "PAYEE-0038";
        public const string RETROACTIVE_BENEFIT_INCREASE_NOTICE = "PAYEE-0039"; //ChangeID: 58624

        public const string IAP_SECOND_PAYMENT_LETTER = "PAYEE-0041";//Ticket#74010

        public const string DEATH_POST_RETIREMENT_LIFE_ANNUNITY = "PAYEE-0042";//Ticket#75270

        public const string CONVERT_TO_LIFE_ANNUNITY_LETTER = "PAYEE-0043";//Ticket#75865

        public const string MINIMUM_DISTRIBUTION_BENEFIT_CONFIRMATION = "PAYEE-0045";//Ticket#69692

        public const string CONVERT_MD_TO_RD_CONF_LETTER = "PAYEE-0047";//Ticket#97167

        public const string IAP_PAYBACK_ANNUAL_BATCH_LETTER = "PAYEE-0049";

        public const string SURVIVING_SPOUSE_BENEFIT_CALCULATION_DETAILS = "BENF-0004";



        public const string Retirement_Application_Packet_Local_161 = "RETR-0055";
        public const string Retirement_Application_Packet_Local_52_600_666_700 = "RETR-0056";

        public const string IAP_Age_65_Retirement_Packet = "WIDRWL-0011";
        public const string IAP_Withdrawal_Packet = "WIDRWL-0012";
        public const string IAP_Special_Accounts_Packet = "WIDRWL-0013";
        public const string Withdrawal_EE_UVHP_Packet = "WIDRWL-0015";
        public const string Withdrawal_IAP_RETIREMENT_PACKET = "WIDRWL-0016";
        public const string Withdrawal_EE_UVHP_Retirement_Disablity_Packet = "WIDRWL-0018";

        public const string IAP_Hardship_Withdrawal_Form = "WIDRWL-0023";       //HS23
        public const string Withdrawal_COVID_Application = "WIDRWL-0019";
        public const string Withdrawal_EE_UVHP_REFUND_LETTER_A = "WIDRWL-0020";

        public const string Withdrawal_EE_UVHP_REFUND_LETTER_B= "WIDRWL-0021";

        public const string Withdrawal_EE_UVHP_REFUND_LETTER_C = "WIDRWL-0022";
        public const string MAILED_OUT_STATUS = "MOUT";


        public const string IAP_Disability_Packet = "DIS-0026";
        public const string IAP_Disability_Terminally_Ill_Packet = "DIS-0027";
        public const string Retirement_Application_Packet_MPI =  "RETR-0054";
        public const string Retirement_Application_Packet_L161 = "RETR-0055";
        public const string Retirement_Application_Packet_L52_600_666_7000 = "RETR-0056";
        public const string IAP_Election_Packet = "RETR-0062";
        public const string MD_TO_RD_PACKET = "RETR-0060";
        public const string IAP_RMD_PACKET = "RETR-0061";
        public const string Retirement_Benefit_Election_Packet_MPI = "RETR-0063";
        public const string Retirement_Benefit_Election_Packet_L52 = "RETR-0064";
        public const string Retirement_Benefit_Election_Packet_L161 = "RETR-0065";
        public const string Retirement_Benefit_Election_Packet_L600 = "RETR-0066";
        public const string Retirement_Benefit_Election_Packet_L666 = "RETR-0067";
        public const string Retirement_Benefit_Election_Packet_L700 = "RETR-0068";

        public const string MPI_Retirement_Workshop = "RETR-0071";

        public const string Disability_Benefit_Election_Packet_MPI  = "DIS-0028";
        public const string Disability_Benefit_Election_Packet_L52  = "DIS-0029";
        public const string Disability_Benefit_Election_Packet_L161 = "DIS-0030";
        public const string Disability_Benefit_Election_Packet_L600 = "DIS-0033";
        public const string Disability_Benefit_Election_Packet_L666 = "DIS-0032";
        public const string Disability_Benefit_Election_Packet_L700 = "DIS-0031";

        public const string Pre_Retirement_Death_non_spouse_Packet = "BENF-0003";

        public const string Beneficiary_IAP_Second_Payment_packet = "BENF-0009";

        public const string Active_Death_Beneficiary_Package_Pension_Iap = "BENF-0007";
        public const string Active_Death_Beneficiary_Package_Pension_Only = "BENF-0008";
        public const string Active_Death_Beneficiary_Package_Defaulted_Annunity = "BENF-0005";
        public const string Active_Death_Beneficiary_Package_LUMPSUM = "BENF-0006";

        public const string Disability_Application_Packet_Conversion = "DIS-0034";
        public const string Disability_Retirement_Not_Enough_HRS_YRS = "DIS-0035";
        public const string Disability_Retirement_BIS_Status = "DIS-0036";


        #endregion

        #region Beneficiary Relationship Types
        public const int BENEFICIARY_RELATIONSHIP_CODE_ID = 6000;
        public const string BENEFICIARY_RELATIONSHIP_CHILD = "CHLD";
        public const string BENEFICIARY_RELATIONSHIP_DOMESTIC_PARTNER = "DPAT";
        public const string BENEFICIARY_RELATIONSHIP_ESTATE = "ESTE";
        public const string BENEFICIARY_RELATIONSHIP_EXSPOUSE = "EXSP";
        public const string BENEFICIARY_RELATIONSHIP_FRIEND = "FRND";
        public const string BENEFICIARY_RELATIONSHIP_OTHER = "OTHR";
        public const string BENEFICIARY_RELATIONSHIP_PARENT = "PRNT";
        public const string BENEFICIARY_RELATIONSHIP_SIBLING = "SIBG";
        public const string BENEFICIARY_RELATIONSHIP_SPOUSE = "SPOU";
        public const string BENEFICIARY_RELATIONSHIP_TRUST = "TRST";
        //PIR-810
        public const string BENEFICIARY_RELATIONSHIP_SPOUSE_DESCRIPTION = "Spouse";
        #endregion
        #region EDD File
        
        public const int  EMPLOYER_CODE_ID = 7057;
        
        public const int EMPLOYER_STREET_ADDRESS_CODE_ID = 7058;
        public const int EMPLOYER_CITY_CODE_ID = 7059;
        public const int EMPLOYER_STATE_CODE_ID = 7060;
        public const int EMPLOYER_ZIP_CODE_EXTENSION_ID = 7061;
        public const int EMPLOYER_ZIP_CODE_ID = 7062;
        public const int STATE_CODE_ID = 7063;
        public const int EMPLOYER_STATE_EMPLOYER_ACCOUNT_NO_CODE_ID = 7064;
        
        public const int EMPLOYER_WAGE_PLAN_ID = 7065;
        public const string YEAR_END_PROC_1099R_ANNUAL = "A99R";
        public const string YEAR_END_PROC_ANNUAL_STATEMENT = "ASPA";
        public const string CORRECTED_PROC_ANNUAL_STATM = "C99R";
        public const string BatchRequest1099rStatusPending = "PEND";
        public const string BatchRequest1099rStatusComplete = "CMPL";
        public const string BatchRequest1099rStatusFailed = "FALD";
        public const string CORRECTED_IDENT_CANCEL = "CHNG";
        public const string CORRECTED_IDENT_SSN = "MERG";
        public const string CORRECTED_IDENT_DSBL = "DSBL";
        public const int Federal_CODE_Id = 7066;
        public const int STATE_CODE_Id = 7072;
        
        public const int TRANSMITTER_CONTROL_CODE = 7067;
        public const int CONTACT_NAME_CODE_ID = 7068;
        public const int CONTACT_EMAIL_CODE_ID = 7069;
        public const int EMPLOYER_NAME_CONTROL_CODE_ID = 7070;
        public const int AMOUNT_CODE_ID = 7071;
        public const int EMPLOYER_TEL_EXTENSION_ID = 7075;
        public const int OTHER_EDD_VALUE = 7076;
        #endregion

		//LA Sunset - Payment Directives
        #region Payment Directives

        public const int RECURRING_PAYMENT_DIRECTIVE = 1;
        public const int ONETIME_PAYMENT_DIRECTIVE = 2;
        public const int IAP_PAYMENT_DIRECTIVE = 3;

        public const int DISTRIBUTION_CODE_ID = 7091;

        public const string PAYMENT_DIRECTIVES= "DIRCTIVES";       
        #endregion Payment Directives

        public abstract class BenefitCalculation
        {
            public const string DISABILITY_CALCULATION_MAINTENANCE = "wfmDisabiltyBenefitCalculationMaintenance";
            public const string RETIREMENT_CALCULATION_MAINTENACE = "wfmBenefitCalculationRetirementMaintenance";
            public const string BENEFIT_CALCULATION_LOOKUP = "wfmBenefitCalculationLookup";
            public const int CALCULATION_TYPE_CODE_ID = 2001;
            public const string CALCULATION_TYPE_ESTIMATE = "ESTI";
            public const string CALCULATION_TYPE_FINAL = "FINL";
            public const string CALCULATION_TYPE_ADJUSTMENT = "ADCL";

            public const int CALCULATION_STATUS_TYPE_CODE_ID = 2002;
            public const string CALCULATION_STATUS_TYPE_PENDING = "PEND";
            public const string CALCULATION_STATUS_TYPE_APPROVED = "APPR";
            public const string CALCULATION_STATUS_TYPE_CANCELED = "CANC";

            public const int CALCULATION_BASED_ON_ID = 6039;
            public const string CALCULATION_BASED_ON_HOUR = "HOUR";
            public const string CALCULATION_BASED_ON_DAYS = "DAYS";
            public const string CALCULATION_BASED_ON_MONTHS = "MNTH";

            public const decimal REDUCTION_FACTOR_LOCAL_161 = 0.005M;
            public const decimal REDUCTION_FACTOR_LOCAL_52 = 0.00555556M;
            public const decimal REDUCTION_FACTOR_LOCAL_666 = 0.00555556M;
            public const decimal REDUCTION_FACTOR_LOCAL_600 = 0.005M;
            public const decimal REDUCTION_FACTOR_LOCAL_700 = 0.004M;

            public const string QUALIFIED_YEARS_200 = "200";
            public const string QUALIFIED_YEARS_20 = "20";
            public const string QUALIFIED_YEARS_10 = "10";

            public const string DATE_08_01_1979 = "08/01/1979";
            public const string DATE_08_01_1988 = "08/01/1988";
            public const string DATE_01_01_1988 = "01/01/1988";
            public const string DATE_01_01_1978 = "01/01/1978";

            public const decimal AGE_55 = 55.0m;
            public const decimal AGE_65 = 65.0m;

            public const string PLAN_B = "B";
            public const string PLAN_C = "C";
            public const string PLAN_Ca = "Ca";

            public const int YEAR_1979 = 1979;
            public const int YEAR_1987 = 1987;
            public const int YEAR_1988 = 1988;
            public const int BIS_COUNT_2 = 2;
            public const decimal QUALIFIED_HOURS_200 = 200.0M;
            public const decimal AGE_70_HALF = 70.5M;
            public const decimal AGE_71_HALF = 71.5M;
            public const decimal AGE_72 = 72M;
            public const int YEAR_1983 = 1983;

            public const int ACCOUNT_RELATIONSHIP_CODE_ID = 2003;
            public const string ACCOUNT_RELATIONSHIP_MEMBER = "PART"; //"MEMB"
            public const string ACCOUNT_RELATIONSHIP_BENEFICIARY = "BENE";
            public const string ACCOUNT_RELATIONSHIP_JOINT_ANNUITANT = "JANT";
            public const string ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE = "ALTP";
            public const int IAP_SET_UP_FEE = 450;

            public const decimal NORMAL_RETIREMENT_AGE_MPIPP = 65.00M;
            public const decimal NORMAL_RETIREMENT_AGE_IAP = 65.00M;
            public const decimal NORMAL_RETIREMENT_AGE_LOCAL_52 = 65.00M;
            public const decimal NORMAL_RETIREMENT_AGE_LOCAL_600 = 65.00M;
            public const decimal NORMAL_RETIREMENT_AGE_LOCAL_666 = 65.00M;
            public const decimal NORMAL_RETIREMENT_AGE_LOCAL_161 = 60.00M;
            public const decimal NORMAL_RETIREMENT_AGE_LOCAL_700 = 62.00M;
            public const decimal LUMP_SUM_LIMIT = 10000M;
            public const decimal LUMP_SUM_LIMIT_5000 = 5000M;
            public const decimal LUMP_SUM_CASH_OUT_LIMIT = 7000M;

            public static DateTime MERGER_DATE_LOCAL_600 = new DateTime(1999, 01, 01);
            public static DateTime MERGER_DATE_LOCAL_666 = new DateTime(1999, 01, 01);
            public static DateTime MERGER_DATE_LOCAL_700 = new DateTime(2002, 07, 01);
            public static DateTime MERGER_DATE_LOCAL_52 = new DateTime(2004, 01, 01);
            public static DateTime MERGER_DATE_LOCAL_161 = new DateTime(2005, 01, 01);
            public static DateTime DISABILITY_JS75_CHECK_DATE = new DateTime(2009, 01, 01);
            public static DateTime DISABILITY_IAP_ONLY_EFFECTIVE_DATE = new DateTime(2025, 04, 01);

            public const string FORFEITURE_COMMENT = "FORFEITURE";
            public const string RELATIVE_VALUE_APPROX_EQUAL = "Approx Equal";

            public const decimal LATE_ACTURIAL_INCREASE_PERCENTAGE = 0.012M;
            public const string BENEFIT_CALCULATION_OPTIONS_GRID = "grvBenefitCalculationOptions";

            public const int RMD_INCREASE_YEAR_SPECIAL_SCENARIO = 2017;
        }

        #region IAP Allocation

        public const string IAPAllocationInvestmentFlag = "I";
        public const string IAPAllocationForfeitureFlag = "F";
        public const string IAPAllocationNonAffiliatesFlag = "N";
        public const string IAPAllocationBothAffAndNonAffFlag = "B";

        public const string RCTransactionTypePayment = "PMNT";
        public const string RCTransactionTypeLateAllocation = "LALC";
        public const string RCTransactionTypeQuarterlyAllocation = "QALC";
        public const string RCTransactionTypeRetirementYear = "RETR";
        public const string RCTransactionTypeYearEndAllocation = "YREA";
        public const string RCTransactionTypeForfeiture = "FORF";
        public const string RCTransactionTypeAdjustment = "ADJS";
        public const string RCTransactionTypeBridging = "BRDG";
        public const string RCTransactionTypeCancelledCalc = "CNCA";
        public const string TransactionTypeInterest = "INTR";

        public const string RCContributionTypeAllocation1 = "ALC1";
        public const string RCContributionTypeAllocation2 = "ALC2";
        public const string RCContributionTypeAllocation3 = "ALC3";
        public const string RCContributionTypeAllocation4 = "ALC4";
        public const string RCContributionTypeAllocation5 = "ALC5";
        public const string RCContributionTypeEEContr = "EE";
        public const string RCContributionTypeLocalFrozenBenfit = "LCL";
        public const string RCContributionTypeUVHP = "UVHP";

        public const string RCContributionSubTypeInvestment = "IALC";
        public const string RCContributionSubTypeForfeited = "FALC";
        public const string RCContributionSubTypeAffiliates = "AFFL";
        public const string RCContributionSubTypeNonAffiliates = "NAFF";
        public const string RCContributionSubTypeBoth = "NAAF";

        public const string IAPAllocationCategoryActive = "ACTV";
        public const string IAPAllocationCategoryActiveDeathFirstPayment = "ADFP";
        public const string IAPAllocationCategoryActiveDeath = "ACTD";
        public const string IAPAllocationCategoryQDROActive = "DROA";
        public const string IAPAllocationCategoryEarlyWithdrawal = "EIAP";
        public const string IAPAllocationCategoryInActiveZeroBalance = "INAZ";
        public const string IAPAllocationCategoryMDActiveReeval = "MDAR";
        public const string IAPAllocationCategoryMDNew = "MDNW";
        public const string IAPAllocationCategoryMDRetiree = "MDRA";
        public const string IAPAllocationCategoryMisc = "MISC";
        public const string IAPAllocationCategoryNegBalance = "NBAL";
        public const string IAPAllocationCategoryNonVstdBIS = "NBIS";
        public const string IAPAllocationCategoryNewParticipants = "NEWP";
        public const string IAPAllocationCategoryRetiree = "RETR";
        public const string IAPAllocationCategoryReempOver65 = "RO65";
        public const string IAPAllocationCategoryReempUnder65 = "RU65";
        public const string IAPAllocationCategoryVstBIS = "VBIS";
        public const string RecalculateIAPAllocationDetailMaintenance = "wfmRecalculateIAPAllocationDetailMaintenance";

        #endregion

        #region CODE ID 52

        public const string QualifiedYearHours = "QFLD";
        public const string NonAffiliateUnionCodes = "NAFF";
        public const string IAPInceptionDate = "INCP";
        public const string IAPAllocation2Factor = "ALC2";
        public const string MPID = "MPID";
        public const string EMAIL_NOTIFICATION = "ADUS";
        public const int TaxOrg_Code_Id = 52;
        public const string FedTax_Code_Value = "FEDT";
        public const string StateTax_Code_Value = "STAT";
        #endregion

        #region CODE_ID_800
        public const int CODE_ID_MONTHS = 800;
        #endregion CODE_ID_800

        #region CODE_ID_1000
        public const int CONTRIBUTION_TYPE_CODE_ID = 1000;
        public const string CONTRIBUTION_TYPE_ALLOCATION_1 = "ALC1";
        public const string CONTRIBUTION_TYPE_ALLOCATION_2 = "ALC2";
        public const string CONTRIBUTION_TYPE_ALLOCATION_3 = "ALC3";
        public const string CONTRIBUTION_TYPE_ALLOCATION_4 = "ALC4";
        public const string CONTRIBUTION_TYPE_ALLOCATION_5 = "ALC5";
        public const string CONTRIBUTION_TYPE_LOCAL_BENEFITS = "LCL";
        #endregion CODE_ID_1000


        #region CODE_ID_1003
        public const int TRANSACTION_TYPE_CODE_ID = 1003;
        public const string TRANSACTION_TYPE_BEGINNING_BALANCE = "BBAL";
        public const string TRANSACTION_TYPE_INTEREST = "INTR";
        public const string TRANSACTION_TYPE_CBA_INCREASE = "CBAI";
        public const string TRANSACTION_TYPE_CANCELLED_CALCULATION = "CNCA";
        public const string TRANSACTION_TYPE_QUARTERLY_ALLOCATION  = "QALC";
        public const string TRANSACTION_TYPE_SSN_MERGE = "SSNM";
        public const string TRANSACTION_TYPE_SSN_ADJS = "ADJS";
        #endregion CODE_ID_1003

        #region CODE_ID_1004
        public const int CONTRIBUTION_SUBTYPE_CODE_ID = 1004;
        public const string CONTRIBUTION_SUBTYPE_INVESTMENT_RELATED_ALLOCATION = "IALC";
        public const string CONTRIBUTION_SUBTYPE_NON_VESTED = "NVES";
        public const string CONTRIBUTION_SUBTYPE_VESTED = "VEST";
        #endregion CODE_ID_1004

        #region Code_id 7040

        public const int INCREASE_PERCENT = 7040;
        public const string INCREASE_PERCENT_100 = "100";
        public const string INCREASE_PERCENT_200 = "200";

        #endregion

        #region Report Table Names
        public const string ReportTable01ForIAP = "ReportTable01";
        public const string ReportTableName01 = "ReportTableName01";
        public const string ReportTableName02 = "ReportTableName02";
        public const string ReportTableName03 = "ReportTableName03";
        public const string ReportTableName04 = "ReportTable04";
        public const string ReportTableName05 = "ReportTable05";
        public const string ReportTableName06 = "ReportTable06";
        public const string ReportTableName07 = "ReportTable07";
        public const string ReportTableName08 = "ReportTable08";
        public const string ReportTableName09 = "ReportTable09";
        public const string ReportTableName10 = "ReportTable10";

        #endregion

        #region Job Schedule Param Names

        public const string JobParamTotalAssetsFrmAccounting = "TotalAssetsFromAccounting";
        public const string JobParamUnallocableOverlimitAmtFrmAccounting = "UnallocableOverlimitAmtFrmAccounting";//PIR 630
        public const string JobParamIAPHourlyFrmAccounting = "IAPHourlyFromAccounting";
        public const string JobParamIAPPercentFrmAccounting = "IAPPercentFromAccounting";
        public const string JobParamTotalInvstAmtFrmAccounting = "TotalInvestmentIncomeFromAccounting";
        public const string JobParamMiscAdjustemntsFrmAccounting = "MiscAdjustmentsFrmAccounting";//PIR 630
        public const string JobParamAdmExpFrmAccounting = "AdministrativeExpensesFromAccounting";
        public const string JobParamPayoutsFrmAccounting = "PayoutsFrmAccounting";//PIR 630
        public const string JobParamWeightedAvgFrmAccounting = "WeightedAverageFromAccounting";
        //ChangeID: 59078
        public const int JobIAPYearEndAllocation = 28;

        public const string JobParamOverlimitInvIncomeOrLossFactor = "OverlimitInvIncome/LossFactor";
        public const string JobParamOverlimitInterest = "OverlimitInterest";
        public const string JobParamOtherMiscAdjustmentsFrmAccounting = "OtherMiscAdjustmentsFrmAccounting";

        //PIR 630 Suresh
        public const string JobParamAdjCutoffToDate = "AdjCutoffToDate";

        //PIR 1024
        public const string JobParamBatchYear = "BatchYear";

        public const string JobParamYear1099r = "ProcessName";
        public const string JobParamProcess1099r = "Year";
        public const string JobParamCorrectionStartDate1099r = "CorrectionStartDate";
        public const string JobParamStartDateEDD = "StartDateEDD";
        public const string JobParamEndDateEDD = "EndDateEDD";

        public const string JobParamRollover = "Rollover";
        public const string JobParamNonRollover = "NonRollover";
        public const string JobParamForeignAddress = "ForeignAddress";
        public const string JobParamLocalAddress = "LocalAddress";
        //WI 14763 RID 118342
        public const string JobParamApprovedGroupOnly = "ApprovedGroupOnly";

        public const string JobParamFromDate = "FromDate";
        public const string JobParamToDate = "ToDate";
        #endregion

        #region Payee Account
        public const string PAYEE_ACCOUNT_ROLLOVER_DETAIL_GRID = "grvPayeeAccountRolloverDetail";
        public const string PAYEE_ACCOUNT_ACH_DETAIL_GRID = "grvPayeeAccountAchDetail";
        public const string PAYEE_ACCOUNT_DEDUCTION_GRID="grvBusPayeeAccountDeduction";
        public const string PAYEE_ACCOUNT_ROLLOVER_MAINTENANCE = "wfmPayeeAccountRolloverMaintenance";
        public const string PAYEE_ACCOUNT_ROLLOVER_DETAIL = "busPayeeAccountRolloverDetail";
        public const string PAYEE_ACCOUNT_TAXWITHHOLDING_GRID = "grvPayeeAccountTaxWithholding";
        public const string PAYEE_ACCOUNT_TAXWITHHOLDING = "busPayeeAccountTaxWithholding";
        public const string PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE = "wfmPayeeAccountTaxwithholdingMaintenance";
        public const string PAYEE_ACCOUNT_FED_WIZARD_TAXWITHHOLDING_MAINTENANCE = "wfmPayeeAccountFedralWthHoldingMaintenance";
        public const string PAYEE_ACCOUNT_STATE_WIZARD_TAXWITHHOLDING_MAINTENANCE = "wfmPayeeAccountStateWthHoldingMaintenance";
        public const string PAYEE_ACCOUNT_ACH_WIZARD_MAINTENANCE = "wfmPayeeAccountACHDetailsWMaintenance";
        public const string PAYEE_ACCOUNT_DEDUCTION = "busPayeeAccountDeduction";
        public const string PAYEE_ACCOUNT_DEDUCTION_MAINTENANCE = "wfmPayeeAccountDeductionMaintenance";
        public const string FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX = "FTIA";
        public const string FEDRAL_TAX_IRS_TABLE = "FTIR";
        public const string PAYEE_ACCOUNT_ACH = "busPayeeAccountAchDetail";
        public const string PAYEE_ACCOUNT_WIRE = "busPayeeAccountWireDetail";
        public const string PAYEE_ACCOUNT_ACH_MAINTENANCE = "wfmPayeeAccountACHDetailsMaintenance";
        public const string PAYEE_ACCOUNT_WIRE_MAINTENANCE = "wfmPayeeAccountWireDetailMaintenance";
        public const string PAYEE_ACCOUNT_MAINTENANCE = "wfmPayeeAccountMaintenance";
        public const string PAYEE_ACCOUNT_STATUS = "busPayeeAccountStatus";
        public const string PAYMENT_HISTORY_DISTRIBUTION = "busPaymentHistoryDistribution";
        public const string PAYEE_ACCOUNT_STATUS_MAINTENANCE = "wfmPayeeAccountStatusMaintenance";
        public const string WITHHOLDING_INFORMATION_MAINTENANCE = "wfmWithholdingInformationMaintenance";
        public const string PAYMENT_REISSUE_DETAIL_MAINTENANCE = "wfmPaymentReissueDetailMaintenance";
        public const string REPAYMENT_SCHEDULE_MAINTENANCE = "wfmRepaymentScheduleMaintenance";
        public const string WITHHODING_INFORMATION = "busWithholdingInformation";
        public const string PAYMENT_REISSUE_DETAIL = "busPaymentReissueDetail";
        public const string REIMBURSEMENT_DETAILS = "busReimbursementDetails";
        public const string PAYEE_STATUS_APPROVED = "APRD";
        public const string FEDRAL_STATE_TAX = "FDRL";
        public const string CA_STATE_TAX = "STAT";
        public const string GA_STATE_TAX = "GAST";
        public const string NC_STATE_TAX = "NCST";
        public const string OR_STATE_TAX = "ORST";
        public const string VA_STATE_TAX = "VAST";
        public const string NO_STATE_TAX = "NSTX";
        public const string NO_FEDRAL_TAX = "NFTX";
        public const string CA_DESCRIPTION = "California";
        public const string VA_DESCRIPTION = "Virginia";
        public const string GA_DESCRIPTION = "Georgia";
        public const string NC_DESCRIPTION = "North Carolina";
        public const string OR_DESCRIPTION = "Oregon";
        public const string PAYEE_ACCOUNT_STATUS_GRID = "grvPayeeAccountStatus";
        public const string PAYMENT_SCHEDULE_MAINTENANCE = "wfmPaymentScheduleMaintenance";
        public const string PAYMENT_SCHEDULE_LOOKUP = "wfmPaymentScheduleLookup";
        public const string RECIPIENT_ROLLOVER_ORGANISATION_LOOKUP = "wfmRecipientRolloverOrganisationLookup";
        public const string RECIPIENT_ROLLOVER_ORGANISATION_MAINTENANCE = "wfmRecipientRolloverOrganisationMaintenance";
        public const string PAYMENT_REISSUE_DETAIL_GRID = "grvPaymentReissueDetail";
        public const string REIMBURSEMENT_DETAILS_GRID = "grvReimbursementDetails";
        public const string ORGANIZATION_BANK_GRID = "grvOrgBank";
        public const string ORGANIZATION_BANK_MAINTENANCE = "wfmOrganizationBankMaintenance";
        public const string ORG_BANK = "busOrgBank";
        public const string CONVERSION_DATA = "Conversion";
        public const int Retirement_Benefit_Election_PacketMPI = 198;
        public const int Retirement_Benefit_Election_Packet_Local_161 = 200;
        public const int Retirement_Benefit_Election_Packet_Local_52 = 199;
        public const int Retirement_Benefit_Election_Packet_Local_600 = 201;
        public const int Retirement_Benefit_Election_Packet_Local666 = 202;
        public const int Retirement_Benefit_Election_Packet_Local700 = 203;


        public const string TAX_WITHHOLDING_CALCULATOR_MAINTENANCE = "wfmTaxWithholdingCalculatorMaintenance";

        public const string BUS_PENSION_VERIFICATION_HISTORY = "busPensionVerificationHistory";

        public const string FLAT_PERCENT = "FLAP";
        public const int TWENTY_PERCENT = 20;
        public const int TWO_PERCENT = 2;
        public const int ONE_PERCENT = 1;
        public const string ITEM1 = "ITEM1";
        public const string ITEM2 = "ITEM2";
        public const string ITEM3 = "ITEM3";
        public const string ITEM4 = "ITEM4";
        public const string ITEM5 = "ITEM5";
        public const string ITEM6 = "ITEM6";
        public const string ITEM7 = "ITEM7";
        public const string ITEM8 = "ITEM8";
        public const string ITEM9 = "ITEM9";
        public const string ITEM10 = "ITEM10";
        public const string ITEM11 = "ITEM11";
        public const string ITEM12 = "ITEM12";
        public const string ITEM13 = "ITEM13";
        public const string ITEM14 = "ITEM14";
        public const string ITEM15 = "ITEM15";
        public const string ITEM16 = "ITEM16";
        public const string ITEM17 = "ITEM17";
        public const string ITEM18 = "ITEM18";
        public const string ITEM19 = "ITEM19";
        public const string ITEM20 = "ITEM20";
        public const string ITEM21 = "ITEM21";
        public const string ITEM22 = "ITEM22";
        public const string ITEM23 = "ITEM23";
        public const string ITEM24 = "ITEM24";
        public const string ITEM25 = "ITEM25";
        public const string ITEM26 = "ITEM26";
        public const string ITEM27 = "ITEM27";
        public const string ITEM28 = "ITEM28";
        public const string ITEM29 = "ITEM29";
        public const string ITEM30 = "ITEM30";
        public const string ITEM31 = "ITEM31";
        public const string ITEM32 = "ITEM32";
        public const string ITEM33 = "ITEM33";
        public const string ITEM34 = "ITEM34";
        public const string ITEM35 = "ITEM35";
        public const string ITEM36 = "ITEM36";
        public const string ITEM37 = "ITEM37";
        public const string ITEM38 = "ITEM38";
        public const string ITEM39 = "ITEM39";
        public const string ITEM40 = "ITEM40";
        public const string ITEM41 = "ITEM41";
        public const string ITEM42 = "ITEM42";
        public const string ITEM43 = "ITEM43";
        public const string ITEM44 = "ITEM44";
        public const string ITEM48 = "ITEM48";
        public const string ITEM49 = "ITEM49";
        public const string ITEM50 = "ITEM50";
        public const string ITEM51 = "ITEM51";
        public const string ITEM52 = "ITEM52";
        public const string ITEM53 = "ITEM53";
        public const string ITEM54 = "ITEM54";
        public const string ITEM55 = "ITEM55 ";

        public const string REEMPLOYMENT_RULE_1 = "REE1";
        public const string REEMPLOYMENT_RULE_2 = "REE2";
        public const string REEMPLOYMENT_RULE_3 = "REE3";
        public const string REEMPLOYMENT_RULE_4 = "REE4";
        public const string REEMPLOYMENT_RULE_5 = "REE5";

        public const string RESUMPTION_RULE_1 = "Resumption Rule 1 - Reemployed in first two months of Retirement.Two consecutive months with 0 hours.";
        public const string RESUMPTION_RULE_2 = "Resumption Rule 2 - Reported less than 50 hours in last payroll month.";
        public const string RESUMPTION_RULE_3 = "Resumption Rule 3 - Participant's age is over minimum distribution age.";
        public const string RESUMPTION_RULE_4 = "Resumption Rule 4 - Suspendible Month.";

        public const string Benefit_Distribution_Type_Monthly_Benefit = "MNBF";
        public const string Benefit_Distribution_Type_LumpSum = "LSDB";
        public const int Withholding_TaxAllowance = 3;
        public const string SpecialTaxIdendtifierFedTax = "FDTX";
        public const string RolloverItemReductionCheck = "RRED";
        public const string FedTaxOptionFedTaxBasedOnIRS = "FTIR";
        public const string FedTaxOptionFederalTaxwithheld = "FTWH";
        public const string FedTaxOptionFedTaxBasedOnIRSAndAdditional = "FTIA";
        public const string StateTaxOptionFedTaxBasedOnIRS = "STST";
        public const string StateTaxOptionFedTaxBasedOnIRSAndAdditional = "STAT";
        public const string PayeeAccountTerminationReasonDeath = "DETH";
        //Ticket#69506
        public const string PayeeAccountSuspensionReasonDeath = "DETH";

        public const string PAYEE_ACCOUNT_STATUS_CANCELLED = "CNCL";
        public const string PAYEE_ACCOUNT_STATUS_COMPLETED = "CMPL";
        public const string PAYEE_ACCOUNT_STATUS_RECEIVING = "RECV";
        public const string PAYEE_ACCOUNT_STATUS_APPROVED = "APRD";
        public const string PAYEE_ACCOUNT_STATUS_REVIEW = "REVW";
        public const string PAYEE_ACCOUNT_STATUS_SUSPENDED = "SPND";
        public const string PAYMENT_OPTION_REGULAR = "REGL";
        public const string PAYMENT_OPTION_SPECIAL_CHECK = "SPCK";
        public const string RETRO_PAYMENT_INITIAL = "IRPM";
        public const string RETRO_PAYMENT_REACTIVATION = "REAC";
        public const string RETRO_POPUP_BENEFIT = "POPB";
        public const string RETRO_RECALCULATION_PAYMENT = "RADJ";
        public const string RETRO_PAYMENT_BENEFIT_OVERPAYMENT = "OVER";
        public const string RETRO_PAYMENT_BENEFIT_UNDERPAYMENT = "UNDR";
        public const string RETRO_PAYMENT_QDRO = "QDRO";
        public const string RETRO_PAYMENT_WITHDRAWAL_BUY_BACK = "WDBB";
        public const string RETRO_PAYMENT_EARLY_TO_DISABILITY_CONVERSION = "ETDC";
        public const string RETRO_PAYMENT_ADJUSTMENT_BATCH = "BADJ";
        public const string RETRO_PAYMENT_REEMPLOYMENT = "REEP";
        public const string RETRO_PAYMENT_DEATH = "DETH";
        public const string RETRO = "Retro";
        public const string RETRO_PAYMENT_MD_ANNUAL_ADJUSTMENT = "AMDR";
        public const string RETRO_PAYMENT_RELEASE_WITHHOLDING = "REWH";
        public const string Suspension_Reason_For_Disability = "DSBN";
        public const string Suspension_Reason_For_Pension_Verification = "PBVC";

        public const string REIMBURSEMENT_STATUS_PENDING = "PEND";
        public const string REIMBURSEMENT_STATUS_PENDING_DESC = "Pending";

        public const string REIMBURSEMENT_STATUS_INPROGRESS = "INPR";
        public const string REIMBURSEMENT_STATUS_INPROGRESS_DESC = "In-Progress";
        public const string REIMBURSEMENT_STATUS_COMPLETED = "CMPL";

        public const string REIMBURSEMENT_STATUS_CANCELLED = "CNLD";

        public const string PACKET_STATUS_MAILED_OUT = "MOUT";

        public const string REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT = "PBEN";
        public const string REPAYMENT_PAYMENT_OPTION_PERSONAL_CHECK = "PRCK";
        public const string REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT_DESC = "Plan Benefit";
        public const string REPAYMENT_PAYMENT_OPTION_PERSONAL_CHECK_DESC = "Personal Check";

        public const string REIMBURSEMENT_PAYMENT_OPTION_CHECK = "CHEK";
        public const string REIMBURSEMENT_PAYMENT_OPTION_PENSION_CHECK = "PNCK";
        public const string REIMBURSEMENT_PAYMENT_OPTION_CHECK_DESC = "Check";
        public const string REIMBURSEMENT_PAYMENT_OPTION_PENSION_CHECK_DESC = "Pension Check";

        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_ACH_REJECTION = "ACHR";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_ADVERSE_INTEREST = "ADIN";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_BAD_ADDRESS = "BADA";
        //public const string PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED = "REED";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_1 = "REE1";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_2 = "REE2";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_3 = "REE3";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_4 = "REE4";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_5 = "REE5";
        public const string PAYEE_ACCOUNT_SUSPENSION_REASON_UNCONFIRMED_DEATH_REPORT = "UROD";

        public const string PayeeAccountRolloverOptionAllOfGross = "ALOG";
        public const string PayeeAccountRolloverOptionAllOfTaxable = "ALOT";
        public const string PayeeAccountRolloverOptionAmountOfTaxable = "DLOT";
        public const string PayeeAccountRolloverOptionPercentageOfTaxable = "PROT";
        public const string PayeeAccountRolloverOptionDollorOfGross = "DLOG";
        public const string PayeeAccountAllowRollover = "ROLL";

        public const string PayeeAccountRolloverDetailStatusActive = "ACTV";
        public const string PayeeAccountRolloverDetailStatusProcessed = "PRCS";
        public const string PayeeAccountRolloverDetailStatusCancelled = "CANC";

        public const string PAYMENT_METHOD_CHECK = "CHK";
        public const string PAYMENT_METHOD_ROLLOVER_CHECK = "RCHK";
        public const string PAYMENT_METHOD_ACH = "ACH";
        public const string PAYMENT_METHOD_ROLLOVER_ACH = "RACH";
        public const string PAYMENT_METHOD_WIRE = "WIRE";
        public const string PAYMENT_METHOD_DD = "Direct Deposit";
        public const string PAYMENT_METHOD_CHK = "Check";

        //Reissue Payment Types
        public const int REISSUE_PAYMENT_CODE_ID = 7048;
        public const string REISSUE_PAYMENT_TYPE_PAYEE = "PYEE";
        public const string REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR = "PTOS";
        public const string REISSUE_PAYMENT_TYPE_TRANSFER_ORGANIZATION = "TORG";
        public const string REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION = "PTRO";
        public const string REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION = "RLOG";
        public const string REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE = "ROTP";
        
        //Org Bank Status
        public const string OrgBankStatusActive="ACTV";
        public const string OrgBankStatusInactive = "INCT";

        //PAYMENT DISTRIBUTION STATUSES
        public const int PAYMENT_DISTRIBUTION_STATUS_ID = 7036;

        public const string PAYMENT_DISTRIBUTION_STATUS_CLEARED = "CLRD";
        public const string PAYMENT_DISTRIBUTION_STATUS_CANCELLED = "CNLD";
        public const string PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING = "OUTS";
        public const string PAYMENT_DISTRIBUTION_STATUS_OVERPAID = "OVRP";
        public const string PAYMENT_DISTRIBUTION_STATUS_RECLAIMED = "RCMD";
        public const string PAYMENT_DISTRIBUTION_STATUS_RECLAMATION_PENDING = "RMPD";
        public const string PAYMENT_DISTRIBUTION_STATUS_REISSUED = "RSUD";
        public const string PAYMENT_DISTRIBUTION_STATUS_REISSUE = "RSUE";
        public const string PAYMENT_DISTRIBUTION_STATUS_STALE = "STLE";
        //PIR:1040
        public const string PAYMENT_DISTRIBUTION_STATUS_3YRS = "3YRS";
        public const string PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT = "STPM";
        public const string PAYMENT_DISTRIBUTION_STATUS_CANCELLED_AND_REISSUE = "CDRE";

        //public const string PayeeAccountAccountRelationshipOwner = "MEMB";
        public const string BankAccountSavings = "SAVE";
        public const string BankAccountChecking = "CHKG";
        public const string PersonAccountBankAccountSavings = "SAV";
        public const string PersonAccountBankAccountChecking = "CHK";
        public const string CHECK_CLEARED = "CLRD";
        public const string PaymentDistributionPaymentMethodRCHK = "RCHK";
        public const string RCHKCleared = "RRCL";
        public const int PAID_CHECK = 11;

        public const string PaymentScheduleStepStatusPending = "PEND";
        public const string PaymentScheduleStepStatusProcessed = "PRCD";
        public const string PaymentScheduleStepStatusFailed = "FALD";
        public const string PaymentScheduleStatusReview = "REVW";
        public const string PaymentScheduleStatusValid = "VALD";
        public const string PaymentScheduleStatusProcessed = "PRCD";
        public const string RCHKOutstanding = "RROT";
        public const string CHKOutstanding = "COTS";

        public const string PaymentScheduleActionStatusPending = "PEND";
        public const string PaymentScheduleActionStatusFailed = "FALD";
        public const string PaymentScheduleActionStatusCancelled = "CNCL";
        public const string PaymentScheduleActionStatusProcessed = "PRCD";
        public const string PaymentScheduleActionStatusReadyforFinal = "RFNL";
        public const string PaymentScheduleActionStatusTrialExecuted = "TREX";
       
        
        public const string CreditTransactionCodeNonPrenoteChecking = "22";
        public const string CreditTransactionCodePrenoteChecking = "23";
        public const string CreditTransactionCodeNonPrenoteSavings = "32";
        public const string CreditTransactionCodePrenoteSavings = "33";

        public const string DebitTransactionCodeNonPrenoteChecking = "27";
        public const string DebitTransactionCodePrenoteChecking = "28";
        public const string DebitTransactionCodeNonPrenoteSavings = "37";
        public const string DebitTransactionCodePrenoteSavings = "38";
        public const int MonthlyPaymentBatchScheduleID = 80;
        public const int AdhocPaymentBatchScheduleID = 86;
        public const int AdhocTrialBatchScheduleID = 88;
        public const string ACHFileNameRetirmentVendorPayment = "DF.BK870034";
        public const string ACHFileNameInsuranceVendorPayment = "DF.BK870066";
        public const string ACHFileNameRetirmentEmployerPayment = "DF.BK870065";
        public const string ACHFileNameInsuranceEmployerPayment = "DF.BK870070";
        public const string ACHFileNameDefCompEmployerPayment = "DF.BK870071";
        public const string ACHFileNameDefCompVendorPayment = "DF.BK870067";
        //prod pir 6696
        public const string InsurancePremiums = "Insurance Premiums (IBS)";
        public const string RetirementAndInsurancePrenote = "Retirement And Insurance Prenote";
        public const string PensionPayments = "Pension payments";
        public const string RetirementVendorPayment = "Retirement Vendor Payment";
        public const string InsuranceVendorPayment = "Insurance Vendor Payment";
        public const string DefferedcompVendorPayment = "Deffered comp Vendor Payment";
        public const string RetirementEmployerPayment = "Employer Retirement Payment";
        public const string InsuranceEmployerPayment = "Employer Insurance Payment";
        public const string DefferedcompEmployerPayment = "Employer Deffered comp Payment";
        public const string ServiceCode_CreditsOnly = "220";
        public const string ServiceCode_DebitsOnly = "225";
        public const string ServiceCode_CreditDebitMixed = "200";
        public const string DateFormat = "MMddyy_HHmmss";
        public const string FileFormattxt = ".txt";

        public const string FundTypePureEE = "PUEE";
        public const string FundTypePureUVHP = "UVHP";
        public const string FundTypeLocal52SpecialAccount = "L052";
        public const string FundTypeLocal161SpecialAccount = "L161";
        public const string FundTypeEEandUVHPCombined = "EEUV";

        public const int GoLiveYear = 2013;//PIR 19

        public const string PensionMonthlyLimit = "PMTL";
        public const string IAPMonthlyLimit = "IAPL";

        public const string PaymentScheduleTypeMonthly = "MTLY";
        public const string PaymentScheduleAdhocMonthly = "ADHM";
        public const string PaymentScheduleTypeWeekly = "WKLY";
        public const string PaymentScheduleAdhocWeekly = "ADHW";
        public const string PaymentScheduleVendor = "VEND";
        public const string BenefitPaymentChangeGroupby = "G";


        public const string DeductionTypeValueIRSL = "IRSL";
        public const string DeductionTypeValueOther = "OTHR";
        public const string DeductionTypeValueChildSupport = "CHDS";
        public const string DeductionTypeValueSpousalSupport= "SPLS";

        public const string FLAT_DOLLAR = "FLAD";

        public const string PayeeAccountStatusApproved = "APRD";
        public const string PayeeAccountStatusReceiving = "RECV";
        public const string PayeeAccountStatusDCReceiving = "DCRC";
        public const string PayeeAccountStatusReview = "REVW";
        public const string PayeeAccountStatusPreDeathReview = "RDRW";
        public const string PayeeAccountStatusPostDeathReview = "RPRW";
        public const string PayeeAccountStatusCancelled = "CNLD";
        public const string PayeeAccountStatusPaymentComplete = "TRMD";
        public const string PayeeAccountStatusTerminated = "TRMD";
        public const string PayeeAccountStatusSuspended = "SPND";
        public const string PayeeAccountStatusCancelPending = "CNLP";
        public const string PayeeAccountStatusSuspendedDescription = "Suspended";

        public const int TAX_IDENTIFIER_ID = 7004;
        public const int BENEFIT_DISTRIBUTION_ID = 7005;
        public const int TAX_OPTION_ID = 7006;
        public const int MARITIAL_STATUS_ID = 7032;
        public const int ROLLOVER_OPTION_ID = 7007;
        //Rollover Types
        public const int ROLLOVER_TYPES_ID = 7041;
        public const string ANNUITY_SEC_403_A = "AS3a";
        public const string ANNUITY_SEC_403_B = "AS3b";
        public const string ANNUITY_SEC_408_B = "AS8b";
        public const string GOVT_DEFFERED_COMP_PLAN_SEC_457_B = "GS7b";
        public const string IRA_SEC_408_A = "IS8a";
        public const string QUALIFIED_TRUST_OR_PLAN_SEC_401_A = "QS1a";
        public const string ROTH_IRA_SEC_408_A = "RS8a";
        public const int STATUS_CODE_ID = 7029;
        public const int SCHEDULE_TYPE_CODE_ID = 7025;

        //6/15/2020 IAP Payback
        public const int MINIMUM_ACCEPTABLE_IAP_PAYBACK_AMOUNT = 100;

        #endregion

        #region CODE_ID_1509

        public const int BENEFICIARY_FORM_ID = 1509;
        public const string BENEFICIARY_FORM_PARTICIPANT = "PART";
        public const string BENEFICIARY_FORM_ALTERNATE_PAYEE = "ALTP";
        public const string BENEFICIARY_FORM_SURVIVOR = "SURV";

        #endregion
        #region CODE_ID_7078
        public const int CODE_ID_DOCUMENT_TYPE_SOURCE = 7078;
        public const string CODE_VALUE_DOCUMENT_TYPE_SOURCE_PENSION = "PEND";
        public const string CODE_VALUE_DOCUMENT_TYPE_SOURCE_HE = "HEDC";
        #endregion CODE_ID_7078

        public const string RECEIVABLE_CREATION_WITH_OR_WITHOUT_1099R = "RECR";
        public const string RECEIVABLE_CREATION_WITH_1099R = "REWT";
        public const string REEMPLOYMENT_OVERPAYMENT_TYPE = "REED";

        #region Data Extraction Batch Status Code

        public const int DATA_EXT_STATUS_CODE_ID = 7052;
        public const string VESTED_ACTIVE_PARTICIPANT = "I";
        public const string NON_VESTED_INACTIVE = "B";
        public const string VESTED_INACTIVE = "C";
        public const string PRE_RETIREMENT_DEATH = "D";
        public const string POST_RETIREMENT_DEATH = "E";
        public const string RETIRED_PART_BENEFICIARY = "F";
        public const string DISABILITY_BENEFITS = "G";
        public const string LUMP = "H";
        public const string NON_VESTED_ACTIVE_PARTICIPANT = "A";

        #endregion

        #region Data Extraction Benefit Option Code

        public const string DATA_EXT_LIFE_ANNUITY = "1";
        public const string DATA_EXT_TWO_YR_CERTAIN = "2";
        public const string DATA_EXT_TWO_YR_CERTAIN_LIFE = "3";
        public const string DATA_EXT_THREE_YR_CERTAIN = "4";
        public const string DATA_EXT_THREE_YR_CERTAIN_LIFE = "5";
        public const string DATA_EXT_FIVE_YR_CERTAIN = "6";
        public const string FIVE_YR_CERTAIN_LIFE = "7";
        public const string DATA_EXT_TEN_YR_CERTAIN = "8";
        public const string DATA_EXT_TEN_YR_CERTAIN_LIFE = "9";
        public const string DATA_EXT_JS_50 = "10";
        public const string DATA_EXT_JS_66 = "11";
        public const string DATA_EXT_JS_75 = "12";
        public const string DATA_EXT_JS_100 = "13";
        public const string DATA_EXT_JS_50_POP_UP = "14";
        public const string DATA_EXT_JS_75_POP_UP = "15";
        public const string DATA_EXT_JS_100_POP_UP = "16";

        #endregion        

        public const int STATE_ID = 150;

        public const int DATA_EXT_RET_TYPE_ID = 7053;
        public const string DATA_EXT_RET_TYPE_DISABILITY = "D";
        public const string DATA_EXT_RET_TYPE_REDUCED_EARLY = "E";
        public const string DATA_EXT_RET_TYPE_REGULAR_PESION = "N";
        public const string DATA_EXT_RET_TYPE_SP_UNREDUCED_EARLY = "S";

        public const int DATA_EXTRACTION_JOB_SCHEDULE_ID = 54;
        public const int HEALTH_ELIGIBILITY_ACTUARY_JOB_SCHEDULE_ID = 55;
        public const int ADDRESS_TYPE_ID = 6013;
        public const int BENEFICIARY_STATUS_ID = 77;
        public const int BRIDGE_TYPE_ID = 6038;

        public const string Retirement_Application_Maintenance_Form_2 = "wfmRtmtApplicationMaintenance";
        public const string Retirement_Application_Lookup_Form = "wfmRetirementApplicationLookup";

        //PIR 845 -- Path Constant
        public const string Correspondence_Path = "Correspondence\\Generated\\";
        public const string Report_Path = "Reports\\Pre Notification BIS Batch\\";

        public const string Report_Path_BIS = "Reports\\Notification BIS Batch\\";
        public const string Report_Path_MD = "Reports\\Min Distribution Batch\\";
        public const string Report_Path_PYBK = "Reports\\IAPHardshipPaybackBatch\\";

        public const string Report_Path_UVHP_EE = "Reports\\UVHP&EE\\";
        //72507
        public const string Report_Path_PensionEligibility = "Reports\\PensionEligibilityBatch\\";

        public const string Report_path_State_Tax_Update_Batch = "Reports\\State Tax Update Batch\\MergedFiles\\";
        public const string Report_Path_Approve10PercentIncrease = "Reports\\Approve10PercentIncreasePayeeAccountBatch\\";

        public const string Merged_Correspondence_path = "OneTimePensionPaymentCorr\\MergedFiles\\";


        public const string Report_IAP_Recalculation_Report_File_Path = "Reports\\ReCalculateIAPAllocation\\Report\\";

        //PIR:1040
        public const string Report_Path_STALE = "Reports\\Pre Notification STALE Batch\\";
        //PIR:868
        public const string MD_BATCH_NAME = "MD_BATCH";
        public const string NOTIFICATIONBIS_BATCH_NAME = "NotificationBISBatchReport";
        //PIR-859-Batch Name //PIR 628 LATEIAP batch name
        public const string LATE_IAP_ALLOCATION_BATCH = "LATEIAP"; 
        public const string BENEFIT_ADJUSTMENT_BATCH = "BENADJ";

        //PIR 337
        public const string Report_Path_Affidavit = "Reports\\RetirementAffidavitBatch\\";
        public const string Report_Path_Affidavit_Temp = "Reports\\RetirementAffidavitBatch\\Temp\\";
        public const string JobParamRetirementDateFrom = "RetirementDateFrom";
        public const string JobParamRetirementDateTo = "RetirementDateTo";

        //PIR 1003
        public const string Report_Path_Annual_Benefit_Correspondence = "Reports\\AnnualBenefitSummaryCorrespondenceBatch\\";
        public const string Report_Path_Annual_Benefit_Correspondence_Temp = "Reports\\AnnualBenefitSummaryCorrespondenceBatch\\Temp\\";

        //PIR 1024
         public const string ANNAUL_BENEFIT_SUMMARY_OVERVIEW_MAINTENANCE = "wfmAnnualBenefitSummaryOverviewMaintenance";
         public const string PERSON_OVERVIEW_MAINTENANCE = "wfmPersonOverviewMaintenance";

        public const string BENEFIT_CALCULATION_RETIREMENT_MAINTENACE = "wfmBenefitCalculationRetirementMaintenance";


        public const string PENSION_ACTUARY_REPORT = "rptPensionActuary";

        //rid 76227
        public const string PENSION_ELIGIBILITY_BATCH_REPORT = "rptPensionEligibilityBatchReport";
        public const string APPROVE_10_PERCENT_INCREASE_BATCH_REPORT = "rptApprove10PercentIncreaseReport";

        public const string MD_PARTICIPANT_ADDRESS_BATCH_REPORT = "rpt23_MDParticipantAddressReport";

        //ID_59363
        public const string REPORT_5500 = "rpt5500Report";

        //ID 59768
        public const string REPORT_ANNUAL_STATEMENT_PARTICIPANT_DETAIL = "rptAnnualStatementParticipantDetail";
        public const string REPORT_IAP_ADJUSTMENT_PAYMENT = "rptIapAdjustmentPayment"; 

        //ID 55732
        public const string REPORT_1099R_PENSION = "rpt1099RPensionDetails";
        public const string REPORT_1099R_IAP = "rpt1099RIAPDetails";

        //ID 68932
        public const string PENSION_VERIFICATION_HISTORY_NINETY_DAYS_LETTER = "PBV-0001";
        public const string PENSION_VERIFICATION_HISTORY_SIXTY_DAYS_LETTER = "PBV-0002";
        public const string PENSION_VERIFICATION_HISTORY_THIRTY_DAYS_LETTER = "PBV-0004";
        public const string PENSION_VERIFICATION_HISTORY_CONFIRMATION_LETTER = "PBV-0003";
        public const string PENSION_VERIFICATION_HISTORY_RESUMPTION_LETTER = "PBV-0005";

        public const int PENSION_VERIFICATION_HISTORY_NINETY_DAYS = 90;
        public const int PENSION_VERIFICATION_HISTORY_SIXTY_DAYS = 60;
        public const int PENSION_VERIFICATION_HISTORY_SUSPENSION = 30;
        public const string REPORT_PENSION_BENEFIT_VERIFICATION = "rptPensionBenefitVerification";
        public const string REPORT_RETIREE_HEALTH_ELIGIBLE = "rptRetireeHealthEligibilityList";
        public const string REPORT_RETIREE_HEALTH_ELIGIBLE_30_DAY = "rptRetireeHealthEligibilityList30Day";
        public const string REPORT_IAP_REQUIRED_MINIMUM_DISTRIBUTION = "rptIAPRequiredMinimumDistribution";

        public const string REPORT_ANNUAL_RECALCULATE_TAXES = "rptAnnualRecalculateTaxes";

        public const string REPORT_UVHP_EE_REFUND = "rptUVHPandEERefundList";

        public const string INPUT_MPI_RETIREMENT_WORKSHOP_FILE = "MPI_Retirement_Workshop";

        public const string LAST_ONE_YEAR_DEATH_NOTIFICATION_REPORT = "rptLastOneYearDeathNotificationReport";
        public const string VENDOR_PAYMENT_SUMMARY_REPORT = "rpt4_VendorPaymentSummary";
        public const string OREGON_PAYMENT_EDD_FILE_REPORT = "rptOregonPaymentEDDFileReport";
        public const string PAYMENT_EDD_FILE_REPORT = "rptPaymentEDDFileReport";
        public const string STATEMENT_PARTICIPANTS_WITH_STATEMENT_FLAG = "StatementParticipantsWithStatementFlag";

        public abstract class MSS
        {


            public const string PENSION_SPD = "PENSION_SPD.pdf";
            public const string L600_SPD = "LOCAL600_SPD.pdf";
            public const string L52_SPD = "LOCAL52_SPD.pdf";
            public const string L666_SPD = "LOCAL666_SPD.pdf";
            public const string L700_SPD = "LOCAL700_SPD.pdf";
            public const string L161_SPD = "LOCAL161_SPD.pdf";
            public const string WORK_HISTORY_REQUEST_MSS = "MSS-0018";
            public const string MSS_PENSION_IAP_VERIFICATION = "MSSPER-0015";
            public const string MSS_PENSION_INCOME_VERIFICATION = "MSSPAY-0027";

            public const string PERSON_PROFILE_FORM = "wfmPersonProfileMaintenance";
            public const string ACTIVE_MEMBER_HOME = "wfmMSSActiveMemberHomeMaintenance";
            public const string BEN_ESTI_MAINT_FORM = "wfmMssBenefitEstimateMaintenance";
            public const string ABS_FORM = "wfmMSSAnnualBenefitSummaryMaintenance";
            public const string APPLICATION_FORM = "wfmMssApplicationMaintenance";
            public const string APP_ACT_FORM = "wfmMssApplicationActivitiesMaintenance";
            public const string ANNUAL_STAT_FORM = "wfmMssAnnualStatementsMaintenance";
            public const string RETIREE_MEMBER_HOME = "wfmMSSRetireeMemberHomeMaintenance";
            public const string PLAN_SUMMARY_FORM = "wfmMSSPensionPlanMaintenance";

            public const string PLAN_DETAILS_FORM = "wfmMSSPlanInformationMaintenance";
            public const string VIEW_RETIREESTIM_FORM = "wfmMssViewRetirementEstimateMaintenance";

            

        }

        #region PIR Scout/Effort Hours Implementation
        public const int PirTaskCodeId = 2500;
        public const int MessageEffortHoursDescriptionUserIdTaskRequire = 9500;
        public const int MessageEffortDateCannotBeFutureDate = 9501;
        //PIR Scout/Effort Hours Implementation
        public const int MessageEffortHours = 9502;
        public const int MessageEffortDescription = 9503;        
        public const int MessageUserId = 9504;
        public const int MessageTaskRequire = 9505;
        public const int MessageEffortDate = 9506;
        #endregion


        public abstract class ReturnToWorkRequest
        {
            public const string STATUS_VALID = "VALD";
            public const string STATUS_REVEIW = "REVW";
            public const string STATUS_RECEIVING = "RECV";
            public const string YES = "Yes";
            public const string NO = "No";
            public const string MET = "Met";
            public const string NOT_MET = "Not Met";
            public const string STATUS_CANCEL = "CANC";
            public const string STATUS_PROCESSED = "PRCD";
            public const string SOURCE_INDEXING = "INDX";
            public const string SOURCE_BATCH = "BTCH";
            public const string SOURCE_ONLINE = "ONLI";
            public const string STATUS_UNPROCESSED = "UNPC";
            public const string STATUS_INPROGRESS = "INPC";
            public const string DOC_TYPE = "RETR0006";
            public const int DOC_TYPE_ID = 2021;
            public const int BPM_REQUEST_STATUS_ID = 3001;
            public const int RTW_BLACKOUT_DAYS = 8002;
            public const string CV_BLACKOUT_DAYS = "DAYS";
            public const int BLACK_OUT_START_DAY = 20;
            public const int BLACK_OUT_END_DAY = 23;
            public const int PAYEE_ACCOUNT_SUSPENSION_DAY = 24;
            public const string MAP_RETURN_TO_WORK = "sbpReturnToWork";
            public const string BPM_RETURN_TO_WORK_PROCESS_NAME = "Return To Work Process";
            public const string BPM_PRINT_REEMPLOYMENT_NOTIFICATION_FORM = "Print Re-employment Notification Form";
            public const string BPM_CAPTURE_AND_VALIDATION_ACTIVITY_NAME = "Capture And Validation";
            public const string BPM_PRINT_REEMPLOYMENT_CONFIRMATION_FORM = "Print Re-employment Confirmation Forms";
            public const string RETURN_TO_WORK_REQUEST_FORM = "RTW";
            public const string RETURN_TO_WORK_REQUEST_LOOKUP_FORM = "wfmReturnToWorkRequestLookup";
            public const string TERMINATION_REASON = "Terminated by user";
            public const string REQUEST_TYPE_RTW = "RTWO";
            public const string REQUEST_TYPE_ERTW = "ERTW";
            public const string MAP_END_RETURN_TO_WORK = "sbpendreturntowork";
            public const string BPM_END_RETURN_TO_WORK_PROCESS_NAME = "End of Return to Work Business Flow";
            public const string ACTIVITY_ERTW_GENERATE_CORR_AND_RESUME_PA = "Generate correspondence, report, resume payee account";
            public const string ACTIVITY_ERTW_CONDUCT_FIRST_AUDIT = "Conduct first audit";
            public const string ACTIVITY_ERTW_CONDUCT_SECOND_AUDIT = "Conduct second audit";
            public const string ACTIVITY_MAIL_CONFIRMATION_LETTER = "Mail Confirmation Letter";
            public const string MONTH_OF_SUSPENDIBLE_SERVICE_REPORT = "RPT-0001";
            public const string PAYEE_ACCOUNT_SUSPENSION_REASON_RETURN_TO_WORK_BPM = "REED";
        }

        public abstract class BPM
        {
            public const int BPM_NOTES_CODE_ID = 2029;
            public const int RESUME_ACTION_CODE_ID = 25;
            public const string OVED = "OVED";
        }
        public abstract class PersonAccountMaintenance
        {
            public const string SIGNED_APPLICATION_FORM_RECEIVED_DATE = "SignedApplicationFormReceivedDate";
            public const string APPLICATION_FORM_SENT = "ApplicationFormSent";
            public const string QDRO_LEGAL_REVIEW_REQUIRED = "QDROLegalReviewRequired";
            public const string ELECTION_PACKET_SENT = "ElectionPacketSent";
            public const string ELECTION_PACKET_RECEIVED = "ElectionPacketReceived";
           // public const string SIGNED_DATE_AND_APPLICATION_RECEIVED_CHECK = "SignedDateAndApplicationReceivedCheck";
            public const string GENERATE_CANCELLATION_NOTICE = "GenerateCancellationNotice";
            public const string PERSON_ACCOUNT_ID = "PersonAccountId";
            public const string PAYEE_ACCOUNT_ID = "PayeeAccountId";
            public const string APPLICATION_ID = "ApplicationId";
            public const string PAYEE_ACCOUNT_APPROVED = "PersonAccountApproved";
            public const string PERSON_ID = "PersonId";
            public const string RETIREMENT_DATE = "RetirementDate";
            public const string RETIREMENT_DATE_DAY_BEFORE = "RetirementDateDayBefore";
            public const string PAYMENT_BEGIN_DATE_PROCESSED = "IsPaymentBenefitbeginDateIsProceesed";
            public const string PLAN_DESCRIPTION = "PlanDescription";
            public const string PAYEE_ACCOUNT_EXISTS = "PayeeAccountExists";
            public const string IAP_PLAN = "IsIAPPlan";
            public const string PLAN_ID = "PlanId";
            public const string IAP_WAIT_TIMER = "IapWaitTimer";
            public const string ACTIVITY_INSTANCE_ID = "iintActivityInstanceId";
            public const string SOURCE_OF_CANCELLATION = "SourceOfCancellation";
            public const string BPM_TIMER_EXPIRTU_BEFORE_ELECTION = "Election";
            public const string BPM_TIMER_EXPIRTU_BEFORE_APPLICATION = "Application";
            public const string PAYMENT_BENEFIT_BEGIN_DATE = "istrIsPaymentBenefitbeginDateIsProceesed";
            public const string SERVICE_RETIREMENT_APPLICATION_EXISTS = "istrIsServiceRetirementApplicationExists";
            public const string SIGNED_DOCUMENT_RECEIVED = "SignedDocumentReceived";
            public const string QDRO_LEGAL_REVIEW_FLAG = "QDROLegalReviewFlag";
            public const string BENEFIT_CALCULATION_APPROVED = "IsBenefitCalculationApproved";
            public const string SERVICE_RETIREMENT_BPM = "sbpserviceretirementbpm";
            public const string SERVICE_RETIREMENT_PROCESS = "Service Retirement Process";
            public const string CANCEL_SERVICE_RETIREMENT_BPM = "sbpsbpcancelapplication";
            public const string CANCEL_SERVICE_RETIREMENT_APPLICATION = "Cancel Application";
            public const string APPLICATION_SERVICE_RETIREMENT_BPM = "sbpapplicationmaintenance";
            public const string APPLICATION_SERVICE_RETIREMENT_PROCESS = "Application Maintenance";          
            public const string PAYEMENT_STATUS_CANCEL = "CNCL";
            public const string PAYMENT_STATUS_COMPLETED = "CMPL";
            public const string PAYMENT_STATUS_RECEIVING = "RECV";
            public const string APPLICATION_APPROVE_STATUS = "APPR";
            public const string SERVICE_RETIREMENT = "SR";
            public const string SIGNED_APPLICATION_FORM_AND_LEGAL_ACTIVITY = "Signed Application Form Received, perform legal review and generate Election packet";
            public const string CANCELLATION_NOTICE_RECEIVED_NOTICE = "Cancellation Notice received";
            public const string DOCUMENT_RECEIVED_ACTIVITY= "Document Received";
            public const string ELECTION_PACKET_RECEIVED_ACTIVITY = "Election Packet received";
            public const string PERSON_ACCOUNT_RETIREMENT_INTAKE_ACTIVITY= "Person Account Retirement Intake";
            public const string GENERATE_RETIREMENT_APPLICATION_CANCELLATION_NOTICE_ACTIVITY = "Generate Retirement  Application Cancellation Notice";
            public const string QDRO_LEAGL_REVIEW_BENEFIT_ELECTION_ACTIVITY = "QDRO Legal Review / Retirement Benefit Election Packet";
            public const string LEGAL_DOCUMENT_REVIEW_ACTIVITY= "Legal Document Review";
            public const string ENTER_RETIREMENT_APPLICATION = "Enter Retirement Application";
            public const string BENEFIT_CALCULATION_ID = "CalCulationId";
            public const string BENEFIT_RETIREMENT_DATE = "RetirementDate";
            public const string AUDIT_PAYEE_ACCOUNT = "Audit Payee Account";
            public const string CANCEL_APPLICATION = "Cancel Application";

        }
    }
}

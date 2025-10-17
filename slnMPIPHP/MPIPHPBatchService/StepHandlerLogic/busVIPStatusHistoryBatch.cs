using Microsoft.Reporting.WinForms;
using MPIPHP;
using MPIPHP.BusinessObjects;
using MPIPHPJobService;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

namespace MPIPHPJobService.StepHandlerLogic
{

    public class busVIPStatusHistoryBatch : busBatchHandler
    {
      
        public Collection<busVipStatusHistory> iclbVipStatusHistoryData { get; set; }
       
        public string istrfileName;
        public string istrGeneratedPath;
        public Collection<busPerson> iclbPerson { get; set; }
        public void LoadVIPStatusHistoryData()
        {

            var firstDayOfMonth = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, iobjSystemManagement.icdoSystemManagement.batch_date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            //Add a logic in OPUS to uncheck VIP flag if no hours found for a year
            DataTable adtCheckVIPInOpus = new DataTable();
            adtCheckVIPInOpus = busBase.Select("cdoVipStatusHistory.GetVIPFromOpus", new object[0] { });
            if (adtCheckVIPInOpus.Rows.Count > 0)
            {
                foreach (DataRow ldrVipPerson in adtCheckVIPInOpus.Rows)
                {

                    CheckParticipantInformation(ldrVipPerson);
                }

            }

            DataTable adtbVIPStatusHistoryDataForExcel = new DataTable();
            adtbVIPStatusHistoryDataForExcel = busBase.Select("cdoVipStatusHistory.GetDataForVIPStatusHistory", new object[2] { firstDayOfMonth, lastDayOfMonth });

            System.Data.DataColumn newColumnDate = new System.Data.DataColumn("Date", typeof(System.DateTime));
            newColumnDate.DefaultValue = iobjSystemManagement.icdoSystemManagement.batch_date;
            adtbVIPStatusHistoryDataForExcel.Columns.Add(newColumnDate);
            System.Data.DataColumn JobName = new System.Data.DataColumn("JobName", typeof(System.String));
            JobName.DefaultValue = "JOB_RAPFLAG";
            adtbVIPStatusHistoryDataForExcel.Columns.Add(JobName);

            adtbVIPStatusHistoryDataForExcel.TableName = "ReportTable01";
            if (adtbVIPStatusHistoryDataForExcel.Rows.Count > 0)
            {
                if (adtbVIPStatusHistoryDataForExcel != null && adtbVIPStatusHistoryDataForExcel.Rows.Count > 0)
                {
                    var value = CreateExcelReport(adtbVIPStatusHistoryDataForExcel, "rptRAPStatusHistory");
                    string fileName = value.istrfileName;
                    string FilePath = value.istrGeneratedPath;
                    string lstrMailFrom = iobjSystemManagement.icdoSystemManagement.email_notification;
                    lstrMailFrom = HelperUtil.GetData1ByCodeValue(52, busConstant.EMAIL_NOTIFICATION);
                    String lstrSubject = "RAP Status History Report";
                    //string lstrBody = "Hi,Please find the VIP Report list";
                    bool ablnHighPriority = true;
                    SendMail(lstrMailFrom, lstrSubject,
                            ablnHighPriority, true, fileName, FilePath);
                }

            }
        }



        private busVIPStatusHistoryBatch CreateExcelReport(DataTable ldtbResultTable, string astrReportName, string astrPrefix = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);

            byte[] bytes = rvViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            else
            {
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            var VIPStatusHistoryBatch = new busVIPStatusHistoryBatch
            {
                istrfileName = lstrReportFullName,
                istrGeneratedPath = labsRptGenPath,
            };
            return VIPStatusHistoryBatch;

        }


        /// <summary>
        /// Add Attachment to email
        /// </summary>
        /// <param name="astrFrom"></param>
        /// <param name="astrTo"></param>
        /// <param name="astrHeading"></param>
        /// <param name="astrMessage"></param>
        /// <param name="ablnHighPriority"></param>
        /// <param name="ablnHtmlFormat"></param>
        public void SendMail(string astrFrom, string astrSubject, bool ablnHighPriority, bool ablnHtmlFormat, string fileName, string FilePath)
        {
            int lIntRoleID = 502;
            DataTable adtbVIPManagerList = new DataTable();
            adtbVIPManagerList = busBase.Select("cdoVipStatusHistory.SelectVIPManagerList", new object[1] { lIntRoleID });

            string appSettings = MPIPHP.Common.ApplicationSettings.Instance.SmtpServer;
            MailMessage message = new MailMessage();
            message.From = new MailAddress(astrFrom);
            if (!string.IsNullOrEmpty(fileName))
            {
                message.Body = "Hello Managers,</br></br>Please click the below link of RAP Status History Report for this month .</br></br><a href = " + fileName + ">Click Here For RAP Status Report</a> <br></br>Thanks </br> </br> OPUS Admin";
            }
            else
            {
                message.Body = "Hello Managers,</br></br>The RAP Status History Batch completed successfully but, RAP Status History Report is not generated because there is no update on RAP flag for this month.</br></br><br></br>Thanks </br> </br> OPUS Admin";
            }
                message.IsBodyHtml = ablnHtmlFormat;
            message.Subject = astrSubject;
            if (adtbVIPManagerList.Rows.Count > 0)
            {
                foreach (DataRow drVIPManagerEmailList in adtbVIPManagerList.Rows)
                {
                    if (drVIPManagerEmailList["EMAIL_ADDRESS"].ToString() != null)
                    {
                        message.To.Add(drVIPManagerEmailList["EMAIL_ADDRESS"].ToString());
                    }
                }
            }

            if (ablnHighPriority)
            {
                message.Priority = MailPriority.High;
            }
            //if (!String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(FilePath))
            //{
            //    message.Attachments.Add(new Attachment(fileName));

            //}
            new SmtpClient(appSettings).Send(message);



               }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drssnVipPerson"></param>
        public void CheckParticipantInformation(DataRow ldrVipPerson)
        {
            bool lBoolCheckHour = false;
            string lStrSSN = null;
           int lIntPersonId = 0;
            int lIntDependedId = 0;
            int lIntBeneficiariesId = 0;
            string lstrMPIPersonId = string.Empty;
            string lstrFirstName = string.Empty;
            string lstrMiddleName = string.Empty;
            string lstrlastName = string.Empty;
            string lstrGender = string.Empty;
            string lstrRelationshipType = string.Empty;
            string lstrVipFlag = string.Empty;
            var TodaysDate = DateTime.Now;
            string lstrHomePhone = string.Empty;
            string lstrCellPhone = string.Empty;
            string lstrFax = string.Empty;
            string lstrEmail = string.Empty;
            string lstrCreatedBy = string.Empty;
            string lstrIsPartitipant = busConstant.NO;
            var DateBeforeOneYear = TodaysDate.AddYears(-1);
            bool lboolupdatepartitipantSucessfull = false;
            string lStrMessage = null;

            if (ldrVipPerson["PERSON_ID"].ToString().IsNotNullOrEmpty())
            {
               
                lIntPersonId = Convert.ToInt32(Convert.ToBoolean(ldrVipPerson["PERSON_ID"].IsDBNull()) ? busConstant.ZERO_INT : ldrVipPerson["PERSON_ID"]);
            }
            if (ldrVipPerson["SSN"].ToString().IsNotNullOrEmpty())
            {

                lStrSSN = ldrVipPerson["SSN"].ToString();
            }

            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            busPerson lbusBeneficiries = new busPerson { icdoPerson = new cdoPerson() };
            busPerson lbusDependent = new busPerson { icdoPerson = new cdoPerson() };
            if (lbusPerson.FindPerson(lIntPersonId))
            {
                lbusPerson.LoadInitialData();
                lstrIsPartitipant = lbusPerson.iblnParticipant;
            }

            //
            if (lstrIsPartitipant == busConstant.YES)
            {
                //Check if hours exist for last Year
                lBoolCheckHour = CheckHoursonReportedYear(lStrSSN, DateBeforeOneYear, TodaysDate);

                if (!lBoolCheckHour)
                {

                    if (lbusPerson!=null)
                    {
                        lbusPerson.icdoPerson.vip_flag = "N";
                        lbusPerson.icdoPerson.Update();
                       // decommissioning demographics informations, since HEDB is retiring.
                      //  lStrMessage=UpdateVIPInformationinHE(lbusPerson);

                        if (lStrMessage=="")
                        {
                            lStrMessage = " Participant Updated Successfully";
                        }
                        
                        UpdateVIPStatusHistory(lIntPersonId, lStrMessage);
                        lboolupdatepartitipantSucessfull = true;
                    }
                    if (lboolupdatepartitipantSucessfull)
                    {
                       
                        DataTable ldtbListBeneficiaries = busPerson.Select("cdoPerson.LoadAllBeneficiaries", new object[1] { lIntPersonId });
                        
                        if(ldtbListBeneficiaries.Rows.Count>0)
                        {
                            var distinctRowsBeneficiaries = (from DataRow dRow in ldtbListBeneficiaries.Rows
                                                where dRow["PERSON_ID"].IsNotNull()
                                                select new
                                                {
                                                    PERSON_ID = dRow["PERSON_ID"],
                                                }).Distinct();
                          
                            foreach (var drBenificiries in distinctRowsBeneficiaries)
                           
                            {
                                lIntBeneficiariesId = Convert.ToInt32(drBenificiries.PERSON_ID);
                            if (lbusBeneficiries.FindPerson(lIntBeneficiariesId))
                            {
                                lbusBeneficiries.LoadInitialData();
                                lstrIsPartitipant = busConstant.FLAG_NO;
                                lstrIsPartitipant = lbusBeneficiries.iblnParticipant;
                                if (lstrIsPartitipant == busConstant.NO)
                                {
                                    lbusBeneficiries.icdoPerson.vip_flag = "N";
                                    lbusBeneficiries.icdoPerson.Update();
                                    lStrMessage = "Beneficiries Updated Successfully";
                                    UpdateVIPStatusHistory(lIntBeneficiariesId, lStrMessage);
                                }
                            }
                        }
                        }
                        DataTable ldtblistDependednd = busPerson.Select("cdoPerson.GetDependentName", new object[1] { lIntPersonId });
                        if (ldtblistDependednd.Rows.Count > 0)
                        {
                            var distinctRowsDepended = (from DataRow dRow in ldtblistDependednd.Rows
                                                             where dRow["DEPENDENT_PERSON_ID"].IsNotNull()
                                                             select new
                                                             {
                                                                 DEPENDENT_PERSON_ID = dRow["DEPENDENT_PERSON_ID"],
                                                             }).Distinct();

                            foreach (var drDepended in distinctRowsDepended)

                            {
                                lIntDependedId = Convert.ToInt32(drDepended.DEPENDENT_PERSON_ID);
                                if (lbusDependent.FindPerson(lIntDependedId))
                                {
                                    lbusDependent.LoadInitialData();
                                    lstrIsPartitipant = busConstant.FLAG_NO;
                                    lstrIsPartitipant = lbusDependent.iblnParticipant;
                                 }
                                if (lstrIsPartitipant == busConstant.NO)
                                {

                                    lbusDependent.icdoPerson.vip_flag = "N";
                                    lbusDependent.icdoPerson.Update();
                                    lStrMessage = "Depended Updated Successfully";
                                     UpdateVIPStatusHistory(lIntDependedId, lStrMessage);

                                }

                            }
                        }

                                         

                    }
                   
                }

            }
        }


        public bool CheckHoursonReportedYear(string lStrSSN, DateTime DateBeforeOneYear, DateTime TodaysDate)
        {
            int lHoursFound = 0;
            bool lBoolCheckHour = false;
            IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
            if (lconLegacy != null)
            {
                IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                lobjParameter.ParameterName = "@SSN";
                lobjParameter.DbType = DbType.String;
                lobjParameter.Value = lStrSSN;
                lcolParameters.Add(lobjParameter);

                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                lobjParameter1.ParameterName = "@START_DATE";
                lobjParameter1.DbType = DbType.DateTime;
                lobjParameter1.Value = DateBeforeOneYear;
                lcolParameters.Add(lobjParameter1);

                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                lobjParameter2.ParameterName = "@END_DATE";
                lobjParameter2.DbType = DbType.DateTime;
                lobjParameter2.Value = TodaysDate;
                lcolParameters.Add(lobjParameter2);

                IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                lobjParameter3.ParameterName = "@RESULT";
                lobjParameter3.DbType = DbType.Int32;
                lobjParameter3.Direction = ParameterDirection.ReturnValue;
                lcolParameters.Add(lobjParameter3);

                DBFunction.DBExecuteProcedure("usp_CheckHoursReportedInGivenInterval", lcolParameters, lconLegacy, null);
                lHoursFound = Convert.ToInt32(lcolParameters[3].Value);
            }

            if (lHoursFound > 0)
            {
                lBoolCheckHour = busConstant.BOOL_TRUE;
            }
            else
            {
                lBoolCheckHour = busConstant.BOOL_FALSE;
            }
            return lBoolCheckHour;
        }
        // decommissioning demographics informations, since HEDB is retiring.
        //public string UpdateVIPInformationinHE(busPerson lbusperson)
        //{
        //    string lstrMessage = string.Empty;

        //public string UpdateVIPInformationinHE(busPerson lbusperson)
        //{
        //    string lstrMessage = string.Empty;

        ////    string lStrSSN = null;
        ////    int lIntPersonId = 0;
        ////    string lstrMPIPersonId = string.Empty;
        ////    string lstrFirstName = string.Empty;
        ////    string lstrMiddleName = string.Empty;
        ////    string lstrlastName = string.Empty;
        ////    string lstrGender = string.Empty;
        //    string lstrRelationshipType = string.Empty;
        ////    string lstrVipFlag = string.Empty;
        ////    var TodaysDate = DateTime.Now;
        ////    string lstrHomePhone = string.Empty;
        ////    string lstrCellPhone = string.Empty;
        ////    string lstrFax = string.Empty;
        ////    string lstrEmail = string.Empty;
        ////    string lstrCreatedBy = string.Empty;
        ////    var DateBeforeOneYear = TodaysDate.AddYears(-1);
        ////    bool lboolVipFlag = false;

        ////    utlPassInfo lobjPassInfo1 = new utlPassInfo();
        ////    lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
        ////    lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");
        ////    if (lbusperson != null)
        ////    {
        ////        lStrSSN = lbusperson.icdoPerson.ssn;
        ////        lstrMPIPersonId = lbusperson.icdoPerson.mpi_person_id;
        ////        lboolVipFlag = false;
        ////        lIntPersonId = lbusperson.icdoPerson.person_id;

        ////        try
        ////        {
        ////            string strQuery = "select * from person where ssn = (select ssn from Eligibility_PID_Reference where PID = '" + lstrMPIPersonId + "')";
        ////            DataTable ldtbResult = DBFunction.DBSelect(strQuery, lobjPassInfo1.iconFramework);
        ////            if (ldtbResult.Rows.Count == 0)
        ////            {
        ////                return lstrMessage;
        ////            }

        ////        }
        ////        catch
        ////        {

        ////        }
        ////        int CountDependent = (int)DBFunction.DBExecuteScalar("cdoPerson.ChechPersonIsDependent", new object[1] { lIntPersonId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        ////        int CountBeneficiary = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPersonIsBeneficiary", new object[1] { lIntPersonId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        ////        if (CountDependent > 0)
        ////        {
        ////            lstrRelationshipType = "D";
        ////        }
        ////        else if (CountBeneficiary > 0)
        ////        {
        ////            lstrRelationshipType = "B";
        ////        }
        ////        else
        ////        {
        ////            lstrRelationshipType = null;
        ////        }

        // //       }
        //        int CountDependent = (int)DBFunction.DBExecuteScalar("cdoPerson.ChechPersonIsDependent", new object[1] { lIntPersonId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        //        int CountBeneficiary = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPersonIsBeneficiary", new object[1] { lIntPersonId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        //        if (CountDependent > 0)
        //        {
        //            lstrRelationshipType = "D";
        //        }
        //        else if (CountBeneficiary > 0)
        //        {
        //            lstrRelationshipType = "B";
        //        }
        //        else
        //        {
        //            lstrRelationshipType = null;
        //        }

        ////            lstrFirstName = lbusperson.icdoPerson.first_name;

        ////        lstrMiddleName = lbusperson.icdoPerson.middle_name;

        ////            lstrlastName = lbusperson.icdoPerson.last_name;

        ////        lstrGender = lbusperson.icdoPerson.gender_value; ;

        ////        DateTime lstrDOB = DateTime.MinValue;
        ////        DateTime lstrDateOfDeath = DateTime.MinValue;

        ////        if (lstrFirstName.IsNotNullOrEmpty())
        ////        {
        ////            lstrFirstName = lstrFirstName.ToUpper();
        ////        }

        ////        if (lstrMiddleName.IsNotNullOrEmpty())
        ////        {
        ////            lstrMiddleName = lstrMiddleName.ToUpper();
        ////        }

        ////        if (lstrlastName.IsNotNullOrEmpty())
        ////        {
        ////            lstrlastName = lstrlastName.ToUpper();
        ////        }

        ////            lstrDOB = lbusperson.icdoPerson.date_of_birth; 


        ////        lstrDateOfDeath = lbusperson.icdoPerson.date_of_death;

        //     //   lstrDateOfDeath = lbusperson.icdoPerson.date_of_death;

        ////        DateTime lstrDOD = DateTime.MinValue;

        ////        if (lstrDateOfDeath == DateTime.MinValue)
        ////        {
        ////            DataTable ldtblGetDateOfDeath = busBase.Select("cdoDeathNotification.GetDateOfDeathInProgress", new object[1] { lIntPersonId });
        ////            if (ldtblGetDateOfDeath != null && ldtblGetDateOfDeath.Rows.Count > 0
        ////                && Convert.ToString(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]).IsNotNullOrEmpty())
        ////            {
        ////                lstrDOD = Convert.ToDateTime(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]);
        ////            }
        ////        }
        ////        else
        ////        {
        ////            lstrDOD = lstrDateOfDeath;
        ////        }
        ////       lstrHomePhone = lbusperson.icdoPerson.home_phone_no;


        ////        lstrFax = lbusperson.icdoPerson.fax_no; 

        ////            lstrEmail = lbusperson.icdoPerson.email_address_1; ;


        ////            lstrCreatedBy = lbusperson.icdoPerson.modified_by; ;


        ////        // bool lboolUpdateinHEsucessfull = false;
        ////        if (lobjPassInfo1.iconFramework != null)
        ////        {

        ////              //If Record updated in VIP History ,send the changes to HE         
        ////                Collection<IDbDataParameter> lcolParametersToUpdateHEDB = new Collection<IDbDataParameter>();
        ////                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
        ////                lobjParameter1.ParameterName = "@PID";
        ////                lobjParameter1.DbType = DbType.String;
        ////                lobjParameter1.Value = lstrMPIPersonId.ToLower();
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter1);

        ////                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
        ////                lobjParameter2.ParameterName = "@SSN";
        ////                lobjParameter2.DbType = DbType.String;

        ////                if (lStrSSN.IsNullOrEmpty())
        ////                    lobjParameter2.Value = DBNull.Value;
        ////                else
        ////                    lobjParameter2.Value = lStrSSN.ToLower();
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter2);

        ////                IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
        ////                lobjParameter3.ParameterName = "@ParticipantPID";
        ////                lobjParameter3.DbType = DbType.String;
        ////                lobjParameter3.Value = DBNull.Value;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter3);

        ////                IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
        ////                lobjParameter4.ParameterName = "@EntityTypeCode";
        ////                lobjParameter4.DbType = DbType.String;
        ////                lobjParameter4.Value = "P";
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter4);

        ////                IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
        ////                lobjParameter5.ParameterName = "@RelationType";
        ////                lobjParameter5.DbType = DbType.String;
        ////                lobjParameter5.Value = lstrRelationshipType;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter5);

        ////                IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
        ////                lobjParameter6.ParameterName = "@FirstName";
        ////                lobjParameter6.DbType = DbType.String;
        ////                lobjParameter6.Value = lstrFirstName;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter6);

        ////                IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
        ////                lobjParameter7.ParameterName = "@MiddleName";
        ////                lobjParameter7.DbType = DbType.String;
        ////                lobjParameter7.Value = lstrMiddleName;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter7);

        ////                IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
        ////                lobjParameter8.ParameterName = "@LastName";
        ////                lobjParameter8.DbType = DbType.String;
        ////                lobjParameter8.Value = lstrlastName;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter8);

        ////                IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
        ////                lobjParameter9.ParameterName = "@Gender";
        ////                lobjParameter9.DbType = DbType.String;
        ////                lobjParameter9.Value = lstrGender;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter9);


        ////                IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
        ////                lobjParameter10.ParameterName = "@DateOfBirth";
        ////                lobjParameter10.DbType = DbType.DateTime;
        ////                if (lstrDOB != DateTime.MinValue)
        ////                {
        ////                    lobjParameter10.Value = lstrDOB;
        ////                }
        ////                else
        ////                {
        ////                    lobjParameter10.Value = DBNull.Value;
        ////                }
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter10);


        ////                IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
        ////                lobjParameter11.ParameterName = "@DateOfDeath";
        ////                lobjParameter11.DbType = DbType.DateTime;

        ////                if (lstrDOD != DateTime.MinValue)
        ////                {
        ////                    lobjParameter11.Value = lstrDOD;
        ////                }
        ////                else
        ////                {
        ////                    lobjParameter11.Value = DBNull.Value;
        ////                }
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter11);

        ////                IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
        ////                lobjParameter12.ParameterName = "@HomePhone";
        ////                lobjParameter12.DbType = DbType.String;
        ////                lobjParameter12.Value = lstrHomePhone;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter12);

        ////                IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
        ////                lobjParameter13.ParameterName = "@CellPhone";
        ////                lobjParameter13.DbType = DbType.String;
        ////                lobjParameter13.Value = lstrCellPhone;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter13);

        ////                IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
        ////                lobjParameter14.ParameterName = "@Fax";
        ////                lobjParameter14.DbType = DbType.String;
        ////                lobjParameter14.Value = lstrFax;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter14);

        ////                IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
        ////                lobjParameter15.ParameterName = "@Email";
        ////                lobjParameter15.DbType = DbType.String;
        ////                lobjParameter15.Value = lstrEmail;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter15);

        ////                IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
        ////                lobjParameter16.ParameterName = "@AuditUser";
        ////                lobjParameter16.DbType = DbType.String;
        ////                lobjParameter16.Value = lstrCreatedBy;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter16);

        ////                IDbDataParameter lobjParameter17 = DBFunction.GetDBParameter();
        ////                lobjParameter17.ParameterName = "@VipFlag";
        ////                lobjParameter17.DbType = DbType.Boolean;
        ////                lobjParameter17.Value = lboolVipFlag;
        ////                lcolParametersToUpdateHEDB.Add(lobjParameter17);

        ////                try
        ////                {
        ////                    lobjPassInfo1.BeginTransaction();
        ////                    DBFunction.DBExecuteProcedure("USP_PID_Person_UPD", lcolParametersToUpdateHEDB, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
        ////                    lobjPassInfo1.Commit();

        ////                }
        ////                catch (Exception e)
        ////                {
        ////                    lobjPassInfo1.Rollback();
        ////                    return lstrMessage = "Participant's RAP Information is not updated in HE because" + " " + e.InnerException.Message;

        ////                }
        ////                finally
        ////                {
        ////                    lobjPassInfo1.iconFramework.Close();
        ////                }

                   

        ////        }

        ////    }
        //    return lstrMessage;
        //}
        public void UpdateVIPStatusHistory(int intPersonId,string lstrmessage)
        {
            int lintCheckPersonExistInVIPStatus = (int)DBFunction.DBExecuteScalar("cdoVipStatusHistory.GetPersonFromVipStatusHistory", new object[1] { intPersonId },
                                                                                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lintCheckPersonExistInVIPStatus <= 0)
            {
                busVipStatusHistory lbusVipStatusHistory = new busVipStatusHistory { icdoVipStatusHistory = new cdoVipStatusHistory() };
                lbusVipStatusHistory.icdoVipStatusHistory.person_id = intPersonId;
                lbusVipStatusHistory.icdoVipStatusHistory.vip_flag_old_value = "Y";
                lbusVipStatusHistory.icdoVipStatusHistory.vip_flag_new_value = "N";
                lbusVipStatusHistory.icdoVipStatusHistory.message = lstrmessage;
                lbusVipStatusHistory.icdoVipStatusHistory.Insert();
            }
        }
    }
}





        

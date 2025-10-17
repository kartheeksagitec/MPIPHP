// -----------------------------------------------------------------------
// <copyright file="busDeathReportOutbound.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MPIPHP.BusinessObjects.Pension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sagitec.BusinessObjects;
    using System.Collections.ObjectModel;
    using System.Data;
    using Sagitec.DBUtility;
    using System.IO;
    using System.Collections;
    using Sagitec.Common;
    using System.Globalization;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using MPIPHP.CustomDataObjects;
    using Sagitec.ExceptionPub;
    using MPIPHP.DataObjects;
    using System.Text.RegularExpressions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class busDeathReportOutbound : busFileBaseOut
    {
        public Collection<busPerson> iclbActiveParticipant { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLock = null;
        DataTable ldtbWorkInformationForAllParticipants { get; set; }
        DataTable ldtbTempTableForAllParticipants { get; set; }

        #region OLDFILE
        /// <summary>
        /// These are kept as placeholders : Was converted to Reports
        /// </summary>
        public Collection<busDeathNotification> iclbDeathNotification { get; set; }
        busDeathNotification ibusDeathNotificationLast { get; set; }
        busDeathNotification ibusDeathNotificationFirst { get; set; }
        int iintIncorrectDeaths { get; set; }
        int iintRetrCount { get; set; }

        int iintPrevUnionCode;
        string istrPrevStatus = string.Empty;

        public void LoadDeathNotificationCollection(DataTable ldtbDeathNotification)
        {
            busBase lbusBase = new busBase();
            Collection<busDeathNotification> lclbDeathNotification = new Collection<busDeathNotification>();
            if (iclbDeathNotification == null)
            {
                iclbDeathNotification = new Collection<busDeathNotification>();
            }
            if (ldtbDeathNotification.Rows.Count > 0)
            {
                lclbDeathNotification = lbusBase.GetCollection<busDeathNotification>(ldtbDeathNotification, "icdoDeathNotification");
            }
            IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");
            if (lconLegacy != null)
            {
                foreach (busDeathNotification lbusDeathNotification in lclbDeathNotification)
                {
                    lbusDeathNotification.icdoDeathNotification.idecAge = busGlobalFunctions.CalculatePersonAge(lbusDeathNotification.icdoDeathNotification.idtDateOfBirth, DateTime.Now);

                    lbusDeathNotification.icdoDeathNotification.istrRetireeStatus = ReturnRetirementStatus(lbusDeathNotification.icdoDeathNotification.person_id);

                    if (lbusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_INCORRECTLY_REPORTED ||
                        lbusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_NOT_DECEASED)
                    {
                        iintIncorrectDeaths++;
                        lbusDeathNotification.icdoDeathNotification.istrStatus = busConstant.INC_REPORT_DEATH_REPORT;
                        lbusDeathNotification.icdoDeathNotification.istrDateofDeath = string.Empty;
                        if (lbusDeathNotification.icdoDeathNotification.date_of_death != DateTime.MinValue)
                        {
                            lbusDeathNotification.icdoDeathNotification.idtPreviousDateofDeath = lbusDeathNotification.icdoDeathNotification.date_of_death.ToString("d", CultureInfo.CreateSpecificCulture("en-US")); ;
                        }
                        else
                        {
                            lbusDeathNotification.icdoDeathNotification.idtPreviousDateofDeath = string.Empty;
                        }
                    }
                    else
                    {
                        lbusDeathNotification.icdoDeathNotification.istrStatus = busConstant.NEW_REPORT_DEATH_REPORT;
                        if (lbusDeathNotification.icdoDeathNotification.date_of_death != DateTime.MinValue)
                        {
                            lbusDeathNotification.icdoDeathNotification.istrDateofDeath = lbusDeathNotification.icdoDeathNotification.date_of_death.ToString("d", CultureInfo.CreateSpecificCulture("en-US")); ;
                            if (lbusDeathNotification.icdoDeathNotification.istrRetireeStatus == "R")
                            {
                                iintRetrCount++;
                            }
                        }
                    }


                    string lstrSSN = lbusDeathNotification.icdoDeathNotification.istrSSn;
                    if (!string.IsNullOrEmpty(lstrSSN))
                    {
                        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                        lobjParameter.ParameterName = "@SSN";
                        lobjParameter.DbType = DbType.String;
                        lobjParameter.Value = lstrSSN.ToLower();
                        lcolParameters.Add(lobjParameter);
                        IDataReader lDataRedaer = DBFunction.DBExecuteProcedureResult("sp_GetTrueUnions", lcolParameters, lconLegacy, null);
                        DataTable ldataTable = new DataTable();
                        if (lDataRedaer != null)
                        {
                            ldataTable.Load(lDataRedaer);
                            if (ldataTable.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(ldataTable.Rows[0][0])))
                                {
                                    lbusDeathNotification.icdoDeathNotification.iintUnionCode = Convert.ToInt32(ldataTable.Rows[0][0]);
                                }
                                lbusDeathNotification.icdoDeathNotification.istrUnionDescription = ldataTable.Rows[0][1].ToString();
                                if (lbusDeathNotification.icdoDeathNotification.istrUnionDescription.Contains(","))
                                {
                                    lbusDeathNotification.icdoDeathNotification.istrUnionDescription.Replace(",", " ");
                                }


                            }
                        }
                    }
                }
            }
            lconLegacy.Close();

            iclbDeathNotification = (from item in lclbDeathNotification orderby item.icdoDeathNotification.istrStatus, item.icdoDeathNotification.iintUnionCode select item).ToList().ToCollection();
            ibusDeathNotificationLast = iclbDeathNotification.Last();
            //ibusDeathNotificationFirst = iclbDeathNotification.First();
        }

        /*
        public override void BeforeWriteRecord()
        {
            if (iobjDetail != null)
            {
                int lintUnionCode = (iobjDetail as busDeathNotification).icdoDeathNotification.iintUnionCode;
                string istrStatus = (iobjDetail as busDeathNotification).icdoDeathNotification.istrStatus;
                if (istrPrevStatus != istrStatus)
                {
                    if (istrStatus == busConstant.NEW_REPORT_DEATH_REPORT)
                    {
                        string lstrIncCount = "Total Incorrectly Reported Death: " + iintIncorrectDeaths.ToString();
                        iswrOut.WriteLine(lstrIncCount);
                        iswrOut.WriteLine(string.Empty);
                    }
                    iswrOut.WriteLine(istrStatus);
                }
                if (lintUnionCode != iintPrevUnionCode)
                {
                    iswrOut.WriteLine(string.Empty);
                    string lstrUnionCode = (iobjDetail as busDeathNotification).icdoDeathNotification.istrUnionDescription + ",,,,,MPI Code:," + lintUnionCode.ToString();
                    iswrOut.WriteLine(lstrUnionCode);
                }

                iintPrevUnionCode = lintUnionCode;
                istrPrevStatus = istrStatus;
            }
            else
            {
                string lstrHeader =  ",,PENSION REPORTS OF DEATH,,,,";
                iswrOut.WriteLine(lstrHeader);

                iswrOut.WriteLine(string.Empty);

                string lstrDateString = ",," + GetDateString() +",,,,";
                iswrOut.WriteLine(lstrDateString);

                iswrOut.WriteLine(string.Empty);
            }
            base.BeforeWriteRecord();
            
        }
        
        public override void AfterWriteRecord()
        {
            if (iobjDetail == ibusDeathNotificationLast)
            {
                if (istrPrevStatus == busConstant.INC_REPORT_DEATH_REPORT)
                {
                    string lstrIncCount = "Total Incorrectly Reported Death: " + iintIncorrectDeaths.ToString();
                    iswrOut.WriteLine(lstrIncCount);
                }
                iswrOut.WriteLine(string.Empty);

                int lintNewDeaths = iclbDeathNotification.Count - iintIncorrectDeaths;
                string lstrNewCount = "Total Newly Reported Death: " + lintNewDeaths.ToString();
                iswrOut.WriteLine(lstrNewCount);

                int lintActive = lintNewDeaths - iintRetrCount;
                string lstrActive = "A(Active):" + lintActive.ToString();
                iswrOut.WriteLine(lstrActive);

                string lstrRetr = "R(Retiree):" + iintRetrCount.ToString();
                iswrOut.WriteLine(lstrRetr);

                iswrOut.WriteLine("TOTAL:" + lintNewDeaths.ToString());

            }
            base.AfterWriteRecord();
        }*/

        public string ReturnRetirementStatus(int aintPersonId)
        {
            string lstrRetireeStatus = "A";
            DataTable ldtbBenefitApplcation = busBase.Select("cdoDeathNotification.GetApprovedReirementorDisability", new object[1] { aintPersonId });
            if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    lstrRetireeStatus = "R";
                }
            }
            return lstrRetireeStatus;

        }

        public string GetDateString()
        {
            string lstrCurrentDate = DateTime.Now.ToString("MM/dd/yyyy");
            string lstrPrevMonthDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");

            return lstrCurrentDate + " - " + lstrPrevMonthDate;
        }
        #endregion

        public void LoadActiveParticipant(DataTable ldtbActiveParticipant)
        {
            ldtbActiveParticipant = (DataTable)iarrParameters[0];
            busBase lbusBase = new busBase();
            iclbActiveParticipant = lbusBase.GetCollection<busPerson>(ldtbActiveParticipant, "icdoPerson");

            //PIR 1078
            foreach (busPerson lbusPerson in iclbActiveParticipant)
            {
                lbusPerson.ibusPersonAddress = new busPersonAddress();
                // lbusPerson.ibusPersonAddress.LoadActiveAddress(lbusPerson.icdoPerson.person_id);

                lbusPerson.ibusPersonAddress.ibusMainParticipantAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                DataTable ldtbActiveAddress = busBase.Select("cdoPersonContact.GetActiveAddress", new object[1] { lbusPerson.icdoPerson.person_id });
                if (ldtbActiveAddress.Rows.Count > 0)
                {
                    if (ldtbActiveAddress.Rows.Count == 1)
                    {
                        lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.LoadData(ldtbActiveAddress.Rows[0]);
                    }
                    else
                    {
                        foreach (DataRow dtRow in ldtbActiveAddress.Rows)
                        {
                            if (dtRow[enmPersonAddressChklist.address_type_value.ToString()].ToString() == busConstant.MAILING_ADDRESS)
                            {
                                lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.LoadData(dtRow);
                                break;
                            }

                        }
                    }
                }


                lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 =
                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 + " " + lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2;

                Regex lrgx = null;
                if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1))
                {
                    lrgx = new Regex(",");
                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1, " ");
                }

                if (Convert.ToInt32(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_value) != busConstant.USA)
                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value = string.Empty;

                if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value))
                {
                    lrgx = new Regex(",");
                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value, " ");
                }

                if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city))
                {
                    lrgx = new Regex(",");
                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city, " ");
                }

                if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code))
                {
                    lrgx = new Regex(",");
                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code, "");
                }

                if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.home_phone_no))
                {
                    lrgx = new Regex("-");
                    lbusPerson.icdoPerson.home_phone_no = lrgx.Replace(lbusPerson.icdoPerson.home_phone_no, "");
                }

                if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.cell_phone_no))
                {
                    lrgx = new Regex("-");
                    lbusPerson.icdoPerson.home_phone_no = lrgx.Replace(lbusPerson.icdoPerson.cell_phone_no, "");
                }

                if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.first_name))
                {
                    lrgx = new Regex(",");
                    lbusPerson.icdoPerson.first_name = lrgx.Replace(lbusPerson.icdoPerson.first_name, "");
                }

                if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.middle_name))
                {
                    lrgx = new Regex(",");
                    lbusPerson.icdoPerson.middle_name = lrgx.Replace(lbusPerson.icdoPerson.middle_name, "");
                }

                if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.last_name))
                {
                    lrgx = new Regex(",");
                    lbusPerson.icdoPerson.last_name = lrgx.Replace(lbusPerson.icdoPerson.last_name, "");
                }

                if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.email_address_1))
                {
                    lrgx = new Regex(",");
                    lbusPerson.icdoPerson.email_address_1 = lrgx.Replace(lbusPerson.icdoPerson.email_address_1, "");
                }

            }

            #region Commented Code
            //busBase lbusBase = new busBase();
            //if (ldtbActiveParticipant.Rows.Count > 0)
            //{
            //    int lintCount = 0;
            //    int lintTotalCount = 0;

            //    Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            //    foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            //    {
            //        ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            //    }

            //    //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            //    iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            //    utlPassInfo lobjMainPassInfo = iobjPassInfo;

            //    busBase lobjBase = new busBase();
            //    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            //    string astrLegacyDBConnection = utlLegacyDBConnetion.istrConnectionString;

            //    SqlParameter[] lParameters = new SqlParameter[1];
            //    SqlParameter param1 = new SqlParameter("@Year", DbType.Int32);
            //    param1.Value = DateTime.Now.Year - 1;
            //    lParameters[0] = param1;

            //    iobjLock = new object();

            //    ldtbTempTableForAllParticipants = ldtbActiveParticipant.AsEnumerable().Where(o => o.Field<string>("istrEADBFlag") == "N").CopyToDataTable();

            //    ldtbWorkInformationForAllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkHistoryForActiveOutboundFile", astrLegacyDBConnection, lParameters);

            //    ParallelOptions po = new ParallelOptions();
            //    po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 1;

            //    Parallel.ForEach(ldtbActiveParticipant.AsEnumerable(), po, (acdoPerson, loopState) =>
            //    {
            //        utlPassInfo lobjPassInfo = new utlPassInfo();
            //        lobjPassInfo.idictParams = ldictParams;
            //        lobjPassInfo.idictParams["ID"] = "ActivePartOutboundFile";
            //        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
            //        utlPassInfo.iobjPassInfo = lobjPassInfo;

            //        ProcessCheckQualifiedYearORMPIAccouredBenefit(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

            //        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
            //        {
            //            lobjPassInfo.iconFramework.Close();
            //        }

            //        lobjPassInfo.iconFramework.Dispose();
            //        lobjPassInfo.iconFramework = null;

            //    });

            //    lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            //    utlPassInfo.iobjPassInfo = lobjMainPassInfo;

            //    if (utlPassInfo.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
            //    {
            //        utlPassInfo.iobjPassInfo.iconFramework.Open();
            //    }

            //    iclbActiveParticipant = lbusBase.GetCollection<busPerson>(ldtbTempTableForAllParticipants, "icdoPerson");
            // }
            #endregion
        }

        #region Process Check Qualified Year OR MPI Accoured Benefit
        public void ProcessCheckQualifiedYearORMPIAccouredBenefit(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            // There must some replacement for this in idict (the Connection String for Legacy)
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            autlPassInfo.BeginTransaction();

            try
            {
                if (ldtbWorkInformationForAllParticipants.Rows.Count > 0 && ldtbWorkInformationForAllParticipants.IsNotNull() && acdoPerson[enmPerson.ssn.ToString()] != DBNull.Value)
                {
                    string lstrSSN = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                    DataRow[] ldrTempWorkInfo = ldtbWorkInformationForAllParticipants.FilterTable(utlDataType.String, "ssn", lstrSSN);

                    if (!ldrTempWorkInfo.IsNullOrEmpty() && ldrTempWorkInfo.Where(item => item.Field<decimal>("QUALIFIED_HOURS") >= 400).Count() > 0)
                    {
                        DataRow dr = ldtbTempTableForAllParticipants.NewRow();
                        dr["MPI_PERSON_ID"] = acdoPerson[enmPerson.mpi_person_id.ToString()];
                        dr["FIRST_NAME"] = acdoPerson[enmPerson.first_name.ToString()];
                        dr["LAST_NAME"] = acdoPerson[enmPerson.last_name.ToString()];
                        dr["SSN"] = acdoPerson[enmPerson.ssn.ToString()];
                        dr["DATE_OF_BIRTH"] = acdoPerson[enmPerson.date_of_birth.ToString()];
                        dr["istrPersonType"] = "PARTICIPANT";
                        ldtbTempTableForAllParticipants.Rows.Add(dr);
                    }
                }
                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                }
                autlPassInfo.Rollback();

            }
        }
        #endregion

    }
}

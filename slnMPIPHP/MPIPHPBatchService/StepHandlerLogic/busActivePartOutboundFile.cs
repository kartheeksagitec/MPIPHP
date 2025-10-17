using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.ExceptionPub;
using MPIPHP.BusinessObjects;
using System.Threading.Tasks;
using Sagitec.Interface;

namespace MPIPHPJobService
{
    public class busActivePartOutboundFile : busBatchHandler
    {

        public Collection<busPerson> iclbActiveParticipant { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLock = null;
        DataTable ldtbWorkInformationForAllParticipants { get; set; }
        DataTable ldtbTempTableForAllParticipants { get; set; }

        public DataTable ProcessActiveParticipant()
        {
            DataTable ldtbActiveParticipant = new DataTable();
            ldtbActiveParticipant = busBase.Select("cdoPerson.LoadDeathOutboundFile", new object[] {});

            return ldtbActiveParticipant;

            #region Commnented Code. We dont really need to talk to EADB or DO a Parallel LOOP since we are going to RUN after BIS BATCH
            //busBase lbusBase = new busBase();
            //if (ldtbActiveParticipant.Rows.Count > 0)
            //{
                //int lintCount = 0;
                //int lintTotalCount = 0;

                //Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                //foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
                //{
                //    ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
                //}

                ////Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
                //iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
                //utlPassInfo lobjMainPassInfo = iobjPassInfo;

                //busBase lobjBase = new busBase();

                //iobjLock = new object();


                //utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                //string astrLegacyDBConnection = utlLegacyDBConnetion.istrConnectionString;

                //SqlParameter[] lParameters = new SqlParameter[1];
                //SqlParameter param1 = new SqlParameter("@Year", DbType.Int32);
                //param1.Value = DateTime.Now.Year - 1;
                //lParameters[0] = param1;

                //ldtbTempTableForAllParticipants = ldtbActiveParticipant.AsEnumerable().Where(o => o.Field<string>("istrEADBFlag") == "N").CopyToDataTable();

                //ldtbWorkInformationForAllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkHistoryForActiveOutboundFile", astrLegacyDBConnection, lParameters);
                

                //ParallelOptions po = new ParallelOptions();
                //po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 3;

                //Parallel.ForEach(ldtbActiveParticipant.AsEnumerable(), po, (acdoPerson, loopState) =>
                //{
                //    utlPassInfo lobjPassInfo = new utlPassInfo();
                //    lobjPassInfo.idictParams = ldictParams;
                //    lobjPassInfo.idictParams["ID"] = "ActivePartOutboundFile";
                //    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                //    utlPassInfo.iobjPassInfo = lobjPassInfo;

                //    ProcessCheckQualifiedYearORMPIAccouredBenefit(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

                //    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                //    {
                //        lobjPassInfo.iconFramework.Close();
                //    }

                //    lobjPassInfo.iconFramework.Dispose();
                //    lobjPassInfo.iconFramework = null;

                //});

                //lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                //utlPassInfo.iobjPassInfo = lobjMainPassInfo;

                //if (utlPassInfo.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                //{
                //    utlPassInfo.iobjPassInfo.iconFramework.Open();
                //}
                //iclbActiveParticipant = lbusBase.GetCollection<busPerson>(ldtbTempTableForAllParticipants, "icdoPerson");
            //}
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

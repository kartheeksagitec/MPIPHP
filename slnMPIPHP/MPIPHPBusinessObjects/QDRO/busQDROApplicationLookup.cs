#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
using Sagitec.Interface;
using MPIPHP.DataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{


    

    /// <summary>
	/// Class MPIPHP.BusinessObjects.busQDROApplicationLookup:
	/// Inherited from busQDROApplicationLookupGen, this class is used to customize the lookup business object busQDROApplicationLookupGen. 
	/// </summary>
    [Serializable]
    public class busQDROApplicationLookup : busQDROApplicationLookupGen
    {


        #region Properties
        private IDbConnection iobjDbConnection = null;
        #endregion

        /// <summary>
        /// MPIPHP.BusinessObjects.busQDROApplicationLookupGen.LoadQdroApplications(DataTable):
        /// Loads Collection object iclbQdroApplication of type busQdroApplication.
        /// </summary>
        /// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busQDROApplicationLookupGen.iclbQdroApplication</param>
        public virtual void LoadQdroApplications(DataTable adtbSearchResult)
        {
            iclbQdroApplication = GetCollection<busQdroApplication>(adtbSearchResult, "icdoDroApplication");
        }

        /// <summary>
        /// Load Other Objects will be called for each DataRow in the Datatable returned by the LookUp Query
        /// </summary>
        /// <param name="adtrRow"></param>
        /// <param name="aobjBus"></param>
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);

            //In the Following code I cannot use LoadData() method since I have 2 cdoPerson in the same DataRow
            //The method will not know what to bind with that
            //Hence we are getting only the necessary information in the LookUp Query
            busQdroApplication lbusQdroApplication = (busQdroApplication)aobjBus;
            lbusQdroApplication.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
            lbusQdroApplication.ibusAlternatePayee = new busPerson() { icdoPerson = new cdoPerson() };

            lbusQdroApplication.ibusParticipant.icdoPerson.first_name = adtrRow["ParticipantFirstName"].ToString();
            lbusQdroApplication.ibusParticipant.icdoPerson.last_name = adtrRow["ParticipantLastName"].ToString();
            lbusQdroApplication.ibusParticipant.icdoPerson.mpi_person_id = adtrRow["ParticipantMPID"].ToString();

            lbusQdroApplication.ibusAlternatePayee.icdoPerson.first_name = adtrRow["PayeeFirstName"].ToString();
            lbusQdroApplication.ibusAlternatePayee.icdoPerson.last_name = adtrRow["PayeeLastName"].ToString();
            lbusQdroApplication.ibusAlternatePayee.icdoPerson.mpi_person_id = adtrRow["PayeeMPID"].ToString();

        }


        public override ArrayList ValidateNew(Hashtable ahstParam)
        {

            ArrayList larrErrors = new ArrayList();
            busPerson lbusPerson = new busPerson();
            bool lblnFlag = false;
            string lstrMpiPersonId = Convert.ToString(ahstParam["astrPersonMPId"]).Trim();
            string lstrPayeeMPId = Convert.ToString(ahstParam["astrPayeeMPId"]).Trim();
            if (string.IsNullOrEmpty(lstrMpiPersonId))
            {
                utlError lobjError = null;
                lobjError = AddError(1102, "");
                larrErrors.Add(lobjError);
                lblnFlag = true;
            }
            else
            {
                if (iobjPassInfo.idictParams.ContainsKey("UserID"))
                {
                    string astrUserSerialID = iobjPassInfo.idictParams["UserID"].ToString();
                    DataTable ldtbParticipantId = busBase.Select("cdoPerson.GetVIPFlagInfo", new object[2] { astrUserSerialID, lstrMpiPersonId });
                    if (ldtbParticipantId.Rows.Count > 0)
                    {
                        if (ldtbParticipantId.Rows[0]["istr_IS_LOGGED_IN_USER_VIP"].ToString() == "N" && ldtbParticipantId.Rows[0]["istrRelativeVipFlag"].ToString() == "Y")
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6175, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }

                }
            }

            if (string.IsNullOrEmpty(lstrPayeeMPId))
            {
                utlError lobjError1 = null;
                lobjError1 = AddError(1103, "");
                larrErrors.Add(lobjError1);
                lblnFlag = true;
            }

            if (lblnFlag == true)
            {
                return larrErrors;
            }
            if (lstrMpiPersonId.Equals(lstrPayeeMPId))
            {
                utlError lobjError = null;
                lobjError = AddError(2008, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }

            DataTable ldtbPersonIDs = busBase.Select("cdoDroApplication.GetPersonID", new object[2] { lstrMpiPersonId, lstrPayeeMPId });
            if (ldtbPersonIDs.Rows.Count > 0)
            {
                if (ldtbPersonIDs.Rows.Count == 1)
                {
                    string lstrxyz = ldtbPersonIDs.Rows[0]["MPI_PERSON_ID"].ToString();
                    if (lstrxyz == lstrMpiPersonId)
                    {

                        utlError lobjError1 = null;
                        lobjError1 = AddError(2005, "");
                        larrErrors.Add(lobjError1);
                    }
                    else
                    {
                        utlError lobjError = null;
                        lobjError = AddError(2004, "");
                        larrErrors.Add(lobjError);

                    }

                }

                foreach (DataRow ldtrRow in ldtbPersonIDs.Rows)   //Rohan : Validation => Participant should have atleast one plan                                           
                {
                    if (ldtrRow["MPI_PERSON_ID"].ToString().ToLower() == lstrMpiPersonId.ToLower())
                    {
                        DataTable ldtbPersonaplans = busBase.Select("cdoPlan.GetPlanCount", new object[1] { Convert.ToInt32(ldtrRow["PERSON_ID"]) });
                        if (Convert.ToInt32(ldtbPersonaplans.Rows[0]["PlanCount"]) == 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(2013, "");
                            larrErrors.Add(lobjError);
                        }
                        break;
                    }
                }

                //PIR 341 fixed
                ////Abhishek - Code Added to Check For Duplicate DRO Entry at the same time
                //int lintCountForExistingNonCancelledDROsForPerson = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountforExistingNonCancelledDROsforPersonandPayee", new object[2] { lstrMpiPersonId, lstrPayeeMPId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                //if (lintCountForExistingNonCancelledDROsForPerson > 0)
                //{
                //    utlError lobjError = null;
                //    lobjError = AddError(2018, "");
                //    larrErrors.Add(lobjError);
                //}

                return larrErrors;
            }
            else
            {
                utlError lobjError = null;
                lobjError = AddError(2004, "");
                larrErrors.Add(lobjError);

                utlError lobjError1 = null;
                lobjError1 = AddError(2005, "");
                larrErrors.Add(lobjError1);

                return larrErrors;
            }    
        }

    }
}

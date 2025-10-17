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
using MPIPHP.DataObjects;
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busReturnToWorkRequestLookup:
    /// Inherited from busReturnToWorkRequestLookupGen, this class is used to customize the lookup business object busPayeeAccountLookupGen. 
    /// </summary>
    [Serializable]
	public class busReturnToWorkRequestLookup : busReturnToWorkRequestLookupGen
    {

        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);

            busReturnToWorkRequest lbusReturnToWork = (busReturnToWorkRequest)abusBase;
            lbusReturnToWork.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
            lbusReturnToWork.ibusPayee.icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
            lbusReturnToWork.ibusPayee.icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
            lbusReturnToWork.ibusPayee.icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
            lbusReturnToWork.ibusPayee.icdoPerson.mpi_person_id = adtrRow[busConstant.MPI_ID].ToString();
            lbusReturnToWork.ibusPayee.istrFullName = adtrRow["PayeeName"].ToString();

        }
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();

            string lstrMPIPersonId = Convert.ToString(ahstParam["aintMPIPersonId"]);
            string lstrSourceValue = Convert.ToString(ahstParam["aintSourceValue"]);
            string lstrRequestType = Convert.ToString(ahstParam["aintRequestTypeValue"]);

            if (lstrMPIPersonId.IsNullOrEmpty())
            {
                utlError lobjError = null;
                lobjError = AddError(7001, "");
                larrErrors.Add(lobjError);
                return larrErrors;

            }
            if (lstrSourceValue.IsNullOrEmpty() && lstrRequestType == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
            {
                utlError lobjError = null;
                lobjError = AddError(7002, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            if (lstrSourceValue == busConstant.ReturnToWorkRequest.SOURCE_BATCH)
            {
                utlError lobjError = null;
                lobjError = AddError(7010, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            if (lstrRequestType.IsNullOrEmpty())
            {
                utlError lobjError = null;
                lobjError = AddError(7015, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            DataTable ldtbPersonIDs = busBase.Select("entReturnToWorkRequest.GetPersonIdByMpiId", new object[1] { lstrMPIPersonId });
            if (ldtbPersonIDs.Rows.Count > 0)
            {
                var personIDs = ldtbPersonIDs.AsEnumerable()
                                             .Select(row => row["PERSON_ID"])
                                             .ToList();

                if (lstrRequestType == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
                {
                    DataTable ldtbPayeeAccountStatus = busBase.Select("entPayeeAccount.LoadPayeeAccountsByPersonID", new object[1] { personIDs[0] });

                    int matchingRowCount = ldtbPayeeAccountStatus.AsEnumerable()
                                                                  .Count(row =>
                                                                  new[] { busConstant.ReturnToWorkRequest.STATUS_RECEIVING }.Contains(row["STATUS_VALUE"].ToString().Trim().ToUpper()) &&
                                                                  row["BENEFIT_ACCOUNT_TYPE_VALUE"].ToString().Trim().ToUpper() == busConstant.BENEFIT_TYPE_RETIREMENT);

                    if (matchingRowCount <= 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(7003, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }
                //Active process already exists for person.
                if (Convert.ToInt32(personIDs[0]) != 0)
                {
                    Collection<IDbDataParameter> lclbParameters = new Collection<IDbDataParameter>();
                    lclbParameters.Add(DBFunction.GetDBParameter("@PERSON_ID", "int", personIDs[0], iobjPassInfo.iconFramework));
                    if (lstrRequestType == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
                    {
                        lclbParameters.Add(DBFunction.GetDBParameter("@CASE_NAME", "string", busConstant.ReturnToWorkRequest.MAP_RETURN_TO_WORK, iobjPassInfo.iconFramework));
                    }
                    else if (lstrRequestType == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
                    {
                        lclbParameters.Add(DBFunction.GetDBParameter("@CASE_NAME", "string", busConstant.ReturnToWorkRequest.MAP_END_RETURN_TO_WORK, iobjPassInfo.iconFramework));
                    }

                    string lstrActiveInProgressInstanceQuery = "entFramework.CountActiveProcessForPersonByCaseName";

                    object lobjActiveWfCount = DBFunction.DBExecuteScalar(lstrActiveInProgressInstanceQuery, lclbParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    if (lobjActiveWfCount != null)
                    {
                        if (Convert.ToInt32(lobjActiveWfCount) > 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(7004, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }
                }
            }
            else
            {
                utlError lobjError = null;
                lobjError = AddError(1600, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            return larrErrors;
        }
    }
}

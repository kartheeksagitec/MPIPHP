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
using System.Data.Sql;
using System.Data.SqlClient;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPersonLookup:
    /// Inherited from busPersonLookupGen, this class is used to customize the lookup business object busPersonLookupGen. 
    /// </summary>
    [Serializable]
    public class busPersonLookup : busPersonLookupGen
    {
        IDbConnection lconLegacy = null;

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);

            string astrSSN = (aobjBus as busPerson).icdoPerson.istrSSNNonEncrypted;
            if (!String.IsNullOrEmpty(astrSSN))
            {
                busPerson lbusPerson = new busPerson();

                if (iobjPassInfo.idictParams["FormName"].ToString() == busConstant.SSN_MERGE_LOOKUP)
                {
                    string lstrEmployerName = string.Empty;
                    int lintUnionCode = 0;
                    lbusPerson.GetEmployerNameBySSN(astrSSN, ref lstrEmployerName, ref lintUnionCode);

                    (aobjBus as busPerson).icdoPerson.istrEmployerName = lstrEmployerName;
                    (aobjBus as busPerson).icdoPerson.UnionCode = Convert.ToString(lintUnionCode);
                    (aobjBus as busPerson).icdoPerson.istrParticipantAddress = lbusPerson.GetMailingAddress((aobjBus as busPerson).icdoPerson.person_id);
                }
                else if (iobjPassInfo.idictParams["FormName"].ToString() == busConstant.Person_Lookup_Mail_Return)
                {
                    lbusPerson = (aobjBus as busPerson);
                    lbusPerson.ibusPersonAddress = new busPersonAddress() { icdoPersonAddress = new cdoPersonAddress() };
                    lbusPerson.ibusPersonAddress.icdoPersonAddress.LoadData(adtrRow);
                }
                else
                {
                    (aobjBus as busPerson).icdoPerson.UnionCode = lbusPerson.GetTrueUnionCodeBySSN(astrSSN);
                }

            }
            
        }

    }
}

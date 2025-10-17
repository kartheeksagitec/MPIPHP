#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.Common;
using Sagitec.Bpm;
using System.Linq;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busSolBpmProcessEscalation:
    /// Inherited from busBpmProcessEscalation, the class is used to customize the business object busBpmProcessEscalation.
    /// </summary>
    [Serializable]
    public class busNeobaseBpmProcessEscalation : busBpmProcessEscalation
    {
        /// <summary>
        /// Property to hold the User Id Details.
        /// </summary>
        public int iintUserId { get; set; }
        /// <summary>
        /// Property to hold the Role Id details.
        /// </summary>
        public int iintRoleId { get; set; }
        /// <summary>
        /// Property to hold the Lapse type code Id details.
        /// </summary>
        private int iintLapseTypeCodeId { get; set; }

        /// <summary>
        /// This function is used to get the collection of Lapse Code values.
        /// </summary>
        /// <returns></returns>
        public Collection<cdoCodeValue> GetLapseCodeValues()
        {
            Collection<cdoCodeValue> lclcLapseCodeValues = new Collection<cdoCodeValue>();
            if (icdoBpmProcessEscalation == null)
            {
                icdoBpmProcessEscalation = new doBpmProcessEscalation();
            }
            if (iintLapseTypeCodeId == 0)
            {
                XmlObject lobjXmlObject = iobjPassInfo.isrvMetaDataCache.GetXmlObject("entBpmProcessEscalation");
                XmlEntityObject lobjCdoProcessEscalationXmlObject = lobjXmlObject as XmlEntityObject;
                if (lobjCdoProcessEscalationXmlObject != null)
                {
                    XmlObject lobjXmlColumns = lobjCdoProcessEscalationXmlObject.icolChildObjects.Where(childObject => childObject.istrElementName.ToLower() == "attributes").FirstOrDefault();

                    XmlObject lobjXmlLapseTypeIdColumn = null;
                    foreach (XmlObject column in lobjXmlColumns.icolChildObjects)
                    {
                        if (column.idictAttributes.ContainsKey("sfwValue") && column.idictAttributes["sfwValue"].ToLower() == "lapse_type_id")
                        {
                            lobjXmlLapseTypeIdColumn = column;
                            break;
                        }
                    }
                    if (lobjXmlLapseTypeIdColumn != null)
                    {
                        if(lobjXmlLapseTypeIdColumn.idictAttributes.ContainsKey("sfwCodeID") && !string.IsNullOrEmpty(lobjXmlLapseTypeIdColumn.idictAttributes["sfwCodeID"]))
                        {
                            iintLapseTypeCodeId = int.Parse(lobjXmlLapseTypeIdColumn.idictAttributes["sfwCodeID"]);
                        }
                    }
                }
            }
            DataTable ldtbLapseCodeValues = iobjPassInfo.isrvDBCache.GetCodeValues(iintLapseTypeCodeId);
            foreach (DataRow ldtrRow in ldtbLapseCodeValues.Rows)
            {
                if (ldtrRow["CODE_VALUE"].ToString() != BpmEscalationLapseTypes.ActivityInstanceUnAssigned)
                {
                    cdoCodeValue lcdoCodeValue = new cdoCodeValue();
                    lcdoCodeValue.LoadData(ldtrRow);
                    lclcLapseCodeValues.Add(lcdoCodeValue);
                }
            }
            return lclcLapseCodeValues;
        }

        /// <summary>
        /// This function is used to load Bpm Process after save finish.
        /// </summary>
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadBpmCase();
            LoadBpmProcess();
        }

        /// <summary>
        /// This function is used to Display the Hard Error.
        /// </summary>
        public override bool HasOneRecipient()
        {
            return iclbBpmProcessEscalationRecipient.Count > 0;
        }
    }
}

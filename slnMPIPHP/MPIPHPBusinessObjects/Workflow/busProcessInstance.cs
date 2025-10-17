#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
using MPIPHP.Common;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busProcessInstance:
    /// Inherited from busProcessInstanceGen, the class is used to customize the business object busProcessInstanceGen.
    /// </summary>
    [Serializable]
    public class busProcessInstance : busProcessInstanceGen
    {
        public long iintReferenceID { get; set; }
        public string istrMPID { get; set; }
        public cdoActivityInstance icdoPrevActivityInstance { get; set; }
        public utlCollection<cdoProcessInstanceParameters> iutlProcessInstanceParams { get; set; }

        public bool FindProcessInstanceByContactTicket(int aintcontactticketid)
        {
            bool lblnResult = false;
            if (icdoProcessInstance == null)
            {
                icdoProcessInstance = new cdoProcessInstance();
            }
            DataTable ldtbProcessInstance = Select<cdoProcessInstance>(new string[1] { "contact_ticket_id" },
                  new object[1] { aintcontactticketid }, null, null);
            if (ldtbProcessInstance.Rows.Count > 0)
            {
                icdoProcessInstance.LoadData(ldtbProcessInstance.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadProcessInstanceParameters()
        {
            ArrayList larrParameters = iobjPassInfo.isrvMetaDataCache.GetProcessInstanceParams(ibusProcess.icdoProcess.name);
            DataTable ldtbProcessInstParams = DBFunction.DBSelect("cdoProcessInstanceParameters.GetParameters", new object[1] { icdoProcessInstance.process_instance_id },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iutlProcessInstanceParams = doBase.GetCollection<cdoProcessInstanceParameters>(ldtbProcessInstParams);
        }
        //Kunal : Fixes For workflow
        public void UpdateProcessInstanceParameter(string astrParamName, string astrParamValue)
        {
            //if (iutlProcessInstanceParams == null)
                LoadProcessInstanceParameters();

            bool lblnFound = false;
            foreach (cdoProcessInstanceParameters lcdoProcessInstParam in iutlProcessInstanceParams)
            {
                if (lcdoProcessInstParam.parameter_name == astrParamName)
                {
                    lblnFound = true;
                    if (astrParamValue.IsNotNullOrEmpty())
                    {
                        lcdoProcessInstParam.parameter_value = astrParamValue;
                        lcdoProcessInstParam.Update();
                    }
                    break;
                }
            }
            if (!lblnFound)
            {
                if (astrParamValue.IsNotNullOrEmpty())
                {
                    cdoProcessInstanceParameters lcdoProcesInstParam = new cdoProcessInstanceParameters();
                    lcdoProcesInstParam.parameter_name = astrParamName;
                    lcdoProcesInstParam.parameter_value = astrParamValue;
                    lcdoProcesInstParam.process_instance_id = icdoProcessInstance.process_instance_id;
                    lcdoProcesInstParam.Insert();
                }
            }
        }

        public string GetParameterValue(string astrParamName)
        {
            return (string)DBFunction.DBExecuteScalar("cdoProcessInstanceParameters.GetParameter", new object[2] { icdoProcessInstance.process_instance_id, astrParamName },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }


        public void AddRequestParameters()
        {
            if (ibusWorkflowRequest == null)
            {
                LoadWorkflowRequest();
            }

            ibusWorkflowRequest.LoadWorkflowRequestParameters();
            foreach (busRequestParameter lbusRequestParameter in ibusWorkflowRequest.iclbRequestParameter)
            {
                cdoProcessInstanceParameters lcdoProcessInstanceParameters = new cdoProcessInstanceParameters();
                lcdoProcessInstanceParameters.process_instance_id = this.icdoProcessInstance.process_instance_id;
                lcdoProcessInstanceParameters.parameter_name = lbusRequestParameter.icdoRequestParameter.parameter_name;
                lcdoProcessInstanceParameters.parameter_value = lbusRequestParameter.icdoRequestParameter.parameter_value;
                lcdoProcessInstanceParameters.Insert();
            }
        }
    }
}

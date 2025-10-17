#region Using directives

using NeoBase.BPM;
using NeoBase.BPMDataObjects;
using NeoSpin.BusinessObjects;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.Bpm.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;

#endregion

namespace NeoSpin.BusinessTier
{
    /// <summary>
    /// Jagruti Bachhav - moved methods of BPM from srvCommon into srvCommonBPM file.
    /// Developer : Tanaji Biradar
    /// Release : Iteration-8
    /// Date : 23rd December 2020
    /// Comments : Applied new AppBase-App changes for Service classes.
    /// </summary>
    public partial class srvABCommon
    {

        //TODO: Check if can be removed
        public Collection<busBpmRequestParameter> LoadBpmProcessParams(int aintProcessID)
        {
            Collection<busBpmRequestParameter> lclbResult = new Collection<busBpmRequestParameter>();
            busBpmProcess lbusBpmProcess = new busBpmProcess();
            if (lbusBpmProcess.FindByPrimaryKey(aintProcessID))
            {
                lbusBpmProcess.ibusBpmCase = busBpmCase.GetBpmCase(lbusBpmProcess.icdoBpmProcess.case_id);
                foreach (busBpmCaseParameter lbusBpmCaseInstanceParameter in lbusBpmProcess.ibusBpmCase.iclbBpmCaseParameter)
                {
                    busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = lbusBpmCaseInstanceParameter.icdoBpmCaseParameter.parameter_name;
                    lclbResult.Add(lbusBpmRequestParameter);
                }
            }
            return lclbResult;
        }

        public busBpmProcess FindBpmProcess(int aintProcessId)
        {
            busBpmProcess lbusBpmProcess = new busBpmProcess();
            busBpmCase lbusBpmCase = null;
            if (lbusBpmProcess.FindByPrimaryKey(aintProcessId))
            {
                lbusBpmCase = busBpmCase.GetBpmCase(lbusBpmProcess.icdoBpmProcess.case_id);
            }
            if (lbusBpmCase != null)
            {
                lbusBpmProcess = lbusBpmCase.GetProcess(aintProcessId);
                // Developer : Vishakha Sancheti
                // Release : Iteration-13.1
                // Date : 04 JULY 2023
                // PIR : 4197 
                // Change Sugested by Framework team Framework PIR - 51749.
                // Comment : We are clearing the grid hash to maintain the changes in grid and stop it from getting disappear.
                if (lbusBpmProcess.idictGridHash != null)
                {
                    lbusBpmProcess.idictGridHash.Clear();
                }
                lbusBpmProcess.LoadRestrictNotifyConfigurationForParentProcess(lbusBpmProcess.icdoBpmProcess.process_id);
                lbusBpmProcess.iclbParentProcessRestrictNotify.ForEach(processRestrictNotifyXr => processRestrictNotifyXr.FindDependentProcess());

                // Developer : Rahul Mane
                // PIR : 1846 
                // Change Sugested by Framework team Framework PIR - 22270.
                // below line is to load the DependentCase with the newly added method FindCaseOfDependentProcess.
                lbusBpmProcess.iclbParentProcessRestrictNotify.ForEach(processRestrictNotifyXr => processRestrictNotifyXr.FindCaseOfDependentProcess());
                lbusBpmProcess.LoadBpmProcessEventXrs();
                lbusBpmProcess.iclbBpmProcessEventXr.ForEach(x => x.ibusBpmProcess = lbusBpmProcess);
                lbusBpmProcess.iclbBpmProcessEventXr.ForEach(x => x.LoadBpmEvent());
                return lbusBpmProcess;
            }
            return null;
        }

        /// <summary>
        /// Loads Bpm Case For Maintenance.
        /// </summary>
        /// <param name="aintCaseId"></param>
        /// <returns></returns>
        public busBpmCase FindBpmCase(int aintCaseId)
        {
            busBpmCase lbusBpmCase =  busBpmCase.GetBpmCase(aintCaseId);
            lbusBpmCase.GetCaseInstancesCountGroppedByStatus();
            return lbusBpmCase;
        }

        public busBpmCaseInstance FindBpmCaseInstanceForExecution(int aintCaseInstanceId)
        {
            busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance();
            if (lbusBpmCaseInstance.FindByPrimaryKey(aintCaseInstanceId))
            {
                lbusBpmCaseInstance.ibusBpmCase = busBpmCase.GetBpmCase(lbusBpmCaseInstance.icdoBpmCaseInstance.case_id);
                if (lbusBpmCaseInstance.ibusBpmCase != null)
                {
                    foreach (busBpmProcess lbusBpmProcess in lbusBpmCaseInstance.ibusBpmCase.iclbBpmProcess)
                    {
                        lbusBpmProcess.iclbBpmActivity.ForEach(bpmActivity => bpmActivity.LoadRoles());
                    }
                }

                lbusBpmCaseInstance.LoadBpmCaseInstanceParameters();
                lbusBpmCaseInstance.LoadBpmProcessInstances();
                lbusBpmCaseInstance.LoadBpmRequest();
                lbusBpmCaseInstance.LoadPerson();
                lbusBpmCaseInstance.LoadOrganization();
                lbusBpmCaseInstance.LoadBpmBpmCaseInstanceExecutionPath();
            }
            return lbusBpmCaseInstance;
        }
        /// <summary>
        /// Finds BPM Case from Case Id
        /// </summary>
        /// <param name="aintProcessId"></param>
        /// <returns></returns>
        public busBpmCase FindBpmCaseToRenderMap(int aintCaseId)
        {
            return busBpmCase.GetBpmCase(aintCaseId);
        }

        ///// <summary>
        ///// Download BPM Uploaded Documents
        ///// </summary>
        ///// <param name="aobjMainObject"></param>
        ///// <param name="aintBpmDocumentUploadId"></param>
        ///// <returns></returns>
        public ArrayList DownloadBPMUploadedDocument(busBase aobjMainObject, int aintBpmDocumentUploadId = 0)
        {
            ArrayList larrResult = new ArrayList();

            busBpmDocumentUpload lbusBpmDocumentUpload = new busBpmDocumentUpload() { icdoBpmDocumentUpload = new Sagitec.Bpm.doBpmDocumentUpload() };
            if (lbusBpmDocumentUpload.FindByPrimaryKey(aintBpmDocumentUploadId))
            {
                string lstrBpmDocumentName = lbusBpmDocumentUpload.icdoBpmDocumentUpload.bpm_document_name;
                string lstrFullFileName = Path.Combine(utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("BPM_UPLD"), Convert.ToString(lbusBpmDocumentUpload.icdoBpmDocumentUpload.bpm_process_instance_id), lstrBpmDocumentName);
                if (string.IsNullOrEmpty(lstrFullFileName))
                {
                    byte[] larrFileContent = null;
                    FileInfo lfioFileInfo = new FileInfo(lstrFullFileName);
                    using (FileStream lobjFileStream = lfioFileInfo.OpenRead())
                    {
                        larrFileContent = new byte[lobjFileStream.Length];
                        lobjFileStream.Read(larrFileContent, 0, (int)lobjFileStream.Length);
                    }
                    larrResult.Add(lstrBpmDocumentName);
                    larrResult.Add(larrFileContent);
                    larrResult.Add(busNeoSpinBase.DeriveMimeTypeFromFileName(lstrBpmDocumentName));
                }
            }
            return larrResult;
        }
    }
}
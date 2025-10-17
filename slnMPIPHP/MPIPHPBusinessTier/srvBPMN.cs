using MPIPHP.BusinessTier;
using NeoBase.BPM;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;
using Sagitec.Common;
using System.Data;

namespace NeoSpin.BusinessTier
{
    public class srvABBPMN : srvMPIPHP
    {
        public busSolBpmCaseInstance LoadBpmCaseInstances(DataTable adtbSearchResult)
        {
            busSolBpmCaseInstance lobjSolBpmCaseInstance = new busSolBpmCaseInstance();
            lobjSolBpmCaseInstance.LoadBpmCaseInstances(adtbSearchResult);
            return lobjSolBpmCaseInstance;
        }
    }

    /// <summary>
    /// Developer : Tanaji Biradar
    /// Release : Iteration-8
    /// Date : 23rd December 2020
    /// Comments : Applied new AppBase-App changes for Service classes.
    /// </summary>
    public class srvBPMN : srvABBPMN
    {
        public busBpmCase FindBpmCase(int aintCaseId)
        {
            busBpmCase lbusBpmCase = busBpmCase.GetBpmCase(aintCaseId);
            lbusBpmCase.GetCaseInstancesCountGroppedByStatus();
            return lbusBpmCase;
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
        public busSolBpmActivityInstance BPMGetUserActivities(int aintActivityInstanceID)
        {
            busSolBpmActivityInstance lbusBpmActivityInstance = new busSolBpmActivityInstance();
            lbusBpmActivityInstance.LoadCenterleftObjects(aintActivityInstanceID);
            if (lbusBpmActivityInstance.icdoBpmActivityInstance != null && lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id > 0)
            {
                lbusBpmActivityInstance.LoadBpmActivity();
                lbusBpmActivityInstance.ibusBpmActivity.LoadBpmProcess();
                lbusBpmActivityInstance.LoadBpmProcessInstance();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadBpmProcess();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadBpmCaseInstance();
                lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadBpmCase();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadPerson();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadOrganization();
                //lbusBpmActivityInstance.LoadBpmActivityInstanceChecklist();
                lbusBpmActivityInstance.LoadProcessInstanceNotes();
                lbusBpmActivityInstance.LoadDocumentUpload();
                lbusBpmActivityInstance.EvaluateInitialLoadRules();

            }
            return lbusBpmActivityInstance;
        }
    }
}

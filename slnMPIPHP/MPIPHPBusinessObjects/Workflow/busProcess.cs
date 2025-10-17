#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busProcess:
    /// Inherited from busProcessGen, the class is used to customize the business object busProcessGen.
    /// </summary>
    [Serializable]
    public class busProcess : busProcessGen
    {
        //This Hash Table used in NeoFlowActivities
        public Hashtable ihstActivities { get; set; }
        public Collection<busActivity> iclbActivity { get; set; }

        public override bool FindProcess(int Aintprocessid)
        {
            
            if (base.FindProcess(Aintprocessid))
            {
                icdoProcess.istrWorkflowFullPath = MPIPHP.Common.ApplicationSettings.Instance.NeoFlowMapPath + icdoProcess.name + ".xaml";
                return true;
            }
            return false;
        }

        public void LoadActivityList()
        {
            DataTable ldtbActivity = Select<cdoActivity>(new string[1] { "process_id" }, new object[1] { icdoProcess.process_id }, null, "sort_order,activity_id");
            ihstActivities = new Hashtable();
            iclbActivity = new Collection<busActivity>();
            foreach (DataRow ldtrRow in ldtbActivity.Rows)
            {
                busActivity lobjActivity = new busActivity();
                lobjActivity.icdoActivity = new cdoActivity();
                lobjActivity.icdoActivity.LoadData(ldtrRow);
                lobjActivity.ibusProcess = this;
				//PIR - 2105
                lobjActivity.LoadRoles();
                //Load the screen name & fous control ID
                lobjActivity.GetDetails();
                ihstActivities.Add(lobjActivity.icdoActivity.name.Trim(), lobjActivity);
                iclbActivity.Add(lobjActivity);
            }
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busDocumentProcessCrossref)
            {
                busDocumentProcessCrossref lbusDocumentProcessCrossRef = (busDocumentProcessCrossref)aobjBus;
                lbusDocumentProcessCrossRef.LoadDocument();
            }
            base.LoadOtherObjects(adtrRow, aobjBus);
        }
    }
}

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
    [Serializable]
    public class busProcessInstanceChecklistGen : busMPIPHPBase
    {
        public busProcessInstanceChecklistGen()
        {

        }

        private cdoProcessInstanceChecklist _icdoProcessInstanceChecklist;
        public cdoProcessInstanceChecklist icdoProcessInstanceChecklist
        {
            get
            {
                return _icdoProcessInstanceChecklist;
            }
            set
            {
                _icdoProcessInstanceChecklist = value;
            }
        }

        public bool FindProcessInstanceChecklist(int Aintprocessinstancechecklistid)
        {
            bool lblnResult = false;
            if (_icdoProcessInstanceChecklist == null)
            {
                _icdoProcessInstanceChecklist = new cdoProcessInstanceChecklist();
            }
            if (_icdoProcessInstanceChecklist.SelectRow(new object[1] { Aintprocessinstancechecklistid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindProcessInstanceChecklistByInstanceAndDocument(int aintProcessInstanceID, int aintDocumentID)
        {
            if (_icdoProcessInstanceChecklist == null)
            {
                _icdoProcessInstanceChecklist = new cdoProcessInstanceChecklist();
            }
            DataTable ldtbProcessInstanceChecklist = Select<cdoProcessInstanceChecklist>(new string[2] { "process_instance_id", "document_id" },
                  new object[2] { aintProcessInstanceID, aintDocumentID }, null, null);
            if (ldtbProcessInstanceChecklist.Rows.Count > 0)
            {
                _icdoProcessInstanceChecklist.LoadData(ldtbProcessInstanceChecklist.Rows[0]);
                return true;
            }
            return false;
        }
    }
}

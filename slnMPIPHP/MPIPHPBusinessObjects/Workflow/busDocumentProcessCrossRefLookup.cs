#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.Bpm;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busDocumentProcessCrossRefLookup : busDocumentProcessCrossRefLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busDocumentProcessCrossref)
            {
                busDocumentProcessCrossref lbusDocumentProcessCrossref = (busDocumentProcessCrossref) aobjBus;

                lbusDocumentProcessCrossref.ibusDocument = new busDocument();
                lbusDocumentProcessCrossref.ibusDocument.icdoDocument = new cdoDocument();
                lbusDocumentProcessCrossref.ibusDocument.icdoDocument.LoadData(adtrRow);
                
                lbusDocumentProcessCrossref.ibusBpmProcess = new busBpmProcess();
                lbusDocumentProcessCrossref.ibusBpmProcess.icdoBpmProcess = new doBpmProcess();
                lbusDocumentProcessCrossref.ibusBpmProcess.icdoBpmProcess.LoadData(adtrRow);
            }
        }
	}
}

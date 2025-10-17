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
	/// Class MPIPHP.BusinessObjects.busNotes:
	/// Inherited from busNotesGen, the class is used to customize the business object busNotesGen.
	/// </summary>
	[Serializable]
	public class busNotes : busNotesGen
	{
        public void InsertNotes(int abusNewPersonId, string astrformvalue, string astrnotes, int aintorgid = 0)
        {
            icdoNotes = new cdoNotes();
            icdoNotes.person_id = abusNewPersonId;
            icdoNotes.org_id=aintorgid;
            icdoNotes.form_id = busConstant.Form_ID;
            icdoNotes.form_value = astrformvalue;
            icdoNotes.notes = astrnotes;
            icdoNotes.Insert();
     
        }
	}
}

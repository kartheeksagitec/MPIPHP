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
	/// Class MPIPHP.BusinessObjects.busDataExtractionBatchInfo:
	/// Inherited from busDataExtractionBatchInfoGen, the class is used to customize the business object busDataExtractionBatchInfoGen.
	/// </summary>
	[Serializable]
	public class busDataExtractionBatchInfo : busDataExtractionBatchInfoGen
	{
        public string istrLast4DigitsofSSN { get; set; }
	}
}

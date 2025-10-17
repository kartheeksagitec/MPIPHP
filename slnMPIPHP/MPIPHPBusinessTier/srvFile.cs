#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.BusinessTier
{
	public class srvFile : srvNeoSpin
	{
		public srvFile()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public busFile FindFileTemplate(int ainttemplateid)
		{
			busFile lobjFileTemplates = new busFile();
			if (lobjFileTemplates.FindFile(ainttemplateid))
			{
			}

			return lobjFileTemplates;
		}


		public busFileLookup LoadFileTemplates(DataTable adtbSearchResult)
		{
			busFileLookup lobjFileTemplatesLookup = new busFileLookup();
			lobjFileTemplatesLookup.LoadFiles(adtbSearchResult);
			return lobjFileTemplatesLookup;
		}

		public busFileLookup LoadFileTracking(DataTable adtbSearchResult)
		{
			busFileLookup lobjFileTrackingLookup = new busFileLookup();
			lobjFileTrackingLookup.LoadFiles(adtbSearchResult);
			return lobjFileTrackingLookup;
		}

	}
}

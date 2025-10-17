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
	/// Class MPIPHP.BusinessObjects.busPirHistory:
	/// Inherited from busPirHistoryGen, the class is used to customize the business object busPirHistoryGen.
	/// </summary>
	[Serializable]
	public class busPirHistory : busPirHistoryGen
	{
        public busPirHistory()
        {
        }

        public cdoPirHistory icdoPirHistory {get;set;}
        public busUser ibusAssignedTo {get;set;}

        public void LoadAssignedTo()
        {
            if (ibusAssignedTo == null)
            {
                ibusAssignedTo = new busUser();
            }
            ibusAssignedTo.FindUser(icdoPirHistory.assigned_to_id);
        }     
	}
}

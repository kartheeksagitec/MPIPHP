using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busIapAllocationDetailCalculation :busBase
    {

        public busIapAllocationDetailCalculation()
		{

		}

        public cdoIapallocationDetailPersonoverview icdoIapallocationDetailPersonoverview { get; set; }
    }
}

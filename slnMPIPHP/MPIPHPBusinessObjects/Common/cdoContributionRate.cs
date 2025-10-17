#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
  [Serializable]
  public class cdoContributionRate : doContributionRate
	{
      public cdoContributionRate() : base() 
      { 

      } 

  } 
} 

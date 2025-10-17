#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
  [Serializable]
  public class cdoSystemManagement : doSystemManagement
	{
      public cdoSystemManagement() : base() 
      { 

      } 

  } 
} 

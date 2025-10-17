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
  public class cdoBatchSchedule : doBatchSchedule
	{
      public cdoBatchSchedule() : base() 
      { 

      }

      public string istrShowStep
      {
          get
          {
              return step_no.ToString() + " - " + step_name;
          }
      }

  } 
} 

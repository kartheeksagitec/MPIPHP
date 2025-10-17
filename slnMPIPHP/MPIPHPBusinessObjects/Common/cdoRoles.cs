#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
  [Serializable]
  public class cdoRoles : doRoles
	{
      public cdoRoles() : base() 
      { 

      }
	  public override int Insert()
	  {
		  int lintResult = base.Insert();
          DBFunction.DBNonQuery("cdoSecurity.InitializeSecurity", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
          return lintResult;
	  }
  } 
} 

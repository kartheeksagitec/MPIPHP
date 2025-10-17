#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
#endregion

namespace MPIPHP.DataObjects
{
[Serializable]
	public class doContributionRate : doBase
	{
		public doContributionRate() : base()
      {
      }
      private int _rate_id;
      public int rate_id
      {
	        get
	        {
		    return _rate_id;
	        }

	        set
	        {
		        _rate_id = value;
	        }
      }
      private int _plan_id;
      public int plan_id
      {
	        get
	        {
		    return _plan_id;
	        }

	        set
	        {
		        _plan_id = value;
	        }
      }
      private int _org_id;
      public int org_id
      {
	        get
	        {
		    return _org_id;
	        }

	        set
	        {
		        _org_id = value;
	        }
      }
      private DateTime _effective_date;
      public DateTime effective_date
      {
	        get
	        {
		    return _effective_date;
	        }

	        set
	        {
		        _effective_date = value;
	        }
      }
      private double _employer_pct;
      public double employer_pct
      {
	        get
	        {
		    return _employer_pct;
	        }

	        set
	        {
		        _employer_pct = value;
	        }
      }
      private double _member_pct;
      public double member_pct
      {
	        get
	        {
		    return _member_pct;
	        }

	        set
	        {
		        _member_pct = value;
	        }
      }
      private string _description;
      public string description
      {
	        get
	        {
		    return _description;
	        }

	        set
	        {
		        _description = value;
	        }
      }
  }
}

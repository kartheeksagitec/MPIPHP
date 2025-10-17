#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.DataObjects
{
	/// <summary>
	/// Class MPIPHP.DataObjects.doMonthlyBenifitSummary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMonthlyBenifitSummary : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doMonthlyBenifitSummary() : base()
         {
         }
         public int monthly_benefit_summary_id { get; set; }
         public int payment_schedule_id { get; set; }
         public Decimal mpipp_amount { get; set; }
         public Decimal l52_amount { get; set; }
         public Decimal l161_amount { get; set; }
         public Decimal l600_amount { get; set; }
         public Decimal l666_amount { get; set; }
         public Decimal l700_amount { get; set; }
    }
    [Serializable]
    public enum enmMonthlyBenifitSummary
    {
         monthly_benefit_summary_id ,
         payment_schedule_id ,
         mpipp_amount ,
         l52_amount ,
         l161_amount ,
         l600_amount ,
         l666_amount ,
         l700_amount ,
    }
}


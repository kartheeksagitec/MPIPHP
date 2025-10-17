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
    /// Class MPIPHP.BusinessObjects.busReturnedMailGen:
    /// Inherited from busBase, used to create new business object for main table cdoReturnedMail and its children table. 
    /// </summary>
	[Serializable]
	public class busReturnedMailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busReturnedMailGen
        /// </summary>
		public busReturnedMailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busReturnedMailGen.
        /// </summary>
		public cdoReturnedMail icdoReturnedMail { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPersonAddress. 
        /// </summary>
		public Collection<busPersonAddress> iclbPersonAddress { get; set; }



        /// <summary>
        /// MPIPHP.busReturnedMailGen.FindReturnedMail():
        /// Finds a particular record from cdoReturnedMail with its primary key. 
        /// </summary>
        /// <param name="aintReturnedMailId">A primary key value of type int of cdoReturnedMail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindReturnedMail(int aintReturnedMailId)
		{
			bool lblnResult = false;
			if (icdoReturnedMail == null)
			{
				icdoReturnedMail = new cdoReturnedMail();
			}
			if (icdoReturnedMail.SelectRow(new object[1] { aintReturnedMailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busReturnedMailGen.LoadPersonAddresss():
        /// Loads Collection object iclbPersonAddress of type busPersonAddress.
        /// </summary>
        //public virtual void LoadPersonAddresss()
        //{
        //    DataTable ldtbList = Select<cdoPersonAddress>(
        //        new string[1] { enmPersonAddress..ToString() },
        //        new object[1] { icdoReturnedMail.returned_mail_id }, null, null);
        //    iclbPersonAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress");
        //}

	}
}

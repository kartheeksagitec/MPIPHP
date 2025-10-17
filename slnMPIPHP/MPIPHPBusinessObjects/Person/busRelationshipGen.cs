#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using   MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace   MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class   MPIPHP.BusinessObjects.busRelationshipGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonBeneficiary and its children table. 
    /// </summary>
	[Serializable]
	public class busRelationshipGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for   MPIPHP.BusinessObjects.busRelationshipGen
        /// </summary>
        public busRelationshipGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busicdoRelationshipGen.
        /// </summary>
        public cdoRelationship icdoRelationship { get; set; }

        ///// <summary>
        ///// Gets or sets the non-collection object of type busOrganization.
        ///// </summary>
        //public busOrganization ibusOrganization { get; set; }


        ///// <summary>
        ///// Gets or sets the collection object of type busPersonAccountBeneficiary. 
        ///// </summary>
        //public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary { get; set; }



        /// <summary>
        ///   MPIPHP.busRelationshipGen.FindPersonBeneficiary():
        /// Finds a particular record from cdoPersonBeneficiary with its primary key. 
        /// </summary>
        /// <param name="aintbeneficiaryid">A primary key value of type int of cdoPersonBeneficiary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindRelationship(int aintrelationshipid)
		{
			bool lblnResult = false;
			if (icdoRelationship== null)
			{
				icdoRelationship= new cdoRelationship();
			}
            if (icdoRelationship.SelectRow(new object[1] { aintrelationshipid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        ///// <summary>
        /////   MPIPHP.busRelationshipGen.LoadOrganization():
        ///// Loads non-collection object ibusOrganization of type busOrganization.
        ///// </summary>
        //public virtual void LoadOrganization()
        //{
        //    if (ibusOrganization == null)
        //    {
        //        ibusOrganization = new busOrganization();
        //    }
        //    ibusOrganization.FindOrganization(icdoRelationship.beneficiary_org_id);
        //}

        ///// <summary>
        /////   MPIPHP.busRelationshipGen.LoadPersonAccountBeneficiarys():
        ///// Loads Collection object iclbPersonAccountBeneficiary of type busPersonAccountBeneficiary.
        ///// </summary>
        //public virtual void LoadPersonAccountBeneficiarys()
        //{
        //    DataTable ldtbList = Select<cdoPersonAccountBeneficiary>(
        //        new string[1] { enmPersonAccountBeneficiary.person_relationship_id.ToString() },
        //        new object[1] { icdoRelationship.person_relationship_id }, null, null);
        //    iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbList, "icdoPersonAccountBeneficiary");
        //}

	}
}

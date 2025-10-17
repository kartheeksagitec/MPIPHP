#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using    MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace    MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class    MPIPHP.BusinessObjects.busBenefitApplicationGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitApplication and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitApplicationGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busBenefitApplicationGen
        /// </summary>
		public busBenefitApplicationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitApplicationGen.
        /// </summary>
		public cdoBenefitApplication icdoBenefitApplication { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }
        public busPersonBase ibusPersonBase { get; set; }

        public busBenefitApplicationChecklist ibusBenefitApplicationChecklist { get; set; }

        public busBenefitApplicationAuditingChecklist ibusBenefitApplicationAuditingChecklist { get; set; }
        /// <summary>
        /// Gets or sets the collection object of type busBenefitApplicationStatusHistory. 
        /// </summary>
		public Collection<busBenefitApplicationStatusHistory> iclbBenefitApplicationStatusHistory { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busBenefitApplicationDetail. 
        /// </summary>
		public Collection<busBenefitApplicationDetail> iclbBenefitApplicationDetail { get; set; }

        /// <summary>
        /// PIR-799
        ///  Gets or sets the collection object of type busBenefitApplicationEligiblePlans
        /// </summary>
        public Collection<busBenefitApplicationEligiblePlans> iclbBenefitApplicationEligiblePlans { get; set; }
        
        /// <summary>
        /// PIR-799
        /// Gets or sets the collection object of type busBenefitApplicationDetail. 
        /// </summary>
        public Collection<busBenefitApplicationDetail> iclbApprovedBenefitApplicationDetail { get; set; }

        /// <summary>
        ///    MPIPHP.busBenefitApplicationGen.FindBenefitApplication():
        /// Finds a particular record from cdoBenefitApplication with its primary key. 
        /// </summary>
        /// <param name="aintBenefitApplicationId">A primary key value of type int of cdoBenefitApplication on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitApplication(int aintBenefitApplicationId)
		{
			bool lblnResult = false;
			if (icdoBenefitApplication == null)
			{
				icdoBenefitApplication = new cdoBenefitApplication();
			}
			if (icdoBenefitApplication.SelectRow(new object[1] { aintBenefitApplicationId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    MPIPHP.busBenefitApplicationGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoBenefitApplication.person_id);
		}

        /// <summary>
        ///    MPIPHP.busBenefitApplicationGen.LoadBenefitApplicationStatusHistorys():
        /// Loads Collection object iclbBenefitApplicationStatusHistory of type busBenefitApplicationStatusHistory.
        /// </summary>
		public virtual void LoadBenefitApplicationStatusHistorys()
		{
			DataTable ldtbList = Select<cdoBenefitApplicationStatusHistory>(
				new string[1] { enmBenefitApplicationStatusHistory.benefit_application_id.ToString() },
                new object[1] { icdoBenefitApplication.benefit_application_id }, null, "STATUS_DATE DESC");
           // ldtbList.DefaultView.Sort = "BENEFIT_APPLICATION_STATUS_HISTORY_ID DESC";
            iclbBenefitApplicationStatusHistory = GetCollection<busBenefitApplicationStatusHistory>(ldtbList, "icdoBenefitApplicationStatusHistory");
		}

        /// <summary>
        ///    MPIPHP.busBenefitApplicationGen.LoadPensionBenefits():
        /// Loads Collection object iclbPensionBenefit of type busPensionBenefit.
        /// </summary>
		public virtual void LoadBenefitApplicationDetails()
		{
            DataTable ldtbList = Select<cdoBenefitApplicationDetail>(
                new string[1] { enmBenefitApplicationDetail.benefit_application_id.ToString() },
				new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
            iclbBenefitApplicationDetail = GetCollection<busBenefitApplicationDetail>(ldtbList, "icdoBenefitApplicationDetail");
		}

        /// <summary>
        /// PIR-799
        /// Loads Collection object iclbBenefitApplicationEligiblePlans of type busBenefitApplicationEligiblePlans
        /// </summary>
        public virtual void LoadBenefitApplicationEligiblePlansDetails()
        {
            DataTable ldtbList = Select<cdoBenefitApplicationEligiblePlans>(
                new string[1] { enmBenefitApplicationEligiblePlans.benefit_application_id.ToString() },
                new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
            iclbBenefitApplicationEligiblePlans = GetCollection<busBenefitApplicationEligiblePlans>(ldtbList, "icdoBenefitApplicationEligiblePlans");
        }

        /// <summary>
        ///  PIR-799
        /// Loads Collection object iclbBenefitApplicationDetail of type busBenefitApplicationDetail for approved benefit applications
        /// </summary>
        public virtual void LoadApprovedBenefitApplicationDetails(int aintBenefitApplicationId)
        {
            DataTable ldtbList = Select<cdoBenefitApplicationDetail>(
                new string[1] { enmBenefitApplicationDetail.benefit_application_id.ToString() },
                new object[1] { aintBenefitApplicationId }, null, null);
            iclbApprovedBenefitApplicationDetail = GetCollection<busBenefitApplicationDetail>(ldtbList, "icdoBenefitApplicationDetail");
        }
	}
}

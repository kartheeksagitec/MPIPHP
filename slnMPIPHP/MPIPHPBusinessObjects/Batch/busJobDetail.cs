#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busJobDetail : busJobDetailGen
    {
        #region Public Variables
        public Collection<busJobParameters> iclbJobParameters { get; set; }

        // Fields added for setting up the JobParameters from the UI
        public Collection<busJobParameters> iclbEditableJobParameters { get; set; }
        public Collection<busJobParameters> iclbNonEditableJobParameters { get; set; }

        public busBatchSchedule ibusBatchSchedule { get; set; }

        public busJobHeader ibusJobHeader { get; set; }
        #endregion

        #region Public Methods

        public void LoadJobHeader()
        {
            if (ibusJobHeader == null)
            {
                DataTable ldtbList = Select<cdoJobHeader>(
                    new string[1] { enmJobHeader.job_header_id.ToString() },
                    new object[1] { icdoJobDetail.job_header_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    ibusJobHeader = new busJobHeader();
                    ibusJobHeader.icdoJobHeader = new cdoJobHeader();
                    ibusJobHeader.icdoJobHeader.LoadData(ldtbList.Rows[0]);
                }
            }
        }
        public void LoadJobParameters()
        {
            // We can't get the records directly from the Job_Parameters table, since 
            // we need to know whether the parameter is read_only or not.
            DataTable ldtbList = Select("cdoJobParameters.GetJobParametersForDetailId",
            new object[1]
				{	
                    icdoJobDetail.job_detail_id
				});

            if (ldtbList.Rows.Count > 0)
            {
                iclbEditableJobParameters = new Collection<busJobParameters>();
                iclbNonEditableJobParameters = new Collection<busJobParameters>();

                // This collection is populated purely for deleting the JobDetail cleanly through the grid
                iclbJobParameters = GetCollection<busJobParameters>(ldtbList, "icdoJobParameters");

            }
        }                

        public void LoadStepInfo()
        {
            DataTable ldtbList = Select<cdoBatchSchedule>(
                new string[1] { enmJobDetail.step_no.ToString() },
                new object[1] { icdoJobDetail.step_no }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                ibusBatchSchedule = new busBatchSchedule();
                ibusBatchSchedule.icdoBatchSchedule = new cdoBatchSchedule();
                ibusBatchSchedule.icdoBatchSchedule.LoadData(ldtbList.Rows[0]);
            }
        }

        #endregion

        #region Overridden Methods
        public override void BeforePersistChanges()
        {
            // Only in the Insert mode, we need to go ahead and populate the parameters table with the appropriate values
            // these values will not be displayed in the grid in "Insert" mode, but they would automatically get
            // persisted the moment we save it.
            if (icdoJobDetail.ienuObjectState == ObjectState.Insert)
            {
                //LoadJobParametersinNewMode();
                // Go ahead and persist the core object, so we could use that identifier to save the other objects.
                icdoJobDetail.Insert();

                //// Now go ahead and insert the editable and non editable parameters collection to the database.
                //foreach (busJobParameters lobjJobParameters in iclbEditableJobParameters)
                //{
                //    lobjJobParameters.icdoJobParameters.job_detail_id = icdoJobDetail.job_detail_id;
                //    lobjJobParameters.icdoJobParameters.Insert();
                //}

                //foreach (busJobParameters lobjJobParameters in iclbNonEditableJobParameters)
                //{
                //    lobjJobParameters.icdoJobParameters.job_detail_id = icdoJobDetail.job_detail_id;
                //    lobjJobParameters.icdoJobParameters.Insert();
                //}

            }
            base.BeforePersistChanges();
        }

        public override bool ValidateSoftErrors()
        {
            bool lblnResult = base.ValidateSoftErrors();

            // We need to explicitly call UpdateValidateStatus() in order to update the status on the detail record
            // before proceeding to validate the header record

            UpdateValidateStatus();
            // Anytime a detail is validated, we need to ensure that the corresponding header record
            // is validated too, since quite a lot of validation on the header depends upon the status of the
            // detail record.
            LoadJobHeader();
            ibusJobHeader.ValidateSoftErrors();
            ibusJobHeader.UpdateValidateStatus();
            return lblnResult;
        }

        public override void AfterPersistChanges()
        {
            // Load the Step information from the SGS_Batch_Schedule table, since we need to show the step name to the user
            LoadStepInfo();
            base.AfterPersistChanges();
        }


        public override int Delete()
        {
            // Get the JobHeaderId before the delete operation and once delete has happened
            // go ahead and fire the validation/updatevalidatestatus to get the header record to the appropriate status
            int lintJobHeaderId = icdoJobDetail.job_header_id;
            base.Delete();

            // Go ahead and load the job header record and try to validate it.
            busJobHeader lobjJobHeader = new busJobHeader();
            lobjJobHeader.FindJobHeader(lintJobHeaderId);

            lobjJobHeader.ValidateSoftErrors();
            lobjJobHeader.UpdateValidateStatus();
            return 0;
        }

        #endregion



    }
}

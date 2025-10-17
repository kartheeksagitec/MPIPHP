using System;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for busInitialize.
    /// </summary>
    public class busInitialize : busMPIPHPBatch
    {
        public void InitializeStep()
        {
            try
            {
                PostInfoMessage("Initialize step started...");

                //Current cycle no
                _iCurrentCycleNo = iobjSystemManagement.icdoSystemManagement.current_cycle_no;

                //Update the end time for current cycle in cycle history
                cdoCycleHistory lobjCycleHistory = new cdoCycleHistory();
                lobjCycleHistory.cycle_no = _iCurrentCycleNo;
                if (lobjCycleHistory.SelectRow(new object[1] { _iCurrentCycleNo }))
                {
                    lobjCycleHistory.end_time = DateTime.Now;
                    lobjCycleHistory.Update();
                }
                else
                {
                    //major error - cycle history not found
                    PostErrorMessage("Cycle history not found for cycle_no= " + _iCurrentCycleNo.ToString());
                    throw new Exception("Cycle history not found for cycle_no=" + _iCurrentCycleNo.ToString());
                }

                PostInfoMessage("Starting new cycle ");
                //Start the new cycle
                _iCurrentCycleNo++;

                //Insert a new record in cycle history and update system management
                lobjCycleHistory = new cdoCycleHistory();
                lobjCycleHistory.cycle_no = _iCurrentCycleNo;
                lobjCycleHistory.start_time = DateTime.Now;
                lobjCycleHistory.end_time = DateTime.MaxValue;
                lobjCycleHistory.Insert();

                //Fire the query to populate the beginning of cycle data
                iobjSystemManagement.icdoSystemManagement.batch_date = DateTime.Now.Date;
                iobjSystemManagement.icdoSystemManagement.current_cycle_no = _iCurrentCycleNo;
                iobjSystemManagement.icdoSystemManagement.Update();
                PostInfoMessage("New cycle started. Initialize Step Complete");
            }
            catch (Exception e)
            {
                // Prem : Changes made on July 13th to fix the error handling procedure 
                // Changing the throw new exception process to just "throw" which would give the exact
                // place where the error took place.
                PostErrorMessage("Error in method InitializeStep() : " + e.Message);
                throw;
            }
        }

    }
}

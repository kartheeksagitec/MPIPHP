using Sagitec.Bpm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    /// Date: 04/25/2024
    /// Iteration: 14
    /// Developer: Jayendran
    /// Comment: This class has been overriden to send reccur type as "None" to the Suspended Activity, On Expiration, and Unassigned Activity.
    public class busSolBpmEscalation : busBpmEscalation
    {
        public busSolBpmEscalation()  :base()
        {

        }
        public override void BeforePersistChanges()
        {
            if( icdoBpmEscalation != null)
            {
                if(icdoBpmEscalation.lapse_type_value == "LTNA")
                {
                    icdoBpmEscalation.recur_type_value = "RTNO";
                }
                else if (icdoBpmEscalation.lapse_type_value == "LTOE")
                {
                    icdoBpmEscalation.recur_type_value = "RTNO";
                }
                else if (icdoBpmEscalation.lapse_type_value == "LTSA")
                {
                    icdoBpmEscalation.recur_type_value = "RTNO";
                }
                else
                {
                    //Skip in case of After Expiration and Before Expiration
                }
            }
            base.BeforePersistChanges();
        }
    }
}

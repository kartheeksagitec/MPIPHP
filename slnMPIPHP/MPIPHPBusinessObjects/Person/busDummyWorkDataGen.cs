using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPIPHP.BusinessObjects.Person
{
    [Serializable]
    class busDummyWorkDataGen : busBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDummyWorkDataGen
        /// </summary>
        public busDummyWorkDataGen()
        {

        }

        /// <summary>
        /// Gets or sets the main-table object contained in busDummyWorkDataGen.
        /// </summary>
        public cdoDummyWorkData icdoDummyWorkData;



    }
}

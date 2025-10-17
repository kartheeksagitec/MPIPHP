using NeoSpin.DataObjects;
using Sagitec.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    public class busSystemConstants : busNeoSpinBase
    {
        #region [Properties]

        /// <summary>
        /// Property for System Constants
        /// </summary>
        public cdoSystemConstants icdoSystemConstants { get; set; }

        #endregion [Properties]

        #region [Constructor]

        public busSystemConstants() : base()
        {

        }

        #endregion [Constructor]
    }
}

using NeoBase.Common.DataObjects;
using Sagitec.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class cdoBpmDashboard : doNeoBase
    {
        #region [Constructor]

        public cdoBpmDashboard()
            : base()
        {

        }

        #endregion [Constructor]

        #region [Overridden Methods]

        /// <summary>
        /// Insert data. DO NOTHING
        /// </summary>
        /// <returns></returns>
        public override int Insert()
        {
            return 0;
        }

        /// <summary>
        /// Update data. DO NOTHING
        /// </summary>
        /// <returns></returns>
        public override int Update()
        {
            return 0;
        }

        /// <summary>
        /// Delete data. DO NOTHING
        /// </summary>
        /// <returns></returns>
        public override int Delete()
        {
            return 0;
        }

        /// <summary>
        /// Select data. DO NOTHING
        /// </summary>
        /// <returns></returns>
        public override bool Select()
        {
            return true;
        }

        #endregion [Overridden Methods]
    }
}

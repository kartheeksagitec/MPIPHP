using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

namespace MPIPHP.BusinessObjects.Person
{
    [Serializable]
    public class busFilePersonIn : busFileBase
    {
        #region Constructors
        public busFilePersonIn() : base()
        {

        }
        #endregion

        #region overriden methods.
        public override busBase NewDetail()
        {
            busPerson lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            return lbusPerson;
        }

        public override bool ValidateFile()
        {
            return base.ValidateFile();
        }
        #endregion
    }
}

#region Using directives

using System;
using System.Data;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busProcessLookup : busMainBase
    {
        private Collection<busProcess> _icolProcess;
        public Collection<busProcess> icolProcess
        {
            get
            {
                return _icolProcess;
            }

            set
            {
                _icolProcess = value;
            }
        }

        public void LoadProcess(DataTable adtbSearchResult)
        {
            _icolProcess = GetCollection<busProcess>(adtbSearchResult, "icdoProcess");
        }        
    }
}
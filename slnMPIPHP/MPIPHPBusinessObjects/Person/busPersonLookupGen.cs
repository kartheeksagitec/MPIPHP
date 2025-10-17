#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPersonLookupGen:
    /// Inherited from busMainBase,It is used to create new look up business object. 
    /// </summary>

    [Serializable]
    public class busPersonLookupGen : busMainBase
    {
        /// <summary>
        /// Gets or sets the collection object of type busPerson. 
        /// </summary>
        public Collection<busPerson> iclbPerson { get; set; }


        /// <summary>
        /// MPIPHP.BusinessObjects.busPersonLookupGen.LoadPersons(DataTable):
        /// Loads Collection object iclbPerson of type busPerson.
        /// </summary>
        /// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPersonLookupGen.iclbPerson</param>
        public virtual void LoadPersons(DataTable adtbSearchResult)
        {
            iclbPerson = GetCollection<busPerson>(adtbSearchResult, "icdoPerson");
        }
    }
}

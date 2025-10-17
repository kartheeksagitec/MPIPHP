// -----------------------------------------------------------------------
// <copyright file="busUploadFile.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Data;
using System.IO;
using NeoSpin.DataObjects;

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]

    public class busUploadFile : busMPIPHPBase
    {
        #region Members
        public int iintOrgID { get; set; }
        public int iintFileType { get; set; }
        public string istrInfo { get; set; }


        #endregion

        #region [Methods]



        #endregion
    }
}
#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvFiles : srvDefault
    {
        public srvFiles()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void generatePBISFile()
        {
            busProcessFiles lobjProcessFiles = new busProcessFiles();
            //lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
            lobjProcessFiles.CreateOutboundFile(40);
            
        }

        public void generatePaymentAuditorFile(DateTime start_date, DateTime end_date)
        {
            busProcessFiles lobjProcessFiles = new busProcessFiles();
            lobjProcessFiles.CreateOutboundFile(41);
        }

        public void generateContributionAuditorFile(DateTime start_date, DateTime end_date)
        {
            busProcessFiles lobjProcessFiles = new busProcessFiles();
            lobjProcessFiles.CreateOutboundFile(42);
        }
    }
}


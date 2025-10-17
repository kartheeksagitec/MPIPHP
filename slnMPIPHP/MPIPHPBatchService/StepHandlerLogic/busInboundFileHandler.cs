using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CorBuilder;
using Sagitec.Interface;
using MPIPHP.BusinessObjects;
using MPIPHP.Common;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;

namespace MPIPHPJobService
{
    public class busInboundFileHandler : busBatchHandler
    {
        private busProcessFiles ibusProcessFiles;
        private int iintFileId;

        public busInboundFileHandler()
        { 
        
        }

        public busInboundFileHandler(int aintFileId, busSystemManagement abusSystemManagement, UpdateProcessLog adlgUpdateProcessLog, int aintCurrentCycleNo, utlPassInfo aobjPassInfo)
        {
            ibusProcessFiles = new busProcessFiles();
            ibusProcessFiles.iobjSystemManagement = abusSystemManagement;
            ibusProcessFiles.idlgUpdateProcessLog = new busProcessFiles.UpdateProcessLog(adlgUpdateProcessLog);
            ibusProcessFiles.iintCycleNo = aintCurrentCycleNo;
            iobjPassInfo = aobjPassInfo;
            iintFileId = aintFileId;
        }

        public override void Process()
        {
            base.Process();

            if (iintFileId > 0)
            {
                //ibusProcessFiles.ReceiveFiles(iintFileId);
                //ibusProcessFiles.UploadFiles(iintFileId);
                ibusProcessFiles.ProcessInboundFiles(iintFileId);
            }
            if (iintFileId == busConstant.File.SMALL_WORLD_FILE_ID)
                CopyFileToV3LifeStatus360Folder();
        }

        private void CopyFileToV3LifeStatus360Folder()
        {
            DataTable dtSystemPath = busBase.Select("cdoSystemPaths.GetPathByCode", new object[1] { ibusProcessFiles.iobjFile.process_path_code });
            string processedFilePath = ((busSystemManagement)ibusProcessFiles.iobjSystemManagement).icdoSystemManagement.base_directory + dtSystemPath.Rows[0]["path_value"].ToString();

            string V3LifeStatus360FilePath = MPIPHP.Common.ApplicationSettings.Instance.V3DataPath + @"LifeStatus360\InFiles\";

            DataTable dtFileHdr = busBase.Select<cdoFileHdr>
                (new string[] { "file_id", "status_value", "created_by" }, new object[] { busConstant.File.SMALL_WORLD_FILE_ID, "PROC", ibusProcessFiles.UserID },
                null, null);
            if (dtFileHdr.Rows.Count > 0)
            {
                string processedFileName = dtFileHdr.Rows[0]["PROCESSED_FILE_NAME"].ToString();
                System.IO.File.Copy(processedFilePath + processedFileName, V3LifeStatus360FilePath + processedFileName, true);
            }
        }
    }
}

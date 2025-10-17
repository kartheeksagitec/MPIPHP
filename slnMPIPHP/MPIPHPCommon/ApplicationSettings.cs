using Sagitec.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MPIPHP.Common
{
    [Serializable]
    public class ApplicationSettings : SystemSettings
    {
        /// <summary>
        /// Project application settings
        /// </summary>
        public new static ApplicationSettings Instance
        {
            get
            {
                return SystemSettings.Instance as ApplicationSettings;
            }
        }

        static ApplicationSettings()
        {
            SystemSettings.InitializeSettingsObject = delegate () { return new ApplicationSettings(); };
        }

        public static void MapSettingsObject()
        {
            SystemSettings.InitializeSettingsObject = delegate () { return new ApplicationSettings(); };
        }

        protected ApplicationSettings() : base()
        {

        }

        public ApplicationSettings(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }


        public string SMTP_HOST_PATH { get; protected set;}

        public string SMTP_USERNAME { get; protected set;}

        public string SMTP_PASSWORD { get; protected set;}

        public string SMTP_HOST_PORT { get; protected set;}

        public string NeoFlowMapPath { get; protected set;}

        public string MetaDataCacheUrl { get; protected set;}

        public string DBCacheUrl { get; protected set;}

        public string WebExtenderPath { get; protected set;}

        public string V3DataPath { get; protected set; }

        public string StateTaxBatchFutureFlag { get; protected set; }

        //FM upgrade: 6.0.7.0 changes
        public string NEOFLOW_SERVICE_WORKFLOW_URL { get; protected set;}
    }
}

using NeoBase.BPM;
using NeoBase.Common;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBPMN.Service
{
    public class ExtendedBPMService : BPMService
    {
        public static void Initialize(string[] args)
        {
            string lstrServerName = "BPM Service";
            string lstrUserId = "BPM Service";
            int lintBpmQueueInterval = 500;
            int lintMaxConcurrentQueueRequests = 10;
            int lintDefaultTimerInterval = 300000;
            string[] details;
            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    details = arg.Split('=');
                    switch (details[0])
                    {
                        case "ServerName":
                            lstrServerName = details[1];
                            break;
                        case "UserId":
                            lstrUserId = details[1];
                            break;
                        case "DeQueueInterval":
                            lintBpmQueueInterval = Convert.ToInt32(details[1]);
                            break;
                        case "ConcurrentQueueRequests":
                            lintMaxConcurrentQueueRequests = Convert.ToInt32(details[1]); ;
                            break;
                        case "DefaultTimerInterval":
                            lintDefaultTimerInterval = Convert.ToInt32(details[1]); ;
                            break;
                    }
                }
            }

            // Developer : Rahul Mane
            // Iteration : 8.2
            // Date : 06_14_2021
            // Comment - Change Related to Override busBPMService method at Solution side /Application side

            Type type = ClassMapper.GetSolutionSideDerivedType(typeof(ExtendedBPMService));
            if(type!=null)
            {
                _instance = type.GetConstructors()[0].Invoke(new object[] { lstrServerName, lstrUserId, lintBpmQueueInterval, lintMaxConcurrentQueueRequests, lintDefaultTimerInterval }) as ExtendedBPMService;
            }
            if(_instance == null)
            
            { _instance =  new ExtendedBPMService(lstrServerName, lstrUserId, lintBpmQueueInterval, lintMaxConcurrentQueueRequests, lintDefaultTimerInterval); }
                
           
        }
        static ExtendedBPMService _instance;
        public static ExtendedBPMService Instance
        {
            get
            {
                return _instance;
            }
        }
        static ExtendedBPMService()
        {
            ilstActionMethods = new utlActionMethods();
            GetActionMethods(typeof(BPMService));
        }
        protected ExtendedBPMService(string astrServerName, string astrUserId, int aintBpmQueueInterval = 500, int aintMaxConcurrentQueueRequests = 10, int aintDefaultTimerInterval = 300000) : base(astrServerName, astrUserId, aintBpmQueueInterval, aintMaxConcurrentQueueRequests, aintDefaultTimerInterval)
        {

        }

        protected override void HandleDocumentRequest(busBpmRequest abusBpmRequest)
        {
            if (abusBpmRequest.icdoBpmRequest.tracking_id > 0 &&
                        abusBpmRequest.icdoBpmRequest.reason_value == "UNDL")
            {
                //Initiate the undelivered document map instance for person/organization
                InitializeUndeliveredDocumentProcessForPersonOrOrganization(abusBpmRequest);
                RequestProcessed(abusBpmRequest);
            }
            else
            {
                if (abusBpmRequest.icdoBpmRequest.doc_class == null)
                {
                    busBpmEvent lbusEvent = new busBpmEvent();
                    abusBpmRequest.icdoBpmRequest.doc_class = lbusEvent.FindDocClassBasedOnDocType(abusBpmRequest.icdoBpmRequest.doc_type, abusBpmRequest.icdoBpmRequest.doc_class, true); ;
                }

                base.HandleDocumentRequest(abusBpmRequest);
            }
        }

        protected override void RequestProcessed(busBpmRequest abusBpmRequest)
        {
            if (abusBpmRequest.icdoBpmRequest.status_value != BpmRequestStatus.Restricted)
                base.RequestProcessed(abusBpmRequest);
        }
        protected override void PostProcessingDocumentRequest(busBpmRequest abusBpmRequest)
        {
            busNeobaseBpmRequest lbusNeobaseBpmRequest = ClassMapper.GetObject<busNeobaseBpmRequest>();

            lbusNeobaseBpmRequest.IncomingDocumentReceived(abusBpmRequest);
        }

        /// <summary>
        /// Start the document exception workflow
        /// </summary>
        /// <param name="abusBpmRequest"></param>
        private void InitializeUndeliveredDocumentProcessForPersonOrOrganization(busBpmRequest abusBpmRequest)
        {
            //Need to call this method here to tell communication module about receipt of
            //turn around or undelivered document
            busNeobaseBpmRequest lbusNeobaseBpmRequest = ClassMapper.GetObject<busNeobaseBpmRequest>();
            lbusNeobaseBpmRequest.IncomingDocumentReceived(abusBpmRequest);

            if (abusBpmRequest.icdoBpmRequest.person_id > 0)
            {
                busBpmProcess lbusBpmProcess = busNeobaseBpmActivityInstance.GetBpmProcess("sbpPersonAddressUndeliveredProcess ", "Person Address Undelivered Process", utlPassInfo.iobjPassInfo);

                if (lbusBpmProcess.icdoBpmProcess.process_id > 0)
                {
                    abusBpmRequest.icdoBpmRequest.process_id = lbusBpmProcess.icdoBpmProcess.process_id;

                    //Create the new request parameter for the TrackingId since map needs value of tracking id from request table.
                    busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter() { icdoBpmRequestParameter = new doBpmRequestParameter() };
                    lbusBpmRequestParameter.icdoBpmRequestParameter.request_id = abusBpmRequest.icdoBpmRequest.request_id;
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = "TrackingId";
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = abusBpmRequest.icdoBpmRequest.tracking_id.ToString();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.Insert();
                    abusBpmRequest.iclbBpmRequestParameter.Add(lbusBpmRequestParameter);

                    busBpmCaseInstance lbusBpmCaseInstance = InitiateCaseInstance(abusBpmRequest);

                    if (lbusBpmCaseInstance != null)
                    {
                        AttachDocumentToProcessInstance(abusBpmRequest, lbusBpmCaseInstance.iclbBpmProcessInstance[0], null);
                    }
                }
            }
            else if (abusBpmRequest.icdoBpmRequest.org_id > 0)
            {
                busBpmProcess lbusBpmProcess = busNeobaseBpmActivityInstance.GetBpmProcess("sbpOrganizationAddressUndeliveredProcess", "Organization Address Undelivered Process", utlPassInfo.iobjPassInfo);

                if (lbusBpmProcess.icdoBpmProcess.process_id > 0)
                {
                    abusBpmRequest.icdoBpmRequest.process_id = lbusBpmProcess.icdoBpmProcess.process_id;

                    //Create the new request parameter for the TrackingId since map needs value of tracking id from request table.
                    busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter() { icdoBpmRequestParameter = new doBpmRequestParameter() };
                    lbusBpmRequestParameter.icdoBpmRequestParameter.request_id = abusBpmRequest.icdoBpmRequest.request_id;
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = "TrackingId";
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = abusBpmRequest.icdoBpmRequest.tracking_id.ToString();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.Insert();
                    abusBpmRequest.iclbBpmRequestParameter.Add(lbusBpmRequestParameter);
                    busBpmCaseInstance lbusBpmCaseInstance = InitiateCaseInstance(abusBpmRequest);
                    if (lbusBpmCaseInstance != null)
                    {
                        AttachDocumentToProcessInstance(abusBpmRequest, lbusBpmCaseInstance.iclbBpmProcessInstance[0], null);
                    }
                }
            }

        }

    }
}

using NeoBase.Common;
using NeoSpin.BusinessObjects;
using Newtonsoft.Json;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
//using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.XPath;
namespace NeoSpinWebClient.Controllers
{
    public class MapController : ApiControllerBase
    {
        //private srvServers isrvServers;

        public MapController()
        {
            isrvServers = new srvServers();
        }
        
        protected new Dictionary<string, object> SetParams(string astrFormID)
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams["SessionID"] = iobjSessionData.istrSessionId;
            ldictParams[utlConstants.istrConstFormName] = astrFormID;
            int lintUserSerialID = int.Parse(iobjSessionData["UserSerialID"].ToString());
            ldictParams[utlConstants.istrConstUserSerialID] = lintUserSerialID;
            ldictParams[utlConstants.istrConstUserID] = iobjSessionData["UserID"].ToString();
            return ldictParams;
        }
        // GET api/Map
        [HttpGet]
        public HttpResponseMessage GetData([System.Web.Http.FromUri]int aintCaseInstanceID)
        {
            HttpResponseMessage response = null;
            var idictParams = SetParams("wfmBpmCaseInstanceMaintenance");
            isrvServers.ConnectToBT("wfmBpmCaseInstanceMaintenance");
            
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("aintCaseInstanceId", aintCaseInstanceID);
            busBpmCaseInstance objCaseInstance = (busBpmCaseInstance)isrvServers.isrvBusinessTier.ExecuteMethod("FindBpmCaseInstanceForExecution", lhstParams, false, idictParams);
            if (!string.IsNullOrWhiteSpace(objCaseInstance.ibusBpmCase.icdoBpmCase.bpmmap))
            {
                MapExecutionDetails objData = new MapExecutionDetails();
                objData.lstShapes = PrePareData(objCaseInstance.ibusBpmCase.icdoBpmCase.bpmmap);
                UpdateCanvasBounds(objData);
                objData.lstExecutedSteps = this.LoadExecutedSteps(objCaseInstance);
                
                response = Request.CreateResponse(HttpStatusCode.OK, objData);
            }
            return response;
        }

        [HttpGet]
        public HttpResponseMessage RenderBPM([System.Web.Http.FromUri]int design_specification_bpm_map_id)
        {
            HttpResponseMessage response = null;
            //var idictParams = SetParams("wfmDesignSpecificationMaintenance");
            //isrvServers.ConnectToBT("wfmDesignSpecificationMaintenance");
            //Hashtable lhstParams = new Hashtable();
            //lhstParams.Add("design_specification_bpm_map_id", design_specification_bpm_map_id);
            //busDesignSpecificationBpmMap lbusDesignSpecificationBpmMap = (busDesignSpecificationBpmMap)isrvServers.isrvBusinessTier.ExecuteMethod("FindDesignSpecificationBpmMap", lhstParams, false, idictParams);
            
            //using (MemoryStream stream = new MemoryStream(lbusDesignSpecificationBpmMap.icdoDesignSpecificationBpmMap.file_data))
            //{
            //    XDocument bpmDocument = XDocument.Load(stream);
            //    busBpmCase objbusBpmCase = new busBpmCase();
            //    objbusBpmCase.LoadFromXDocument(bpmDocument);
            //    MapExecutionDetails objData = new MapExecutionDetails();
            //    objData.lstShapes = PrePareData(objbusBpmCase.icdoBpmCase.bpmmap);
            //    UpdateCanvasBounds(objData);
            //    response = Request.CreateResponse(HttpStatusCode.OK, objData);
            //}
            return response;
        }
        // GET api/ReadOnlyMap
        [HttpGet]
        public HttpResponseMessage RenderReadOnlyBPM([System.Web.Http.FromUri]int aintProcessId, [System.Web.Http.FromUri]int aintCaseId)
        {
            HttpResponseMessage response = null;
            var idictParams = SetParams("wfmBpmCaseInstanceMaintenance");
            isrvServers.ConnectToBT("wfmBpmCaseInstanceMaintenance");

            busBpmCase objCaseInstance = null;
            Hashtable lhstParams = new Hashtable();
            if (aintProcessId > 0)
            {
                lhstParams.Add("aintProcessId", aintProcessId);
                objCaseInstance = (busBpmCase)isrvServers.isrvBusinessTier.ExecuteMethod("FindBpmCaseForMap", lhstParams, false, idictParams);
            }
            else if (aintCaseId > 0)
            {
                lhstParams.Add("aintCaseId", aintCaseId);
                objCaseInstance = (busBpmCase)isrvServers.isrvBusinessTier.ExecuteMethod("FindBpmCaseToRenderMap", lhstParams, false, idictParams);
            }
            if (objCaseInstance.IsNotNull() && objCaseInstance.icdoBpmCase.IsNotNull() && !string.IsNullOrWhiteSpace(objCaseInstance.icdoBpmCase.bpmmap))
            {
                MapExecutionDetails objData = new MapExecutionDetails();
                objData.lstShapes = PrePareData(objCaseInstance.icdoBpmCase.bpmmap);
                UpdateCanvasBounds(objData);
                response = Request.CreateResponse(HttpStatusCode.OK, objData);
            }
            return response;
        }
        // POST api/Map
        [HttpPost]
        public HttpResponseMessage GetCallActivityData([System.Web.Http.FromBody]object astrXMLFile)
        {
            HttpResponseMessage response = null;
            CallActivityPostBackData objData = JsonConvert.DeserializeObject<CallActivityPostBackData>(Convert.ToString(astrXMLFile));
            if (!string.IsNullOrWhiteSpace(objData.astrXMLFile))
            {
                MapExecutionDetails objResponse = new MapExecutionDetails();
                objResponse.Title = objData.Title;
                objResponse.WindowID =string.Format("wndw_{0}", Guid.NewGuid().ToString());
                objResponse.lstShapes = PrePareData(objData.astrXMLFile);
                UpdateCanvasBounds(objResponse);
                objResponse.lstExecutedSteps = objData.lstExecutedSteps;
                if (objData.IsExecuted)
                {
                    this.UpdateExecutionStatus(objResponse);
                }
                response = Request.CreateResponse(HttpStatusCode.OK, objResponse);
            }
            return response;
        }

        private void UpdateExecutionStatus(MapExecutionDetails objResponse)
        {
            foreach (ShapeDetails objShape in objResponse.lstShapes)
            {
                if (objResponse.lstExecutedSteps.Any(ele => ele.ElementId == objShape.Id))
                {
                    objShape.IsExecuted = true;
                }
            }
        }

        //private void AssignDBConnections()
        //{
        //    try
        //    {
        //        HelperFunction.istrAppSettingsLocation = @"C:\Source\Studio4.5\SagitecModellingStudio\SagitecModellingStudio\AppSettings.xml";
        //        HelperFunction.PopulateConnections();
        //        iStudioDbConnection = DBFunction.GetDBConnection();
        //        iStudioDbConnection.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //        iStudioDbConnection = null;
        //    }
        //    finally
        //    {
        //    }
        //}

        //private void DisposeDBConnection()
        //{
        //    if (null != this.iStudioDbConnection)
        //    {
        //        this.iStudioDbConnection.Close();
        //        this.iStudioDbConnection.Dispose();
        //    }
        //}

        private List<ShapeDetails> PrePareData(string astrXml)
        {
            List<ShapeDetails> lst = new List<ShapeDetails>();
            XElement xEleMap = XElement.Parse(astrXml);
            foreach (XElement xEleDiagram in xEleMap.Elements())
            {
                if (xEleDiagram.Name.LocalName == "BPMNDiagram")
                {
                    foreach (XElement xElePlane in xEleDiagram.Elements())
                    {
                        foreach (XElement xele in xElePlane.Elements())
                        {
                            ShapeDetails objShape = new ShapeDetails();
                            objShape.ShapeName = xele.Name.LocalName;
                            SetShapeDetails(xele, objShape, xEleMap);
                            lst.Add(objShape);
                        }
                    }
                }
            }

            return lst;
        }

        private void UpdateCanvasBounds(MapExecutionDetails objData)
        {
            double maxHeight = 0;
            double maxWidth = 0;

            foreach (var objShape in objData.lstShapes)
            {
                if ((objShape.Top + objShape.Height) > maxHeight)
                {
                    maxHeight = (objShape.Top + objShape.Height);
                }

                if ((objShape.Left + objShape.Width) > maxWidth)
                {
                    maxWidth = (objShape.Left + objShape.Width);
                }

                if ((objShape.LabelTop + objShape.LabelHeight) > maxHeight)
                {
                    maxHeight = (objShape.LabelTop + objShape.LabelHeight);
                }

                if ((objShape.LabelLeft + objShape.LabelWidth) > maxWidth)
                {
                    maxWidth = (objShape.LabelLeft + objShape.LabelWidth);
                }
            }

            if (maxHeight > objData.Height)
            {
                objData.Height = maxHeight + 100;
            }
            if (maxWidth > objData.Width)
            {
                objData.Width = maxWidth + 100;
            }
        }

        private void SetShapeDetails(XElement xlBPMNShape, ShapeDetails objShape, XElement xEleMap)
        {
            double Left, width, Top, height;
            double LabelLeft, Labelwidth, LabelTop, Labelheight;
            Left = width = Top = height = 0;
            LabelLeft = Labelwidth = LabelTop = Labelheight = 0;

            if (objShape.ShapeName == "BPMNShape")
            {
                XAttribute xAttr = xlBPMNShape.Attributes().FirstOrDefault(itm => itm.Name == "bpmnElement");
                if (xAttr != null)
                {
                    string strId = xAttr.Value;
                    objShape.Id = strId;
                }

                foreach (XElement xlBPMNChildShapes in xlBPMNShape.Elements())
                {
                    if (xlBPMNChildShapes.Name.LocalName == "Bounds")
                    {
                        string strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "x");
                        double.TryParse(strTemp, out Left);

                        strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "y");
                        double.TryParse(strTemp, out Top);

                        strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "height");
                        double.TryParse(strTemp, out height);

                        strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "width");
                        double.TryParse(strTemp, out width);
                    }
                    else if (xlBPMNChildShapes.Name.LocalName == "BPMNLabel")
                    {
                        foreach (XElement xlBPMNChildChildShapes in xlBPMNChildShapes.Elements())
                        {
                            if (xlBPMNChildChildShapes.Name.LocalName == "Bounds")
                            {
                                string strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "x");
                                double.TryParse(strTemp, out LabelLeft);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "y");
                                double.TryParse(strTemp, out LabelTop);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "height");
                                double.TryParse(strTemp, out Labelheight);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "width");
                                double.TryParse(strTemp, out Labelwidth);
                            }
                        }
                    }
                }
                objShape.Left = Left;
                objShape.Top = Top;
                objShape.Width = width;
                objShape.Height = height;
                objShape.LabelLeft = LabelLeft;
                objShape.LabelTop = LabelTop;
                objShape.LabelWidth = Labelwidth;
                objShape.LabelHeight = Labelheight;

            }
            else if (objShape.ShapeName == "BPMNEdge")
            {
                XAttribute xAttr = xlBPMNShape.Attributes().FirstOrDefault(itm => itm.Name == "bpmnElement");
                if (xAttr != null)
                {
                    string strId = xAttr.Value;
                    objShape.Id = strId;
                }

                foreach (XElement xlwaypoint in xlBPMNShape.Elements())
                {
                    if (xlwaypoint.Name.LocalName == "waypoint")
                    {
                        string strTemp = this.GetXmlAttributeValue(xlwaypoint, "x");
                        double.TryParse(strTemp, out Left);
                        strTemp = this.GetXmlAttributeValue(xlwaypoint, "y");
                        double.TryParse(strTemp, out Top);

                        objShape.lstWayPoints.Add(new ShapeDetails() { Left = Left, Top = Top });
                    }
                    else if (xlwaypoint.Name.LocalName == "BPMNLabel")
                    {
                        foreach (XElement xlBPMNChildChildShapes in xlwaypoint.Elements())
                        {
                            if (xlBPMNChildChildShapes.Name.LocalName == "Bounds")
                            {
                                string strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "x");
                                double.TryParse(strTemp, out LabelLeft);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "y");
                                double.TryParse(strTemp, out LabelTop);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "height");
                                double.TryParse(strTemp, out Labelheight);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "width");
                                double.TryParse(strTemp, out Labelwidth);
                            }
                        }
                    }
                }
                objShape.LabelLeft = LabelLeft;
                objShape.LabelTop = LabelTop;
                objShape.LabelWidth = Labelwidth;
                objShape.LabelHeight = Labelheight;

            }
            this.SetShapeType(objShape, xEleMap);
        }

        //private void SetShapeType(ShapeDetails objShape, XElement xEleMap)
        //{
        //    XElement xelmBPMNElement = xEleMap.Descendants().Where(itm => this.GetXmlAttributeValue(itm, "id") == objShape.Id).FirstOrDefault();
        //    if (xelmBPMNElement != null)
        //    {
        //        objShape.ShapeType = xelmBPMNElement.Name.LocalName;
        //        objShape.Text = this.GetXmlAttributeValue(xelmBPMNElement, "name");
        //    }
        //}

        private void SetShapeType(ShapeDetails objShape, XElement xEleMap)
        {
            XElement xelmBPMNElement = xEleMap.Descendants().Where(itm => this.GetXmlAttributeValue(itm, "id") == objShape.Id).FirstOrDefault();
            if (xelmBPMNElement != null)
            {
                objShape.ShapeType = xelmBPMNElement.Name.LocalName;
                if (xelmBPMNElement.Name == "textAnnotation")
                {
                    objShape.Text = GetTextAnnotationText(xelmBPMNElement);
                }
                else
                {
                    objShape.Text = this.GetXmlAttributeValue(xelmBPMNElement, "name");
                }
            }
        }
        private string GetTextAnnotationText(XElement aXEle)
        {
            string retVal = string.Empty;
            XElement xEleText=aXEle.Elements().FirstOrDefault(ele => ele.Name == "text");
            if (null != xEleText)
            {
                try
                {
                    System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
                    rtBox.Rtf = xEleText.Value;
                    retVal = rtBox.Text;
                }
                catch 
                {

                }
            }

            return retVal;
        }

        public string GetXmlAttributeValue(XElement xEle, string attrName)
        {
            return null != xEle.Attribute(attrName) ? xEle.Attribute(attrName).Value : null;
        }

        //private string GetXML(string astrMapName, string astrCaseID)
        //{
        //    string retVal = string.Empty;
        //    if (null != this.iStudioDbConnection)
        //    {
        //        DataTable dtColumnDetails = new DataTable();
        //        string strQuery = string.Format("select BPMMAP from SGW_BPM_CASE where NAME='{0}' and VERSION='{1}'", astrMapName, astrCaseID);
        //        try
        //        {
        //            dtColumnDetails = DBFunction.DBSelect(strQuery, this.iStudioDbConnection);

        //            if (dtColumnDetails != null)
        //            {
        //                foreach (DataRow dr in dtColumnDetails.Rows)
        //                {
        //                    retVal = Convert.ToString(dr["BPMMAP"]);
        //                    break;
        //                }
        //            }
        //        }
        //        catch
        //        {
        //        }
        //        finally
        //        {
        //        }

        //    }

        //    return retVal;

        //}

        private List<clsExecutedStep> LoadExecutedSteps(busBpmCaseInstance objCaseInstance)
        {
            List<clsExecutedStep> lstExecutedShapes = new List<clsExecutedStep>();
            string lstrXML = objCaseInstance.ibusBpmCase.icdoBpmCase.bpmmap;
            List<busBpmCaseInstanceExecutionPath> result = objCaseInstance.iclbBpmCaseInstanceExecutionPath.OrderBy(executionPath => executionPath.icdoBpmCaseInstanceExecutionPath.execution_path_id).ToList();
            foreach (busBpmCaseInstanceExecutionPath ele in result)
            {
                string elementId = ele.icdoBpmCaseInstanceExecutionPath.element_id;
                //code to get containerelementid
                string containerElementId = "";
                int lintActivityInstanceId = 0;
                if (ele.icdoBpmCaseInstanceExecutionPath.activity_instance_id != 0)
                    lintActivityInstanceId = ele.icdoBpmCaseInstanceExecutionPath.activity_instance_id;
                else if (ele.icdoBpmCaseInstanceExecutionPath.parent_activity_instance_id != 0)
                    lintActivityInstanceId = ele.icdoBpmCaseInstanceExecutionPath.parent_activity_instance_id;
                busBpmActivityInstance lobjContainerActivityInstance = null;
                foreach (busBpmProcessInstance lbusBpmProcessInstance in objCaseInstance.iclbBpmProcessInstance)
                {
                    lobjContainerActivityInstance = lbusBpmProcessInstance.iclbBpmActivityInstance.Where(activityInstance => activityInstance.icdoBpmActivityInstance.activity_instance_id == lintActivityInstanceId).FirstOrDefault();
                    if (lobjContainerActivityInstance != null)
                        break;
                }
                busBpmActivity lobjContainerActivity = null;
                if (lobjContainerActivityInstance != null)
                {
                    foreach (busBpmProcess lbusBpmProcess in objCaseInstance.ibusBpmCase.iclbBpmProcess)
                    {
                        lobjContainerActivity = lbusBpmProcess.iclbBpmActivity.Where(activity => activity.icdoBpmActivity.activity_id == lobjContainerActivityInstance.icdoBpmActivityInstance.activity_id && activity.icdoBpmActivity.parent_activity_id != 0).FirstOrDefault();
                        if (lobjContainerActivity != null)
                        {
                            lobjContainerActivity = lbusBpmProcess.iclbBpmActivity.Where(activity => activity.icdoBpmActivity.activity_id == lobjContainerActivity.icdoBpmActivity.parent_activity_id).FirstOrDefault();
                            break;
                        }
                    }
                }
                if (lobjContainerActivity != null)
                    containerElementId = lobjContainerActivity.icdoBpmActivity.bpm_activity_id;
                //code to get containerelementid ends
                clsExecutedStep obj = new clsExecutedStep();
                obj.ContainerElementId = containerElementId;
                obj.ElementId = elementId;
                obj.ParametersSnapShot = ele.icdoBpmCaseInstanceExecutionPath.parameters_snapshot;//dr["PARAMETERS_SNAPSHOT"] as byte[];
                obj.ActivityInstanceId = ele.icdoBpmCaseInstanceExecutionPath.activity_instance_id;
                obj.Parameters = HelperFunction.DeSerializeToObject(obj.ParametersSnapShot);
                if (obj.Parameters != null && obj.Parameters is System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>)
                {
                    obj.Parameters = obj.Parameters as System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>;
                }
                else if (obj.Parameters != null && obj.Parameters is Dictionary<string, System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>>)
                {
                    obj.Parameters = (obj.Parameters as Dictionary<string, System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>>)["Initial"];
                }
                clsExecutedStep parentStep = GetParentStep(lstExecutedShapes, containerElementId);
                obj.EleType = ele.icdoBpmCaseInstanceExecutionPath.element_type_value;
                if (ele.icdoBpmCaseInstanceExecutionPath.element_type_value == "USTK")
                {
                    //UpdateUserTaskExecutionDetails(obj);
                }
                if (parentStep != null)
                {
                    obj.ParentEleId = parentStep.ElementId;
                    obj.XMLFile = GetCallActivityXML(elementId, parentStep.XMLFile);
                    parentStep.lstExecutedSteps.Add(obj);
                }
                else
                {
                    obj.XMLFile = GetCallActivityXML(elementId, lstrXML);
                    lstExecutedShapes.Add(obj);
                }
            }
            return lstExecutedShapes;
        }

        private clsExecutedStep GetParentStep(List<clsExecutedStep> alst, string containerElementId)
        {
            clsExecutedStep retVal = null;
            if (null != alst)
            {
                retVal = alst.FirstOrDefault(itm => itm.ElementId == containerElementId);
                if (null == retVal)
                {
                    foreach (clsExecutedStep itm in alst)
                    {
                        retVal = this.GetParentStep(itm.lstExecutedSteps, containerElementId);
                        if (null != retVal)
                        {
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        private string GetCallActivityXML(string astrCallActivityID, string astrXML)
        {
            string strReturn = null;
            try
            {
                if (!string.IsNullOrEmpty(astrXML))
                {
                    XElement xelem = XElement.Parse(astrXML);
                    XElement xelemCallActivity = xelem.Descendants().FirstOrDefault(itm => itm.Name.LocalName == "callActivity" && this.GetXmlAttributeValue(itm, "id") == astrCallActivityID);
                    if (xelemCallActivity != null)
                    {
                        XElement xelemCalledCaseBpm = xelemCallActivity.XPathSelectElement(string.Format("{0}/calledCaseBpmMap", "extensionElements"));
                        if (xelemCalledCaseBpm != null)
                        {
                            strReturn = xelemCalledCaseBpm.Value;
                        }
                    }
                }
            }
            catch
            {
            }
            return strReturn;
        }

    }

    public class ShapeDetails
    {
        public string Id { get; set; }

        public string ShapeType { get; set; }

        public List<ShapeDetails> lstWayPoints { get; set; }

        public double Left { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public string ShapeName { get; set; }

        public string Text { get; set; }

        public double LabelLeft { get; set; }

        public double LabelWidth { get; set; }

        public double LabelHeight { get; set; }

        public double LabelTop { get; set; }

        public bool IsExecuted { get; set; }

        public bool IsCurrentShape { get; set; }

        public MapExecutionDetails CallActivityDetails { get; set; }

        public ShapeDetails()
        {
            this.lstWayPoints = new List<ShapeDetails>();
        }
    }

    public class clsExecutedStep
    {

        public string ParentEleId;

        public bool IsCurrentNode;

        public bool IsExecuted;

        public bool IsBreakPointAddedForChild; //only for call activity

        public bool IsBreakPointAdded;

        public string ContainerElementId;

        public string ElementId;

        public byte[] ParametersSnapShot;

        public object Parameters;

        public List<clsExecutedStep> lstExecutedSteps;

        public string XMLFile;

        public int ActivityInstanceId;

        public string StartDate;

        public string EndDate;

        public string DueDate;

        //public List<clsEscalations> lstEscalations;
        public string EleType;



        public clsExecutedStep()
        {
            lstExecutedSteps = new List<clsExecutedStep>();
        }
    }

    public class MapExecutionDetails
    {
        public List<ShapeDetails> lstShapes { get; set; }

        public List<clsExecutedStep> lstExecutedSteps { get; set; }

        public double Height;

        public double Width;

        public string WindowID;

        public string Title;

        public int ExecutedStepIndex;

        public MapExecutionDetails()
        {
            this.lstShapes = new List<ShapeDetails>();
            this.lstExecutedSteps = new List<clsExecutedStep>();
        }

    }

    public class CallActivityPostBackData
    {
        public string  astrXMLFile { get; set; }

        public string Title { get; set; }

        public List<clsExecutedStep> lstExecutedSteps { get; set; }

        public bool IsExecuted { get; set; }
    }
   
}
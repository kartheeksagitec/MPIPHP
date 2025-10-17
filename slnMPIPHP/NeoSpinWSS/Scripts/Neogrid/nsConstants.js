var nsConstants;
(function (nsConstants) {
    //Commonely used
    nsConstants.REGX_NUMBER = /^\D+/g;
    nsConstants.BLANK_STRING = "";
    nsConstants.SPACE = " ";
    nsConstants.HASH = "#";
    nsConstants.GLOBAL_VARIABLE_INDICATOR = "~";
    nsConstants.SPACE_HASH = " #";
    nsConstants.SPACE_DOT = " .";
    nsConstants.TRUE = "true";
    nsConstants.FALSE = "false";
    nsConstants.LOOKUP = "Lookup";
    nsConstants.MAINTENANCE = "Maintenance";
    nsConstants.WFM = "wfm";
    nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE = "wfmBPMWorkflowCenterLeftMaintenance";
    nsConstants.CORRESPONDENCE_CLIENT_MVVM_CORR_DIV = "wfmCorrespondenceClientMVVM_CorrDiv";
    nsConstants.REPORT_CLIENT_MVVM_RPT_DIV = "wfmReportClientMVVM_RptDiv";
    nsConstants.CORRESPONDENCE_CLIENT_MVVM = "wfmCorrespondenceClientMVVM";
    nsConstants.REPORT_CLIENT_MVVM = "wfmReportClientMVVM";
    nsConstants.LOGIN_WINDOW_NAME = "LoginWindowName";
    nsConstants.UNDERSCORE_ACTIVITY_INSTANCE_DETAILS = "_ActivityInstanceDetails";
    nsConstants.UNDERSCORE_HOLDER = "_Holder";
    //Form names & div Ids
    nsConstants.DASHBOARD_CALENDAR_MAINTENANCE = "wfmDashboardCalenderMaintenance";
    nsConstants.CALENDAR_PANEL = "pnlCalender";
    nsConstants.STEP_DIV = "stepdiv";
    nsConstants.ERROR_DIV = "ErrorDiv";
    nsConstants.GLOBAL_MESSAGE_DIV = "GlobalMessageDiv";
    nsConstants.MESSAGE_DIV = "MessageDiv";
    nsConstants.CENTER_SPLITTER = "CenterSplitter";
    nsConstants.RIGHT_SPLITTER = "RightSplitter";
    nsConstants.MIDDLE_SPLITTER = "MiddleSplitter";
    nsConstants.QUESTIONNAIRE_CONTAINER = "QuestionnaireContainer";
    nsConstants.GRID_TABLE_UNDERSCORE = "GridTable_";
    nsConstants.LISTVIEW_CONTAINER_UNDERSCORE = "ListViewContainer_";
    nsConstants.DDL_CORRESPONDENCE_LIST = "ddlCorrespondenceList";
    //Class names
    nsConstants.HTML_EDITOR = "HtmlEditor";
    nsConstants.GLOBAL_MESSAGE = "GlobalMessage";
    nsConstants.GLOBAL_ERROR = "GlobalError";
    nsConstants.HIGHLIGHT_ERROR = "HighlightError";
    //jQuery Selectors
    nsConstants.FORMCONTAINER_SELECTOR = "div[role='group']";
    nsConstants.SLIDEOUT_LOOKUP_SELECTOR = "#SlideOutLookup";
    nsConstants.LOOKUP_NAME_SELECTOR = "#LookupName";
    nsConstants.LOOKUP_HOLDER_SELECTOR = "#LookupHolder";
    nsConstants.CRUM_DIV_SELECTOR = "#crumDiv";
    nsConstants.DASHBOARD_SELECTOR = "#DashBoard";
    nsConstants.RIGHT_SPLITTER_SELECTOR = "#RightSplitter";
    nsConstants.CONTENT_SPLITTER_SELECTOR = "#ContentSplitter";
    nsConstants.CENTER_LEFT_SELECTOR = "#CenterLeft";
    nsConstants.DIV_SW_MAIN = "div.swMain";
    nsConstants.GRIDTABLE_STARTWITH_SELECTOR = "[id^=GridTable_]";
    nsConstants.LISTVIEW_CONTAINER_STARTWITH_SELECTOR = "[id^=ListViewContainer_]";
    nsConstants.DIV_LISTVIEW_ITEMS = "div.ListViewItems";
    nsConstants.CORR_HOLDER_SELECTOR = "#CorrHolder";
    nsConstants.MY_BASKET_SELECTOR = "#MyBasket";
    nsConstants.RPT_HOLDER_SELECTOR = "#RptHolder";
    //kendo data-Controls
    nsConstants.KENDO_SCHEDULER = "kendoScheduler";
    nsConstants.KENDO_DATE_PICKER = "kendoDatePicker";
    //Other data-Controls
    nsConstants.DATE_PICKER = "datepicker";
    nsConstants.SMART_WIZARD = "smartWizard";
    //Tags
    nsConstants.INPUT_TAG = "INPUT";
    nsConstants.SPAN_TAG = "SPAN";
    nsConstants.TEXTAREA_TAG = "TEXTAREA";
    nsConstants.SELECT_TAG = "SELECT";
    nsConstants.IMG = "img";
    nsConstants.OPTION = "option";
    nsConstants.INPUT = "input";
    nsConstants.SELECT = "select";
    //Attributes
    nsConstants.DATA_BIND = "data-bind";
    nsConstants.CONTROL_TYPE = "controltype";
    nsConstants.TYPE = "type";
    nsConstants.DISABLED = "disabled";
    nsConstants.TITLE = "title";
    nsConstants.READONLY = "readonly";
    //sfw Attributes
    nsConstants.SFW_AUTO_QUERY = "sfwautoquery";
    nsConstants.SFW_CONTROL_TYPE = "sfwcontroltype";
    nsConstants.SFW_RADIO_BUTTON_LIST_LOWER = "sfwradiobuttonlist";
    nsConstants.SFW_RADIO_BUTTON_LIST = "sfwRadioButtonList";
    nsConstants.SFW_CHECKBOX_LIST_LOWER = "sfwcheckboxlist";
    nsConstants.SFW_CHECKBOX_LIST = "sfwCheckBoxList";
    nsConstants.SFW_RELATED_CONTROL = "sfwRelatedControl";
    nsConstants.SFW_DATA_FIELD = "sfwDataField";
    nsConstants.SFW_OPERATOR = "sfwOperator";
    nsConstants.SFW_METHOD_NAME = "sfwMethodName";
    nsConstants.SFW_NAVIGATION_PARAM = "sfwNavigationParameter";
    nsConstants.SFW_TOOLTIP_TABLE_PARAM = "sfwTooltipTableParams";
    //ControlTypes
    nsConstants.CHECKBOX = "checkbox";
    nsConstants.RADIO = "radio";
    nsConstants.TEXT = "text";
    //Events
    nsConstants.CLICK = "click";
})(nsConstants || (nsConstants = {}));

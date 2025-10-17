<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>

<!DOCTYPE html>

<html lang="en">
<head>
   <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    
    <link href="../../Styles/neoGrid.min.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet"  linkUserCssTheme="true" href="<%=System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl("~/bundles/AppSideCSS")%>" type="text/css" />
    <link rel="stylesheet" href="<%=System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl("~/bundles/FMStyle")%>" type="text/css" />
    <link rel="stylesheet" href="<%=System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl("~/bundles/AppCSS")%>" type="text/css" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css?family=Roboto" rel="stylesheet">
    <link rel="stylesheet" linkUserCssTheme="true" href="<%= System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl ("~/bundles/AppSideCSS")%>" type="text/css" /> 

    <meta name="viewport" content="width=device-width" />
     <style type="text/css">
        html,
        body {
            height: 100%;
            overflow: hidden;
        }

        #page-loader {
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            z-index: 99999;
            width: 100%;
            height: 100%;
            padding-top: 25px;
            display: none;
            background: rgba(0,0,0,.3);
        }

        /* default style */
        .selectnav {
            display: none;
        }

        /* small screen */
        @media screen and (max-width: 900px) {
            .js #MenuUl {
                display: none;
            }

            .js .selectnav {
                display: block;
            }
        }

        .placeholder {
            opacity: 0.4;
            border: 1px dashed #a6a6a6;
        }
    </style>
</head>
<body class="login-page-Bg">
    <%= Html.AntiForgeryToken() %>
    <%=Html.HiddenFor(m => m.LoginWindowName) %>
    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>" />
    <input type="hidden" id="LoginWindowName" name="LoginWindowName" />
    <div id="dropDiv">
        <table style="height: 100%; width: 100%; z-index: 300">
            <tr>
                <td id="dropLeft" class="dropTd">
                    <h2>Open on left</h2>
                </td>
                <td id="dropRight" class="dropTd">
                    <h2>Open on right</h2>
                </td>
            </tr>
        </table>
    </div>

    <div id="pnlLoading" style="z-index: 9999; height: 100%; width: 100%">
        <table width="100%" height="100%">
            <tr>
                <td style="vertical-align: middle; text-align: center; font-size: x-large; color: green">
                    <span>Please wait, initial page is loading...
                    </span>
                </td>
            </tr>
        </table>
    </div>
    <div id="MainSplitter" style="height: 100%; border: 0; opacity: 0;">
        <div class="page-header navbar navbar-fixed-top">
            <div class="header-inner">

                <a class="page-logo">
                    <img src="../Images/neospin-logo.png" alt="logo" class="img-responsive">
                </a>
                <div class="portal-head">
                    Employer Self Service Portal
                </div>
            </div>



            <div class="Chartconfiguration" id="Chartconfiguration" style="display: none">
                <div id="GridGroupChart">.</div>
            </div>
        </div>

        <div class="page-container">

            <div id="MiddleSplitter" style="left: -1px;">

                <div id="CenterLeft">
                </div>

                <div id="page-content-wrapper">
                    <div class="">
                        <div style="display: none !important;" id="CenterLeftTabs" class="SlideoutSplitter">

                            <div id="SlideOuts">
                                <div id="SearchTriger" title="Lookup Forms" class="trigger left  lookups"></div>
                                <div id="SlideOutLookup" class="panel left">
                                    <div id="LookupParent">
                                        <div id="drpLookupNamesParent">
                                        </div>
                                        <div style="padding: 10px;" role="group" id="LookupHolder">
                                        </div>
                                        <div class="lookups-image">
                                        </div>
                                    </div>
                                </div>

                                <div id="navTreeTriger" class="trigger left navigation LeftPanel_IconNavigator navigator"></div>
                                <div id="SlideOutTree" class="panel left">
                                    <div class="navDiv">
                                        <div style="padding: 10px;" id="TabsTree"></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row page-header-wrap">
                            <div id="crumDiv" class="crumDiv"></div>

                        </div>
                        <div id="GlobalMessageDiv"></div>
                        <div id="ContentSplitter" role="group">
                        </div>

                    </div>
                </div>
            </div>

        </div>


        <div id="page-loader" style="display: none;" class="preloader">
            <table width="100%" height="100%">
                <tr>
                    <td style="vertical-align: middle; text-align: center">
                        <%
                            string lstrLoaderUrl = Url.Content("~/");
                            lstrLoaderUrl += (lstrLoaderUrl.Contains(ConfigurationManager.AppSettings["ApplicationName"]) ? string.Empty : ConfigurationManager.AppSettings["ApplicationName"]);
                        %>
                        <img alt="loader" src='<%=lstrLoaderUrl%>/images/301.gif'>
                    </td>
                </tr>
            </table>
        </div>

        <div style="display: none;" id='DivExportWindow'>
            <div id="ExptColumnsMessage">Select the columns to be exported from the following list:</div>
            <div id="DivExportCols"></div>
            <button id="clickExcel">Download As Excel</button>
        </div>
        <div id="ToolTipDiv" class="tooltip configpanel"></div>


        <div id="SessionExpired" style="display: none">
            <div id="ExpiredMessage">
                <table style="width: 100%; height: 100%;">
                    <tr>
                        <td>Your session will timeout in <span id="spnRemainTimeSec"></span>second, please click a "Continue Session" button or navigate to another page to refresh your session before it expires.                       
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;               
                        </td>
                    </tr>
                    <tr>
                        <td>Do you want to Continue Session?                       
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;             
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <button id="btnContinue" class="k-button" onclick="ns.refreshSession();">Continue Session</button>
                            <button id="btnLogout" class="k-button" onclick="ns.logoutSesssion();">Logout</button>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <input type="hidden" id="SiteName" value="<%=new Uri(Convert.ToString(Request.Url)).GetLeftPart(UriPartial.Authority)%>" />
        <input type="hidden" id="LandingPage" value="<%=Convert.ToString(Model.iobjSessionData["Landing_Page"] ?? "") %>" />
        <script id="treeview-template" type="text/kendo-ui-template"><span class="FormNode" LinkedTo="#:item.divID#">#:item.title#</span><a onclick='javascript: if(nsEvents != undefined && nsEvents.OnDeleteNodeClick != undefined) { nsEvents.OnDeleteNodeClick(this); }  else { OnDeleteNodeClick(this); }' class='delete-link'></a></script>
        <%--  Collpasing of left panel  --%>
        <script type="text/javascript">
</script>

        <div class="FilterBox" style="display: none;">
            <div class="row">Filter value(s) that:</div>
            <div class="row">
                <select id='selectFilterOptions1'></select>
            </div>
            <div class="row">
                <input type='text' id='filterBox1' />
            </div>
            <div class="row">
                <label for="rdoAnd">
                    <input name="FilterCondition" value="and" type="radio" id="rdoAnd" />
                    And
                </label>
                /
            <label for="rdoOr">
                <input name="FilterCondition" value="or" type="radio" id="rdoOr" />
                Or
            </label>
            </div>
            <div class="row">
                <select id='selectFilterOptions2'></select>
            </div>
            <div class="row">
                <input type='text' id='filterBox2' />
            </div>
            <div class="row">
                <input type='button' id='btnFilter' class="s-grid-btnFilter" value='Filter' />
                <input type='button' id='btnClearFilter' class="s-grid-btnClearFilter" value='Clear' />
            </div>

        </div>
        <!-- Added for Framework 6.0.0.24 changes these changes are not requried for MVVM Portals-->
        <%--   <div>
             <div style="display: none;" id='DivExportWindow'>
            <div id="ExptColumnsMessage">Select the columns to be exported from the following list:</div>
            <div id="DivExportCols"></div>
            <button id="clickExcel">Download As Excel</button>
        </div>--%>

        <div class="s-grid-settings-overlay" style="display: none;">
            <div class="s-grid-settings-box" style="display: none;">
                <h3>Settings <span class="s-grid-settings-box-close" title="Close"></span>
                </h3>
                <div class="s-grid-setting-content">
                    <div class="s-grid-setting-row s-grid-settings-sortmode">
                        <div class="captiondiv">
                            Sort Mode:
                        </div>
                        <div class="inputdiv">
                            <div>
                                <select class="s-grid-settings-sortmode-dropdown" id="ddlNeoGridSettingsSortMode"></select>
                            </div>
                        </div>
                    </div>
                    <div class="s-grid-setting-row s-grid-settings-pagesize">
                        <div class="captiondiv">
                            Page Size:
                        </div>
                        <div class="inputdiv">
                            <div>
                                <select class="s-grid-settings-pagesize-dropdown" id="ddlNeoGridSettingsPageSize"></select>
                            </div>
                        </div>
                    </div>
                    <div class="s-grid-setting-columns-row">
                        <div class="captiondiv" title="Drag & drop the columns to rearrange.">
                            Columns (Drag & drop to rearrange)
                        </div>
                        <div class="inputdiv">
                            <div class="s-grid-settings-columns-div">
                                <div class="s-grid-settings-columns" id="spnNeoGridHideColumns">
                                    <ul id="ulNeoGridSettingsColumns" class="s-grid-settings-columns-ul">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="s-grid-settings-footer">
                    <input type="button" id="bntNeoGridSettingsApply" title="Apply settings" value="Apply" class="s-grid-settings-box-apply button" />
                    <input type="button" id="bntNeoGridSettingsOriginal" title="Reset settings to the original state" value="Reset & Apply" class="s-grid-settings-box-original button" />
                </div>
            </div>
        </div>

        <script type="text/javascript">
            window.name = "";
    </script>
        <script src="<%=System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl("~/bundles/FMLibScript")%>" type="text/javascript"></script>
        <%--  <script src="<%=Url.Content("~/Scripts/App/kendo.all.min.js")%>"></script>--%>
        <script src="<%=System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl("~/bundles/FMScript")%>" type="text/javascript"></script>
        <script src="<%=System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl("~/bundles/Assests")%>" type="text/javascript"></script>
        <%--<script src="<%=System.Web.Optimization.BundleTable.Bundles.ResolveBundleUrl("~/bundles/AppSideJS")%>" type="text/javascript"></script>--%>
        <script src="<%=Url.Content("~/Scripts/App/ExternalLoginInitializer.js")%>"></script>
        <script src="<%=Url.Content("~/Scripts/App/UserDefinedFunctions.js")%>"></script>
        <script src="<%=Url.Content("~/Scripts/App/machineSecret.js")%>"></script>
        <script type="text/javascript">
            var Language = "en-US";
            sessionStorage.clear();
            nsCommon.SetWindowName();
        </script>
</body>
</html>

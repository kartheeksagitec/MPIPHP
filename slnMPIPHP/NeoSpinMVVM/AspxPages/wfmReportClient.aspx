<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmReportClient.aspx.cs" Inherits="Neo.AspxPages.wfmReportClient" %>

        <%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body onload="DisplayMessage()">
    <form id="form1" runat="server">

         <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
        
        <rsweb:ReportViewer ID="rvViewer" runat="server" Width="100%" SizeToReportContent="true" Height="100%" Visible="false" />
    </form>
    <script>
        function DisplayMessage() {
            if (window.parent.ns.viewModel.currentModel.startsWith("wfmReportClient"))
                window.parent.nsCommon.DispalyMessage("[ Report Successfully Generated ]", window.parent.ns.viewModel.currentModel);
        }
    </script> 
</body>
</html>

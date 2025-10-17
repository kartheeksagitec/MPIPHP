<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="wfmExportToExcel.aspx.cs" Inherits="wfmExportToExcel" %>
<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Export to Excel</title>
        <script type="text/javascript">
            function btnSelectAll_Click() {
                var chkList1 = document.getElementById("cblExportToExcel").getElementsByTagName("input");
                for (i = 0; i < chkList1.length; i++)
                    chkList1[i].checked = true;
            }
            function btnClearAll_Click() {
                var chkList1 = document.getElementById("cblExportToExcel").getElementsByTagName("input");
                for (i = 0; i < chkList1.length; i++)
                    chkList1[i].checked = false;
            } 
</Script>
</head>
<body>
    <form id="ExportToExcel" title="Export selected columns from grid to excel" runat="server">
    <div>
        <swc:sfwTable ID="tblExport" runat="server">
         <swc:sfwRow ID="SfwSelect" runat="server">
                <swc:sfwColumn ID="SfwColumn4" runat="server">
                 <swc:sfwButton runat="server" ID="btnSelectAll" Text="SelectAll" OnClientClick="btnSelectAll_Click();return false;" EnableViewState="False"/>
                 <swc:sfwButton runat="server" ID="SfwButton1" Text="ClearAll" OnClientClick="btnClearAll_Click();return false;" EnableViewState="False"/>
                </swc:sfwColumn>
            </swc:sfwRow>
            <swc:sfwCMCRow runat="server">
                <swc:sfwCMCColumn runat="server">
                    <swc:sfwLabel ID="lblCaption" Text="&nbsp;&nbsp;&nbsp;&nbsp&nbsp;Select the columns to be exported from the list below : " runat="server">
                    </swc:sfwLabel>
                </swc:sfwCMCColumn>
            </swc:sfwCMCRow>
            <swc:sfwCMCRow ID="tbrExport" runat="server">
                <swc:sfwCMCColumn ID="tbcExport" runat="server">
                   <swc:sfwCheckBoxList ID="cblExportToExcel" runat="server" RepeatDirection="Horizontal" RepeatColumns="3">
                   </swc:sfwCheckBoxList>
                </swc:sfwCMCColumn>
            </swc:sfwCMCRow>
            <swc:sfwCMCRow ID="SfwCMCRow1" runat="server">
                <swc:sfwCMCColumn ID="SfwCMCColumn1" runat="server">
                   <swc:sfwGridView ID="grvExcel" runat="server">
                   </swc:sfwGridView>
                </swc:sfwCMCColumn>
            </swc:sfwCMCRow>
            <swc:sfwCMCRow ID="ExportButton" runat="server">
                <swc:sfwCMCColumn runat="server">
                   <swc:sfwImageButton runat="server" 
                    ID="btnExportExcel" ImageUrl="~/Image/gExport.gif" sfwMethodName="btnExportToExcel_Click" 
                    sfwRelatedControl="dgrResult" OnClick="btnExportToExcel_Click"/>
                </swc:sfwCMCColumn>
            </swc:sfwCMCRow>
        </swc:sfwTable>
    </div>
    </form>
</body>
</html>

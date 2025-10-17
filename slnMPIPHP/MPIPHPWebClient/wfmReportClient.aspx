<%@ Page Language="C#" MasterPageFile="~/wfmMPIPHPBase.master" AutoEventWireup="true"
    CodeFile="wfmReportClient.aspx.cs" Inherits="wfmReportClient_aspx" MaintainScrollPositionOnPostback="true"
    Title="Untitled Page" EnableViewState="true"%>

<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCenterMiddle" runat="Server">
    <swc:sfwTable ID="tblMain" runat="server" CellPadding="0" CellSpacing="0" CssClass="PanelMiddle">
        <asp:TableRow ID="tr1" runat="server">
            <asp:TableCell ID="tc1" runat="server">
                <swc:sfwTable ID="tblReports" runat="server" sfwGroupCaption="Reports" CellPadding="0"
                    CellSpacing="0" CssClass="Table">
                    <asp:TableRow ID="tr2" runat="server">
                        <asp:TableCell ID="tc2" runat="server">
                            <swc:sfwLabel ID="SfwLabel1" runat="server" Text="Select a report :  " CssClass="Label">
                            </swc:sfwLabel>
                            <swc:sfwDropDownList runat="server" sfwCodeTable="Reports" AutoPostBack="True" ID="ddlReports"
                                OnSelectedIndexChanged="ddlReports_SelectedIndexChanged">
                            </swc:sfwDropDownList>
                            <table class="ButtonTable">
                                <tr>
                                    <td class="ButtonCell">
                                        <swc:sfwButton ID="btnGenerate" runat="server" Text="View" sfwMethodName="btnGenerateReport_Click" />
                                    </td>
                                </tr>
                            </table>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="tr3" runat="server">
                        <asp:TableCell ID="tc3" runat="server">
                            <swc:sfwTable runat="server" ID="tblQueryParameters" CssClass="Table">
                            </swc:sfwTable>
                        </asp:TableCell>
                    </asp:TableRow>
                </swc:sfwTable>
                <swc:sfwTable ID="tblReportViewer" runat="server" CellPadding="0" CellSpacing="0" CssClass="Table">
                    <asp:TableRow ID="tr4" runat="server">

                        <asp:TableCell ID="tc4" runat="server">
                            <rsweb:ReportViewer ID="rvViewer" runat="server" Width="1000px" SizeToReportContent="false" Height="1000px" Visible="true" />
                        </asp:TableCell>
                    </asp:TableRow>
                </swc:sfwTable>
            </asp:TableCell>
        </asp:TableRow>
    </swc:sfwTable>
</asp:Content>

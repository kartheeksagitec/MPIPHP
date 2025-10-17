<%@ Page Title="" Language="C#" MasterPageFile="~/wfmMPIPHPBase.master" AutoEventWireup="true" CodeFile="wfmCorrespondenceClient.aspx.cs" Inherits="wfmCorrespondenceClient_aspx" %>
<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5"%>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCenterMiddle" Runat="Server">

 <swc:sfwTable ID="tblMain" runat="server" CellPadding="0" CellSpacing="0" Width="100%">
        <asp:TableRow runat="server">
          <asp:TableCell runat="server">
            <swc:sfwTable ID="tblGenerate" runat="server" sfwTabControl="tbcCorrespondence" CellPadding="0" CellSpacing="0">
              <swc:sfwRow ID="SfwRow1" runat="server">
                <swc:sfwColumn ID="SfwColumn1" runat="server">
                    <asp:Literal ID="Literal1" Text="&nbsp;&nbsp;&nbsp;&nbsp" runat="server"/>
                    <swc:sfwLabel runat="server" ID="lblCorr" Text="Letter:  "></swc:sfwLabel>
                    <swc:sfwDropDownList runat="server" sfwCodeTable="Templates" AutoPostBack="True" ID="mdl_Correspondence" OnSelectedIndexChanged="mdl_Correspondence_SelectedIndexChanged"></swc:sfwDropDownList>
                    <table class="ButtonTable">
                        <tr>
                            <td class="ButtonCell">
                                <swc:sfwButton ID="lbtnGenerate" Runat="server" Text="Generate" sfwMethodName="btnGenerateCorrespondence_Click" />
                            </td>
                        </tr>
                    </table>
                    <table class="ButtonTable">
                        <tr>
                            <td class="ButtonCell">
                                <%--<swc:sfwButton ID="lbtnEdit" Runat="server" Text="Edit" sfwMethodName="btnOpenDoc_Click"/>--%>
                                <swc:sfwButton ID="lbtnEdit" Runat="server" Text="Edit" OnClientClick="EditCorrOnLocalTool(); return false;"  />
                            </td>
                        </tr>
                    </table>
                    <%-- <table class="ButtonTable">
                        <tr>
                            <td class="ButtonCell">
                                <swc:sfwButton ID="lbtnView" Runat="server" Text="View" OnClick="btnView_Click" sfwTriggerPostBack="true" />
                            </td>
                        </tr>
                    </table>--%>
                </swc:sfwColumn>
              </swc:sfwRow>
              <swc:sfwRow ID="SfwRow2" runat="server">
                <swc:sfwColumn ID="SfwColumn2" runat="server">
                  <swc:sfwTable ID="tblQueryBkmks" runat="server" >
                  </swc:sfwTable>
                </swc:sfwColumn>
              </swc:sfwRow>
            </swc:sfwTable>
          </asp:TableCell>
        </asp:TableRow>
    </swc:sfwTable>
</asp:Content>




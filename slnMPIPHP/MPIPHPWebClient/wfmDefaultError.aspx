<%@ Page Language="C#" MasterPageFile="~/wfmMPIPHPBase.master" AutoEventWireup="true" CodeFile="wfmDefaultError.aspx.cs" Inherits="wfmDefaultError" Title="Untitled Page" %>
<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5"%>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCenterMiddle" Runat="Server">
    <span style="font-family:Arial;font-size:12pt;">
        <b>We are very sorry for the inconvenience caused to you, following error occured</b>
    </span>
    <asp:Panel runat="server" GroupingText="Error Message">
        <swc:sfwTable runat="server">
          <swc:sfwCMCRow>
            <swc:sfwCMCColumn>
                <swc:sfwTextBox id="txtDescription" TextMode="multiline" Rows="4"  ReadOnly="true" Width="100%" runat="server"/>
            </swc:sfwCMCColumn>
        </swc:sfwCMCRow>
        </swc:sfwTable>
    </asp:Panel>
    <asp:Panel runat="server" GroupingText="Additional Error Message">
        <swc:sfwTable ID="SfwTable1" runat="server">
        <swc:sfwCMCRow>
            <swc:sfwCMCColumn>
                <swc:sfwTextBox id="txtError" TextMode="multiline" height="350" width="100%" runat="server"/>
            </swc:sfwCMCColumn>
          </swc:sfwCMCRow>
          <swc:sfwCMCRow>
            <swc:sfwCMCColumn>
                <swc:sfwButton ID="btnSend" Text="Send additional error details to technical support"  runat="server" OnClick="btnSend_Click"/>
            </swc:sfwCMCColumn>
          </swc:sfwCMCRow>
        </swc:sfwTable>
    </asp:Panel>
    <swc:sfwLabel id="lblResult" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphCenterRight" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphBottom" Runat="Server">
</asp:Content>


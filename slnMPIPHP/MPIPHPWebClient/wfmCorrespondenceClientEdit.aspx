<%@ Page Language="C#" MasterPageFile="~/wfmCorrespondenceClientEdit.master" AutoEventWireup="true"
    CodeFile="wfmCorrespondenceClientEdit.aspx.cs" Inherits="wfmCorrespondenceClientEdit_aspx" %>

<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCenterMiddle" runat="Server">
    <!-- web page script -->
    <asp:Table ID="tabMainTable" runat="server" CellPadding="0" CellSpacing="0" Width="100%" Height="100%">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <table class="ButtonTable">
                    <tr>
                        <td class="ButtonCell">
                            <swc:sfwButton ID="btnSave" runat="server" Text="Save" sfwMethodName="btnSave_Correspondence_Click" />
                        </td>
                    </tr>
                </table>
                <table class="ButtonTable">
                    <tr>
                        <td class="ButtonCell">
                            <swc:sfwButton ID="btnPrint" runat="server" Text="Print" sfwMethodName="btnPrint_Correspondence_Click" />
                        </td>
                    </tr>
                </table>
                <table class="ButtonTable">
                    <tr>
                        <td class="ButtonCell">
                            <swc:sfwButton ID="lbtnGenerateImage" Runat="server" Text="Image" OnClick="btnImageCorrespondence_Click" />
                        </td>
                    </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ID="tbcWord">
                <div> 
                    <object classid="clsid:00460182-9E5E-11d5-B7C8-B8269041DD57" id="oframe" width="998px" height="785px">
                        <param name="BorderStyle" value="1" />
                        <param name="Titlebar" value="0" />
                        <param name="TitlebarTextColor" value="0" />
                        <param name="Menubar" value="1" /> 
                    </object>   
                </div>               
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>

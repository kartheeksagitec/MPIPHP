<%@ Page Language="C#" MasterPageFile="~/wfmMPIPHPBase.master" AutoEventWireup="true"
    CodeFile="wfmInUploadFile.aspx.cs" Inherits="wfmInUploadFile" Title="Upload Files" %>

<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCenterMiddle" runat="Server">
    <asp:Table runat="server" ID="TblUpload">
        <asp:TableRow>
            <asp:TableCell Style="padding-left: 15px; padding-right: 3px; padding-top: 15px;
                font-family: Verdana; font-size: 11px; font-weight: normal; color: #135a7e;">
                         To submit a file:
                     			<ol>
                    				<li>Use the "Browse" button to select your file</li>
                                    <li>Select or provide the Organization ID to associate the file to an Organization.</li>
                    				<li>Click the "Upload File" button</li>
                    				<li>Wait for Confirmation</li></ol>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>&nbsp;</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Table runat="server" ID="tbl2">
                    <asp:TableRow>
                        <asp:TableCell>
                            <swc:sfwLabel ID="lblFileTypeCaption" Text="File Type : " CssClass="Label" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell>
                            <swc:sfwDropDownList ID="ddlFileType" runat="server" sfwCodeTable="cdoFile.LookupInbound"
                                DataTextField="description" DataValueField="file_id" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>
                            <swc:sfwLabel ID="lblFileCaption" Text="File : " CssClass="Label" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:FileUpload ID="filMyFile" runat="server" Width="693px" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:Table ID="Table1" runat="server" CssClass="ButtonTable" Style="border-collapse: collapse;"
                                border="0" CellSpacing="0" CellPadding="0">
                                <asp:TableRow>
                                    <asp:TableCell CssClass="ButtonCell">
                                        <swc:sfwButton ID="cmdSend" runat="server" Text="Upload File" OnClick="cmdSend_Click" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:Label ID="lblInfo" runat="server" Font-Bold="True" Visible="false"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>&nbsp;</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>

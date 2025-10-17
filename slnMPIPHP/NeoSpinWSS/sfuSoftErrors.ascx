<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sfuSoftErrors.ascx.cs" Inherits="sfuSoftErrors" %>
<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls"%>
<swc:sfwGridView runat="server" Id="grvSoftErrors" AllowPaging="true" AllowSorting="true" AutoGenerateColumns="false">
</swc:sfwGridView>
<swc:sfwLabel runat="server" ID="lblZerogrvSoftErrors" Visible="false" Text="No records to display" />
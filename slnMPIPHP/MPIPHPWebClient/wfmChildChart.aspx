<%@ Page Language="C#" AutoEventWireup="true" Theme="" CodeFile="wfmChildChart.aspx.cs" Inherits="wfmChildChart" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Chart runat="server" ID="chrChildChart" Width="200px" Height="200px" RenderType="BinaryStreaming" ImageType="Png">
    <Series>
        <asp:Series Name="chsChildSeries"/>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="chaChildChartArea"/>
    </ChartAreas>
</asp:Chart>

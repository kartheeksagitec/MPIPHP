<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wfmMSSVideos.aspx.cs" Inherits="wfmMSSVideos"%>

<%@ Register assembly="Media-Player-ASP.NET-Control" namespace="Media_Player_ASP.NET_Control" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function changeScreenSize(w, h) {
            window.resizeTo(w, h)
        }
</script>
</head>
<body onload="changeScreenSize(600,600)">
    <form id="form1" runat="server" >     
    <div align="center" runat="server" id="divVideo">
    <cc1:Media_Player_Control ID="Media_Player_Control1" runat="server" 
        MovieURL="./Video/NDPERS - Life Plan.wmv" CurrentPosition="0" 
        Font-Size="Medium" FullScreen="False" Height="500px" Width="500px" />
    </div>    
    </form>
</body>
</html>

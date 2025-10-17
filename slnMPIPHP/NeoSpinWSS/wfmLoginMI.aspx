<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wfmLoginMI.aspx.cs" Inherits="wfmLoginMI" Theme="LoginTheme" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Welcome to OPUS Member Web Portal for Internal Users</title>
    <style type="text/css">
        <!--
        body {
            background-color: #d7dadd;
        }

        html {
            height: auto !important;
        }

        .style1 {
            width: 288px;
        }
        -->
    </style>
</head>
<body onload="SetWindowName();">
    <form id="form2" runat="server" defaultbutton="btnLogin">
        <script type="text/javascript">
            function SetWindowName() {
                if (document.getElementById("hfldLoginWindowName").value !== "") {
                    window.name = document.getElementById("hfldLoginWindowName").value;
                }
            }
        </script>
        <table width="778" border="0" align="center" cellpadding="0" cellspacing="0">
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <table width="710" border="0" align="center" cellpadding="0" cellspacing="0">
                        <tr>
                            <td width="10" height="13">
                                <img src="images/Border_Top_Left.jpg" alt="Left" width="13" height="13" /></td>
                            <td background="images/Border_Top_middle.jpg"></td>
                            <td width="10" height="13">
                                <img src="images/Border_Top_Right.jpg" width="15" height="13" /></td>
                        </tr>
                        <tr>
                            <td background="images/Border_Center_Left.jpg">&nbsp;</td>
                            <td bgcolor="#FFFFFF">
                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td colspan="3">
                                            <table width="678" border="0" cellspacing="0" cellpadding="0">
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <div align="center">
                                                <img src="images/loginBanner.png" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="135" valign="top" class="style1">
                                            <table border="0" cellspacing="10" cellpadding="0">
                                                <tr>
                                                    <td colspan="2" valign="top" class="contenttxt">
                                                        <table border="0" cellspacing="0" cellpadding="0">
                                                            <tr>
                                                                <td class="contenttxt" style="text-align: justify; width: 300px; padding-left: 0px !important;">If you are a <strong>Participant</strong> or <strong>Business Partner</strong> of the Motion 
                                Picture Industry(MPI) Pension and Health Plans and already have an online 
                                account please enter your user ID and password and then click the <strong>Login</strong> button.</td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>

                                            </table>
                                        </td>
                                        <td width="1" bgcolor="#C1C4C9"></td>
                                        <td width="387" rowspan="2" valign="top">
                                            <table width="100%" border="0" cellspacing="10" cellpadding="0">
                                                <tr>
                                                    <td>
                                                        <table width="85%" border="0" align="center" cellpadding="0" cellspacing="2">
                                                            <tr>
                                                                <td height="30" class="contenthead" align="right">
                                                                    <asp:RequiredFieldValidator ControlToValidate="txtUserId" ErrorMessage="*" ForeColor="red" ID="rfvUserId" runat="server"></asp:RequiredFieldValidator>
                                                                    <label for="txtUserId">User Name:</label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtUserId" Text="" ToolTip="OPUS User ID" Width="150"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="30" class="contenthead" align="right">
                                                                    <asp:RequiredFieldValidator ControlToValidate="txtPassword" ErrorMessage="*" ForeColor="Red" ID="rfvPassword" runat="server"></asp:RequiredFieldValidator>
                                                                    <label for="txtPassword">Password</label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" ToolTip="Password" Width="150"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="30" class="contenthead" align="right">
                                                                    <asp:RequiredFieldValidator ControlToValidate="txtPersonID" ErrorMessage="*" ForeColor="Red" ID="rfvPersonID" runat="server"></asp:RequiredFieldValidator>
                                                                    <label for="txtPersonID">Member ID</label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtPersonID" ToolTip="Member ID" Width="150"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="contenttxt" colspan="2" style="text-align: center; width: 250px; float: left;">
                                                                    <asp:Label runat="server" ID="lblError" ForeColor="Red" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td>
                                                                    <asp:Button runat="server" Text=" Login " class="buttonbg" ID="btnLogin" OnClick="btnLogin_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="20" valign="top" style="padding-left: 10px;" class="style1">
                                            <div align="left">
                                                <span class="footertxt">Privacy Policy &amp; Disclaimer </span><span class="footertxt">| 
                        MPIPHP © 2013 </span>
                                            </div>
                                        </td>
                                        <td bgcolor="#C1C4C9"></td>
                                    </tr>
                                </table>
                            </td>
                            <td background="images/Border_Center_Right.jpg">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <img src="images/Border_Bottom_Left.jpg" width="13" height="15" /></td>
                            <td background="images/Border_Bottom_Center.jpg"></td>
                            <td>
                                <img src="images/Border_Bottom_Right.jpg" width="15" height="15" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td height="25">
                    <div align="center"></div>
                </td>
            </tr>
        </table>
        <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
    </form>
</body>
</html>
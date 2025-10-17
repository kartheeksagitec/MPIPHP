<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wfmLogin.aspx.cs" Inherits="wfmLogin_aspx"
    EnableTheming="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OPUS - One Pension Unified System Login</title>
    <link href="Common.css" type="text/css" rel="stylesheet" />
</head>
<body onload="pwdsetfocus();SetWindowName();" class="loginBody">
    <form id="wfmLogin" runat="server">
    <script type="text/javascript">
        var sgtForm = document.forms['wfmLogin'];
        if (!sgtForm) {
            sgtForm = document.wfmLogin;
        }
        function pwdsetfocus() {
            sgtForm.lgnBase_Password.focus()
        }
        function SetWindowName() {
            if (document.getElementById("hfldLoginWindowName").value !== "") {
                window.name = document.getElementById("hfldLoginWindowName").value;
            }
        }
    </script>
        <div id="wrap">
            <div class="bannerBg"></div>
            <div class="loginBg">
                <div id="midContent">
                    <div class="midContentBoxLt">
                        <div class="midBoxContent">
                            <h1>Welcome to MPI :</h1>
                                <p>
                                    We are pleased to provide you with the Motion Picture Industry (MPI) Pension and Individual Account Plans 2011 Summary Plan Description (SPD). 
                                    The SPD will provide you with highlights of your pension benefits, including the changes that have been adopted since the 2005 SPD was published.
                                </p>
                        </div>
                        <div class="midBoxBlue">
                            <p>Private Policy &amp; Disclaimer | MPI &copy; 2011</p>
                            <%--<p>Click the <span>New User</span> link to register with OPUS application</p>--%>
                        </div>
                    </div>
                    <div class="midContentBoxRt">
                        <div class="midBoxContent">
                            <h1>User Login:</h1>
                            <asp:Login TitleText="" ID="lgnBase" runat="server" OnAuthenticate="lgnBase_Authenticate"
                                LoginButtonImageUrl="Image/log_button.png" DisplayRememberMe="False" LabelStyle-CssClass="loginTxt"
                                LoginButtonType="Image" LoginButtonStyle-CssClass="loginButton" TextBoxStyle-CssClass="loginTxtBox">
                            </asp:Login>
                            <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
                        </div>
                    <div class="midBoxBlue">
 <%--                      <p>
                           <a class="textLinks" href="#">New User</a>
                           <a class="textLinks" href="#">Forgot User Name</a>
                        </p>--%>
                    </div>
                    </div>
<%--
                    <div class="loginFooter">
                        <div class="loginFooterCenter">
                            <p>Private Policy &amp; Disclaimer | MPI &copy; 2011</p>
                        </div>
                    </div>
--%>
                </div>
            </div>
        </div>
    </form>
</body>
</html>

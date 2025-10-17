<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>OPUS - One Pension Unified System Login</title>
    <link href="../Styles/login.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet"> 
    <link href="https://fonts.googleapis.com/css?family=Inter" rel="stylesheet">   
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>
</head>
<body class="login-page-Bg">
    <% using (Html.BeginForm(new { astrReturnUrl = ViewBag.ReturnUrl }))
        { %>
    <%: Html.AntiForgeryToken() %>
    <div class="login-container">
        <div class="login-wrapper">
            <div class="content">
                <div class="logo">
                    <img src="../Images/TopInnerHeaderLogo.png" alt="Logo" />
                </div>
                <h2>User Login</h2>
                <div class="loginContent">
                    <span class="text-input username-input login-row">
                        <span class="username"></span>
                        <%: Html.TextBoxFor(m => m.UserName,new { @autocomplete = "off",@maxlength="30", @placeholder = "Username",}) %>
                        <%: Html.HiddenFor(m=> m.LoginWindowName) %> 
                    </span>
                    <span class="password-input login-row">
                        <span class="password"></span>
                        <%: Html.PasswordFor(m => m.Password,new { @autocomplete = "off", @maxlength="30", @placeholder = "Password" }) %>   
                    </span>              
                      <div class="logerror login-errors">
                        <%= Model.Message %>
                        <%: Html.ValidationMessageFor(m => m.UserName) %>
                           <%: Html.ValidationMessageFor(m => m.Password) %>                                                        
                    </div> 
                    <input id="Submit1" type="submit" class="login-btn" value="Login" />
                    <%--<p class="remember-text">
                        <label><%: Html.CheckBox("RememberMe", Model.CustomField1 == "true" ? true:false)%> Remember Me </label>
                    </p>
                    <span>
                        <p class="remember-text">
                            <% if(Model.ReferenceID == 0)
                                {%>
                            <%=Html.ActionLink("Forgot Password", "ActivateUser", "Account", routeValues:new { p= "SagiResetPassword" }, htmlAttributes:null)%>
                            <%}
                            %>
                        </p>
                    </span>--%>
                    <div class="loginFooter">
                        <p>
                            Private Policy & Disclaimer | MPI © 2024
                        </p>
                    </div>
                </div>
            </div>
        </div>
     </div>
     <!--Below div Changes Added as part of Framework changes 6.0.7.0 version on 07 may 2019-->
    <%:Html.HiddenFor(m=>m.IsCaptchaRequired) %>
    <div class="login-row" id="captchacontrol" style="display: none">
        <%--<img  src="x" id="captchadispaly_img" />--%>
        <img src="../Images/refresh.jfif" id="refresh_img" style="cursor: pointer; vertical-align: top; margin-top: 6%" />
        <%--<img  id="audio_img" style="cursor: pointer; position: relative; right: 6%" />--%>
        <%: Html.HiddenFor(m => m.SessionPreserveCaptcha) %>
        <%: Html.HiddenFor(m => m.EncryptedCaptchaText) %>
        <%: Html.HiddenFor(m => m.Formname) %>
        <audio id="speak"></audio>
        <p>
            <%: Html.TextBoxFor(m => m.CaptchaTextByUser,new { @autocomplete = "off", @placeholder = "Captcha", oncut="return false", oncopy= "return false", onpaste="return false" ,  @title = "Password" , @name="CaptchatextByUser",@style="width: 35%;margin-right:5%" }) %>
        </p>
    </div>

    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>" />

    <script type="text/javascript">
        var username = document.getElementById("UserName").value;
        if (username == "") {
            document.getElementById("UserName").focus();
        }
        else {
            document.getElementById("Password").focus();
        }
        var Language = "en-US";
        sessionStorage.clear();
        nsCommon.SetWindowName();

        $('#Password').keypress(function (e) {
            if (event.keyCode == 60)
                return false;
            else
                return true;
        });

        //Changes Added as part of Framework changes 6.0.7.0 version on 07 may 2019
        var IsCaptchaRequired = $("#IsCaptchaRequired").val() === "False" ? false : true;
        if (!IsCaptchaRequired) {
            $("#captchacontrol").remove();
        }
        $(function () {
            $("[data-valmsg-for='UserName']").text() != "" ? $("[data-valmsg-for='UserName']").text("Username is required.") : "";
            $("[data-valmsg-for='Password']").text() != "" ? $("[data-valmsg-for='Password']").text("Password is required.") : "";
        }) 
        setInterval(function () { location.reload(); }, 900000);
    </script>

    <% } %>
        <%--<script src="<%=Url.Content("~/js/jquery.min.js")%>"></script>--%>
</body>

</html>

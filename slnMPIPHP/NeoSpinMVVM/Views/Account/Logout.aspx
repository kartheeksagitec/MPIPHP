<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="NeoSpin Pension Solution Logout" />
    <title>NeoSpin Solution</title>
    <script>
        //Not to go back once user click on Browser back buttonafter logout.
        window.location.hash = "no-back-button";
        window.location.hash = "Again-No-back-button";   //For Chrome as chrome is not recognizing the no-back-button
        window.onhashchange = function () {
            window.location.hash = "no-back-button";
        }
    </script>
    <style type="text/css">
        * {
            margin: 0px;
            padding: 0px;
        }

        .logout-bg {
            background: url('../Images/login-Bg.png') no-repeat;
            background-size: cover;
        }

        .logout-wrapper {
            max-width: 600px;
            height: auto;
            padding: 20px;
            box-sizing: border-box;
            color: #555555;
            margin: 200px auto;
            background: #d0d0d0;
            text-align: center;
            border-radius: 10px;
        }

            .logout-wrapper a {
                color: #555555;
            }

        h1 {
            margin-bottom: 15px;
        }
    </style>
</head>
<body class="logout-bg">
    <noscript>
        Javascript is not enabled on the browser, please enable it and try again.
    </noscript>
    <% using (Html.BeginForm(new { astrReturnUrl = ViewBag.ReturnUrl }))
        { %>
    <%: Html.AntiForgeryToken() %>

    <div class="logout-wrapper">
        <h1>You have successfully logged out of the system.</h1>

        Click here to <a href="Login">login</a>

        <%--<input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>" />

        <input id="LoginWindowName" name="LoginWindowName" type="text" value="<%=Convert.ToString(Model.iobjSessionData["WindowName"]) %>" style="display: none" />--%>

        <div class="err">
        <%--    <%= Model.Message %>--%>
        </div>

    </div>

    <script type="text/javascript">
        sessionStorage.clear();
    </script>

    <% } %>
</body>

</html>

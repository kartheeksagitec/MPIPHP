
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>
<!DOCTYPE html>
<html>
<head>
    <title>Login Page</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>

</head>
<body class="LoginBg" >



    <% using (Html.BeginForm(new { astrReturnUrl = ViewBag.ReturnUrl }))
       { %>
        <%: Html.AntiForgeryToken() %>
        <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>" />

        <%: Html.HiddenFor(m=> m.LoginWindowName) %> 
            <script type="text/javascript">
                nsCommon.SetWindowName();
            </script>
        
    <% } %>
       
    <script src="<%=Url.Content("~/js/jquery.min.js")%>"></script>


    <script>sessionStorage.clear(); document.location.href = "<%=Model.Password%>";</script> 

    <%--<script type="text/javascript">
        sessionStorage.clear();
        document.forms[0].submit();
    </script>--%>
    
</body>

</html>

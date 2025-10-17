<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>

<!DOCTYPE html>
<html>
<head>
    <title>Render BPM Map</title>

    <%:@System.Web.Optimization.Styles.Render("~/BPMExecution/Styles/kendo.common.min.css") %>
    <%:@System.Web.Optimization.Styles.Render("~/BPMExecution/Styles/kendo.default.min.css")%>
    <%:@System.Web.Optimization.Styles.Render("~/BPMExecution/Styles/App.css") %>
    <%:@System.Web.Optimization.Styles.Render("~/BPMExecution/Styles/jquery.contextMenu.css")%>
    <%:@System.Web.Optimization.Scripts.Render("~/BPMExecution/Scripts/jquery.min.js")%>
    <%:@System.Web.Optimization.Scripts.Render("~/BPMExecution/Scripts/kendo.ui.core.min.js")%>
    <%:@System.Web.Optimization.Scripts.Render("~/BPMExecution/Scripts/Plane.js") %>
    <%:@System.Web.Optimization.Scripts.Render("~/BPMExecution/Scripts/DataContext.js")%>
    <%:@System.Web.Optimization.Scripts.Render("~/BPMExecution/Scripts/DataServ.js")%>
    <%:@System.Web.Optimization.Scripts.Render("~/BPMExecution/Scripts/jquery.contextMenu.js")%>
    <%:@System.Web.Optimization.Scripts.Render("~/BPMExecution/Scripts/jquery.ui.position.js")%>
</head>
<body onload="">
    <% using (Html.BeginForm(new { ReturnUrl = ViewBag.ReturnUrl }))
       { %>
    <%: Html.AntiForgeryToken() %>
    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>" />
     <%: Html.HiddenFor(m=> m.LoginWindowName) %>
    <div id="map">
        <!--<button title="Demo" id="btnDemo">Demo</button>

        <button title="Run" id="btnRun">Run</button>-->

        <div id="Container">
            <canvas class="canvasclass" id="myCanvas" width="3000" height="2500" ondblclick="OnDoubleClick(event)"></canvas>
        </div>
    </div>

    <div id="page-loader" style="display: none;" class="preloader">
        <table width="100%" height="100%">
            <tr>
                <td style="vertical-align: middle; text-align: center">
                    <img alt="loader" src="../images/301.gif">
                </td>
            </tr>

        </table>
    </div>

    <div id="variables" style="display: none;">
        <table id="tableVariables" class="variablesTable">
            <tr>
                <th class="title">ID</th>
                <th class="title">Data Type</th>
                <th class="title">Value</th>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        $(function () {
            ns.SiteName = "/" + location.pathname.split("/")[1];

            if (document.getElementById("LoginWindowName").value !== "") {
                window.name = document.getElementById("LoginWindowName").value;
                sessionStorage.setItem("LoginWindowName", window.name);
            }
            Render();
            $.contextMenu({
                selector: 'canvas',
                callback: function (key, options, e) {
                    if (key == "showVariables") {
                        y = options.$menu.css("top");
                        x = options.$menu.css("left");
                        showVariables(options.$trigger[0], x.replace("px", ""), y.replace("px", ""));
                    }
                },
                items: {
                    "showVariables": { name: "Show Variables" },
                }
            });
            
        });

    </script>


    <% } %>
</body>

</html>

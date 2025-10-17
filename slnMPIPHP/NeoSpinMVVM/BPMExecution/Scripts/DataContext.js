

var nsRequest = {
    AjaxRequest: function (reqObject) {

        if (reqObject.Param === undefined) {
            reqObject.Type = "GET";
        } else {
            reqObject.Type = "POST";
        }
        ns.LastAction = reqObject.Action;
        ns.IsRun = reqObject.IsRun;
        $.ajax({
            url: ns.SiteName + "/api/Map/" + reqObject.Action,
            async: true,
            data: JSON.stringify(reqObject.Param),
            dataType: "json",
            type: reqObject.Type,
            headers: GetHeaders(),
            cache: false,
            contentType: "application/json; charset=utf-8",
            success: function (data, textStatus, xhr) {
                ns.sessionStartTime = new Date().getTime();
                ns.deferred.resolve(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR.responseText);
                if (jqXHR.status === 403) {
                    ns.logoutSesssion();
                }
                else {
                    alert(textStatus + "  " + errorThrown);
                }
                ns.deferred.reject();
            },
            complete: function (xhr, status) {
                ns.sessionStartTime = new Date().getTime();
                if (xhr.status === 401) {
                    ns.logoutSesssion();
                }
            }
        });
    }
};

function GetHeaders()
{
    var WindowName = window.name;

    if (WindowName == "" || WindowName == undefined) {
        var lsrtTempName = sessionStorage.getItem("LoginWindowName");
        if (lsrtTempName != null) {
            WindowName = lsrtTempName;
            window.name = WindowName;
        }
    }
    sessionStorage.removeItem("LoginWindowName");
    var RequestVerificationToken = $("#antiForgeryToken").val();
    var Header = {
        'RequestVerificationToken': RequestVerificationToken,
        'WindowName': WindowName
    }
    return Header;
}
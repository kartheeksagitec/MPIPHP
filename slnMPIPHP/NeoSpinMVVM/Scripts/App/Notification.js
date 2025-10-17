var nsSessionNotify = nsSessionNotify || {};
nsSessionNotify = {
    warn_second: 30,
    StartSessionTimer: function (timeout_minute) {
        InitSessionTimer(nsSessionNotify.warn_second, timeout_minute);
    }    
};

function InitSessionTimer(warn_second, timeout_minute) {
    min = 60000;
    timeout_sec = timeout_minute * min;         
    warn_sec = (timeout_sec- (warn_second *  1000));            
    show_warning = true;
    ns.sessionStartTime = new Date().getTime();
    timeoutCounter = 0;
    CheckSessionStatus();   
}

function CheckSessionStatus() {    
    var RemainTimeSec = 0;
    currentTime = new Date().getTime();
    
    if (currentTime >= ns.sessionStartTime + warn_sec && currentTime < ns.sessionStartTime + timeout_sec) {
        if (show_warning == false) {
            timeoutCounter = timeoutCounter + 1;
            RemainTimeSec = (timeout_sec - warn_sec) / 1000;
            RemainTimeSec = RemainTimeSec - timeoutCounter;
            ShowTimeoutWarning(RemainTimeSec, true);
            down = setTimeout("CheckSessionStatus();", 1000);
        }
        else {
            show_warning = false; 
            timeoutCounter = 0;            
            RemainTimeSec = (timeout_sec - warn_sec) / 1000;
            ShowTimeoutWarning(RemainTimeSec, true);
            down = setTimeout("CheckSessionStatus();", 1000);
        }
    } else if (currentTime >= ns.sessionStartTime + timeout_sec) {
        ShowTimeoutWarning(1, false);
        timeoutCounter = 0;
        window.location.href = ns.SiteName + "/account/logout";
    } else {
        down = setTimeout("CheckSessionStatus();", 1000);
        ShowTimeoutWarning(1, false)
        timeoutCounter = 0;
    }
}

function ShowTimeoutWarning(RemainTimeSec,showHide) {
    var SessionExpireWindow = $('#SessionExpired');
    if (SessionExpireWindow.length > 0) {
        $("#SessionExpired #spnRemainTimeSec").text(RemainTimeSec);
        if (!SessionExpireWindow.data("kendoWindow")) {
            SessionExpireWindow.kendoWindow({
                width: "400px",
                height: "150px",
                actions: [],
                visible: false,
                modal: false,
                title: "Your Session is about to Expire!"
            });            
        }
        if (showHide === false) {
            SessionExpireWindow.data("kendoWindow").close();
        }
        else {
            SessionExpireWindow.data("kendoWindow").open().center();
        }        
    }        
}


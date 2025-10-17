//for balloon tooltip
//on page load (as soon as its ready) call Script_Init

var ids = {
    FormName: "",
    GridID: ""
}

function $$(id, context) {
    var el = $("#" + id, context);
    if (el.length < 1)
        el = $("[id$=_" + id + "]", context);
    return el;
}


$(document).ready(Script_Init);
function Script_Init() {
    $.ajaxSetup({ async: false });
    $.getScript('Scripts/tooltip/wz_tooltip.js');
    $.getScript('Scripts/tooltip/tip_centerwindow.js');
    $.getScript('Scripts/tooltip/tip_balloon.js');
    $.getScript('Scripts/tooltip/tip_followscroll.js');
    $.ajaxSetup({ async: true });

   
}

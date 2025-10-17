$(function () {
    // Reference the auto-generated proxy for the hub.  
    var chat = $.connection.sagitecSignalRHub;
    if (chat != undefined) {
        // Create a function that the hub can call back to display messages.
        chat.client.receiveLeftPanelMessage = function (message) {
            // Add the message to the page. 
            var n = noty({
                text: message,
                type: 'information',
                dismissQueue: true,
                layout: 'top',
                theme: 'defaultTheme'
            });
            ns.BuildLeftForm("wfmBPMWorkflowCenterLeftMaintenance");
        };

        chat.client.refreshLeftPanel = function () { ns.BuildLeftForm("wfmBPMWorkflowCenterLeftMaintenance"); };
    }
    // Start the connection.
    $.connection.hub.start().done(function () {
    });
});
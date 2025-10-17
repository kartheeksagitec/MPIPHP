var nsWpsUDF = {};

nsWpsUDF = {
};

function IsMachineRegistrationTurningOffMSS() {
    // MSS
    $(document).on("click", "#chkMachineRegistrationIndMSS", function (e) {
        if (!$('#chkMachineRegistrationIndMSS').is(':checked'))
            return confirm("Are you sure you want to turn the Machine Registration Check off? It is strongly not recommended. Press 'OK' to continue or 'Cancel' to return to the Web Portal Administration Configuration screen.");
    });
}

function IsMachineRegistrationTurningOffESS() {
    // Date: 22nd September 2020
    // Iteration: Iteration 7
    // Developer: Animesh Patni.
    // Comment : Validation Popup on Web Portal Admin Screen when user unchecks the machine registration radio button. {PIR 2145}.
    // ESS
    $(document).on("click", "#chkMachineRegistrationIndESS", function (e) {
        if (!$('#chkMachineRegistrationIndESS').is(':checked'))
            return confirm("Are you sure you want to turn the Machine Registration Check off? It is strongly not recommended. Press 'OK' to continue or 'Cancel' to return to the Web Portal Administration Configuration screen.");
    });
}


nsUserFunctions = Object.assign(nsUserFunctions, nsWpsUDF);
nsWpsUDF = undefined;

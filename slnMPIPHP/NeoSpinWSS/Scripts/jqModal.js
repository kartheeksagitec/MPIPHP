$(document).ready(function () {
    $("#btnShowSimple").click(function (e) {
        ShowDialog(false);
        e.preventDefault();
    });

    $("#btnShowModal").click(function (e) {
        ShowDialog(true);
        e.preventDefault();
    });

    $("#btnCancel").click(function (e) {
        HideDialog();
        e.preventDefault();
    });

    $("#btnSubmit").click(function (e) {
        var brand = $("#brands textarea#txtReason").val();
        $("#output").html("<b>Your favorite mobile brand: </b>" + brand);
        HideDialog();
        e.preventDefault();
    });

});

function ShowDialog(modal) {
    $("#overlay").show();
    $("#dialog").fadeIn(300);

    if (modal) {
        $("#overlay").unbind("click");
    }
    else {
        $("#overlay").click(function (e) {
            HideDialog();
        });
    }
}

function HideDialog() {
    $("#overlay").hide();
    $("#dialog").fadeOut(300);
} 
        
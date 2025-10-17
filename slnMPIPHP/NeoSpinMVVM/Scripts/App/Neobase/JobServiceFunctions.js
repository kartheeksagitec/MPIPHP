var nsJobServiceUDF = {};

nsJobServiceUDF = {
    CancelClickHandler: function (e) {
        var grid = $(e.currentTarget).closest("table");
        //Show confirm popup if atleast one record is selected
        //if (grid.find("input:checkbox").length > 1 && grid.find("input:checkbox:checked").length > 0 && grid.find("input:checkbox:checked").closest("tr").find("td:nth-child(4)").text() == 'Queued')
        if (grid.find("input:checkbox").length > 1 && grid.find("input:checkbox:checked").length > 0) {
            return confirm(e.context.idictParam.param0 ? e.context.idictParam.param0 : "Type your message here.");
        }
        else {
            return true;
        }
    },
};

nsUserFunctions = Object.assign(nsUserFunctions, nsJobServiceUDF);
nsJobServiceUDF = undefined;

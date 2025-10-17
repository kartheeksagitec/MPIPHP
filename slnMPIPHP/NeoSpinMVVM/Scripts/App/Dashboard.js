
var nsDash = nsDash || {};

nsDash = {
    availableList: {

    },
    ShowSelectedWidgets: function () {
        var DashBoard = $("#DashboardList");
        for (var i in nsDash.availableList) {
            nsDash.availableList[i] = "false";
        }

        var widgetSettings = ns.WidgetData.WidgetSettings.toJSON();
        var WidgetList = ns.WidgetData.WidgetList.toJSON();

        if (WidgetList !== undefined && widgetSettings !== undefined) {
            for (var i in WidgetList) {
                var formname = WidgetList[i];
                if (nsDash.availableList[formname] == undefined) {
                    ns.WidgetData.WidgetList.splice(i, 1);
                    delete ns.WidgetData.WidgetSettings[formname];
                    //DashBoard.find("#" + formname + "0").remove();
                }
            }

            widgetSettings = ns.WidgetData.WidgetSettings.toJSON();
            WidgetList = ns.WidgetData.WidgetList.toJSON();

            var temp, i = 0, j = 0;

            //sorting the widgetlist as per the index sequence in widgetsettings
            //This code is used when we do not have columns structure in Dashboard
            for (i = 0 ; i < (WidgetList.length - 1) ; i++) {
                for (j = 0 ; j < WidgetList.length - i - 1; j++) {
                    if (widgetSettings[WidgetList[j + 1]] !== undefined) {
                        if (widgetSettings[WidgetList[j]].index > widgetSettings[WidgetList[j + 1]].index) {
                            temp = WidgetList[j];
                            WidgetList[j] = WidgetList[j + 1];
                            WidgetList[j + 1] = temp;
                        }
                    }
                }
            }

            for (var i in WidgetList) {
                var formname = WidgetList[i];
                nsDash.availableList[formname] = "true";
                var ColumnToAddWidget = nsDash.GetCoulmnToAddWidget();
                if (DashBoard.find("#" + formname).length == 0 && DashBoard.find("#" + formname + "0").length == 0) {
                    var html = "<div style='float:left; height:50px; width:200px; border:1px solid red' id='" + formname + "'><h1>" + formname + "</h1><div>This is test data for widget</div></div>";

                    if (formname.indexOf("wfm") == 0) {
                        ns.viewModel.currentModel = formname;
                        ns.viewModel.currentForm = formname;
                        var data = ns.getTemplate(formname);
                        html = "";

                        var widget = $("#WidgetTemplate").clone();
                        if (formname.indexOf("Lookup") == -1)
                            widget.attr("id", formname + "0");
                        else
                            widget.attr("id", formname);

                        widget.attr("formname", formname);
                        widget.append("<div class='contents'>" + "<div id='" + formname + "ErrorDiv' class='ErrorDiv'></div>" + data.Template + "</div>");
                        widget.find(".div-header-text").html(data.ExtraInfoFields.FormTitle);
                        widget.show();

                        nsDash.ResizeWidget(widget);
                        nsDash.AddWidgetToColumn(ColumnToAddWidget, widget, formname);

                        if (formname.indexOf("Lookup") >= 0) {
                            ns.Templates[formname].HeaderData = kendo.observable(ns.Templates[formname].HeaderData);
                            kendo.bind($(".DashboardColumn #" + formname)[0], ns.Templates[formname].HeaderData);
                        }
                        //else {
                        //    ns.Templates[formname].ClientVisibility = kendo.observable(ns.Templates[formname].ClientVisibility);
                        //    ns.Templates[formname].ControlAttribites = kendo.observable(ns.Templates[formname].ControlAttribites);
                        //    ns.Templates[formname].ExtraInfoFields = kendo.observable(ns.Templates[formname].ExtraInfoFields);
                        //}

                        var setting = ns.WidgetData.WidgetSettings[formname];
                        if (setting !== undefined) {
                            widget.width(setting.width);
                            widget.height(setting.height);
                            widget.find(".div-header-text").html(setting.title);
                            widget.find(".div-header").css("background-color", setting.color);
                        }

                        if (formname.indexOf("Lookup") == -1)
                            nsDash.getDashboardData();
                        //raiseEvent(nsDash.getData);

                    } else {

                        var widget = $("#WidgetTemplate").clone();

                        if (formname.indexOf("Lookup") == -1)
                            widget.attr("id", formname + "0");
                        else
                            widget.attr("id", formname);
                        widget.attr("formname", formname);
                        widget.append("<div class='contents'>" + "<div id='" + formname + "ErrorDiv' class='ErrorDiv'></div><h1>" + formname + "</h1><div>This is test data for widget</div></div>");
                        if (ns.WidgetData.WidgetSettings[formname] !== undefined)
                            widget.find(".div-header-text").html(ns.WidgetData.WidgetSettings[formname].title);
                        else
                            widget.find(".div-header-text").html(formname);
                        widget.show();
                        nsDash.AddWidgetToColumn(ColumnToAddWidget, widget, formname);
                    }
                }
            }
        }

        for (var i in nsDash.availableList) {
            var formname = i;
            //if (formname.indexOf("wfm") == 0) {
            //    if (formname.indexOf("Lookup") == -1)
            //        widget.attr("id", formname + "0");
            //    else
            //        widget.attr("id", formname);
            //}

            if (nsDash.availableList[i] !== "true") {
                if (formname.indexOf("Lookup") == -1 && DashBoard.find("#" + formname + "0").length > 0)
                    DashBoard.find("#" + formname + "0").remove();
                else if (DashBoard.find("#" + formname).length > 0)
                    DashBoard.find("#" + formname).remove();
            }
        }

        $("#wfmDashBoard").find("input,select,a")
        .bind('mousedown.ui-disableSelection selectstart.ui-disableSelection', function (e) {
            e.stopImmediatePropagation();
        });

        nsCommon.SetTitle("Dashboard");
    },

    AddWidgetToColumn: function (ColumnToAddWidget, widget, formname) {
        if (ns.WidgetData.WidgetSettings[formname] !== undefined) {
            if (ns.WidgetData.WidgetSettings[formname].index !== undefined) {
                //var colindex = ns.WidgetData.WidgetSettings[formname].colindex;
                var index = ns.WidgetData.WidgetSettings[formname].index;
                //ColumnToAddWidget = $($(".DashboardColumn")[colindex]);
                ColumnToAddWidget.append(widget);
                //if (index === 0) {
                //    ColumnToAddWidget.prepend(widget);
                //}
                //else {
                //    //if (ColumnToAddWidget.find(">div:nth-child(" + index + ")").length == 0) {
                //       ColumnToAddWidget.append(widget);
                //}
                //else
                //  ColumnToAddWidget.find(">div:nth-child(" + index + ")").before(widget);
                //}
            }
            else
                ColumnToAddWidget.append(widget);
        }
        else
            ColumnToAddWidget.append(widget);
    },

    placeholder: function (element) {
        return element.clone().addClass("placeholder");
    },

    hint: function (element) {
        return element.clone().addClass("hint")
                    .height(element.height())
                    .width(element.width());
    },

    GetCoulmnToAddWidget: function () {
        // get column to add widget
        var columnToReturn = $(".DashboardColumn");
        //$(".DashboardColumn").each(function (e) {
        //    if (columnToReturn.find(">div").length > $(this).find(">div").length) {
        //        columnToReturn = $(this)
        //    }
        //});
        return columnToReturn;
    },

    LoadDashBoard: function () {
        $(".DashboardColumn").empty();
        nsDash.availableList = {};
        ns.deferred = $.Deferred();
        var PredefinedWidgetData = JSON.parse(nsCommon.SyncPostDefered("GetPredefinedWidgets"));
        if (PredefinedWidgetData !== undefined) {
            for (var i in PredefinedWidgetData)
                nsDash.availableList[PredefinedWidgetData[i].NAME] = PredefinedWidgetData[i].DESCRIPTION;
        }
        //nsDash.availableList = {
        //    "wfmPersonMaintenance": "Person Maintenance",
        //    "wfmOrganizationMaintenance": "Organization Maintenance"
        //}
        var WidgetData = nsCommon.SyncPostDefered("GetDashboardSettings");
        if (WidgetData !== null) {
            WidgetData = eval("(" + WidgetData + ')');
        }
        else {
            WidgetData = {
                WidgetList: [],
                WidgetSettings: {}
            }
        }

        ns.WidgetData = kendo.observable(WidgetData);
        var headlineFile = nsCommon.SyncPost("GetAgencyHeadlines?astrFileUploadPath=" + "FileUpload");
        $('#agencyHeadLines').hide();
        if (headlineFile != "") {
            $('#agencyHeadLines').show();
            var newIframe = document.getElementById('frameHeadline');
            newIframe.contentWindow.document.open('text/htmlreplace');
            newIframe.contentWindow.document.write(headlineFile);
            newIframe.contentWindow.document.close();
        }

        if (!ns.blnUseSlideoutForLookup) {
            $("#LookupName").hide();
            $("#wfmDashBoard #WidgetList").hide();
            $("#wfmDashBoard #WidgetListSettings").hide();
            showDiv("#wfmDashBoard");
            $("#crumDiv").hide();
        }

        var CheckboxListHtml = "";
        for (var i in nsDash.availableList) {
            var name = nsDash.availableList[i];
            if (name !== "") {
                CheckboxListHtml = CheckboxListHtml + "<input data-bind='checked: WidgetList' type='checkbox' value='" + i + "'/> " + name + "<br />"
            }
            else {
                CheckboxListHtml = CheckboxListHtml + "<input data-bind='checked: WidgetList' type='checkbox' value='" + i + "'/> " + i + "<br />"
            }
        }

        $("#wfmDashBoard #WidgetList").html(CheckboxListHtml);

        kendo.bind($("#wfmDashBoard #WidgetList")[0], ns.WidgetData.WidgetList);

        $(".DashboardColumn").each(function () {
            $(this).sortable(function () {
            });
        });

        nsDash.ShowSelectedWidgets();
        //exapand
        $("#wfmDashBoard").off("click", ".icon-maximize");
        $("#wfmDashBoard").on("click", ".icon-maximize", function (e) {
            var widget = $(e.target).closest(".widget");
            var contentElement = widget.find(".contents");
            $(e.target)
                .removeClass("icon-maximize")
                .addClass("icon-minimize");
            contentElement.slideDown("fast", function () {
                nsDash.ResizeWidget(widget);
                widget.height($(e.target).find('input').val());
                widget.resizable("enable");
            });
        });
        //collapse
        $("#wfmDashBoard").off("click", ".icon-minimize");
        $("#wfmDashBoard").on("click", ".icon-minimize", function (e) {
            var widget = $(e.target).closest(".widget");
            var contentElement = widget.find(".contents");
            $(e.target)
                .removeClass("icon-minimize")
                .addClass("icon-maximize");
            if (widget.find(".settings").is(":visible")) {
                $(e.target).find('input').val(widget.height() - widget.find(".settings").height());
                widget.find(".settings").hide();
            }
            else
                $(e.target).find('input').val(widget.height());

            contentElement.slideUp("fast", function () {
                widget.height(widget.find('.div-header').height() + 10);
                widget.resizable("destroy");
            });
        });

        //exapand / collapse settings
        $("#wfmDashBoard").off("click", ".icon-settings");
        $("#wfmDashBoard").on("click", ".icon-settings", function (e) {
            var widget = $(e.target).closest(".widget");
            var settings = widget.find(".settings");
            var title = widget.find(".div-header-text").text();
            widget.find(".widgettitle").val(title);
            var widgetHeight = widget.height();
            settings.slideToggle("fast", function () {
                if ($(this).is(":visible")) {
                    widget.height(widgetHeight + widget.find(".settings").height());
                } else {
                    widget.height(widgetHeight - widget.find(".settings").height());
                }
            });
        });

        $("#wfmDashBoard").off("click", ".dashboard-settings")
        $("#wfmDashBoard").on("click", ".dashboard-settings", function (e) {
            $("#wfmDashBoard #WidgetList").toggle();
            $("#wfmDashBoard #WidgetListSettings").toggle();
        });

        //refresh Widget
        $("#wfmDashBoard").off("click", ".icon-refresh");
        $("#wfmDashBoard").on("click", ".icon-refresh", function (e) {
            var widget = $(e.target).closest(".widget");
            var formname = widget.attr("formname");
            ns.viewModel.currentModel = formname;
            ns.viewModel.currentForm = formname;
            raiseEvent(nsDash.getData);
        });

        $("#wfmDashBoard").off("click", ".icon-close");
        $("#wfmDashBoard").on("click", ".icon-close", function (e) {
            var widget = $(e.target).closest(".widget");
            var formname = widget.attr("formname");
            var title = widget.find(".div-header-text").text();
            var result = confirm(title + " widget will be removed.");
            if (result) {
                var checkbox = $("#WidgetList").find("input[value='" + formname + "']");
                if (checkbox[0] != undefined) {
                    checkbox[0].checked = false;
                }
                checkbox.trigger("change");
                delete ns.WidgetData.WidgetSettings[formname];
                widget.remove();
            }
        });
    },

    ResizeWidget: function (element) {
        element.resizable({
            maxHeight: $(".DashboardColumn").height(),
            maxWidth: $(".DashboardColumn").width(),
            resize: function (event, ui) {
                var headerWidth = parseInt($(this).find('.div-header-text').width()) + parseInt($(this).find('.div-header-icon').width()) + 25;
                minwidth = $(this).find("table[id*='Main']").outerWidth(true) > 365 ? $(this).find("table[id*='Main']").outerWidth(true) : 365;
                minheight = $(this).find("table[id*='Main']").outerHeight(true) + 38;
                //ui.size.height = ui.size.height > maxheight ? maxheight : ui.size.height < minheight ? minheight: ui.size.height;
                ////if (ui.size.height > maxheight)
                ////    ui.size.height = maxheight;
                if (ui.size.height < minheight)
                    ui.size.height = minheight;

                //ui.size.width = ui.size.width > maxwidth ? maxwidth : ui.size.width < minwidth ? minwidth : ui.size.width;
                ////if (ui.size.width > maxwidth)
                ////    ui.size.width = maxwidth
                if (ui.size.width < minwidth)
                    ui.size.width = minwidth;

            }
            //    //stop: function (event, ui) {
            //    //    //var theDiv= $(this).find("table[id^='Main']")
            //    //    //var totalWidth = theDiv.outerHeight(true);
            //    //    //totalWidth += parseInt(theDiv.css("padding-top"), 10) + parseInt(theDiv.css("padding-bottom"), 10); //Total Padding Width
            //    //    //totalWidth += parseInt(theDiv.css("margin-top"), 10) + parseInt(theDiv.css("margin-bottom"), 10); //Total Margin Width
            //    //    //totalWidth += parseInt(theDiv.css("borderTopWidth"), 10) + parseInt(theDiv.css("borderBottomWidth"), 10);

            //    //    alert($(this).find("table[id^='Main']").outerHeight(true) + "-" + ui.size.height);
            //    //}
        });
    },

    SetHeaderColor: function (element) {
        var color = $(element).css("background-color");
        $(element).closest(".widget").find(".div-header").css("background-color", color);
    },

    SettingsOk: function (element) {
        var widget = $(element).closest(".widget");
        var title = widget.find(".widgettitle").val();
        var color = widget.find(".div-header").css("background-color");
        widget.find(".div-header-text").text(title);

        var index = widget.index();
        var colindex = widget.closest(".DashboardColumn").index();
        var widgetName = widget.attr("formname");

        ns.WidgetData.WidgetSettings[widgetName] = {
            title: title,
            color: color,
            index: index,
            colindex: colindex
        }
        var settings = $(element).closest(".widget").find(".settings");
        settings.slideToggle("fast");
    },

    Settingscancle: function (element) {
        var settings = $(element).closest(".widget").find(".settings");
        settings.slideToggle("fast");
    },

    SaveDashBoardSettings: function () {
        ns.viewModel.currentModel = "DashBoard";
        nsDash.SetDashboardDataToSave();
        ns.deferred = $.Deferred();
        var param = { UserSettingsData: kendo.stringify(ns.WidgetData.toJSON()) }
        nsRequest.AjaxRequest({ action: "SaveDashboardSettings", "param": param });
        return ns.deferred;
    },

    SetDashboardDataToSave: function () {
        $(".DashboardColumn").each(function () {
            colindex = $(this).index();
            $(this).find(".widget").each(function () {
                formname = $(this).attr("formname");
                if (ns.WidgetData.WidgetSettings[formname] == undefined) {
                    ns.WidgetData.WidgetSettings[formname] = {};
                }

                ns.WidgetData.WidgetSettings[formname].colindex = colindex;
                ns.WidgetData.WidgetSettings[formname].width = $(this).width();
                if ($(this).find('div.icon-maximize').find('input').val() === undefined)
                    ns.WidgetData.WidgetSettings[formname].height = $(this).find(".settings").is(":visible") ? $(this).height() - $(this).find(".settings").height() : $(this).height();
                else
                    ns.WidgetData.WidgetSettings[formname].height = $(this).find('div.icon-maximize').find('input').val();

                ns.WidgetData.WidgetSettings[formname].index = $(this).index();
            });
        });
    },

    LoadDashBoardChart: function (astrFormID, activeForm, defaultChartType) {
        if (typeof defaultChartType === "undefined") { defaultChartType = "column"; }
        $("#CenterSplitter").trigger('mousedown');

        var data = ns.getTemplate(astrFormID, true);
        var lstrTitle = ns.Templates[astrFormID].ExtraInfoFields.FormTitle;
        var lstrContent = ns.Templates[astrFormID].Template;

        //get html for dashboardItem
        var NewItemHTML = "<div id='Parent" + astrFormID + "' style='width:380px; float:left'>";
        NewItemHTML += "<div id='" + astrFormID + "'>";
        NewItemHTML += "<ul controlType='panelbar' id='Panel" + astrFormID + "'>";
        NewItemHTML += "<li class='k-state-active k-state-selected'>";
        NewItemHTML += " <span>" + lstrTitle + "</span>";
        NewItemHTML += "<div>";
        NewItemHTML += "<input type='button' class='btnLookupChartConfig'>";
        NewItemHTML += "<div class='TestChart' id='DashItem" + astrFormID + "'></div>";
        NewItemHTML += "<div id='ChartCriteriaDiv" + astrFormID + "'></div>";
        NewItemHTML += "</div>";
        NewItemHTML += "</li>";
        NewItemHTML += "</ul>";
        NewItemHTML += "</div>";
        NewItemHTML += "</div>";

        var DashboardList = $("#DashboardList");

        DashboardList = $("#DashboardList").append(NewItemHTML);

        //$($(".PanelbarParent")[0]).append(NewItemHTML);
        // $(".panel-container").kendoOBSortableGrid({});
        // var NewPanelBar = $($(".PanelbarParent")[0]).find("#Panel" + astrFormID);
        var ChartCriteriaDiv = DashboardList.find("#ChartCriteriaDiv" + astrFormID);

        ChartCriteriaDiv.append("<div id='" + astrFormID + "'>" + lstrContent + "</div>").hide();

        ns.Templates[astrFormID].HeaderData = kendo.observable(ns.Templates[astrFormID].HeaderData);

        kendo.bind($("#wfmDashBoard #" + astrFormID)[0], ns.Templates[astrFormID].HeaderData);
        ns.Templates[astrFormID].ChartConfig;
        ns.applyKendoUI("#wfmDashBoard", astrFormID, astrFormID);

        var options = {
            Title: astrFormID,
            FormID: astrFormID,
            //Chart Axis
            XAxisField: "",
            YAxisField: "",
            DefaultChartType: defaultChartType,
            ChartTypes: ["column", "bar", "line"],
            Height: 350,
            Width: 350,
            ChartCriteriaDivID: "ChartCriteriaDiv" + astrFormID,
            ActiveForm: activeForm
        };

        var ChartDiv = DashboardList.find("#DashItem" + astrFormID);
        ns.Templates[astrFormID].ChartConfig = ChartDiv.SagiChart(options).data("SagiChart");

        //  setTimeout(function (e) {
        $("#wfmDashBoard #ChartCriteriaDiv" + astrFormID).find("input[value='Search']").trigger("click");
        //}, 500);
        // $('#Parent' + astrFormID).resizable();
    },

    getData: function () {
        ns.deferred = $.Deferred();
        var larrRows = nsCommon.sessionGet(ns.viewModel.currentModel + "_Params");
        ns.blnDataFromServer = true;
        //nsRequest.AjaxRequest({ action: "GetFormForOpenDashboard?astrFormID=" + ns.viewModel.currentForm, "param": larrRows });
        //return ns.deferred;
        //nsDash.bindDashboardData(ns.deferred, false);
        var ajaxdata = nsCommon.SyncPost("GetFormForOpenDashboard?astrFormID=" + ns.viewModel.currentForm, larrRows);
        nsDash.bindDashboardData(ajaxdata, false);
        nsCommon.SetTitle("Dashboard");
    },

    getDashboardData: function () {
        var larrRows = nsCommon.sessionGet(ns.viewModel.currentModel + "_Params");
        var ajaxdata = nsCommon.SyncPost("GetFormForOpenDashboard?astrFormID=" + ns.viewModel.currentForm, larrRows);
        nsDash.bindDashboardData(ajaxdata, false);
        nsCommon.SetTitle("Dashboard");
    },

    bindDashboardData: function (data, ablnRebindData) {
        var lstrFormId = data.ExtraInfoFields["FormId"];

        var NewFormid = lstrFormId;
        if (NewFormid.indexOf("wfm") < 0)// for prototype forms
            NewFormid = "wfm" + NewFormid;

        var lstrKeyField = data.ExtraInfoFields["KeyField"] === undefined ? 0 : data.ExtraInfoFields["KeyField"];

        var PrimaryKeyforTreeNode = lstrKeyField;
        var lstrModel = NewFormid + "0";

        if (data.ValidationSummary.length > 0) {
            return;
        } else {
            $("#" + lstrModel + "ErrorDiv").hide();
        }

        $("#" + lstrModel + "ErrorDiv").html("");

        if (ablnRebindData === undefined)
            ablnRebindData = false;

        if ($("#MainSplitter").css("opacity") === "0") {
            $("#pnlLoading").css("display", "none");
            $("#MainSplitter").css("opacity", 1);
        }

        var FormContainerID = "#wfmDashBoard";

        ns.viewModel["EnableSessionStore"] = false;

        ns.startBindTime = new Date().getTime();
        var lstrFormId = data.ExtraInfoFields["FormId"];

        //dka ns.destroyAll(lstrFormId);
        ns.viewModel.currentForm = lstrFormId;
        ns.viewModel.currentModel = lstrModel;
        var lstrTitle = "";
        ActiveDivID = NewFormid + lstrKeyField;

        //clear dirty data
        delete ns.DirtyData[ActiveDivID];

        var dataItem;
        dataItem = nsCommon.GetDataItemFromDivID(ActiveDivID);

        if (dataItem === undefined) {
            var lstrTitleField = ns.Templates[NewFormid].ExtraInfoFields["TitleField"];
            if (lstrTitleField !== "" && lstrTitleField !== undefined) {
                lstrTitle = data.DomainModel.HeaderData.MaintenanceData[lstrTitleField];
            } else {
                lstrTitle = ns.Templates[NewFormid].ExtraInfoFields.FormTitle;
            }
        } else {
            lstrTitle = dataItem.title;
        }
        var lstrContent = ns.Templates[NewFormid].Template;

        ActiveDivID = lstrModel;

        // Creating HTML for DIV
        var DefaultButtonID = ns.Templates[NewFormid].ExtraInfoFields.DefaultButtonID;
        var DefaultButtonAttribute = "";
        if (DefaultButtonID !== undefined) {
            DefaultButtonAttribute = " DefaultButtonID='" + DefaultButtonID + "' ";
        }
        var lstrActiveDivHtml = "<div " + DefaultButtonAttribute + " id='" + ActiveDivID + "'><div id='" + ActiveDivID + "ErrorDiv' class='ErrorDiv'></div>" + lstrContent + "</div>";

        if (ns.blnDataFromServer === true) {
            nsCommon.sessionSet(lstrModel, data);
            ns.blnDataFromServer = false;
        }

        ns.viewModel[lstrModel] = { HeaderData: {}, DetailsData: {}, ListControlData: {}, ExtraInfoFields: {}, ControlsHaveingVisibility: {}, KeysData: {} };

        data.DomainModel.HeaderData.ClientVisibility = ns.Templates[NewFormid].ClientVisibility;

        ns.viewModel[lstrModel].HeaderData = kendo.observable(data.DomainModel.HeaderData);
        ns.viewModel[lstrModel].KeysData = data.DomainModel.KeysData;
        ns.viewModel[lstrModel].ListControlData = data.DomainModel.ListControlData;

        nsVisi.BindChangeEventForClientCicibility(lstrModel);

        ns.viewModel[lstrModel].DetailsData = kendo.observable(data.DomainModel.DetailsData);
        ns.viewModel[lstrModel].ExtraInfoFields = kendo.observable(data.ExtraInfoFields);

        //adding vallidator
        ns.viewModel[lstrModel].Validator = $("#" + lstrModel).kendoValidator().data("kendoValidator");

        kendo.bind($("#FormTitle")[0], ns.Templates[NewFormid].ExtraInfoFields);

        ns.applyKendoUI(FormContainerID, ActiveDivID, lstrFormId);

        if (data.DomainModel.HeaderData !== null && data.DomainModel.HeaderData.ControlList !== undefined) {
            for (var cntrl in data.DomainModel.HeaderData.ControlList.HiddenControls) {
                if (cntrl === "" || cntrl.indexOf("@") === 0)
                    continue;

                var ControltoHide = $(FormContainerID + " #" + ActiveDivID + " #" + cntrl);
                if (ControltoHide.length > 0 && ControltoHide[0].tagName === "SELECT" && $(ControltoHide.data("kendoDropDownList")).length > 0) {
                    ControltoHide.closest(".k-widget").hide();
                }
                else if (ControltoHide.length > 0 && ControltoHide[0].tagName === "TEXTAREA" && $(ControltoHide.data("kendoEditor")).length > 0) {
                    ControltoHide.closest(".k-widget").hide();
                }
                else if (ControltoHide.length > 0 && ControltoHide[0].tagName === "INPUT" && $(ControltoHide.data("kendoNumericTextBox")).length > 0) {
                    ControltoHide.data("kendoNumericTextBox").wrapper.hide();
                }
                else if (ControltoHide.attr("controltype") == "tabdiv") {
                    var tabLinktoHide = ControltoHide[0].id + "_Header ";
                    $("Li [id^=" + cntrl + "_Header").hide()
                    ControltoHide.hide();
                }
                else
                    ControltoHide.hide();
            }
        }

        //applying client visibility to form
        //adding listner for checkbox event
        ns.viewModel[lstrModel].HeaderData["checkBoxListener"] = function (e) {
            inspectCheckBox(e.target);
        };

        nsVisi.AddClientVisibilityAttributes(FormContainerID + " #" + ActiveDivID, ns.viewModel[lstrModel].HeaderData.ClientVisibility);

        if ($(FormContainerID + " #" + ActiveDivID + " [data-bind]").length > 0)
            kendo.bind($(FormContainerID + " #" + ActiveDivID), ns.viewModel[lstrModel].HeaderData);
        //kendo.bind($(FormContainerID + " #" + ActiveDivID + " [data-bind]"), ns.viewModel[lstrModel].HeaderData);

        ns.gridBindTime = new Date().getTime();

        ns.viewModel[lstrModel].grids = [];
        for (var grid in data.DomainModel.DetailsData) {
            ns.viewModel[lstrModel].grids.push(grid);
            if (grid.indexOf("chr") == 0) {
                ns.BindChartData(data, grid);
            } else if (grid != "HeaderTemplate") {
                ns.BindGridFromData(data, grid, FormContainerID, ActiveDivID, ablnRebindData);
            }
        }

        setTimeout(function () {
            for (var grid in data.DomainModel.DetailsData) {
                ns.viewModel[lstrModel].grids.push(grid);
                if (grid.indexOf("chr") == 0) {
                } else if (grid != "HeaderTemplate") {
                    ns.HideGridColumns(data, grid, FormContainerID, ActiveDivID, ablnRebindData);
                }
            }
        }, 100);

        nsVisi.ApplyClientVisibilityToAllControls(ActiveDivID);

        ns.endBindTime = new Date().getTime();

        if (!ablnRebindData)
            ns.RestorePageFromSessionStore(ActiveDivID);
        ns.viewModel["EnableSessionStore"] = true;

        StoreTreeViewInSessionStore();
        PopulateDirtyFormList();

        ns.ApplyReadOnlyAndEnableRules(ActiveDivID);
        $("#" + ActiveDivID + " select[IsCascadingDropDown='true']").each(function () {
            $(this).trigger("change", true);
        });
        $("#ContentSplitter").trigger('mousedown');

        var fn = nsUserFunctions["AfterBindFormData"];
        if (typeof fn === 'function') {
            var Context = {
                activeDivID: ActiveDivID,
            };
            var e = {};
            e.context = Context;
            var result = fn(e);
        }

    },
}
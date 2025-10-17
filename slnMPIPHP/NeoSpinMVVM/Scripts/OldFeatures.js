

function $$(id, context) {
    var el = $("#" + id, context);
    if (el.length < 1)
        el = $("[id$=_" + id + "]", context);
    return el;
}


function InitializeAutoComplete() {
    $('input[sfwAutoQuery]').each(function () {
        // Get all attributes related to autocomplete
        var lstrQuery = $(this).attr('sfwAutoQuery');
        var lstrAutoColumns = $(this).attr('sfwAutoColumns');
        var lintMinLength = $(this).attr('sfwAutoMinLength') == undefined ? 3 : $(this).attr('sfwAutoMinLength');
        var lintDelay = $(this).attr('sfwDelay') == undefined ? 1000 : $(this).attr('sfwDelay');
        var lstrAutoFillMapping = $(this).attr('sfwAutoFillMapping');
        var lstrClientVisibilitySource = $(this).attr('sfwClientVisibilitySource');
        var lstrClientVisibility = $(this).attr('sfwClientVisibility');

        // Additional variables
        var widtharray = new Array();
        var lblnClearRelatedFields = true;
        var selfid = $(this).attr('ID').split("_");
        selfid = selfid[selfid.length - 1];
        var SelfMappingSource = "";
        var SelectedFromlist = false;

        // Total fields, (combined fields-to-display & mapping fields) used while creating autocomplete result jsonobj
        var totalFields = "";

        var arrAutoColumns = lstrAutoColumns.split(";");
        var lstrFieldstoDisplay = "";
        var lstrFieldsHeader = "";
        var lstrFieldsOperator = "";
        var lstrFieldsDataType = "";
        for (i = 0; i < arrAutoColumns.length; i++) {
            var temparry = arrAutoColumns[i].split(",");
            lstrFieldstoDisplay += "," + temparry[0];

            lstrFieldsHeader += "," + (temparry.length > 1 ? temparry[1] : temparry[0]);
            lstrFieldsOperator += "," + (temparry.length > 2 ? temparry[2] : "like");
            lstrFieldsDataType += "," + (temparry.length > 3 ? temparry[3] : "string");
        }

        lstrFieldstoDisplay = lstrFieldstoDisplay.substring(1, lstrFieldstoDisplay.length);
        lstrFieldsHeader = lstrFieldsHeader.substring(1, lstrFieldsHeader.length);
        lstrFieldsOperator = unescape(lstrFieldsOperator.substring(1, lstrFieldsOperator.length));
        lstrFieldsDataType = lstrFieldsDataType.substring(1, lstrFieldsDataType.length);

        var arrfields = lstrFieldstoDisplay.split(",");
        var arrOperators = lstrFieldsOperator.split(",");
        var arrDataTypes = lstrFieldsDataType.split(",");

        totalFields += lstrFieldstoDisplay;
        var arrmapingfields = new Object();
        if (lstrAutoFillMapping != undefined) {
            arrmapingfields = lstrAutoFillMapping.split(";");
            for (i = 0; i < arrmapingfields.length; i++) {
                totalFields += "," + arrmapingfields[i].split("=")[1];

                if (arrmapingfields[i].split("=")[0] == selfid) {
                    SelfMappingSource = arrmapingfields[i].split("=")[1];
                }
            }
        }
        var arrTotalFields = totalFields.split(",");

        $(this).autocomplete({
            source: function (request, response) {  // Source start
                $.ajax({
                    url: "SagitecWebServices.asmx/GetAutoCompleteData",
                    //url: "wfmSPA.aspx/GetAutoCompleteData",
                    data: "{ 'astrSearchCriteria': '" + request.term + "', 'astrQuery': '" + lstrQuery + "', 'astrAllColumns': '" +
                      arrTotalFields + "', 'astrDisplayColumns': '" + lstrFieldstoDisplay + "', 'astrColumnHeaders': '" +
                      lstrFieldsHeader + "', 'astrFieldOperators': '" + lstrFieldsOperator + "', 'astrFieldDataTypes': '" + lstrFieldsDataType + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function (data) { return data; },
                    success: function (data) {
                        $('.ui-autocomplete-loading').removeClass("ui-autocomplete-loading");
                        if (data.d.length <= 0) {
                            $('input[sfwAutoQuery]').autocomplete("close");
                            return;  // If result is empty
                        }
                        widtharray = GetMaxWidthArray(eval('(' + data.d + ')'), arrfields);

                        response($.map(eval('(' + data.d + ')'), function (item) {
                            // Create object containing total fields & related values
                            var jsonobj = new Object();

                            if (SelfMappingSource == "")
                                jsonobj["value"] = item[arrTotalFields[0]];  // Mostly it will be the first field.
                            else
                                jsonobj["value"] = item[SelfMappingSource];  // If assining other fields value to textbox

                            // Debugger
                            for (i = 0; i < arrTotalFields.length; i++) {

                                if (item["rowindex"] != undefined) {
                                    jsonobj["value"] = request.term;
                                    jsonobj["rowindex"] = item["rowindex"];
                                    if (i <= arrfields.length)
                                        jsonobj[arrTotalFields[i]] = item[arrTotalFields[i]];
                                }
                                else {
                                    if (item[arrTotalFields[i]].indexOf("/") > 0)  // Checking for date
                                        jsonobj[arrTotalFields[i]] = (new Date(item[arrTotalFields[i]].split(" ")[0])).format("mm/dd/yyyy");
                                    else
                                        jsonobj[arrTotalFields[i]] = item[arrTotalFields[i]];
                                }
                            }
                            return jsonobj;
                        }))
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $('.ui-autocomplete-loading').removeClass("ui-autocomplete-loading");
                        alert(textStatus);
                    }
                });
            },  // Source end
            minLength: lintMinLength,
            delay: lintDelay,
            select: function (event, ui) {  // Select start
                // Debugger
                if (ui.item["rowindex"] != undefined)
                    return false;

                var InsideGrid = false;
                var Rownumber = -1;
                Rownumber = TryParseInt($(this)[0].id.substring($(this)[0].id.lastIndexOf('_') + 1), -1);
                if (Rownumber >= 0)
                    InsideGrid = true;

                SelectedFromlist = true;
                // Mapping all values to specified controls
                if (arrmapingfields.length > 0) {
                    for (i = 0; i < arrmapingfields.length; i++) {
                        var ctrl = arrmapingfields[i].split("=");
                        if (ctrl.length == 2) {
                            if (InsideGrid) {
                                ctrl[0] = ctrl[0] + "_" + Rownumber;
                            }

                            if ($$(ctrl[0])[0].tagName.toLowerCase() == 'span') {
                                $$(ctrl[0]).text(ui.item[ctrl[1]]);
                                ApplyLabelVisibility($$(ctrl[0]));
                            }
                            else {
                                //if (ui.item[ctrl[1]].indexOf("/") > 0) {
                                //    $$(ctrl[0]).val(new Date(ui.item[ctrl[1]]).format("yyyy-mm-dd"));
                                //    alert(new Date(ui.item[ctrl[1]]).format("mm/dd/yyyy"));
                                //}
                                //else
                                $$(ctrl[0]).val(ui.item[ctrl[1]]);
                                $$(ctrl[0]).trigger("change");

                            }
                        }
                    }
                }
                $("#autocompletediv").hide();
                return true;
            },  // Select end
            change: function (event, ui) {
                if (ui.item != null && SelectedFromlist == true) {
                    return;
                }
                var InsideGrid = false;
                var Rownumber = -1;
                Rownumber = TryParseInt($(this)[0].id.substring($(this)[0].id.lastIndexOf('_') + 1), -1);
                if (Rownumber >= 0)
                    InsideGrid = true;
                if (arrmapingfields.length > 0 && lblnClearRelatedFields && SelectedFromlist) {
                    for (i = 0; i < arrmapingfields.length; i++) {
                        var ctrl = arrmapingfields[i].split("=");
                        if (ctrl.length == 2) {
                            if (InsideGrid) {
                                ctrl[0] = ctrl[0] + "_" + Rownumber;
                            }
                            if ($$(ctrl[0])[0].tagName.toLowerCase() == 'span')
                                $$(ctrl[0]).text("");
                            else
                                $$(ctrl[0]).val("");
                        }
                    }
                    SelectedFromlist = false;
                }
            },
            search: function (event, ui) {
                var searchterm = $(this).val();
                if (searchterm.replace(/,/g, '') == "")
                    return false;
                else {

                    var searchterms = $(this).val().split(",");
                    for (i = 0; i < searchterms.length; i++) {
                        if (searchterms[i] == "")
                            continue;

                        if (ValidateAutoFields(searchterms[i], arrDataTypes[i]))
                            continue;
                        else
                            return false;
                    }
                    return true;
                }
            },
            position: {
                my: "left top",
                at: "left bottom",
                collision: "flip flip"
            },
            autoFocus: true
        }).data("autocomplete")._renderItem = function (ul, item) {
            var style = ""
            if (item["rowindex"] != undefined) {
                style = "class='autocompleteheader'";
            }

            var formateditem = "<table height='10px'><tr>";
            for (i = 0; i < arrfields.length; i++)
                formateditem += "<td " + style + " width='" + widtharray[i] * 9 + "px'>" + item[arrfields[i]] + "</td>";
            formateditem += "</tr></table>";

            return $("<li></li>")
              .data("item.autocomplete", item)
              .append("<a>" + formateditem + "</a>")
              .appendTo(ul);
        };

        $(this).keyup(function (event) {
            var searchterms = $(this).val().split(",");
            var headers = lstrFieldsHeader.split(",");
            var searchstring = "";
            var currentOperator = ":";
            for (i = 0; i < searchterms.length; i++) {
                if (i > arrfields.length)
                    continue;
                if (i == arrfields.length)
                    searchstring += "<b style='color:yellow;'>Exceeding fields...</b>";
                else {
                    currentOperator = ":";
                    if (arrOperators[i] != "like")
                        currentOperator = arrOperators[i];
                    searchstring += "<b>" + headers[i] + " " + currentOperator + "</b> " + searchterms[i] + "  ";
                }
            }
            var newdiv = document.getElementById('autocompletediv');
            if (newdiv == undefined || newdiv == null)
                newdiv = document.createElement('div');

            if ($(this).val() == "")
                newdiv.style.display = "none";
            else
                newdiv.style.display = "block";

            newdiv.setAttribute('id', 'autocompletediv');
            newdiv.style.width = (searchstring.length * 7) + 'px';
            var winw = document.body.offsetWidth;

            if (winw < $(this).position().left + $(this).width() + searchstring.length * 5) {
                var moveby = $(this).position().left + $(this).width() + searchstring.length * 6 - winw
                newdiv.style.left = ($(this).position().left - moveby) + 'px';
            }
            else
                newdiv.style.left = ($(this).position().left) + 'px';

            newdiv.style.top = ($(this).position().top - 20) + 'px';
            newdiv.className = "autocompleteUpperDiv";
            newdiv.innerHTML = "";
            newdiv.innerHTML = searchstring;
            $(this).parent().append(newdiv);
        });

        $(this).blur(function (event) {
            $("#autocompletediv").hide();
        });

        //adding button next to textbox
        //debugger;
        var AutoTextbox = $(this);
        var buttonid = AutoTextbox[0].id + "_autobutton";
        if ($$(buttonid).length == 0) {
            $("<input type='button' value='' id='" + buttonid + "' />")
					.attr("tabIndex", -1)
					.attr("title", "Show All Items")
					.insertAfter(AutoTextbox)
					.addClass("autocompleteIndicator")
					.click(function () {

					    if (AutoTextbox.autocomplete("widget").is(":visible")) {
					        AutoTextbox.autocomplete("close");
					        return;
					    }
					    // Pass empty string as value to search for, displaying all results
					    AutoTextbox.autocomplete("search", AutoTextbox.val());
					    AutoTextbox.focus();
					});
        }
    });
}

//try to convert string to int & is not returns the default value...
//used in autocomplete
function TryParseInt(str, defaultValue) {
    var retValue = defaultValue;
    if (str != null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseInt(str);
            }
        }
    }
    return retValue;
}


function GetMaxWidthArray(jsonarray, arrfields) {
    var widtharray = new Array();
    for (j = 0; j < arrfields.length; j++) {
        widtharray[j] = jsonarray[0][arrfields[j]].length
    }
    for (i = 0; i < jsonarray.length; i++) {
        for (j = 0; j < arrfields.length; j++) {

            if (jsonarray[i][arrfields[j]].indexOf("/") > 0) // checking for date
                jsonarray[i][arrfields[j]] = (new Date(jsonarray[i][arrfields[j]].split(" ")[0])).format("mm/dd/yyyy");
            //jsonarray[i][arrfields[j]] = $.format.date(jsonarray[i][arrfields[j]].split(" ")[0], "MM/dd/yyyy");
            //jsonarray[i][arrfields[j]] = (new Date(jsonarray[i][arrfields[j]].split(" ")[0])).format("MM/dd/yyyy");

            if (widtharray[j] < jsonarray[i][arrfields[j]].length)
                widtharray[j] = jsonarray[i][arrfields[j]].length
        }
    }
    return widtharray;
}

function ValidateAutoFields(value, datatype) {
    try {
        switch (datatype) {
            case 'String': return true;
            case 'Numeric':
                if ((value - 0) != value)
                    return false;
                break;
            case 'DateTime':
                var dt = new Date(value);
                if (!(dt.getFullYear() > 1000 && dt.getYear() < 9999) || !(dt.getDate() && dt.getMonth() + 1 && dt.getYear()))
                    return false;
                break;
        }
        return true;
    }
    catch (e) {
        return false;
    }
}


var dateFormat = function () {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
        timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
        timezoneClip = /[^-+\dA-Z]/g,
        pad = function (val, len) {
            val = String(val);
            len = len || 2;
            while (val.length < len) val = "0" + val;
            return val;
        };

    // Regexes and supporting functions are cached through closure
    return function (date, mask, utc) {
        var dF = dateFormat;

        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }

        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        if (isNaN(date)) throw SyntaxError("invalid date");

        mask = String(dF.masks[mask] || mask || dF.masks["default"]);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) == "UTC:") {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? "getUTC" : "get",
            d = date[_ + "Date"](),
            D = date[_ + "Day"](),
            m = date[_ + "Month"](),
            y = date[_ + "FullYear"](),
            H = date[_ + "Hours"](),
            M = date[_ + "Minutes"](),
            s = date[_ + "Seconds"](),
            L = date[_ + "Milliseconds"](),
            o = utc ? 0 : date.getTimezoneOffset(),
            flags = {
                d: d,
                dd: pad(d),
                ddd: dF.i18n.dayNames[D],
                dddd: dF.i18n.dayNames[D + 7],
                m: m + 1,
                mm: pad(m + 1),
                mmm: dF.i18n.monthNames[m],
                mmmm: dF.i18n.monthNames[m + 12],
                yy: String(y).slice(2),
                yyyy: y,
                h: H % 12 || 12,
                hh: pad(H % 12 || 12),
                H: H,
                HH: pad(H),
                M: M,
                MM: pad(M),
                s: s,
                ss: pad(s),
                l: pad(L, 3),
                L: pad(L > 99 ? Math.round(L / 10) : L),
                t: H < 12 ? "a" : "p",
                tt: H < 12 ? "am" : "pm",
                T: H < 12 ? "A" : "P",
                TT: H < 12 ? "AM" : "PM",
                Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
            };

        return mask.replace(token, function ($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
}();

// Some common format strings
dateFormat.masks = {
    "default": "ddd mmm dd yyyy HH:MM:ss",
    shortDate: "m/d/yy",
    mediumDate: "mmm d, yyyy",
    longDate: "mmmm d, yyyy",
    fullDate: "dddd, mmmm d, yyyy",
    shortTime: "h:MM TT",
    mediumTime: "h:MM:ss TT",
    longTime: "h:MM:ss TT Z",
    isoDate: "yyyy-mm-dd",
    isoTime: "HH:MM:ss",
    isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
    isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
};

// Internationalization strings
dateFormat.i18n = {
    dayNames: [
        "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    ],
    monthNames: [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
        "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
    ]
};

// For convenience...
Date.prototype.format = function (mask, utc) {
    return dateFormat(this, mask, utc);
};

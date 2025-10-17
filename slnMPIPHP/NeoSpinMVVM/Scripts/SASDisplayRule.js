var SASDisplayRule;
(function (SASDisplayRule) {
    var uniqueId = 0;
    var callActUniqueId = 0;
    function CreateRuleHtml(data, effectiveDate) {
        if (data.RuleType == "ExcelMatrix") {
            $("#divView").hide();
        }
        document.title = data.RuleID;
        $("#ruleHeader").text(data.RuleID);
        var entitySpan = $("#entity");
        if (data.Entity == null || data.Entity == "" || data.Entity === undefined) {
            entitySpan.text("Entity");
            entitySpan.addClass("rule-details-disable");
        }
        else {
            entitySpan.text(data.Entity);
        }
        var descriptionSpan = $("#description");
        if (data.Description == null || data.Description == "" || data.Description === undefined) {
            descriptionSpan.text("Description");
            descriptionSpan.addClass("rule-details-disable");
        }
        else {
            descriptionSpan.text(data.Description);
        }
        var statusSpan = $("#status");
        if (data.Status == null || data.Status == "" || data.Status === undefined) {
            statusSpan.text("Status");
            statusSpan.addClass("rule-details-disable");
        }
        else {
            statusSpan.text(data.Status);
        }
        var staticSpan = $("#static");
        if (data.Static == null || data.Static == "" || data.Static === undefined) {
            staticSpan.text("Static");
            staticSpan.addClass("rule-details-disable");
        }
        else {
            staticSpan.text(data.Static);
        }
        var returnTypeSpan = $("#returnType");
        if (data.ReturnType == null || data.ReturnType == "" || data.ReturnType === undefined) {
            returnTypeSpan.text("ReturnType");
            returnTypeSpan.addClass("rule-details-disable");
        }
        else {
            returnTypeSpan.text(data.ReturnType);
        }
        var div = $('#dvLogicalRule');
        var html;
        var rule = GetRuleByEffectiveDate(data, effectiveDate);
        var matchAllConditionWrapper = $("#matchAllConditionWrapper");
        matchAllConditionWrapper.hide();
        if (data.RuleType == "LogicalRule") {
            html = [html, LoadLogicalRule(rule.Elements)].join('');
            $("#ruleIcon").addClass("logical-rule-icon");
        }
        else if (data.RuleType == "DecisionTable") {
            html = [html, LoadDecisionTable(rule.Rows)].join('');
            $("#ruleIcon").addClass("decision-table-icon");
            matchAllConditionWrapper.show();
            if (data.MatchAllConditions == nsConstants.TRUE || data.MatchAllConditions == "True") {
                $("#matchAllConditions").attr('checked', 'checked');
            }
            else {
                $("#matchAllConditions").removeAttr('checked');
            }
        }
        else if (data.RuleType == "ExcelMatrix") {
            html = [html, LoadExcelMatrix(rule.Rows)].join('');
            $("#ruleIcon").addClass("excel-matrix-icon");
        }
        if (div != undefined) {
            div.html(html);
        }
    }
    function LoadDecisionTable(rows) {
        var html = [html, "<table cellspacing=\"0\" cellpadding=\"0\" class=\"dt-table-border\">"].join('');
        for (var row in rows) {
            html = [html, "<tr>"].join('');
            for (var cell in rows[row].Cells) {
                html = [html, ["<td valign=\"top\" rowspan=\"", rows[row].Cells[cell].Rowspan, "\" colspan=\"", rows[row].Cells[cell].Colspan, "\" class=\"", GetBGColorFromCellData(rows[row].Cells[cell].Item), "\" >"].join('')].join('');
                html = [html, GetDTCellData(rows[row].Cells[cell].Item)].join('');
                html = [html, "</td>"].join('');
            }
            html = [html, "</tr>"].join('');
        }
        html = [html, "</table>"].join('');
        return html;
    }
    function LoadExcelMatrix(rows) {
        var html = [html, "<table cellspacing=\"0\" cellpadding=\"0\" class=\"excel-table\">"].join('');
        for (var row in rows) {
            html = [html, "<tr>"].join('');
            for (var cell in rows[row].Cells) {
                html = [html, ["<td valign=\"top\" class=\"", GetClassExcelMatrixCell(rows[row].Cells[cell].Item), "\" >"].join('')].join('');
                html = [html, rows[row].Cells[cell].Item.Description].join('');
                html = [html, "</td>"].join('');
            }
            html = [html, "</tr>"].join('');
        }
        html = [html, "</table>"].join('');
        return html;
    }
    function GetClassExcelMatrixCell(celldata) {
        var bgclass = "";
        if (celldata.ItemType == "colheader") {
            bgclass = "excel-colheader";
        }
        else if (celldata.ItemType == "rowheader") {
            bgclass = "excel-rowheader";
        }
        else if (celldata.ItemType == "data") {
            bgclass = "excel-data";
        }
        return bgclass;
    }
    function GetDTCellData(celldata) {
        var actualValueClass = "actual-value";
        var actualValue = celldata.ActualValue;
        if (actualValue === undefined || actualValue == "" || actualValue == null) {
            actualValue = "[BLANK]";
            actualValueClass += "-blank";
        }
        var html = [html, ["<table id=\"", celldata.NodeID, "\">"].join('')].join('');
        if (celldata.ItemType == "returnheader") {
            html = [html, ["<tr><td><div class=\"return-block-icon-dt\" ></div><td>", celldata.Description, "</td></tr>"].join('')].join('');
        }
        else {
            if (celldata.ItemType == "rowheader" || celldata.ItemType == "colheader") {
                html = [html, ["<tr><td rowspan=\"2\"><div class=\"switch-block-icon-dt\" ></div></td><td>", celldata.Description, "</td></tr>"].join('')].join('');
            }
            else if (celldata.ItemType == "assignheader") {
                html = [html, ["<tr><td rowspan=\"2\"><div class=\"action-block-icon-dt\" ></div></td><td>", celldata.Description, "</td></tr>"].join('')].join('');
            }
            else if (celldata.ItemType == "notesheader") {
                html = [html, ["<tr><td rowspan=\"2\"><div class=\"notes-block-icon-dt\" ></div></td><td>", celldata.Description, "</td></tr>"].join('')].join('');
            }
            else {
                html = [html, ["<tr><td rowspan=\"2\">    </td><td>", celldata.Description, "</td></tr>"].join('')].join('');
            }
            html = [html, ["<tr><td><span class=\"", actualValueClass, "\">", actualValue, "</span>", celldata.Expression, "</td></tr>"].join('')].join('');
        }
        html = [html, "</table>"].join('');
        return html;
    }
    function GetBGColorFromCellData(celldata) {
        var bgclass = "";
        if (celldata.ItemType == "assignheader" || celldata.ItemType == "returnheader" || celldata.ItemType == "notesheader") {
            bgclass = "dt-assign-header";
        }
        else if (celldata.ItemType == "assign" || celldata.ItemType == "return" || celldata.ItemType == "notes") {
            bgclass = "dt-assign";
        }
        else if (celldata.ItemType == "rowheader" || celldata.ItemType == "colheader") {
            bgclass = "dt-row-col-header";
        }
        else if (celldata.ItemType == "if") {
            bgclass = "dt-if";
        }
        return bgclass;
    }
    function LoadLogicalRule(elements, divEle) {
        var html = [html, "<p class=\"start-arrow\"></p><span class=\"start-line\"></span><ul class=\"start-ul\">"].join('');
        var ind;
        for (ind in elements) {
            var objStep = elements[ind];
            var position = "First";
            if (elements.length == 1) {
                position = "FirstAndLast";
            }
            else if (ind == 0) {
                position = "First";
            }
            else if (ind == elements.length - 1) {
                position = "Last";
            }
            else {
                position = "Intermediate";
            }
            html = [html, GetHtmlForElement(objStep, position)].join('');
        }
        html = [html, "</ul>"].join('');
        return html;
    }
    function GetHtmlForElement(objStepDetails, position, itemType, parentElement) {
        uniqueId++;
        var html = [html, ""].join('');
        if (itemType == "Element") {
            html = [html, "<li class=\"condition-li\">"].join('');
        }
        else {
            html = [html, "<li>"].join('');
        }
        if (itemType === undefined) {
            html = [html, "<span class=\"nowrapped\">"].join('');
        }
        html = [html, "<span class=\"nodes\">"].join('');
        if (position != "First" && position != "FirstAndLast") {
            html = [html, GetSpanTopBorder(objStepDetails, position, parentElement)].join('');
        }
        var item = objStepDetails.ObjItems;
        var elements = objStepDetails.Elements;
        if (objStepDetails.StepType == "case" || objStepDetails.StepType == "default") {
            item = objStepDetails;
            elements = null;
        }
        else if (objStepDetails.StepType == "actions" || objStepDetails.StepType == "notes") {
            elements = null;
        }
        if ((elements != null && elements.length > 0) || (item != null && item.Elements.length > 0)) {
            var spandId = ["span_", uniqueId].join('');
            var elementsId = ["elements_ul_", uniqueId].join('');
            var itemsId = ["items_ul_", uniqueId].join('');
            var middleBorderId = ["middleBorder_span_", uniqueId].join('');
            html = [html, ["<span id=\"", spandId, "\" class=\"", (objStepDetails.IsExecuted ? "expanded" : "collapsed"), "\" onclick=\"SASDisplayRule.expandCollapse('", spandId, "','", elementsId, "','", itemsId, "','", middleBorderId, "');\"></span>"].join('')].join('');
        }
        if (position != "Last" && position != "FirstAndLast" && position != "IntermediateFirstAndLast") {
            html = [html, GetSpanBottomBorder(objStepDetails)].join('');
        }
        html = [html, GetSpanRightBorder(objStepDetails, position)].join('');
        html = [html, GetSpanText1(objStepDetails)].join('');
        if (elements != null && elements.length > 0) {
            if (objStepDetails.StepType == "foreach" || objStepDetails.StepType == "while") {
                if (objStepDetails.IsExecuted) {
                    html = [html, ["<ul id=\"", elementsId, "\" class=\"condition-ul-loop\">"].join('')].join('');
                }
                else {
                    html = [html, ["<ul id=\"", elementsId, "\" class=\"condition-ul-loop\" style=\"display:none;\">"].join('')].join('');
                }
            }
            else {
                if (objStepDetails.IsExecuted) {
                    html = [html, ["<ul id=\"", elementsId, "\" class=\"condition-ul\">"].join('')].join('');
                }
                else {
                    html = [html, ["<ul id=\"", elementsId, "\" class=\"condition-ul\" style=\"display:none;\">"].join('')].join('');
                }
            }
            for (var i = 0; i < elements.length; i++) {
                var elePosition;
                if ((elements[i].StepType == "case" ||
                    elements[i].StepType == "default") && elements.length == 1) {
                    elePosition = "IntermediateFirstAndLast";
                }
                else {
                    if (i == elements.length - 1) {
                        elePosition = "Last";
                    }
                    else if (i == 0) {
                        elePosition = "IntermediateFirst";
                    }
                    else {
                        elePosition = "Intermediate";
                    }
                }
                html = [html, GetHtmlForElement(elements[i], elePosition, "Element", objStepDetails.StepType)].join('');
            }
            html = [html, "</ul>"].join('');
        }
        html = [html, "</span>"].join('');
        if (item != null && item.Elements.length > 0) {
            var items = item.Elements;
            if (objStepDetails.StepType != "switch" && objStepDetails.StepType != "foreach" && objStepDetails.StepType != "while") {
                html = [html, GetSpanMiddleBorder(middleBorderId, objStepDetails.IsExecuted)].join('');
            }
            html = [html, "<span class=\"nodes\">"].join('');
            if (objStepDetails.IsExecuted) {
                html = [html, ["<ul id=\"", itemsId, "\" class=\"tree-li-ul\">"].join('')].join('');
            }
            else {
                html = [html, ["<ul id=\"", itemsId, "\" class=\"tree-li-ul\" style=\"display:none;\">"].join('')].join('');
            }
            for (var i = 0; i < items.length; i++) {
                var elePosition;
                if (items.length == 1) {
                    elePosition = "FirstAndLast";
                }
                else if (i == 0) {
                    elePosition = "First";
                }
                else if (i == items.length - 1) {
                    elePosition = "Last";
                }
                else {
                    elePosition = "Intermediate";
                }
                html = [html, GetHtmlForElement(item.Elements[i], elePosition, "Item")].join('');
            }
            html = [html, "</ul>"].join('');
            html = [html, "</span>"].join('');
        }
        if (itemType === undefined) {
            html = [html, "</span>"].join('');
        }
        html = [html, "</li>"].join('');
        return html;
    }
    function GetSpanText1(objStepDetails) {
        var hasItems = false;
        var elements = objStepDetails.Elements;
        var item = objStepDetails.ObjItems;
        if (objStepDetails.StepType == "case" || objStepDetails.StepType == "default") {
            item = objStepDetails;
            elements = null;
        }
        if (item != null && item.Elements.length > 0) {
            hasItems = true;
        }
        var html = [html, ""].join('');
        if (objStepDetails.StepType == "actions") {
            var expressions = "";
            for (var i = 0; i < objStepDetails.Elements.length; i++) {
                if (i != 0) {
                    expressions += "|";
                }
                expressions = objStepDetails.Elements[i].Expression;
            }
            html = [html, GetSpanText(objStepDetails.Description, expressions, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "switch" || objStepDetails.StepType == "case" || objStepDetails.StepType == "default" || objStepDetails.StepType == "return") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.Expression, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "notes") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.Notes, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "calldecisiontable" || objStepDetails.StepType == "calllogicalrule" || objStepDetails.StepType == "callexcelmatrix") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.RuleID, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "foreach") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.ObjectID, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "break" || objStepDetails.StepType == "continue") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.Expression, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "while") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.Expression, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "query") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.QueryID, hasItems, objStepDetails)].join('');
        }
        else if (objStepDetails.StepType == "method") {
            html = [html, GetSpanText(objStepDetails.Description, objStepDetails.MethodName, hasItems, objStepDetails)].join('');
        }
        return html;
    }
    function GetSpanContainer() {
        return "<span class=\"span-container\">";
    }
    function GetSpanMiddleBorder(middleBorderId, isExecuted) {
        if (isExecuted) {
            return ["<span id=\"", middleBorderId, "\" class=\"span-middle-border\"></span>"].join('');
        }
        else {
            return ["<span id=\"", middleBorderId, "\" class=\"span-middle-border\" style=\"display:none;\"></span>"].join('');
        }
    }
    function GetSpanBottomBorder(objStepDetails) {
        var html = [html, ""].join('');
        if (objStepDetails.StepType == "case" || objStepDetails.StepType == "default") {
            if (objStepDetails.IsExecuted) {
                html = [html, "<span class=\"span-condition-bottom-border\"></span>"].join('');
            }
            else {
                html = [html, "<span class=\"span-condition-bottom-border-disable\"></span>"].join('');
            }
        }
        else {
            html = [html, "<span class=\"span-bottom-border\"></span>"].join('');
        }
        return html;
    }
    function GetSpanText(description, expression, hasItems, objStepDetails) {
        var html = [html, ""].join('');
        var i;
        var parameter;
        var descTextAssignClass = "text-assign";
        var expTextAssignClass = "text-assign";
        if (objStepDetails.StepType == "default" || objStepDetails.StepType == "break" || objStepDetails.StepType == "continue") {
            descTextAssignClass = "default-text-assign";
            expTextAssignClass = "default-text-assign";
        }
        if (expression == "" || expression == null) {
            expression = "[BLANK]";
            expTextAssignClass += "-blank";
        }
        if (description == "" || description == null) {
            description = "[BLANK]";
            descTextAssignClass += "-blank";
        }
        var spanClass = null;
        if (objStepDetails.StepType == "foreach" || objStepDetails.StepType == "while" || objStepDetails.StepType == "switch") {
            spanClass = "span-head";
            if (!hasItems) {
                spanClass += "-last";
            }
        }
        else {
            spanClass = "span-text";
        }
        var actualValue = ""; //objStepDetails.ActualValue;
        var actualValueClass = "actual-value";
        //if (actualValue == "" || actualValue == null) {
        //    actualValue = "[BLANK]";
        //    actualValueClass += "-blank";
        //}
        if (objStepDetails.StepType == "switch") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "switch-block"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"switch-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"", expTextAssignClass, "\"><span class=\"", actualValueClass, "\">{2}</span>  {1}</td></tr></tbody></table></span></span>"].join(''), description, expression, actualValue)].join('');
        }
        else if (objStepDetails.StepType == "actions") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "action-block"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"action-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr>"].join(''), description)].join('');
            for (i = 0; i < objStepDetails.Elements.length; i++) {
                actualValue = objStepDetails.Elements[i].ActualValue;
                actualValueClass = "actual-value";
                if (actualValue == "" || actualValue == null) {
                    actualValue = "[BLANK]";
                    actualValueClass += "-blank";
                }
                if (i == 0) {
                    html = [html, String.format(["<tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td colspan=\"2\" class=\"", expTextAssignClass, "\"><span class=\"", actualValueClass, "\">{1}</span>  {0}</td></tr>"].join(''), objStepDetails.Elements[i].Expression, actualValue)].join('');
                }
                else {
                    html = [html, String.format(["<tr><td></td><td><div class=\"line-assign\"></div></td></tr><tr><td></td><td class=\"", expTextAssignClass, "\"><span class=\"", actualValueClass, "\">{1}</span>  {0}</td></tr>"].join(''), objStepDetails.Elements[i].Expression, actualValue)].join('');
                }
            }
            html = [html, "</tbody></table></span></span>"].join('');
        }
        else if (objStepDetails.StepType == "foreach") {
            var iscollNameBlank = false;
            var isItemNameBlank = false;
            var itemName = objStepDetails.ItemName;
            if (expression == "[BLANK]") {
                expression = "Collection";
                iscollNameBlank = true;
            }
            if (itemName == "" || itemName == null) {
                itemName = "ItemName";
                isItemNameBlank = true;
            }
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "loop-block"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"foreach-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}<span class=\"loop-execution-count\">{3}</span></td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td><span ", (isItemNameBlank ? "class=\"text-blank\"" : ""), " >{1}</span> of <span ", (iscollNameBlank ? "class=\"text-blank\"" : ""), " >{2}</span></td></tr></tbody></table></span></span>"].join(''), description, itemName, expression, objStepDetails.LoopExecutionCount)].join('');
        }
        else if (objStepDetails.StepType == "while") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "loop-block"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"while-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}<span class=\"loop-execution-count\">{2}</span></td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"", expTextAssignClass, "\">{1}</td></tr></tbody></table></span></span>"].join(''), description, expression, objStepDetails.LoopExecutionCount)].join('');
        }
        else if (objStepDetails.StepType == "case") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "case-block"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"case-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"", expTextAssignClass, "\"><span class=\"", actualValueClass, "\">{2}</span>  {1}</td></tr></tbody></table></span></span>"].join(''), description, expression, actualValue)].join('');
        }
        else if (objStepDetails.StepType == "default") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "case-block"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"default-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr></tbody></table></span></span>"].join(''), description)].join('');
        }
        else if (objStepDetails.StepType == "break") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"break-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr></tbody></table></span></span>"].join(''), description)].join('');
        }
        else if (objStepDetails.StepType == "continue") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"continue-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr></tbody></table></span></span>"].join(''), description)].join('');
        }
        else if (objStepDetails.StepType == "calllogicalrule") {
            var isReturnFieldBlank = false;
            var returnField = objStepDetails.ReturnField;
            var isIDBlank = false;
            var effectiveDate = objStepDetails.EffectiveDate;
            var isEffectiveDateBlank = false;
            if (expression == "[BLANK]") {
                expression = "ID";
                isIDBlank = true;
            }
            if (returnField == null || returnField == "") {
                returnField = "ReturnField";
                isReturnFieldBlank = true;
            }
            if (effectiveDate == null || effectiveDate == "") {
                effectiveDate = "Effective Date";
                isEffectiveDateBlank = true;
            }
            callActUniqueId++;
            var pId = ["p_", callActUniqueId].join('');
            var tableId = ["table_id", callActUniqueId].join('');
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"calllogicalrule-block-icon\" ondblclick=\"SASDisplayRule.onCallLogicalRuleDoubleClick(event)\" nodeId={2}></div></div></td><td class=\"", descTextAssignClass, "\"><p id=", pId, " onclick=\"SASDisplayRule.expandCollapseParameters('", pId, "','", tableId, "')\" class=\"callactivity-arrow-right\"></p> {0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"text-assign\"><span ", (isReturnFieldBlank ? "class=\"text-blank\"" : ""), " >{3}</span><span> = </span><span ", (isIDBlank ? "class=\"text-blank\"" : ""), " >{1}</span><span>  [</span><span ", (isEffectiveDateBlank ? "class=\"text-blank\"" : ""), " >{4}</span><span>] </span></td></tr><tr><td></td><td><table style=\"display:none\" id=", tableId, "><tbody>"].join(''), description, expression, objStepDetails.ID, returnField, effectiveDate)].join('');
            for (i in objStepDetails.Parameters) {
                parameter = objStepDetails.Parameters[i];
                html = [html, String.format("<tr><td>{0}[{1}{2}]</td><td>&nbsp;&nbsp;&nbsp;&nbsp;= {3}</td></tr>", parameter.ParameterName, parameter.Direction, parameter.DataType, parameter.Value)].join('');
            }
            html = [html, "</tbody></table></td></tr></tbody></table></span></span>"].join('');
        }
        else if (objStepDetails.StepType == "calldecisiontable") {
            var isReturnFieldBlank = false;
            var returnField = objStepDetails.ReturnField;
            var isIDBlank = false;
            var effectiveDate = objStepDetails.EffectiveDate;
            var isEffectiveDateBlank = false;
            if (expression == "[BLANK]") {
                expression = "ID";
                isIDBlank = true;
            }
            if (returnField == null || returnField == "") {
                returnField = "ReturnField";
                isReturnFieldBlank = true;
            }
            if (effectiveDate == null || effectiveDate == "") {
                effectiveDate = "Effective Date";
                isEffectiveDateBlank = true;
            }
            callActUniqueId++;
            var pId = ["p_", callActUniqueId].join('');
            var tableId = ["table_id", callActUniqueId].join('');
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"calldecisiontable-block-icon\" ondblclick=\"SASDisplayRule.onCallLogicalRuleDoubleClick(event)\" nodeId={2}></div></div></td><td class=\"", descTextAssignClass, "\"><p id=", pId, " onclick=\"SASDisplayRule.expandCollapseParameters('", pId, "','", tableId, "')\" class=\"callactivity-arrow-right\"></p> {0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"text-assign\"><span ", (isReturnFieldBlank ? "class=\"text-blank\"" : ""), " >{3}</span><span> = </span><span ", (isIDBlank ? "class=\"text-blank\"" : ""), " >{1}</span><span>  [</span><span ", (isEffectiveDateBlank ? "class=\"text-blank\"" : ""), " >{4}</span><span>] </span></td></tr><tr></td><td><td><table style=\"display:none\" id=", tableId, "><tbody>"].join(''), description, expression, objStepDetails.ID, returnField, effectiveDate)].join('');
            for (i in objStepDetails.Parameters) {
                parameter = objStepDetails.Parameters[i];
                html = [html, String.format("<tr><td>{0}[{1}{2}]</td><td>&nbsp;&nbsp;&nbsp;&nbsp;= {3}</td></tr>", parameter.ParameterName, parameter.Direction, parameter.DataType, parameter.Value)].join('');
            }
            html = [html, "</tbody></table></td></tr></tbody></table></span></span>"].join('');
        }
        else if (objStepDetails.StepType == "callexcelmatrix") {
            var isReturnFieldBlank = false;
            var returnField = objStepDetails.ReturnField;
            var isIDBlank = false;
            var effectiveDate = objStepDetails.EffectiveDate;
            var isEffectiveDateBlank = false;
            if (expression == "[BLANK]") {
                expression = "ID";
                isIDBlank = true;
            }
            if (returnField == null || returnField == "") {
                returnField = "ReturnField";
                isReturnFieldBlank = true;
            }
            if (effectiveDate == null || effectiveDate == "") {
                effectiveDate = "Effective Date";
                isEffectiveDateBlank = true;
            }
            callActUniqueId++;
            var pId = ["p_", callActUniqueId].join('');
            var tableId = ["table_id", callActUniqueId].join('');
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"callexcelmatrix-block-icon\" ondblclick=\"SASDisplayRule.onCallLogicalRuleDoubleClick(event)\" nodeId={2}></div></div></td><td class=\"", descTextAssignClass, "\"><p id=", pId, " onclick=\"SASDisplayRule.expandCollapseParameters('", pId, "','", tableId, "')\" class=\"callactivity-arrow-right\"></p> {0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"text-assign\"><span ", (isReturnFieldBlank ? "class=\"text-blank\"" : ""), " >{3}</span><span> = </span><span ", (isIDBlank ? "class=\"text-blank\"" : ""), " >{1}</span><span>  [</span><span ", (isEffectiveDateBlank ? "class=\"text-blank\"" : ""), " >{4}</span><span>] </span></td></tr><tr></td><td><td><table style=\"display:none\" id=", tableId, "><tbody>"].join(''), description, expression, objStepDetails.ID, returnField, effectiveDate)].join('');
            for (i in objStepDetails.Parameters) {
                parameter = objStepDetails.Parameters[i];
                html = [html, String.format("<tr><td>{0}[{1}{2}]</td><td>&nbsp;&nbsp;&nbsp;&nbsp;= {3}</td></tr>", parameter.ParameterName, parameter.Direction, parameter.DataType, parameter.Value)].join('');
            }
            html = [html, "</tbody></table></td></tr></tbody></table></span></span>"].join('');
        }
        else if (objStepDetails.StepType == "method") {
            var isReturnFieldBlank = false;
            var returnField = objStepDetails.ReturnField;
            var isIDBlank = false;
            var isEffectiveDateBlank = false;
            if (expression == "[BLANK]") {
                expression = "ID";
                isIDBlank = true;
            }
            if (returnField == null || returnField == "") {
                returnField = "ReturnField";
                isReturnFieldBlank = true;
            }
            callActUniqueId++;
            var pId = ["p_", callActUniqueId].join('');
            var tableId = ["table_id", callActUniqueId].join('');
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"method-block-icon\" nodeId={2}></div></div></td><td class=\"", descTextAssignClass, "\"><p id=", pId, " onclick=\"expandCollapseParameters('", pId, "','", tableId, "')\" class=\"callactivity-arrow-right\"></p> {0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"text-assign\"><span ", (isReturnFieldBlank ? "class=\"text-blank\"" : ""), " >{3}</span><span> = </span><span ", (isIDBlank ? "class=\"text-blank\"" : ""), " >{1}</span></td></tr><tr></td><td><td><table style=\"display:none\" id=", tableId, "><tbody>"].join(''), description, expression, objStepDetails.ID, returnField)].join('');
            for (var keyField in objStepDetails.Parameters) {
                var parameter = objStepDetails.Parameters[keyField];
                html = [html, String.format("<tr><td>{0}[{1},{2}]</td><td>&nbsp;&nbsp;&nbsp;&nbsp;= {3}</td></tr>", parameter.ParameterName, parameter.Direction, parameter.DataType, parameter.Value)].join('');
            }
            html = [html, "</tbody></table></td></tr></tbody></table></span></span>"].join('');
        }
        else if (objStepDetails.StepType == "return") {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"return-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"", expTextAssignClass, "\">{1}</td></tr></tbody></table></span></span>"].join(''), description, expression)].join('');
        }
        else if (objStepDetails.StepType == "query") {
            if (expression == "[BLANK]") {
                expression = "ID";
            }
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"query-block-icon\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"", expTextAssignClass, "\">{1}</td></tr></tbody></table></span></span>"].join(''), description, expression)].join('');
        }
        else if (objStepDetails.StepType == "notes") {
            //if (expression == "[BLANK]") {
            expression = "Notes";
            //}
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"notes-block-icon\" ondblclick=\"OnNotesDoubleClick('{2}');\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"", expTextAssignClass, "\">{1}</td></tr></tbody></table></span></span>"].join(''), description, expression, objStepDetails.ID)].join('');
        }
        else {
            html = [html, String.format(["<span class=\"", spanClass, "\"><span class=\"", (objStepDetails.IsExecuted == false ? "bdr-assign-disable" : "bdr-assign"), "\"><table class=\"tbl-assign\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td rowspan=\"3\"><div class=\"crcl-assign\"><div class=\"dimond-assign\"></div></div></td><td class=\"", descTextAssignClass, "\">{0}</td></tr><tr><td colspan=\"2\"><div class=\"line-assign\"></div></td></tr><tr><td class=\"", expTextAssignClass, "\">{1}</td></tr></tbody></table></span></span>"].join(''), description, expression)].join('');
        }
        return html;
    }
    function GetSpanRightBorder(objStepDetails, position) {
        var html = [html, ""].join('');
        if (objStepDetails.StepType == "case" || objStepDetails.StepType == "default") {
            if (position == "Last" || position == "IntermediateFirstAndLast") {
                if (objStepDetails.IsExecuted) {
                    html = [html, "<span class=\"span-condition-right-border-last\"></span>"].join('');
                }
                else {
                    html = [html, "<span class=\"span-condition-right-border-last-disable\"></span>"].join('');
                }
            }
            else {
                if (objStepDetails.IsExecuted) {
                    html = [html, "<span class=\"span-condition-right-border\"></span>"].join('');
                }
                else {
                    html = [html, "<span class=\"span-condition-right-border-disable\"></span>"].join('');
                }
            }
        }
        else {
            html = [html, "<span class=\"span-right-border\"></span>"].join('');
        }
        return html;
    }
    function GetSpanTopBorder(objStepDetails, position, parentElement) {
        var html = [html, ""].join('');
        if (objStepDetails.StepType == "case" || objStepDetails.StepType == "default") {
            if (position == "IntermediateFirst" || position == "IntermediateFirstAndLast") {
                if (objStepDetails.IsExecuted) {
                    html = [html, "<span class=\"span-condition-top-border-first\"></span>"].join('');
                }
                else {
                    html = [html, "<span class=\"span-condition-top-border-first-disable\"></span>"].join('');
                }
            }
            else {
                if (objStepDetails.IsExecuted) {
                    html = [html, "<span class=\"span-condition-top-border\"></span>"].join('');
                }
                else {
                    html = [html, "<span class=\"span-condition-top-border-disable\"></span>"].join('');
                }
            }
        }
        else {
            if (parentElement == "while" || parentElement == "foreach") {
                html = [html, "<span class=\"span-top-border-loop\"></span>"].join('');
            }
            else {
                html = [html, "<span class=\"span-top-border\"></span>"].join('');
            }
        }
        return html;
    }
    function OnForEachDoubleClick(e) {
        //TODO: This code is currently not required, but need to be implemented/modified in future for showing loop steps in execution.
        //if (e.currentTarget != undefined) {
        //    var nodeId = e.currentTarget.getAttribute("nodeId");
        //    var data = ns.RuleData["ruleData"];
        //    if (data != undefined) {
        //        var objStepDetails = GetStepDetails(nodeId, data.Elements);
        //        if (objStepDetails != undefined) {
        //            ShowCollectionExectionWindow(objStepDetails);
        //        }
        //    }
        //}
    }
    function OnCallLogicalRuleDoubleClick(e, var1) {
        if (e.currentTarget != undefined) {
            var nodeId = e.currentTarget.getAttribute("nodeId");
            ShowCallLogicalRule(nodeId);
        }
    }
    function ShowCallLogicalRule(nodeId) {
        var jsonvalue = $('#lscriptRuleData').html();
        var data = jQuery.parseJSON(jsonvalue);
        if (data != undefined) {
            var rule = GetRuleByEffectiveDate(data, $("#ddlEffectiveDate").val());
            var objStepDetails = GetStepDetails(nodeId, rule.Elements);
            if (objStepDetails != undefined && objStepDetails.LogicalRule != undefined) {
                OpenPopup(objStepDetails.LogicalRule, true, undefined, true);
            }
        }
    }
    function OnNotesDoubleClick(nodeId) {
        if (nodeId != undefined && nodeId != null && nodeId != "") {
            var jsonvalue = $('#lscriptRuleData').html();
            var data = jQuery.parseJSON(jsonvalue);
            if (data != undefined) {
                var rule = GetRuleByEffectiveDate(data, $("#ddlEffectiveDate").val());
                var objStepDetails = GetStepDetails(nodeId, rule.Elements);
                ShowNotesWindow(objStepDetails);
            }
        }
    }
    function ShowNotesWindow(objStepDetails) {
        var html = [html, "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">"].join('');
        html = [html, String.format("<html><head></head><body><div>{0}</div></body></html>", objStepDetails.Notes)].join('');
        var wndw = window.open("", "_blank", "toolbar=yes, scrollbars=yes, resizable=yes, width=800, height=600");
        var newDoc = wndw.document;
        newDoc.write(html);
        newDoc.close();
    }
    function GetStepDetails(nodeId, elements) {
        var retVal;
        for (var ind in elements) {
            var objStep = elements[ind];
            if (objStep.ID == nodeId) {
                retVal = objStep;
                break;
            }
            if (objStep.Elements != undefined) {
                retVal = GetStepDetails(nodeId, objStep.Elements);
                if (retVal != undefined) {
                    break;
                }
            }
            if (objStep.ObjItems != undefined) {
                retVal = GetStepDetails(nodeId, objStep.ObjItems.Elements);
                if (retVal != undefined) {
                    break;
                }
            }
        }
        return retVal;
    }
    function OpenPopup(objLogicalRule, ablnIsFirstTime, astrControlId, ablnOpenInPopUp) {
        if (ablnIsFirstTime == undefined) {
            ablnIsFirstTime = false;
        }
        var Wid = "CallLogicalRule";
        var svgID = [Wid, "Svg"].join('');
        var selectedView = $('#ddlView option:selected').text();
        var html = [html, "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><!-- Note: IE8 supports the content property only if a !DOCTYPE is specified. --><html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">"].join('');
        html = [html, "<head><title>li</title>"].join('');
        html = [html, ["<link rel=\"stylesheet\" href=\"", ns.SiteName, "/Styles/SASDisplayRule.css\">"].join('')].join('');
        html = [html, ["<link rel=\"stylesheet\" href=\"", ns.SiteName, "/Styles/Kendo/kendo.common.min.css\">"].join('')].join('');
        html = [html, ["<link rel=\"stylesheet\" href=\"", ns.SiteName, "/Styles/Kendo/kendo.blueopal.min.css\">"].join('')].join('');
        //html = [html, "<script src=\"" , ns.SiteName , "/Home/GetEmbeddedResource?astrResourceName=Lib.jquery.min.js\"></script>";
        //html = [html, "<script src=\"" , ns.SiteName , "/Home/GetEmbeddedResource?astrResourceName=Lib.kendo.all.min.js\"></script>";
       
        MVVMGlobal.GetFMSctipts();
        html = [html, MVVMGlobal.aobjFMScripts["FMLibScript"]].join('');
        html = [html, MVVMGlobal.aobjFMScripts["FMScript"]].join('');
        html = [html, "<script src=\"", ns.SiteName, "/Scripts/SASDisplayRule.js\"></script></head>"].join('');
        html = [html, "<script type='text/template' id='lscriptRuleData'>" + JSON.stringify(objLogicalRule) + "</script>"].join('');
        html = [html, "</head>"].join('');
        html = [html, ["<body onload='SASDisplayRule.loadChildRule(\"", selectedView, "\");'><div class=\"wrapper\"><div class=\"header-left\"><table><tbody><tr><td rowspan=\"2\"><div id=\"ruleIcon\"></div> \
            <td id=\"ruleHeader\" class=\"rule-header\"></td></td></tr><tr><td id=\"ruleDetails\" class=\"rule-details\"><span id=\"entity\"> \
            </span>&nbsp;&nbsp;|&nbsp;&nbsp;<span id=\"description\"></span>&nbsp;&nbsp;|&nbsp;&nbsp;<span id=\"matchAllConditionWrapper\"> \
            <input id=\"matchAllConditions\" type=\"checkbox\" disabled=\"disabled\" style=\"margin-bottom:-2px;\" />&nbsp;<span>Match All Conditions</span>&nbsp;&nbsp;|&nbsp;&nbsp;</span> \
            <span id=\"returnType\"></span>&nbsp;&nbsp;|&nbsp;&nbsp;<span id=\"status\"></span>&nbsp;&nbsp;|&nbsp;&nbsp;<span id=\"static\"></span></td></tr></tbody></table></div> \
            <div class=\"header-right\" align=\"right\"><label>Effective Date:&nbsp;</label><select id=\"ddlEffectiveDate\" onchange=\"SASDisplayRule.updateRule();\"></select></div> \
            <div id=\"divView\" class=\"header-right\" align=\"right\"><label>View:&nbsp;</label><select id=\"ddlView\" onchange=\"SASDisplayRule.updateView();\"><option>Developer View</option><option>Analyst View</option></select>&nbsp;&nbsp;</div> \
            <div class=\"tree\" id='dvLogicalRule'>"].join('')].join('');
        html = [html, ["</div></div>"].join('')].join('');
        var antiForgeryToken = $("#antiForgeryToken").val();
        html = [html, "<input type='hidden' id='antiForgeryToken' value='", antiForgeryToken, "' />"].join('');
        html = [html, "</body></html>"].join('');
        var divid = objLogicalRule.RuleID;
        if (divid == undefined) {
            divid = objLogicalRule.Rules[0].Elements[0].ID;
        }
        var SASwindowsize = "100%";
        if (ablnIsFirstTime) {
            SASwindowsize = "90%";
        }
        var PopupDiv = $(["<div id='", divid, "'></div>"].join(''));
        if (ablnOpenInPopUp != undefined && !ablnOpenInPopUp) {
            var ActiveDivID = nsCommon.GetActiveDivId(this);
            var lobjParent = $([nsConstants.HASH, ActiveDivID].join('')).find([nsConstants.HASH, astrControlId].join(''));
            lobjParent.empty();
            PopupDiv.appendTo(lobjParent);
            $(PopupDiv).attr("width", SASwindowsize);
            $(PopupDiv).attr("height", SASwindowsize);
        }
        else {
            document.body.appendChild(PopupDiv[0]);
            if (ns.arrDialog[divid] === undefined) {
                // load idictSelectedControls from parent window object.
                MVVMGlobal.idictSelectedControls = window.parent["MVVMGlobal"].idictSelectedControls;
                ns.arrDialog[divid] = MVVM.Controls.Dialog.CreateInstance(PopupDiv, divid, {
                    title: "Rule",
                    width: SASwindowsize,
                    height: SASwindowsize,
                    resizable: false,
                    deactivate: function () {
                        PopupDiv.html('');
                    },
                    arrObjCollection: { arrCollection: ns.arrDialog, divID: divid }
                });
            }
        }
        // PopupDiv = $([nsConstants.HASH, divid].join(''));
        var iframe = document.createElement('iframe');
        PopupDiv[0].appendChild(iframe);
        iframe.style.width = [100, "%"].join('');
        iframe.style.height = [100, "%"].join('');
        iframe.style.border = '1px solid #888888';
        iframe.style.minHeight = '300px';
        iframe.contentWindow.window.name = window.name;
        iframe.contentWindow.document.open();
        iframe.contentWindow.document.write(html);
        iframe.contentWindow.document.close();
        if (!ablnOpenInPopUp)
            $(PopupDiv).show();
        else {
            ns.arrDialog[divid].show();
            ns.arrDialog[divid].open();
        }
    }
    function ExpandCollapse(spanId, elementsId, itemsId, middleBorderId) {
        var span = $([nsConstants.HASH, spanId].join(''));
        var elements = $([nsConstants.HASH, elementsId].join(''));
        var items = $([nsConstants.HASH, itemsId].join(''));
        var middleBorder = $([nsConstants.HASH, middleBorderId].join(''));
        if (span != null) {
            if (span.hasClass("expanded")) {
                span.removeClass("expanded");
                span.addClass("collapsed");
                if (elements != null) {
                    elements.attr('style', 'display:none');
                }
                if (items != null) {
                    items.attr('style', 'display:none');
                }
                if (middleBorder != null) {
                    middleBorder.attr('style', 'display:none');
                }
            }
            else {
                span.removeClass("collapsed");
                span.addClass("expanded");
                if (elements != null) {
                    elements.removeAttr('style');
                }
                if (items != null) {
                    items.removeAttr('style');
                }
                if (middleBorder != null) {
                    middleBorder.removeAttr('style');
                }
            }
        }
    }
    function ExpandCollapseParameters(pId, tableId) {
        var p = $([nsConstants.HASH, pId].join(''));
        var table = $([nsConstants.HASH, tableId].join(''));
        if (p != null) {
            if (p.hasClass("callactivity-arrow-right")) {
                p.removeClass('callactivity-arrow-right');
                p.addClass('callactivity-arrow-down');
                if (table != null) {
                    table.removeAttr('style');
                }
            }
            else {
                p.removeClass('callactivity-arrow-down');
                p.addClass('callactivity-arrow-right');
                if (table != null) {
                    table.attr('style', 'display:none');
                }
            }
        }
    }
    function GetKeyName() {
    }
    function UpdateRule() {
        var jsonvalue = $('#lscriptRuleData').html();
        var data = jQuery.parseJSON(jsonvalue);
        CreateRuleHtml(data, $("#ddlEffectiveDate").val());
        SASDisplayRule.updateView();
    }
    function UpdateView() {
        var selectedView = $('#ddlView option:selected').text();
        var jsonvalue = $('#lscriptRuleData').html();
        var data = JSON.parse(jsonvalue);
        if (selectedView == "Analyst View") {
            setAnalystView(data.RuleType);
        }
        else {
            setDeveloperView(data.RuleType);
        }
    }
    function setAnalystView(ruleType) {
        if (ruleType == "LogicalRule") {
            $(".tbl-assign").parent("span").css("margin-top", "15px");
            $(".default-text-assign").css("height", "22px");
            $(".tbl-assign").each(function () {
                $(this).find("tr").each(function (index) {
                    if (index > 0) {
                        $(this).hide();
                    }
                });
            });
        }
        else if (ruleType == "DecisionTable") {
            $(".dt-assign-header, .dt-assign, .dt-row-col-header, .dt-if").each(function () {
                $(this).find("table tr").each(function (index) {
                    if (index > 0) {
                        $(this).hide();
                    }
                });
            });
        }
    }
    function setDeveloperView(ruleType) {
        if (ruleType == "LogicalRule") {
            $(".tbl-assign").parent("span").removeAttr("style");
            $(".default-text-assign").removeAttr("style");
            $(".tbl-assign").each(function () {
                $(this).find("tr").each(function (index) {
                    if (index > 0) {
                        $(this).show();
                    }
                });
            });
        }
        else if (ruleType == "DecisionTable") {
            $(".dt-assign-header, .dt-assign, .dt-row-col-header, .dt-if").each(function () {
                $(this).find("table tr").each(function (index) {
                    if (index > 0) {
                        $(this).show();
                    }
                });
            });
        }
    }
    function GetRuleByEffectiveDate(data, effectiveDate) {
        if (effectiveDate === undefined || effectiveDate == null) {
            effectiveDate = "Default";
        }
        var rule;
        var defaultRule = null;
        for (var i in data.Rules) {
            var ruleEffectiveDate;
            if (data.Rules[i].EffectiveDate == null || data.Rules[i].EffectiveDate == "" || data.Rules[i].EffectiveDate == "Default") {
                ruleEffectiveDate = "Default";
                defaultRule = data.Rules[i];
            }
            else {
                ruleEffectiveDate = data.Rules[i].EffectiveDate;
            }
            if (ruleEffectiveDate == effectiveDate) {
                rule = data.Rules[i];
                break;
            }
        }
        if (rule === undefined || rule == null) {
            if (defaultRule === undefined || defaultRule == null) {
                return data.Rules[0];
            }
            else {
                return defaultRule;
            }
        }
        else {
            return rule;
        }
    }
    function LoadEffectiveDates(data) {
        var html = [html, ""].join('');
        for (var i in data.Rules) {
            if (data.Rules[i].EffectiveDate == null || data.Rules[i].EffectiveDate == "" || data.Rules[i].EffectiveDate == "Default") {
                html = [html, "<option value=\"Default\">Default</option>"].join('');
            }
            else {
                html = [html, ["<option value=\"", data.Rules[i].EffectiveDate, "\">", data.Rules[i].EffectiveDate, "</option>"].join('')].join('');
            }
        }
        $("#ddlEffectiveDate").html(html);
    }
    function LoadChildRule(selectedView) {
        //ns = {};
        ns.SiteName = ["/", location.pathname.split("/")[1]].join('');
        var jsonvalue = $('#lscriptRuleData').html();
        var data = jQuery.parseJSON(jsonvalue);
        if (data != undefined) {
            LoadEffectiveDates(data);
            CreateRuleHtml(data);
        }
        if (selectedView != null && selectedView != '') {
            $(["#ddlView option:contains('", selectedView, "')"].join('')).attr("selected", true);
            SASDisplayRule.updateView();
        }
    }
    function LoadRule(data, astrControlId, ablnOpenInPopUp) {
        $("#jsonvalue").val(JSON.stringify(data));
        LoadEffectiveDates(data);
        CreateRuleHtml(data);
        OpenPopup(data, true, astrControlId, ablnOpenInPopUp);
    }
    SASDisplayRule.loadRule = LoadRule;
    SASDisplayRule.loadChildRule = LoadChildRule;
    SASDisplayRule.expandCollapse = ExpandCollapse;
    SASDisplayRule.expandCollapseParameters = ExpandCollapseParameters;
    SASDisplayRule.onCallLogicalRuleDoubleClick = OnCallLogicalRuleDoubleClick;
    SASDisplayRule.updateRule = UpdateRule;
    SASDisplayRule.updateView = UpdateView;
})(SASDisplayRule || (SASDisplayRule = {}));

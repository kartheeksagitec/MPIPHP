var MVVM, MVVMGlobal = {}, ns = {};
(function (MVVM) {
    var JQueryControls;
    (function (JQueryControls) {
        var GridView = (function () {
            function GridView(element, astrActiveDivID, aobjData) {
                this.element = element;
                this.jsObject = undefined;
                this.istrActiveDivID = astrActiveDivID;
                this.istrGridId = this.element[0].id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
                if (aobjData.IsChartGrid === true) {
                    this.iblnChartGrid = true;
                    this.initChartGrid(aobjData.GridOptions);
                }
                else {
                    this.iblnChartGrid = false;
                    this.istrFormContainerId = aobjData.FormContainerId;
                    this.istrCollectionOf = aobjData.data.DomainModel.KeysData[["CollectionOf_", this.istrGridId].join('')];
                    this.istrGridPath = [this.istrFormContainerId, nsConstants.SPACE_HASH, astrActiveDivID, nsConstants.SPACE_HASH, this.element[0].id].join('');
                    this.idomGridElement = $([this.istrFormContainerId, nsConstants.SPACE_HASH, astrActiveDivID, nsConstants.SPACE_HASH, this.istrGridId].join(''));
                    this.iobjGridData = aobjData.data.DomainModel.DetailsData[this.istrGridId];
                    this.beforeInit();
                }
            }
            Object.defineProperty(GridView.prototype, "iintRecordLength", {
                get: function () {
                    if (this.jsObject == undefined)
                        return 0;
                    return this.jsObject.totalRecords;
                },
                enumerable: true,
                configurable: true
            });
            ;
            Object.defineProperty(GridView.prototype, "iarrAllColumns", {
                get: function () {
                    if (this.jsObject == undefined)
                        return [];
                    return this.jsObject.columns;
                },
                enumerable: true,
                configurable: true
            });
            ;
            /**
            * Method to check existance of grid instance, & if exist  then, destroy the grid.
            */
            GridView.checkExistanceAndDestroy = function (adomGridElement, astrActiveDivId) {
                var lblnDestroyed = false;
                var lobjGridWidget = nsCommon.GetWidgetControl(adomGridElement);
                if (lobjGridWidget != undefined && lobjGridWidget.jsObject != undefined) {
                    lobjGridWidget.jsObject.destroy();
                    lobjGridWidget.jsObject = null;
                    lobjGridWidget = null;
                    lblnDestroyed = true;
                }
                else if (adomGridElement.length > 0 && adomGridElement.data("neoGrid") != undefined) {
                    adomGridElement.data("neoGrid").destroy();
                    lblnDestroyed = true;
                }
                if (lblnDestroyed && adomGridElement.length > 0 && adomGridElement[0].id.indexOf(nsConstants.GRID_TABLE_UNDERSCORE) === 0) {
                    var ldomGridToBound = $([nsConstants.HASH, astrActiveDivId, nsConstants.SPACE_HASH, adomGridElement[0].id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "")].join(''));
                    if (ldomGridToBound.attr("RenderingMode") === 'old') {
                        adomGridElement.empty();
                    }
                }
            };
            /**
            *Method will be called before initialize grid view instance.
            */
            GridView.prototype.beforeInit = function () {
                this.setProperties();
                this.idomGridElement[0].innerHTML = "";
                this.setSortExpression();
                if (this.istrGridMode === 'old') {
                    this.createColumnsForOldMode();
                }
                else {
                    this.createColumnsForNewMode();
                    this.createAndApplyTemplate();
                }
                this.setDataSourceOptions();
            };
            GridView.prototype.setProperties = function () {
                var ldomGridElement = this.idomGridElement;
                this.istrGridMode = ldomGridElement.attr("RenderingMode");
                this.istrGridSelection = ldomGridElement.attr("sfwSelection");
                this.istrGridSelection = (this.istrGridSelection === "Many") ? "multiple" : ((this.istrGridSelection === "One") ? "single" : "none");
                this.iintPageSize = ldomGridElement[0].getAttribute("PageSize") != undefined ? parseInt(ldomGridElement[0].getAttribute("PageSize"), 10) : 10;
                ldomGridElement.attr("sfwCollectionOf", this.istrCollectionOf);
                this.iblnGroupable = ldomGridElement[0].getAttribute("AllowGrouping") === "True";
                this.iblnFilterable = ldomGridElement[0].getAttribute("AllowFiltering") === "True";
                this.iblnEditable = ldomGridElement[0].getAttribute("AllowEditing") === "True";
                this.iblnSortable = ldomGridElement[0].getAttribute("AllowSorting") === "True";
                this.iblnShowGridHeader = ldomGridElement[0].getAttribute("ShowHeader") === "True" || ldomGridElement[0].getAttribute("ShowHeader") == null;
                this.iblnPaging = ldomGridElement[0].getAttribute("AllowPaging") === "True";
                this.istrEmptyDataText = ldomGridElement[0].getAttribute("EmptyDataText") != undefined ? ldomGridElement[0].getAttribute("EmptyDataText") : "No records to display.";
                this.iblnShowHeaderWhenEmpty = ldomGridElement[0].getAttribute("ShowHeaderWhenEmpty") == "True" || ldomGridElement[0].getAttribute("ShowHeaderWhenEmpty") == null;
                if (this.iblnSortable) {
                    this.iobjSortable = {
                        allowUnsort: true
                    };
                }
                this.iobjPageable = {};
                if (this.iblnPaging && !ns.iblnVisuallyImpaired) {
                    this.iobjPageable = true;
                }
                else {
                    this.iobjPageable = false;
                    this.iblnPaging = false;
                }
                var lblnGotoLastPage = this.iobjGridData["GotoLastPage"] === true;
                this.iintGotoPageNo = 1;
                if (lblnGotoLastPage) {
                    if (this.iobjGridData.Records.length % this.iintPageSize == 0)
                        this.iintGotoPageNo = this.iobjGridData.Records.length / this.iintPageSize;
                    else
                        this.iintGotoPageNo = Math.floor(this.iobjGridData.Records.length / this.iintPageSize) + 1;
                }
                if (this.istrGridMode !== "old") {
                    // check for guid attribute.If not present generate guid and append it as 
                    // attribute any apply same as class name which will be used to fetch the grid in rowtemplate 
                    this.istrGridGuid = $(this.element).attr("guid");
                    if (this.istrGridGuid == undefined) {
                        this.istrGridGuid = MVVMGlobal.Generateguid();
                        $(this.element).attr("guid", this.istrGridGuid);
                        $(this.element).addClass(this.istrGridGuid);
                    }
                    $(this.element).show();
                }
            };
            /**
            *Method is used to initialize grid view instance.
            */
            GridView.prototype.init = function () {
                this.jsObject = $(this.element).neoGrid(this.getGridOptions()).data("neoGrid");
                this.afterInit();
            };
            /**
            *Method is used to initialize chart grid view instance.
            */
            GridView.prototype.initChartGrid = function (aobjGridOptions) {
                this.jsObject = $(this.element).neoGrid(aobjGridOptions).data("neoGrid");
            };
            /**
           *Method to to get grid options
           */
            GridView.prototype.getGridOptions = function () {
                var lobjGriOptions = {
                    selection: this.istrGridSelection,
                    dataSource: this.iobjDataSourceOptions,
                    groupable: this.iblnGroupable,
                    sortable: this.iblnSortable,
                    scrollable: false,
                    filterable: this.iblnFilterable,
                    pageable: this.iobjPageable,
                    navigatable: ns.iblnVisuallyImpaired,
                    columns: this.iarrGridColumns,
                    dataBound: this.onDataBound,
                    CellFormatAttributes: this.idomGridElement.attr("CellFormatAttributes") != undefined ? nsCommon.Eval('(' + this.idomGridElement.attr("CellFormatAttributes") + ')') : null,
                    RowFormatAttributes: this.idomGridElement.attr("RowFormatAttributes") != undefined ? nsCommon.Eval('(' + this.idomGridElement.attr("RowFormatAttributes") + ')') : null,
                    ActiveDivId: this.istrActiveDivID
                };
                if (this.istrGridMode === 'old') {
                    //lobjGriOptions.excelExport = this.onExportToExcel;
                    lobjGriOptions.editable = this.iblnEditable;
                }
                else {
                    var ldomTemplateContainer = $([this.istrFormContainerId, nsConstants.SPACE_HASH, this.istrActiveDivID, nsConstants.SPACE_HASH, this.istrGridId, "-row-template"].join(''));
                    lobjGriOptions.rowTemplate = ldomTemplateContainer.html();
                    lobjGriOptions.iblnRowTemplate = true;
                }
                return lobjGriOptions;
            };
            /**
            *Method will be called after initialization of grid view instance.
            */
            GridView.prototype.afterInit = function () {
                // this.movePagerAtTop();
                if (this.iblnShowGridHeader === false)
                    this.element.find("thead").hide();
                var lblnHeaderVisible = true;
                //AK : var lblnHeaderVisible = MVVMGlobal.GetControlAttribute(this.element, "sfwHeaderVisible", this.istrActiveDivID);
                if (lblnHeaderVisible !== undefined && lblnHeaderVisible === "False") {
                    this.element.find("thead").remove();
                }
                if (this.istrActiveDivID.indexOf("_retrieve") < 0) {
                    this.element.tooltip({
                        items: "td.hasTooltip",
                        tooltipClass: "s-grid-tooltip",
                        content: function (e) {
                            var container = $(this); //e.target;
                            container.removeAttr("title");
                            var tooltipatrribute = container.attr('tooltip');
                            if (tooltipatrribute !== null) {
                                var ActiveDivID = nsCommon.GetActiveDivId(container);
                                var GridID = $(container).closest('[data-role=neogrid]')[0].id;
                                var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, GridID);
                                var GridData = lobjGridWidget.jsObject.dataSource.data;
                                var rowIndex = container.closest('tr').attr("rowIndex");
                                var dataItem = lobjGridWidget.getRowByIndex(rowIndex);
                                if (dataItem[tooltipatrribute] != undefined) {
                                    return MVVMGlobal.htmlEncode(dataItem[tooltipatrribute]);
                                }
                                return MVVMGlobal.htmlEncode(tooltipatrribute);
                            }
                            return;
                        }
                    });
                }
                // ns.AddGridConstraints(this.element, this.iobjGridData);
            };
            /**
            *Method to grid pager at the top of the grid
            */
            GridView.prototype.movePagerAtTop = function () {
            };
            /**
            *Method to create columns for grid in old mode
            */
            GridView.prototype.createColumnsForOldMode = function () {
                var cols = MVVMGlobal.GetControlAttribute(this.idomGridElement, "sfwColumns", this.istrActiveDivID);
                this.iarrGridColumns = nsCommon.Eval(['(', cols, ')'].join(''));
                var lobjFieldTypes = this.iobjGridData.FieldsType;
                var liinIndex;
                this.iarrAggregateItems = [];
                for (liinIndex = 1; liinIndex < this.iarrGridColumns.length; liinIndex++) {
                    if (this.iarrGridColumns[liinIndex].field !== undefined && lobjFieldTypes[this.iarrGridColumns[liinIndex].field] !== undefined) {
                        if (this.iarrGridColumns[liinIndex].attributes != null) {
                            if (lobjFieldTypes[this.iarrGridColumns[liinIndex].field].align == "right") {
                                if (this.iarrGridColumns[liinIndex].attributes["style"] != null) {
                                    this.iarrGridColumns[liinIndex].attributes["style"] = this.iarrGridColumns[liinIndex].attributes["style"].replace("text-align:inherit", "text-align:right");
                                }
                                else {
                                    this.iarrGridColumns[liinIndex].attributes["style"] = "text-align:right";
                                }
                            }
                            if (this.iarrGridColumns[liinIndex].attributes["style"] != null && this.iarrGridColumns[liinIndex].attributes["style"].indexOf("text-align:inherit") == 0) {
                                this.iarrGridColumns[liinIndex].attributes["style"] = "text-align:left";
                            }
                        }
                    }
                    var lstrFormat = this.iarrGridColumns[liinIndex].format;
                    //Adding precision count for Pecentage Format
                    if (lstrFormat != undefined && lstrFormat.toUpperCase().indexOf("{0:P") === 0) {
                        lstrFormat = neoFormat.getPercentagePrecision(lstrFormat);
                        lstrFormat = ["{0:0", lstrFormat, "'%}"].join("");
                        this.iarrGridColumns[liinIndex].format = lstrFormat;
                    }
                    // if sfwShowInExportToExcel is present then set hidden property of column to true 
                    if (this.iarrGridColumns[liinIndex].sfwShowInExportToExcel != undefined && this.iarrGridColumns[liinIndex].sfwShowInExportToExcel.trim() == "True") {
                        this.iarrGridColumns[liinIndex].hidden = true;
                    }
                    if (this.iarrGridColumns[liinIndex].field !== undefined && lobjFieldTypes[this.iarrGridColumns[liinIndex].field] !== undefined && this.iarrGridColumns[liinIndex].editable === false) {
                        lobjFieldTypes[this.iarrGridColumns[liinIndex].field].editable = false;
                    }
                    if (this.iarrGridColumns[liinIndex].field !== undefined && lobjFieldTypes[this.iarrGridColumns[liinIndex].field] !== undefined && this.iarrGridColumns[liinIndex].CustomAttributes !== undefined) {
                        lobjFieldTypes[this.iarrGridColumns[liinIndex].field].CustomAttributes = this.iarrGridColumns[liinIndex].CustomAttributes;
                    }
                    if (this.iarrGridColumns[liinIndex].field !== undefined && lobjFieldTypes[this.iarrGridColumns[liinIndex].field] !== undefined && this.iarrGridColumns[liinIndex].maxlength !== undefined) {
                        lobjFieldTypes[this.iarrGridColumns[liinIndex].field].maxlength = this.iarrGridColumns[liinIndex].maxlength;
                    }
                    if (this.iarrGridColumns[liinIndex].field != undefined && lobjFieldTypes[this.iarrGridColumns[liinIndex].field] != undefined && lobjFieldTypes[this.iarrGridColumns[liinIndex].field].format != undefined) {
                        var lstrFormat = lobjFieldTypes[this.iarrGridColumns[liinIndex].field].format;
                        //Adding precision count for Pecentage Format
                        if (lstrFormat != undefined && lstrFormat.toUpperCase().indexOf("{0:P") === 0) {
                            lstrFormat = neoFormat.getPercentagePrecision(lstrFormat);
                            lstrFormat = ["{0:0", lstrFormat, "'%}"].join("");
                            lobjFieldTypes[this.iarrGridColumns[liinIndex].field].format = lstrFormat;
                        }
                        this.iarrGridColumns[liinIndex].format = lstrFormat;
                        if (this.iarrGridColumns[liinIndex].format === '{0:(###)###-####}' && this.iarrGridColumns[liinIndex].editable == true) {
                            this.iarrGridColumns[liinIndex].template = "#= (" + this.iarrGridColumns[liinIndex].field + ") ? formatPhoneNumber(" + this.iarrGridColumns[liinIndex].field + ") : '' #";
                        }
                    }
                    var lstrFooterTemplateType = this.iarrGridColumns[liinIndex].FooterTemplateType;
                    var lstrFooterTemplateText = this.iarrGridColumns[liinIndex].FooterTemplateText;
                    delete this.iarrGridColumns[liinIndex].FooterTemplateType;
                    delete this.iarrGridColumns[liinIndex].FooterTemplateText;
                    if ((lstrFooterTemplateType !== undefined || lstrFooterTemplateText !== undefined) && this.iobjGridData.Records.length > 0) {
                        if (lstrFooterTemplateType !== undefined && lstrFooterTemplateType !== "") {
                            var aggr = {};
                            if (lstrFooterTemplateType.indexOf(",") > 0) {
                                var larrType = lstrFooterTemplateType.split(",");
                                var lintCnt;
                                for (lintCnt = 0; lintCnt < larrType.length; lintCnt++) {
                                    aggr = {};
                                    aggr.field = this.iarrGridColumns[liinIndex].field;
                                    aggr.aggregate = larrType[lintCnt].trim();
                                    this.iarrAggregateItems.push(aggr);
                                }
                            }
                            else {
                                aggr.field = this.iarrGridColumns[liinIndex].field;
                                aggr.aggregate = lstrFooterTemplateType.trim();
                                this.iarrAggregateItems.push(aggr);
                            }
                        }
                        var larrColsaggregate;
                        if (lstrFooterTemplateText !== undefined && lstrFooterTemplateText.indexOf("{0}") > 0) {
                            this.iarrGridColumns[liinIndex].footerTemplate = lstrFooterTemplateText;
                            var larrType = lstrFooterTemplateType.split(",");
                            larrColsaggregate = [];
                            var lintCnt;
                            for (lintCnt = 0; lintCnt < larrType.length; lintCnt++) {
                                this.iarrGridColumns[liinIndex].footerTemplate = String(this.iarrGridColumns[liinIndex].footerTemplate).replace("{" + lintCnt + "}", "#=ns.ApplyCustomFormatForGrid('" + this.iarrGridColumns[liinIndex].format + "'," + larrType[lintCnt] + ") #");
                                larrColsaggregate.push(larrType[lintCnt]);
                            }
                            this.iarrGridColumns[liinIndex].aggregate = larrColsaggregate;
                        }
                        else {
                            this.iarrGridColumns[liinIndex].footerTemplate = (lstrFooterTemplateText !== undefined ? lstrFooterTemplateText : "") + (lstrFooterTemplateType !== undefined && lstrFooterTemplateType !== "" ? " #=ns.ApplyCustomFormatForGrid('" + this.iarrGridColumns[liinIndex].format + "'," + lstrFooterTemplateType + ") #" : "");
                            larrColsaggregate = [lstrFooterTemplateType];
                            this.iarrGridColumns[liinIndex].aggregate = larrColsaggregate;
                        }
                    }
                    //When Grouping is enabled on the grid, then show the grouping count for the column.
                    if (this.iblnGroupable) {
                        if (this.iarrGridColumns[liinIndex].aggregates == undefined || this.iarrGridColumns[liinIndex].aggregates.legth <= 0) {
                            this.iarrGridColumns[liinIndex].aggregates = ["Count"];
                        }
                        else if (this.iarrGridColumns[liinIndex].aggregates != undefined || this.iarrGridColumns[liinIndex].aggregate.length > 0 && this.iarrGridColumns[liinIndex].aggregates.indexOf("Count") < 0) {
                            this.iarrGridColumns[liinIndex].aggregates.push("Count");
                        }
                        this.iarrGridColumns[liinIndex].groupHeaderTemplate = this.iarrGridColumns[liinIndex].title + ": #= value # , Total: #= Count #";
                    }
                }
            };
            /**
            *Method to create columns for grid in new mode
            */
            GridView.prototype.createColumnsForNewMode = function () {
                var lblnGroupable = this.iblnGroupable;
                var lobjGridData = this.iobjGridData;
                var lstrGridId = this.istrGridId;
                var lobjGrid = this;
                this.iarrGridColumns = [];
                this.element.find("th").each(function (e) {
                    var aggr = {};
                    var col = {};
                    var footerText = "", footerType = "";
                    col.field = $(this).attr("data-field");
                    var lstrCellStyle = $(this).attr("itemstyle");
                    if (lstrCellStyle != undefined && lstrCellStyle != "") {
                        col.attributes = {};
                        col.attributes.style = lstrCellStyle;
                        col.style = lstrCellStyle;
                    }
                    var lstrHeaderStyle = $(this).attr("style");
                    if (lstrHeaderStyle != undefined && lstrHeaderStyle != "") {
                        col.headerAttributes = {};
                        col.headerAttributes.style = lstrHeaderStyle;
                    }
                    col.sfwShowInExportToExcel = $(this).attr("sfwShowInExportToExcel");
                    if (col.field == "rowIndex") {
                        col.hidden = true;
                    }
                    // set header to display none if sfwShowInExportToExcel is set true
                    if (col.sfwShowInExportToExcel != undefined && col.sfwShowInExportToExcel.trim() == "True") {
                        col.sfwShowInExportToExcel = true;
                        $(this).css("display", "none");
                    }
                    col.title = $(this).text();
                    //Check for title in table column header template
                    if ($(this).attr("data-title") == undefined) {
                        $(this).attr("data-title", col.title);
                    }
                    if (lobjGridData.HiddenColumns != null) {
                        if (lobjGridData.HiddenColumns[col.field] !== undefined) {
                            return;
                        }
                    }
                    footerType = $(this).attr("footertemplatetype");
                    footerText = $(this).attr("footertemplatetext");
                    if ((footerType !== undefined || footerText !== undefined) && lobjGridData.Records.length > 0) {
                        if (footerType !== undefined && footerType !== "") {
                            if (footerType.indexOf(",") > 0) {
                                var arrType = footerType.split(",");
                                for (var i = 0; i < arrType.length; i++) {
                                    aggr = {};
                                    aggr.field = $(this).attr("data-field");
                                    aggr.aggregate = arrType[i];
                                    lobjGrid.iarrAggregateItems.push(aggr);
                                }
                            }
                            else {
                                aggr.field = $(this).attr("data-field");
                                aggr.aggregate = $(this).attr("footertemplatetype");
                                lobjGrid.iarrAggregateItems.push(aggr);
                            }
                        }
                        if (footerText !== undefined && footerText.indexOf("{0}") > 0) {
                            col.footerTemplate = footerText;
                            var arrType = footerType.split(",");
                            for (var i = 0; i < arrType.length; i++) {
                                col.footerTemplate = col.footerTemplate.replace("{" + i + "}", "#=" + arrType[i] + nsConstants.HASH);
                            }
                        }
                        else {
                            col.footerTemplate = (footerText !== undefined ? footerText : "") + (footerType !== undefined && footerType !== "" ? " #=" + footerType + nsConstants.HASH : "");
                        }
                    }
                    //When Grouping is enable on the grid, then show the grouping count
                    if (lblnGroupable) {
                        if (col.aggregates == undefined || col.aggregates.legth <= 0) {
                            col.aggregates = ["count"];
                        }
                        else if (col.aggregates != undefined || col.aggregates.length > 0 && col.aggregates.indexOf("Count") < 0) {
                            col.aggregates.push("count");
                        }
                        col.groupHeaderTemplate = col.title + ": #= value# , Total: #= count#";
                        if ($(this).attr("data-aggregates") == undefined) {
                            $(this).attr("data-aggregates", col.aggregates.join(','));
                        }
                    }
                    lobjGrid.iarrGridColumns.push(col);
                });
            };
            /**
            *Method to create template & apply for grid in new mode
            */
            GridView.prototype.createAndApplyTemplate = function () {
                var lobjGrid = this;
                var ldomTemplateContainer = $([lobjGrid.istrFormContainerId, nsConstants.SPACE_HASH, lobjGrid.istrActiveDivID, nsConstants.SPACE_HASH, lobjGrid.istrGridId, "-row-template"].join(''));
                var lstrRowTemplateText = ldomTemplateContainer.html();
                // removes script above tr to avoid breaking while converting to Jquery element 
                var larrColSelector = lstrRowTemplateText.split("<tr")[0];
                if (larrColSelector != "") {
                    lstrRowTemplateText = lstrRowTemplateText.replace(larrColSelector, "");
                }
                var ldomNewRowTemplate = $(lstrRowTemplateText);
                ldomNewRowTemplate.find("td").each(function () {
                    for (var i in lobjGrid.iobjGridData.HiddenColumns) {
                        var colname = lobjGrid.iobjGridData.HiddenColumns[i];
                        if (colname.indexOf("dt_") == 0 && $(this).text().indexOf(colname) > 0) {
                            $(this).remove();
                        }
                    }
                });
                //set align right to text in numeric columns
                var ldictGridFields = lobjGrid.iobjGridData.FieldsType;
                var dataContainer;
                if (ldictGridFields != undefined) {
                    var colField;
                    for (colField in ldictGridFields) {
                        if (ldictGridFields[colField] !== undefined && ldictGridFields[colField].align == "right") {
                            dataContainer = ldomNewRowTemplate.find(["[data-bind*='", colField, "'],[databind*='", colField, "']"].join(''));
                            if (dataContainer != undefined && dataContainer.length === 1) {
                                dataContainer.closest('td').attr("style", "text-align: right !important");
                            }
                        }
                    }
                }
                //check for each columns if any style attribute supplied from studio it will be overriding the existing style value  
                var column;
                for (column in lobjGrid.iarrGridColumns) {
                    if (lobjGrid.iarrGridColumns[column] != undefined && lobjGrid.iarrGridColumns[column].field != undefined && lobjGrid.iarrGridColumns[column].field != " ") {
                        dataContainer = ldomNewRowTemplate.find(["[data-bind*='", lobjGrid.iarrGridColumns[column].field, "'],[databind*='", lobjGrid.iarrGridColumns[column].field, "'],[data-field='", lobjGrid.iarrGridColumns[column].field, "']"].join(''));
                        if (dataContainer != undefined && dataContainer.length === 1) {
                            // fetch the style attribute into a variable. If any column specific style present append it to variable
                            // add row template condition to hide column if sfwShowInExportToExcel present and add value present in 
                            // variable with rowstyle attribute to the td node and remove the style attribute of td node.
                            var lstrtdStyle = dataContainer.closest('td').attr("style") != undefined ? [dataContainer.closest('td').attr("style"), ";"].join('') : "";
                            if (lobjGrid.iarrGridColumns[column].style != undefined) {
                                lstrtdStyle = [lstrtdStyle, lobjGrid.iarrGridColumns[column].style].join('');
                            }
                            lstrtdStyle = [lstrtdStyle, "#:columns[", parseInt(column), "].sfwShowInExportToExcel ? 'display:none;' : '' # "].join('');
                            dataContainer.closest('td').attr("rowstyle", lstrtdStyle);
                            dataContainer.closest('td').removeAttr("style");
                        }
                    }
                }
                //  var lstrReplaceWithString = " # if (rowSelect===\"on\") { # checked='checked' # }# ";
                var lstrReplaceWithString = " ";
                var lstrFinalString = ldomNewRowTemplate[0].outerHTML;
                // fetch the grid columns using the guid based class in row template 
                //  lstrFinalString = ["#= var columns = $(\".", lobjGrid.istrGridGuid, "\").data(\"neoGrid\").columns; # ", lstrFinalString].join('');
                lstrFinalString = lstrFinalString.replace("}=\"\"", "");
                lstrFinalString = lstrFinalString.replace("#=\"\"", "");
                lstrFinalString = lstrFinalString.replace("}#=\"\"", "");
                lstrFinalString = lstrFinalString.replace("{=\"\"", "");
                lstrFinalString = lstrFinalString.replace("(rowselect='==\"on\")'", "");
                lstrFinalString = lstrFinalString.replace("checked=\"checked\"", "");
                lstrFinalString = lstrFinalString.replace("if=\"\"", lstrReplaceWithString);
                lstrFinalString = lstrFinalString.replace(/\#[:columns](.+?)\#/g, "");
                // replace all the rowstyle attribute presnt in the rowtemplate  with style
                var lintRowStyleCnt = lstrFinalString.indexOf("rowstyle");
                while (lintRowStyleCnt >= 0) {
                    lstrFinalString = lstrFinalString.replace("rowstyle", "style");
                    lintRowStyleCnt = lstrFinalString.indexOf("rowstyle");
                }
                ldomTemplateContainer.html("").text(lstrFinalString);
                this.istrTemplate = lstrFinalString;
            };
            /**
            *Method to set sort expression for grid
            */
            GridView.prototype.setSortExpression = function () {
                var ldomGridElement = this.idomGridElement;
                this.iarrGridSortExpressions = [];
                var lstrSortExpression = MVVMGlobal.GetControlAttribute(ldomGridElement, "sfwSortExpression", this.istrActiveDivID);
                if (lstrSortExpression != null && $.trim(lstrSortExpression) == "") {
                    console.log(neoFormat.format(DefaultMessages.InvalidSortExpression, this.istrGridId));
                }
                if (lstrSortExpression != null && $.trim(lstrSortExpression) != "") {
                    var lstrSortFields = lstrSortExpression.split(',');
                    for (var cnt = 0; cnt < lstrSortFields.length; cnt++) {
                        var lSortFields = lstrSortFields[cnt].split(' ');
                        var fieldName = lSortFields[0];
                        var fieldOrder = "";
                        if (lSortFields.length > 1) {
                            fieldOrder = lSortFields[1];
                        }
                        else {
                            fieldOrder = "asc";
                        }
                        var SortField = { field: fieldName, dir: fieldOrder };
                        this.iarrGridSortExpressions.push(SortField);
                    }
                    if (lstrSortFields.length > 1 && this.iblnSortable) {
                        this.iobjSortable.mode = "multiple";
                    }
                }
            };
            /**
            *Method to set dataSource options for grid
            */
            GridView.prototype.setDataSourceOptions = function () {
                var lstrHiddenCol;
                var liinIndex;
                for (lstrHiddenCol in this.iobjGridData.HiddenColumns) {
                    var colName = this.iobjGridData.HiddenColumns[lstrHiddenCol];
                    _.remove(this.iarrGridColumns, function (obj) {
                        return obj.field === colName;
                    });
                    if (this.iobjGridData.FieldsType[colName] !== undefined) {
                        delete this.iobjGridData.FieldsType[colName];
                    }
                }
                this.iobjDataSourceOptions = {
                    aggregate: this.iarrAggregateItems,
                    data: this.iobjGridData.Records,
                    pageSize: this.iintPageSize,
                    currentPage: this.iintGotoPageNo,
                    change: this.onChange,
                    schema: {
                        model: {
                            id: "PrimaryKey",
                            fields: this.iobjGridData.FieldsType
                        }
                    },
                    sort: this.iarrGridSortExpressions
                };
            };
            /**
            *Event on data change of grid data source
            */
            GridView.prototype.onChange = function (e) {
                var lstrGridId = e.sender.id;
                var lobjGridWidget = nsCommon.GetWidgetControl(e.sender.element);
                var lstrActiveDivId = lobjGridWidget.istrActiveDivID;
                if (lobjGridWidget != undefined) {
                    var lstrFormContainerID = lobjGridWidget.istrFormContainerId != undefined ? lobjGridWidget.istrFormContainerId : [nsConstants.HASH, $([nsConstants.HASH, lstrActiveDivId].join('')).closest(nsConstants.FORMCONTAINER_SELECTOR)[0].id].join('');
                    if (e.sender.pageSize != e.sender.totalRecords && ns.FilterAppliedToGrid === [lstrFormContainerID, nsConstants.SPACE_HASH, lstrActiveDivId, nsConstants.SPACE_HASH, nsConstants.GRID_TABLE_UNDERSCORE, lstrGridId].join('')) {
                        var lobjGridJsObject = lobjGridWidget.jsObject != undefined ? lobjGridWidget.jsObject : $(lobjGridWidget.element).data("neoGrid");
                        if (lobjGridJsObject.groupedColumns.length <= 1) {
                            nsEvents.RefreshGridChart(lstrFormContainerID, lstrActiveDivId, lstrGridId);
                        }
                        else {
                            ns.ClearGridChart(DefaultMessages.MulitpleGroupingForChart);
                        }
                    }
                    if (e.action === "itemchange") {
                        MVVMGlobal.AddDirtyData(lstrActiveDivId, lobjGridWidget.element, e);
                    }
                }
            };
            /**
            *Event on data bound to  grid
            */
            GridView.prototype.onDataBound = function (e) {
                //var lobjGridElement = $(e.sender.element);
                //var lobjEventControl = e.sender;
                //var lstrActiveDivId = e.sender.options.ActiveDivId;
                //var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(lstrActiveDivId, lobjGridElement[0].id.replace(nsConstants.GRID_TABLE_UNDERSCORE, ""));
                //var lobjGridJsObject = lobjGridWidget.jsObject != undefined ? lobjGridWidget.jsObject : lobjEventControl;
                //if (lobjGridWidget.jsObject == undefined && lobjGridJsObject != undefined) {
                //    lobjGridWidget.jsObject = lobjGridJsObject;
                //}
                //// checking for all rows selected form grid view
                //var lstrGridID = lobjGridElement[0].id;
                ////var ldomChkAll = $(["#checkAll_", lstrGridID.replace(nsConstants.GRID_TABLE_UNDERSCORE, "")].join(''));
                //if (ns.CanStoreInSession()) {
                //    var lobjGridStateInfo = lobjGridWidget.getState();
                //    ns.SessionStorePageState(lstrActiveDivId, "grid", lstrGridID, lobjGridStateInfo);
                //}
                //ns.AddGridConstraints(lobjGridWidget.element, lobjGridWidget.iobjGridData);
                //var lblnCanCheckAll = false;;
                //if (ldomChkAll.length > 0 && ldomChkAll[0] !== undefined) {
                //    lblnCanCheckAll = ldomChkAll[0].getAttribute("CanCheckAll") === nsConstants.TRUE;
                //    if (lblnCanCheckAll) {
                //        var lblnAllSelected = lobjGridWidget.isAllGridViewItemsChecked();
                //        ldomChkAll.prop('checked', lblnAllSelected);
                //    }
                //}
            };
            /**
            *Event on export to excel
            */
            GridView.prototype.onExportToExcel = function (e) {
            };
            /**
            *Hide grid.
            */
            GridView.prototype.hide = function () {
                var lobjGridElement = $(this.element);
                if (lobjGridElement[0].tagName === 'TABLE' && this.istrGridMode !== 'old') {
                    lobjGridElement.closest('.s-grid-container').hide();
                }
                lobjGridElement.hide();
            };
            /**
            *Show grid.
            */
            GridView.prototype.show = function () {
                var lobjGridElement = $(this.element);
                if (lobjGridElement[0].tagName === 'TABLE' && this.istrGridMode !== 'old') {
                    lobjGridElement.closest('.s-grid-container').hide();
                }
                lobjGridElement.show();
            };
            /**
            *Get state of grid.
            */
            GridView.prototype.getState = function () {
                var ldicDataSource = this.jsObject;
                var lobjGridStateInfo = {
                    page: ldicDataSource.currentPage,
                    sort: ldicDataSource.sortFields,
                    group: ldicDataSource.groupedColumns,
                    filter: ldicDataSource.filterColumns,
                    columns: ldicDataSource.columns
                };
                return lobjGridStateInfo;
            };
            /**
            *Restore state of grid.
            @Param {any}  lobjGridStateInfo This is object of state info of grid
            */
            GridView.prototype.restoreState = function (lobjGridStateInfo) {
                if (this.jsObject != undefined) {
                    var sort = lobjGridStateInfo.sort;
                    var page = neoFormat.parseInt(lobjGridStateInfo.page);
                    var group = lobjGridStateInfo.group;
                    var filter = lobjGridStateInfo.filter;
                    this.jsObject.setSort((sort != undefined && $.isArray(sort)) ? sort : []);
                    this.jsObject.setGroup((group != undefined && $.isArray(group)) ? group : []);
                    this.jsObject.currentPage = (page != undefined) ? page : 1;
                    this.jsObject.filterColumns = (lobjGridStateInfo.filter != undefined && $.isArray(filter)) ? filter : [];
                    this.jsObject.restoreState();
                }
            };
            /**
            *Enable grid.
            */
            GridView.prototype.enable = function () {
            };
            /**
            *Disable grid.
            */
            GridView.prototype.disable = function () {
            };
            /**
           *Refresh grid.
           */
            GridView.prototype.refresh = function () {
                if (this.jsObject != undefined) {
                    this.jsObject.refresh();
                }
            };
            /**
            *Destroy grid.
            */
            GridView.prototype.destroy = function () {
                if (this.jsObject != undefined)
                    this.jsObject.destroy();
                this.jsObject = null;
            };
            /**
           *Get data of grid.
           */
            GridView.prototype.getData = function () {
                return this.jsObject.dataSource.data;
            };
            /**
            *Set row property by index.
            *@Param {number | any} aintRowIndex
            *@Param {string} astrProperty
            *@Param {any} aobjValue
            */
            GridView.prototype.setRowPropertyByIndex = function (aintRowIndex, astrProperty, aobjValue, control) {
                var lobjRowData = this.getRowByIndex(aintRowIndex);
                if (lobjRowData != undefined) {
                    lobjRowData[astrProperty] = aobjValue;
                    if (control != undefined && control.length > 0) {
                        control.trigger("change");
                    }
                }
            };
            /**
            *Get row property by index.
            *@Param {number | any} aintRowIndex
            *@Param {string} astrProperty
            */
            GridView.prototype.getRowPropertyByIndex = function (aintRowIndex, astrProperty) {
                var lobjRowData = this.getRowByIndex(aintRowIndex);
                if (lobjRowData != undefined) {
                    return lobjRowData[astrProperty];
                }
                return undefined;
            };
            /**
            *Get row by index.
            *@Param {number | any} aintRowIndex
            */
            GridView.prototype.getRowByIndex = function (aintRowIndex) {
                var larrRowData = this.jsObject.dataSource.data;
                var larrSelectedItems = $.grep(larrRowData, function (aDataRow) {
                    return aDataRow.rowIndex == aintRowIndex;
                });
                aintRowIndex = null;
                if (larrSelectedItems.length > 0)
                    return larrSelectedItems[0];
                return undefined;
            };
            /**
            *Get selected indexes.
            @Param {number | any} aintRowIndex This is optional.
            @Param {boolean} ablnJSON This is optional.
            */
            GridView.prototype.getSelectedRows = function (aintRowIndex, ablnJSON) {
                var larrSelectedRows = [];
                var lUIGrid = this.jsObject;
                var larrData;
                var larrDataRows = lUIGrid.dataSource.data;
                var ldtrSelected;
                if (larrDataRows.length > 0) {
                    if (aintRowIndex != undefined && aintRowIndex != "" && aintRowIndex >= 0) {
                        larrSelectedRows = $.grep(larrDataRows, function (aDataRow) {
                            return aDataRow.rowIndex == aintRowIndex;
                        });
                    }
                    else {
                        larrSelectedRows = $.grep(larrDataRows, function (aDataRow) {
                            return aDataRow.rowSelect == true || aDataRow.rowSelect == "on";
                        });
                    }
                }
                return larrSelectedRows;
            };
            /**
            *Get selected indexes.
            @Param {number | any} aintRowIndex This is optional.
            */
            GridView.prototype.getSelectedIndexes = function (aintRowIndex) {
                var larrSelectedRows = this.getSelectedRows(aintRowIndex, true);
                var larrSelectedIndexes = [];
                if (larrSelectedRows.length > 0) {
                    var lintI = 0;
                    for (lintI = 0; lintI < larrSelectedRows.length; lintI++) {
                        larrSelectedIndexes.push(larrSelectedRows[lintI]["rowIndex"]);
                    }
                }
                return larrSelectedIndexes;
            };
            GridView.prototype.checkRow = function (aDataRow, ablnCheck) {
                var lchkSelected = $(this.element).find("tbody").find(["tr[data-uid=", aDataRow.uid, "]"].join('')).find(".s-grid-check-row");
                aDataRow.rowSelect = ablnCheck;
                if (lchkSelected.length > 0) {
                    lchkSelected[0].checked = ablnCheck;
                }
                lchkSelected = null;
            };
            /**
            *Get selected indexes.
            @Param {number | any} aintRowIndex This is optional.
            @Param {boolean} ablnSelect This is optional.
            */
            GridView.prototype.selectRowByIndex = function (aintRowIndex, ablnSelect) {
                this.setRowPropertyByIndex(aintRowIndex, "rowSelect", ablnSelect);
            };
            /**
            *Method to get dirty rows of the grid
            */
            GridView.prototype.getDirtyRows = function () {
                var larrDataRows = this.jsObject.dataSource.data;
                var lintdataRowsLen = larrDataRows.length;
                var larrModifiedRows = [];
                for (var i = 0; i < lintdataRowsLen; i++) {
                    if (larrDataRows[i].dirty) {
                        larrModifiedRows.push(larrDataRows[i]);
                    }
                }
                return larrModifiedRows;
            };
            /**
            *Method to set view key by row index
            */
            GridView.prototype.setViewKeyByIndex = function (aintRowIndex, astrKey, aobjValue) {
                var larrView = this.jsObject.view;
                larrView[aintRowIndex][astrKey] = aobjValue;
                if (astrKey == "rowSelect") {
                    var lchkSelected = $(this.element).find("tbody").find(["tr[data-uid=", larrView[aintRowIndex]['uid'], "]"].join('')).find(".check_row");
                    if (lchkSelected.length > 0) {
                        lchkSelected[0].checked = aobjValue;
                    }
                }
            };
            /**
            *Method to get view key value by row index & key
            */
            GridView.prototype.getViewKeyByIndex = function (aintRowIndex, astrKey) {
                var resultVal;
                var larrView = this.jsObject.view;
                resultVal = larrView[aintRowIndex][astrKey];
                return resultVal;
            };
            /**
            *Method to set data view by row index & key & will be called recursively when grid is grouped
            */
            GridView.prototype.setDataViewByKey = function (aView, aintRowIndex, astrKey, aobjValue) {
                aView[astrKey] = aobjValue;
                if (astrKey == "rowSelect") {
                    var lchkSelected = $(this.element).find("tbody").find(["tr[data-uid=", aView['uid'], "]"].join('')).find(".check_row");
                    if (lchkSelected.length > 0) {
                        lchkSelected[0].checked = aobjValue;
                    }
                }
            };
            /**
            *Method to get data view by row index & key & will be called recursively when grid is grouped
            */
            GridView.prototype.getDataViewByKey = function (aView, aRowIndex, aKey) {
                return aView[aKey];
            };
            /**
            *Check for all rows in grid view are selected or not
            @Param {any} aobjGridView This is view of the gird & is optional
            */
            GridView.prototype.isAllGridViewItemsChecked = function (aobjGridView) {
                if (aobjGridView == undefined)
                    aobjGridView = this.jsObject.view;
                var lintViewLength = aobjGridView.length;
                var lblnIsAllChecked = true;
                for (var i = 0; i < lintViewLength; i++) {
                    if (aobjGridView[i].rowSelect == false) {
                        lblnIsAllChecked = false;
                        break;
                    }
                }
                return lblnIsAllChecked;
            };
            /**
            *Check all rows of grid view on singel page
            */
            GridView.prototype.checkAll = function (ablnCheckAll) {
                //var ldomChkAll = $([nsConstants.HASH, this.istrActiveDivID, " #checkAll_", this.istrGridId].join(''));
                //ldomChkAll.attr("CanCheckAll", "false");
                //var larrViewRows = this.jsObject.view;
                //var lintViewRowsLen = larrViewRows.length;
                //var lintI = 0;
                //for (lintI = 0; lintI < lintViewRowsLen; lintI++) {
                //    this.setViewKeyByIndex(lintI, "rowSelect", ablnCheckAll);
                //}
                //ldomChkAll.attr("CanCheckAll", nsConstants.TRUE);
            };
            /**
            *Check all rows of grid view on singel page
            */
            GridView.prototype.checkAllPages = function (ablnCheckAll) {
                if (this.jsObject != undefined) {
                    //var lobjFilters = this.jsObject.dataSource.filter();
                    var larrData = this.jsObject.dataSource.data;
                    //var larrDataRows = lobjQuery.filter(lobjFilters).data;
                    var lintRowLength = larrData.length;
                    for (var idx = 0; idx < lintRowLength; idx++) {
                        larrData[idx]["rowSelect"] = ablnCheckAll;
                    }
                }
            };
            GridView.prototype.checkLastSelectedIndex = function (aintRowIndex) {
                var lintLastSelectedIndex = $(this.element).attr("LastSelectedIndex");
                if (aintRowIndex != lintLastSelectedIndex) {
                    if (lintLastSelectedIndex != undefined && lintLastSelectedIndex != "") {
                        this.setRowPropertyByIndex(lintLastSelectedIndex, "rowSelect", false);
                    }
                    if (aintRowIndex <= this.iintRecordLength) {
                        $(this.element).attr("LastSelectedIndex", aintRowIndex);
                    }
                }
            };
            /*
            *Method is used to export grid data
            */
            GridView.prototype.exportToExcel = function () {
                var lobjUiGrid = this.jsObject;
                // var ldictGridOptions = lobjUiGrid.getOptions();
                var larrOptionColumns = lobjUiGrid.columnFields;
                var ldomCheckedCols = $("#DivExportCols input:checked");
                if (ldomCheckedCols.length == 0) {
                    alert(DefaultMessages.SelectColumnToExport);
                    return false;
                }
                var lobjExportCols = {};
                var larrExportCols = [];
                //adding table header
                ldomCheckedCols.each(function () {
                    var title = $(['label[for="', this.id, '"]'].join('')).text().trim();
                    var field = $(this).val();
                    larrExportCols.push(field);
                    lobjExportCols[field] = "";
                });
                var lstrKeyI, lstrTitle;
                for (lstrKeyI in larrOptionColumns) {
                    // Prior saving to Excel change hidden property to false for all the columns for which sfwShowInExportToExcel is true 
                    if (larrOptionColumns[lstrKeyI].sfwShowInExportToExcel != undefined && (larrOptionColumns[lstrKeyI].sfwShowInExportToExcel == true || larrOptionColumns[lstrKeyI].sfwShowInExportToExcel.trim() == "True")) {
                        if (larrExportCols.indexOf(lstrKeyI) < 0) {
                            larrExportCols.push(lstrKeyI);
                        }
                    }
                }
                lobjUiGrid.exportToExcel({ columns: larrExportCols, fileName: "Excel.xlsx" });
                return true;
            };
            /**
            *Get template for export to excel
            */
            GridView.prototype.getColumnTemplateForExportToExcel = function () {
                var lstrHTML = "";
                var Allcolumns = this.element.find('thead').find("th[data-field]");
                var larrColumns = this.jsObject.columns;
                lstrHTML = "<table id='tblExcelColumns'><tr>";
                var lstrTempHtml = "";
                var lintI = 0, lintJ = 0;
                for (var lintJ = 0; lintJ < Allcolumns.length; lintJ++) {
                    var col = Allcolumns[lintJ];
                    if (col.getAttribute('data-field') == "rowSelect" || col.getAttribute('data-field') == "rowIndex") {
                        continue;
                    }
                    // added check to show columns for which sfwShowInExportToExcel attribute is true
                    if ($(col).text() == undefined || $.trim($(col).text()) === 'Select' || ((larrColumns[lintJ].hidden == nsConstants.TRUE || larrColumns[lintJ].hidden == true) && !(larrColumns[lintJ].sfwShowInExportToExcel != undefined && (larrColumns[lintJ].sfwShowInExportToExcel == true || larrColumns[lintJ].sfwShowInExportToExcel.trim() == "True")))) {
                        continue;
                    }
                    lintI++;
                    lstrHTML = [lstrHTML, '<td><label for="ExportChk', lintJ, '"> <input type="checkbox" id="ExportChk', lintJ, '" checked="checked" value="', col.attributes["data-field"].value, '" />', $(col).text(), '</label></td>'].join('');
                    if (lintI % 3 === 0) {
                        lstrHTML = [lstrHTML, "</tr><tr>"].join('');
                    }
                }
                lstrHTML = [lstrHTML, "</tr></table>"].join('');
                return lstrHTML;
            };
            /**
            *Remove selected rows
            */
            GridView.prototype.removeSelectedRows = function () {
                //var larrSelectedRows = this.getSelectedRows(null, false);
                //if (larrSelectedRows.length > 0) {
                this.jsObject.remove(null, true);
                //}
                // this.refresh();
            };
            /**
            *Clear grid filters applied for chart
            */
            GridView.prototype.clearFilters = function () {
                if (this.jsObject != undefined) {
                    this.jsObject.filterColumns.splice(0);
                    this.jsObject.setSort([]);
                    this.jsObject.setGroup([]);
                    this.jsObject.currentPage = 1;
                    this.jsObject.refresh(true);
                }
            };
            /**
            *Get template for chart
            */
            GridView.prototype.getChartTemplate = function () {
                var GridColumns = this.jsObject.columns;
                var filterColumns = this.jsObject.filterColumns;
                var lstrFilterString = "";
                var lstrGroupingString = "";
                if (filterColumns != undefined && filterColumns.length > 0) {
                    var totalFiltes = filterColumns.length;
                    if (totalFiltes > 0) {
                        lstrFilterString = [lstrFilterString, "<table>"].join('');
                        for (var i = 0; i < totalFiltes; i++) {
                            //get field title
                            var item = $.grep(GridColumns, function (a) {
                                return a.field === filterColumns[i].field;
                            });
                            lstrFilterString = [lstrFilterString, "<tr>", "<td>", item[0].title, ": </td><td>", filterColumns[i].filterBox1, "</td></tr>"].join('');
                        }
                        lstrFilterString = [lstrFilterString, "</table>"].join('');
                    }
                }
                var GroupdOn = MVVMGlobal.arrayUnique(ns.FilterAppliedGroup.concat(this.jsObject.groupedColumns));
                if (GroupdOn !== undefined && GroupdOn.length > 0) {
                    for (var i = GroupdOn.length - 1; i < GroupdOn.length; i++) {
                        if (GroupdOn[i] === "")
                            continue;
                        var item = $.grep(GridColumns, function (a) {
                            return a.field === GroupdOn[i];
                        });
                        if (i > 0)
                            lstrGroupingString = [lstrGroupingString, " , ", item[0].title].join('');
                        else
                            lstrGroupingString = [lstrGroupingString, item[0].title].join('');
                    }
                }
                if (lstrFilterString !== "") {
                    lstrFilterString = ["<br/><b>Filtered By</b><br>", lstrFilterString].join('');
                }
                if (lstrGroupingString !== "") {
                    lstrGroupingString = ["<br><b>Grouped By</b><br>", lstrGroupingString].join('');
                }
                return [lstrGroupingString, lstrFilterString].join('');
            };
            /**
            *Move back from chart
            */
            GridView.prototype.moveBackChart = function () {
                if (this.jsObject == undefined)
                    return;
                var filterColumns = this.jsObject.filterColumns;
                var totalFiltes = filterColumns.length;
                if (totalFiltes == 0)
                    return;
                if (ns.FilterAppliedGroup.length === 0 || ns.FilterAppliedGroup[ns.FilterAppliedGroup.length - 1] === undefined) {
                    this.jsObject.setGroup([]);
                    ns.ClearGridChart();
                }
                else {
                    this.jsObject.setGroup([ns.FilterAppliedGroup[ns.FilterAppliedGroup.length - 1]]);
                    if (totalFiltes > 0) {
                        filterColumns.splice(totalFiltes - 1, 1);
                    }
                }
                if (ns.FilterAppliedGroup.length > 0) {
                    ns.FilterAppliedGroup.splice(ns.FilterAppliedGroup.length - 1, 1);
                }
                this.jsObject.currentPage = 1;
                this.jsObject.refresh(true);
            };
            /**
            *Get view chart
            */
            GridView.prototype.getViewForChart = function () {
                var lobjView = {};
                lobjView.iblnContinue = false;
                if (this.jsObject != undefined) {
                    var MainDatasource = this.jsObject.dataSource;
                    if (this.jsObject.groupedColumns.length == 0) {
                        if (ns.FilterAppliedToGrid == [this.istrFormContainerId, nsConstants.SPACE_HASH, this.istrActiveDivID, nsConstants.SPACE_HASH, nsConstants.GRID_TABLE_UNDERSCORE, this.istrGridId].join('')) {
                            return lobjView;
                        }
                        else {
                            ns.ClearGridChart();
                            return lobjView;
                        }
                    }
                    else if (this.jsObject.groupedColumns.length > 1) {
                        ns.ClearGridChart(DefaultMessages.MulitpleGroupingForChart);
                        return lobjView;
                    }
                    ns.FilterAppliedToGrid = [this.istrFormContainerId, nsConstants.SPACE_HASH, this.istrActiveDivID, nsConstants.SPACE_HASH, nsConstants.GRID_TABLE_UNDERSCORE, this.istrGridId].join('');
                    //creating tepm datasource for chart
                    lobjView.views = jQuery.extend(true, {}, this.jsObject.groupedData);
                    if (lobjView.views != undefined && Object.keys(lobjView.views).length <= 0) {
                        ns.ClearGridChart(DefaultMessages.NoRecordForChart);
                        return lobjView;
                    }
                    var lstrField = this.jsObject.groupedColumns[0];
                    lobjView.views = _.map(lobjView.views, function (value, key) {
                        var obj = {
                            aggregates: {},
                            field: lstrField,
                            hasSubgroups: false,
                            items: value,
                            value: key.substring(key.indexOf(":") + 2, key.indexOf(", Total:"))
                        };
                        return obj;
                    });
                    lstrField = null;
                    lobjView.iblnContinue = true;
                }
                return lobjView;
            };
            /**
            *Method will called on series click call back of chart
            */
            GridView.prototype.onSeriesClick = function (aobjSeriesData) {
                if (this.jsObject != undefined) {
                    if (this.jsObject.groupedColumns.length > 0)
                        ns.FilterAppliedGroup.push(this.jsObject.groupedColumns[0]);
                    this.jsObject.setGroup([]);
                    if (this.jsObject.filterColumns.length == 0)
                        this.jsObject.filterColumns.push({ field: aobjSeriesData.NameofGroupedField, selectFilterOptions1: "==", filterBox1: aobjSeriesData.Event.category, filterBox2: "", selectFilterOptions2: "", rdoAndOr: "" });
                    else {
                        for (var i = 0; i < aobjSeriesData.views.length; i++) {
                            for (var j = this.jsObject.filterColumns.length - 1; j >= 0; j--) {
                                if (aobjSeriesData.views[i].field == this.jsObject.filterColumns[j].field) {
                                    this.jsObject.filterColumns.splice(j, 1);
                                }
                            }
                        }
                        this.jsObject.filterColumns.push({ field: aobjSeriesData.NameofGroupedField, selectFilterOptions1: "==", filterBox1: aobjSeriesData.Event.category, filterBox2: "", selectFilterOptions2: "", rdoAndOr: "" });
                    }
                    this.jsObject.currentPage = 1;
                    this.jsObject.refresh(true);
                }
            };
            //Start ns Methods moved here
            /**
            *Method used to make the grid row editable.
            */
            GridView.prototype.makeRowEditable = function () {
            };
            /**
            *Method used to apply rules for grid items.
            */
            GridView.prototype.applyGridItemRules = function () {
            };
            /**
            *Method used to apply cell & row formatting to grid.
            */
            GridView.prototype.applyCellAndRowFormatting = function () {
                var lobjGridElement = $(this.element);
                var lobjGridJsObject = this.jsObject != undefined ? this.jsObject : lobjGridElement.data("neoGrid");
                var lstrGridID = this.istrGridId;
                var lstrActiveDivID = this.istrActiveDivID;
                var CellFormatAttributes = $([nsConstants.HASH, lstrActiveDivID, nsConstants.SPACE_HASH, lstrGridID].join('')).attr("CellFormatAttributes");
                var RowFormatAttributes = $([nsConstants.HASH, lstrActiveDivID, nsConstants.SPACE_HASH, lstrGridID].join('')).attr("RowFormatAttributes");
                if (CellFormatAttributes == null && RowFormatAttributes == null) {
                    return;
                }
                if (lobjGridJsObject.dataSource.data.length == 0) {
                    return;
                }
                var lstrField;
                var lstrVal;
                if (CellFormatAttributes != null) {
                    CellFormatAttributes = nsCommon.Eval(['(', CellFormatAttributes, ')'].join(''));
                    for (lstrField in CellFormatAttributes) {
                        var thText = $(lobjGridElement.find(["th[data-field='", lstrField, "']"].join(''))[0]).text();
                        lobjGridElement.find(["td[data-container-for='", lstrField, "']"].join('')).each(function (e) {
                            var checkval = ""; //lobjGridJsObject.dataItem($(this).parent())[CellFormatAttributes[lstrField].DataField];
                            for (lstrVal in CellFormatAttributes[lstrField]) {
                                if (lstrVal == checkval) {
                                    $(this).addClass(CellFormatAttributes[lstrField][lstrVal]);
                                }
                            }
                        });
                    }
                }
                if (RowFormatAttributes != null) {
                    RowFormatAttributes = nsCommon.Eval(['(', RowFormatAttributes, ')'].join(''));
                    var larrRowData = lobjGridJsObject.dataSource.data;
                    for (lstrField in RowFormatAttributes) {
                        for (lstrVal in RowFormatAttributes[lstrField]) {
                            var classToAdd = RowFormatAttributes[lstrField][lstrVal];
                            var rowsToAddCss = $.grep(larrRowData, function (a) { return a[lstrField] == lstrVal; });
                            for (var i = 0; i < rowsToAddCss.length; i++) {
                                lobjGridElement.find(["tr[data-uid='", rowsToAddCss[i].uid, "']"].join('')).addClass(classToAdd);
                            }
                        }
                    }
                }
            };
            /**
            *Method used to hide columns of the grid
            */
            GridView.prototype.hideGridColumns = function () {
                var lobjGridElement = $(this.element);
                var lobjGridJsObject = this.jsObject != undefined ? this.jsObject : lobjGridElement.data("neoGrid");
                if (this.iobjGridData.HiddenColumns != undefined) {
                    for (var col in this.iobjGridData.HiddenColumns) {
                        lobjGridJsObject.hideColumn(this.iobjGridData.HiddenColumns[col]);
                        if (this.iblnEditable && lobjGridElement.find(["td[data-container-for='", this.iobjGridData.HiddenColumns[col], "']"].join('')).length > 0) {
                            lobjGridElement.find(["td[data-container-for='", this.iobjGridData.HiddenColumns[col], "']"].join('')).hide();
                        }
                    }
                }
            };
            //End ns Methods moved here
            /**
            *Used to add css style to the specified row
            */
            GridView.prototype.highlightRow = function (arowNumber) {
                var lstrSelector = neoFormat.format("tr[rowIndex='{0}']", lstrSelector);
                if (this.element.find(lstrSelector).length > 0) {
                    this.element.find(lstrSelector).addClass("s-grid-invalid-row");
                }
            };
            return GridView;
        }());
        JQueryControls.GridView = GridView;
    })(JQueryControls = MVVM.JQueryControls || (MVVM.JQueryControls = {}));
})(MVVM || (MVVM = {}));
NeoGrid.applyDate = function (control) {
    MVVMGlobal.applyDate(control);
};
//NeoGrid Binders
NeoGrid.bindDropDown = function (element, bindings) {
    element.attr("proceed", "true");
    var PropertyPath = $.trim(bindings.field);
    var gridID = bindings.sender.id.replace("GridTable_", "");
    ns.SenderID = gridID + "." + PropertyPath;
    element.data("sfwProp", PropertyPath);
    var DataItem = ns.isRightSideForm === false ? ns.FormOpenedOnLeft : ns.FormOpenedOnRight;
    var CustomAttributes = {};
    if (element.attr("CustomAttributes") != undefined)
        CustomAttributes = jQuery.parseJSON(element.attr("CustomAttributes") + "}");
    var astrActiveDivID = bindings.sender.options.ActiveDivId;
    var astrFormType = nsCommon.GetFormType(astrActiveDivID);
    var Attributes = {};
    var attrMap = element[0].attributes;
    $.each(attrMap, function (i, e) {
        Attributes[e.nodeName.toLowerCase()] = e.nodeValue;
    });
    for (var key in CustomAttributes) {
        Attributes[key.toLowerCase()] = CustomAttributes[key];
    }
    Attributes["islookup"] = false;
    Attributes["formname"] = nsCommon.GetFormNameFromDivID(ns.viewModel.currentForm);
    if (ns.viewModel[astrActiveDivID] !== undefined && ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"] !== undefined)
        Attributes["primarykey"] = ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"];
    var data = nsCommon.GetOptionsForDropdown(Attributes, astrFormType);
    if (data.DomainModel !== undefined) {
        data = data.DomainModel.HeaderData.DropDownValues.Options;
    }
    if (data.length == 0) {
        data.push({ text: "", value: "" });
    }
    nsCommon.SetDropDownValues($(element), data);
    var valueToSet = bindings.item[PropertyPath];
    element.val(valueToSet);
};
NeoGrid.bindRadioButtonList = function (element, bindings) {
    element.attr("proceed", "true");
    var PropertyPath = $.trim(bindings.field);
    var gridID = bindings.sender.id.replace("GridTable_", "");
    ns.SenderID = gridID + "." + PropertyPath;
    element.data("sfwProp", PropertyPath);
    var DataItem = ns.isRightSideForm === false ? ns.FormOpenedOnLeft : ns.FormOpenedOnRight;
    var CustomAttributes = {};
    if (element.attr("CustomAttributes") != undefined)
        CustomAttributes = jQuery.parseJSON(element.attr("CustomAttributes") + "}");
    var astrActiveDivID = bindings.sender.options.ActiveDivId;
    var astrFormType = nsCommon.GetFormType(astrActiveDivID);
    var Attributes = {};
    var attrMap = element[0].attributes;
    $.each(attrMap, function (i, e) {
        Attributes[e.nodeName.toLowerCase()] = e.nodeValue;
    });
    for (var key in CustomAttributes) {
        Attributes[key.toLowerCase()] = CustomAttributes[key];
    }
    Attributes["gridid"] = gridID;
    Attributes["rowindex"] = bindings.rowIndex;
    Attributes["islookup"] = false;
    Attributes["formname"] = nsCommon.GetProperFormName(astrActiveDivID);
    Attributes[nsConstants.CONTROL_TYPE] = nsConstants.SFW_RADIO_BUTTON_LIST;
    if (ns.viewModel[astrActiveDivID] != undefined && ns.viewModel[astrActiveDivID].ExtraInfoFields != undefined && ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"] != undefined)
        Attributes["primarykey"] = ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"];
    //Check if radiobutton list is populating by items
    var lblnItemGroup = false;
    if (Attributes["sfwcodegroup"] === undefined && Attributes["sfwcodetable"] === undefined && Attributes["sfwcodemethod"] === undefined && Attributes["dropdownoptions"] != undefined && Attributes["dropdownoptions"] != "") {
        lblnItemGroup = true;
    }
    var OptionTemplate = "";
    if (!lblnItemGroup) {
        var data = nsCommon.GetOptionsForDropdown(Attributes, astrFormType);
        if (data != undefined && data.DomainModel != undefined) {
            data = data.DomainModel.HeaderData.DropDownValues.Options;
        }
        if (data != undefined) {
            var controltype = Attributes[nsConstants.CONTROL_TYPE];
            var Template = nsCommon.SetListValues(data, controltype, Attributes);
            OptionTemplate = Template;
        }
    }
    else {
        OptionTemplate = Attributes["dropdownoptions"];
    }
    if (element[0].tabName !== "SPAN") {
        var domSpan = $("<span controltype='sfwradiobuttonlist' data-field='" + bindings.field + "'></span>");
        domSpan.html(OptionTemplate);
        OptionTemplate = domSpan[0].outerHTML;
        domSpan = null;
    }
    element.html(OptionTemplate);
    element.find("input").each(function () {
        $(this).attr(nsConstants.DATA_BIND, ["checked:", bindings.field].join(''));
        $(this).attr("name", [bindings.field, bindings.model.rowIndex].join(''));
    });
    var valueToSet = bindings.item[PropertyPath];
    element.find("input[type=radio]").val([valueToSet]);
};
NeoGrid.bindCheckBox = function (element, bindings) {
    element.attr("proceed", nsConstants.TRUE);
    var PropertyPath = $.trim(bindings.field);
    var gridID = bindings.sender.id.replace("GridTable_", "");
    ns.SenderID = gridID + "." + PropertyPath;
    element.data("sfwProp", PropertyPath);
    var CustomAttributes = {};
    if (bindings.model.fields[PropertyPath] != undefined && bindings.model.fields[PropertyPath].CustomAttributes != undefined)
        CustomAttributes = bindings.model.fields[PropertyPath].CustomAttributes;
    else if (element.attr("CustomAttributes") != undefined)
        CustomAttributes = jQuery.parseJSON(element.attr("CustomAttributes") + "}");
    var checkedVal = "Y";
    var unCheckedVal = "N";
    if (CustomAttributes["sfwValueChecked"] !== undefined)
        checkedVal = CustomAttributes["sfwValueChecked"];
    if (CustomAttributes["sfwValueUnChecked"] !== undefined)
        unCheckedVal = CustomAttributes["sfwValueUnChecked"];
    if (bindings.item[PropertyPath] !== undefined) {
        var lstrLeftSide = bindings.item[PropertyPath];
        if (typeof lstrLeftSide !== 'string') {
            lstrLeftSide = lstrLeftSide.toString();
        }
        $(element)[0].checked = lstrLeftSide.trim() === checkedVal.trim();
    }
};

//Overriden Global methods
MVVMGlobal.GridDropDownEditor = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    var astrActiveDivID = options.sender.options.ActiveDivId;
    var rowIndex = options.rowIndex;
    ns.setSenderData([GridID, ".", options.field].join(''), nsCommon.GetProperFormName(astrActiveDivID), ns.SenderKey);
    var CustomAttributes = options.model.fields[options.field].CustomAttributes;
    var astrFormType = nsCommon.GetFormType(astrActiveDivID);
    var Attributes = {};
    for (var key in CustomAttributes) {
        Attributes[key.trim().toLowerCase()] = CustomAttributes[key.trim()];
    }
    if (Attributes["id"] == "" || Attributes["id"] == undefined) {
        Attributes["id"] = options.field;
    }
    Attributes["gridid"] = GridID;
    Attributes["rowindex"] = rowIndex;
    Attributes["islookup"] = false;
    Attributes["formname"] = nsCommon.GetProperFormName(astrActiveDivID);
    if (ns.viewModel[astrActiveDivID] != undefined && ns.viewModel[astrActiveDivID].ExtraInfoFields != undefined && ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"] != undefined)
        Attributes["primarykey"] = ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"];
    var containerFor = container.attr("data-container-for");
    var lblnItemGroup = false;
    if (Attributes["sfwcodegroup"] === undefined && Attributes["sfwcodetable"] === undefined && Attributes["sfwcodemethod"] === undefined && Attributes["dropdownoptions"] != undefined && Attributes["dropdownoptions"] != "") {
        lblnItemGroup = true;
    }
    else {
        if (nsCommon[["EditableGrid_", astrActiveDivID].join('')] == null) {
            nsCommon[["EditableGrid_", astrActiveDivID].join('')] = {};
        }
        var data = nsCommon[["EditableGrid_", astrActiveDivID].join('')][[GridID, "_", containerFor].join('')];
        if (ns.viewModel[astrActiveDivID] !== undefined && ns.viewModel[astrActiveDivID].DetailsData != undefined && ns.viewModel[astrActiveDivID].DetailsData[GridID].Records[rowIndex.trim()].ListControlData[Attributes["id"]] !== undefined && ns.viewModel[astrActiveDivID].DetailsData[GridID].Records[rowIndex.trim()].ListControlData[Attributes["id"]].length > 0) {
            data = ns.viewModel[astrActiveDivID].DetailsData[GridID].Records[rowIndex.trim()].ListControlData[Attributes["id"]];
        }
        if (data == null) {
            data = nsCommon.GetOptionsForDropdown(Attributes, astrFormType);
            nsCommon[["EditableGrid_", astrActiveDivID].join('')][[GridID, "_", containerFor].join('')] = data;
        }
        var source = data;
        if (data != undefined && data.DomainModel != undefined) {
            if (data.ExtraInfoFields != undefined && data.ExtraInfoFields.AccessDenied != undefined) {
                return;
            }
            source = data.DomainModel.HeaderData.DropDownValues.Options;
        }
    }
    var DropDownControl = $(["<select style='width:100%' name='", options.field, "' data-field='", options.field, "'></select>"].join(''));
    if (!lblnItemGroup) {
        if (source != undefined) {
            $.each(source, function (val, text) {
                DropDownControl.append($('<option></option>').val(text.value).html(text.text == "" && ns.iblnVisuallyImpaired ? "BLANK" : text.text));
            });
        }
    }
    else {
        if (Attributes["dropdownoptions"] != undefined) {
            DropDownControl.html(Attributes["dropdownoptions"]);
        }
    }
    DropDownControl.appendTo(container);
};
MVVMGlobal.GridRadioButtonListEditor = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    var astrActiveDivID = options.sender.options.ActiveDivId;
    var rowIndex = options.rowIndex;
    var CustomAttributes = options.model.fields[options.field].CustomAttributes;
    var astrFormType = nsCommon.GetFormType(astrActiveDivID);
    var Attributes = {};
    for (var key in CustomAttributes) {
        Attributes[key.trim().toLowerCase()] = CustomAttributes[key.trim()];
    }
    Attributes["gridid"] = GridID;
    Attributes["rowindex"] = rowIndex;
    Attributes["islookup"] = false;
    Attributes["islistcontrol"] = true;
    Attributes["formname"] = nsCommon.GetProperFormName(astrActiveDivID);
    Attributes[nsConstants.CONTROL_TYPE] = nsConstants.SFW_RADIO_BUTTON_LIST;
    if (ns.viewModel[astrActiveDivID] != undefined && ns.viewModel[astrActiveDivID].ExtraInfoFields != undefined && ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"] != undefined)
        Attributes["primarykey"] = ns.viewModel[astrActiveDivID].ExtraInfoFields["KeyField"];
    //Check if radiobutton list is populating by items
    var lblnItemGroup = false;
    if (Attributes["sfwcodegroup"] === undefined && Attributes["sfwcodetable"] === undefined && Attributes["sfwcodemethod"] === undefined && Attributes["dropdownoptions"] != undefined && Attributes["dropdownoptions"] != "") {
        lblnItemGroup = true;
    }
    var OptionTemplate = "";
    if (!lblnItemGroup) {
        ns.setSenderData([GridID, ".", options.field].join(''), Attributes["formname"], ns.SenderKey);
        var data = nsCommon.GetOptionsForDropdown(Attributes, astrFormType);
        if (data != undefined && data.DomainModel != undefined) {
            data = data.DomainModel.HeaderData.DropDownValues.Options;
        }
        if (data != undefined) {
            var controltype = Attributes[nsConstants.CONTROL_TYPE];
            var Template = nsCommon.SetListValues(data, controltype, Attributes);
            OptionTemplate = Template;
        }
    }
    else {
        OptionTemplate = Attributes["dropdownoptions"];
    }
    var domSpan = $("<span controltype='sfwradiobuttonlist' data-field='" + options.field + "'></span>");
    domSpan.html(OptionTemplate);
    OptionTemplate = domSpan[0].outerHTML;
    domSpan = null;
    container.html(OptionTemplate);
    container.find("input").each(function () {
        $(this).attr(nsConstants.DATA_BIND, ["checked:", options.field].join(''));
        $(this).attr("name", [options.field, options.model.rowIndex].join(''));
    });
};
MVVMGlobal.GridCheckBoxEditor = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    var CustomAttributes = options.model.fields[options.field].CustomAttributes;
    var checkedValue = "Y";
    var uncheckedValue = "N";
    if (CustomAttributes["sfwValueChecked"] !== undefined)
        checkedValue = CustomAttributes["sfwValueChecked"];
    if (CustomAttributes["sfwValueUnChecked"] !== undefined)
        uncheckedValue = CustomAttributes["sfwValueUnChecked"];
    //var checked = options.model[options.field] === checkedValue ? "checked" : "false"; //onchange="MVVMGlobal.inspectCheckBox(this)"
    var checkbox = $(['<input type="checkbox" GridID="', GridID, '" class="GridCheckBox" rowIndex="', options.model["rowIndex"], '"  name="', options.field, '"  data-field="', options.field, '" />'].join(''));
    for (var key in CustomAttributes) {
        checkbox.attr(key.trim(), CustomAttributes[key.trim()]);
    }
    checkbox[0].checked = options.item[options.field] === checkedValue;
    checkbox.appendTo(container);
};
MVVMGlobal.GridTextAreaEditor = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    var CustomAttributes = options.model.fields[options.field].CustomAttributes;
    var Width = CustomAttributes["Width"] != undefined ? ["style=\"width:", CustomAttributes["Width"], "\"", " wrap=\"hard\""].join('') : "";
    var Class = CustomAttributes["CssClass"] != undefined ? ["class=\"", CustomAttributes["CssClass"], "\" "].join('') : "class=\"GridTextArea\" ";
    var textarea = $(['<textarea GridID="', GridID, '"  ', Class, Width, ' rowIndex="', options.model["rowIndex"], '"  name="', options.field, '"  data-field="', options.field, '"></textarea>'].join(''));
    for (var key in CustomAttributes) {
        textarea.attr(key.trim(), CustomAttributes[key.trim()]);
    }
    textarea.appendTo(container);
};
MVVMGlobal.GridButtonEditor = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    var CustomAttributes = options.model.fields[options.field].CustomAttributes;
    var Width = CustomAttributes["Width"] != undefined ? ["style=\"width:", CustomAttributes["Width"], "\"", " wrap=\"hard\" "].join('') : "";
    var Class = CustomAttributes["CssClass"] != undefined ? ["class=\"", CustomAttributes["CssClass"], "\" "].join('') : "class=\"GridButton\" ";
    var button = $(['<input base_click="true" type="button" GridID="', GridID, '"  ', Class, Width, ' rowIndex="', options.model["rowIndex"], '" name="', options.field, '"  data-field="', options.field, '"/>'].join(''));
    for (var key in CustomAttributes) {
        button.attr(key.trim(), CustomAttributes[key.trim()]);
    }
    button.appendTo(container);
};
MVVMGlobal.GridLinkButtonEditor = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    var CustomAttributes = options.model.fields[options.field].CustomAttributes;
    var Width = CustomAttributes["Width"] != undefined ? ["style=\"width:", CustomAttributes["Width"], "\"", " wrap=\"hard\" "].join('') : "";
    var Class = CustomAttributes["CssClass"] != undefined ? ["class=\"", CustomAttributes["CssClass"], "\" "].join('') : "class=\"GridLink\" ";
    var button = $(['<a linkbutton="true" onclick="clickListner(this);" style="color: blue; text-decoration: underline; cursor: pointer;" base_click="true"  GridID="', GridID, '"  ', Class, Width, ' rowIndex="', options.model["rowIndex"], '"  name="', options.field, '"  data-field="', options.field, '"></a>'].join(''));
    for (var key in CustomAttributes) {
        button.attr(key.trim(), CustomAttributes[key.trim()]);
    }
    var lstrText = options.item[options.field];
    if (lstrText != undefined) {
        button.text(lstrText);
    } else if (lstrText != undefined && $.trim(lstrText) != "") {
        lstrText = button.attr("text");
        button.text(lstrText);
    }
    else {
        button.text("Field not found to bind.");
    }

    button.appendTo(container);
};
MVVMGlobal.GridTextBoxEditor = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    var CustomAttributes = options.model.fields[options.field].CustomAttributes;
    var Width = CustomAttributes["Width"] != undefined ? ["style=\"width:", CustomAttributes["Width"], "\""].join('') : "";
    var Class = CustomAttributes["CssClass"] != undefined ? ["class=\"", CustomAttributes["CssClass"], "\" "].join('') : "class=\"GridTextBox\" ";
    var textbox = $(['<input type="text" GridID="', GridID, '" class="GridTextBox" ', Class, Width, ' rowIndex="', options.model["rowIndex"], '" name="', options.field, '"  data-field="', options.field, '"/>'].join(''));
    for (var key in CustomAttributes) {
        textbox.attr(key.trim(), CustomAttributes[key.trim()]);
    }
    textbox.appendTo(container);
};
MVVMGlobal.GridRowCheckEditMode = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    $(['<input type="checkbox" rowIndex="', options.rowIndex, '" class="s-grid-check-row" GridID="', GridID, '" data-field="', options.field, '"/>'].join('')).appendTo(container);
};
MVVMGlobal.GridRowRadioEditMode = function (container, options) {
    var GridID = options.sender.id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
    $(['<input type="Radio" name="Rdo', GridID, '" rowIndex="', options.rowIndex, '" class="s-grid-check-row" GridID="', GridID, '" data-field="', options.field, '"/>'].join('')).appendTo(container);
};


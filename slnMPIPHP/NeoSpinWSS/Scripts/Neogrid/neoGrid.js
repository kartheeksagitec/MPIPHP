///\#[=,\-,:](.*?#.*?)(\).*?\#)|\#[=,\-,:](.+?)\#/g,
_.templateSettings = {
    interpolate: /\#[=,-,:](.+?)\#/g,
    evalulate: /\#[=,-, :](.+?)\#/g,
    escape: /\#[=,-,:](.+?)\#/g
};
var NeoGrid = (function () {
    function NeoGrid(adomElement, aobjOptions) {
        this.iintStartTime = new Date().getTime();
        var startTime = new Date().getTime();
        this.iblnRendering = false;
        this.element = $(adomElement);
        if (this.element.attr("id") == undefined || this.element.attr("id") == "")
            this.element.attr("id", NeoGrid.generateGUID());
        this.id = this.element[0].id.replace("GridTable_", "");
        this.iblnTable = false;
        this.istrTableInnerHTML = "";
        this.gridContainer = null;
        this.iblnHasInputsInRawTemplate = false;
        this.element.addClass("s-gridparent s-grid-container").attr("data-role", "neogrid");
        if (this.element[0].tagName === "TABLE") {
            this.iblnTable = true;
            this.gridContainer = $("<div class='s-grid-helper s-grid-container' id='GridHelper_" + this.id + "'></div>");
            this.gridContainer.insertBefore(this.element);
            this.istrTableInnerHTML = this.element.html();
            this.element.removeClass("s-grid-container").addClass("s-grid fluid-table").attr("role", "table").empty();
            this.element.appendTo(this.gridContainer);
        }
        this.options = aobjOptions;
        this.istrRowTemplate = "";
        this.istrHdrTemplate = "";
        this.istrFooterRowTemplate = "";
        this.sortFields = [];
        this.groupedColumns = [];
        this.groupedData = undefined;

        this.filterColumns = [];
        this.filterData = [];

        this.prevPage = 0;
        this.iblnCallToRender = true;
        this.pager = undefined;
        this.view = [];
        this.setOptions();
        this.init();
        this.iintEndTime = new Date().getTime();
        this.istrGridRenderingTime = [(this.iintEndTime - startTime), " ms"].join('');
        //console.log(this.istrGridRenderingTime);
    }
    NeoGrid.prototype.setOptions = function () {
        this.istrEmptyDataText = (this.options.istrEmptyDataText == undefined || this.options.istrEmptyDataText == "") ? "No records to display." : this.options.istrEmptyDataText;
        this.iblnShowHeaderWhenEmpty = (this.options.ShowHeaderWhenEmpty === false) ? false : true;
        this.selection = this.options.selection || ""; //'multiple';
        this.iblnGrouping = (this.options.groupable === true) ? true : false; //true;
        this.iblnSorting = (this.options.sortable === true) ? true : false;
        this.iblnPaging = (this.options.pageable === false) ? false : true;
        //this.iblnEditable = (this.options.editable === true) ? true : false; //true;
        this.iblnEditable = true;
        this.iblnFilterable = (this.options.filterable === true) ? true : false;
        this.iblnNavigatable = (this.options.navigatable === true) ? true : false; //true;
        this.pageSize = (this.options.dataSource.pageSize != undefined) ? this.options.dataSource.pageSize : 10; //set from options
        this.currentPage = (this.options.dataSource.currentPage != undefined) ? this.options.dataSource.currentPage : 1;
        this.totalRecords = this.options.dataSource.data.length;
        this.iobjFields = this.options.dataSource.schema.model.fields;
        this.dataSource = this.options.dataSource;
        this.sortFields = _.isArray(this.dataSource.sort) && this.dataSource.sort.length > 0 ? this.dataSource.sort : [];
        this.columns = (this.options.columns != undefined) ? this.options.columns : [];
        this.iblnRowTemplate = (this.options.iblnRowTemplate === true) ? true : false;
        this.iblnHdrTemplate = (this.options.iblnHdrTemplate === true) ? true : false;
        if (this.options.iblnRowSelect !== true && (this.selection === 'multiple' || this.selection === 'single')) {
            var lobjRowSelect = $.grep(this.columns, function (col) {
                return col.field === "rowSelect";
            });
            if (lobjRowSelect.length === 0) {
                var rowCol = {
                    field: "rowSelect",
                    title: "Select",
                    width: "44px"
                };
                this.columns.unshift(rowCol);
                if (this.iobjFields["rowSelect"] == undefined)
                    this.iobjFields["rowSelect"] = {
                        type: 'boolean'
                    };
            }
        }
        //Set Editable columns & columns fields, so that no need to call $.grep or $.extend multiple times
        var ltempFields = _.cloneDeep(this.iobjFields);
        this.columnFields = _.reduce(this.columns, function (hash, value) {
            hash[value.field] = _.assign({}, ltempFields[value.field], value);
            return hash;
        }, {});
        ltempFields = null;
        this.irrEditableColumns = [];
        if (this.iblnEditable === true) {
            this.irrEditableColumns = _.filter(this.columnFields, function (col) {
                return (col.editable === 'true' || col.editable === true || typeof col.editor === 'function') && col.field !== 'rowSelect';
            });
        }
    };
    NeoGrid.prototype.init = function () {
        // this.registerEvents();
        this.RenderData = [];
        this.setRenderData();
        if (this.sortFields.length > 0) {
            this.sort();
        }
        if (this.groupedColumns.length > 0) {
            this.onGroup();
        }
        else {
            this.renderGrid();
        }
    };
    NeoGrid.prototype.getColumnTemplate = function (astrField) {
        var lstrField = astrField;
        var lstrColTemplate = this.columnFields[lstrField]['template'];
        var lblnHidden = (this.columnFields[lstrField].hidden === "true" || this.columnFields[lstrField].hidden === true);
        if ((lstrField === "rowSelect" && !(this.selection === 'multiple' || this.selection === 'single')) || (lstrField === "rowIndex")) {
            lblnHidden = true;
        }
        var lstrFieldType = this.columnFields[lstrField].type;
        var lstrClass = "";
        var lstrCellStyle = " ";
        var lstrToolTip = "";
        if (lstrFieldType == "number") {
            lstrClass = "s-number";
        }
        if (lblnHidden === true) {
            lstrCellStyle = " style='display:none;' ";
            if (lstrField === 'rowIndex') {
                lstrCellStyle = [lstrCellStyle, " coltype='rowIndex' "].join('');
            }
        }
        else if (this.columnFields[lstrField] != undefined && this.columnFields[lstrField].attributes != undefined) {
            if (this.columnFields[lstrField].attributes.style != undefined && this.columnFields[lstrField].attributes.style != "") {
                lstrCellStyle = [" style='", this.columnFields[lstrField].style, "' "].join("");
            }
            if (this.columnFields[lstrField].attributes.class != undefined && this.columnFields[lstrField].attributes.class != "") {
                lstrClass = [lstrClass, " ", this.columnFields[lstrField].attributes.class].join("");
            }
            if (this.columnFields[lstrField].attributes.tooltip != undefined && this.columnFields[lstrField].attributes.tooltip != "") {
                lstrToolTip = [" tooltip='", this.columnFields[lstrField].attributes.tooltip, "' "].join("");
            }
        }
        if (lstrClass != "") {
            lstrClass = [" class ='", lstrClass, "' "].join("");
        }

        if (lstrColTemplate == undefined || lstrColTemplate == "") {
            var lstrFormat = this.columnFields[lstrField].format;
            if (lstrFormat != undefined && lstrFormat != "") {
                lstrFormat = lstrFormat.replace(neo.templateHashRegExp, "\\#");
                lstrColTemplate = ["#= neoFormat.GetFormatedValue(\"", lstrFormat, "\",", lstrField, ") #"].join('');
            }
            else {
                lstrColTemplate = ["#= ", lstrField, "#"].join('');
            }
        }
        var lstrTD = " ";
        if (lstrField === 'rowIndex' || lblnHidden !== true) {
            lstrTD = ["<td data-container-for='", lstrField, "' ", lstrClass, lstrCellStyle, lstrToolTip, " role='gridcell' >", lstrColTemplate, "</td>"].join('');
        }
        return lstrTD;
    };
    NeoGrid.prototype.createTemplate = function () {
        var lstrRowTemplate = "";
        var lstrFooterRowTemplate = "<tr class='s-grid-ftrrow' role='footer-row'>";
        var lstrHdrTemplate = "";
        if (this.iblnRowTemplate && this.options.rowTemplate != undefined && $.trim(this.options.rowTemplate) != "") {
            lstrRowTemplate = this.options.rowTemplate; //row-template-element
            var ldomTemp = $(lstrRowTemplate);
            ldomTemp.find("input.check_row").each(function () {
                $(this).removeClass("check_row").addClass("s-grid-check-row").attr("data-field", "rowSelect").closest('td').attr("data-container-for", "rowSelect");
            });
            ldomTemp.find("[data-bind], [databind]").not("[data-field]").each(function () {
                var $this = $(this);
                var lstrDataBind = $this.attr('data-bind') || $this.attr("databind");
                if (lstrDataBind.indexOf("text:") >= 0) {
                    lstrDataBind = lstrDataBind.replace("text:", "");
                    if ($.trim($this.html()) == "")
                        $this.html(["#=", lstrDataBind, "#"].join(''));
                    if ($this[0].tagName === "LABEL") {
                        var span = $("<span></span>").html($this.html());
                        span.insertAfter($this);
                        $this.remove();
                        $this = span;
                    }
                    $this.attr("data-field", lstrDataBind);
                    $this.removeAttr("title");
                }
                else if ($this[0].tagName === "A") {
                    if ($this.find("div.GridLinks").length > 0) {
                        $this.find("div.GridLinks").attr({ "databind": lstrDataBind, "data-field": lstrDataBind });
                        $this.removeAttr("databind, data-field");
                    }
                    else {
                        $this.attr({ "databind": lstrDataBind, "data-field": lstrDataBind });
                    }
                }
                else if ($this.hasClass("GridLinks")) {
                    if ($this.closest("a").length > 0) {
                        $this.closest("a").removeAttr("databind, data-field");
                    }
                    $this.attr({ "databind": lstrDataBind, "data-field": lstrDataBind });
                }
                else if (lstrDataBind.indexOf(":") >= 0) {
                    lstrDataBind = lstrDataBind.substr(lstrDataBind.indexOf(":") + 1);
                    if ($this[0].tagName === "LABEL" && (lstrDataBind.indexOf("value:") >= 0 || lstrDataBind.indexOf("text:") >= 0)) {
                        lstrDataBind = $this.closest("td").attr("data-container-for");
                        var span = $("<span></span>").html($this.html());
                        span.insertAfter($this);
                        $this.remove();
                        $this = span;
                    }
                    $this.attr("data-field", lstrDataBind);
                    if ($this.attr("type") === "text") {
                        $this.attr("IsNeoGrid", 'true');
                    }
                }
                $this = null;
            });
            ldomTemp.attr("role", "row");

            var ldomTds = ldomTemp.find("td[data-container-for]:not([data-container-for='rowSelect'])");
            if (ldomTds.length > 0) {
                for (var c = 0, tdLen = ldomTds.length; c < tdLen; c++) {
                    var lstrField = $(ldomTds[c]).attr("data-container-for");
                    if (lstrField != undefined && this.columnFields[lstrField] == undefined) {
                        $(ldomTds[c]).remove();
                    }
                }
            }
            ldomTds = null;
            this.iblnHasInputsInRawTemplate = ldomTemp.find("td:not([data-container-for='rowSelect'])").find("input:not(.s-grid-check-row), select, textarea, span[controltype]").length > 0;
            if ((this.selection === 'multiple' || this.selection === 'single') && ldomTemp.find("td[data-container-for='rowSelect']").length == 0) {
                ldomTemp.prepend("<td data-container-for='rowSelect' role='gridcell' ></td>")
            }

            lstrRowTemplate = ldomTemp[0].outerHTML;
            this.options.rowTemplate = lstrRowTemplate;
            if ($(["#", this.options.ActiveDivId, " #", this.id, "-row-template"].join('')).length > 0) {
                $(["#", this.options.ActiveDivId, " #", this.id, "-row-template"].join('')).html(lstrRowTemplate);
            }
            ldomTemp = null;
        } else {
            lstrRowTemplate = "<tr  rowIndex='#= rowIndex #' role='row' data-uid='#:uid#'>";
        }

        var FiledsWithAggregate = _.filter(this.columnFields, function (item) {
            if (item.aggregate != undefined) {
                return true;
            }
        });

        var lblnHasFooter = FiledsWithAggregate.length > 0;

        lstrHdrTemplate = "<thead><tr class='s-hdrrow' role='row'>";
        for (var i = 0, lintLength = this.columns.length; i < lintLength; i++) {
            var lstrField = this.columns[i]['field'];
            if (this.iblnRowTemplate !== true && lstrRowTemplate !== "") {
                var lstrTD = this.getColumnTemplate(lstrField);
            }
            var lstrColHdrTemplate = this.columns[i]['headerTemplate'];
            var lblnHidden = (this.columns[i].hidden === "true" || this.columns[i].hidden === true);
            if (lstrField === "rowSelect") {
                if (this.selection === 'multiple') {
                    lstrColHdrTemplate = ["<input IsNeoGrid='true' class='s-grid-check-all' type='checkbox' title='Select All Record' id='checkAll_", this.id, "'  GridID='", this.id, "'"].join('');
                } else if (this.selection === 'single') {
                    lstrColHdrTemplate = "Select";
                } else {
                    lblnHidden = true;
                }
            }
            else if (lstrField === "rowIndex") {
                lblnHidden = true;
            }
            if (lstrColHdrTemplate == undefined || lstrColHdrTemplate == "") {
                var lstrSpanClass = "s-arrow";
                if (this.sortFields.length > 0) {
                    var lobjSort = _.filter(this.sortFields, { field: this.columns[i].field });
                    if (lobjSort.length > 0)
                        lstrSpanClass += lobjSort[0].dir === "asc" ? " s-asc" : " s-desc";
                }
                lstrColHdrTemplate = ["<span>", this.columns[i]['title'], "<span class='", lstrSpanClass, "'></span></span>"].join('');
            }
            var lstrHdrRowCell = " ";
            var lstrCellStyle = " ";
            if (lblnHidden === true) {
                lstrCellStyle = " style='display:none;' ";
                if (lstrField === 'rowIndex')
                    lstrCellStyle = [lstrCellStyle, " coltype='rowIndex' "].join('');
            }
            var lstrHdrCellClass = " ";
            if (lstrField !== 'rowSelect' && this.iblnSorting === true) {
                lstrHdrCellClass = " class='s-sortable' ";
            } else if (lstrField === 'rowSelect') {
                lstrHdrCellClass = " class='s-row-select-th' ";
            }


            var filterHtml = ""
            if (this.iblnFilterable && lstrField != "rowSelect") {
                filterHtml = " <span class='s-FilterTrigger'></span>";
            }

            if (lstrField === 'rowIndex' || lblnHidden !== true) {
                lstrHdrRowCell = ["<th", lstrHdrCellClass, " data-field='", lstrField, "' data-title='", this.columns[i]['title'], "' ", lstrCellStyle, " >", lstrColHdrTemplate, filterHtml, "</th>"].join("");

                if (lblnHasFooter && lstrField !== 'rowIndex') {
                    lstrFooterRowTemplate = [lstrFooterRowTemplate, "<td ", lstrCellStyle, " container-for='", lstrField, "'></td>"].join('');
                }
            }
            if (this.iblnRowTemplate !== true && lstrRowTemplate !== "") {
                lstrRowTemplate = [lstrRowTemplate, lstrTD].join("");
            }
            //  lstrAltRowTemplate = [lstrAltRowTemplate, lstrTD].join("");
            lstrHdrTemplate = [lstrHdrTemplate, lstrHdrRowCell].join("");
        }
        if (this.iblnRowTemplate !== true && lstrRowTemplate !== "") {
            lstrRowTemplate = [lstrRowTemplate, "</tr>"].join("");
        }
        lstrHdrTemplate = [lstrHdrTemplate, "</tr></thead>"].join("");
        this.istrRowTemplate = lstrRowTemplate;
        this.istrHdrTemplate = lstrHdrTemplate;
        this.istrFooterRowTemplate = [lstrFooterRowTemplate, "</tr>"].join("");;
        //lstrRowTemplate = neoFormat.replaceAll(lstrRowTemplate, "\\\\#", "X~X");
        this.fnRowTemplate = _.template(lstrRowTemplate);
        //lstrRowTemplate = neoFormat.replaceAll(lstrRowTemplate, "X~X", "\\\\#");
    };
    NeoGrid.prototype.renderPager = function () {
        var lpager = $(["<div class='s-pager' id='Pager_", this.id, "'></div>"].join(""));
        if (this.iblnTable) {
            this.gridContainer.prepend(lpager);
            this.pager = this.gridContainer.find(".s-pager");
        }
        else {
            this.element.append(lpager);
            this.pager = this.element.find(".s-pager");
        }
        this.pager.pagination({
            items: this.totalRecords,
            itemsOnPage: this.pageSize,
            cssStyle: 'light-theme',
            currentPage: this.currentPage,
            onPageClick: this.onPageClick
        });
        this.pager.data('pagination').GridElement = this.element;
        //prefix active div id
    };
    NeoGrid.prototype.onPageClick = function (pageno, event) {
        var lobjSender = this.GridElement.data('neoGrid');
        if (lobjSender.iblnCallToRender !== false && pageno > 0 && this.pages > 0 && pageno <= this.pages) {
            lobjSender.prevPage = lobjSender.currentPage;
            lobjSender.currentPage = pageno;
            lobjSender.renderGrid();
            lobjSender.onPageChange({
                pageNo: pageno,
                event: event,
                pager: this,
                sender: lobjSender
            });
        }
        lobjSender.iblnCallToRender = true;
    };
    NeoGrid.prototype.remove = function (aobjData, ablnSelected, ablnRowIndexes) {
        if (ablnSelected === true) {
            _.remove(this.dataSource.data, { rowSelect: true });
        } else {
            var larrRowIndex = [];
            if (_.isArray(aobjData)) {
                if (ablnRowIndexes !== true) {
                    larrRowIndex = _.map(aobjData, 'rowIndex');
                } else {
                    larrRowIndex = aobjData;
                }
            }
            else if (_.isPlainObject(aobjData)) {
                if (ablnRowIndexes !== true)
                    larrRowIndex = _.values(_.pick(aobjData, 'rowIndex'));
            } else {
                larrRowIndex.splice(0);
                larrRowIndex.push(neoFormat.parseInt(aobjData))
            }
            _.remove(this.dataSource.data, function (dataItem) {
                return _.indexOf(larrRowIndex, dataItem.rowIndex) !== -1;
            });
            larrRowIndex.splice(0);
            larrRowIndex = null;
        }
        this.setRenderData();
        this.refresh();
    }
    NeoGrid.prototype.refresh = function (ablnSetRenderData) {
        if (ablnSetRenderData === true && this.filterColumns.length <= 0) {
            this.setRenderData();
        }
        this.restoreState(true);
    };
    NeoGrid.prototype.setRenderData = function () {
        var lintPageToSet = this.currentPage;
        if (this.filterColumns.length > 0) {
            this.RenderData = this.filterData;
        } else {
            this.RenderData = this.dataSource.data;
        }

        if (_.map(this.columnFields, "aggregate").length > 0) {

            var FiledsWithAggregate = _.pickBy(this.columnFields, function (item) {
                if (item.aggregate != undefined) {
                    return true;
                }
            });

            this.RenderData = NeoGrid.getAggregatedDataArray(this.RenderData, FiledsWithAggregate);
        }

        if (this.iblnPaging === true && this.pager != undefined) {
            this.pager.pagination('updateItems', this.RenderData.length);
            var lintPages = this.pager.pagination("getPagesCount");
            this.currentPage = this.pager.pagination('getCurrentPage');
            if (this.currentPage > lintPages || lintPageToSet > lintPages) {
                this.currentPage = lintPages;
                this.iblnCallToRender = false;
                this.pager.pagination('selectPage', lintPages);
            }
            else if (lintPageToSet > 0 && this.currentPage != lintPageToSet && lintPageToSet <= lintPages) {
                this.iblnCallToRender = false;
                this.currentPage = lintPageToSet;
                this.pager.pagination('selectPage', lintPageToSet);
            }
            this.prevPage = 0;
        }

    };
    NeoGrid.prototype.restoreState = function (ablnRender) {
        var lintPageToSet = this.currentPage;
        var lblnIsSortExpression = true;
        if (ablnRender !== true && this.sortFields.length > 0 && _.isArray(this.dataSource.sort) && this.dataSource.sort.length === this.sortFields.length && _.isEqual(this.dataSource.sort, this.sortFields)) {
            lblnIsSortExpression = false;
        }
        if (this.filterColumns.length > 0) {
            this.onFilter();
        }
        else if (this.sortFields.length > 0 && lblnIsSortExpression === true) {
            this.sort();
        }
        else if (this.groupedColumns.length > 0) {
            this.onGroup();
        }
        else {
            var lblnRender = false;
            if (this.iblnPaging === true && this.pager != undefined) {
                var lintPages = this.pager.pagination("getPagesCount");
                this.currentPage = this.pager.pagination('getCurrentPage');
                if (this.currentPage > lintPages || lintPageToSet > lintPages) {
                    var lblnRender = true;
                    lintPageToSet = lintPages;
                }
                else if (lintPageToSet > 1 && this.currentPage != lintPageToSet && lintPageToSet <= lintPages) {
                    var lblnRender = true;
                }
            }
            if (lblnRender) {
                this.currentPage = lintPageToSet;
                this.iblnCallToRender = false;
                this.pager.pagination('selectPage', lintPageToSet);
                this.prevPage = 0;
                this.renderGrid();
            }
            else if (ablnRender === true) {
                this.prevPage = 0;
                this.renderGrid();
            }
        }
    };

    NeoGrid.prototype.gotoLastPage = function () {
        if (this.iblnPaging === true && this.pager != undefined) {
            var lintPageToSet = this.pager.pagination("getPagesCount");
            this.currentPage = this.pager.pagination('getCurrentPage');
            if (lintPageToSet > 0 && this.currentPage != lintPageToSet) {
                this.currentPage = lintPageToSet;
                this.prevPage = 0;
                this.pager.pagination('selectPage', lintPageToSet);
            }
        }
    }

    NeoGrid.prototype.destroy = function () {
        if (this.pager != undefined && this.pager.length > 0 && this.pager.data("pagination") != undefined) {
            this.pager.pagination('destroy');
            jQuery.removeData(this.pager, "pagination");
        }
        jQuery.removeData(this.element, "neoGrid");
        this.element.find('*').unbind().end().empty();
        if (this.iblnTable && this.element.closest(".s-grid-helper").length > 0) {
            this.element.insertAfter(this.gridContainer);
            this.element.html(this.istrTableInnerHTML);
            this.gridContainer.find('*').unbind().end().remove();
        }
    };
    NeoGrid.prototype.renderGrid = function () {
        this.iintStartTime = new Date().getTime();
        this.iblnRendering = true;
        this.totalRecords = this.RenderData.length;
        if (this.istrRowTemplate == undefined || this.istrRowTemplate == '') {
            this.createTemplate();
        }
        var ldomTBody = this.element.find("tbody");
        var TBodyFragment = document.createDocumentFragment();
        var lblnAddHeader = true;
        var ldomTable;
        if (ldomTBody.length > 0) {
            if (this.iblnTable) {
                ldomTable = this.element;
            } else {
                ldomTable = this.element.find("table.s-grid");
            }
            lblnAddHeader = false;
            ldomTBody.find('*').unbind().end().empty();
            //ldomTBody = $("<tbody role='tbody' class='s-tbody'></tbody>");
            //ldomTBody.empty();
        } else {
            this.element.find('*').unbind().end().empty();
            //this.element.empty();
            if (this.iblnPaging && this.totalRecords > 0) {
                this.renderPager();
            }
            if (this.iblnGrouping) {
                var lstrGroupableTemplate = "<div class='s-groups'> <ul class='s-groups-list'> <li class='placeholder s-groupds-drop-header'>Drag a column header and drop it here to group by that column</li> </ul></div>";
                if (this.iblnTable) {
                    $(lstrGroupableTemplate).insertBefore(this.element);
                }
                else {
                    this.element.append($(lstrGroupableTemplate));
                }
            }
            if (this.iblnTable) {
                ldomTable = this.element;
            } else {
                ldomTable = $(["<table  role='table' class='s-grid fluid-table' id='Table_", this.element[0].id, "'></table>"].join(''));
            }
            ldomTBody = $("<tbody role='tbody' class='s-tbody'></tbody>");
            ldomTable.append(this.istrHdrTemplate);

            if (this.iblnGrouping) {
                this.registerGroupableEvents(ldomTable);
            }
        }
        if (this.totalRecords <= 0) {
            if (this.pager != undefined && this.pager.length > 0) {
                this.pager.hide();
            }
            if (this.iblnShowHeaderWhenEmpty === false) {
                ldomTable.find("thead").hide();
                if (this.iblnGrouping) {
                    if (this.iblnTable) {
                        this.gridContainer.find(".s-groups").hide();
                    }
                    else {
                        this.element.find(".s-groups").hide();
                    }
                }
            }
            var lstrEmptyRow = ['<tr class="s-grid-empty-row"><td colspan="', this.columns.length, '" style="text-align:center">', this.istrEmptyDataText, '</td></tr>'].join('');
            //ldomTBody.append(lstrEmptyRow);
            TBodyFragment.appendChild($(lstrEmptyRow)[0]);
        } else {
            if (this.pager != undefined && this.pager.length > 0) {
                this.pager.show();
                var lintPages = this.pager.pagination("getPagesCount");
                if (lintPages == 1) {
                    this.pager.hide();
                }
            }
            if (this.iblnShowHeaderWhenEmpty === false) {
                ldomTable.find("thead").show();
                if (this.iblnGrouping)
                    this.element.find(".s-groups").show();
            }
            ldomTBody.find("*").unbind().end().empty();
            this.view = [];
            var currentIndex = (this.currentPage - 1) * this.pageSize;
            var aintPageSize = this.currentPage * this.pageSize;
            var lintTolatRecs = this.totalRecords;
            if (this.prevPage != this.currentPage && this.iblnPaging) {
                var lstrDisplayingInfo = ["<span class='s-paging-msg'>Displaying ", (currentIndex + 1), " to ", (aintPageSize > lintTolatRecs ? lintTolatRecs : aintPageSize), " of ", lintTolatRecs, "</span>"].join("");
                this.pager.find("ul").append(lstrDisplayingInfo);
            }
            aintPageSize = (aintPageSize > lintTolatRecs ? lintTolatRecs : aintPageSize);
            this.prevPage = this.currentPage;
            ldomTable.find("thead tr").find(".s-empty-th").remove();

            if (this.groupedColumns.length > 0 && this.iblnGrouping) {
                this.renderGridByGroupedData(ldomTBody, currentIndex, aintPageSize, TBodyFragment);
                var ths = "";
                for (var i = 0, colLen = this.groupedColumns.length; i < colLen; i++) {
                    ths = [ths, "<th class='s-empty-th'></th>"].join("");
                }
                ldomTable.find("thead tr").prepend(ths);
            }
            else {
                for (var i = currentIndex; i < aintPageSize; i++) {
                    if (this.RenderData[i]['uid'] === undefined) {
                        this.RenderData[i].uid = this.RenderData[i].rowIndex;
                    }
                    var ldomRow = $(this.fnRowTemplate(this.RenderData[i]));
                    if (i % 2 == 0) {
                        ldomRow.addClass("s-row");
                    } else {
                        ldomRow.addClass("s-altrow");
                    }
                    this.view.push(this.RenderData[i]);
                    if (this.iblnTable) {
                        this.renderRawTemplateRow(ldomRow, this.RenderData[i]);
                    } else if (this.iblnEditable) {
                        this.renderRow(ldomRow, this.RenderData[i]);
                    }
                    if ((this.selection === 'multiple' || this.selection === 'single')) {
                        this.renderSelectCell(ldomRow, this.RenderData[i]);
                    }
                    //ldomTBody.append(ldomRow);
                    TBodyFragment.appendChild(ldomRow[0]);
                    this.onRowRender({
                        row: ldomRow,
                        item: this.RenderData[i],
                        sender: this
                    });
                }

                // Add aggregate row
                if (_.map(this.columnFields, "aggregate").length > 0) {
                    var FooterRow = NeoGrid.getAggregatedRow(this, this.RenderData, "")
                    TBodyFragment.appendChild(FooterRow[0]);
                }
            }
        }
        ldomTBody[0].appendChild(TBodyFragment);
        if (lblnAddHeader) {
            ldomTable.append(ldomTBody);
            if (!this.iblnTable)
                this.element.append(ldomTable);
        }
        jQuery.removeData(this.element, "neoGrid");
        this.element.data("neoGrid", this);
        this.onDataBind();
        this.iintEndTime = new Date().getTime();
        this.istrGridRenderingTime = (this.iintEndTime - this.iintStartTime) + " ms";
        //console.log(this.istrGridRenderingTime);
        this.iblnRendering = false;
        this.setGroupHeaderWidth();
        //jQuery.removeData(this.element, "neoGrid");
        this.element.data("neoGrid", this);
    };

    NeoGrid.prototype.setGroupHeaderWidth = function () {
        if (this.iblnTable) {
            var width = this.element.width();
            if (width > 0)
                this.gridContainer.find(".s-groups").width(width);
        } else {
            var width = this.element.find("thead").width();
            if (width > 0)
                this.element.find(".s-groups").width(width);
        }
    };

    NeoGrid.prototype.renderGridByGroupedData = function (adomTBody, aintCurrentIndex, aintPageSize, TBodyFragment) {
        var AllCounts = {
            count: 0,
            renderCount: 0,
        };
        var lblnAddHeaderRow = false;
        var lstrMaxTds = "";
        // debugger;

        var lblnIsAggregate = _.filter(this.columnFields, "aggregate").length > 0;

        var RecFun = function (data, tds, adomTBody, aobjGrid, aintCurrentIndex, aintPageSize, TBodyFragment, ablnIsAggregate) {
            if ($.isArray(data) && data[0]["rowIndex"] != undefined) {
                for (var i = 0, cnt = data.length; i < cnt; i++) {
                    if (AllCounts.count >= aintCurrentIndex && AllCounts.count < aintPageSize) {
                        if (data[i]['uid'] === undefined) {
                            data[i].uid = data[i].rowIndex;
                        }
                        var ldomRow = $(aobjGrid.fnRowTemplate(data[i]));
                        if (i % 2 == 0) {
                            ldomRow.addClass("s-row");
                        } else {
                            ldomRow.addClass("s-altrow");
                        }
                        aobjGrid.view.push(data[i]);
                        if (aobjGrid.iblnTable) {
                            aobjGrid.renderRawTemplateRow(ldomRow, data[i]);
                        }
                        else if (aobjGrid.iblnEditable) {
                            aobjGrid.renderRow(ldomRow, data[i]);
                        }
                        if ((aobjGrid.selection === 'multiple' || aobjGrid.selection === 'single')) {
                            aobjGrid.renderSelectCell(ldomRow, data[i]);
                        }
                        ldomRow.prepend(tds);
                        //adomTBody.append(ldomRow);
                        TBodyFragment.appendChild(ldomRow[0]);
                        lblnAddHeaderRow = true;
                        aobjGrid.onRowRender({
                            row: ldomRow,
                            item: data[i],
                            sender: aobjGrid
                        });
                        AllCounts.renderCount++;
                    }
                    AllCounts.count++;
                    if (AllCounts.renderCount == aobjGrid.pageSize) {
                        return -1;
                    }
                }
                return AllCounts.count;
            } else {
                tds = [tds, "<td class='s-empty-td'></td>"].join("");
                var lstrTds = tds.replace("<td class='s-empty-td'></td>", "");
                if (lstrMaxTds.length < tds.length) {
                    lstrMaxTds = tds;
                }
                for (var key in data) {
                    if (key.indexOf("~~") == 0) {
                        continue;
                    }
                    var lhtmlGroupHeaderRow = ["<tr key=\"", key, "\"  class='s-group-row'>", lstrTds, "<td class='s-group-col' colspan='", (aobjGrid.columns.length + (aobjGrid.groupedColumns.length - $(lstrTds).length - 1)), "'><span class='s-group-row-value s-group-row-value-expand'></span>", key, "</td></tr>"].join("");
                    var lstrGroupHeaderRow = $(lhtmlGroupHeaderRow);
                    var AddSum = false;
                    if (AllCounts.count >= aintCurrentIndex && AllCounts.count < aintPageSize) {
                        TBodyFragment.appendChild(lstrGroupHeaderRow[0]);
                        AddSum = true;
                    }
                    var result = RecFun(data[key], tds, adomTBody, aobjGrid, aintCurrentIndex, aintPageSize, TBodyFragment, ablnIsAggregate);
                    AllCounts.count = result;
                    var groupHeaderCount = TBodyFragment.querySelectorAll(['tr.s-group-row[key="', key, '"]'].join('')).length;
                    if (groupHeaderCount == 0 && lblnAddHeaderRow) {
                        var groupRowCount = TBodyFragment.querySelectorAll("tr[role=row]").length;
                        if (groupRowCount > 0) {
                            TBodyFragment.insertBefore(lstrGroupHeaderRow[0], TBodyFragment.firstChild);
                            AddSum = true;
                        }
                    }

                    if (AddSum && ablnIsAggregate) {
                        var FooterRow = NeoGrid.getAggregatedRow(aobjGrid, data[key], lstrMaxTds)
                        TBodyFragment.appendChild(FooterRow[0]);
                    }

                    if (result == -1) {
                        return -1;
                    }
                }
                return AllCounts.count;
            }
        };
        RecFun(this.groupedData, "", adomTBody, this, aintCurrentIndex, aintPageSize, TBodyFragment, lblnIsAggregate);
        AllCounts = null;
        lblnAddHeaderRow = null;
    }
    NeoGrid.prototype.registerGroupableEvents = function (adomTable) {
        if (this.totalRecords > 0) {
            adomTable.find("thead").find("th:not(th[data-field=rowSelect])").draggable({
                appendTo: 'body',
                helper: 'clone'
            });

            var domElement = (this.iblnTable === true) ? this.gridContainer : this.element;
            domElement.find(".s-groups ul.s-groups-list").droppable({
                activeClass: 'ui-state-default',
                hoverClass: 'ui-state-hover',
                accept: ':not(.ui-sortable-helper)',
                drop: function (event, ui) {
                    var $this = $(this);
                    $this.find('.placeholder').remove();
                    var field = ui.draggable.attr('data-field');
                    var ldomGrid = $this.closest(".s-gridparent");
                    if (ldomGrid.length == 0) {
                        ldomGrid = $this.closest(".s-grid-helper").find("table.s-grid");
                    }
                    var lobjGrid = ldomGrid.data("neoGrid");
                    if (lobjGrid.groupedColumns.indexOf(field) >= 0) {
                        return;
                    }
                    var groupingColumn = $('<li></li>').attr('data-column', field);
                    $('<span class="s-grid-ui-icon s-grid-ui-icon-close"></span>').attr('data-column', field).click(NeoGrid.onGroupClose).appendTo(groupingColumn);
                    groupingColumn.append(ui.draggable.text().trim());
                    groupingColumn.appendTo($this);
                    lobjGrid.groupedColumns.push(field);
                    lobjGrid.onGroup();
                    ldomGrid = null;
                    $this = null;
                }
            }).sortable({
                items: 'li:not(.placeholder)',
                sort: function () {
                    $(this).removeClass('ui-state-default');
                },
                stop: function () {
                    var $this = $(this);
                    var domGrid = $this.closest(".s-gridparent");
                    if (domGrid.length == 0) {
                        domGrid = $this.closest(".s-grid-helper").find("table.s-grid");
                    }
                    var lobjGrid = domGrid.data("neoGrid");
                    lobjGrid.groupedColumns.splice(0);
                    $('.s-groups ul li:not(.placeholder)').each(function (e) {
                        // alert($(this).attr("data-column"));
                        lobjGrid.groupedColumns.push($(this).attr("data-column"));
                    });
                    // call on group
                    lobjGrid.onGroup();
                    domGrid = null;
                    $this = null;
                }
            });
        }
    };
    NeoGrid.prototype.renderRow = function (adomRow, aDataItem) {
        var larrColumns = this.irrEditableColumns;
        if (larrColumns.length > 0) {
            for (var c = 0, lintColLen = larrColumns.length; c < lintColLen; c++) {
                var lstrField = larrColumns[c]['field'];
                var lintrowIndex = adomRow.attr("rowIndex");
                var ldomTD = adomRow.find(["td[data-container-for='", lstrField, "']"].join(''));
                var lobjOptions = {
                    field: lstrField,
                    rowIndex: lintrowIndex,
                    sender: this,
                    item: aDataItem,
                    model: {
                        fields: this.columnFields,
                        rowIndex: lintrowIndex
                    }
                };
                ldomTD.empty();
                if (typeof larrColumns[c].editor === 'function') {
                    larrColumns[c].editor(ldomTD, lobjOptions);
                    if (ldomTD.find("input[type=text], textarea").length > 0) {
                        var ltextControl = $(ldomTD.find("input[type=text], textarea")[0]);
                        if (aDataItem.HiddendFields != undefined && aDataItem.HiddendFields[lstrField] !== undefined) {
                            aDataItem[lstrField] = "";
                            ltextControl.remove();
                            ltextControl = null;
                        }
                        else if (aDataItem.ReadOnlyFields != undefined && aDataItem.ReadOnlyFields[lstrField] !== undefined) {
                            var lstrFormat = (this.columnFields[lstrField] != undefined && this.columnFields[lstrField].format != undefined) ? this.columnFields[lstrField].format : ltextControl.attr("sfwdataformat") || ltextControl.attr("sfwextendcustom");
                            var lstrValue = (lstrFormat != undefined && lstrFormat != "") ? neoFormat.GetFormatedValue(lstrFormat, aDataItem[lstrField]) : aDataItem[lstrField];
                            $(["<span>", lstrValue, "</span>"].join('')).insertAfter(ltextControl);
                            ltextControl.remove();
                            ltextControl = null;
                        }
                        else {
                            ltextControl.val([aDataItem[lstrField]]);
                            ltextControl.attr("name", lstrField);
                            if (ltextControl[0].tagName === "INPUT")
                                this.applyFormatting(ltextControl, lstrField);
                        }
                    }
                    else if (ldomTD.find("select").length > 0) {
                        var lselectControl = $(ldomTD.find("select")[0]);
                        lselectControl.val([aDataItem[lstrField]]);
                        if (aDataItem.HiddendFields != undefined && aDataItem.HiddendFields[lstrField] !== undefined) {
                            aDataItem[lstrField] = "";
                            lselectControl.remove();
                            lselectControl = null;
                        }
                        else if (aDataItem.ReadOnlyFields != undefined && aDataItem.ReadOnlyFields[lstrField] !== undefined) {
                            var lstrValue = aDataItem[lstrField];
                            if (lselectControl.find('option').length > 0) {
                                lstrValue = lselectControl.children("option").filter(":selected").text();
                            }
                            $(["<span>", lstrValue, "</span>"].join('')).insertAfter(lselectControl);
                            lselectControl.remove();
                            lselectControl = null;
                        }
                        else {
                            lselectControl.attr("name", lstrField);
                        }
                    }
                    else if (ldomTD.find("span[controltype]").find("input[type=radio]").length > 0) {
                        var lspanControl = $(ldomTD.find("span[controltype]")[0]);
                        lspanControl.find("input[type=radio]").val([aDataItem[lstrField]]);
                        if (aDataItem.HiddendFields != undefined && aDataItem.HiddendFields[lstrField] !== undefined) {
                            aDataItem[lstrField] = "";
                            lselectControl.remove();
                            lselectControl = null;
                        }
                        else if (aDataItem.ReadOnlyFields != undefined && aDataItem.ReadOnlyFields[lstrField] !== undefined) {
                            lspanControl.find("input[type=radio]").attr("disabled", "disabled").end().attr("disabled", "disabled");
                            lspanControl = null;
                        }
                    }
                    else if (ldomTD.find("input[type=checkbox]").length > 0) {
                        var lchkControl = $(ldomTD.find("input[type=checkbox]")[0]);
                        if (aDataItem.HiddendFields != undefined && aDataItem.HiddendFields[lstrField] !== undefined) {
                            aDataItem[lstrField] = "";
                            lchkControl.remove();
                            lchkControl = null;
                        }
                        else if (aDataItem.ReadOnlyFields != undefined && aDataItem.ReadOnlyFields[lstrField] !== undefined) {
                            lchkControl.attr("disabled", "disabled");
                            lchkControl = null;
                        }
                        else {
                            lchkControl.attr("name", lstrField);
                        }
                    }
                } else {
                    this.renderTextBox(ldomTD, lobjOptions);
                }
            }
        }
    };
    NeoGrid.prototype.renderRawTemplateRow = function (adomRow, aDataItem) {
        if (this.iblnHasInputsInRawTemplate) {
            lintrowIndex = adomRow.attr("rowIndex");
            var controls = adomRow.find("input, select, textarea, span[controltype]");
            var lintControlLen = controls.length;
            if (lintControlLen > 0) {
                for (var j = 0; j < lintControlLen; j++) {
                    var control = $(controls[j]);
                    var lstrDataBind = control.attr('data-field') || control.attr('data-bind') || "";
                    lstrDataBind = lstrDataBind.substr(lstrDataBind.indexOf(":") + 1);
                    this.renderEditableControl(control, lstrDataBind, lintrowIndex, aDataItem);
                }
            }
        }
    };
    NeoGrid.prototype.renderEditableControl = function (control, astrDataField, aintRowIndex, aDataItem) {
        var lobjOptions = {
            field: astrDataField,
            rowIndex: aintRowIndex,
            sender: this,
            item: aDataItem,
            model: {
                fields: this.columnFields,
                rowIndex: aintRowIndex
            }
        };

        if ((control[0].tagName === "INPUT" && control[0].type === "text") || control[0].tagName === "TEXTAREA") {
            this.renderTextBox(control, lobjOptions);
        }
        else if (control[0].tagName === "INPUT" && control[0].type === "checkbox") {
            this.renderCheckBox(control, lobjOptions);
        }
            //else if (control[0].tagName === "INPUT" && control[0].type === "radio") {
            //    this.renderRadioButton(control, lobjOptions);
            //}
        else if (control[0].tagName === "SELECT") {
            this.renderDropDown(control, lobjOptions);
        }
        else if (control[0].tagName === "SPAN" && control.attr("controltype") != undefined && $.trim(control.attr("controltype")).toLowerCase() === "sfwradiobuttonlist") {
            this.renderRadioButtonList(control, lobjOptions);
        }
    };
    NeoGrid.prototype.renderSelectCell = function (adomRow, aDataItem) {
        var lstrField = "rowSelect";
        var lintrowIndex = adomRow.attr("rowIndex");
        var ldomTD = adomRow.find(["td[data-container-for='", lstrField, "']"].join(''));
        if (ldomTD.length > 0) {
            var lobjOptions = {
                field: lstrField,
                rowIndex: lintrowIndex,
                sender: this,
                item: aDataItem
            };
            ldomTD.empty();
            var lobjField = this.columnFields[lstrField];
            if (this.options.iblnCallRowSelectEditor && lobjField != undefined && typeof lobjField.editor === 'function') {
                lobjOptions.model = lobjField;
                lobjField.editor(ldomTD, lobjOptions);
            } else {
                this.rowSelectionMode(ldomTD, lobjOptions);
            }
        }
    };
    NeoGrid.prototype.rowSelectionMode = function (container, options) {
        var control;
        if (this.selection == 'single') {
            control = $(["<input class='s-grid-check-row' type='radio' title='Select Record' name='gridrowselect_", this.id, "'  GridID='", this.id, "' rowIndex='", options.rowIndex, "' data-bind='checked:", options.field, "' data-field='", options.field, "'></input>"].join(''));
        } else if (this.selection == 'multiple') {
            control = $(["<input class='s-grid-check-row' type='checkbox' title='Select Record' GridID='", this.id, "' rowIndex='", options.rowIndex, "' data-bind='checked:", options.field, "' data-field='", options.field, "' ></input>"].join(''));
        }
        if (control != undefined) {
            control[0].checked = options.item[options.field];
            control.appendTo(container);
        }
    };
    NeoGrid.prototype.renderTextBox = function (container, options) {
        var CustomAttributes = {};
        var Width = " style='width:auto;' ";
        var Class = "";
        var textbox;
        if (container[0].tagName === "INPUT" || container[0].tagName === "TEXTAREA") {
            textbox = container;
            // textbox.attr("data-field", options.field);
        }
        else {
            textbox = $(['<input type="text" GridID="', this.id, '" class="GridTextBox" ', Class, Width, ' rowIndex="', options.rowIndex, '"  data-bind="value:', options.field, '"  data-field="', options.field, '" name ="', options.field, '"/>'].join(''));
        }
        if (!(container[0].tagName === "INPUT" || container[0].tagName === "TEXTAREA")) {
            textbox.appendTo(container);
        }
        if (options.item.HiddendFields != undefined && options.item.HiddendFields[options.field] !== undefined) {
            options.item[options.field] = "";
            textbox.remove();
            textbox = null;
        }
        else if (options.item.ReadOnlyFields != undefined && options.item.ReadOnlyFields[options.field] !== undefined) {
            var lstrFormat = (options.model.fields[options.field] != undefined && options.model.fields[options.field].format != undefined) ? options.model.fields[options.field].format : textbox.attr("sfwdataformat") || textbox.attr("sfwextendcustom");
            var lstrValue = (lstrFormat != undefined && lstrFormat != "") ? neoFormat.GetFormatedValue(lstrFormat, options.item[options.field]) : options.item[options.field];
            $(["<span>", lstrValue, "</span>"].join('')).insertAfter(textbox);
            textbox.remove();
            textbox = null;
        }
        else {
            textbox.attr("name", options.field);
            var lstrValue = options.item[options.field];
            var format = (options.model.fields[options.field] != undefined && options.model.fields[options.field].format != undefined) ? options.model.fields[options.field].format : textbox.attr("sfwdataformat") || textbox.attr("sfwextendcustom");
            var type = (options.model.fields[options.field] != undefined && options.model.fields[options.field].type != undefined) ? options.model.fields[options.field].type : "string";
            if ((format === '{0:MM/dd/yyyy}' || format === '{0:d}') || (type === 'date')) {
                lstrValue =
                lstrValue = neoFormat.GetFormatedValue("{0:MM/dd/yyyy}", neoFormat.parseDate(lstrValue));
            }
            textbox.val([lstrValue]);
            if (textbox[0].tagName === "INPUT") {
                this.applyFormatting(textbox, options.field);
            }
        }
    };
    NeoGrid.prototype.applyFormatting = function (control, astrField) {
        if (control.length > 0) {
            var lobjFields = this.columnFields;
            var format = (lobjFields[astrField] != undefined && lobjFields[astrField].format != undefined) ? lobjFields[astrField].format : control.attr("sfwdataformat") || control.attr("sfwextendcustom");
            var type = (lobjFields[astrField] != undefined && lobjFields[astrField].type != undefined) ? lobjFields[astrField].type : "string";
            if (format != undefined && format != "") {
                format = neoFormat.replaceAll(format, "X~X", "\\\\#")
                format = neoFormat.replaceAll(format, "\\\\", "");
            }
            var lblnCanApplyDate = false;
            var lblnCanApplyMask = false;
            var lblnCanApplyCustomFormat = false;
            var lstrInputMask = "";
            if (format != undefined && format != "") {
                switch (format) {
                    case "{0:MM/dd/yyyy}":
                    case "{0:d}":
                    case "{0:D}":
                        lstrInputMask = "{0:MM/dd/yyyy}";
                        lblnCanApplyDate = true;
                        break;
                    case "{0:(###)###-####}":
                    case "{0:(\\#\\#\\#)\\#\\#\\#-\\#\\#\\#\\#}":
                    case "{0:(\\\\#\\\\#\\\\#)\\\\#\\\\#\\\\#-\\\\#\\\\#\\\\#\\\\#}":
                        lstrInputMask = "(999)999-9999";
                        lblnCanApplyMask = true;
                        break;
                    case "{0:000-##-####}":
                    case "{0:000-\\#\\#-\\#\\#\\#\\#}":
                    case "{0:000-\\\\#\\\\#-\\\\#\\\\#\\\\#\\\\#}":
                        lstrInputMask = "999-99-9999";
                        lblnCanApplyMask = true;
                        break;

                    case "{0:#0.00'%}":
                    case "{0:#0.000'%}":
                    case "{0:#0.0000'%}":
                    case "{0:#0.00000'%}":
                    case "{0:P}":
                    case "{0:p}":
                        lstrInputMask = format;
                        lblnCanApplyCustomFormat = false;
                        break;
                    default:
                        lstrInputMask = format;
                        lblnCanApplyCustomFormat = true;
                }
                if (lstrInputMask.indexOf("\\\\") >= 0) {
                    lstrInputMask = neoFormat.replaceAll(lstrInputMask, "\\\\", "");
                }
            }
            var lobjFormatDetails = {
                InputMask: lstrInputMask,
                lblnCanApplyCustomFormat: lblnCanApplyCustomFormat,
                lblnCanApplyDate: lblnCanApplyDate,
                lblnCanApplyDate: lblnCanApplyMask
            }
            control.attr("sfwExtendCustom", lstrInputMask);
            if (typeof this.options.CustomFormat === 'function') {
                var lobjContent = {
                    field: astrField,
                    control: control,
                    column: lobjFields,
                    format: format,
                    type: type,
                    lobjFormatDetails: lobjFormatDetails,
                    sender: this
                };
                this.options.CustomFormat(lobjContent);
            } else {
                if ((type === 'date') || (lstrInputMask === '{0:MM/dd/yyyy}')) {
                    NeoGrid.applyDate(control);
                    //control.css({
                    //    "width": "90px",
                    //    "float": "left",
                    //    "display": "inline-block"
                    //});
                    control.next().addClass("s-datepicker-img");
                }
                else {
                    NeoGrid.applyFormat(control, { format: lstrInputMask, type: type, IsMask: lblnCanApplyMask, lobjFormatDetails: lobjFormatDetails });
                }
            }
        }
    };

    NeoGrid.prototype.renderDropDown = function (container, options) {
        if (typeof NeoGrid.bindDropDown === 'function' && container.find('option').length == 0) {
            NeoGrid.bindDropDown(container, options)
            if (container.find('option').length > 0) {
                container.val([options.item[options.field]]);
                //container.attr("data-field", options.field);
                //container.val(options.item[options.field]);
                //options.item[options.field + "_Text"]
            }
        }
        else if (container.find('option').length > 0) {
            container.val([options.item[options.field]]);
        }
        if (options.item.HiddendFields != undefined && options.item.HiddendFields[options.field] !== undefined) {
            options.item[options.field] = "";
            container.remove();
            container = null;
        }
        else if (options.item.ReadOnlyFields != undefined && options.item.ReadOnlyFields[options.field] !== undefined) {
            var lstrValue = options.item[options.field];
            if (container.find('option').length > 0) {
                lstrValue = container.children("option").filter(":selected").text();
            }
            $(["<span>", lstrValue, "</span>"].join('')).insertAfter(container);
            container.remove();
            container = null;
        }
        else {
            container.attr("name", options.field);
        }
    };

    NeoGrid.prototype.renderRadioButtonList = function (container, options) {
        if (typeof NeoGrid.bindRadioButtonList === 'function' && container.find('input[type=radio]').length === 0) {
            NeoGrid.bindRadioButtonList(container, options);
            if (container.find('input').length > 0) {
                container.find('input[type=radio]').val([options.item[options.field]]);
                //container.val(options.item[options.field]);
                //options.item[options.field + "_Text"]
            }
        }
        else if (container.find('input[type=radio]').length > 0) {
            container.find('input[type=radio]').val([options.item[options.field]]);
        }
        if (options.item.HiddendFields != undefined && options.item.HiddendFields[options.field] !== undefined) {
            options.item[options.field] = "";
            container.remove();
            container = null;
        }
        else if (options.item.ReadOnlyFields != undefined && options.item.ReadOnlyFields[options.field] !== undefined) {
            container.find("input[type=radio]").attr("disabled", "disabled").end().attr("disabled", "disabled");
            container = null;
        }
    };

    NeoGrid.prototype.renderCheckBox = function (control, options) {
        if (typeof NeoGrid.bindCheckBox === 'function') {
            NeoGrid.bindCheckBox(control, options);
            if (options.item.HiddendFields != undefined && options.item.HiddendFields[options.field] !== undefined) {
                options.item[options.field] = "";
                control.remove();
                control = null;
            }
            else if (options.item.ReadOnlyFields != undefined && options.item.ReadOnlyFields[options.field] !== undefined) {
                control.attr("disabled", "disabled");
                control = null;
            }
            else {
                control.attr("name", options.field);
            }
        }
    };


    //Methods may be removed
    NeoGrid.applyDate = function (control) {
        if (control.datepicker != undefined && control.datepicker().length > 0) {
            control.datepicker("destroy");
        }
        var lstrDateRange = '1901:2200';
        control.datepicker({
            changeMonth: true,
            yearRange: lstrDateRange,
            changeYear: true,
            showOn: "button",
            buttonImage: [((ns.SiteName != undefined && ns.SiteName != "") ? ["/", ns.SiteName].join('') : ns.SiteName), "/images/calender.png"].join(''),
            buttonImageOnly: true
        });
        NeoGrid.applyFormat(control, { format: '99/99/9999', IsMask: true });
    };

    NeoGrid.getAggregatedRow = function (aobjGrid, data, lstrMaxTds) {
        var FooterRow = $(aobjGrid.istrFooterRowTemplate);
        var AggregateFields = _.pickBy(aobjGrid.columnFields, "aggregate");
        for (var field in AggregateFields) {
            var lobjToSend = {};
            for (var i = 0 ; i < AggregateFields[field].aggregate.length; i++) {
                var AggregateType = AggregateFields[field].aggregate[i];
                lobjToSend[AggregateType] = data["~~" + AggregateType.toLowerCase() + "_" + field];
            }
            var TdResult = AggregateFields[field].footerTemplate(lobjToSend);
            TdResult = NeoGrid.htmlEncodeForFooter(TdResult);
            FooterRow.find("[container-for=" + field + "]").html(TdResult);
        }
        FooterRow.prepend(lstrMaxTds);
        return FooterRow;
    };

    NeoGrid.getAggregatedData = function (colFields, data) {
        var FiledsWithAggregate = _.pickBy(colFields, function (item) {
            if (item.aggregate != undefined) {
                return true;
            }
        });

        if (_.map(FiledsWithAggregate, "field").length == 0) { return data }

        data = _.pickBy(data, function (element) {
            return NeoGrid.getAggregatedDataArray(element, FiledsWithAggregate);
        });

        return data;
    };

    NeoGrid.getAggregatedDataArray = function (element, FiledsWithAggregate) {
        for (var key in FiledsWithAggregate) {
            for (var i = 0; i < FiledsWithAggregate[key].aggregate.length; i++) {
                var aggregateType = FiledsWithAggregate[key].aggregate[i];
                switch (aggregateType.toLowerCase()) {
                    case "sum":
                        var temp = ("~~sum_" + key);
                        element[temp] = _.sumBy(element, key);
                        break;
                    case "min":
                        var temp = ("~~min_" + key);
                        element[temp] = _.minBy(element, key);
                        break;
                    case "max":
                        var temp = ("~~max_" + key);
                        element[temp] = _.maxBy(element, key);
                        break;
                    case "avg":
                        var temp = ("~~avg_" + key);
                        element[temp] = _.meanBy(element, key);
                        break;
                    case "count":
                        var temp = ("~~count_" + key);
                        element[temp] = element.length;
                        break;
                }
            }
        }
        return element;
    }

    NeoGrid.prototype.onGroup = function () {
        var groupByMulti = function (obj, values, context, colFields, ablnEditable, ablnTable) {
            if (!values.length)
                return obj;
            var column = colFields[values[0]];
            var colTitle = (column.title != undefined) ? column.title + ": " : column.field;
            var byFirst = _.groupBy(obj, values[0], context),
              rest = values.slice(1);



            byFirst = NeoGrid.getAggregatedData(colFields, byFirst);

            var lstrFormat = column.format;
            //if ((ablnEditable === true || column.editable === true) || ablnTable === true) {
            if (lstrFormat != undefined && lstrFormat != "") {
                byFirst = _.mapKeys(byFirst, function (value, key) {
                    var keyVal = key;
                    //if (lstrFormat != undefined && lstrFormat != "") 
                    keyVal = neoFormat.GetFormatedValue(lstrFormat, key);
                    return [colTitle, keyVal, ", Total: ", value.length, ""].join('');
                });
            }
            else {
                byFirst = _.mapKeys(byFirst, function (value, key) {
                    return [colTitle, key, ", Total: ", value.length, ""].join('');
                });
            }

            for (var prop in byFirst) {
                byFirst[prop] = groupByMulti(byFirst[prop], rest, context, colFields, ablnEditable, ablnTable);
            }

            for (var key in obj) {
                if (key.indexOf("~~") == 0)
                    byFirst[key] = obj[key];
            }

            return byFirst;
        };
        this.groupedData = groupByMulti(this.RenderData, this.groupedColumns, null, this.columnFields, this.iblnEditable, this.iblnTable);

        this.renderGrid();
    };
    NeoGrid.prototype.onSort = function (e) {
        var field = e.field;
        var result = $.grep(this.sortFields, function (evt) {
            return evt.field == field;
        });
        if (result.length > 0) {
            for (var i = 0; i < this.sortFields.length; i++) {
                if (this.sortFields[i].field == field) {
                    if (this.sortFields[i].dir == "asc") {
                        $(e.target).find("span.s-arrow").removeClass("s-asc").addClass("s-desc"); //.css("visibility", "visible");

                        this.sortFields[i].dir = "desc";
                    } else {
                        this.sortFields.splice(i, 1);
                        $(e.target).find("span.s-arrow").removeClass("s-desc").removeClass("s-asc");//.css("visibility", "hidden");
                    }
                    break;
                }
            }
        }
        else {
            $(e.target).find("span.s-arrow").removeClass("s-desc").addClass("s-asc");//.css("visibility", "visible");
            this.sortFields.push({
                field: field,
                dir: "asc"
            });
        }
        this.sort();
    };
    NeoGrid.prototype.sort = function () {
        var sortField = "rowIndex";
        if (this.options.sortExpression != undefined && this.options.sortExpression != "") {
            sortField = $.trim(this.options.sortExpression.split(" ")[0]);
        }
        this.RenderData = _.sortBy(this.RenderData, sortField);
        if (this.sortFields.length > 0) {
            this.sortFields.reverse();
            var lstrsortfield = "";
            var lstrFieldType = "";
            for (var i = 0; i < this.sortFields.length; i++) {
                lstrsortfield = this.sortFields[i].field;
                lstrFieldType = this.columnFields[lstrsortfield] != undefined && this.columnFields[lstrsortfield].type != undefined ? this.columnFields[lstrsortfield].type : "string";
                if (lstrFieldType == "number" || lstrFieldType == "date") {
                    this.RenderData = _.sortBy(this.RenderData, function (item) {
                        var OriginalValue = item[lstrsortfield];
                        if (lstrFieldType == "date") {
                            if (OriginalValue != null && OriginalValue !== "") {
                                var lDateVaue = neoFormat.parseDate(OriginalValue);
                                return lDateVaue != null ? lDateVaue : neoFormat.parseDate("01/01/0001").getTime();;
                            } else {
                                return neoFormat.parseDate("01/01/0001").getTime();
                            }
                        } else if (lstrFieldType == "number") {
                            if (OriginalValue != null && OriginalValue !== "" && String(OriginalValue).indexOf("%") >= 0) {
                                return neoFormat.parseFloat(OriginalValue);
                            }
                            else if (OriginalValue != null && OriginalValue !== "" && String(OriginalValue).indexOf("($") == 0) {
                                // OriginalValue = OriginalValue.replace("($", "-").replace(")", "");
                                return neoFormat.parseFloat(OriginalValue);
                            }
                            if (OriginalValue != null && OriginalValue !== "" && !isNaN(OriginalValue)) {
                                return neoFormat.parseFloat(OriginalValue);
                            } else {
                                var value = (OriginalValue == null || OriginalValue === "") ? "-9999999999" : String(OriginalValue);
                                if (value != null && value !== "" && !isNaN(value)) {
                                    return neoFormat.parseFloat(value);
                                }
                                value = value.match(/\d+(?:\.\d+)?/g);
                                value = value != null ? neoFormat.parseFloat(value.join("")) : -9999999999;
                                return value;
                            }
                        }
                    });
                } else {
                    this.RenderData = _.sortBy(this.RenderData, lstrsortfield);
                }
                if (this.sortFields[i].dir == 'desc')
                    this.RenderData.reverse();
                if (this.sortFields[i + 1] && this.sortFields[i + 1].dir == 'desc')
                    this.RenderData.reverse();
                if (this.columnFields[lstrsortfield] == undefined) {
                    this.sortFields.splice(i, 1);
                }
            }
            this.sortFields.reverse();

        }
        if (this.groupedColumns.length > 0 && this.iblnGrouping) {
            this.onGroup();
        } else {
            this.renderGrid();
        }
    }
    NeoGrid.prototype.setSort = function (aarrSortColumns) {
        if (aarrSortColumns.length > 0) {
            this.sortFields.splice(0);
            _.merge(this.sortFields, aarrSortColumns);
            if (this.iblnSorting) {
                for (var i = 0, len = this.sortFields.length; i < len; i++) {
                    this.element.find("th.s-sortable[data-field=" + this.sortFields[i].field + "] span.s-arrow").removeClass("s-asc").removeClass("s-desc").addClass("s-" + this.sortFields[i].dir);
                }
            }
        }
        else if (this.sortFields.length > 0) {
            if (this.iblnSorting) {
                for (var i = 0, len = this.sortFields.length; i < len; i++) {
                    this.element.find("th.s-sortable[data-field=" + this.sortFields[i].field + "] span.s-arrow").removeClass("s-asc").removeClass("s-desc");
                }
            }
            this.sortFields.splice(0);
        }
    };
    NeoGrid.prototype.setGroup = function (aarrGroupedColumns) {
        var lsgroupList = this.element.find(".s-groups");
        if (this.iblnTable)
            lsgroupList = this.gridContainer.find(".s-groups");
        if (aarrGroupedColumns.length == 0 && this.groupedColumns.length > 0) {
            if (lsgroupList != undefined && lsgroupList.length > 0) {
                lsgroupList.find("*").unbind().end().find('ul').empty();
                if (lsgroupList.find('ul li:not(.placeholder)').length === 0) {
                    $('<li class="placeholder s-groupds-drop-header">Drag a column header and drop it here to group by that column</li>').appendTo(lsgroupList.find('ul'));
                }
            }
            this.groupedColumns.splice(0);
        }
        else if (aarrGroupedColumns.length > 0) {
            this.groupedColumns.splice(0);
            _.merge(this.groupedColumns, aarrGroupedColumns);
            if (lsgroupList != undefined && lsgroupList.length > 0) {
                lsgroupList.find("*").unbind().end().find('ul').empty();
                for (var i = 0, len = this.groupedColumns.length; i < len; i++) {
                    var field = this.groupedColumns[i];
                    var groupingColumn = $('<li></li>').attr('data-column', field);
                    $('<span class="s-grid-ui-icon s-grid-ui-icon-close"></span>').attr('data-column', field).click(NeoGrid.onGroupClose).appendTo(groupingColumn);
                    groupingColumn.append(this.columnFields[field].title);
                    groupingColumn.appendTo(lsgroupList.find('ul'));
                }
            }
        }
        lsgroupList = null
    };
    NeoGrid.onGroupClose = function () {
        // call on group
        var ldomSpan = $(this);
        var field = ldomSpan.attr('data-column');
        var domGrid = ldomSpan.closest(".s-gridparent");
        if (domGrid.length == 0) {
            domGrid = ldomSpan.closest(".s-grid-helper").find("table.s-grid");
        }
        var lobjGrid = domGrid.data("neoGrid");
        if (lobjGrid.groupedColumns.indexOf(field) >= 0) {
            lobjGrid.groupedColumns.splice(lobjGrid.groupedColumns.indexOf(field), 1);
        }
        ldomSpan.parent().remove();
        lobjGrid.onGroup();
        var gridElement = lobjGrid.element;
        if (lobjGrid.iblnTable === true) {
            gridElement = lobjGrid.gridContainer;
        }
        if (gridElement.find('.s-groups ul li:not(.placeholder)').length === 0) {
            $('<li class="placeholder s-groupds-drop-header">Drag a column header and drop it here to group by that column</li>').appendTo(gridElement.find('.s-groups ul'));
        }
        ldomSpan = null;
        domGrid = null;
        gridElement = null;
    };
    NeoGrid.prototype.onFilter = function () {

        if (this.filterColumns.length > 0) {
            this.filterData = this.dataSource.data;
            var filterColumn;
            for (var i = 0, cnt = this.filterColumns.length; i < cnt; i++) {
                filterColumn = this.filterColumns[i];
                var field = filterColumn.field;
                var type = this.columnFields[field].type;

                this.filterData = _.filter(this.filterData, function (item) {
                    var result = true;
                    var firstValue = true;
                    var value = item[field];

                    if (filterColumn.filterBox1 != "" || filterColumn.selectFilterOptions1.toLowerCase() == "isnullorempty") {
                        firstValue = NeoGrid.GetResultOfContition(type, value, filterColumn.selectFilterOptions1.toLowerCase(), filterColumn.filterBox1);
                    }
                    var secondValue = true;

                    if (filterColumn.filterBox2 != "" || filterColumn.selectFilterOptions2.toLowerCase() == "isnullorempty") {
                        secondValue = NeoGrid.GetResultOfContition(type, value, filterColumn.selectFilterOptions2.toLowerCase(), filterColumn.filterBox2);
                    }

                    if (filterColumn.rdoAndOr === "or" && filterColumn.filterBox2 != "") {
                        return firstValue || secondValue;
                    }

                    return firstValue && secondValue;
                });
            }
        }
        this.setRenderData();
        if (this.sortFields.length > 0) {
            this.sort();
        }
        else if (this.groupedColumns.length > 0 && this.iblnGrouping) {
            this.onGroup();
        } else {
            this.renderGrid();
        }
    }
    NeoGrid.GetResultOfContition = function (type, value, condition, inputValue) {
        var result = false;

        switch (type) {
            case "string":
                value = value.toLowerCase();
                inputValue = inputValue.toLowerCase();
                switch (condition.toLowerCase()) {
                    case "isnullorempty":
                        if (value == "" || value == null) {
                            result = true;
                        }
                        break;
                    case "startswith":
                        if (value.indexOf(inputValue) == 0) {
                            result = true;
                        }
                        break;

                    case "contains":
                        if (value.indexOf(inputValue) >= 0) {
                            result = true;
                        }
                        break;

                    case "doesnotcontain":
                        if (value.indexOf(inputValue) < 0) {
                            result = true;
                        }
                        break;

                    case "endswith":
                        if (value.indexOf(inputValue, value.length - inputValue.length) !== -1) {
                            result = true;
                        }
                        break;
                    case "==":
                        if (value == inputValue) {
                            result = true;
                        }
                        break;
                    case "!=":
                        if (value == inputValue) {
                            result = true;
                        }
                        break;
                }
                break;
            case "date":
                value = neoFormat.parseDate(value);
                inputValue = neoFormat.parseDate(inputValue);
            case "number":
                if (type == "number") {
                    value = neoFormat.parseFloat(value);
                    inputValue = neoFormat.parseFloat(inputValue);
                }
                switch (condition) {
                    case "==":
                        if (+value == +inputValue) {
                            result = true;
                        }
                        break;
                    case "!=":
                        if (+value != +inputValue) {
                            result = true;
                        }
                        break;
                    case "<":
                        if (+value < +inputValue) {
                            result = true;
                        }
                        break;
                    case "<=":
                        if (+value <= +inputValue) {
                            result = true;
                        }
                        break;
                    case ">":
                        if (+value > +inputValue) {
                            result = true;
                        }
                        break;
                    case ">=":
                        if (+value >= +inputValue) {
                            result = true;
                        }
                        break;
                }
                break;

        }

        return result;
    }

    NeoGrid.registerEvents = function () {
        $(document).on("click", ".s-grid-btnClearFilter", function (e) {
            neo.FilterBoxControls.filterBox1.val("");
            neo.FilterBoxControls.filterBox2.val("");
            neo.FilterBoxControls.selectFilterOptions1.val("==");
            neo.FilterBoxControls.selectFilterOptions2.val("==");
            neo.FilterBox.find(".s-grid-btnFilter").trigger("click");
            neo.FilterBox.hide();
        });
        $(document).on("click", ".s-grid-btnFilter", function (e) {
            var gridElement = $(this).closest(".FilterBox").data("FilterGrid");
            if (gridElement != undefined) {
                var lobjSender = gridElement.data('neoGrid');
                var field = neo.FilterBox.attr("data-field");
                _.remove(lobjSender.filterColumns, function (n) {
                    return n.field == field;
                });
                var FilterObject = {
                    field: field,
                    selectFilterOptions1: neo.FilterBoxControls.selectFilterOptions1.val(),
                    selectFilterOptions2: neo.FilterBoxControls.selectFilterOptions2.val(),
                    rdoAndOr: neo.FilterBox.find('input[name=FilterCondition]:checked').val(),
                    filterBox1: neo.FilterBoxControls.filterBox1.val(),
                    filterBox2: neo.FilterBoxControls.filterBox2.val()
                }
                var FilterTrigger = neo.FilterBox.data("FilterTrigger");
                if (FilterObject.filterBox1 != "" || FilterObject.filterBox2 != "" || FilterObject.selectFilterOptions1.toLowerCase() == "isnullorempty") {
                    $(FilterTrigger).addClass("s-grid-FilterAdded");
                    lobjSender.filterColumns.push(FilterObject);
                }
                else {
                    $(FilterTrigger).removeClass("s-grid-FilterAdded");
                }
                lobjSender.onFilter();
            }
            neo.FilterBox.hide();
        });
        $(document).mouseup(function (e) {
            if (neo.FilterBox == undefined) {
                neo.SetFilterBox();
            }
            if (!neo.FilterBox.is(e.target) && e.target.className != "s-FilterTrigger" && !($(e.target).closest(".ui-datepicker").length > 0)// if the target of the click isn't the container...	
              && neo.FilterBox.has(e.target).length === 0) // ... nor a descendant of the container	
            {
                neo.FilterBox.hide();
            }
        });
        $(document).on("keydown", "div.FilterBox", function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode !== 13)
                return;
            $(this).find(".s-grid-btnFilter").trigger("click");
        });
        $(document).on("click", "span.s-FilterTrigger", function (e) {
            e.stopPropagation();
            var th = $(this).closest("th");
            var field = th.attr("data-field");
            var lobjSender = $(this).closest('.s-gridparent[data-role=neogrid]').data('neoGrid');
            neo.FilterBox.toggle();
            if (neo.FilterBox.is(":visible")) {
                neo.FilterBox.data("FilterGrid", lobjSender.element);
                neo.FilterBox.data("FilterTrigger", this);
                neo.ModifyFilterBox(lobjSender, field);
                var triggerBox = this.getBoundingClientRect();
                neo.FilterBox.neoGetInScreen(false, triggerBox);

            }
        });

        $(document).on("click", "table.s-grid thead th.s-sortable:not(th[data-field=rowSelect])", function (e) {
            var lobjSender = $(this).closest('.s-gridparent[data-role=neogrid]').data('neoGrid');
            var field = $(this).attr("data-field");
            lobjSender.onSort({
                field: field,
                target: this,
                sender: lobjSender
            });
        });
        $(document).on("change", "table.s-grid tbody.s-tbody input:not(input.s-grid-check-row),table.s-grid tbody.s-tbody select,table.s-grid tbody.s-tbody textarea", function (e) {
            var control = $(this);
            var lobjSender = control.closest('.s-gridparent[data-role=neogrid]').data('neoGrid');
            if (!lobjSender.iblnRendering) {
                var lstrDataField = control.closest('td[data-container-for]').attr("data-container-for");
                var lstrField = control.attr('data-field') || control.attr('data-bind') || lstrDataField || "";
                lstrField = lstrField.substr(lstrField.indexOf(":") + 1);

                var lintRowIndex = control.closest("[rowIndex]").attr("rowIndex");
                var items = $.grep(lobjSender.dataSource.data, function (dataItem) {
                    return dataItem.rowIndex == lintRowIndex;
                });

                if (control.attr("type") === "checkbox" && lstrField !== 'rowSelect') {
                    var CustomAttributes = {};
                    if (lobjSender.columnFields != undefined && lobjSender.columnFields[lstrField] != undefined && lobjSender.columnFields[lstrField].CustomAttributes != undefined)
                        CustomAttributes = lobjSender.columnFields[lstrField].CustomAttributes;
                    else if (control.attr("CustomAttributes") != undefined)
                        CustomAttributes = jQuery.parseJSON(element.attr("CustomAttributes") + "}");

                    var checkedVal = "Y";
                    var unCheckedVal = "N";
                    if (CustomAttributes["sfwValueChecked"] !== undefined)
                        checkedVal = CustomAttributes["sfwValueChecked"];

                    if (CustomAttributes["sfwValueUnChecked"] !== undefined)
                        unCheckedVal = CustomAttributes["sfwValueUnChecked"];
                    var valueToSet = unCheckedVal.trim();
                    if (control.is(":checked") === true) {
                        valueToSet = checkedVal.trim();
                    }
                    items[0][lstrField] = valueToSet;
                    control[0].checked = valueToSet === checkedVal.trim();
                } else {
                    var value = $(this).val();
                    if (lstrField === 'rowSelect') {
                        value = control.is(":checked");
                        value = ((value === "on") ? true : (value === "off" ? false : value));
                    }
                    var type = lobjSender.columnFields[lstrField].type;
                    var format = (lobjSender.columnFields[lstrField] != undefined && lobjSender.columnFields[lstrField].format != undefined) ? lobjSender.columnFields[lstrField].format : control.attr('sfwdataformat');
                    if (format != undefined && ($.trim(format).toLowerCase().indexOf("{0:c") === 0 || $.trim(format).toLowerCase().indexOf("%") > 0 || $.trim(format).toLowerCase() === "{0:p}")) {
                        value = neoFormat.parseFloat(control.autoNumeric('get'));
                    } else if (type == 'number') {
                        value = neoFormat.parseFloat(value);
                    }
                    else if ((type == 'date') || (format != undefined && ($.trim(format).toLowerCase().indexOf("{0:d") === 0 || $.trim(format).indexOf("{0:MM/dd/yyyy}") === 0))) {
                        if (value != "") {
                            value = neoFormat.parseDate(value);
                            if (value == null || (value != null & value < neoFormat.parseDate("01/01/1753", "MM/dd/yyyy"))) {
                                //alert("Invalid date");
                                control.focus();
                                control = null;
                                return false;
                            }
                            value = neoFormat.GetFormatedValue("{0:MM/dd/yyyy}", value);
                        }
                    }
                    value = value == null ? "" : value;
                    items[0][lstrField] = value;
                }
                items[0].dirty = true;
                if (typeof lobjSender.dataSource.change === 'function') {
                    lobjSender.dataSource.change({
                        field: lstrField,
                        rowIndex: lintRowIndex,
                        target: this,
                        items: items,
                        sender: lobjSender,
                        action: 'itemchange',
                        grid: 'neoGrid'
                    });
                }
            }
            control = null;
        });
        $(document).on("click", "table.s-grid thead th[data-field=rowSelect] input[type=checkbox]", function (e) {
            var control = $(this);
            var lbnlChecked = control.is(":checked");
            lbnlChecked = ((lbnlChecked === "on") ? true : (lbnlChecked === "off" ? false : lbnlChecked));
            var lobjSender = control.closest('.s-gridparent[data-role=neogrid]').data('neoGrid');
            var larrViewRows = lobjSender.view;
            for (var lintI = 0, lintViewRowsLen = larrViewRows.length; lintI < lintViewRowsLen; lintI++) {
                larrViewRows[lintI].rowSelect = lbnlChecked;
                var lchkSelected = $(lobjSender.element).find("tbody.s-tbody").find(["tr[rowIndex=", larrViewRows[lintI].rowIndex, "]"].join('')).find(".s-grid-check-row");
                if (lchkSelected.length > 0) {
                    lchkSelected[0].checked = lbnlChecked;
                }
            }
            control = null;
        });
        $(document).on("click", "table.s-grid tbody.s-tbody td[data-container-for=rowSelect] input.s-grid-check-row", function (e) {
            var control = $(this);
            var ilbnRadio = control.attr("type") === 'radio';
            var lbnlChecked = control.is(":checked");
            lbnlChecked = ((lbnlChecked === "on") ? true : (lbnlChecked === "off" ? false : lbnlChecked));
            var lobjSender = control.closest('.s-gridparent[data-role=neogrid]').data('neoGrid');
            var larrViewRows = lobjSender.view;
            var lintRowIndex = control.closest("[rowIndex]").attr("rowIndex");
            if (ilbnRadio) {
                var lobjData = $.grep(lobjSender.dataSource.data, function (dataItem) {
                    return (dataItem.rowSelect == true || dataItem.rowSelect == 'true');
                });
                if (lobjData.length > 0)
                    lobjData[0].rowSelect = false;
            }
            var items = $.grep(larrViewRows, function (dataItem) {
                return dataItem.rowIndex == lintRowIndex;
            });
            items[0].rowSelect = lbnlChecked;
            var selectAll = lbnlChecked;
            if (selectAll === true) {
                for (var lintI = 0, lintViewRowsLen = larrViewRows.length; lintI < lintViewRowsLen; lintI++) {
                    if (larrViewRows[lintI].rowSelect == false) {
                        selectAll = false;
                    }
                }
            }
            var chkAll = $(this).closest('table.s-grid[role=table]').find("th[data-field=rowSelect] input[type=checkbox]");
            if (chkAll.length > 0) {
                chkAll[0].checked = selectAll;
            }
            control = null;
        });

        $(document).on("click", "table.s-grid tbody.s-tbody td span.s-group-row-value", function (e) {
            var ldomSpan = $(this);
            var ldomParentTr = ldomSpan.closest("tr.s-group-row");//key="Product Name:
            var lstrKeyAttr = ldomParentTr.attr("key");
            lstrKeyAttr = lstrKeyAttr.substr(0, lstrKeyAttr.indexOf(":") + 1);
            ldomParentTr.nextUntil("[key^='" + lstrKeyAttr + "']").toggle();
            ldomSpan.toggleClass("s-group-row-value-expand").toggleClass("s-group-row-value-collapse");
            var lobjSender = ldomSpan.closest('.s-gridparent[data-role=neogrid]').data('neoGrid');
            lobjSender.setGroupHeaderWidth();
            ldomParentTr = null;
            ldomSpan = null;
        });

    };

    //Static methods
    NeoGrid.applyFormat = function (adomControl, aData) {
        var format = aData.format || adomControl.attr("sfwdataformat") || adomControl.attr("sfwextendcustom") || "";

        if (aData.IsMask === true) {
            adomControl.mask(aData.format);
        }
        if (aData.lobjFormatDetails != undefined && aData.lobjFormatDetails.lblnCanApplyMask === false && aData.lobjFormatDetails.lblnCanApplyCustomFormat === false) {
            NeoGrid.applyCustomFormat(adomControl, format);
        }
        if (aData.lobjFormatDetails != undefined && aData.lobjFormatDetails.lblnCanApplyCustomFormat === true) {
            NeoGrid.applyCustomFormat(adomControl);
        }
            //else if (format != undefined && format.toLowerCase().indexOf("{0:c") == 0) {
            //    NeoGrid.applyCurrencyFormat(adomControl, aData.format)
            //}
            //else if (format != undefined && (format.toLowerCase() == "{0:p}" || format.indexOf("%") > 0)) {
            //    if (format.toLowerCase() == "{0:p}") {
            //        format = "{0:#0.00'%}";
            //    }
            //    NeoGrid.applyCustomFormat(adomControl, format);
            //}
            //else if (format != undefined && format != "") {
            //    NeoGrid.applyCustomFormat(adomControl, format);
            //}
        else if (aData.type === "number") {
            adomControl.neoForceNumeric();
            if (adomControl.attr("maxlength") == undefined) {
                adomControl.attr("maxlength", "10");
            }
        }
        adomControl.trigger("focus");
        adomControl.trigger("blur");
    };

    NeoGrid.applyCurrencyFormat = function (control, format) {
        var lstrMinNumericRange = "-999999999";
        var lstrMaxNumericRange = "999999999";
        var lstrPrecisionString = ".";
        var lintMinPresicionCount = 2;
        var lintMaxPrecisionCount = 10;
        var lintFormatLength = 4; // Length pf  "{0:C

        var lintPresicionCount = lintMinPresicionCount;
        var lstrDataFormat = format;
        if (lstrDataFormat != undefined && lstrDataFormat != "") {
            lstrDataFormat = lstrDataFormat.slice(0, -1);
            var lstrPresicionCount = lstrDataFormat.substring(lintFormatLength);
            if (lstrPresicionCount != "" && !isNaN(Number(lstrPresicionCount))) {
                if (Number(lstrPresicionCount) > 0) {
                    if (Number(lstrPresicionCount) > lintMaxPrecisionCount)
                        lintPresicionCount = lintMaxPrecisionCount;
                    else
                        lintPresicionCount = Number(lstrPresicionCount);
                }
            }
        }
        for (var i = 0; i < lintPresicionCount; i++) {
            lstrPrecisionString = [lstrPrecisionString, "9"].join('');
        }
        lstrMinNumericRange = neoFormat.parseFloat([lstrMinNumericRange, lstrPrecisionString].join(''));
        lstrMaxNumericRange = neoFormat.parseFloat([lstrMaxNumericRange, lstrPrecisionString].join(''));
        $(control).autoNumeric('init', {
            aSign: "$",
            nBracket: '(,)',
            aNeg: '-',
            vMax: lstrMaxNumericRange,
            vMin: lstrMinNumericRange
            // ,Sign: 's'
        }).focus(function (e) { $(this).select() });

        /*
        $(control).autoNumeric({ aNeg: '-', vMin: lstrMinNumericRange, vMax: lstrMaxNumericRange, aSign: '$' })
            .focus(function (e) {
                if ($(this).attr("selected")) {
                    $(this).removeAttr("selected");
                    return;
                }
                var ctrlvalue = $(this).val();
                var that = this;
                if (ctrlvalue.length > 1) {
                    if ((ctrlvalue.substr(0, 1) == '(') && ctrlvalue.substr(ctrlvalue.length - 1, 1) == ')') {
                        ctrlvalue = ['-', ctrlvalue.substring(1, ctrlvalue.length - 1)].join('');
                        $(this).val(ctrlvalue);
                        if (!ns.blnLoading) {
                            setTimeout(function () {
                                $(that).attr("selected", nsConstants.TRUE);
                                $(that).select();
                            }, 30);
                        }
                    }
                    else {
                        if (!ns.blnLoading) {
                            setTimeout(function () {
                                $(that).attr("selected", nsConstants.TRUE);
                                $(that).select();
                            }, 30);
                        }
                    }
                }
            }).blur(function (e) {
                $(this).removeAttr("selected");
            });*/
    }
    NeoGrid.applyCustomFormat = function (control, dataformat) {
        if (dataformat == null) {
            dataformat = $(control).attr('sfwExtendCustom');
        }
        if (dataformat == "{0:999999999}") {
            $(control).neoForceNumeric();
            if ($(control).attr("maxlength") == undefined) {
                $(control).attr("maxlength", 10);
            }
        }
        if ((dataformat != null) && (dataformat.length > 0)) {
            // Check for numeric data format
            if ((dataformat.substr(0, 4) == '{0:9') && (dataformat.indexOf(".") < 0) && dataformat.substr(dataformat.length - 2, 2) == '9}') {
                var format = dataformat.substring(3, dataformat.length - 1);
                if (format.length > 0) {
                    //$(control).autoNumeric({ aSep: '', vMin: '-' + format, vMax: format });
                    $(control).neoForceNumeric();
                    if ($(control).attr("maxlength") == undefined || $(control).attr("maxlength") == null) {
                        $(control).attr("maxlength", format.length);
                    }
                }
            }
            else if (dataformat.toLowerCase().indexOf("{0:c") == 0) {
                NeoGrid.applyCurrencyFormat(control, dataformat);
            }
            else if (dataformat.substr(0, 6) == '{0:#0.' && dataformat.substr(dataformat.length - 3, 3) == '\'%}') {
                var format = dataformat.substring(6, dataformat.length - 3);
                format = ['999999999.', format.replace(/0/g, '9')].join('');
                $(control).autoNumeric({ aNeg: '-', vMin: ['-', format].join(''), vMax: format, aSign: '%', pSign: 's' });
            }
            else if ((dataformat.substr(0, 4) == '{0:n') ||
                (dataformat.substr(0, 4) == '{0:9' && (dataformat.indexOf(".") > 0) && dataformat.substr(dataformat.length - 2, 2) == '9}')) {
                var format = dataformat.substring(3, dataformat.length - 1);
                if (dataformat.indexOf(".") < 0) {
                    var format = dataformat.substring(4, dataformat.length - 1);
                    var declen = parseInt(format);
                    format = '999999999.';
                    for (var i = 0; i < declen; i++)
                        format = [format + '9'].join('');
                }
                $(control).autoNumeric({ aNeg: '-', vMin: ['-', format].join(''), vMax: format })
                    .focus(function (e) {
                        var ctrlvalue = $(control).val();
                        if (ctrlvalue.length > 1) {
                            if ((ctrlvalue.substr(0, 1) == '(') && ctrlvalue.substr(ctrlvalue.length - 1, 1) == ')') {
                                ctrlvalue = ['-', ctrlvalue.substring(1, ctrlvalue.length - 1)].join('');
                                $(control).val(ctrlvalue);
                                setTimeout(function () {
                                    $(control).select();
                                }, 30);
                            }
                            else {
                                setTimeout(function () {
                                    $(control).select();
                                }, 30);
                            }
                        }
                    });
            }
            else if (dataformat === "{0:00-#######}" || dataformat === "{0:00-\\\\#\\\\#\\\\#\\\\#\\\\#\\\\#\\\\#}") {
                NeoGrid.applyFormat($(control), { format: '99-9999999', IsMask: true });
            }
        }
    }
    NeoGrid.generateGUID = function () {
        var lintCurentTime = new Date().getTime();
        var lstrGuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var lintRandom = (lintCurentTime + Math.random() * 16) % 16 | 0;
            lintCurentTime = Math.floor(lintCurentTime / 16);
            return (c == 'x' ? lintRandom : (lintRandom & 0x3 | 0x8)).toString(16);
        });
        return lstrGuid;
    };

    NeoGrid.GetFormatedValue = function (format, value) {
        return neoFormat.GetFormatedValue(format, value);
    };
    NeoGrid.FormatValue = function (val, format) {
        return neoFormat.FormatValue(format, value);
    };
    NeoGrid.applyCellAndRowFormatting = function (adomRow, aDataItem, CellFormatAttributes, RowFormatAttributes) {
        if (CellFormatAttributes == null && RowFormatAttributes == null) {
            return;
        }
        if (CellFormatAttributes != null) {
            for (field in CellFormatAttributes) {
                var ldomTd = adomRow.find(["td[data-container-for='", field, "']"].join(""))
                if (ldomTd.length > 0) {
                    var checkval = aDataItem[CellFormatAttributes[field].DataField];
                    for (val in CellFormatAttributes[field]) {
                        if (val == checkval) {
                            ldomTd.addClass(CellFormatAttributes[field][val]);
                        }
                    }
                }
            }
        }
        if (RowFormatAttributes != null) {
            for (field in RowFormatAttributes) {
                for (val in RowFormatAttributes[field]) {
                    var classToAdd = RowFormatAttributes[field][val];
                    if (aDataItem[field] == val) {
                        adomRow.addClass(classToAdd);
                    }
                }
            }
        }
    };

    NeoGrid.htmlEncodeForFooter = function (value) {
        return value;
        //value = value.replace(/<br>/g, "\n");
        //return "<pre>" + $('<div/>').text(value).html() + "</pre>";
    };

    //Control Binders
    //call back methods

    NeoGrid.prototype.onRowRender = function (e) {
        var columnFileds = this.columnFields;
        var CellFormatAttributes = this.options.CellFormatAttributes;
        var RowFormatAttributes = this.options.RowFormatAttributes;
        if (!(CellFormatAttributes == null && RowFormatAttributes == null)) {
            NeoGrid.applyCellAndRowFormatting(e.row, e.item, CellFormatAttributes, RowFormatAttributes);
        }
        if ((e.item.HiddendFields != undefined && Object.keys(e.item.HiddendFields).length > 0)
            || (e.item.ReadOnlyFields != undefined && Object.keys(e.item.ReadOnlyFields).length > 0)) {
            var NonEditableControls = e.row.find("label[data-bind], a[databind], div.GridLinks[databind], span[data-field]:not([controltype])");
            if (NonEditableControls != undefined && NonEditableControls.length > 0) {
                var lintNonControlLen = NonEditableControls.length;
                for (var lintC = 0; lintC < lintNonControlLen; lintC++) {
                    var nonEditcontrol = $(NonEditableControls[lintC]);
                    var lstrDataBindField = nonEditcontrol.attr('data-field') || nonEditcontrol.attr('data-bind') || nonEditcontrol.attr('databind');
                    if (lstrDataBindField != undefined && lstrDataBindField != "") {
                        if (nonEditcontrol.hasClass("GridLinks")) {
                            nonEditcontrol = nonEditcontrol.parent();
                        }
                        lstrDataBindField = lstrDataBindField.substr(lstrDataBindField.indexOf(":") + 1);
                        if (e.item.HiddendFields != undefined && e.item.HiddendFields[lstrDataBindField] !== undefined) {
                            e.item[lstrDataBindField] = "";
                            nonEditcontrol.remove();
                        }
                        else if (e.item.ReadOnlyFields != undefined && e.item.ReadOnlyFields[lstrDataBindField] !== undefined) {
                            var lstrFormat = (this.columnFields[lstrDataBindField] != undefined && this.columnFields[lstrDataBindField].format != undefined) ? this.columnFields[lstrDataBindField].format : nonEditcontrol.attr("sfwdataformat") || nonEditcontrol.attr("sfwextendcustom");
                            var lstrValue = (lstrFormat != undefined && lstrFormat != "") ? neoFormat.GetFormatedValue(lstrFormat, e.item[lstrDataBindField]) : e.item[lstrDataBindField];
                            $(["<span>", lstrValue, "</span>"].join('')).insertAfter(nonEditcontrol);
                            nonEditcontrol.remove();
                        }
                    }
                    nonEditcontrol = null;
                }
            }
            NonEditableControls = null;
        }
        e.row.find("td[role='gridcell']").each(function () {
            var $td = $(this);
            var field = $td.attr("data-container-for") != undefined ? $td.attr("data-container-for") : (($td.css("display") === "none") ? "rowIndex" : "");
            var title = (columnFileds[field] != undefined && columnFileds[field].title != undefined) ? columnFileds[field].title : "";
            $td.attr("data-th", title);
            if ($td.text() == "" && $td.find("*").length == 0)
                $td.addClass("HideInResponsive");
            $td = null;
        });
        columnFileds = null;
        if (typeof this.options.rowRender === 'function') {
            this.options.rowRender(e);
        }
    };

    NeoGrid.prototype.onDataBind = function () {
        var selectAll = false;
        if (this.totalRecords > 0) {
            selectAll = true;
            if (selectAll === true) {
                for (var lintI = 0, lintViewRowsLen = this.view.length; lintI < lintViewRowsLen; lintI++) {
                    if (this.view[lintI].rowSelect == false) {
                        selectAll = false;
                        break;
                    }
                }
            }
        }
        var chkAll = this.element.find('thead').find("th[data-field=rowSelect] input[type=checkbox]");
        if (chkAll.length > 0) {
            chkAll[0].checked = selectAll;
        }
        if (typeof this.options.dataBound === 'function') {
            this.options.dataBound({
                sender: this
            });
        }
    };

    NeoGrid.prototype.onPageChange = function (e) {
        if (typeof this.options.pageChange === 'function') {
            this.options.pageChange(e);
        }
    };

    //Export To Excel
    NeoGrid.prototype.exportToExcel = function (aData) {
        var cols = (aData != undefined && aData.columns != undefined) ? aData.columns : [];
        var fileName = "Excel.xlsx"
        if (aData != undefined && aData.fileName != undefined && aData.fileName != "") {
            fileName = aData.fileName;
            fileName = fileName.substr(fileName.indexOf(".") + 1, fileName.length).toLowerCase() === "xlsx" ? fileName : "Excel.xlsx";
        }

        //var larrHeaderCell = (aData != undefined && aData.HeaderCell != undefined)? aData.HeaderCell: [];

        var lobjGrid = this;
        var data;
        var dataToSend
        var lblnGrouped = false,
          lintGroupedLen = lobjGrid.groupedColumns.length,
          larrGroupedTitles;
        if (cols.length == 0)
            cols = _.map(_.filter(lobjGrid.columns, function (col) {
                return ((col.field != 'rowSelect' && col.field != 'rowIndex' && !(col.hidden === true || col.hidden === 'true')) || (col.sfwShowInExportToExcel === 'True' || col.sfwShowInExportToExcel === true));
            }), function (col) {
                return (col.field)
            });
        //var larrHeaderCell = _.map(_.filter(lobjGrid.columns, function (col) {
        //    return ((col.field != 'rowSelect' && col.field != 'rowIndex' && !(col.hidden === true || col.hidden === 'true')) || (col.sfwShowInExportToExcel === 'True' || col.sfwShowInExportToExcel === true));
        //}), function (col) {
        //    return (col.title != undefined ? col.title : col.field);
        //});
        var lobjColFields = _.assign({}, lobjGrid.columnFields);
        var larrHeaderCell = _.map(_.pick(lobjColFields, cols), function (col) {
            return (col.title != undefined ? col.title : col.field);
        });

        var lobjAggregatedFields = _.filter(lobjGrid.columnFields, "aggregate");
        var lstrFooterRowTemplate = lobjGrid.istrFooterRowTemplate;

        if (lintGroupedLen <= 0) {
            var data = lobjGrid.RenderData;
            dataToSend = _.map(data, function (col) {
                var lCol = col;
                return _.map(_.pick(col, cols), function (value, key) {
                    return neoFormat.GetFormatedValue(lobjColFields[key].format, value);
                });
            });
            //adding aggregate row
            if (lobjAggregatedFields.length > 0) {
                var lobjLikeGridForAggregatedRow = {
                    istrFooterRowTemplate: lstrFooterRowTemplate,
                    columnFields: lobjColFields
                }
                var FooterRow = NeoGrid.getAggregatedRow(lobjLikeGridForAggregatedRow, data, /*lstrMaxTds*/"");
                var AgrOutRow = [];

                $(FooterRow).find("td").each(function () {
                    var field = $(this).attr("container-for");
                    if (field != "rowSelect" && lobjColFields[field].hidden == undefined) {
                        AgrOutRow.push($(this).text());
                    }
                });
                dataToSend.push(AgrOutRow);
            }

            dataToSend.unshift(larrHeaderCell);
        } else {
            larrGroupedTitles = _.map(lobjGrid.groupedColumns, function (col) {
                return (lobjColFields[col].title != undefined ? lobjColFields[col].title : lobjColFields[col].field);
            });
            //for (var i = 0; i < lintGroupedLen; i++) {
            //    larrHeaderCell.unshift(null);
            //}
            larrHeaderCell = _.map(lobjGrid.groupedColumns, function () { return null; }).concat(larrHeaderCell);
            dataToSend = lobjGrid.groupedData;
            lblnGrouped = true;
        }

        var lobjData = {
            aData: dataToSend,
            albnIsGrouped: lblnGrouped,
            arrColumns: cols,
            arrHeaderCell: larrHeaderCell,
            aiintGroupedColumns: lintGroupedLen,
            aarrGroupedTitles: larrGroupedTitles,
            aobjColFields: lobjColFields,
            aobjAggregatedFields: lobjAggregatedFields,
            astrFooterRowTemplate: lstrFooterRowTemplate
        };
        NeoGrid.exportDataToExcel(lobjData, fileName);
    }

    NeoGrid.exportDataToExcel = function (aobjData, astrFileName) {
        var oo;
        // aData, albnIsGrouped, arrColumns, arrHeaderCell, aiintGroupedColumns
        if (aobjData.albnIsGrouped) {
            oo = NeoGrid.generateRowsFromGroupedJSON(aobjData);
        }
        var ranges = aobjData.albnIsGrouped && oo != undefined ? oo[1] : [];
        /* original data */
        var data = aobjData.albnIsGrouped && oo != undefined ? oo[0] : aobjData.aData;
        var ws_name = "ExcelJS";
        var wb = new Workbook(),
          ws = NeoGrid.sheet_from_array_of_arrays(data);

        /* add ranges to worksheet */
        ws['!merges'] = ranges;

        /* add worksheet to workbook */
        wb.SheetNames.push(ws_name);
        wb.Sheets[ws_name] = ws;
        var wscols = _.map(aobjData.arrHeaderCell, function (col) {
            return (col == null) ? {
                wch: 2
            } : {
                wch: 20
            };
        })

        /* TEST: column widths */
        ws['!cols'] = wscols;

        /* write file */
        // XLSX.writeFile(wb, 'sheetjs.xlsx');
        var wbout = XLSX.write(wb, {
            bookType: 'xlsx',
            bookSST: false,
            type: 'binary'
        });
        astrFileName = astrFileName || "Excel.xlsx";
        NeoGrid.saveAs(new Blob([NeoGrid.s2ab(wbout)], {
            type: "application/octet-stream"
        }), astrFileName)
    }
    NeoGrid.generateRowsFromGroupedJSON = function (aobjData) {
        //aData: dataToSend, albnIsGrouped: lblnGrouped, arrColumns: cols, arrHeaderCell: cols, aiintGroupedColumns: lintGroupedLen, aarrGroupedTitles: larrGroupedTitles

        var out = [];
        //Push Headers
        out.unshift(aobjData.arrHeaderCell);
        var rows = aobjData.aData;
        var ranges = [];
        var colLen = aobjData.arrColumns.length;
        var rowsLen = 1;
        var lobjColFields = aobjData.aobjColFields;
        var CreateExcelData = function (data, aColumns, tds, aout, aranges, acolLen, aiintGroupedColumns, aarrGroupedTitles, aobjColFields) {
            if ($.isArray(data) && data[0][aColumns[0]] != undefined) {
                for (var R = 0, RLen = data.length; R < RLen; ++R) {
                    var outRow = [],
                      row = data[R],
                      columns = aColumns;
                    outRow = tds.concat(outRow);
                    for (var C = 0; C < columns.length; ++C) {
                        var cell = columns[C];
                        var cellValue;
                        var format = aobjColFields[cell].format;
                        if (format != undefined && format != "") {
                            cellValue = neoFormat.GetFormatedValue(format, row[cell]);
                        }
                        else {
                            cellValue = row[cell];
                        }
                        if (cellValue !== "" && cellValue == +cellValue) cellValue = +cellValue;
                        //Skip ranges
                        aranges.forEach(function (range) {
                            if (rowsLen >= range.s.r && rowsLen <= range.e.r && outRow.length >= range.s.c && outRow.length <= range.e.c) {
                                for (var i = 0; i <= range.e.c - range.s.c; ++i) outRow.push(null);
                            }
                        });
                        //Handle Value
                        outRow.push(cellValue !== "" ? cellValue : null);
                    }
                    aout.push(outRow);
                    rowsLen++;
                }
            } else {
                for (var key in data) {
                    if (key.indexOf("~~") == 0) {
                        continue;
                    }
                    var keyTitle = $.trim(key.substr(0, (key.indexOf(":"))));
                    var index = aarrGroupedTitles.indexOf(keyTitle);
                    if (tds.length > index)
                        tds.splice(index);
                    tds.push(null);
                    var tdClone = _.assign([], tds);
                    tdClone.pop(null); //tds.replace("<td class='s-empty-td'></td>", "");
                    var outRow = [],
                      columns = aColumns,
                      colspan = colLen + (aiintGroupedColumns - tdClone.length), //(colLen + (aobjGrid.groupedColumns.length - larrTds.length - 1))
                      rowspan, cellValue = key;
                    if (cellValue !== "" && cellValue == +cellValue) cellValue = +cellValue;
                    outRow = tdClone.concat(outRow);
                    //Skip ranges
                    aranges.forEach(function (range) {
                        if (rowsLen >= range.s.r && rowsLen <= range.e.r && outRow.length >= range.s.c && outRow.length <= range.e.c) {
                            for (var i = 0; i <= range.e.c - range.s.c; ++i) outRow.push(null);
                        }
                    });

                    //Handle Row Span
                    if (rowspan || colspan) {
                        rowspan = rowspan || 1;
                        colspan = colspan || 1;
                        ranges.push({
                            s: {
                                r: rowsLen,
                                c: outRow.length
                            },
                            e: {
                                r: rowsLen + rowspan - 1,
                                c: outRow.length + colspan - 1
                            }
                        });
                    };

                    //Handle Value
                    outRow.push(cellValue !== "" ? cellValue : null);

                    //Handle Colspan
                    if (colspan)
                        for (var k = 0; k < colspan - 1; ++k) outRow.push(null);

                    aout.push(outRow);
                    rowsLen++;
                    CreateExcelData(data[key], aColumns, tds, aout, aranges, acolLen, aiintGroupedColumns, aarrGroupedTitles, aobjColFields);

                    //adding aggregate row
                    if (aobjData.aobjAggregatedFields.length > 0) {
                        var lobjLikeGridForAggregatedRow = {
                            istrFooterRowTemplate: aobjData.astrFooterRowTemplate,
                            columnFields: aobjColFields
                        }
                        var AggregateFields = _.pickBy(aobjData.aobjColFields, "aggregate");
                        var FooterRow = NeoGrid.getAggregatedRow(lobjLikeGridForAggregatedRow, data[key], /*lstrMaxTds*/"");
                        var AgrOutRow = [];
                        var tdClone = _.assign([], tds);
                        tdClone.pop(null);
                        AgrOutRow = tdClone.concat(AgrOutRow);

                        $(FooterRow).find("td").each(function () {
                            AgrOutRow.push($(this).text());
                        });
                        aout.push(AgrOutRow);
                        rowsLen++;
                    }

                    var j = 10;
                }

            }
        };
        var arrTds = [];
        CreateExcelData(rows, aobjData.arrColumns, arrTds, out, ranges, colLen, aobjData.aiintGroupedColumns, aobjData.aarrGroupedTitles, lobjColFields)
        return [out, ranges];
    };
    NeoGrid.generatArrayFromJSON = function (data, ablnIsGrouped, aColumns) {
        var out = [];
        var rows = data;
        var ranges = [];
        for (var R = 0; R < rows.length; ++R) {
            var outRow = [];
            var row = rows[R];
            var columns = aColumns;
            for (var C = 0; C < columns.length; ++C) {
                var cell = columns[C];
                var colspan;
                var rowspan;
                var cellValue = row[cell];
                if (cellValue !== "" && cellValue == +cellValue) cellValue = +cellValue;

                //Skip ranges
                ranges.forEach(function (range) {
                    if (R >= range.s.r && R <= range.e.r && outRow.length >= range.s.c && outRow.length <= range.e.c) {
                        for (var i = 0; i <= range.e.c - range.s.c; ++i) outRow.push(null);
                    }
                });

                //Handle Row Span
                if (rowspan || colspan) {
                    rowspan = rowspan || 1;
                    colspan = colspan || 1;
                    ranges.push({
                        s: {
                            r: R,
                            c: outRow.length
                        },
                        e: {
                            r: R + rowspan - 1,
                            c: outRow.length + colspan - 1
                        }
                    });
                };

                //Handle Value
                outRow.push(cellValue !== "" ? cellValue : null);

                //Handle Colspan
                if (colspan)
                    for (var k = 0; k < colspan - 1; ++k) outRow.push(null);
            }
            out.push(outRow);
        }
        return [out, ranges];
    };
    NeoGrid.datenum = function (v, date1904) {
        if (date1904) v += 1462;
        var epoch = Date.parse(v);
        return (epoch - new Date(Date.UTC(1899, 11, 30))) / (24 * 60 * 60 * 1000);
    }
    NeoGrid.sheet_from_array_of_arrays = function (data, opts) {
        var ws = {};
        var range = {
            s: {
                c: 10000000,
                r: 10000000
            },
            e: {
                c: 0,
                r: 0
            }
        };
        for (var R = 0; R != data.length; ++R) {
            for (var C = 0; C != data[R].length; ++C) {
                if (range.s.r > R) range.s.r = R;
                if (range.s.c > C) range.s.c = C;
                if (range.e.r < R) range.e.r = R;
                if (range.e.c < C) range.e.c = C;
                var cell = {
                    v: data[R][C]
                };
                if (cell.v == null) continue;
                var cell_ref = XLSX.utils.encode_cell({
                    c: C,
                    r: R
                });

                if (typeof cell.v === 'number') cell.t = 'n';
                else if (typeof cell.v === 'boolean') cell.t = 'b';
                else if (cell.v instanceof Date) {
                    cell.t = 'n';
                    cell.z = XLSX.SSF._table[14];
                    cell.v = NeoGrid.datenum(cell.v);
                } else cell.t = 's';
                cell.bold = 1;
                ws[cell_ref] = cell;
            }
        }
        if (range.s.c < 10000000) ws['!ref'] = XLSX.utils.encode_range(range);
        return ws;
    }
    NeoGrid.s2ab = function (s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    return NeoGrid;
}());
//Workbook class
function Workbook() {
    if (!(this instanceof Workbook)) return new Workbook();
    this.SheetNames = [];
    this.Sheets = {};
}

//neoGrid Plugin
(function ($) {
    var neo = window.neo = window.neo || {};
    $.fn.neoGetInScreen = function (partial, trigerbox) {
        //let's be sure we're checking only one element (in case function is called on set)
        var t = $(this).first();
        //set starting position
        $(this).css("left", trigerbox.left + "px");
        $(this).css("top", trigerbox.bottom + "px");

        //we're using getBoundingClientRect to get position of element relative to viewport
        //so we dont need to care about scroll position
        var box = t[0].getBoundingClientRect();

        //let's save window size
        var win = {
            h: $(window).height(),
            w: $(window).width()
        };

        //now we check against edges of element

        //firstly we check one axis
        //for example we check if left edge of element is between left and right edge of scree (still might be above/below)
        var topEdgeInRange = box.top >= 0 && box.top <= win.h;
        var bottomEdgeInRange = box.bottom >= 0 && box.bottom <= win.h;
        var leftEdgeInRange = box.left >= 0 && box.left <= win.w;
        var rightEdgeInRange = box.right >= 0 && box.right <= win.w;

        //here we check if element is bigger then window and 'covers' the screen in given axis
        var coverScreenHorizontally = box.left <= 0 && box.right >= win.w;
        var coverScreenVertically = box.top <= 0 && box.bottom >= win.h;

        //now we check 2nd axis
        var topEdgeInScreen = topEdgeInRange && (leftEdgeInRange || rightEdgeInRange || coverScreenHorizontally);
        var bottomEdgeInScreen = bottomEdgeInRange && (leftEdgeInRange || rightEdgeInRange || coverScreenHorizontally);

        var leftEdgeInScreen = leftEdgeInRange && (topEdgeInRange || bottomEdgeInRange || coverScreenVertically);
        var rightEdgeInScreen = rightEdgeInRange && (topEdgeInRange || bottomEdgeInRange || coverScreenVertically);

        var bodyBox = $("body")[0].getBoundingClientRect();

        if (!leftEdgeInRange) {
            if (box.left < 0) {
                $(this).css("left", (0) + "px")
            } else if (box.left > win.w) {
                $(this).css("left", (win.w - (box.right - box.left)) + "px");
            }
        }

        if (!rightEdgeInRange) {
            if (box.right < 0) {
                $(this).css("left", (0) + "px")
            } else if (box.right > win.w) {
                $(this).css("left", (trigerbox.right - (box.right - box.left) - 5) + "px");
            }
        }
        // debugger;
        if (!bottomEdgeInScreen) {
            if (box.bottom > win.h) {
                $(this).css("top", (trigerbox.top - (box.bottom - box.top)) + "px");
            } else if (box.bottom > win.w) {
                $(this).css("top", (win.w - box.width) + "px");
            }
        }

        var bodyBoxTopup = bodyBox.top < 0 ? (bodyBox.top * -1) : 0;

        if (!topEdgeInScreen) {
            if (box.top < 0) {
                $(this).css("top", trigerbox.bottom + bodyBoxTopup + "px");
            } else if (box.top > win.h) {
                $(this).css("top", (win.h - (box.bottom - box.top)) + "px");
            }
        }

        //now knowing presence of each edge on screen, we check if element is partially or entirely present on screen
        var isPartiallyOnScreen = topEdgeInScreen || bottomEdgeInScreen || leftEdgeInScreen || rightEdgeInScreen;
        var isEntirelyOnScreen = topEdgeInScreen && bottomEdgeInScreen && leftEdgeInScreen && rightEdgeInScreen;

        return partial ? isPartiallyOnScreen : isEntirelyOnScreen;

    };
    neo.stringOptions = {
        "Is equal to": "==",
        "Is null or empty": "Isnullorempty",
        "Is not equal to": "!=",
        "Starts with": "Startswith",
        "Contains": "Contains",
        "Does not contain": "Doesnotcontain",
        "Ends with": "Endswith",
    };

    neo.numberOptions = {
        "Is equal to": "==",
        "Is not equal to": "!=",
        "Is less than": "<",
        "Is less than equal to": "<=",
        "Is greater than": ">",
        "Is greater than equal to": "<=",
    };

    neo.dateOptions = {
        "Is equal to": "==",
        "Is not equal to": "!=",
        "Is less than": "<",
        "Is less than equal to": "<=",
        "Is greater than": ">",
        "Is greater than equal to": "<=",
    };
    neo.SetFilterBox = function () {
        if (neo.FilterBox == undefined) {
            neo.FilterBox = $(".FilterBox");
            neo.FilterBoxControls = {
                selectFilterOptions1: neo.FilterBox.find("#selectFilterOptions1"),
                selectFilterOptions2: neo.FilterBox.find("#selectFilterOptions2"),
                rdoAnd: neo.FilterBox.find("#rdoAnd"),
                rdoOr: neo.FilterBox.find("#rdoOr"),
                filterBox1: neo.FilterBox.find("#filterBox1"),
                filterBox2: neo.FilterBox.find("#filterBox2")
            }
        }
    }
    neo.ModifyFilterBox = function (lobjGrid, field) {
        var type = lobjGrid.columnFields[field].type;
        neo.FilterBox.attr("data-field", field);
        neo.FilterBoxControls.selectFilterOptions1.empty();
        neo.FilterBoxControls.selectFilterOptions2.empty();

        var OptionsToTake = {};
        if (type == 'number') {
            OptionsToTake = neo.numberOptions;
        } else if (type == 'date') {
            OptionsToTake = neo.dateOptions;
        } else {
            OptionsToTake = neo.stringOptions;
        }

        for (var key in OptionsToTake) {
            var option = ["<option value='", OptionsToTake[key], "'>", key, "</option>"].join("");
            neo.FilterBoxControls.selectFilterOptions1.append(option);
            neo.FilterBoxControls.selectFilterOptions2.append(option);
        }

        var filterData = _.filter(lobjGrid.filterColumns, function (item) {
            return item.field == field;
        });

        if (filterData.length > 0) {
            neo.FilterBoxControls.selectFilterOptions1.val(filterData[0].selectFilterOptions1);
            neo.FilterBoxControls.selectFilterOptions2.val(filterData[0].selectFilterOptions2);
            // neo.FilterBox.find('input[name=FilterCondition][value="' + filterData[0].rdoAndOr + '"]').attr("checked", true);
            neo.FilterBox.find('input[name=FilterCondition]').val([filterData[0].rdoAndOr]);
            neo.FilterBoxControls.filterBox1.val(filterData[0].filterBox1);
            neo.FilterBoxControls.filterBox2.val(filterData[0].filterBox2);
        }
        else {
            neo.FilterBoxControls.selectFilterOptions1.val("==");
            neo.FilterBoxControls.selectFilterOptions2.val("==");
            neo.FilterBox.find('input[name=FilterCondition]').val([""]);
            neo.FilterBoxControls.filterBox1.val("");
            neo.FilterBoxControls.filterBox2.val("");
        }

        if (type == 'date') {
            NeoGrid.applyDate(neo.FilterBoxControls.filterBox1);
            NeoGrid.applyDate(neo.FilterBoxControls.filterBox2);
        } else {
            if (neo.FilterBoxControls.filterBox1.datepicker != undefined && neo.FilterBoxControls.filterBox1.datepicker().length > 0) {
                neo.FilterBoxControls.filterBox1.datepicker("destroy");
                neo.FilterBoxControls.filterBox1.unmask();
            }
            if (neo.FilterBoxControls.filterBox2.datepicker != undefined && neo.FilterBoxControls.filterBox2.datepicker().length > 0) {
                neo.FilterBoxControls.filterBox2.datepicker("destroy");
                neo.FilterBoxControls.filterBox2.unmask();
            }
        }
    }
    neo.templateHashRegExp = /#/ig;
    NeoGrid.registerEvents();
    $.fn.neoGrid = function (options) {
        // This is the easiest way to have default options.
        var settings = $.extend({
            // These are the defaults.
            columns: [],
        }, options);
        if (this.length == 1)
            new NeoGrid(this, settings);
        return this;
    };
}(jQuery));

(function ($) {
    $.fn.neoForceNumeric = function (options) {
        var settings = $.extend({
            'allowNegative': false,
            'allowDecimal': false,
            'decimalPlaces': 0
        }, options);

        return this.each(function () {
            $(this).keypress(function (e) {
                var key;
                var isCtrl = false;
                var keychar;
                var reg;

                if (window.event) {
                    key = e.keyCode;
                    isCtrl = window.event.ctrlKey;
                }
                else if (e.which) {
                    key = e.which;
                    isCtrl = e.ctrlKey;
                }

                if (isNaN(key)) return true;

                keychar = String.fromCharCode(key);

                // check for backspace or delete, or if Ctrl was pressed
                if (key == 8 || isCtrl) {
                    return true;
                }

                reg = /\d/;
                var isFirstNegative = settings.allowNegative ? keychar == '-' && this.value.indexOf('-') == -1 : false;
                var isFirstDecimal = settings.allowDecimal ? keychar == '.' && this.value.indexOf('.') == -1 : false;

                var cursorPosition = $(this).getCursorPosition();
                var currentValue = $(this).val();
                var currentValueLength = currentValue.length;

                //only allow negative character in the beginning
                if (settings.allowNegative && isFirstNegative) {
                    return cursorPosition == 0;
                }

                //if first decimal, make sure it is in the right decimal position, else move it automatically to the right decimal place
                if (settings.allowDecimal && isFirstDecimal && settings.decimalPlaces > 0) {
                    //first decimal and at the end of the value, it's ok
                    if (cursorPosition == currentValueLength || currentValueLength - cursorPosition <= settings.decimalPlaces) {
                        return true;
                    }

                    //else we have to move it automatically to the right decimal place
                    if (currentValueLength - cursorPosition != settings.decimalPlaces) {
                        var integerPart = currentValue.substring(0, currentValueLength - settings.decimalPlaces);
                        var fractionalPart = currentValue.substring(currentValueLength - settings.decimalPlaces, currentValueLength);
                        $(this).val(integerPart + "." + fractionalPart);
                        //return false to avoid system to print the . at the end
                        return false;
                    }
                }

                //if there's a decimal in place, make sure no more numeric allowed after specified decimal place
                if (settings.allowDecimal && currentValue.indexOf('.') > 0) {
                    //if decimal places was reached, cancel the input
                    if (cursorPosition > currentValue.indexOf('.') && currentValue.length - currentValue.indexOf('.') > settings.decimalPlaces) {
                        return false;
                    }
                }

                return isFirstNegative || isFirstDecimal || reg.test(keychar);
            });
            $(this).bind("paste drag drop", function (e) {
                var $this = $(this);
                var oldValue = $(this).val();
                setTimeout(function () {
                    if (!$.isNumeric($this.val())) {
                        $this.val(oldValue);
                    }
                }, 0); //just break the callstack to let the event finish 
            });
        });
    };

    $.fn.getCursorPosition = function () {
        var pos = 0;
        var input = $(this).get(0);
        // IE Support
        if (document.selection) {
            input.focus();
            var sel = document.selection.createRange();
            var selLen = document.selection.createRange().text.length;
            sel.moveStart('character', -input.value.length);
            pos = sel.text.length - selLen;
        }
            // Firefox support
        else if (input.selectionStart || input.selectionStart == '0')
            pos = input.selectionStart;

        return pos;
    };
})(jQuery);

/*Code related to saveAss of Export to Excel */

(function (view) {
    "use strict";

    view.URL = view.URL || view.webkitURL;

    if (view.Blob && view.URL) {
        try {
            new Blob;
            return;
        } catch (e) { }
    }

    // Internally we use a BlobBuilder implementation to base Blob off of
    // in order to support older browsers that only have BlobBuilder
    var BlobBuilder = view.BlobBuilder || view.WebKitBlobBuilder || view.MozBlobBuilder || (function (view) {
        var
          get_class = function (object) {
              return Object.prototype.toString.call(object).match(/^\[object\s(.*)\]$/)[1];
          },
          FakeBlobBuilder = function BlobBuilder() {
              this.data = [];
          },
          FakeBlob = function Blob(data, type, encoding) {
              this.data = data;
              this.size = data.length;
              this.type = type;
              this.encoding = encoding;
          },
          FBB_proto = FakeBlobBuilder.prototype,
          FB_proto = FakeBlob.prototype,
          FileReaderSync = view.FileReaderSync,
          FileException = function (type) {
              this.code = this[this.name = type];
          },
          file_ex_codes = (
            "NOT_FOUND_ERR SECURITY_ERR ABORT_ERR NOT_READABLE_ERR ENCODING_ERR " + "NO_MODIFICATION_ALLOWED_ERR INVALID_STATE_ERR SYNTAX_ERR"
          ).split(" "),
          file_ex_code = file_ex_codes.length,
          real_URL = view.URL || view.webkitURL || view,
          real_create_object_URL = real_URL.createObjectURL,
          real_revoke_object_URL = real_URL.revokeObjectURL,
          URL = real_URL,
          btoa = view.btoa,
          atob = view.atob

        , ArrayBuffer = view.ArrayBuffer, Uint8Array = view.Uint8Array;
        FakeBlob.fake = FB_proto.fake = true;
        while (file_ex_code--) {
            FileException.prototype[file_ex_codes[file_ex_code]] = file_ex_code + 1;
        }
        if (!real_URL.createObjectURL) {
            URL = view.URL = {};
        }
        URL.createObjectURL = function (blob) {
            var
              type = blob.type,
              data_URI_header;
            if (type === null) {
                type = "application/octet-stream";
            }
            if (blob instanceof FakeBlob) {
                data_URI_header = "data:" + type;
                if (blob.encoding === "base64") {
                    return data_URI_header + ";base64," + blob.data;
                } else if (blob.encoding === "URI") {
                    return data_URI_header + "," + decodeURIComponent(blob.data);
                }
                if (btoa) {
                    return data_URI_header + ";base64," + btoa(blob.data);
                } else {
                    return data_URI_header + "," + encodeURIComponent(blob.data);
                }
            } else if (real_create_object_URL) {
                return real_create_object_URL.call(real_URL, blob);
            }
        };
        URL.revokeObjectURL = function (object_URL) {
            if (object_URL.substring(0, 5) !== "data:" && real_revoke_object_URL) {
                real_revoke_object_URL.call(real_URL, object_URL);
            }
        };
        FBB_proto.append = function (data /*, endings*/) {
            var bb = this.data;
            // decode data to a binary string
            if (Uint8Array && (data instanceof ArrayBuffer || data instanceof Uint8Array)) {
                var
                  str = "",
                  buf = new Uint8Array(data),
                  i = 0,
                  buf_len = buf.length;
                for (; i < buf_len; i++) {
                    str += String.fromCharCode(buf[i]);
                }
                bb.push(str);
            } else if (get_class(data) === "Blob" || get_class(data) === "File") {
                if (FileReaderSync) {
                    var fr = new FileReaderSync;
                    bb.push(fr.readAsBinaryString(data));
                } else {
                    // async FileReader won't work as BlobBuilder is sync
                    throw new FileException("NOT_READABLE_ERR");
                }
            } else if (data instanceof FakeBlob) {
                if (data.encoding === "base64" && atob) {
                    bb.push(atob(data.data));
                } else if (data.encoding === "URI") {
                    bb.push(decodeURIComponent(data.data));
                } else if (data.encoding === "raw") {
                    bb.push(data.data);
                }
            } else {
                if (typeof data !== "string") {
                    data += ""; // convert unsupported types to strings
                }
                // decode UTF-16 to binary string
                bb.push(unescape(encodeURIComponent(data)));
            }
        };
        FBB_proto.getBlob = function (type) {
            if (!arguments.length) {
                type = null;
            }
            return new FakeBlob(this.data.join(""), type, "raw");
        };
        FBB_proto.toString = function () {
            return "[object BlobBuilder]";
        };
        FB_proto.slice = function (start, end, type) {
            var args = arguments.length;
            if (args < 3) {
                type = null;
            }
            return new FakeBlob(
              this.data.slice(start, args > 1 ? end : this.data.length), type, this.encoding
            );
        };
        FB_proto.toString = function () {
            return "[object Blob]";
        };
        FB_proto.close = function () {
            this.size = this.data.length = 0;
        };
        return FakeBlobBuilder;
    }(view));

    view.Blob = function Blob(blobParts, options) {
        var type = options ? (options.type || "") : "";
        var builder = new BlobBuilder();
        if (blobParts) {
            for (var i = 0, len = blobParts.length; i < len; i++) {
                builder.append(blobParts[i]);
            }
        }
        return builder.getBlob(type);
    };
}(typeof self !== "undefined" && self || typeof window !== "undefined" && window || this.content || this));
//var saveAs = saveAs
NeoGrid.saveAs = NeoGrid.saveAs
  // IE 10+ (native saveAs)
  || (typeof navigator !== "undefined" &&
    navigator.msSaveOrOpenBlob && navigator.msSaveOrOpenBlob.bind(navigator))
  // Everyone else
  || (function (view) {
      "use strict";
      // IE <10 is explicitly unsupported
      if (typeof navigator !== "undefined" &&
        /MSIE [1-9]\./.test(navigator.userAgent)) {
          return;
      }
      var
        doc = view.document
        // only get URL when necessary in case BlobBuilder.js hasn't overridden it yet
        ,
        get_URL = function () {
            return view.URL || view.webkitURL || view;
        },
        URL = view.URL || view.webkitURL || view,
        save_link = doc.createElementNS("http://www.w3.org/1999/xhtml", "a"),
        can_use_save_link = !view.externalHost && "download" in save_link,
        click = function (node) {
            var event = doc.createEvent("MouseEvents");
            event.initMouseEvent(
              "click", true, false, view, 0, 0, 0, 0, 0, false, false, false, false, 0, null
            );
            node.dispatchEvent(event);
        },
        webkit_req_fs = view.webkitRequestFileSystem,
        req_fs = view.requestFileSystem || webkit_req_fs || view.mozRequestFileSystem,
        throw_outside = function (ex) {
            (view.setImmediate || view.setTimeout)(function () {
                throw ex;
            }, 0);
        },
        force_saveable_type = "application/octet-stream",
        fs_min_size = 0,
        deletion_queue = [],
        process_deletion_queue = function () {
            var i = deletion_queue.length;
            while (i--) {
                var file = deletion_queue[i];
                if (typeof file === "string") { // file is an object URL
                    URL.revokeObjectURL(file);
                } else { // file is a File
                    file.remove();
                }
            }
            deletion_queue.length = 0; // clear queue
        },
        dispatch = function (filesaver, event_types, event) {
            event_types = [].concat(event_types);
            var i = event_types.length;
            while (i--) {
                var listener = filesaver["on" + event_types[i]];
                if (typeof listener === "function") {
                    try {
                        listener.call(filesaver, event || filesaver);
                    } catch (ex) {
                        throw_outside(ex);
                    }
                }
            }
        },
        FileSaver = function (blob, name) {
            // First try a.download, then web filesystem, then object URLs
            var
              filesaver = this,
              type = blob.type,
              blob_changed = false,
              object_url, target_view, get_object_url = function () {
                  var object_url = get_URL().createObjectURL(blob);
                  deletion_queue.push(object_url);
                  return object_url;
              },
              dispatch_all = function () {
                  dispatch(filesaver, "writestart progress write writeend".split(" "));
              }
              // on any filesys errors revert to saving with object URLs
              ,
              fs_error = function () {
                  // don't create more object URLs than needed
                  if (blob_changed || !object_url) {
                      object_url = get_object_url(blob);
                  }
                  if (target_view) {
                      target_view.location.href = object_url;
                  } else {
                      if (navigator.userAgent.match(/7\.[\d\s\.]+Safari/) // is Safari 7.x
                        && typeof window.FileReader !== "undefined" // can convert to base64
                        && blob.size <= 1024 * 1024 * 150 // file size max 150MB
                      ) {
                          var reader = new window.FileReader();
                          reader.readAsDataURL(blob);
                          reader.onloadend = function () {
                              var frame = doc.createElement("iframe");
                              frame.src = reader.result;
                              frame.style.display = "none";
                              doc.body.appendChild(frame);
                              dispatch_all();
                              return;
                          }
                          filesaver.readyState = filesaver.DONE;
                          filesaver.savedAs = filesaver.SAVEDASUNKNOWN;
                          return;
                      } else {
                          window.open(object_url, "_blank");
                          filesaver.readyState = filesaver.DONE;
                          filesaver.savedAs = filesaver.SAVEDASBLOB;
                          dispatch_all();
                          return;
                      }
                  }
              },
              abortable = function (func) {
                  return function () {
                      if (filesaver.readyState !== filesaver.DONE) {
                          return func.apply(this, arguments);
                      }
                  };
              },
              create_if_not_found = {
                  create: true,
                  exclusive: false
              },
              slice;
            filesaver.readyState = filesaver.INIT;
            if (!name) {
                name = "download";
            }
            if (can_use_save_link) {
                object_url = get_object_url(blob);
                // FF for Android has a nasty garbage collection mechanism
                // that turns all objects that are not pure javascript into 'deadObject'
                // this means `doc` and `save_link` are unusable and need to be recreated
                // `view` is usable though:
                doc = view.document;
                save_link = doc.createElementNS("http://www.w3.org/1999/xhtml", "a");
                save_link.href = object_url;
                save_link.download = name;
                var event = doc.createEvent("MouseEvents");
                event.initMouseEvent(
                  "click", true, false, view, 0, 0, 0, 0, 0, false, false, false, false, 0, null
                );
                save_link.dispatchEvent(event);
                filesaver.readyState = filesaver.DONE;
                filesaver.savedAs = filesaver.SAVEDASBLOB;
                dispatch_all();
                return;
            }
            // Object and web filesystem URLs have a problem saving in Google Chrome when
            // viewed in a tab, so I force save with application/octet-stream
            if (view.chrome && type && type !== force_saveable_type) {
                slice = blob.slice || blob.webkitSlice;
                blob = slice.call(blob, 0, blob.size, force_saveable_type);
                blob_changed = true;
            }
            // Since I can't be sure that the guessed media type will trigger a download
            // in WebKit, I append .download to the filename.
            if (webkit_req_fs && name !== "download") {
                name += ".download";
            }
            if (type === force_saveable_type || webkit_req_fs) {
                target_view = view;
            }
            if (!req_fs) {
                fs_error();
                return;
            }
            fs_min_size += blob.size;
            req_fs(view.TEMPORARY, fs_min_size, abortable(function (fs) {
                fs.root.getDirectory("saved", create_if_not_found, abortable(function (dir) {
                    var save = function () {
                        dir.getFile(name, create_if_not_found, abortable(function (file) {
                            file.createWriter(abortable(function (writer) {
                                writer.onwriteend = function (event) {
                                    target_view.location.href = file.toURL();
                                    deletion_queue.push(file);
                                    filesaver.readyState = filesaver.DONE;
                                    filesaver.savedAs = filesaver.SAVEDASBLOB;
                                    dispatch(filesaver, "writeend", event);
                                };
                                writer.onerror = function () {
                                    var error = writer.error;
                                    if (error.code !== error.ABORT_ERR) {
                                        fs_error();
                                    }
                                };
                                "writestart progress write abort".split(" ").forEach(function (event) {
                                    writer["on" + event] = filesaver["on" + event];
                                });
                                writer.write(blob);
                                filesaver.abort = function () {
                                    writer.abort();
                                    filesaver.readyState = filesaver.DONE;
                                    filesaver.savedAs = filesaver.FAILED;
                                };
                                filesaver.readyState = filesaver.WRITING;
                            }), fs_error);
                        }), fs_error);
                    };
                    dir.getFile(name, {
                        create: false
                    }, abortable(function (file) {
                        // delete file if it already exists
                        file.remove();
                        save();
                    }), abortable(function (ex) {
                        if (ex.code === ex.NOT_FOUND_ERR) {
                            save();
                        } else {
                            fs_error();
                        }
                    }));
                }), fs_error);
            }), fs_error);
        },
        FS_proto = FileSaver.prototype,
        saveAs = function (blob, name) {
            return new FileSaver(blob, name);
        };
      FS_proto.abort = function () {
          var filesaver = this;
          filesaver.readyState = filesaver.DONE;
          filesaver.savedAs = filesaver.FAILED;
          dispatch(filesaver, "abort");
      };
      FS_proto.readyState = FS_proto.INIT = 0;
      FS_proto.WRITING = 1;
      FS_proto.DONE = 2;
      FS_proto.FAILED = -1;
      FS_proto.SAVEDASBLOB = 1;
      FS_proto.SAVEDASURI = 2;
      FS_proto.SAVEDASUNKNOWN = 3;

      FS_proto.error =
        FS_proto.onwritestart =
        FS_proto.onprogress =
        FS_proto.onwrite =
        FS_proto.onabort =
        FS_proto.onerror =
        FS_proto.onwriteend =
        null;

      view.addEventListener("unload", process_deletion_queue, false);
      saveAs.unload = function () {
          process_deletion_queue();
          view.removeEventListener("unload", process_deletion_queue, false);
      };
      return saveAs;
  }(
    typeof self !== "undefined" && self || typeof window !== "undefined" && window || this.content
  ));
// `self` is undefined in Firefox for Android content script context
// while `this` is nsIContentFrameMessageManager
// with an attribute `content` that corresponds to the window

if (typeof module !== "undefined" && module !== null) {
    module.exports = NeoGrid.saveAs;
} else if ((typeof define !== "undefined" && define !== null) && (define.amd != null)) {
    define([], function () {
        return NeoGrid.saveAs;
    });
} else if (typeof Meteor !== 'undefined') { // make it available for Meteor
    Meteor.saveAs = NeoGrid.saveAs;
}

/*End Code related to saveAss of Export to Excel */


var EmployeeBalanceManager = (function () {

    function EmployeeBalanceManagerInner() {

        var Options = null;
        var self = this;
        this.Init = function (options) {
            Options = options;
            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);
        }
        var initGrid = function (options) {

            var gridId = options["grid_id"];
            var height = options["grid_height"];
            var captions = options["grid_captions"];
            var noDataText = options["no_data"];
            var pageInfo = options["page_info"];
            var url = options["url"];
            return {
                dataSource: CommonManager.Instance.GetGridDataSource(url, gridId),
                columns: [
                    {
                        dataField: "FullName", caption: captions["fullName"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            return row.FullName;
                        }
                    },
                    {
                        dataField: "", caption: "", width: 100,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-success' href='" + options["add_url"] + "?sellerID=" + id + "' >" + options["add_text"] + "</a>");
                        },
                        visible: options["view_add"]
                    },
                    {
                        dataField: "", caption: "", width: 190,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-primary' href='" + options["details_url"] + "?id=" + id + "' >" + options["details_text"] + "</a>");
                        },
                        visible: options["view_details"]
                    }
                ],
                showBorders: true,
                scrolling: {
                    useNative: false,
                    scrollByContent: true,
                    scrollByThumb: true,
                    showScrollbar: "always"
                },
                rtlEnabled: true,
                pager: {
                    showPageSizeSelector: true,
                    allowedPageSizes: [5, 10, 20],
                    showInfo: true,
                    infoText: pageInfo,
                },
                rowAlternationEnabled: true,
                hoverStateEnabled: true,
                remoteOperations: {
                    paging: true,
                    filtering: true,
                    sorting: true
                },
                noDataText: noDataText,
                height: height,
                filterRow: {
                    visible: false,
                    applyFilter: "auto",
                    showAllText: options["all_text"],
                },
                headerFilter: {
                    visible: false
                }
            }
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new EmployeeBalanceManagerInner();
            return instance;
        })()
    }
})();
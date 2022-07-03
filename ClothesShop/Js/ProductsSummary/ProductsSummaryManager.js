var ProductsSummaryManager = (function () {

    function ProductsSummarysManagerInner() {

        var Options = null;
        var self = this;
        this.Init = function (options) {
            Options = options;
            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);
        }

        this.OnDeleteRow = function (id) {
            var obj = {
                Message: Options["confirm_delete"],
                Title: Options["confirm_title"],
                Yes: Options["yes_text"],
                No: Options["no_text"],
                Url: Options["delete_url"] + "?id=" + id,
                Type: "POST",
                Success: Options["success_delete"],
                Error: Options["error_delete"]
            };

            CommonManager.Instance.Confirm(obj, function (result) {
                if (result) {
                    $(Options["grid_id"]).dxDataGrid("instance").refresh();
                }
            });
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
                        dataField: "ProductName", caption: captions["productName"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "TotalIncoming", caption: captions["totalIncoming"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                    },
                    {
                        dataField: "TotalReturned", caption: captions["totalReturned"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                    },
                    {
                        dataField: "TotalSelled", caption: captions["totalSelled"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                    },
                    {
                        dataField: "TotalRemaining", caption: captions["totalRemaining"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                    },
                    {
                        dataField: "OriginalPrice", caption: captions["originalPrice"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                    },
                    {
                        dataField: "TotalProductPrice", caption: captions["totalProductPrice"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                    }
                ],
                summary: {
                    totalItems: [{
                        column: "TotalProductPrice",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} " + captions["currency"]
                    },
                    {
                        column: "TotalRemaining",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        column: "TotalSelled",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        column: "TotalReturned",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        column: "TotalIncoming",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    }
                    ]
                },
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
                    visible: true,
                    applyFilter: "auto",
                    showAllText: options["all_text"],
                }
            }
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new ProductsSummarysManagerInner();
            return instance;
        })()
    }
})();
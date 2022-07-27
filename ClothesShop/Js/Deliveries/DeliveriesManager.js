var DeliveriesManager = (function () {

    function DeliveriesManagerInner() {

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
                        dataField: "EmployeeName", caption: captions["employee_name"], minWidth: 120,
                        alignment: "right",
                        allowFiltering: true,
                        allowSorting: false,
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "EmployeeMobileNumber", caption: captions["employeeMobileNumber"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "NumberOfOrders", caption: captions["numberOfOrders"],
                        allowSorting: true,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        minWidth: 80,
                        alignment: "right",
                        dataType: "string", // date
                    },
                    {
                        dataField: "TotalOrderCash",
                        caption: captions["totalOrderCash"],
                        minWidth: 80,
                        alignment: "right",
                        allowFiltering: false,
                        allowSorting: true,
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "", caption: "", width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            
                            var id = row.data.EmployeeID;
                            content.append("<a class='btn btn-sm btn-info' href='" + options["add_url"] + "?id=" + id + "' >" + options["add_text"] + "</a>");
                        },
                        visible: options["view_add"]
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.EmployeeID;
                            content.append("<a class='btn btn-sm btn-primary' href='" + options["update_url"] + "?id=" + id + "' >" + options["update_text"] + "</a>");
                        },
                        visible: options["view_update"]
                    },
                    {
                        dataField: "", caption: "", width: 85,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {

                            var id = row.data.EmployeeID;
                            content.append("<a class='btn btn-sm btn-secondary' onclick='DeliveriesManager.Instance.Print(" + id +")'>" + options["print_text"] + "</a>");
                        },
                        visible: options["view_details"]
                    },
                    {
                        dataField: "", caption: "", width: 110,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {

                            var id = row.data.EmployeeID;
                            content.append("<a class='btn btn-sm btn-secondary' onclick='DeliveriesManager.Instance.Export("+id+")'>" + options["export_text"] + "</a>");
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
                    visible: true,
                    applyFilter: "auto",
                    showAllText: options["all_text"],
                },
                onCellPrepared: function (e) {
                    if (e.rowType == "data" && e.data.NumberOfOrders == 0 && (e.columnIndex == 6 || e.columnIndex == 7))
                        e.cellElement.empty();
                },
            }
        }

        this.Export = function (id) {
            CommonManager.Instance.Export(Options["export_url"] + "?id=" + id);
        }

        this.Print = function (id) {
            CommonManager.Instance.print(Options["print_url"] + "?id=" + id);
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new DeliveriesManagerInner();
            return instance;
        })()
    }
})();
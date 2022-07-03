var TodaySalesManager = (function () {

    function TodaySalesManagerInner() {

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
            var detailsUrl = options["details_url"];
            var height = options["grid_height"];
            var captions = options["grid_captions"];
            var noDataText = options["no_data"];
            var pageInfo = options["page_info"];
            var url = options["url"];
            return {
                dataSource: CommonManager.Instance.GetGridDataSource(url, gridId),
                columns: [
                    {
                        dataField: "CreatedOn", caption: captions["createdOn"], minWidth: 150,
                        allowSorting: true,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: ["="],
                        alignment: "right",
                        cssClass: "dir-ltr",
                        dataType: "date", // date
                        calculateDisplayValue: function (row) {
                            var date = row.CreatedOn;
                            date = CommonManager.Instance.ParseDate(date, DateFormat.DayMonthYearHourMin);
                            return date;
                        }
                    },
                    {
                        dataField: "TodayTotalTransactionsSellingPrice",
                        caption: captions["todayTotalTransactionsSellingPrice"],
                        minWidth: 145,
                        alignment: "right",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "", caption: "", width: 100,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {

                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-info' href='" + options["details_url"] + "?id=" + id + "' >" + options["details_text"] + "</a>");
                        },
                        visible: options["view_details"]
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-primary' href='" + options["update_url"] + "?id=" + id + "' >" + options["update_text"] + "</a>");
                        },
                        visible: options["view_update"]
                    },
                    {
                        dataField: "", caption: "", width: 70,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {

                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='TodaySalesManager.Instance.OnDeleteRow(" + id + ")'>" + options["delete_text"] + "</a>");
                        },
                        visible: options["view_delete"]
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
                    if (e.rowType == "data" && e.data.IsApproved == true && (e.columnIndex == 3 || e.columnIndex == 4))
                        e.cellElement.empty();
                },
                headerFilter: {
                    visible: true
                }
            }
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new TodaySalesManagerInner();
            return instance;
        })()
    }
})();
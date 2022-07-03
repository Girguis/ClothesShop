var ColorsManager = (function () {

    function ColorsManagerInner() {

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
                        dataField: "Name", caption: captions["name"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            return row.Name;
                        }
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
                        dataField: "", caption: "", width: 80,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {

                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='ColorsManager.Instance.OnDeleteRow(" + id + ")'>" + options["delete_text"] + "</a>");
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
                    if (e.rowType == "data" && e.data.OrderStatusID == 3 && (e.columnIndex == 13 || e.columnIndex == 11))
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
                instance = new ColorsManagerInner();
            return instance;
        })()
    }
})();
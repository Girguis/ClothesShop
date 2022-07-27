var ProductSuppliersManager = (function () {

    function ProductSuppliersManagerInner() {

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
            let transistionType = CommonManager.Instance.NeowtonsoftDesealize(options["transistion_type"]);
            var url = options["url"];
            return {
                dataSource: CommonManager.Instance.GetGridDataSource(url, gridId),
                columns: [
                    {
                        dataField: "CreatedOn_", caption: captions["createdOn"], minWidth: 150,
                        allowSorting: true,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: ["="],
                        alignment: "right",
                        cssClass: "dir-ltr",
                        dataType: "date", // date
                    },
                    {
                        dataField: "ProductName", caption: captions["productName"], minWidth: 120,
                        allowSorting: true,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "SupplierName", caption: captions["supplierName"], minWidth: 120,
                        allowSorting: true,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "TransactionName", caption: captions["transactionName"], width: 120,
                        allowSorting: true,
                        allowFiltering: false,
                        allowHeaderFiltering: true,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            var status = transistionType.find(function (e) { return e.ID == row.TransactionTypeID });
                            return status ? status.Name : "";
                        },
                        headerFilter: {
                            dataSource: CommonManager.Instance.EnumToDataSourceHeaderFilter("TransactionTypeID", "=", transistionType, "ID", "Name"),
                        }
                    },
                    {
                        dataField: "NumberOfPieces", caption: captions["numberOfPieces"], minWidth: 120,
                        alignment: "right",
                        allowFiltering: true,
                        allowSorting: true,
                        allowHeaderFiltering: false
                    },
                    {
                        dataField: "OrginalPrice", caption: captions["orginalPrice"], minWidth: 120,
                        alignment: "right",
                        allowFiltering: true,
                        allowSorting: true,
                        allowHeaderFiltering: false
                    },
                    {
                        dataField: "CreatedBy", caption: captions["createdBy"], minWidth: 120,
                        alignment: "right",
                        allowFiltering: true,
                        allowSorting: true,
                        allowHeaderFiltering: false
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: true,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-primary' href='" + options["update_url"] + "?id=" + id + "' >" + options["update_text"] + "</a>");
                        },
                        visible: options["view_update"]
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: true, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='ProductSuppliersManager.Instance.OnDeleteRow(" + id + ")'>" + options["delete_text"] + "</a>");
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
                instance = new ProductSuppliersManagerInner();
            return instance;
        })()
    }
})();
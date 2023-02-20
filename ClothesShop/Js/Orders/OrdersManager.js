var OrdersManager = (function () {

    function OrdersManagerInner() {
        
        var Options = null;
        var self = this;
        this.Init = function (options) {
            Options = options;
            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);

            $(options["print_multiple_btn"]).unbind("click").bind("click", function () {
                var selectedRow = $(options["grid_id"]).dxDataGrid("instance").getSelectedRowsData().map(function (x) { return x.ID });

                if (!selectedRow || selectedRow.length < 1) {
                    CommonManager.Instance.ShowNotification(options["not_valid_msg"], NotificationType.error);
                    return false;
                }
                else {
                    var ids = selectedRow.join(",");
                    CommonManager.Instance.print(options["print_multiple_url"] + "?ids=" + ids + "&PageID=" + options["pageID"]);

                    return true;
                }
            });
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
            let orderStatuses = CommonManager.Instance.NeowtonsoftDesealize(options["order_statuses"]);
            var url = options["url"];
            debugger;
            return {
                dataSource: CommonManager.Instance.GetGridDataSource(url, gridId),
                columns: [
                    {
                        dataField: "ID", caption: captions["id"],
                        allowSorting: true,
                        allowFiltering: true,
                        filterOperations: FilterLst,
                        allowHeaderFiltering: false,
                        sortOrder: "desc",
                        width: 100,
                        alignment: "right",
                        dataType: "string", // date
                        calculateDisplayValue: function (row) {
                            return row.ID;
                        }
                    },
                    {
                        dataField: "RequestDate_", caption: captions["request_date"], minWidth: 150,
                        allowSorting: true,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: ["="],
                        alignment: "right",
                        cssClass: "dir-ltr",
                        dataType: "date", // date
                        
                    },
                    {
                        dataField: "Customer_Name", caption: captions["customer_name"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            return row.CustomerName;
                        }
                    },
                    {
                        dataField: "Customer_MobileNumber1", caption: captions["mobile_number_1"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            return row.CustomerNumber;
                        },
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "Customer_Address", caption: captions["address"], minWidth: 120,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: true, allowSorting: false, allowHeaderFiltering: false,
                        calculateDisplayValue: function (row) {
                            var address = "";
                            if (!row.CustomerAddress)
                                return "";
                            address = row.CustomerAddress.split("-");
                            if (address.length > 1)
                                address = address[address.length - 1];
                            else
                                address = address[0];
                            return address;
                        }
                    },
                    {
                        dataField: "Total", caption: captions["total"], minWidth: 90,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "ShipmentPrice", caption: captions["shipment_price"], minWidth: 90,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "OrderStatusID", caption: captions["order_status"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: true,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            var status = orderStatuses.find(function (e) { return e.ID == row.OrderStatusID });
                            return status ? status.Name : "";
                        },
                        headerFilter: {
                            dataSource: CommonManager.Instance.EnumToDataSourceHeaderFilter("OrderStatusID", "=", orderStatuses, "ID", "Name"),
                        }
                    },
                    {
                        dataField: "EmployeeName", caption: captions["employee_name"], minWidth: 120,
                        alignment: "right",
                        dataType: "string",
                        filterOperations: FilterLst,
                        allowFiltering: true, allowSorting: false, allowHeaderFiltering: false,
                        calculateDisplayValue: function (row) {
                            return row.DeliveryManName;
                        },
                    },
                    {
                        dataField: "SellerName", caption: captions["seller_name"], minWidth: 120,
                        alignment: "right",
                        dataType: "string",
                        filterOperations: FilterLst,
                        allowFiltering: true, allowSorting: false, allowHeaderFiltering: false,
                        calculateDisplayValue: function (row) {
                            return row.SellerName;
                        }
                    },
                    {
                        dataField: "", caption: "", width: 95,
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
                        dataField: "", caption: "", width: 85,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                           
                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-secondary' onclick = 'OrdersManager.Instance.print(" + id + ")' >" + options["print_text"] + "</a>");
                        },
                        visible: options["view_print"]
                    },
                    {
                        dataField: "", caption: "", width: 70,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {

                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='OrdersManager.Instance.OnDeleteRow(" + id + ")'>" + options["delete_text"] + "</a>");
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
                selection: {
                    mode: "multiple",
                    selectAllMode: "page",
                    showCheckBoxesMode:"always"
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
                    if (e.rowType == "data" && e.data.OrderStatusID == 3 &&( e.columnIndex == 13 || e.columnIndex == 11))
                        e.cellElement.empty();
                },
                headerFilter: {
                    visible: true
                }
            }
        }
        this.print = function (id) {
            CommonManager.Instance.print(Options["print_url"] + "?id=" + id + "&PageID=" + Options["pageID"]);
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new OrdersManagerInner();
            return instance;
        })()
    }
})();
var DeliveriesInfoManager = (function () {

    function DeliveriesInfoManagerInner() {
        var self = this;
        var Options;
        var OrderInfo = null;
        var OrdersInfo = null;
        this.Init = function (options) {
            Options = options;
            var lst = [];
            if (options["url"] == "")
                lst = CommonManager.Instance.NeowtonsoftDesealize(options["lst"]);
            else
                lst = CommonManager.Instance.GetGridDataSource(options["url"], options["gridId"]);
            options["lst"] = lst;

            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);
            self.InitChangeOrdersPopUp(options);
            self.InitChangeOrderPopUp(options);

            if (options["is_create_view"]) {
                $(options["save_btn"]).unbind("click").bind("click", function () {
                    var selectedRow = $(options["grid_id"]).dxDataGrid("instance").getSelectedRowsData().map(function (x) { return x.ID });
                    if (!selectedRow || selectedRow.length < 1) {
                        CommonManager.Instance.ShowNotification(options["no_added_orders_msg"], NotificationType.error);
                        return false;
                    }
                    else {
                        var data = { employeeId: options["employee_id"], orderIds: selectedRow }
                        CommonManager.Instance.SendRequest(options["create_url"], "POST", JSON.stringify(data),
                            function (result) {
                                if (result)
                                    window.location = options["index_url"];
                                else
                                    CommonManager.Instance.ShowNotification(options["error_occured"], NotificationType.error);
                            });
                        return true;
                    }
                });
            }
            else
            {
                $(options["change_orders_status"]).unbind("click").bind("click", function () {      
                    var selectedRow = $(options["grid_id"]).dxDataGrid("instance").getSelectedRowsData().map(function (x) { return x.ID });
                    if (!selectedRow || selectedRow.length < 1) {
                        CommonManager.Instance.ShowNotification(options["no_added_orders_msg"], NotificationType.error);
                        return false;
                    }
                    else {
                        OrdersInfo = null;
                        OrdersInfo = {
                            orderIds: selectedRow,
                        };
                        $(options["change_orders_popup_id"]).dxPopup("instance").show();
                    }
                });
            }
        }



        var initGrid = function (options) {
            var gridId = options["grid_id"];
            var height = options["grid_height"];
            var captions = options["grid_captions"];
            var noDataText = options["no_data"];
            var pageInfo = options["page_info"];
            var url = options["url"];
            return {
                dataSource: options["lst"],
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
                        dataField: "Customer.Name", caption: captions["customer_name"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            return row.Customer.Name;
                        }
                    },
                    {
                        dataField: "Customer.Address", caption: captions["address"], minWidth: 120,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: true, allowSorting: false, allowHeaderFiltering: false,
                        calculateDisplayValue: function (row) {
                            var address = "";
                            if (!row.Customer.Address)
                                return "";
                            address = row.Customer.Address.split("-");
                            if (address.length > 1)
                                address = address[address.length - 1];
                            else
                                address = address[0];
                            return address;
                        }
                    },
                    {
                        dataField: "Customer.MobileNumber1", caption: captions["mobile_number"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "OrderTotalPrice", caption: captions["total"], minWidth: 120,
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
                        dataField: "PageID", caption: "", minWidth: 90,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                        calculateDisplayValue: function (row) {
                            if (row.PageID == "137D0514-F286-48AA-BCD4-B7FE7C5B79D8")
                                return "EY Store";
                            else
                                return "Wolf Store";
                        }
                    },
                    {
                        dataField: "SellerName", caption: captions["seller_name"], minWidth: 90,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: true, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "OrderStatusName", caption: captions["order_status"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "", caption: "", width: 115,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.ID;
                            var statusId = row.data.OrderStatusID;
                            content.append("<a class='btn btn-sm btn-secondary' onclick = 'DeliveriesInfoManager.Instance.ChangeOrderStatus(" + id + "," + statusId + ")' >" + options["edit_order_status"] + "</a>");
                        },
                        visible: !options["is_create_view"]
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var id = row.data.ID;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='DeliveriesInfoManager.Instance.OnDeleteRow(" + id + ")'>" + options["delete_text"] + "</a>");
                        },
                        visible: !options["is_create_view"]
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
                    showCheckBoxesMode: "always"
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
        this.OnDeleteRow = function (id) {
            var obj = {
                Message: Options["confirm_delete"],
                Title: Options["confirm_title"],
                Yes: Options["yes_text"],
                No: Options["no_text"],
                Url: Options["change_order_url"] + "?orderId=" + id +"&orderStatusId=0",
                Type: "POST",
                Success: Options["success_delete"],
                Error: Options["error_delete"]
            };

            CommonManager.Instance.Confirm(obj, function (result) {
                if (result)
                    window.location.reload();
            });
        }

        this.InitChangeOrdersPopUp = function (options) {
            
            $(options["change_orders_popup_id"]).dxPopup({
                contentTemplate: '<p>' + options["order_status_text"] + '</p><div id ="OrderStatus"></div>',
                width: 300,
                height: 280,
                rtlEnabled: true,
                container: '.content',
                showTitle: true,
                title: options["order_status_text"],
                visible: false,
                dragEnabled: false,
                hideOnOutsideClick: true,
                showCloseButton: false,
                position: {
                    at: 'center',
                    my: 'center',
                },
                onContentReady: function (e)
                {
                    var orders = CommonManager.Instance.NeowtonsoftDesealize(options["order_statuses"]);
                    
                    CommonManager.Instance.InitDefaultSelectList("#OrderStatus", orders, options["place_holder"], options["no_data"]);
                },
                toolbarItems: [{
                    widget: 'dxButton',
                    toolbar: 'bottom',
                    location: 'before',
                    options: {
                        icon: '',
                        text: options["save_text"],
                        onClick() {
                            
                            var orderStatus_id = $("#OrderStatus").dxSelectBox("instance").option("value");
                            OrdersInfo.orderStatusId = orderStatus_id;
                            CommonManager.Instance.SendRequest(options["change_orders_url"], "POST", JSON.stringify(OrdersInfo), function (result) {
                                if (result) {
                                    CommonManager.Instance.ShowNotification(options["saved_text"], NotificationType.success);
                                    $(options["change_orders_popup_id"]).dxPopup("instance").hide();
                                    window.location.reload();
                                }
                                else
                                    CommonManager.Instance.ShowNotification(options["error_occured"], NotificationType.error);
                            });
                        },
                    },
                }, {
                    widget: 'dxButton',
                    toolbar: 'bottom',
                    location: 'after',
                    options: {
                        text: options["close_popup_btn"],
                        onClick() {
                            $(options["change_orders_popup_id"]).dxPopup("instance").hide();
                        },
                    },
                }],
            });
        }

        this.ChangeOrderStatus = function (id, statusId) {
            OrderInfo = null;
            OrderInfo = {
                orderId: id,
                orderStatusId: statusId
            };
            $(Options["change_order_popup_id"]).dxPopup("instance").show();
        }

        this.InitChangeOrderPopUp = function (options) {
            $(options["change_order_popup_id"]).dxPopup({
                contentTemplate: '<p>' + options["order_status_text"] + '</p><div id ="OrderStatus"></div>' +
                    '<div id="paidAmountDiv" class="mt-2">' + options["grid_captions"]["paid_amount"] + '<input type="number" id="PaidAmount" class="form-control"/><div>',
                width: 300,
                height: 280,
                rtlEnabled: true,
                container: '.content',
                showTitle: true,
                title: options["order_status_text"],
                visible: false,
                dragEnabled: false,
                hideOnOutsideClick: true,
                showCloseButton: false,
                position: {
                    at: 'center',
                    my: 'center',
                },
                onContentReady: function (e) {
                    var orders = CommonManager.Instance.NeowtonsoftDesealize(options["order_statuses"]);
                    CommonManager.Instance.InitDefaultSelectList("#OrderStatus", orders, options["place_holder"], options["no_data"], DeliveriesInfoManager.Instance.OnChangeOrderStatus);
                    if (OrderInfo) {
                        $("#OrderStatus").dxSelectBox("instance").option("value", OrderInfo.orderStatusId);
                    }
                },
                toolbarItems: [{
                    widget: 'dxButton',
                    toolbar: 'bottom',
                    location: 'before',
                    options: {
                        icon: '',
                        text: options["save_text"],
                        onClick() {
                            var orderStatus_id = $("#OrderStatus").dxSelectBox("instance").option("value");
                            var paidAmount = parseFloat($("#PaidAmount").val());
                            OrderInfo.orderStatusId = orderStatus_id;
                            OrderInfo.paidAmount = paidAmount;
                            CommonManager.Instance.SendRequest(options["change_order_url"], "POST", JSON.stringify(OrderInfo), function (result) {
                                if (result) {
                                    CommonManager.Instance.ShowNotification(options["saved_text"], NotificationType.success);
                                    $(options["change_order_popup_id"]).dxPopup("instance").hide();
                                    window.location.reload();
                                }
                                else
                                    CommonManager.Instance.ShowNotification(options["error_occured"], NotificationType.error);
                            });
                        },
                    },
                }, {
                    widget: 'dxButton',
                    toolbar: 'bottom',
                    location: 'after',
                    options: {
                        text: options["close_popup_btn"],
                        onClick() {
                            $(options["change_order_popup_id"]).dxPopup("instance").hide();
                        },
                    },
                }],
            });
        }
        this.OnChangeOrderStatus = function (orderStatus) {
            
            if (orderStatus == 2 || orderStatus == 3)
                $("#paidAmountDiv").show();
            else
                $("#paidAmountDiv").hide();
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new DeliveriesInfoManagerInner();
            return instance;
        })()
    }
})();
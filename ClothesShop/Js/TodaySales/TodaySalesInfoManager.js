var TodaySalesInfoManager = (function () {

    function TodaySalesInfoManagerInner() {
        var self = this;
        var Options = null;
        this.Init = function (options)
        {
            Options = options;
            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);

            $(options["sellingPrice_id"]).dxNumberBox({
                value: 1,
                min: 0,
                showSpinButtons: true,
            });

            $(options["numberOfPieces_id"]).dxNumberBox({
                value: 1,
                min: 0,
                showSpinButtons: true,
            });
            $(options["add_id"]).unbind("click").bind("click", function () {

                var productId = $(options["product_id"]).dxSelectBox("instance").option("value");
                var productName = $(options["product_id"]).dxSelectBox("instance").option("text");
                var sellingPrice = $(options["sellingPrice_id"]).dxNumberBox("instance").option("value");
                var numberOfPieces = $(options["numberOfPieces_id"]).dxNumberBox("instance").option("value");
                if (!productId)
                    return;

                var obj = {
                    ID: 0,
                    ProductID: productId,
                    ProductName: productName,
                    SellingPrice: sellingPrice,
                    NumberOfPieces: numberOfPieces,
                    Guid: CommonManager.Instance.GenerateGuid()
                };
                $(options["grid_id"]).dxDataGrid("instance").option("dataSource").push(obj);
                $(options["grid_id"]).dxDataGrid("instance").refresh();
            });
            $(options["form_id"]).unbind("submit").bind("submit", function () {

                var jsonObj = $(options["grid_id"]).dxDataGrid("instance").option("dataSource");
                if (!jsonObj || jsonObj.length < 1) {
                    CommonManager.Instance.ShowNotification(options["must_sales_msg"], NotificationType.error)
                    return false;
                }
                else {
                    jsonObj = CommonManager.Instance.DeleteGuidsFromLst(jsonObj);
                    jsonObj = JSON.stringify(jsonObj);
                    $(options["todaySalesSeralized_id"]).val(jsonObj);
                    return true;
                }

            });


        }

        this.OnDeleteRow = function (guid) {
            var gridId = Options["grid_id"];
            var ds = $(gridId).dxDataGrid("instance").option("dataSource");
            var rows = ds.filter(function (e) { return e.Guid != guid; });
            $(gridId).dxDataGrid("instance").option("dataSource", rows);
            $(gridId).dxDataGrid("instance").refresh();
        }

        var initGrid = function (options) {

            var captions = options["grid_captions"];
            var noDataText = options["no_data"];
            var pageInfo = options["page_info"];
            var salesList = options["lst"];
            salesList = CommonManager.Instance.GenerateGuidsInLst(salesList);
            return {
                dataSource: salesList,
                columns: [
                    {
                        dataField: "ProductName",
                        caption: captions["product"],
                        minWidth: 120,
                        alignment: "right",
                        allowFiltering: false,
                        allowSorting: false,
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "NumberOfPieces",
                        caption: captions["numberOfPieces"],
                        minWidth: 120,
                        alignment: "right",
                        allowFiltering: false,
                        allowSorting: false,
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "SellingPrice",
                        caption: captions["sellingPrice"],
                        minWidth: 120,
                        alignment: "right",
                        allowFiltering: false,
                        allowSorting: false,
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "Total",
                        caption: captions["total"],
                        minWidth: 120,
                        alignment: "right",
                        visible: options["is_details"],
                        allowFiltering: false,
                        allowSorting: false,
                        allowHeaderFiltering: false,
                        calculateDisplayValue: function (row) {
                            return row.SellingPrice * row.NumberOfPieces + " " + options["currency"];
                        }
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var guid = row.data.Guid;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='TodaySalesInfoManager.Instance.OnDeleteRow(" + "\"" + guid + "\"" + ")'>" + captions["delete"] + "</a>");
                        },
                        visible: !options["is_details"]
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
            }
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new TodaySalesInfoManagerInner();
            return instance;
        })()
    }
})();
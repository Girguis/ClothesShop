var OrderInfoManager = (function () {

    function OrderInfoManagerInner() {
        var self = this;
        var Options = null;
        this.Init = function (options) {
            Options = options;
            var selectText = options["select"];
            var noData = options["no_data"];
            var isDetailsView = options["products"]["is_details_view"];
            if (!isDetailsView) {
                var city = options["city"];
                CommonManager.Instance.InitSelectList(city["id"], city["div_id"], city["lst"], selectText, noData);

                var orderstatus = options["order_status"];
                CommonManager.Instance.InitSelectList(orderstatus["id"], orderstatus["div_id"], orderstatus["lst"], selectText, noData);

                var employee = options["employee"];
                CommonManager.Instance.InitSelectList(employee["id"], employee["div_id"], employee["lst"], selectText, noData);

                var seller = options["seller"];
                CommonManager.Instance.InitSelectList(seller["id"], seller["div_id"], seller["lst"], selectText, noData);

                var shipment = options["shipment_company"];
                CommonManager.Instance.InitSelectList(shipment["id"], shipment["div_id"], shipment["lst"], selectText, noData);

                var delivery_Date = options["delivery_Date"];
                CommonManager.Instance.InitDateBox(delivery_Date["id"], delivery_Date["div_id"], delivery_Date["message"], noData);
                initProducts(options);
                var isAdd = options["isAdd"];
                if (isAdd)
                    $(city["div_id"]).dxSelectBox("instance").option("value", 1);

            }
            else {
                var productsOptions = options["products"];
                var lst = [];
                var json = productsOptions["product_seralized"];
                if (json)
                    lst = JSON.parse(json);
                initProductsGrid(productsOptions["grid_id"], productsOptions["grid_captions"], lst, noData, isDetailsView);
            }
        }

        var initProducts = function (allOptions) {
            var options = allOptions["products"];
            var placeholder = allOptions["select"];
            var noData = allOptions["no_data"];
            var captions = options["captions"];
            var json = $(options["product_seralized_id"]).val();
            var lst = [];
            if (json)
                lst = JSON.parse(json);

            lst = CommonManager.Instance.GenerateGuidsInLst(lst);

            CommonManager.Instance.InitDefaultSelectList(options["color_id"], [], placeholder, noData);
            CommonManager.Instance.InitDefaultSelectList(options["size_id"], [], placeholder, noData);

            $(options["num_pieces_id"]).dxNumberBox({
                value: 1,
                min: 0,
                showSpinButtons: true,
            });

            $(options["selling_price_id"]).dxNumberBox({
                value: 0,
                min: 0,
                showSpinButtons: true,
            });

            $(options["product_id"]).dxSelectBox({
                dataSource: new DevExpress.data.ArrayStore({
                    data: options["lst"],
                    key: "ID",
                }),
                value: null,
                searchEnabled: true,
                rtlEnabled: true,
                displayExpr: "Name",
                valueExpr: "ID",
                onValueChanged: function (e) {
                    var id = e.value;

                    CommonManager.Instance.SendRequest(options["info_url"] + "?productId=" + id, "POST", null,
                        function (result) {
                            var colors = [];
                            var sizes = [];
                            if (result && result.Colors)
                                colors = result.Colors;

                            if (result && result.Sizes)
                                sizes = result.Sizes;

                            CommonManager.Instance.InitDefaultSelectList(options["color_id"], colors, placeholder, noData);
                            CommonManager.Instance.InitDefaultSelectList(options["size_id"], sizes, placeholder, noData);

                        });
                },
            });

            $(options["product_id"]).dxSelectBox("instance").option("value", options["lst"][0].ID);

            initProductsGrid(options["grid_id"], captions, lst, noData, options["is_details_view"]);

            $(options["add_id"]).unbind("click").bind("click", function () {
              
                var productId = $(options["product_id"]).dxSelectBox("instance").option("value");
                var productName = $(options["product_id"]).dxSelectBox("instance").option("text");

                var colorId = $(options["color_id"]).dxSelectBox("instance").option("value");
                var colorName = $(options["color_id"]).dxSelectBox("instance").option("text");

                var sizeId = $(options["size_id"]).dxSelectBox("instance").option("value");
                var sizeName = $(options["size_id"]).dxSelectBox("instance").option("text");

                var price = $(options["selling_price_id"]).dxNumberBox("instance").option("value");
                var quantity = $(options["num_pieces_id"]).dxNumberBox("instance").option("value");

                if (!productId || !sizeId || !colorId)
                    return;

                var obj = {
                    ID: 0,
                    ProductID: productId,
                    Name: productName,
                    NumberOfPieces: quantity,
                    OriginalPrice: 0.0,
                    SellingPrice: price,
                    ColorID: colorId,
                    SizeID: sizeId,
                    ColorName: colorName,
                    SizeName: sizeName,
                    Guid: CommonManager.Instance.GenerateGuid()
                };
                $(options["grid_id"]).dxDataGrid("instance").option("dataSource").push(obj);
                $(options["grid_id"]).dxDataGrid("instance").refresh();
            });

            $(options["form_id"]).unbind("submit").bind("submit", function () {
               
                var jsonObj = $(options["grid_id"]).dxDataGrid("instance").option("dataSource");
                if (!jsonObj || jsonObj.length < 1) {
                    CommonManager.Instance.ShowNotification(options["must_product_msg"], NotificationType.error)
                    return false;
                }
                else {
                    jsonObj = CommonManager.Instance.DeleteGuidsFromLst(jsonObj);
                    jsonObj = JSON.stringify(jsonObj);
                    $(options["product_seralized_id"]).val(jsonObj);
                    return true;
                }
               
            });
        }

        var initProductsGrid = function (gridId, captions, lst, noDataText, isDetailsView) {
            if (!isDetailsView)
                isDetailsView = false;

            $(gridId).dxDataGrid({
                dataSource: lst,
                columns: [
                    { dataField: "Name", caption: captions["product"], width: 150 },
                    { dataField: "ColorName", caption: captions["color"], width: 100 },
                    { dataField: "SizeName", caption: captions["size"], width: 100 },
                    { dataField: "NumberOfPieces", caption: captions["quantity"], width: 90 },
                    { dataField: "SellingPrice", caption: captions["price"], width: 90 },
                    {
                        dataField: "TotalSellingPrice", caption: captions["total"], width: 160,
                        calculateDisplayValue: function (row) {
                            return row.NumberOfPieces * row.SellingPrice;
                        }
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {

                            var guid = row.data.Guid;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='OrderInfoManager.Instance.OnDeleteRow(" +"\"" + guid  + "\""+ ")'>" + captions["delete"] + "</a>");
                        },
                        visible: !isDetailsView
                    }
                ],
                summary: {
                    totalItems: [{
                        column: "NumberOfPieces",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0}"
                    }, {
                        name: "TotalSellingPrice",
                        summaryType: "custom",
                        showInColumn: "TotalSellingPrice",
                        displayFormat: captions["total"] + ": {0} " + captions["currency"],
                    }
                    ],
                    calculateCustomSummary: function (option) {
                        if (option.name == "TotalSellingPrice") {
                            if (option.summaryProcess === "start") {
                                option.totalValue = 0;
                            }
                            if (option.summaryProcess === "calculate") {
                                var row = option.value;
                                option.totalValue += row.NumberOfPieces * row.SellingPrice;
                            }
                        }
                    },
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
                    showPageSizeSelector: false,
                    showInfo: false,
                },
                rowAlternationEnabled: true,
                hoverStateEnabled: true,
                remoteOperations: {
                    paging: false,
                    filtering: false,
                    sorting: false
                },
                noDataText: noDataText,
            });
        }

        this.OnDeleteRow = function (guid) {
            var gridId = Options["products"]["grid_id"];
            var ds = $(gridId).dxDataGrid("instance").option("dataSource");
            var rows = ds.filter(function (e) { return e.Guid != guid; });
            $(gridId).dxDataGrid("instance").option("dataSource", rows);
            $(gridId).dxDataGrid("instance").refresh();
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new OrderInfoManagerInner();
            return instance;
        })()
    }
})();
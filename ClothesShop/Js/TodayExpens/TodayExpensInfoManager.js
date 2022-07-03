var TodayExpensInfoManager = (function () {

    function TodayExpensInfoManagerInner() {
        var self = this;
        var Options = null;
        this.Init = function (options) {
            Options = options;
            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);

            $(options["name_id"]).dxTextBox({
                value:''
            });

            $(options["cost_id"]).dxNumberBox({
                value: '',
                min: 0,
                showSpinButtons: true,
            });
            $(options["add_id"]).unbind("click").bind("click", function () {
                var name = $(options["name_id"]).dxTextBox("instance").option("value");
                var cost = $(options["cost_id"]).dxNumberBox("instance").option("value");
                if (name == '' || cost == '')
                    return;
                $(options["name_id"]).dxTextBox("instance").option("value",'');
                $(options["cost_id"]).dxNumberBox("instance").option("value",'');
                var obj = {
                    ID: 0,
                    Name: name,
                    Cost: cost,
                    Guid: CommonManager.Instance.GenerateGuid()
                };
                $(options["grid_id"]).dxDataGrid("instance").option("dataSource").push(obj);
                $(options["grid_id"]).dxDataGrid("instance").refresh();
            });
            $(options["form_id"]).unbind("submit").bind("submit", function () {
                var jsonObj = $(options["grid_id"]).dxDataGrid("instance").option("dataSource");
                if (!jsonObj || jsonObj.length < 1) {
                    CommonManager.Instance.ShowNotification(options["must_expens_msg"], NotificationType.error)
                    return false;
                }
                else {
                    jsonObj = CommonManager.Instance.DeleteGuidsFromLst(jsonObj);
                    jsonObj = JSON.stringify(jsonObj);
                    $(options["todayExpenseSeralized_id"]).val(jsonObj);
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
            var expensList = options["lst"];
            expensList = CommonManager.Instance.GenerateGuidsInLst(expensList);
            return {
                dataSource: expensList,
                columns: [
                    {
                        dataField: "Name",
                        caption: captions["name"],
                        minWidth: 120,
                        alignment: "right",
                        allowFiltering: false,
                        allowSorting: false,
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "Cost",
                        caption: captions["cost"],
                        minWidth: 120,
                        alignment: "right",
                        allowFiltering: false,
                        allowSorting: false,
                        allowHeaderFiltering: false,
                    },
                    {
                        dataField: "", caption: "", width: 80,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                        cellTemplate: function (content, row) {
                            var guid = row.data.Guid;
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='TodayExpensInfoManager.Instance.OnDeleteRow(" + "\"" + guid + "\"" + ")'>" + captions["delete"] + "</a>");
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
                instance = new TodayExpensInfoManagerInner();
            return instance;
        })()
    }
})();
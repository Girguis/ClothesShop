var EmployeeBalanceInfoManager = (function () {

    function EmployeeBalanceInfoManagerInner() {

        var Options = null;
        var self = this;
        this.Init = function (options) {
            
            Options = options;
            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);
            getData(options);
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
                    getData(Options);
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
            let types = CommonManager.Instance.NeowtonsoftDesealize(options["types"]);
            return {
                dataSource: [],
                columns: [
                    {
                        dataField: "Amount", caption: captions["amount"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "Type", caption: captions["type"], minWidth: 120,
                        allowSorting: false,
                        allowFiltering: true,
                        allowHeaderFiltering: false,
                        filterOperations: FilterLst,
                        alignment: "right",
                        dataType: "string",
                        calculateDisplayValue: function (row) {
                            var type = types.find(function (e) { return e.ID == row.Type });
                            return type ? type.Name : "";
                        },
      //                  groupIndex:2
                    },
                    {
                        dataField: "Year",
                        sortOrder:'desc',
                        caption: captions["year"],
                        groupIndex: 0,
                    },
                    {
                        dataField: "Month",
                        sortOrder: 'desc',
                        caption: captions["month"],
                        groupIndex: 1,
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
                            content.append("<a class='btn btn-sm btn-danger delete-order-btn' onclick='EmployeeBalanceInfoManager.Instance.OnDeleteRow(" + id + ")'>" + options["delete_text"] + "</a>");
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
                    visible: false,
                    applyFilter: "auto",
                    showAllText: options["all_text"],
                },
                headerFilter: {
                    visible: false
                },
                grouping: {
                    autoExpandAll: false,
                    expandMode: "rowClick",
                }

            }
        }

        var getData = function (options) {
            CommonManager.Instance.ShowHideLoading(true);
            var obj = { id: options["seller_id"] };
            CommonManager.Instance.SendRequest(options["url"], "POST", JSON.stringify(obj),
                function (result) {
                    var data = result.Data;
                    var totalWithdraw = 0, totalReward = 0, totalDeduction = 0, totalBalance = 0, availableBalance = 0;
                    
                    for (var i = 0; i < data.length;i++) {
                        if (data[i].Type == 0)
                            totalBalance += data[i].Amount;
                        else if (data[i].Type == 1)
                            totalWithdraw += data[i].Amount;
                        else if (data[i].Type == 2)
                            totalReward += data[i].Amount;
                        else
                            totalDeduction += data[i].Amount;
                    }
                    availableBalance = totalBalance + totalReward - totalDeduction - totalWithdraw;
                    $("#TotalWithdraw").text(totalWithdraw);
                    $("#TotalReward").text(totalReward);
                    $("#TotalDeduction").text(totalDeduction);
                    $("#TotalBalance").text(totalBalance);
                    $("#AvailableBalance").text(availableBalance);
                    $(options["grid_id"]).dxDataGrid("instance").option("dataSource", data);
                    $(options["grid_id"]).dxDataGrid("instance").refresh();
                    CommonManager.Instance.ShowHideLoading(false);
                });
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new EmployeeBalanceInfoManagerInner();
            return instance;
        })()
    }
})();
var TodayTransactionsManager = (function () {
    //----------------------------------------------------------

    //-----------------------------------------------------------
    function TodayTransactionsManagerInner() {
        var self = this;
        var Options = null;
        this.Init = function (options) {
            Options = options;

            var grid = initGrid(options);
            $(options["grid_id"]).dxDataGrid(grid);

            var noData = options["no_data"];
            

            var start_date = options["start_date"];
            CommonManager.Instance.InitDateBox(start_date["id"], start_date["div_id"], start_date["message"], noData, TodayTransactionsManager.Instance.SetToDate);
            
            var end_date = options["end_date"];
            CommonManager.Instance.InitDateBox(end_date["id"], end_date["div_id"], end_date["message"], noData, TodayTransactionsManager.Instance.SetFromDate);
            $(end_date["div_id"]).dxDateBox("instance").option("value", new Date());

            getData(options);

            $(options["search_btn"]).unbind("click").bind("click", function () {
                var start = $(start_date["div_id"]).dxDateBox("instance").option("value");
                var end = $(end_date["div_id"]).dxDateBox("instance").option("value");

                var diff = end.getTime() - start.getTime();

                diff = diff / (1000 * 60 * 60 * 24);

                if (diff > options["interval"]) {
                    CommonManager.Instance.ShowNotification(options["not_valid_msg"], NotificationType.Error)
                    return false;
                }
                else {
                    getData(options);
                    $(options["grid_id"]).dxDataGrid("instance").refresh();
                    SetWeekDays(options);
                    return true;
                }
            });
            $(options["pre_btn"]).unbind("click").bind("click", function () {
                var start = new Date($(start_date["div_id"]).dxDateBox("instance").option("value"));
                if (!start)
                    start = new Date();

                start.setDate(start.getDate() - 7);

                $(start_date["div_id"]).dxDateBox("instance").option("value", start);
                getData(options);
                return true;
            });

            $(options["next_btn"]).unbind("click").bind("click", function () {
                var end = $(end_date["div_id"]).dxDateBox("instance").option("value");
                if (!end)
                    end = new Date();

                end.setDate(end.getDate() + 1);
                $(start_date["div_id"]).dxDateBox("instance").option("value", end);
                getData(options);
                return true;
            });

        }

        var initGrid = function (options) {
            var gridId = options["grid_id"];
            var height = options["grid_height"];
            var captions = Options["grid_captions"];
            var noDataText = options["no_data"];
            var pageInfo = options["page_info"];
            var url = options["grid_data_url"];
            return {
                dataSource: GetGridDataSource(url, gridId, options["start_date"]["div_id"], options["end_date"]["div_id"]),
                columns: [
                    {
                        dataField: "ProductName", caption: captions["product_name"], width: 200,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "DayOne", caption: captions["day_one"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "DayTwo", caption: captions["day_two"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "DayThree", caption: captions["day_three"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "DayFour", caption: captions["day_four"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "DayFive", caption: captions["day_five"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "DaySix", caption: captions["day_six"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "DaySeven", caption: captions["day_seven"], width: 120,
                        allowSorting: false,
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        alignment: "right",
                        dataType: "string",
                    },
                    {
                        dataField: "TotalDays", caption: captions["total_number_of_pieces"], width: 150,
                        allowSorting: false, allowFiltering: false, allowHeaderFiltering: false,
                    },
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
                onCellPrepared: function (e) {
                    if (e.rowType == "header" && (e.columnIndex > 0 && e.columnIndex < 8)) {
                        var index = e.columnIndex - 1;
                        var dayName = SetDaysOfWeekHeader(options, index);
                        e.cellElement.html(dayName);
                    }
                },
                headerFilter: {
                    visible: true
                }
            }
        }

        this.SetToDate = function (fromDate) {

            if (!fromDate)
                return;
            var newDate = new Date(fromDate);
            newDate.setDate(newDate.getDate() + 6);
            $(Options["end_date"]["div_id"]).dxDateBox("instance").option("value", newDate);
        }

        this.SetFromDate = function (toDate) {

            if (!toDate)
                return;
            var newDate = new Date(toDate);
            newDate.setDate(newDate.getDate() - 6);
            $(Options["start_date"]["div_id"]).dxDateBox("instance").option("value", newDate);
        }

        var getData = function (options) {
            var start_date = options["start_date"];
            var end_date = options["end_date"];
            var start = $(start_date["div_id"]).dxDateBox("instance").option("value");
            var end = $(end_date["div_id"]).dxDateBox("instance").option("value");
            var obj = { StartDate: moment(start).format(DateFormat.DayMonthYear), EndDate: moment(end).format(DateFormat.DayMonthYear) };
            CommonManager.Instance.SendRequest(options["week_summary_url"], "POST", JSON.stringify(obj), function (result) {
                if (result) {
                    var sales = 0;
                    var expanses = 0;
                    var remaining = 0;
                    if (result) {
                        sales = result.TotalSales;
                        expanses = result.TotalExpenses;
                        remaining = result.TotalRemaining;
                    }
                    sales += options["currency"];
                    expanses += options["currency"];
                    remaining += options["currency"];
                    $(options["s_total_selled"]).html(sales);
                    $(options["s_total_expenses"]).html(expanses);
                    $(options["s_total_remaining"]).html(remaining);
                    $(options["grid_id"]).dxDataGrid("instance").refresh();
                }
            })
        }

        var GetGridDataSource = function (url, gridId,startID,endID) {
            return {
                load: function (loadOptions) {
                    var deferred = $.Deferred();
                    var filters = CommonManager.Instance.getFiltersList(loadOptions.filter);
                    var sort = CommonManager.Instance.getSort(loadOptions.sort);
                    var skip = loadOptions.skip;
                    var take = loadOptions.take;
                    var start = $(startID).dxDateBox("instance").option("value");
                    var end = $(endID).dxDateBox("instance").option("value");

                    if (!skip)
                        skip = 0;

                    if (!take) {
                        take = 20;
                        $(gridId).dxDataGrid("instance").option("paging.pageSize", take);
                    }

                    var obj = {
                        PageNumber: parseInt(skip) / parseInt(take) + 1,
                        PageSize: take,
                        FilteredColumns: filters.length == 0 ? null : filters,
                        OrderBy: sort
                    };
                    var data = {
                        model: { StartDate: moment(start).format(DateFormat.DayMonthYear), EndDate: moment(end).format(DateFormat.DayMonthYear) },
                        obj: obj,
                    };
                    CommonManager.Instance.SendRequest(url, "POST", JSON.stringify(data), function (result) {
                        var data = [];
                        var totalCount = 0;
                        if (result && result.Data)
                            data = result.Data;

                        if (result && result.TotalCount)
                            totalCount = result.TotalCount;

                        deferred.resolve(data, {
                            totalCount: totalCount
                        });
                    });
                    if ($(gridId).data("dxDataGrid"))
                           
                    return deferred.promise();
                }
            }
        }

        var SetDaysOfWeekHeader = function (options,index) {
            var days = options["days_list"];
            var start_date = options["start_date"];
            var startDate = $(start_date["div_id"]).dxDateBox("instance").option("value");
            var date = null;
            var today = new Date(startDate);
            date = moment(new Date(today.getFullYear(), today.getMonth(), today.getDate() + index)).format("MM/DD/YYYY");
            date = new Date(date);
            var dayName = days[date.getDay()];
            return dayName;
        }

        var SetWeekDays = function (options) {
            for (var i = 0; i < 7; i++) {
                var dayName = SetDaysOfWeekHeader(options, i);
                $($(options["grid_id"] + " table tr td[role='columnheader']")[i + 1]).html(dayName);
            }
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new TodayTransactionsManagerInner();
            return instance;
        })()
    }
})();
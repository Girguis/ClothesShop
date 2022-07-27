var EmployeeStatisticsManager = (function () {

    function EmployeeStatisticsManagerInner() {

        var Options = null;
        var self = this;
        this.Init = function (options) {
            Options = options;
            var grid = initGrid(options);
            var chart = initChart(options);
            $(options["grid_id"]).dxDataGrid(grid);
            $(options["chart_id"]).dxPieChart(chart);

            var date = options["date"];
            CommonManager.Instance.InitDateBox(date["id"], date["div_id"], date["message"], options["no_data"]);
            $(date["div_id"]).dxDateBox("instance").option("value", new Date());

            getData(new Date());

            $(date["div_id"]).dxDateBox({
                displayFormat: 'M/yyyy',
                onValueChanged: function (e) {
                    getData(e.value);
                }
            });

            $(options["pre_btn"]).unbind("click").bind("click", function () {
                var start = new Date($(date["div_id"]).dxDateBox("instance").option("value"));
                if (!start)
                    start = new Date();

                var newStart = new Date(start.getFullYear(), start.getMonth() - 1, 1);

                $(date["div_id"]).dxDateBox("instance").option("value", newStart);
                return true;
            });

            $(options["next_btn"]).unbind("click").bind("click", function () {
                var start = $(date["div_id"]).dxDateBox("instance").option("value");
                if (!start)
                    start = new Date();

                var newStart = new Date(start.getFullYear(), start.getMonth() + 1, 1);
                $(date["div_id"]).dxDateBox("instance").option("value", newStart);
                return true;
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
                dataSource:[],
                columns: [
                    {
                        dataField: "SellerName", caption: captions["seller"], minWidth: 150,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "TotallyDelivered", caption: captions["totally_delivered"], width: 110,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "PartialyDelivered", caption: captions["partialy_delivered"], width: 110,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "New", caption: captions["new"], width: 100,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "Waiting", caption: captions["waiting"], width: 100,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "CanceledByAgent", caption: captions["canceled_by_agent"], width: 140,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    },
                    {
                        dataField: "Total", caption: captions["total"], width: 110,
                        alignment: "right",
                        dataType: "string",
                        allowFiltering: false, allowSorting: false, allowHeaderFiltering: false,
                    }
                ],
                summary: {
                    totalItems: [{
                        column: "TotallyDelivered",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        column: "PartialyDelivered",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        column: "New",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        column: "Waiting",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        column: "CanceledByAgent",
                        summaryType: "sum",
                        displayFormat: captions["total"] + ": {0} "
                    },
                    {
                        name: "Total",
                        summaryType: "custom",
                        showInColumn: "Total",
                        displayFormat: captions["total"] + ": {0} "
                    }
                    ],
                    calculateCustomSummary: function (option) {
                     
                        if (option.name == "Total") {
                            if (option.summaryProcess === "start") {
                                option.totalValue = 0;
                            }
                            if (option.summaryProcess === "calculate") {
                                var row = option.value;
                                option.totalValue += row.TotallyDelivered + row.PartialyDelivered + row.New + row.CanceledByAgent + row.Waiting;
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
                }
            }
        }
        var initChart = function (options) {
            $(options["chart_id"]).dxPieChart({
                palette: 'bright',
                dataSource: [],
                size: {
                    width: 300,
                    height: 300
                },
                title: '',
                legend: {
                    visible: false,
                    orientation: 'horizontal',
                    itemTextPosition: 'right',
                    horizontalAlignment: 'center',
                    verticalAlignment: 'bottom',
                    columnCount: 4,
                },
                export: {
                    enabled: false,
                },
                series: [{
                    argumentField: 'SellerName',
                    valueField: 'Total',
                }],
                tooltip: {
                    enabled: true,
                    location: 'edge',
                    customizeTooltip(arg) {
                        return {
                            text: arg.argumentText + "\n(" + arg.valueText + ")" + arg.percentText
                        };
                    },
                }
            });
        }
        var getData = function (date) {
            var data = { date: moment(date).format(DateFormat.DayMonthYear) };
            CommonManager.Instance.SendRequest(Options["url"], "POST", JSON.stringify(data), function (result) {
                if (result) {
                               
                    var data = result.Data;
                    $(Options["chart_id"]).dxPieChart("instance").option("dataSource", data);
                    $(Options["chart_id"]).dxPieChart("instance").refresh();
                    $(Options["grid_id"]).dxDataGrid("instance").option("dataSource", data);
                    $(Options["grid_id"]).dxDataGrid("instance").refresh();
                }
            })
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new EmployeeStatisticsManagerInner();
            return instance;
        })()
    }
})();
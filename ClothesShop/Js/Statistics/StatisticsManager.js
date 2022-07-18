var StatisticsManager = (function () {

    function StatisticsManagerInner() {
        var self = this;
        var Options = null;
        this.Init = function (options) {
            Options = options;
            var noData = options["no_data"];
            initChart(options);

            var start_date = options["start_date"];
            CommonManager.Instance.InitDateBox(start_date["id"], start_date["div_id"], start_date["message"], noData, StatisticsManager.Instance.SetToDate);
            
            var end_date = options["end_date"];
            CommonManager.Instance.InitDateBox(end_date["id"], end_date["div_id"], end_date["message"], noData, StatisticsManager.Instance.SetFromDate);
            $(end_date["div_id"]).dxDateBox("instance").option("value",new Date());

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
                    return true;
                }
            });

            $(options["pre_btn"]).unbind("click").bind("click", function () {
                var start = new Date($(start_date["div_id"]).dxDateBox("instance").option("value"));
                if (!start)
                    start = new Date();

                var newStart = start.setDate(start.getDate() - 7);

                $(start_date["div_id"]).dxDateBox("instance").option("value", newStart);
                getData(options);
                return true;
            });

            $(options["next_btn"]).unbind("click").bind("click", function () {
                var end = $(end_date["div_id"]).dxDateBox("instance").option("value");
                if (!end)
                    end = new Date();

                var newStart = end.setDate(end.getDate()+1);
                $(start_date["div_id"]).dxDateBox("instance").option("value", newStart);
                getData(options);
                return true;
            });
        }

        var initChart = function (options) {
            $(options["chart_id"]).dxChart({
                dataSource:[] ,
                palette: 'soft',
                title: {
                    text: options["chart_title"]
                },
                commonSeriesSettings: {
                    type: 'bar',
                    valueField: 'total',
                    argumentField: 'day',
                    ignoreEmptyPoints: true,
                },
                seriesTemplate: {
                    nameField: 'day',
                },
                tooltip: {
                    enabled: true,
                    location: 'edge',
                    customizeTooltip(arg) {
                        return {
                            text: arg.seriesName + "\n" + options["total_selled_text"] + " " + arg.valueText + options["currency"],
                        };
                    },
                }
            });
       
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
            CommonManager.Instance.ShowHideLoading(true);
            var start_date = options["start_date"];
            var end_date = options["end_date"];
            var start = $(start_date["div_id"]).dxDateBox("instance").option("value");
            var end = $(end_date["div_id"]).dxDateBox("instance").option("value");
            var obj = { start: moment(start).format(DateFormat.DayMonthYear), end: moment(end).format(DateFormat.DayMonthYear) };

            CommonManager.Instance.SendRequest(options["url"], "POST", JSON.stringify(obj), function (result) {
                if (result) {
                    var chartData = result.data;
                    var data = [];
                    for (var i = 0; i < chartData.length;i++) {
                        var date = new Date(chartData[i].CreatedDate);
                        var dayName = options["days_list"][date.getDay()];
                        data.push({ indx: ((date.getDay() + 1) % 7), day: dayName + " (" + chartData[i].CreatedOn_ + ")", total: chartData[i].TotalTransactions })
                    }
                    data = CommonManager.Instance.Sort(data, "indx");
                    $(options["chart_id"]).dxChart("instance").option("dataSource",data)
                    $(options["payment_average"]).html(result.PaymentAverage.toFixed(2) +" " + options["currency"]);
                    $(options["payment_rate"]).css("background-color", result.RateColor);
                    $(options["payment_rate"]).html(result.Rate);
                    CommonManager.Instance.ShowHideLoading(false);
                }
                CommonManager.Instance.ShowHideLoading(false);
            })
        }

    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new StatisticsManagerInner();
            return instance;
        })()
    }
})();
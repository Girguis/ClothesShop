var MonthlyStatisticsManager = (function () {

    function MonthlyStatisticsManagerInner() {
        var self = this;
        var Options = null;
        this.Init = function (options) {
            Options = options;
            var noData = options["no_data"];
            initChart(options);

            var start_date = options["start_date"];
            CommonManager.Instance.InitDateBox(start_date["id"], start_date["div_id"], start_date["message"], noData);

            var end_date = options["end_date"];
            CommonManager.Instance.InitDateBox(end_date["id"], end_date["div_id"], end_date["message"], noData);

            $(start_date["div_id"]).dxDateBox("instance").option("value", getFirstDayOfMonth(new Date()));
            $(end_date["div_id"]).dxDateBox("instance").option("value", new Date());

            $(options["search_btn"]).unbind("click").bind("click", function () {
                var start = $(start_date["div_id"]).dxDateBox("instance").option("value");
                var end = $(end_date["div_id"]).dxDateBox("instance").option("value");
                
                var diff = end.getTime() - start.getTime();

                diff = diff / (1000 * 60 * 60 * 24);

                if (diff > options["interval"]) {
                    CommonManager.Instance.ShowNotification(options["not_valid_msg"], NotificationType.error)
                    return false;
                }
                else {
                    getData(options);
                    return true;
                }
            });

        }

        var getFirstDayOfMonth = function (date) {
            return new Date(date.getFullYear(), date.getMonth(), 1);
        }
        var initChart = function (options) {
            $(options["chart_id"]).dxChart({
                dataSource: [],
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


        var getData = function (options) {
            var start_date = options["start_date"];
            var end_date = options["end_date"];
            var start = $(start_date["div_id"]).dxDateBox("instance").option("value");
            var end = $(end_date["div_id"]).dxDateBox("instance").option("value");
            var obj = { start: moment(start).format(DateFormat.DayMonthYear), end: moment(end).format(DateFormat.DayMonthYear) };

            CommonManager.Instance.SendRequest(options["url"], "POST", JSON.stringify(obj), function (result) {
                if (result) {
                    var lst = result.data;
                    var groupedList = GroupDataByWeek(start, end, lst);
                    var data = [];
                    for (var i = 0; i < groupedList.length; i++) {
                        data.push({ day: groupedList[i].Range, total: groupedList[i].TotalTransactions });
                    }
                    $(options["chart_id"]).dxChart("instance").option("dataSource", data)
                    $(options["payment_average"]).html(result.PaymentAverage.toFixed(2) + " " + options["currency"]);
                    $(options["payment_rate"]).css("background-color", result.RateColor);
                    $(options["payment_rate"]).html(result.Rate);
                }
            })
        }

         var Filter = function (from, to, lst) {
            var _lst = lst.filter(function (obj) {
                var createdDate = new Date(obj.CreatedDate);

                return from <= createdDate && to >= createdDate
            });

             from = moment(from).format(DateFormat.DayMonthYear);
             to = moment(to).format(DateFormat.DayMonthYear);
            var range = (from != to) ? (to + " - " + from) : from;
            var total = 0;
            if (_lst && _lst.length) {
                for (var i = 0; i < _lst.length; i++) {
                    total += _lst[i].TotalTransactions;
                }
            }
            return { TotalTransactions: total, Range: range };
        }

         var GroupDataByWeek = function (startDate, endDate, lst) {
            var groupedList = [];
            var from = new Date(startDate);
            var to = new Date(from.getFullYear(), from.getMonth(), from.getDate() + 6);
            endDate = new Date(endDate);
            for (var i = 0; i < 5; i++) {
                if (endDate < to) {
                    to = endDate;
                }
                if (from > to)
                    break;
                var week = Filter(from, to, lst);
                groupedList.push(week);

                from = new Date(to.getFullYear(), to.getMonth(), to.getDate() + 1);
                to = new Date(from.getFullYear(), from.getMonth(), from.getDate() + 6);
            }
            return groupedList;
        }

    }



    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new MonthlyStatisticsManagerInner();
            return instance;
        })()
    }
})();
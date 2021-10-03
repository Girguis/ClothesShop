var StatisticsManager = function StatisticsManager() {
    var self = this;
    StatisticsManager.prototype.Init = function () {
        $("#StartDate").val(_StartDate);
        $("#EndDate").val(_EndDate);

        $("#StartDate").on("change", function (e) {
            
            var fromValue = $(this).val();
            var startDate = new Date(fromValue);

            var toDate = $("#EndDate").val();
            var endDate = new Date(toDate);
            if (startDate > endDate) {
                $("#EndDate").val(fromValue);
            }
        });
        $("#EndDate").on("change", function (e) {
            
            var toValue = $(this).val();
            var endDate = new Date(toValue);

            var fromValue = $("#StartDate").val();
            var startDate = new Date(fromValue);
            if (startDate > endDate) {
                $("#StartDate").val(toValue);
            }
        });
    }

    StatisticsManager.prototype.SearchByWeek = function() {
        
        var fromDate = $("#StartDate").val();
        var toDate = $("#EndDate").val();

        var startDate = new Date(fromDate);
        var endDate = new Date(toDate);
        var daysDiff = 1 + Math.ceil(Math.abs(endDate - startDate) / (1000 * 60 * 60 * 24));
        if (daysDiff > 7) {
            CommonManager.prototype.Alert(notValidDays);
            return;
        }
        else {
            self.GetStatistics(fromDate, toDate, function (result) {
                
                var labels = [];
                var data = [];
                if (result) {
                   
                    var lst = result.data;
                    for (var i = 0; i < lst.length; i++) {
                        date = new Date(lst[i].CreatedDate);
                        var dayName = days[date.getDay()];
                        labels.push(dayName + " (" + lst[i].CreatedOn_ + ")");
                        data.push(lst[i].TotalTransactions);
                    }
                    $("#PaymentAverage").html(result.PaymentAverage.toFixed(2) + +" "+currencyLE);
                    $("#PaymentRate").css("background-color", result.RateColor);
                    $("#PaymentRate").html(result.Rate);
                }
                if (chart)
                    chart.destroy();
                self.InitChart(title, label, labels, data);
            });
        }
    }
    StatisticsManager.prototype.GetStatistics = function(start, end, callback) {
        var obj = {
            start: start,
            end: end
        };
        $.ajax({
            url: getStatisticsURL,
            type: "POST",
            dataType: "JSON",
            data: obj,
            success: function (result) {
                callback(result);
            },
            error: function (err) {
                callback(null);
            }
        });
    }
    StatisticsManager.prototype.InitChart = function (title, label, labels, data) {
        var ctx = document.getElementById(chartId);
        chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: label,
                        backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#c45850", "#e8c3b9", "#c45850"],
                        data: data
                    }
                ]
            },
            options: {
                legend: { display: false },
                title: {
                    display: true,
                    text: title
                }
            }
        });
    }

    StatisticsManager.prototype.SearchByMonth = function () {
        
        var fromDate = $("#StartDate").val();
        var toDate = $("#EndDate").val();

        var startDate = new Date(fromDate);
        var endDate = new Date(toDate);
        var daysDiff = 1 + Math.ceil(Math.abs(endDate - startDate) / (1000 * 60 * 60 * 24));
        if (daysDiff > 31) {
            CommonManager.prototype.Alert(notValidDays);
            return;
        }
        else {
            self.GetStatistics(fromDate, toDate, function (result) {
                
                var labels = [];
                var data = [];
                if (result) {
                    var lst = result.data;
                    // group by week
                    var groupedList = self.GroupDataByWeek(fromDate, toDate, lst);
                    for (var i = 0; i < groupedList.length; i++) {
                        labels.push(groupedList[i].Range);
                        data.push(groupedList[i].TotalTransactions);
                    }
                    $("#PaymentAverage").html(result.PaymentAverage.toFixed(2) + " " + currencyLE);
                    $("#PaymentRate").css("background-color", result.RateColor);
                    $("#PaymentRate").html(result.Rate);
                }
                if (chart)
                    chart.destroy();
                self.InitChart(title, label, labels, data);
            });
        }
    }
    StatisticsManager.prototype.Filter = function(from, to, lst) {
        var _lst = lst.filter(function (obj) {
            var createdDate = new Date(obj.CreatedDate);

            return from <= createdDate && to >= createdDate
        });

        from = moment(from).format(dateFormat);
        to = moment(to).format(dateFormat);
        var range = (from != to) ? (to + " - " + from) : from;
        var total = 0;
        if (_lst && _lst.length) {
            for (var i = 0; i < _lst.length; i++) {
                total += _lst[i].TotalTransactions;
            }
        }
        return { TotalTransactions: total, Range: range };
    }
    StatisticsManager.prototype.GroupDataByWeek = function (startDate, endDate, lst) {

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
            var week = self.Filter(from, to, lst);
            groupedList.push(week);

            from = new Date(to.getFullYear(), to.getMonth(), to.getDate() + 1);
            to = new Date(from.getFullYear(), from.getMonth(), from.getDate() + 6);
        }
        return groupedList;
    }

}
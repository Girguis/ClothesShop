var SingleSellerStatisticsManager = (function () {

    function SingleSellerStatisticsManagerInner() {

        var Options = null;
        var self = this;
        this.Init = function (options) {
            Options = options;
            var date = options["date"];
            CommonManager.Instance.InitDateBox(date["id"], date["div_id"], date["message"], options["no_data"]);
            $(date["div_id"]).dxDateBox("instance").option("value", new Date());
            $(date["div_id"]).dxDateBox({
                displayFormat: 'M/yyyy',
                onValueChanged: function (e) {
                    getData(options["seller_id"], e.value);
                }
            });
            getData(options["seller_id"], new Date());
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

        var getData = function (id, date) {
            
            CommonManager.Instance.ShowHideLoading(true);
            var data = { id: id, date: date };

            $.ajax({
                url: Options["url"] + "?id=" + id + "&date=" + moment(date).format(DateFormat.DayMonthYear),
                type: "GET",
                success: function (result) {
                    CommonManager.Instance.ShowHideLoading(false);
                    $("#DataContainer").html(result);
                },
                error: function (err) {
                    CommonManager.Instance.ShowHideLoading(false);
                }
            })
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new SingleSellerStatisticsManagerInner();
            return instance;
        })()
    }
})();
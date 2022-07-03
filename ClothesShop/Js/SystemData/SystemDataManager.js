var SystemDataManager = (function () {

    function SystemDataManagerInner() {
        var self = this;

        this.Init = function (options) {
            $(options["btn_id"]).unbind("click").bind("click", function (e) {
                Confirm(options);
            });
        }

        var Confirm = function (options) {
            var obj = {
                Message: options["confirm_delete"],
                Title: options["confirm_title"],
                Yes: options["yes_text"],
                No: options["no_text"],
                Url: options["delete_url"],
                Type: "POST",
                Success: options["success_delete"],
                Error: options["error_delete"]
            };
            CommonManager.Instance.Confirm(obj, function (result) {
               
            });
        }
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new SystemDataManagerInner();
            return instance;
        })()
    }
})();
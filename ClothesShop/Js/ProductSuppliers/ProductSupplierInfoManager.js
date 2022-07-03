var ProductSupplierInfoManager = (
    function () {
        function ProductSupplierInfoManagerInner() {
            var self = this;
            this.Init = function (options) {
                var selectText = options["select"];
                var noData = options["no_data"];

                var products = options["products"];
                CommonManager.Instance.InitSelectList(products["id"], products["div_id"], products["lst"], selectText, noData);

                var suppliers = options["suppliers"];
                CommonManager.Instance.InitSelectList(suppliers["id"], suppliers["div_id"], suppliers["lst"], selectText, noData);

                var transitions = options["transactions"];
                CommonManager.Instance.InitSelectList(transitions["id"], transitions["div_id"], transitions["lst"], selectText, noData);
            }
        }
        var instance;
        return {
            Instance: (function () {
                if (!instance)
                    instance = new ProductSupplierInfoManagerInner();
                return instance;
            })()
        };
    })()
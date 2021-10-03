var LayoutManager = function LayoutManager() {
    var self = this;
    self.CurrentTabIndex = 1;
    LayoutManager.prototype.GetActiveItem = function () {
        var activeItem = "";
        var url = window.location.href;
        url = url.toLowerCase();
        var parentId1 = "";
        var parentId2 = "";
        
        if (!url.includes("/settings/")) {
            // first menu
            if (url.includes("home")) {
                activeItem = "HomeMenuItem";
                parentId1 = "";
            }
            if (url.includes("/orders")) {
                activeItem = "Orders";
                parentId1 = "HomeMenuItem";
            }
            else if (url.includes("/delivery")) {
                activeItem = "Delivery";
                parentId1 = "HomeMenuItem";
            }
            else if (url.includes("/todaytransactions")) {
                activeItem = "TodaySummary";
                parentId1 = "HomeMenuItem";
                parentId2 = "TodayTransactionsItem";
            }
            else if (url.includes("/todaysales")) {
                activeItem = "TodaySales";
                parentId1 = "HomeMenuItem";
                parentId2 = "TodayTransactionsItem";
            }
            else if (url.includes("/todayexpens")) {
                activeItem = "TodayExpenses";
                parentId1 = "HomeMenuItem";
                parentId2 = "TodayTransactionsItem";
            }
            else if (url.includes("/productsuppliers")) {
                activeItem = "ProductSuppliers";
                parentId1 = "HomeMenuItem";
                parentId2 = "StoringRecordsItem";
            }
            else if (url.includes("/productssummary")) {
                activeItem = "ProductSummary";
                parentId1 = "HomeMenuItem";
                parentId2 = "StoringRecordsItem";
            } // second menu
            else if (url.includes("/employees")) {
                activeItem = "Employees";
                parentId1 = "OtherInfoItem";
            }
            else if (url.includes("/products")) {
                activeItem = "Products";
                parentId1 = "OtherInfoItem";
            }
            else if (url.includes("/shipmentcompanies")) {
                activeItem = "ShipmentCompanies";
                parentId1 = "OtherInfoItem";
            }
            else if (url.includes("/suppliers")) {
                activeItem = "Suppliers";
                parentId1 = "OtherInfoItem";
            }
            else if (url.includes("/colors")) {
                activeItem = "Colors";
                parentId1 = "OtherInfoItem";
            }
            else if (url.includes("/sizes")) {
                activeItem = "Sizes";
                parentId1 = "OtherInfoItem";
            }
            else if (url.includes("/salesrates")) {
                activeItem = "SalesRate";
                parentId1 = "StatisticsMenuItem";
            }
            else if (url.includes("/statistics/weekly")) {
                activeItem = "WeeklyStatistics";
                parentId1 = "StatisticsMenuItem";
            }
            else if (url.includes("statistics/monthly")) {
                activeItem = "MonthlyStatistics";
                parentId1 = "StatisticsMenuItem";
            }

        }
        else {
            if (url.includes("roles/edit/1")) {
                activeItem = "ManagerRoles";
                parentId1 = "SettingsMenuItem";
            }
            else if (url.includes("roles/edit/2")) {
                activeItem = "WorkerRoles";
                parentId1 = "SettingsMenuItem";
            }
            else if (url.includes("roles/edit/3")) {
                activeItem = "DeliveryManRoles";
                parentId1 = "SettingsMenuItem";
            }
            else if (url.includes("roles/edit/4")) {
                activeItem = "EmployeeRoles";
                parentId1 = "SettingsMenuItem";
            }
            else if (url.includes("/logins")) {
                activeItem = "Employees";
                parentId1 = "SettingsMenu";
            }
            else if (url.includes("/systemdata")) {
                activeItem = "SystemData";
                parentId1 = "SettingsMenu";
            }
        }
        $(".list-group-item.list-group-item-action").removeClass("active");

        if (parentId2 != "" && parentId2 != null && parentId2 != undefined) {
            $("#" + parentId2).trigger("click");
        }

        if (parentId1 != "" && parentId1 != null && parentId1 != undefined) {
            $("#" + parentId1).trigger("click")
        }
        if (!IsNullOrEmpty(activeItem)) {
            $("#" + activeItem).addClass("active");
        }
        
    }
    LayoutManager.prototype.ShowHideLoading = function (isShown) {
        if (isShown) {
            $("#loading").show();
        }
        else {
            $("#loading").hide();
        }
    }
    LayoutManager.prototype.CollapseMenu = function () {
        $("#wrapper").toggleClass("active");
        $("#bodyContainer").toggleClass("leftMenuActive");
        $("#footer").toggleClass("leftMenuActive");
        $("#navbar").toggleClass("leftMenuActive");
    }
    $(function () {
        self.GetActiveItem();
        self.CollapseMenu();
    });
    function IsNullOrEmpty(val) {
        try {
            return (val == "" || val == null || val == undefined);
        }
        catch (e) {
            return false;
        }
    }
}
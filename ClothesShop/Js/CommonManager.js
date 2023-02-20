var CommonManager = (function () {

    function CommonManagerInner() {
        var self = this;

        this.IsMobileDevice = function () {
            return (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent));
        }

        this.ShowHideLoading = function (isShown) {
            if (isShown) {
                $("#loading").show();
            }
            else {
                $("#loading").hide();
            }
        }

        this.Confirm = function (data, callback) {
            var obj = {
                buttons: [{
                    text: data.Yes,
                    type: "danger",
                    onClick: function (e) {
                        return true;
                    }
                }, {
                    text: data.No,
                    type: "normal",
                    onClick: function (e) {
                        return false;
                    }
                    }],
                rtlEnabled: true,
                message: data.Message,
                title: data.Title,
                showTitle: true,
            };

            var result = DevExpress.ui.dialog.confirm(obj);
            result.done(function (dialogResult) {
                if (!dialogResult)
                    return callback();

                self.ShowHideLoading(true);
                CommonManager.Instance.SendRequest(data.Url, data.Type, null, function (result) {
                    self.ShowHideLoading(false);
                    if (result)
                        CommonManager.Instance.ShowNotification(data.Success, NotificationType.success);
                   
                    else
                        CommonManager.Instance.ShowNotification(data.Error, NotificationType.error);
                    
                    return callback(result);
                });
            });
        }
        this.print = function (url) {
            self.ShowHideLoading(true);
            $.ajax({
                url: url,
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    self.ShowHideLoading(false);
                    printing = window.open(result);
                    printing.focus();
                    printing.print();
                },
                error: function (err) {
                    self.ShowHideLoading(false);
                }
            });
        }

        this.SendRequest = function (url, type, data, callback) {
            $.ajax({
                url: url,
                data: data,
                dataType: "JSON",
                type: type,
                contentType: 'application/json; charset=utf-8',
                success: function (res) {
                    
                    if (callback)
                        callback(res);
                },
                error: function (r) {
                    
                    if (callback)
                        callback();
                }
            })
        }

        this.ParseObjToQueryString = function (obj) {
         
            return "";
        }

        this.getFiltersList = function (filters) {
            var lst = [];
            console.log(filters);
            if (!filters || filters.length < 1)
                return lst;

            if (!Array.isArray(filters[0]) && filters.length == 3)
                return [{ ColumnName: filters[0], SearchValue: filters[2] }];

            for (var i = 0; i < filters.length; i++) {
                if (Array.isArray(filters[i]) && !lst.find(function (e) { return e.ColumnName == filters[i][0] })) {
                    if (Array.isArray(filters[i][0])) {
                        for (var j = 0; j < filters[i].length; j++)
                        {
                            if (Array.isArray(filters[i][j]) && !lst.find(function (e) { return e.ColumnName == filters[i][j][0] }))
                                lst.push({ ColumnName: filters[i][j][0], SearchValue: filters[i][j][2] });
                        }
                    }
                    else
                        lst.push({ ColumnName: filters[i][0], SearchValue: filters[i][2] });
                }
            }
            console.log(lst);
            return lst;
        }

        this.getSort = function (sort) {
            if (sort == null || sort.length < 1)
                return null;
            sort = sort[0];
            if (sort.selector.context == undefined) {
                return {
                    ColumnName: sort.selector,
                    Direction: sort.desc ? "DESC" : "ASC"
                };
            }
            return {
                ColumnName: sort.selector.context.column.dataField,
                Direction: sort.desc ? "DESC" : "ASC"
            };
        }

        this.ToArray = function (obj) {
            if (!obj)
                return [];

            return Object.keys(obj).map((key) => [key, obj[key]]);
        }

        this.ParseDate = function (date, format) {
            if (date) {
                date = date.replaceAll("/Date(", "")
                date = date.replaceAll(")/", "");
            }

            if (date && !isNaN(date))
                date = new Date(parseInt(date));

            date = moment(date).format(format);
            return date;
        }

        this.ReplaceAll = function (str, oldVal, newVal) {
            return str.replaceAll(oldVal, newVal);
        }

        this.ShowNotification = function (message, type) {
            let direction = 'up-push';
            let position = 'bottom center';

            DevExpress.ui.notify({
                message: message,
                height: 45,
                width: 300,
                minWidth: 300,
                type: type,
                displayTime: 3500,
                animation: {
                    show: {
                        type: 'fade', duration: 400, from: 0, to: 1,
                    },
                    hide: { type: 'fade', duration: 40, to: 0 },
                },
            },
                {
                    position,
                    direction
                });
        }

        this.EnumToDataSourceHeaderFilter = function (id, operator, lst, key, value) {
            var ds = [];
            if (!lst || lst.length < 1)
                return ds;

            for (var i = 0; i < lst.length; i++) {
                ds.push({ "text": lst[i][value], "value": [id, operator, lst[i][key]] });
            }
            return ds;
        }

        this.NeowtonsoftDesealize = function (str) {
            let arr = self.ReplaceAll(str, "&quot;", "\"")
            arr = JSON.parse(arr);
            return arr;
        }

        this.InitSelectList = function (id, divId, lst, placeholder, noData) {

            if (!lst)
                lst = [];

            var ds = new DevExpress.data.ArrayStore({
                data: lst,
                key: "ID",
            });

            var defaultValue = $(id).val();
            var elem = lst.find(function (e) { return e.ID == defaultValue });

            if ($(divId).data("dxSelectBox")) {
                $(divId).dxSelectBox("instance").option("dataSource", ds);

                if (elem)
                    $(divId).dxSelectBox("instance").option("value", elem.ID);
                return;
            }

            $(divId).dxSelectBox({
                dataSource:ds,
                placeholder: placeholder,
                noDataText: noData,
                value: null,
                searchEnabled: true,
                rtlEnabled: true,
                displayExpr: "Name",
                valueExpr: "ID",
                onValueChanged: function (e) {
                    $(id).val("");
                    if (e)
                        $(id).val(e.value);
                },
            });
            if (elem && $(divId).data("dxSelectBox"))
                $(divId).dxSelectBox("instance").option("value", elem.ID);
        }

        this.InitDefaultSelectList = function (divId, lst, placeholder, noData, funName) {
            
            if (!lst)
                lst = [];

            var ds = new DevExpress.data.ArrayStore({
                data: lst,
                key: "ID",
            });
            if ($(divId).data("dxSelectBox")) {
                $(divId).dxSelectBox("instance").option("dataSource", ds);

                if (lst && lst.length > 0)
                    $(divId).dxSelectBox("instance").option("value", lst[0].ID);

                return;
            }

            $(divId).dxSelectBox({
                dataSource:ds,
                placeholder: placeholder,
                noDataText: noData,
                value: null,
                searchEnabled: true,
                rtlEnabled: true,
                displayExpr: "Name",
                valueExpr: "ID",
                onValueChanged: function (e) {
                    
                    if (funName && typeof (funName) == "function") {
                        funName.apply(null, [e.value]);
                    }
                },
            });
            if (lst && lst.length > 0)
                $(divId).dxSelectBox("instance").option("value", lst[0].ID);
        }

        this.InitDateBox = function (id, divId, placeholder, noData, funName) {

            var defaultValue = $(id).val();
            
            if ($(divId).data("dxDateBox")) {
                    $(divId).dxDateBox("instance").option("value", defaultValue);
                return;
            }
            $(divId).dxDateBox({
                placeholder: placeholder,
                noDataText: noData,
                value: null,
                searchEnabled: true,
                rtlEnabled: true,
                onValueChanged: function (e) {
                    $(id).val("");
                    if (e && e.value)
                        $(id).val(moment(e.value).format(DateFormat.DayMonthYearHourMin));
                       
                    if (funName && typeof (funName) == "function" && e && e.value) {
                        funName.apply(null, [e.value]);
                    }
                },
            });
            if ($(divId).data("dxDateBox"))
                $(divId).dxDateBox("instance").option("value", defaultValue);
        }

        this.GenerateGuid = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        this.GenerateGuidsInLst = function (lst) {
            if (!lst)
                lst = [];
            $(lst).each(function (indx, elem) {
                elem.Guid = CommonManager.Instance.GenerateGuid();
            });
            return lst;
        }

        this.DeleteGuidsFromLst = function (lst) {
            if (!lst)
                lst = [];
            $(lst).each(function (indx, elem) {
                delete elem.Guid;
            });
            return lst;
        }

        this.IsNullOrEmpty = function (val) {
            try {
                return (val == "" || val == null || val == undefined);
            }
            catch (e) {
                return false;
            }
        }

        this.GetActiveMenuItem = function () {
            var activeItem = "";
            var url = window.location.href;
            url = url.toLowerCase();
            if (!url.includes("/settings/")) {
                // first menu
                if (url.includes("/orders?pageid=137d0514-f286-48aa-bcd4-b7fe7c5b79d8")) {
                    activeItem = "Orders1";
                }
                else if (url.includes("/orders?pageid=80db4628-f4d2-45ea-b82f-b0e2b7e9fd09")) {
                    activeItem = "Orders2";
                }
                else if (url.includes("/delivery")) {
                    activeItem = "Delivery";
                }
                else if (url.includes("/employeestatistics")) {
                    activeItem = "EmployeeStatistics";
                }
                else if (url.includes("/employeebalance")) {
                    activeItem = "EmployeeBalance";
                }
                else if (url.includes("/todaytransactions")) {
                    activeItem = "TodayTransactions";
                }
                else if (url.includes("/todaysales")) {
                    activeItem = "TodaySales";
                }
                else if (url.includes("/todayexpens")) {
                    activeItem = "TodayExpens";
                }
                else if (url.includes("/productsuppliers")) {
                    activeItem = "ProductSuppliers";
                }
                else if (url.includes("/productssummary")) {
                    activeItem = "ProductsSummary";
                } // second menu
                else if (url.includes("/employees")) {
                    activeItem = "Employees";
                }
                else if (url.includes("/products")) {
                    activeItem = "Products";
                }
                else if (url.includes("/shipmentcompanies")) {
                    activeItem = "ShipmentCompanies";
                }
                else if (url.includes("/suppliers")) {
                    activeItem = "Suppliers";
                }
                else if (url.includes("/colors")) {
                    activeItem = "Colors";
                }
                else if (url.includes("/sizes")) {
                    activeItem = "Sizes";
                }
                else if (url.includes("/salesrates")) {
                    activeItem = "SalesRates";
                }
                else if (url.includes("/statistics/weekly")) {
                    activeItem = "WeeklyStatistics";
                }
                else if (url.includes("statistics/monthly")) {
                    activeItem = "MonthlyStatistics";
                }
            }
            else {
                if (url.includes("roles/edit/1")) {
                    activeItem = "ManagerRoles";
                }
                else if (url.includes("roles/edit/2")) {
                    activeItem = "SellerRoles";
                }
                else if (url.includes("roles/edit/3")) {
                    activeItem = "DeliveryManRoles";
                }
                else if (url.includes("roles/edit/4")) {
                    activeItem = "PageOneSellerRoles";
                }
                else if (url.includes("roles/edit/5")) {
                    activeItem = "PageTwoSellerRoles";
                }
                else if (url.includes("/logins")) {
                    activeItem = "AccountsRoles";
                }
                else if (url.includes("/systemdata")) {
                    activeItem = "DeleteSystemDataRoles";
                }
            }
            $("#sidebar .sidebar-nav .sidebar-item").removeClass("active");

            if (!self.IsNullOrEmpty(activeItem)) {
                $("#" + activeItem).addClass("active");
            }
        }

        this.GetGridDataSource = function (url, gridId) {
            return {
                load: function (loadOptions) {
                    var deferred = $.Deferred();
                    var filters = self.getFiltersList(loadOptions.filter);
                    var sort = self.getSort(loadOptions.sort);
                    var skip = loadOptions.skip;
                    var take = loadOptions.take;

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

                    self.SendRequest(url, "POST", JSON.stringify(obj), function (result) {
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
                    debugger;
                    return deferred.promise();
                }
            }
        }

        this.Sort = function (lst, key) {
            lst.sort((a, b) => (a[key] > b[key]) ? 1 : ((b[key] > a[key]) ? -1 : 0));
            return lst;
        }

        this.Export = function (url) {
            self.ShowHideLoading(true);
            $.ajax({
                url: url,
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    
                    self.ShowHideLoading(false);
                    if (!result)
                        return;
                    var url = result;

                    const a = document.createElement('a');
                    a.style.display = 'none';
                    a.href = url;
                    // the filename you want
                    var t = result.split("/");

                    a.download = t[t.length - 1];
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    
                },
                error: function (err) {
                    self.ShowHideLoading(false);
                }
            });
        }
        $(function () {
            self.GetActiveMenuItem();
        });
    }

    var instance;
    return {
        Instance: (function () {
            if (!instance)
                instance = new CommonManagerInner();
            return instance;
        })()
    }
})();
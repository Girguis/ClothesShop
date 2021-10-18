var TableManager = function TableManager() {
    var self = this;

    self.PageNumber = 1;
    self.PageSize = 20;
    self.NumberOfPages = 0;
    self.PagerID = "Pager";
    self.OrderByColumnName = "";
    self.OrderByDirection = "";
    self.IsResetSearch = false;
    TableManager.prototype.InitPagination = function () {
        self.IsResetSearch = false;
        if (self.NumberOfPages <= 1)
            $("#" + self.PagerID).hide();
        else {
            $("#" + self.PagerID).show();
            if ($("#" + self.PagerID).data("twbs-pagination"))
                $("#" + self.PagerID).twbsPagination('destroy');

            $("#" + self.PagerID).twbsPagination({
                totalPages: self.NumberOfPages,
                visiblePages: self.NumberOfPages > 5 ? 5 : self.NumberOfPages,
                first: first,
                prev: previous,
                next: next,
                last: last,
                startPage: self.PageNumber,
                show: self.PageNumber,
                initiateStartPageClick:false,
                onPageClick: function (x, page) {
                   
                    if (page == self.PageNumber)
                        return;
                    else {
                        self.PageNumber = page;
                        self.IsResetSearch = false;
                        self.GetData();
                        $("#" + self.PagerID).twbsPagination({ render: self.PageNumber });
                        self.IsResetSearch = false;
                    }
                }
            });
        }
    }
    TableManager.prototype.Init = function () {
        self.OrderByColumnName = orderBy.ColumnName;
        self.OrderByDirection = orderBy.Direction;
        if (searchColumns && searchColumns.length > 0) {
            for (var i = 0; i < searchColumns.length; i++) {
               
                $("#" + searchColumns[i].Name).on("change keyup", function (e) {
                    
                    var id = e.target.id;
                    var column = searchColumns.find(function (e) { return e.Name == id });
                    if (e.keyCode == 13 || (column != undefined && (column.Type == "select" || column.Type == "date")))
                        self.GetData();
                });
            }
        }
        self.GetData();
    }
    TableManager.prototype.Sort = function (element) {
       
        var headerId = $(element).attr("data-headerid");
        var dir = $(element).attr("data-dir");

        if ($(element).find(".searchArrow") && $(element).find(".searchArrow").length > 0) {
            $($(element).find(".searchArrow")).removeClass("fa-arrow-down");
            $($(element).find(".searchArrow")).removeClass("fa-arrow-up");

            if (dir == "desc") {
                $($(element).find(".searchArrow")).addClass("fa-arrow-up");
                dir = "asc";
            }
            else if (dir == "asc") {
                dir = "desc";
                $($(element).find(".searchArrow")).addClass("fa-arrow-down");
            }
        }
        else {
            var icon = "<i class='fa fa-arrow-down searchArrow'></i>" + $(element).html();
            $(element).html(icon);
        }
        var currentIndex = $(element)[0].cellIndex;
        $("#"+gridId + " thead .theadData th").each(function (index, item) {
           
            if (index != currentIndex && ($(item).find(".searchArrow") && $(item).find(".searchArrow").length > 0) ) {

                    $($(item).find(".searchArrow")).removeClass("fa-arrow-down");
                    $($(item).find(".searchArrow")).removeClass("fa-arrow-up");

            }
        });


        if (!dir)
            dir = "desc";

        $(element).attr("data-dir", dir);

        self.OrderByColumnName = headerId;
        self.OrderByDirection = dir;

        self.GetData();

    }
    TableManager.prototype.GetSearchCriteria = function () {
        var obj = {
            PageNumber: self.PageNumber,
            PageSize: self.PageSize,
        };
        var columns = [];
        debugger;
        if (self.IsResetSearch) {
            for (var i = 0; i < searchColumns.length; i++) {
                var searchType = searchColumns[i].Type;
                if (searchValue == "-1" && searchType == "select")
                    $("#" + searchColumns[i].Name).val("-1");
                else
                    $("#" + searchColumns[i].Name).val("");
            }
        }
        if (searchColumns && searchColumns.length > 0) {
            for (var i = 0; i < searchColumns.length; i++) {
                var searchValue = $("#" + searchColumns[i].Name).val();
                var searchType = searchColumns[i].Type;
                if (searchValue == "-1" && searchType == "select")
                    searchValue = "";
                else if (searchValue && searchType == "date")
                    searchValue = moment(new Date(searchValue)).format("DD/MM/YYYY");

                if (searchValue)
                    columns.push({ ColumnName: searchColumns[i].Name, SearchValue: searchValue });
            }
        }            

        obj.FilteredColumns = columns;

        obj.OrderBy = {
            ColumnName: self.OrderByColumnName,
            Direction: self.OrderByDirection
        };
        if (viewName && viewName == "TodayTransactions") {
            return {
                obj: obj,
                model:
                {
                    StartDate: $("#StartDate").val(),
                    EndDate: $("#EndDate").val(),
                }
            };
        }
        else
            return obj;
    }
    TableManager.prototype.GetData = function () {
        
        $("#loading").show();
        var obj = self.GetSearchCriteria();
        $.ajax({
            url: dataURL,
            dataType: "JSON",
            type: "POST",
            data: obj,
            success: function (result) {
             
                $("#loading").hide();
             
                if (viewName && viewName == "Orders")
                    self.FillOrdersData(result);
                else if (viewName && viewName == "DeliveryAddUpdate")
                    self.FillDeliveryAddUpdate(result);
                else if (viewName && (viewName == "TodaySales" || viewName == "TodayExpens"))
                    self.FillTodaySales(result);
                else
                    self.FillData(result);
                if (viewName =="ProductsSummary")
                    self.setFooterData(result);
               
                if (result)
                    self.NumberOfPages = result.NumberOfPages;
         
                self.InitPagination();
            },
            error: function (err) {
                $("#loading").hide();
                
                self.NumberOfPages = 0;
                self.InitPagination();
            }
        })
    }
    TableManager.prototype.setFooterData = function (result) {
        var footer = "";
        footer += "<tr>";
        footer += "<td style='text-align: left;'>";
        footer += totalSum + " : " +result.Total + " " + currencyLE;
        footer += "</td>";
        footer += "</tr>";

        $("#" + gridId + " tfoot").empty();
        $("#" + gridId + " tfoot").html(footer);
    }
    TableManager.prototype.FillData = function (result) {
        var table = "";
        if (result && result.data && result.data.length > 0) {
            var lst = result.data;
            for (var i = 0; i < lst.length; i++) {
                table += "<tr>";
                for (var j = 0; j < columns.length; j++) {
                    table += "<td class='" + columns[j].Class + "'>";
                    if (columns[j].Name == "ADD") {
                      
                        if (addURL && isAdd)
                            table += "<a class='btn  btn-outline-success' href='" + addURL + lst[i][uniqueID] + "' ><i class='fa fa-plus'></i> "+addText+"</a>";
                    }
                    else if (columns[j].Name == "EDIT") {
                        if (updateURL && isUpdate)
                            table += "<a class='btn  btn-outline-primary' href='" + updateURL + lst[i][uniqueID] + "' ><i class='fa fa-edit'></i> " + updateText + "</a>";
                    }
                    else if (columns[j].Name == "DELETE") {
                        if (deleteURL && isDelete)
                            table += "<a class='btn  btn-outline-danger' onclick='" + deleteURL + "(" + lst[i][uniqueID] +")"+ "' ><i class='fa fa-trash'></i> " + deleteText + "</a>";
                    }
                    else if (columns[j].Name == "DETAILS") {
                        if (detailsURL && isDetails)
                            table += "<a class='btn  btn-outline-info' href='" + detailsURL + lst[i][uniqueID] + "' ><i class='fa fa-info'></i> " + detailsText + "</a>";
                    }
                    else if (columns[j].Name == "PRINT") {
                        if (printURL && isDetails)
                            table += "<a class='btn btn-outline-secondary'  onclick='PrintManager.prototype.Print(\"" + printURL + "?id="+lst[i][uniqueID]+"\")' href='#'><i class='fa fa-print'></i> " + printText + "</a>";
                    }
                    else if (columns[j].Name == "ExportToExcel") {
                        if (ExportURL && isDetails)
                            table += "<a class='btn btn-outline-secondary'  onclick='PrintManager.prototype.Export(\"" + ExportURL + "?id="+lst[i][uniqueID]+"\")' href='#'><i class='fa fa-file-excel-o'></i> " + exportText + "</a>";
                    }
                    else {
                        var columnName = columns[j].Name;
                        var data = lst[i][columnName];
                        if (!data || data == null)
                            data = "";
                        if (data && typeof data == "object")
                            table += data.join(" ,");
                        else
                            table += data;
                    }

                    table += "</td>";
                }
                table += "</tr>";
            }

        }
        else {
            table += "<tr class='NoDataText'>";
            table += "<td class='NoDataText text-center' colspan='" + columns.length + "'>";
            table += noDataText;
            table += "</td>";
            table += "</tr>";
        }
        $("#" + gridId + " tbody").empty();
        $("#" + gridId + " tbody").html(table);
    }
    TableManager.prototype.FillOrdersData = function (result) {
        var table = "";
        if (result && result.data && result.data.length > 0) {
            var lst = result.data;
            console.log(lst);
            for (var i = 0; i < lst.length; i++) {
                table += "<tr>";
                var date = lst[i].RequestDate
                if (date)
                    date = date.replaceAll("/Date(", "")
                date = date.replaceAll(")/", "")

                if (date && !isNaN(date))
                    date = new Date(parseInt(date));

                table += "<td class='width15'>";
                table += lst[i].ID;
                table += "</td>";

                table += "<td class='width30' style='direction: ltr;'>";
                table += date ? moment(date).format("DD/MM/YYYY hh:mm A"):"";
                table += "</td>";

                table += "<td class='width30'>";
                table += lst[i].CustomerName;
                table += "</td>";

                table += "<td class='width30'>";
                table += lst[i].CustomerNumber;
                table += "</td>";

                var address = "";
                if (lst[i].CustomerAddress) {
                    address = lst[i].CustomerAddress.split("-");
                    if (address.length > 1)
                        address = address[address.length - 1];
                    else
                        address = address[0];
                }

                table += "<td class='width30'>";
                table += address;
                table += "</td>";

                table += "<td class='width15'>";
                table += lst[i].Total;
                table += "</td>";
                table += "<td class='width15'>";
                table += lst[i].ShipmentPrice ? lst[i].ShipmentPrice:"";
                table += "</td>";
                table += "<td class='width30'>";
                table += lst[i].OrderStatusName;
                table += "</td>";
                table += "<td class='width30'>";
                table += lst[i].DeliveryManName ? lst[i].DeliveryManName:"";
                table += "</td>";

                table += "<td class='width30'>";
                table += lst[i].SellerName ? lst[i].SellerName:"";
                table += "</td>";

                table += "<td class='width15'>";
                if (detailsURL && isDetails)
                    table += "<a class='btn  btn-outline-info' href='" + detailsURL + lst[i].ID + "' ><i class='fa fa-info'></i> " + detailsText + "</a>";
                table += "</td>";

                table += "<td class='width15'>";
                if (printURL && isDetails)
                    table += "<a class='btn btn-outline-secondary' onclick='PrintManager.prototype.Print(\"" + printURL + "?id=" + lst[i].ID + "\")' href='#'><i class='fa fa-print'></i> " + printText + "</a>";
                table += "</td>";
                table += "<td class='width15'>";
                if (updateURL && isUpdate && lst[i].OrderStatusID != totallyDeliveredStatus)
                    table += "<a class='btn  btn-outline-primary' href='" + updateURL + lst[i].ID + "' ><i class='fa fa-edit'></i> " + updateText + "</a>";
                table += "</td>";

                table += "<td class='width15'>";
                if (deleteURL && isDelete && lst[i].OrderStatusID != totallyDeliveredStatus)
                    table += "<a class='btn  btn-outline-danger' onclick='" + deleteURL + "(" + lst[i].ID + ")" + "' ><i class='fa fa-trash'></i> " + deleteText + "</a>";
                table += "</td>";

                table += "</tr>";
            }

        }
        else {
            table = "<tr class='NoDataText'>";
            table += "<td class='NoDataText text-center' colspan='14'>";
            table += noDataText;
            table += "</td>";
            table += "</tr>";
        }
        $("#" + gridId + " tbody").empty();
        $("#" + gridId + " tbody").html(table);
    }
    TableManager.prototype.FillDeliveryAddUpdate = function (result) {
        var table = "";
        if (result && result.data && result.data.length > 0) {
            var lst = result.data;
            for (var i = 0; i < lst.length; i++) {
                table += "<tr>";

                table += "<td class='width5'>";
                table += "<input type='checkbox' id='checkBox_" + lst[i][uniqueID] + "' data-id='" + lst[i][uniqueID] + "' onchange='OnChangeSelection(this)' " + (IdsList && IdsList.length > 0 && IdsList.indexOf(lst[i][uniqueID]) != -1 ? "checked" : "") + " />";
                table += "</td>";

                table += "<td class='width10'>";
                table += lst[i].ID;
                table += "</td>";

                table += "<td class='width20'>";
                table += lst[i].Customer.Name;
                table += "</td>";

                var address = "";
                if (lst[i].Customer && lst[i].Customer.Address) {
                    address = lst[i].Customer.Address.split("-");
                    if (address.length > 1)
                        address = address[address.length - 1];
                    else
                        address = address[0];
                }

                table += "<td class='width20'>";
                table += address;
                table += "</td>";

                table += "<td class='width10'>";
                table += lst[i].Customer.MobileNumber1;
                table += "</td>";


                table += "<td class='width10'>";
                table += lst[i].OrderTotalPrice;
                table += "</td>";

                table += "<td class='width10'>";
                table += lst[i].ShipmentPrice ? lst[i].ShipmentPrice : "";
                table += "</td>";
                table += "<td class='width15'>";
                table += lst[i].OrderStatusName;
                table += "</td>";
                
                table += "</tr>";
            }

        }
        else {
            table = "<tr class='NoDataText'>";
            table += "<td class='NoDataText text-center' colspan='14'>";
            table += noDataText;
            table += "</td>";
            table += "</tr>";
        }
        $("#" + gridId + " tbody").empty();
        $("#" + gridId + " tbody").html(table);

        RefillSelection();
    }
    TableManager.prototype.FillTodaySales = function (result) {
        var table = "";
        if (result && result.data && result.data.length > 0) {
            var lst = result.data;
            for (var i = 0; i < lst.length; i++) {
                table += "<tr>";
                for (var j = 0; j < columns.length; j++) {

                    if (columns[j].Name == "EDIT" || columns[j].Name == "DELETE") {
                        if (columns[j].Name == "EDIT") {
                            table += "<td class='" + columns[j].Class + "'>";
                            if (lst[i].IsApproved == true) {
                                if (isDetails)
                                    table += "<a class='btn  btn-outline-info' href='" + detailsURL + lst[i].ID + "' ><i class='fa fa-info'></i> " + detailsText + "</a>";
                            }
                            else {
                                if (isUpdate)
                                    table += "<a class='btn  btn-outline-primary' href='" + updateURL + lst[i].ID + "' ><i class='fa fa-edit'></i> " + updateText + "</a>";
                            }
                            table += "</td>";
                        }
                        else {
                            table += "<td class='" + columns[j].Class + "'>";
                            if (isDelete && lst[i].IsApproved == false) {
                                table += "<a href='#' class='btn  btn-outline-danger' onclick='" + deleteURL + "(" + lst[i].ID + ");'><i class='fa fa-trash'></i> " + deleteText + "</a>";
                            }
                            table += "</td>";
                        }
                    }
                    else {
                        table += "<td class='" + columns[j].Class + "'>";
                        table += lst[i][columns[j].Name];
                        table += "</td>";
                    }
                }
                table += "</tr>";
            }
        }
        else {
            table = "<tr class='NoDataText'>";
            table += "<td class='NoDataText text-center' colspan='4'>";
            table += noDataText;
            table += "</td>";
            table += "</tr>";
        }
        $("#" + gridId + " tbody").empty();
        $("#" + gridId + " tbody").html(table);
    }

}
var OrdersManager = function OrdersManager() {
    var self = this;
    OrdersManager.prototype.Init = function () {
        $(window).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
        $("#submit").on("click", function (e) {
            var i = $("#tBody tr").length - 2;
            if (i == 0) {
                CommonManager.prototype.Alert(mustHaveProducts);
                e.preventDefault();
                return false;
            }
            return true;
        });
        $("#addNew").click(function (e) {
            
            e.preventDefault();
            $(".tfooter").hide();
            $(".tFoot").show();
            var i = $("#tBody tr").length - 2;
            var productId = $("#Product_ID option:selected").val();
            var productName = $("#Product_ID option:selected").text();
            var colorId = $("#Product_ColorID option:selected").val();
            var colorName = $("#Product_ColorID option:selected").text();
            var sizeId = $("#Product_SizeID option:selected").val();
            var sizeName = $("#Product_SizeID option:selected").text();
            var employeeName = $("#Product_EmployeeID option:selected").text();
            var numberOfPieces = $("#Product_NumberOfPieces").val();
            var sellingPrice = $("#Product_SellingPrice").val();
            var notes = $("#Product_Notes").val();
            var tr = "<tr>";

            tr += "<input type='hidden' id='Products[" + i + "]_ID' name='Products[" + i + "].ID' value='0'>";
            tr += "<input type='hidden' id='Products[" + i + "]_ProductID' name='Products[" + i + "].ProductID' value='" + productId + "'>";
            tr += "<input type='hidden' id='Products[" + i + "]_ColorID' name='Products[" + i + "].ColorID' value='" + colorId + "'>";
            tr += "<input type='hidden' id='Products[" + i + "]_SizeID' name='Products[" + i + "].SizeID' value='" + sizeId + "'>";
            tr += "<input type='hidden' id='Products[" + i + "]_ColorName' name='Products[" + i + "].ColorName' value='" + colorName + "'>";
            tr += "<input type='hidden' id='Products[" + i + "]_SizeName' name='Products[" + i + "].SizeName' value='" + sizeName + "'>";
            tr += "<input type='hidden' id='Products[" + i + "]_NumberOfPieces' name='Products[" + i + "].NumberOfPieces' value='" + numberOfPieces + "'>";
            tr += "<input type='hidden' id='Products[" + i + "]_SellingPrice' name='Products[" + i + "].SellingPrice' value='" + sellingPrice + "'>";
            tr += "<input type='hidden' id='Products[" + i + "]_Name' name='Products[" + i + "].Name' value='" + productName + "'>";
            tr += '<td class="width20">';
            tr += '<span>' + productName + '</span>';
            tr += "</td>";

            tr += '<td class="width20">';
            tr += '<span>' + colorName + '</span>';
            tr += "</td>";

            tr += '<td class="width20">';
            tr += '<span>' + sizeName + '</span>';
            tr += "</td>";

            tr += "<td class='width10'>";
            tr += '<span>' + numberOfPieces + '</span>';
            tr += "</td>";
  

            tr += "<td class='width10'>";
            tr += '<span>' + sellingPrice + '</span>';
            tr += "</td>";

            tr += "<td class='width20'>";
            tr += '<span>' + sellingPrice * numberOfPieces + currencyLE + '</span>';
            tr += '<input type="hidden" value="' + sellingPrice * numberOfPieces+'" class="TotalValue">';
            tr += "</td>";

            tr += '<td class="width10">';
            tr += '<button type="button" class="btn btn-lg btn-danger" onclick="OrdersManager.prototype.DeleteRow(this);">';
            tr += '<i class="fa fa-remove"></i>';
            tr += '</button>';
            tr += '</td>';

            tr += '</div>';
            tr += '</td>';
            tr += '</tr>';

            $("#tBody").append(tr);

            //$("#Product_NumberOfPieces").val("");
            $("#Product_SellingPrice").val("");
            self.OnChangeTotal();
        });

    }
    OrdersManager.prototype.DeleteRow = function (row) {
        
        $(row).closest("tr").remove();
        var rowsCount = $("#tBody tr").length - 2;
        
        if (rowsCount <= 0) {
            $(".tFoot").hide();
            $(".tfooter").show();
        }
            
        $("#tBody tr").each(function (index, element) {
            index -= 2;
            $(element).find("input,span").each(function (i, elem) {
                
                var id = $(elem).attr("id");
                if (id != undefined) {
                    id = id.replace(/\d+/, index);
                    $(elem).attr("id", id);
                    $(elem).attr("data-valmsg-for", id);
                }
                var name = $(elem).attr("name");
                if (name != undefined) {
                    name = name.replace(/\d+/, index);
                    $(elem).attr("name", name);
                }
            });
        });
        self.OnChangeTotal();
    }
    OrdersManager.prototype.OnChangeProduct = function () {
        var productId = $("#Product_ID option:selected").val();
        $.ajax({
            url: getProductInfoURL,
            data: { productId: productId },
            dataType: "JSON",
            type: "POST",
            success: function (result) {
                
                var colors = [];
                var sizes = [];
                if (result) {
                    colors = result.Colors;
                    sizes = result.Sizes;
                    self.FillSelectOptions("Product_ColorID", colors, true);
                    self.FillSelectOptions("Product_SizeID", sizes,true);
                }
            }
        });
    }
    OrdersManager.prototype.FillSelectOptions = function (id, lst,isSearchable = false) {
        $("#" + id).empty();
        var options = "";
        for (var i = 0; i < lst.length; i++) {
            options += "<option "+(i == 0 ?"selected":"")+" value='" + lst[i].ID + "'>" + lst[i].Name + "</option>";
        }
        $("#" + id).html(options);
      
        if (isSearchable == true)
            $(".SelectDropDownList").select();

        $("#" + id).selectpicker("refresh");
    }
    OrdersManager.prototype.OnChangeTotal = function () {
        
        var total = 0;
        $("#tBody tr").each(function (index, element) {
            
            var value = $(element).find(".TotalValue").val();
            if (value != null && value != "" && value != undefined)
                total += parseFloat(value);
        });
        $("#tFootTotal").html(totalText + total + currencyLE);

        var rowsCount = $("#tBody tr").length - 2;
        if (rowsCount == 0) {
            $(".tFoot").hide();
            $(".tfooter").show();
        }
        else {
            $(".tFoot").show();
            $(".tfooter").hide();
        }
    }
}
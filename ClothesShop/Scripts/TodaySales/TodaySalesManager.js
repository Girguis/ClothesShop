var TodaySalesManager = function TodaySalesManager() {
    var self = this;
    TodaySalesManager.prototype.Init = function () {
        $(window).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });

        $("#addNew").click(function (e)     {
           
            e.preventDefault();
            var i = $("#tBody tr").length;
            var productId = $("#Transaction_ProductID option:selected").val();
            var productName = $("#Transaction_ProductID option:selected").text();
            //var employeeId = $("#Transaction_EmployeeID").val();
            //var employeeName = $("#Transaction_EmployeeID option:selected").text();
            var numberOfPieces = $("#Transaction_NumberOfPieces").val();
            var sellingPrice = $("#Transaction_SellingPrice").val();
            var notes = $("#Transaction_Notes").val();
            var tr = "<tr>";
            tr += "<td>";
            tr += "<div class='form-group col-xs-12 f-right txt-right'>";
            tr += "<input type='hidden' id='Transactions[" + i + "]_ID' name='Transactions[" + i + "].ID' value='0'>";
            tr += "<input type='hidden' id='Transactions[" + i + "]_ProductID' name='Transactions[" + i + "].ProductID' value='" + productId + "'>";
            //tr += "<input type='hidden' id='Transactions[" + i + "]_EmployeeID' name='Transactions[" + i + "].EmployeeID' value='" + employeeId + "'>";
            tr += "<div class='form-group col-xs-12 col-md-5 f-right' style='margin-right: 0px;'>";
            tr += '<div class="col-xs-12 col-md-12 f-right">';
            tr += '<input class="form-control text-box single-line" data-val="true" id="Transactions[' + i + ']_ProductName" name="Transactions[' + i + '].ProductName" type="text" value="' + productName + '" readonly>';
            tr += '<span class="field-validation-valid text-danger col-xs-12 padding0" data-valmsg-for="Transactions[' + i + '].ProductName" data-valmsg-replace="true"></span>';
            tr += "</div>";
            tr += "</div>";

            //tr += "<div class='form-group col-xs-12 col-md-3 f-right'>";
            //tr += '<div class="col-xs-12 col-md-12 f-right">';
            //tr += '<input class="form-control text-box single-line" data-val="true" id="Transactions[' + i + ']_EmployeeName" name="Transactions[' + i + '].EmployeeName" type="text" value="' + employeeName + '" readonly>';
            //tr += '<span class="field-validation-valid text-danger col-xs-12 padding0" data-valmsg-for="Transactions[' + i + '].EmployeeName" data-valmsg-replace="true"></span>';
            //tr += "</div>";
            //tr += "</div>";

            tr += "<div class='form-group col-xs-12 col-md-3 f-right'>";
            tr += '<div class="col-xs-12 col-md-12 f-right">';
            tr += '<input class="form-control text-box single-line" data-val="true" id="Transactions[' + i + ']_NumberOfPieces" name="Transactions[' + i + '].NumberOfPieces" type="number" value="' + numberOfPieces + '">';
            tr += '<span class="field-validation-valid text-danger col-xs-12 padding0" data-valmsg-for="Transactions[' + i + '].NumberOfPieces" data-valmsg-replace="true"></span>';
            tr += "</div>";
            tr += "</div>";

            tr += "<div class='form-group col-xs-12 col-md-3 f-right'>";
            tr += '<div class="col-xs-12 col-md-12 f-right">';
            tr += '<input class="form-control text-box single-line" data-val="true" id="Transactions[' + i + ']_SellingPrice" name="Transactions[' + i + '].SellingPrice" type="number" value="' + sellingPrice + '">';
            tr += '<span class="field-validation-valid text-danger col-xs-12 padding0" data-valmsg-for="Transactions[' + i + '].SellingPrice" data-valmsg-replace="true"></span>';
            tr += "</div>";
            tr += "</div>";

            //tr += "<div class='form-group col-xs-12 col-md-3 f-right'>";
            //tr += '<div class="col-xs-12 col-md-12 f-right">';
            //tr += '<textarea rows="2" class="form-control text-box single-line" data-val="true" id="Transactions[' + i + ']_Notes" name="Transactions[' + i + '].Notes">' + notes +'</textarea>';
            //tr += '<span class="field-validation-valid text-danger col-xs-12 padding0" data-valmsg-for="Transactions[' + i + '].Notes" data-valmsg-replace="true"></span>';
            //tr += "</div>";
            //tr += "</div>";

            tr += '<div class="">';
            tr += '<button type="button" class="btn btn-lg btn-danger" onclick="TodaySalesManager.prototype.DeleteRow(this);">';
            tr += '<i class="fa fa-remove"></i>';
            tr += '</button>';
            tr += '</div>';

            tr += '</div>';
            tr += '</td>';
            tr += '</tr>';

            $("#tBody").append(tr);

            $("#Transaction_NumberOfPieces").val("");
            $("#Transaction_SellingPrice").val("");
            $("#Transaction_Notes").val("");


            // Re-assign Validation
            var form = $("form")
                .removeData("validator")
                .removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse(form);
        });

    }
    TodaySalesManager.prototype.DeleteRow = function (row) {
        
        $(row).closest("tr").remove();

        $("#tBody tr td").each(function (index, element) {

            $(element).find(":input").each(function (i, elem) {
               
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
    }
}
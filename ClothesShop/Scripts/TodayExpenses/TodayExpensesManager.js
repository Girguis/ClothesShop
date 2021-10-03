var TodayExpensesManager = function TodayExpensesManager() {
    var self = this;
    TodayExpensesManager.prototype.Init = function () {
        $(window).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
        $("#addNew").click(function (e) {
           
            e.preventDefault();
            var i = $("#tBody tr").length;
            var name = $("#Expense_Name").val();
            var cost = $("#Expense_Cost").val();
            var tr = "<tr>";
            tr += "<td>";
            tr += "<div class='form-group col-xs-12 f-right txt-right'>";
            tr += "<input type='hidden' id='Expenses[" + i + "]_ID' name='Expenses[" + i + "].ID' value='0'>";
            tr += "<div class='col-xs-12 col-md-5 f-right'>";
            tr += '<input class="form-control text-box single-line" data-val="true" id="Expenses[' + i + ']_Name" name="Expenses[' + i + '].Name" type="text" value="' + name + '">';
            tr += '<span class="field-validation-valid text-danger col-xs-12 padding0" data-valmsg-for="Expenses[' + i + '].Name" data-valmsg-replace="true"></span>';
            tr += "</div>";

            tr += "<div class='col-xs-12 col-md-5 f-right'>";
            tr += '<input class="form-control text-box single-line" data-val="true"  id="Expenses[' + i + ']_Cost" name="Expenses[' + i + '].Cost" type="text" value="' + cost + '">';
            tr += '<span class="field-validation-valid text-danger col-xs-12 padding0" data-valmsg-for="Expenses[' + i + '].Cost" data-valmsg-replace="true"></span>';
            tr += "</div>";

            tr += '<div class="col-xs-12 col-md-2 f-right">';
            tr += '<button type="button" class="btn btn-lg btn-danger" onclick="TodayExpensesManager.prototype.DeleteRow(this);">';
            tr += '<i class="fa fa-remove"></i>';
            tr += '</button>';
            tr += '</div>';

            tr += '</div>';
            tr += '</td>';
            tr += '</tr>';

            $("#tBody").append(tr);

            $("#Expense_Name").val("");
            $("#Expense_Cost").val("");

            // Re-assign Validation
            var form = $("form")
                .removeData("validator")
                .removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse(form);
        });

    }

    TodayExpensesManager.prototype.DeleteRow = function (row) {
        
        $(row).closest("tr").remove();

        $("#tBody tr td").each(function (index, element) {

            var _name = "Expenses[" + index + "].ID";
            var idname = "Expenses_" + index + "__ID";

            $($(element).find("div input")[0]).attr("id", idname);
            $($(element).find("div span")[0]).attr("data-valmsg-for", idname);
            $($(element).find("div input")[0]).attr("name", _name);


            
            _name = "Expenses[" + index + "].Name";
            idname = "Expenses_" + index + "__Name";

            $($(element).find("div input")[1]).attr("id", idname);
            $($(element).find("div span")[1]).attr("data-valmsg-for", idname);
            $($(element).find("div input")[1]).attr("name", _name);

            var cost = "Expenses[" + index + "].Cost";
            var idcost = "Expenses_" + index + "__Cost";

            $($(element).find("div input")[2]).attr("id", idcost);
            $($(element).find("div span")[2]).attr("data-valmsg-for", idcost);
            $($(element).find("div input")[2]).attr("name", cost);
        });
    }

}
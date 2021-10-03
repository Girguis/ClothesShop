var PrintManager = function PrintManager() {
    PrintManager.prototype.Print = function (url) {
        $("#loading").show();
        $.ajax({
            url: url,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                
                var data = rootURL + result;
                printing = window.open(data);
                printing.focus();
                printing.print();
                $("#loading").hide();
            },
            error: function (err) {
                
                $("#loading").hide();

            }
        });
    }
}
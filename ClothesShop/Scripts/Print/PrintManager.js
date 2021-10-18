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
        PrintManager.prototype.Export = function (url) {
        $("#loading").show();
        $.ajax({
            url: url,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                
                var url = rootURL + result;

                const a = document.createElement('a');
                a.style.display = 'none';
                a.href = url;
                // the filename you want
                var t = result.split("/");
                
                a.download = t[t.length - 1];
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                $("#loading").hide();
            },
            error: function (err) {
                $("#loading").hide();
            }
        });
    }

}
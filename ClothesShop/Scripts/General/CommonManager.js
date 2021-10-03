var CommonManager = function CommonManager() {
    var self = this;
    CommonManager.prototype.ConfirmDelete = function (url, jsonData) {
        CommonManager.prototype.Confirm(function (result) {
            if (result) {
                self.AjaxCall(url, jsonData, function (res) {
                    if (res && res == true)
                        window.location.reload();
                    else
                        self.Alert(deleteError);
                        
                });
            }
        });
    }
    CommonManager.prototype.Alert = function (msg) {
        bootbox.alert({
            message: msg
        });
    }
    CommonManager.prototype.Confirm = function (_callback) {
        bootbox.confirm({
            message: msg,
            buttons: {
                confirm: {
                    label: yesText,
                    className: 'btn-lg btn-success'
                },
                cancel: {
                    label: noText,
                    className: 'btn-lg btn-danger'
                }
            },
            callback: function (result) {
                _callback(result);
            }
        });
    }
    CommonManager.prototype.AjaxCall = function (url, data, callback) {
        $.ajax({
            url: url,
            data: data,
            dataType: "JSON",
            type: "POST",
            contentType: "application/json;utf-8",
            success: function (res) {
               
                callback(res);
            },
            error: function (r) {
                callback(false);
            }
        })
    }
}
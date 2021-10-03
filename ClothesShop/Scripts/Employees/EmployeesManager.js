var EmployeesManager = function EmployeesManager() {
    var self = this;
    EmployeesManager.prototype.Read = function (input, id) {
        
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                
                $(id).attr("src", e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        }
    }
    EmployeesManager.prototype.Load = function () {
        $("#FrontSSN").change(function () {
            
            EmployeesManager.prototype.Read(this, "#frontSSNImage");
        });
        $("#BackSSN").change(function () {
            EmployeesManager.prototype.Read(this, "#backSSNImage");
        });
        $("#FrontLicence").change(function () {
            EmployeesManager.prototype.Read(this, "#frontLicenceImage");
        });
        $("#BackLicence").change(function () {
            EmployeesManager.prototype.Read(this, "#backLicenceImage");
        });
    }
    EmployeesManager.prototype.OnChangeJob = function() {
        var selectedJob = $("#JobTypeID option:selected").text();
        if (selectedJob == deliveryMan) {
            $("#LicenceDiv").show();
        }
        else {
            $("#LicenceDiv").hide();
        }

        if (selectedJob == employee) {
            $("#JobNameDiv").show();
        }
        else {
            $("#JobNameDiv").hide();
        }
    }
}
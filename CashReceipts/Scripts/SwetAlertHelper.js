(function(window) {
    var showError = function(msg) {
        swal({ title: "Error!", text: msg, type: "error", confirmButtonText: "Ok" });
    };

    var confirm = function(title, text, callback) {
        swal({
                title: title,
                text: text,
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok",
                closeOnConfirm: true
            },
            function(isConfirm) {
                if (callback)
                    callback(isConfirm);
            });
    };

    var showSuccess = function(msg) {
        swal({ title: "Success!", text: msg, type: "success", confirmButtonText: "Ok" });
    }

    var postConfirm = function() {
        swal({
                title: "Are you sure?",
                text: "You will not be able to recover this imaginary file!",
                type: "success",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Yes, lock receipt!",
                cancelButtonText: "No, cancel!",
                closeOnConfirm: false,
                closeOnCancel: false
            },
            function(isConfirm) {
                if (isConfirm) {
                    swal("Deleted!", "Your imaginary file has been deleted.", "success");
                } else {
                    swal("Cancelled", "Your imaginary file is safe :)", "error");
                }
            });
    }

    window.notify = {
        showError: showError,
        showSuccess: showSuccess,
        confirm: confirm
    };
})(window);
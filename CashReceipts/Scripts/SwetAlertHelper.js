(function (window) {
    var showError = function (msg) {
        swal({ title: "Error!", text: msg, type: "error", confirmButtonText: "Ok" });
    };

    var confirm = function (title, text, callback) {
        swal({
            title: title,
            text: text,
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Ok",
            closeOnConfirm: true
        },
            function (isConfirm) {
                if (callback)
                    callback(isConfirm);
            });
    };

    var showSuccess = function (msg) {
        swal({ title: "Success!", text: msg, type: "success", confirmButtonText: "Ok" });
    }

    var postConfirm = function (msg, preConfirmCallback) {
        swal({
                title: "Success!",
                text: msg,
                type: "success",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "lock receipt",
                cancelButtonText: "Ok",
                closeOnConfirm: false,
                closeOnCancel: true
            },
            function(isConfirm) {
                if (isConfirm) {
                    preConfirmCallback();
                }
            });
    }

    window.notify = {
        showError: showError,
        showSuccess: showSuccess,
        confirm: confirm,
        postConfirm: postConfirm
    };
})(window);
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

    window.notify = {
        showError: showError,
        confirm: confirm
    };
})(window);
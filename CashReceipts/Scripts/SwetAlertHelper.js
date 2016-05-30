(function(window) {
    var showError = function(msg) {
        swal({ title: "Error!", text: msg, type: "error", confirmButtonText: "Ok" });
    };

    window.notify = {
        showError: showError
    };
})(window);
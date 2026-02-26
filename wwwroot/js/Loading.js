(function () {
    window.loadingScreen = function (show) {
        var overlay = document.getElementById('loader-overlay');
        if (!overlay) return;

        if (show) {
            overlay.classList.add('active');
        } else {
            overlay.classList.remove('active');
        }
    };
})();
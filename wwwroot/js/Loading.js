
(function () {
    var _tl = null;

    function getTimeline() {
        if (_tl) return _tl;
        _tl = gsap.timeline({ repeat: -1, yoyo: true, paused: true, defaults: { ease: "sine.inOut" } });
        _tl.to(".mascot-body", { y: -80, scaleY: 1.1, scaleX: 0.95, duration: 0.6 }, "jump")
           .to(".shadow-oval", { scale: 0.5, opacity: 0.2, duration: 0.6 }, "jump")
           .to(".tassel", { rotation: 20, duration: 0.6 }, "jump");
        return _tl;
    }

    window.loadingScreen = function (show) {
        var overlay = document.getElementById('loader-overlay');
        if (!overlay) return;

        overlay.style.visibility = show ? 'visible' : 'hidden';
        overlay.style.opacity = show ? '1' : '0';

        var tl = getTimeline();
        if (show) {
            tl.restart();
        } else {
            tl.pause();
            gsap.set([".mascot-body", ".shadow-oval", ".tassel"], { clearProps: "all" });
        }
    };
})();

window.loadingScreen = function (show) {
    var overlay = document.getElementById('loader-overlay');

    overlay.style.visibility = show ? 'visible' : 'hidden';
    overlay.style.opacity = show ? '1' : '0';

    const tl = gsap.timeline({ repeat: -1, yoyo: true, defaults: { ease: "sine.inOut" } });

    tl.to(".mascot-body", {
        y: -80,
        scaleY: 1.1,
        scaleX: 0.95,
        duration: 0.6
    }, "jump")
        .to(".shadow-oval", {
            scale: 0.5,
            opacity: 0.2,
            duration: 0.6
        }, "jump")
        .to(".tassel", {
            rotation: 20,
            duration: 0.6
        }, "jump");

    tl.play();
}
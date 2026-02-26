(function () {
    const form = document.getElementById('testLoginForm');
    const code = document.getElementById('testId');
    const fb = document.getElementById('codeFeedback');

    function setFeedback(el, feedbackEl, ok, msg) {
        el.classList.remove('valid', 'invalid');
        feedbackEl.classList.remove('show', 'ok', 'err');
        if (msg) {
            el.classList.add(ok ? 'valid' : 'invalid');
            feedbackEl.textContent = msg;
            feedbackEl.classList.add('show', ok ? 'ok' : 'err');
        }
    }

    // Auto-uppercase
    code.addEventListener('input', function () {
        this.value = this.value.toUpperCase();
        const v = this.value.trim();
        if (!v) setFeedback(code, fb, false, '');
        else if (v.length < 4) setFeedback(code, fb, false, 'Kód je příliš krátký');
        else setFeedback(code, fb, true, 'Kód je v pořádku ✓');
    });

    form.addEventListener('submit', function (e) {
        if (!code.value.trim()) {
            setFeedback(code, fb, false, 'Zadejte kód testu');
            e.preventDefault();
            return;
        }
        loadingScreen(true);
    });
})();
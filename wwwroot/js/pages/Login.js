(function () {
    const form = document.getElementById('loginForm');
    const nick = document.getElementById('NickName');
    const pw = document.getElementById('Password');
    const nickFb = document.getElementById('nickFeedback');
    const pwFb = document.getElementById('pwFeedback');

    function setFeedback(el, fb, ok, msg) {
        el.classList.remove('valid', 'invalid');
        fb.classList.remove('show', 'ok', 'err');
        if (msg) {
            el.classList.add(ok ? 'valid' : 'invalid');
            fb.textContent = msg;
            fb.classList.add('show', ok ? 'ok' : 'err');
        }
    }

    nick.addEventListener('input', function () {
        const v = this.value.trim();
        if (!v) setFeedback(nick, nickFb, false, '');
        else if (v.length < 2) setFeedback(nick, nickFb, false, 'Přezdívka je příliš krátká');
        else setFeedback(nick, nickFb, true, 'Vypadá dobře ✓');
    });

    pw.addEventListener('input', function () {
        const v = this.value;
        if (!v) setFeedback(pw, pwFb, false, '');
        else if (v.length < 4) setFeedback(pw, pwFb, false, 'Heslo musí mít alespoň 4 znaky');
        else setFeedback(pw, pwFb, true, 'Heslo je v pořádku ✓');
    });

    form.addEventListener('submit', function (e) {
        let ok = true;
        if (!nick.value.trim()) { setFeedback(nick, nickFb, false, 'Přezdívka je povinná'); ok = false; }
        if (!pw.value) { setFeedback(pw, pwFb, false, 'Heslo je povinné'); ok = false; }
        if (!ok) { e.preventDefault(); return; }
        loadingScreen(true);
    });

    // Toggle password visibility
    document.querySelector('.auth-toggle-pw')?.addEventListener('click', function () {
        const input = this.closest('.auth-input-wrap').querySelector('input');
        const icon = this.querySelector('i');
        if (input.type === 'password') {
            input.type = 'text';
            icon.classList.replace('bi-eye', 'bi-eye-slash');
        } else {
            input.type = 'password';
            icon.classList.replace('bi-eye-slash', 'bi-eye');
        }
    });
})();
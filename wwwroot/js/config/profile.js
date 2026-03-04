document.getElementById('btnSaveProfile')?.addEventListener('click', async function () {
    const btn = this;
    const form = document.getElementById('profileForm');

    const firstName = form.querySelector('#profileFirstName').value.trim();
    const lastName = form.querySelector('#profileLastName').value.trim();
    const nickName = form.querySelector('#profileNickName').value.trim();
    const newPassword = form.querySelector('#profileNewPassword').value;

    if (!firstName || !lastName || !nickName) {
        toastr.error('Jméno, příjmení a přezdívka jsou povinné.');
        return;
    }

    btn.disabled = true;
    loadingScreen(true);
    try {
        const response = await fetch('/api/config/profile', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
            body: JSON.stringify({
                firstName: firstName,
                lastName: lastName,
                nickName: nickName,
                newPassword: newPassword || null
            })
        });

        if (!response.ok) {
            const text = await response.text();
            toastr.error(text);
            return;
        }

        const data = await response.json();
        toastr.success(data.message);
        form.querySelector('#profileNewPassword').value = '';

        // Update all visible name elements in the DOM
        if (data.name) {
            document.querySelectorAll('.config-sidebar-name, .TeacherName').forEach(el => {
                // TeacherName also contains a .Nickname span — preserve it
                var nickSpan = el.querySelector('.Nickname');
                if (nickSpan) {
                    el.childNodes.forEach(n => { if (n.nodeType === 3) n.textContent = ''; });
                    el.insertBefore(document.createTextNode(data.name + ' '), nickSpan);
                } else {
                    el.textContent = data.name;
                }
            });
            // Navbar user name
            document.querySelectorAll('.user-name').forEach(el => el.textContent = data.name);
            // Mobile config topbar name
            var mobileNameEl = document.querySelector('.config-mobile-topbar strong');
            if (mobileNameEl) mobileNameEl.textContent = data.name;

            // Profile header name
            var profileH5 = document.getElementById('profileHeaderName');
            if (profileH5) profileH5.textContent = data.name;

            // Update initials avatar
            var parts = data.name.split(' ');
            var initials = parts.map(function(p) { return p.charAt(0); }).join('').substring(0, 2).toUpperCase();
            var avatarEl = document.querySelector('#modalContainer .rounded-circle');
            if (avatarEl) avatarEl.textContent = initials;
        }
        if (data.nickname) {
            document.querySelectorAll('.config-sidebar-nick, .Nickname').forEach(el => el.textContent = data.nickname);
            var mobileNickEl = document.querySelector('.config-mobile-topbar small');
            if (mobileNickEl) mobileNickEl.textContent = data.nickname;
            var profileNickEl = document.getElementById('profileHeaderNick');
            if (profileNickEl) profileNickEl.textContent = data.nickname;
        }
    } catch (error) {
        console.error(error);
        toastr.error('Nastala chyba při ukládání profilu.');
    } finally {
        btn.disabled = false;
        loadingScreen(false);
    }
});
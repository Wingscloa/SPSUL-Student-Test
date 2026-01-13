$(document).on('click', '.menuItem', function () {
    console.log("kliknuto");
    $(".menuItem").each(function () {
        $(this).removeClass("active");
    });
    $(this).addClass("active");
});

// diagnostics
window.__configJsLoaded = true;
console.log('[configuration.js] loaded');

function initTitlesSelect2(scope) {
    const $scope = scope ? $(scope) : $(document);
    const $selects = $scope.find('#EditTitles, #EditTitlesMobile, .js-example-basic-multiple');
    if ($selects.length === 0) return;

    $selects.each(function () {
        const $sel = $(this);
        if ($sel.hasClass('select2-hidden-accessible')) return;

        // Prefer dropdown inside config modal content to avoid z-index issues
        const dropdownParent = $('#configModal').length ? $('#configModal') : $(document.body);

        $sel.select2({
            theme: 'bootstrap-5',
            width: '100%',
            dropdownAutoWidth: true,
            placeholder: 'Vyberte tituly...',
            allowClear: true,
            closeOnSelect: false,
            dropdownParent: dropdownParent
        });
    });
}

// Re-init select2 whenever a config section is swapped in via loadConfig()
document.addEventListener('DOMNodeInserted', function (e) {
    const target = e.target;
    if (target && target.id === 'modalContainer') {
        initTitlesSelect2(target);
    }
});

// Mobile teacher editor (offcanvas) - delegated handler because ConfigTeacher is loaded via fetch
(function () {
    function fillEditor(data) {
        const setVal = (id, val) => {
            const el = document.getElementById(id);
            if (el) el.value = val || '';
        };
        setVal('EditFirstName', data.first);
        setVal('EditLastName', data.last);
        setVal('EditNickName', data.nick);
        setVal('EditFirstNameMobile', data.first);
        setVal('EditLastNameMobile', data.last);
        setVal('EditNickNameMobile', data.nick);
    }

    function showTeacherOffcanvas(row) {
        console.log('[offcanvas] teacher row click', row);

        const first = row.getAttribute('data-first');
        const last = row.getAttribute('data-last');
        const nick = row.getAttribute('data-nick');
        fillEditor({ first, last, nick });

        initTitlesSelect2(document.getElementById('ctEditor'));

        // Only on mobile
        if (!window.matchMedia('(max-width: 991.98px)').matches) {
            console.log('[offcanvas] not mobile, skipping offcanvas');
            return;
        }

        const offcanvasEl = document.getElementById('ctEditor');
        if (!offcanvasEl) {
            console.warn('[offcanvas] #ctEditor not found in DOM');
            return;
        }
        if (!window.bootstrap || !bootstrap.Offcanvas) {
            console.warn('[offcanvas] bootstrap.Offcanvas not available');
            return;
        }

        offcanvasEl.style.zIndex = '11060';

        const instance = bootstrap.Offcanvas.getOrCreateInstance(offcanvasEl);
        instance.show();

        setTimeout(() => {
            document.querySelectorAll('.offcanvas-backdrop.show').forEach(b => b.style.zIndex = '11050');
            document.body.classList.add('modal-open');
        }, 0);
    }

    document.addEventListener('click', function (e) {
        const row = e.target.closest('#teacherTable .teacher-row');
        if (!row) return;
        showTeacherOffcanvas(row);
    });

    document.addEventListener('keydown', function (e) {
        if (e.key !== 'Enter' && e.key !== ' ') return;
        const row = e.target.closest('#teacherTable .teacher-row');
        if (!row) return;
        e.preventDefault();
        showTeacherOffcanvas(row);
    });
})();
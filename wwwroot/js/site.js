window.onload = () => {
    var select2elements = document.querySelectorAll('.select2');

    select2elements.forEach(function (element) {
        $(element).select2({
            theme: "bootstrap-5",
            width: '100%',
        })
    });
}

async function handleResponse(response) {
    const data = await response.json();

    if (response.ok) {
        toastr.success(data.message || "Akce proběhla úspěšně");
    } else {
        toastr.error(data.message || "Něco se nepovedlo");
    }
}

// Toastr config must run after toastr.js is loaded
if(typeof toastr !== 'undefined'){
  toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
  };
  console.log('[toastr] configured with options:', toastr.options);
} else {
  console.error('[toastr] library not loaded - check if toastr.min.js is included before site.js');
}

async function loadConfig(componentName) {
    const container = document.getElementById("modalContainer")
    container.innerHTML = "Načítám...";
    const response = await fetch(`/api/config/section/${componentName}`);
    const html = await response.text();
    container.innerHTML = html;
    $('.js-example-basic-multiple').select2({
        theme: "bootstrap-5",
        width: '100%',
        placeholder: "Vyberte tituly...",
        allowClear: true,
        dropdownParent: container,
        closeOnSelect: false
    });
}

async function openTeacherDetail(teacherId) {
    const myModal = new bootstrap.Modal(document.getElementById('teacherDetailModal'));
    myModal.show();

    //try {
    //    const response = await fetch(`/Configuration/GetTeacherDetail?id=${teacherId}`);
    //    const html = await response.text();

    //    document.getElementById('teacherModalContent').innerHTML = html;

    //    $('#teacherModalContent').find('.js-example-basic-multiple').select2({
    //        theme: "bootstrap-5",
    //        width: '100%',
    //        dropdownParent: $('#teacherDetailModal')
    //    });

    //} catch (error) {
    //    console.error("Chyba při načítání detailu:", error);
    //    document.getElementById('teacherModalContent').innerHTML = '<div class="p-4 text-danger">Nepodařilo se načíst data.</div>';
    //}
}

document.addEventListener('click', function(e){
  const trigger = e.target.closest('.profile');
  if(!trigger) return;

  const modalEl = document.getElementById('configModal');
  if(!modalEl) return;

  if (window.bootstrap && bootstrap.Modal) {
    const instance = bootstrap.Modal.getOrCreateInstance(modalEl);
    instance.show();
  } else {
    modalEl.classList.add('show');
    modalEl.style.display = 'block';
    modalEl.removeAttribute('aria-hidden');
    document.body.classList.add('modal-open');
  }
});

(function(){
  document.addEventListener('shown.bs.offcanvas', function(e){
    const off = e.target;
    if(!off) return;
    const modalOpen = document.querySelector('.modal.show');
    if(!modalOpen) return;

    off.style.zIndex = '11060';

    const backdrops = Array.from(document.querySelectorAll('.offcanvas-backdrop.show'));
    backdrops.forEach(b => b.style.zIndex = '11050');

    document.body.classList.add('modal-open');
  });
})();

(function(){
  if(!window.gsap) return;

  const modalEl = document.getElementById('configModal');
  if(!modalEl) return;

  modalEl.addEventListener('show.bs.modal', function(){
    const dialog = this.querySelector('.modal-dialog');
    const content = this.querySelector('.modal-content');
    if(!dialog || !content) return;

    gsap.fromTo(dialog, 
      { scale: 0.8, opacity: 0 },
      { scale: 1, opacity: 1, duration: 0.4, ease: 'back.out(1.3)' }
    );

    gsap.fromTo(content,
      { y: -30, opacity: 0 },
      { y: 0, opacity: 1, duration: 0.35, ease: 'power2.out', delay: 0.05 }
    );
  });

  modalEl.addEventListener('hide.bs.modal', function(){
    const dialog = this.querySelector('.modal-dialog');
    if(!dialog) return;

    gsap.to(dialog, { scale: 0.85, opacity: 0, duration: 0.25, ease: 'power2.in' });
  });

  const offcanvasEl = document.getElementById('ctEditor');
  if(!offcanvasEl){
    console.warn('[GSAP] #ctEditor not found for animation');
    return;
  }

  offcanvasEl.addEventListener('show.bs.offcanvas', function(){
    gsap.fromTo(this, 
      { x: -320, opacity: 0 },
      { x: 0, opacity: 1, duration: 0.4, ease: 'power3.out' }
    );
  });

  offcanvasEl.addEventListener('hide.bs.offcanvas', function(){
    gsap.to(this, { x: -320, opacity: 0, duration: 0.3, ease: 'power2.in' });
  });
})();


function hideMode() {
    const elements = document.querySelectorAll('.mode')
    elements.forEach(function (e) { e.style.display = 'none'; });
}

function showMode() {
    const elements = document.querySelectorAll('.mode')
    elements.forEach(function (e) { e.style.display = 'inline-block'; });
}

function showOptions() {
    const elements = document.querySelectorAll('.option')
    elements.forEach(function (e) { e.style.display = 'inline-block'; });
}
function hideOptions() {
    const elements = document.querySelectorAll('.option')
    elements.forEach(function (e) { e.style.display = 'none'; });
}

activatedMode = ''
function setMode(name) {
    activatedMode = name
    var modes = ['edit', 'delete', 'deactivate','activate']
    modes = modes.filter(m => m !== name);

    modes.forEach(function (m) {  
        var button = m + 'Button'
        const elements = document.querySelectorAll('.'+ m + 'Button')
        elements.forEach(function (e) { e.classList.add('d-none') });
        const header = document.getElementById(m + 'Header')
        header.classList.add('d-none')
    });

    const elements = document.querySelectorAll('.' + name + 'Button')
    elements.forEach(function (e) { e.classList.remove('d-none')});
    const header = document.getElementById(name + 'Header')
    header.classList.remove('d-none')
}

function resetRows() {
    const rows = document.querySelectorAll('tr.table-danger, tr.table-warning')
    rows.forEach(function (row) { arguments[0].classList.remove('table-danger', 'table-warning'); });
    document.getElementById('Ids').value = '';
    setMode('edit');
}

async function rowFetch(endpoint, method, ids, failMsg, scsMsg) {
    try {
        loadingScreen(true);
        const response = await fetch(endpoint, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(ids)
        })

        const result = await response.json();

        if (response.ok) {
            toastr.success(result.message || 'Otázka je aktualizovaná!', 'Úspěch');
        } else {
            toastr.error(result.message || 'Chyba při aktualizaci otázky', 'Chyba');
        }

        loadingScreen(false);
        location.reload()
    } catch (err) {
        loadingScreen(false)
        toastr.error('Chyba při komunikaci se serverem', 'Chyba');
        console.error(err);
    }
}


function deactivateRows(endpoint) {
    alert("Deactivate rows: " + endpoint);
}


// Row selection for delete /deactivate actions
document.addEventListener('click', function (e) {
    const deleteMode = e.target.closest('#deleteMode');

    if (deleteMode) {
        hideMode()
        showOptions();
        setMode('delete');
    }

    const deactivateMode = e.target.closest('#deactivateMode');

    if (deactivateMode) {
        hideMode()
        showOptions();
        setMode('deactivate');
    }

    const activate = e.target.closest("#activate");

    if (activate) {
        hideMode();
        showOptions();
        setMode('activate');
    }

    const cancelOption = e.target.closest('#cancelOption');

    if (cancelOption) {
        showMode()
        hideOptions();
        resetRows();
    }

    const acceptOption = e.target.closest('#acceptOption');

    if (acceptOption) {
        var ids = document.getElementById('Ids').value.split(';').map(id => parseInt(id)).filter(id => !isNaN(id));
        if (activatedMode == 'delete') {
            var scsMsg = 'Otázka byla úspěšně smazány';
            var failMsg = 'Otázka byla neúspěšně smazány';
            rowFetch('/Question/Delete/', 'POST', ids, failMsg, scsMsg);
        }
        else if (activatedMode == 'deactivate') {
            var scsMsg = 'Otázka byla úspěšně deaktivována';
            var failMsg = 'Otázka byla neúspěšně deaktivována';
            rowFetch('/Question/Deactivate/', 'PUT', ids, failMsg, scsMsg);
        }
        else if (activatedMode == 'activate') {
            var scsMsg = 'Otázka byla úspěšně aktivována';
            var failMsg = 'Otázka byla neúspěšně aktivována';
            rowFetch('/Question/Activate/', 'PUT', ids, failMsg, scsMsg);

        }
        showMode()
        hideOptions();
        resetRows();
    }

    const deactivateBtn = e.target.closest('.btn-deactivate');
    const deleteBtn = e.target.closest('.btn-delete');
    const activateBtn = e.target.closest('.btn-activate');

    if (deleteBtn) {
        const row = deleteBtn.closest('tr');
        addIdFromRow(row, 'table-danger');
    }

    if (deactivateBtn) {
        const row = deactivateBtn.closest('tr');
        addIdFromRow(row, 'table-warning');
    }

    if (activateBtn) {
        const row = activateBtn.closest('tr');
        addIdFromRow(row, 'table-success');
    }

})

function addIdFromRow(row, className) {
    var ids = document.getElementById('Ids').value.split(';');
    var rowId = row.getAttribute('data-id');

    if (row.classList.contains(className)) {
        row.classList.remove(className)
        ids = ids.filter(id => id !== rowId);
    }
    else {
        row.classList.add(className)
        ids.push(rowId)
    }
    document.getElementById('Ids').value = ids.filter(id => id).join(';');
    console.log(document.getElementById('Ids').value.split(';'))
}
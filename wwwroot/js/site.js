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
        // Tady zachytíš chyby z API (např. 401 Unauthorized)
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
    var modes = ['edit', 'delete','deactivate']
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
    document.getElementById('deleteIds').value = '';
    document.getElementById('deactivateIds').value = '';
    setMode('edit');
}

function deleteRows(endpoint) {
    alert("Delete rows: " + endpoint);
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

    const cancelOption = e.target.closest('#cancelOption');

    if (cancelOption) {
        showMode()
        hideOptions();
        resetRows();
        if (activatedMode == 'delete') {

        }
    }

    const acceptOption = e.target.closest('#acceptOption');

    if (acceptOption) {
        showMode()
        hideOptions();
        resetRows();
        if (activatedMode == 'delete') {
            deleteRows(deleteMode.getAttribute('data-endpoint'));
        }
        else if (activatedMode == 'deactivate') {
            deactivateRows(deactivateMode.getAttribute('data-endpoint'));
        }
    }

    const deactivateBtn = e.target.closest('.btn-deactivate');
    const deleteBtn = e.target.closest('.btn-delete');

    if (deleteBtn) {
        const row = deleteBtn.closest('tr');
        var ids = document.getElementById('deleteIds').value.split(';');
        var rowId = row.getAttribute('data-id');

        if (row.classList.contains('table-danger')) {
            row.classList.remove('table-danger')
            ids = ids.filter(id => id !== rowId);
        }
        else {
            var ids = document.getElementById('deleteIds').value.split(';');
            row.classList.add('table-danger')
            ids.push(rowId)
        }
        document.getElementById('deleteIds').value = ids.filter(id => id).join(';');
        console.log("To be deleted IDs:", document.getElementById('deleteIds').value.split(';'));
    }

    if (deactivateBtn) {
        const row = deactivateBtn.closest('tr');
        var ids = document.getElementById('deactivateIds').value.split(';');
        var rowId = row.getAttribute('data-id');

        if (row.classList.contains('table-warning')) {
            row.classList.remove('table-warning')
            ids = ids.filter(id => id !== rowId);
        }
        else {
            row.classList.add('table-warning')
            ids.push(rowId)
        }
        document.getElementById('deactivateIds').value = ids.filter(id => id).join(';');
        console.log("To be deactivate IDs:", document.getElementById('deactivateIds').value.split(';'));
    }
})
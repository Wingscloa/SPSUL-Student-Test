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

// Ensure profile click always opens config modal (fallback to BS API)
document.addEventListener('click', function(e){
  const trigger = e.target.closest('.profile');
  if(!trigger) return;

  const modalEl = document.getElementById('configModal');
  if(!modalEl) return;

  // If Bootstrap is available, open programmatically
  if (window.bootstrap && bootstrap.Modal) {
    const instance = bootstrap.Modal.getOrCreateInstance(modalEl);
    instance.show();
  } else {
    // last resort: toggle classes (won't create backdrop)
    modalEl.classList.add('show');
    modalEl.style.display = 'block';
    modalEl.removeAttribute('aria-hidden');
    document.body.classList.add('modal-open');
  }
});

// When offcanvas is opened while a modal is open, ensure it stacks above the modal
(function(){
  document.addEventListener('shown.bs.offcanvas', function(e){
    const off = e.target;
    if(!off) return;
    const modalOpen = document.querySelector('.modal.show');
    if(!modalOpen) return;

    // Raise offcanvas above modal/backdrop
    off.style.zIndex = '11060';

    const backdrops = Array.from(document.querySelectorAll('.offcanvas-backdrop.show'));
    backdrops.forEach(b => b.style.zIndex = '11050');

    // Keep body in modal-open state so background doesn't scroll
    document.body.classList.add('modal-open');
  });
})();

// GSAP animations for config modal and offcanvas
(function(){
  if(!window.gsap) return;

  const modalEl = document.getElementById('configModal');
  if(!modalEl) return;

  // Config modal GSAP animation
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

  // Offcanvas GSAP animation
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
var onMobile = false;

document.addEventListener('DOMContentLoaded', function () {
    onMobile = window.innerWidth < 992;
})

window.addEventListener('resize', () => {
    onMobile = window.innerWidth < 992;
});

// Sidebar nav: highlight active
$(document).on('click', '#configModal .config-nav-item:not(.config-logout)', function () {
    $('#configModal .config-nav-item').removeClass('active');
    $(this).addClass('active');
});

// Close any open Select2 dropdowns when config modal opens
$(document).on('show.bs.modal', '#configModal', function () {
    $('.select2-hidden-accessible').each(function () {
        if ($(this).data('select2')) {
            $(this).select2('close');
        }
    });
});

// Mobile tab: highlight active
$(document).on('click', '.config-tab', function () {
    $('.config-tab').removeClass('active');
    $(this).addClass('active');
});
function initTitlesSelect2() {
    $selects = $(".select2")
    $selects.each(function () {
        const $sel = $(this);
        const dropdownParent = $sel.closest('.offcanvas').length
            ? $sel.closest('.offcanvas')
            : ($('#configModal').length ? $('#configModal') : $(document.body));

        const placeholder = $(this).data('v-placeholder')

        $sel.select2({
            theme: 'bootstrap-5',
            width: '100%',
            dropdownAutoWidth: true,
            dropdownParent: dropdownParent,
            placeholder: placeholder
        });
    });
}

async function loadConfig(componentName, clickedEl) {
    const container = document.getElementById("modalContainer")
    container.innerHTML = "Načítám...";
    const response = await fetch(`/api/config/section/${componentName}`);
    const html = await response.text();
    container.innerHTML = html;
    initTitlesSelect2()
}

(function () {
function showEditorOffcanvas() {
    const offcanvasEl = document.getElementById('ctEditor');
    // Ensure offcanvas renders above the fullscreen modal (z-index 1055)
    offcanvasEl.style.zIndex = '1075';
    const instance = bootstrap.Offcanvas.getOrCreateInstance(offcanvasEl);
    instance.show();
    // Push backdrop above the modal too
    setTimeout(() => {
        document.querySelectorAll('.offcanvas-backdrop').forEach(b => {
            b.style.zIndex = '1070';
        });
    }, 10);
}

// Handle clicks 

document.addEventListener('click', function (e) {
    const cancel = e.target.closest('#btnCancelDesktop');
    if (!cancel) return;
    const offcanvasEl = document.getElementById('ctEditor');
    const instance = bootstrap.Offcanvas.getInstance(offcanvasEl);
    if (instance) instance.hide();
})

    document.addEventListener('click', async function (e) {
        const submit = e.target.closest(".configSubmit")
        if (!submit) return;

        try {
            const button = submit.closest('button[data-form]') || submit;
            const formId = button.getAttribute('data-form');

            if (!formId || !document.getElementById(formId)) {
                toastr.error('Formulář nebyl nalezen.');
                return;
            }

            const isValid = validateDataAttributes(formId);
            if (!isValid) { return; }

            const data = getDataFromForm(formId);
            const crudEl = document.getElementById('crudURL');
            if (!crudEl || !crudEl.value) {
                toastr.error('URL pro uložení nebyla nalezena. Zkuste znovu načíst sekci.');
                return;
            }
            const url = crudEl.value;

            if (editModeOn) {
                await fetchDataForm(url, data, 'PUT');
                await filterForm();
            } else {
                await fetchDataForm(url, data, 'POST');
                await filterForm();
            }
        } catch (error) {
            loadingScreen(false);
            console.error('Config save error:', error);
            toastr.error('Nastala chyba při ukládání. Zkuste to znovu.');
        }
    })

    document.addEventListener('click', async function (e) {
        const row = e.target.closest('#table .configRow');
        if (!row) return;
        console.log(deleteMode)
        if (deleteMode == false) {
            editMode()
            const url = document.getElementById('EditForm').value;
            const id = row.getAttribute('data-id');
            await fetchForm(url, id);
            showEditorOffcanvas();

            initTitlesSelect2();
        }
        else {
            const element = document.getElementById('deleteIds');
            row.classList.add('is-deleting')
            const id = row.getAttribute('data-id');
            let currentIds = element.value ? element.value.split(';').map(i => i.trim()) : [];
            if (currentIds.includes(id)) {
                currentIds = currentIds.filter(i => i !== id);
                row.classList.remove('is-deleting');
            }
            else {
                currentIds.push(id);
            }
            console.log(currentIds);
            element.value = currentIds.join(';');
        }
    });

    function cancelDeleteMode() {
        const tr = document.querySelectorAll('.configRow');

        tr.forEach(row => {
            row.classList.remove('deleteMode','is-deleting');
        })

        const iconDelete = document.querySelectorAll('.iconDelete');
        iconDelete.forEach(e => { e.classList.add('d-none'); })
        const iconEdit = document.querySelectorAll('.iconEdit');
        iconEdit.forEach(e => { e.classList.remove('d-none'); })

        document.getElementById('deleteIds').value = '';
        deleteMode = false;
    }
    function showConfigMods(state) {
        const configmods = document.querySelector("#configMods")

        if (state) {
            configmods.classList.remove("d-none")
        }
        else {
            configmods.classList.add("d-none")
        }
    }

    function showConfigOptions(state) {
        const configOptions = document.querySelector("#configOptions")
        if (state) {
            configOptions.classList.remove("d-none")
        }
        else {
            configOptions.classList.add("d-none")
        }
    }

    document.addEventListener('click', async function (e) {
        const confirm = e.target.closest("#configAccept")
        if (!confirm) return;

        const idsElement = document.getElementById('deleteIds').value;
        let obj = idsElement ? idsElement.split(';').map(i => Number(i.trim())) : []

        const url = document.getElementById('DeleteUrl').value;

        await fetchDataForm(url, obj, 'POST');
        await filterForm()

        cancelDeleteMode()
        showConfigMods(true);
        showConfigOptions(false);
    })

    document.addEventListener('click', function (e) {
        const cancel = e.target.closest("#configCancel")
        if (!cancel) return;
        showConfigMods(true);
        showConfigOptions(false);

        cancelDeleteMode();
    })

    let deleteMode = false;
    document.addEventListener('click', function (e) {
        const deletebtn = e.target.closest("#btnDelete")
        if (!deletebtn) return;
        if (deleteMode) return;

        deleteMode = true; 

        var elements = document.querySelectorAll('.configRow');
        elements.forEach(element => {
            element.classList.add('deleteMode');
        });

        const editIcon = document.querySelectorAll('.iconEdit');
        editIcon.forEach(e => {
            e.classList.add('d-none');
        })

        const deleteIcon = document.querySelectorAll('.iconDelete');
        deleteIcon.forEach(e => {
            e.classList.remove('d-none');
        })

        showConfigMods(false);
        showConfigOptions(true);
    });

    document.addEventListener('click', async function (e) {
        const submit = e.target.closest("#btnCreate")
        if (!submit) return;
        createMode();
        const url = document.getElementById('CreateForm').value;
        await fetchForm(url);
        showEditorOffcanvas();
        initTitlesSelect2()
    })

    let editModeOn = true;
    function createMode() {
        if (editModeOn === false) return;
        editModeOn = false;

        const elHeader = document.getElementById('ctEditorLabel');
        if (elHeader) elHeader.textContent = 'Vytvořit nový záznam';
    }
    function editMode() {
        if (editModeOn === true) return;
        editModeOn = true;

        const elHeader = document.getElementById('ctEditorLabel');
        if (elHeader) elHeader.textContent = 'Editace záznamu';
    }

    function getDataFromForm(formId) {
        const form = document.getElementById(formId)
        const data = new FormData(form);
        const obj = {};
        data.forEach((value, key) => {
            if (key == 'SearchFilter') {
                obj[key] = value;
                return;
            }

            var allValues = data.getAll(key);

            if (allValues.length > 1 || key.includes('Ids')) {
                allValues = allValues.filter(v => v !== "");
                obj[key] = allValues.map(v => Number(v));
            } else {
                if (value === "") {
                    obj[key] = null;
                }
                else if (!isNaN(value) && value.trim() !== "") {
                    obj[key] = Number(value);
                }
                else if (value === "on") {
                    obj[key] = true;
                }
                else if (value === "off") {
                    obj[key] = false;
                }
                else {
                    obj[key] = value;
                }
            }
        });
        return obj;
    }

    async function fetchDataForm(url, data, method) {
        loadingScreen(true);
        inputDisable(true);
        try {
            const response = await fetch(url, {
                method: method,
                headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
                body: JSON.stringify(data)
            });

            const text = await response.text();
            inputDisable(false);
            loadingScreen(false);

            if (!response.ok) {
                toastr.error(text);
                return;
            }

            toastr.success(text);

            // Zavřít offcanvas po úspěšném uložení
            const offcanvasEl = document.getElementById('ctEditor');
            const instance = bootstrap.Offcanvas.getInstance(offcanvasEl);
            if (instance) instance.hide();
        }
        catch (error) {
            inputDisable(false);
            loadingScreen(false);
            console.error('Error during fetch:', error);
            toastr.error('Nastala chyba během odesílání dat. Pokud problém přetrváva kontaktujte administrátora.');
        }
    }

    async function fetchForm(url, Id) {
        try {
            const myUrl = url + (Id != null ? Id : "");

            const response = await fetch(myUrl);

            const data = await response.text();

            const editBody = document.getElementById('formDesktop');

            editBody.innerHTML = data;
            toastr.success('Podařilo se načíst formulář.');
        }
        catch (error) {
            toastr.error('Nepodařilo se načíst formulář.');
            console.error('Error fetching edit form:', error);
        }
    }

    function inputDisable(state) {
        inputs = getInputs();
        inputs.forEach(input => { input.disabled = state });
    }
    function getInputs() {
        const inputs = document.querySelectorAll('#formDesktop input, #formDesktop select, #formDesktop textarea');
        return inputs;
    }

    let filterTimeout; 
    let filterToast

    $(document).on('change', '#filterForm', 'focusout', function (e) {
        const target = $(e.relatedTarget);
        const isInsideForm = $(this).has(e.relatedTarget).length > 0;
        const isSelect2 = target.closest('.select2-container').length > 0;
        if (isInsideForm || isSelect2) {
            return;
        }

        filterToast = toastr.info('Filtrace se spustí za 1 sekundy...', 'Čekám...', {
            timeOut: 1000,
            progressBar: true,
            closeButton: false,
            preventDuplicates: true
        });

        clearTimeout(filterTimeout);

        filterTimeout = setTimeout(() => {
            $('#filterForm').find(':focus').blur();
            filterForm();
        }, 1000);
    })

    $(document).on('input', '#filterForm', 'focus', function () {
        if (filterToast) { toastr.clear(filterToast); };
    })

    async function filterForm() {
        const data = getDataFromForm('filterForm');
        const url = document.getElementById('filterUrl').value;
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
                body: JSON.stringify(data)
            });

            const text = await response.text();

            const tableBody = document.getElementById('list');
            tableBody.innerHTML = text;
        }
        catch (error) {
            console.error('Error during fetch:', error);
            toastr.error('Nastala chyba během odesílání dat. Pokud problém přetrváva kontaktujte administrátora.');
        }
    }

    //<input type="text" name="FirstName"
    //    data-v-name="Jméno"
    //    data-v-required="true"
    //    data-v-reg="^[a-zA-Zá-žÁ-Ž ]+$"
    //    data-v-msg="Jméno musí obsahovat pouze písmena a být vyplněno."
    //    placeholder="Jméno">

    // Validace
    function validateDataAttributes(formId) {
        const form = document.getElementById(formId);
        // Výběr všech relevantních prvků (včetně selectů)
        const inputs = form.querySelectorAll('input[data-v-reg], input[data-v-required], input[data-v-length], select[data-v-required], select[data-v-length]');
        let isValid = true;

        // Reset chyb
        inputs.forEach(input => {
            input.classList.remove('is-invalid');
            // Speciální reset pro Select2
            const s2Container = input.nextElementSibling;
            if (s2Container && s2Container.classList.contains('select2-container')) {
                s2Container.classList.remove('is-invalid');
            }
        });

        inputs.forEach(input => {
            let value;
            // Zjištění hodnoty u multi-selectu vs klasického inputu
            if (input.tagName === 'SELECT' && input.multiple) {
                // Získá pole vybraných hodnot
                value = Array.from(input.selectedOptions).map(opt => opt.value).filter(v => v !== "");
            } else {
                value = input.value.trim();
            }

            const isRequired = input.dataset.vRequired === 'true';
            const lengthRule = input.dataset.vLength;
            const regexStr = input.dataset.vReg;
            const name = input.dataset.vName || "Pole";
            const errorMsg = input.dataset.vMsg || "Chybný vstup";

            // 1. Kontrola povinnosti (u pole i u multi-selectu)
            const isEmpty = Array.isArray(value) ? value.length === 0 : value === "";
            if (isRequired && isEmpty) {
                showError(errorMsg, name, input);
                isValid = false;
                return;
            }

            // 2. Kontrola délky (u textu počet znaků, u selectu počet vybraných položek)
            if (lengthRule && !isEmpty) {
                const parts = lengthRule.split(',');
                const min = parseInt(parts[0]);
                const max = parts[1] ? parseInt(parts[1]) : null;
                const currentCount = Array.isArray(value) ? value.length : value.length;

                if (currentCount < min || (max !== null && currentCount > max)) {
                    const lengthError = max
                        ? `${name}: Vyberte/zadejte ${min} až ${max} položek.`
                        : `${name}: Minimální počet je ${min}.`;
                    showError(lengthError, name, input);
                    isValid = false;
                    return;
                }
            }

            // 3. Kontrola regexem (jen pro textové hodnoty)
            if (regexStr && !Array.isArray(value) && value !== "") {
                const regex = new RegExp(regexStr);
                if (!regex.test(value)) {
                    showError(errorMsg, name, input);
                    isValid = false;
                }
            }
        });

        return isValid;
    }

    function showError(msg, name, element) {
        toastr.error(msg, name);
        element.classList.add('is-invalid');

        const s2Container = element.nextElementSibling;
        if (s2Container && s2Container.classList.contains('select2-container')) {
            s2Container.classList.add('is-invalid');
        }
    }

    // dynamic events on elements like inputs, focus etc.
    document.addEventListener('input', function (event) {
        const el = event.target

        if (el.classList.contains('onlyYear')) {
            const value = event.target.value
            
            if (value.length > 4) {
                event.target.value = event.target.value.slice(0, 4);
            }

           
        }
    })

        //uzivatel klikne na input
    document.addEventListener('focusin', function (event) {
        const el = event.target;
        
        if (el.classList.contains('onlyYear')) {
            const value = event.target.value
            const originValue = el.dataset.vOriginvalue;
            if (value == '') {
                event.target.value = originValue;
            }
        }
    })
        // uzivatel zmackne tlacitko
    document.addEventListener('keydown', function (event) {
        const el = event.target;
        const key = event.key;

        if (el.classList.contains('onlyYear')) {
            const value = event.target.value;
            const originValue = el.dataset.vOriginvalue;

            const keysToDisable= ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab'];

            if (value.length <= 2 && keysToDisable.includes(event.key)) {
                event.preventDefault();
            }

            if (event.key == 'ArrowDown' && value == originValue) {
                event.preventDefault()
            }
        }
    })

    // filter clear
    document.addEventListener('click', async function (event) {
        if (event.target.classList.contains('btn-clear')) {
            const container = event.target.closest('.input-container');
            const input = container.querySelector('.configInput');
            if (input) {
                input.value = '';
                await filterForm();
            }
        }
    });

    // PDF export from config sections
    document.addEventListener('click', async function (e) {
        const btn = e.target.closest('#btnExportPdf');
        if (!btn) return;

        const pdfUrlEl = document.getElementById('pdfUrl');
        if (!pdfUrlEl) {
            toastr.error('PDF export není pro tuto sekci dostupný.');
            return;
        }

        const data = getDataFromForm('filterForm');
        try {
            const response = await fetch(pdfUrlEl.value, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-XSRF-TOKEN': getAntiForgeryToken() },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                toastr.error('Nepodařilo se vygenerovat PDF.');
                return;
            }

            const blob = await response.blob();
            const disposition = response.headers.get('Content-Disposition');
            let filename = 'export.pdf';
            if (disposition) {
                const match = disposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
                if (match && match[1]) filename = match[1].replace(/['"]/g, '');
            }

            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            a.click();
            URL.revokeObjectURL(url);
            toastr.success('PDF bylo úspěšně staženo.');
        }
        catch (error) {
            console.error('PDF export error:', error);
            toastr.error('Nastala chyba při generování PDF.');
        }
    });

})();
var onMobile = false;

document.addEventListener('DOMContentLoaded', function () {
    var el = document.getElementById('configModal');
    var modal = bootstrap.Modal.getOrCreateInstance(el);
    modal.show();

    if (window.innerWidth > 992) { onMobile = false }
    console.log(onMobile)
})

window.addEventListener('resize', () => {
    const width = window.innerWidth;
    if (width < 992) {
        console.log("Mobile view active");
    }
});

$(document).on('click', '.menuItem', function () {
    console.log("kliknuto");
    $(".menuItem").each(function () {
        $(this).removeClass("active");
    });
    $(this).addClass("active");
});

function initTitlesSelect2() {
    $selects = $(".select2")
    $selects.each(function () {
        const $sel = $(this);
        const dropdownParent = $('#configModal').length ? $('#configModal') : $(document.body);
        $sel.select2({
            theme: 'bootstrap-5',
            width: '100%',
            dropdownAutoWidth: true,
            dropdownParent: dropdownParent,
        });
    });
}

(function () {
    function showCanvas() {
        if (!window.matchMedia('(max-width: 991.98px)').matches) {
            return;
        }

        const offcanvasEl = document.getElementById('ctEditor');
        offcanvasEl.style.zIndex = '11060';

        const instance = bootstrap.Offcanvas.getOrCreateInstance(offcanvasEl);
        instance.show();

        setTimeout(() => {
            document.querySelectorAll('.offcanvas-backdrop.show').forEach(b => b.style.zIndex = '11050');
            document.body.classList.add('modal-open');
        }, 0);
    }


    // Handle clicks 

    document.addEventListener('click', function (e) {
        const cancel = e.target.closest('#btnCancelDesktop');
        if (!cancel) return;
        hideDesktopEdit();
    })

    document.addEventListener('click', async function (e) {
        const submit = e.target.closest(".configSubmit")
        if (!submit) return;
        const el = e.target;
        const button = el.closest('button[data-form]');
        const formId = button.getAttribute('data-form');

        const isValid = validateDataAttributes(formId);

        if (!isValid) { return; }

        const data = getDataFromForm(formId)
        const url = document.getElementById('crudURL').value

        if (editModeOn) {
            await fetchDataForm(url, data, 'PUT')
            await filterForm();
        }
        else {
            await fetchDataForm(url, data, 'POST')
            await filterForm();
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
            if (!window.matchMedia('(max-width: 991.98px)').matches) {
                showDesktopForm();
            }
            else {
                showCanvas();
            }

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

        cancelDeleteMode()
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
        if (!window.matchMedia('(max-width: 991.98px)').matches) {
            showDesktopForm();
        }
        else {
            showCanvas();
        }
        initTitlesSelect2()
    })

    let editModeOn = true;
    function createMode() {
        if (editModeOn === false) return;
        editModeOn = false;

        const elHeader = document.getElementById('formTitleTextDesktop');
        const elHeaderMobile = document.getElementById('ctEditorLabel');

        elHeader.textContent = 'Vytvořit nového učitele';
        elHeaderMobile.textContent = 'Vytvořit nového učitele';
    }
    function editMode() {
        if (editModeOn === true) return;
        editModeOn = true
        const elHeader = document.getElementById('formTitleTextDesktop');
        const elHeaderMobile = document.getElementById('ctEditorLabel');
        elHeader.textContent = 'Editace učitele';
        elHeaderMobile.textContent = 'Editace učitele';
    }

    function getDataFromForm(formId) {
        const form = document.getElementById(formId)
        const data = new FormData(form);
        const obj = {};
        data.forEach((value, key) => {
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
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            const text = await response.text();
            if (!response.ok) {
                toastr.error(text)
            }

            toastr.success(text);
            inputDisable(false);
            loadingScreen(false);
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
            const editCanvas = document.getElementById('ConfigFormMobile');

            editBody.innerHTML = data;
            editCanvas.innerHTML = data;
            toastr.success('Podařilo se načíst formulář.');
        }
        catch (error) {
            toastr.error('Nepodařilo se načíst formulář.');
            console.error('Error fetching edit form:', error);
        }
    }
    
    function showDesktopForm() {
        var list = document.getElementById("list");
        var edit = document.getElementById("editDesktop");
        list.classList.remove("col-12");
        list.classList.add("col-7");
        edit.style.display = 'block';
    }
    function hideDesktopEdit() {
        var list = document.getElementById("list");
        var edit = document.getElementById("editDesktop");

        list.classList.remove("col-7");
        list.classList.add("col-12");
        edit.style.display = 'none';
    }

    function inputDisable(state) {
        inputs = getInputs();
        inputs.forEach(input => { input.disabled = state });
    }
    function getInputs() {
        const inputs = document.querySelectorAll('#formDesktop input, #formDesktop select, #formDesktop textarea,' +
            '#ConfigFormMobile input, #ConfigFormMobile select, #ConfigFormMobile textarea');
        return inputs;
    }

    let filterTimeout; 
    let filterToast

    $('#filterForm').on('focusout', function (e) {
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
    });

    $('#filterForm').on('focus', function () {
        if (filterToast) { toastr.clear(filterToast); };
    });

    async function filterForm() {
        const data = getDataFromForm('filterForm');
        const url = document.getElementById('filterUrl').value;
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
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

        // FIX PRO SELECT2: Přidá červený rámeček i na Select2 vizuál
        const s2Container = element.nextElementSibling;
        if (s2Container && s2Container.classList.contains('select2-container')) {
            s2Container.classList.add('is-invalid');
        }
    }
})();
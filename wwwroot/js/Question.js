const readAsBase64 = (file) => {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = (error) => reject(error);
        reader.readAsDataURL(file);
    });
};


async function UpdateQuestion() {
    disableInputs(true);
    disableSubmitButton(true);
    const isValid = await Validate();
    if (!isValid) {
        disableInputs(false);
        disableSubmitButton(false);
        return;
    }
    try {
        loadingScreen(true);
        const data = await GatherData();
        const response = await fetch('/Question/Update', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        })

        const result = await response.json();

        if (response.ok) {
            toastr.success(result.message || 'Otázka aktualizovaná!', 'Úspěch');
            setTimeout(() => {
                window.location.href = '/Question/Index';
            }, 1500);
        } else {
            loadingScreen(false)
            console.error('Server validation errors:', result);
            if (result.errors && result.errors.length > 0) {
                result.errors.forEach(err => toastr.error(err, 'Validace'));
            } else {
                toastr.error(result.message || 'Chyba při aktualizaci otázky', 'Chyba');
            }
        }
    } catch (err) {
        loadingScreen(false)
        toastr.error('Chyba při komunikaci se serverem', 'Chyba');
        console.error(err);
    } finally {
        disableInputs(false);
        disableSubmitButton(false);
    }
}

async function generateOptions() {
    const el = document.getElementById('optionCount')
    const count = el.value ?? 0

    const optionsContainer = document.querySelectorAll('.previewOption')
    if (!el) { return; }
    if (count <= 0) { return; }

    var fetchOptions = count > optionsContainer.length;

    if (fetchOptions) {
        const toFetch = count - optionsContainer.length;

        const data = {
            QuestionTypeId: parseInt(document.getElementById('QuestionTypeId').value),
            QuestionCount: parseInt(toFetch),
            CurrentCount: parseInt(optionsContainer.length)
        };

        const responseOption = await fetch(`/api/QuestionView/AnswerOption/`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        })

        const responsePreview = await fetch(`/api/QuestionView/PreviewOptions/`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        })

        const val = await responseOption.headers.get('X-Question-Type')
        const typeName = decodeURIComponent(val);
        if (responseOption.ok && responsePreview.ok)
        {
            // Select Text Container
            const optionResult = await responseOption.text()
            const previewResult = await responsePreview.text()

            const optionContainer = document.getElementById('optionsContainer')
            const previewContainer = document.getElementById('previewOptionContainer')
            optionContainer.insertAdjacentHTML('beforeend', optionResult)
            previewContainer.insertAdjacentHTML('beforeend', previewResult)

            toastr.success('Možnosti vygenerovány', 'Úspěch');
        }
        else
        {
            toastr.warning('Chyba při generování možností', 'Chyba');
        }
    }
    else // remove last n options
    {
        const toRemove = optionsContainer.length - count;
        const radioOptionsContainer = document.querySelectorAll('.option-card');

        for (let i = 0; i < toRemove; i++) {
            optionsContainer[optionsContainer.length - 1 - i].remove();
            radioOptionsContainer[radioOptionsContainer.length - 1 - i].remove();
        }
    }
}
function setUpFlex(rowCount) {
    const elements = document.querySelectorAll('.imageOption');
    const count = elements.length;
    var lastCount = elements / rowCount;

    for (let i = 0; i < elements.length; i++) {
        const el = elements[i];
        const arr = el.classList.toString().split(' ');
        filtered = arr.filter(c => c.startsWith('optionItem-'))
        el.classList.remove(...filtered);


        if (count - rowCount <= lastCount & !(count <= rowCount)) {
            el.classList.add(`optionItem-${count - lastCount}`);
        }
        else {
            el.classList.add(`optionItem-${rowCount}`);
        }
    }
}

document.addEventListener('click', (e) => {
    const el = e.target;

    // Dynamic Remove Event Delegation
    if (el.classList.contains('questionRemove') || el.classList.contains('cross-line')) {
        const optionCards = document.querySelectorAll('.option-card');

        if (optionCards.length <= 2) {
            toastr.error('Musí být alespoň dvě možnosti odpovědí', 'Chyba');
            return;
        }
        const optionCard = el.closest('.option-card') 
        const id = optionCard.getAttribute('data-index');
        const optionPreview = document.querySelector(`.previewOption[data-index="${id}"]`);

        optionPreview.remove()
        optionCard.remove();
        resetOptionIndexes()
    }

    // image file input (.noImage)

    if (el.classList.contains('noImage') || el.classList.contains('noImageTxt')) {
        const option = el.closest('.previewOption')
        const idx = option.dataset.index;
        const optionCard = document.querySelector(`.option-card[data-index="${idx}"]`);
        const fileInput = optionCard.querySelector('input[name="imageQuestion"]');
        fileInput.click();
    }
}); 

function resetOptionIndexes() {
    const optionInputs = document.querySelectorAll('.option-card');
    const optionPreviews = document.querySelectorAll('.previewOption');

    optionInputs.forEach((card, idx) => {
        card.setAttribute('data-index', idx);
        var label = card.querySelector('.option-text');
        const defaultText = label.dataset.default
        var newText = ''

        if (defaultText.lastIndexOf(') Možnost ') != -1) {
            newText = `Možnost ${String.fromCharCode(65 + idx)}`;
        }
        else if (defaultText.lastIndexOf('Nadpis ') != -1) {
            newText = `Nadpis ${idx + 1}`;
        }

        label.placeholder = newText;
    })

    optionPreviews.forEach((preview, idx) => {
        preview.setAttribute('data-index', idx);
        var label = preview.querySelector('.optionPreviewLabel');

        const defaultText = preview.dataset.default;
        var newDefaultText = '';

        // Moznosti ve formatu "A) Možnost A"
        if (defaultText.lastIndexOf(") Možnost ") != -1) {
            const abcChar = `${String.fromCharCode(65 + idx)}`;
            newDefaultText = `${abcChar}) Možnost ${abcChar}`
        }
        else if (defaultText.lastIndexOf("Nadpis ") != -1) {
            newDefaultText = 'Nadpis ' + (idx + 1);
        }

        if (label.textContent == defaultText) {
            label.textContent = newDefaultText;
        }
        preview.dataset.default = newDefaultText;
    });
}

async function CreateQuestion() {
    disableInputs(true);
    disableSubmitButton(true);
    const isValid = await Validate();

    if (!isValid) {
        disableInputs(false);
        disableSubmitButton(false);
        return;
    }

    try {
        loadingScreen(true); 
        const data = await GatherData();
        const response = await fetch('/Question/CreateQuestion', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        })

        const result = await response.json();

        if (response.ok) {
            toastr.success(result.message || 'Otázka vytvořena!', 'Úspěch');
            setTimeout(() => {
                window.location.href = '/Question/Index';
            }, 1500);
        } else {
            loadingScreen(false)
            console.error('Server validation errors:', result);
            if (result.errors && result.errors.length > 0) {
                result.errors.forEach(err => toastr.error(err, 'Validace'));
            } else {
                toastr.error(result.message || 'Chyba při vytváření otázky', 'Chyba');
            }
        }
    } catch (err) {
        loadingScreen(false)
        toastr.error('Chyba při komunikaci se serverem', 'Chyba');
        console.error(err);
    } finally {
        disableInputs(false);
        disableSubmitButton(false);
    }
}



async function Validate() {
    const header = document.getElementById('Header');
    const description = document.getElementById('Description');
    const questionTypeId = document.getElementById('QuestionTypeId');
    const fieldId = document.getElementById('FieldId');

    clearAllValidation();
    let isValid = true;

    // Header
    if (!header.value.trim()) {
        setFieldError(header, 'Nadpis je povinný');
        isValid = false;
    } else if (header.value.trim().length < 3) {
        setFieldError(header, 'Nadpis musí mít alespoň 3 znaky');
        isValid = false;
    } else if (header.value.trim().length > 128) {
        setFieldError(header, 'Nadpis nesmí být delší než 128 znaků');
        isValid = false;
    } else {
        setFieldSuccess(header);
    }

    // Description
    if (!description.value.trim()) {
        setFieldError(description, 'Popis je povinný');
        isValid = false;
    } else if (description.value.trim().length < 10) {
        setFieldError(description, 'Popis musí mít alespoň 10 znaků');
        isValid = false;
    } else if (description.value.trim().length > 512) {
        setFieldError(description, 'Popis nesmí být delší než 512 znaků');
        isValid = false;
    } else {
        setFieldSuccess(description);
    }

    // Question type
    if (!questionTypeId.value || parseInt(questionTypeId.value) <= 0) {
        setFieldError(questionTypeId, 'Vyberte typ otázky');
        isValid = false;
    } else {
        setFieldSuccess(questionTypeId);
    }

    // Field
    if (!fieldId.value || parseInt(fieldId.value) <= 0) {
        setFieldError(fieldId, 'Vyberte předmět');
        isValid = false;
    } else {
        setFieldSuccess(fieldId);
    }

    // Options existence
    const optionInputs = document.querySelectorAll('.option-text');
    if (optionInputs.length < 2) {
        toastr.warning('Otázka musí mít alespoň 2 možnosti odpovědí. Klikněte na Generovat.', 'Chybí možnosti');
        isValid = false;
    } else if (optionInputs.length > 10) {
        toastr.warning('Otázka může mít maximálně 10 možností odpovědí.', 'Příliš mnoho možností');
        isValid = false;
    }

    // Options validation
    let hasCorrect = false;
    let allOptionsFilled = true;
    optionInputs.forEach((input, idx) => {
        const card = input.closest('.option-card');
        const text = input.value.trim();
        const isCorrect = document.getElementById(`isCorrect_${idx}`)?.checked || false;

        if (!text) {
            card?.classList.add('is-invalid');
            input.classList.add('is-invalid');
            allOptionsFilled = false;
        } else {
            card?.classList.remove('is-invalid');
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        }
        if (isCorrect) hasCorrect = true;
    });

    if (!allOptionsFilled) {
        toastr.warning('Vyplňte text u všech možností odpovědí', 'Prázdné možnosti');
        isValid = false;
    }

    if (optionInputs.length >= 2 && !hasCorrect) {
        toastr.warning('Označte alespoň jednu správnou odpověď', 'Chybí správná odpověď');
        isValid = false;
    }

    // File inputs (required images)
    const fileInputs = document.querySelectorAll('input[type="file"][required="true"]');
    let allFilesOk = true;
    fileInputs.forEach((input, idx) => {
        if (!input.files[0]) {
            const card = input.closest('.option-card');
            card?.classList.add('is-invalid');
            allFilesOk = false;
        }
    });
    if (!allFilesOk) {
        toastr.warning('Vyberte obrázek u všech možností, které ho vyžadují', 'Chybí obrázky');
        isValid = false;
    }

    if (!isValid) {
        toastr.error('Opravte chyby ve formuláři před odesláním', 'Validace selhala');
    }

    return isValid;
}

function setFieldError(el, message) {
    el.classList.remove('is-valid');
    el.classList.add('is-invalid');
    // Remove old feedback
    const existing = el.parentElement.querySelector('.q-feedback');
    if (existing) existing.remove();
    // Add error feedback
    const fb = document.createElement('div');
    fb.className = 'q-feedback text-danger';
    fb.style.fontSize = '.8rem';
    fb.style.marginTop = '.25rem';
    fb.innerHTML = '<i class="bi bi-exclamation-circle me-1"></i>' + message;
    el.parentElement.appendChild(fb);
}

function setFieldSuccess(el) {
    el.classList.remove('is-invalid');
    el.classList.add('is-valid');
    const existing = el.parentElement.querySelector('.q-feedback');
    if (existing) existing.remove();
}

function clearAllValidation() {
    document.querySelectorAll('.is-invalid, .is-valid').forEach(el => {
        el.classList.remove('is-invalid', 'is-valid');
    });
    document.querySelectorAll('.q-feedback').forEach(el => el.remove());
}


async function GatherData() {
    const header = document.getElementById('Header').value.trim();
    const description = document.getElementById('Description').value.trim();
    const questionTypeId = parseInt(document.getElementById('QuestionTypeId').value) || 0;
    const fieldId = parseInt(document.getElementById('FieldId').value) || 0;
    const options = [];

    const optionInputs = document.querySelectorAll('.option-card');

    for (const [idx, input] of optionInputs.entries()) {
        var base64 = null;
        const imageInput = input.querySelectorAll('input[name="imageQuestion"]');

        if (imageInput.length > 0) { 
            const file = imageInput[0].files[0];
            if (file) {
                base64 = await readAsBase64(file);
            }
        }

        var optionData = {
            text: input.querySelector('input[name="optionText"]').value.trim(),
            isCorrect: input.querySelector('input[name="CorrectInput"]')?.checked || false,
            imageBase64: base64
        };

        options.push(optionData);
    }

    const data = {
        header: header,
        description: description,
        questionTypeId: questionTypeId,
        fieldId: fieldId,
        options: options
    };

    // Edit 
    const questionIdEl = document.getElementById('QuestionId');
    if (questionIdEl) {
        data.questionId = parseInt(questionIdEl.value) || 0;
    }

    return data;
}

function disableInputs(choice) {
    const inputs = document.querySelectorAll('input, select, textarea, button');
    inputs.forEach(input => input.disabled = choice);
}

function disableSubmitButton(choice) {
    const submitButton = document.getElementById('submit');
    submitButton.disabled = choice;
}
function resetForm() {
    location.reload()
}

// Header input listener

document.addEventListener('DOMContentLoaded', () => {
setUpFlex(3);
updatePreview();
// Header
const headerInput = document.getElementById('Header');

headerInput.addEventListener('input', (e) => {
    const preview = document.getElementById('previewHeader')
    if (preview) {
        preview.textContent = e.target.value || '';
    }
    // Live validation
    const v = e.target.value.trim();
    if (v.length >= 3 && v.length <= 128) setFieldSuccess(e.target);
    else if (v.length > 0) { setFieldError(e.target, v.length < 3 ? 'Min. 3 znaky' : 'Max. 128 znaků'); }
    else { e.target.classList.remove('is-valid', 'is-invalid'); const fb = e.target.parentElement.querySelector('.q-feedback'); if (fb) fb.remove(); }
})

// Description
const descriptionInput = document.getElementById('Description');

descriptionInput.addEventListener('input', (e) => {
    const preview = document.getElementById('previewDescription')
    if (preview) {
        preview.textContent = e.target.value || '';
    }
    // Live validation
    const v = e.target.value.trim();
    if (v.length >= 10 && v.length <= 512) setFieldSuccess(e.target);
    else if (v.length > 0) { setFieldError(e.target, v.length < 10 ? 'Min. 10 znaků' : 'Max. 512 znaků'); }
    else { e.target.classList.remove('is-valid', 'is-invalid'); const fb = e.target.parentElement.querySelector('.q-feedback'); if (fb) fb.remove(); }
})

    // Generate Button

    const generateBtn = document.getElementById('Generate')

    generateBtn.addEventListener('click', async (e) => {
        await generateOptions();
    });


    // QuestionTypeId change listener

    const questionTypeSelect = document.getElementById('QuestionTypeId');

    $(questionTypeSelect).on('change', async (e) => {

        // clear
        $("#previewContainer").remove()
        var optionsAnswer = $(".option-card").toArray();
        optionsAnswer.forEach((option) => {
            option.remove()
        })

        var questionTypeId = parseInt(e.target.value);
        var count = parseInt($("#optionCount").val());

        // Preview
        var optionsPreviewFetch = await fetch('/questionview/preview',{
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ QuestionTypeId: questionTypeId, OptionCount: count }),
        });

        if (optionsPreviewFetch.ok) {
            $("#previewBody").append(await optionsPreviewFetch.text())
        }

        // Options

        const responseOption = await fetch(`/api/QuestionView/AnswerOption/`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ QuestionTypeId: questionTypeId, QuestionCount: count, CurrentCount: 0})
        })

        if (responseOption.ok) {
            const optionContainer = document.getElementById('optionsContainer')
            const optionResult = await responseOption.text()
            optionContainer.insertAdjacentHTML('beforeend', optionResult)
        }

        updatePreview();
    });
})

// CorrectInput
document.addEventListener('input', (e) => {

    const el = e.target
    const value = el.value
    const name = el.getAttribute('name');

    // Option text input
    if (name == 'optionText') {
        const dataIndex = el.closest('.option-card').dataset.index;
        const option = document.querySelector(`.previewOption[data-index="${dataIndex}"]`);

        const defaultValue = option.dataset.default

        if (!option) { toastr.error('interní problém webové aplikace') }
         
        const label = option.querySelector('.optionPreviewLabel');

        if (label) {
            label.textContent = `${value || defaultValue}`;
        }
    }
})

document.addEventListener('change', (e) => {
    const el = e.target;
    const name = el.getAttribute('name');

    // Correct answer checkbox
    if (name == 'CorrectInput') {
        const optionCard = el.closest('.option-card');
        const id = optionCard.getAttribute('data-index');

        let previewBorder;

        const optionInput = document.querySelector(`.previewOption[data-index="${id}"]`);
        if (optionInput.classList.contains('optionPreviewBorder')) {
            previewBorder = optionInput;
        }
        else {
            previewBorder = optionInput.querySelector('.optionPreviewBorder');

        }

        if (el.checked) {
            optionCard.classList.add('correct')
            previewBorder.classList.add('correct')
        }
        else {
            optionCard.classList.remove('correct')
            previewBorder.classList.remove('correct')
        }
    }

    // Image Input
    
    if (name == 'imageQuestion') {
        const optionCard = el.closest('.option-card');
        const id = optionCard.getAttribute('data-index');
        const file = e.target.files[0];

        const previewOption = document.querySelector(`.previewOption[data-index="${id}"]`);
        const imageContainer = previewOption.querySelector('.imageContainer');
        const noImageHolder = previewOption.querySelector('.noImage'); 
        const previewImage = previewOption.querySelector('.imageContainer>img');

        if (file) {
            const reader = new FileReader();

            reader.onload = function (event) {
                previewImage.src = event.target.result;
                imageContainer.classList.remove('d-none');
                noImageHolder.classList.add('d-none');
            }

            reader.readAsDataURL(file);
        } else {
            noImageHolder.classList.remove('d-none');
            imageContainer.classList.add('d-none');
        }
    }
})

function updatePreview() {
    const header = document.getElementById('Header').value.trim();
    const description = document.getElementById('Description').value.trim();

    const previewHeader = document.getElementById('previewHeader');
    const previewDescription = document.getElementById('previewDescription');

    previewHeader.textContent = header.length <= 0 ? 'Nadpis otázky' : header;
    previewDescription.textContent = description.length <= 0 ? 'Popis/znění otázky' : description;
}
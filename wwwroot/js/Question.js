async function EditQuestion() {
    disableInputs(true);
    disableSubmitButton(true);
    const isValid = await Validate();
    if (!isValid) {
        return;
    }
    try {
        loadingScreen(true);
        const data = GatherData();
        const myData = {
            header: data.header,
            description: data.description,
            questionTypeId: data.questionTypeId,
            FieldId: data.FieldId,
            options: data.options,
            QuestionId: parseInt(document.getElementById('QuestionId').value),
            IsActive: document.getElementById('IsActive').value === 'True'
        };
        const response = await fetch('/Question/Update', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(myData)
        })

        const result = await response.json();

        if (response.ok) {
            toastr.success(result.message || 'Otázka je aktualizovaná!', 'Úspěch');
            setTimeout(() => {
                window.location.href = '/Question/Index';
            }, 1500);
        } else {
            loadingScreen(false)
            toastr.error(result.message || 'Chyba při aktualizaci otázky', 'Chyba');
        }
    } catch (err) {
        loadingScreen(false)
        toastr.error('Chyba při komunikaci se serverem', 'Chyba');
        console.error(err);
    }
}

async function generateOptions() {
    const el = document.getElementById('optionCount')
    const count = el.value

    const optionsContainer = document.querySelectorAll('.option-card');
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

        if (responseOption.ok && responsePreview.ok)
        {
            const optionContainer = document.getElementById('optionsContainer')
            const previewContainer = document.getElementById('previewOptions').querySelector('.radio-group')

            const optionResult = await responseOption.text()
            const previewResult = await responsePreview.text()

            optionContainer.insertAdjacentHTML('beforeend', optionResult);
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
        const radioOptionsContainer = document.querySelectorAll('.radio-option');

        for (let i = 0; i < toRemove; i++) {
            optionsContainer[optionsContainer.length - 1 - i].remove();
            radioOptionsContainer[radioOptionsContainer.length - 1 - i].remove();
        }
    }
}

document.addEventListener('click', (e) => {
    const el = e.target;

    const optionCards = document.querySelectorAll('.option-card');

    if (optionCards.length <= 2) {
        toastr.error('Musí být alespoň dvě možnosti odpovědí', 'Chyba');
        return;
    }

    if (el.classList.contains('questionRemove') || el.classList.contains('cross-line')) {
        const optionCard = el.closest('.option-card') 
        const id = optionCard.getAttribute('data-option-index');
        const optionPreview = document.querySelector(`.radio-option[data-option-index="${id}"]`);

        optionPreview.remove()
        optionCard.remove();
        resetOptionIndexes()
    }
}); 

function resetOptionIndexes() {
    const optionInputs = document.querySelectorAll('.option-card');
    const optionPreviews = document.querySelectorAll('.radio-option');

    optionInputs.forEach((card, idx) => {
        card.setAttribute('data-option-index', idx);
        var label = card.querySelector('.option-text');
        if (label.value == "") {
            label.placeholder = `Možnost ${String.fromCharCode(65 + idx)}`;
        }
    })

    optionPreviews.forEach((preview, idx) => {
        preview.setAttribute('data-option-index', idx);
        var label = preview.querySelector('.radio-label');
        const abcChar = `${String.fromCharCode(65 + idx)}`;

        if (label.textContent.length == 12 && label.textContent.lastIndexOf(") Možnost ") != -1) {
            label.textContent = `${abcChar}) Možnost ${abcChar}`;
        }
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
        const data = GatherData();
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
        } else
        {
            loadingScreen(false)
            toastr.error(result.message || 'Chyba při vytváření otázky', 'Chyba');
        }
    } catch (err) {
        loadingScreen(false)
        toastr.error('Chyba při komunikaci se serverem', 'Chyba');
        console.error(err);
    }
}

async function Validate() {
    const header = document.getElementById('Header').value.trim();
    const description = document.getElementById('Description').value.trim();
    const questionTypeId = parseInt(document.getElementById('QuestionTypeId').value);
    const fieldId = parseInt(document.getElementById('FieldId').value);

    if (!header || !description) {
        toastr.warning('Vyplň nadpis a popis otázky', 'Varování');
        return false;
    }

    const optionInputs = document.querySelectorAll('.option-text');

    if (optionInputs.length === 0) {
        toastr.warning('Vygeneruj nejdříve možnosti odpovědí', 'Varování');
        return false;
    }

    let hasCorrect = false;
    let isValid = true;
    optionInputs.forEach((input, idx) => {
        if (!isValid) return;
        const text = input.value.trim();
        const isCorrect = document.getElementById(`isCorrect_${idx}`)?.checked || false;

        if (!text) {
            toastr.warning(`Vyplň text pro možnost ${String.fromCharCode(65 + idx)}, zkontroluj vše před uložením`, 'Varování');
            isValid = false;
            return false;
        }
        if (isCorrect) hasCorrect = true;
    });

    if (!isValid) { return false; }

    if (!hasCorrect) {
        toastr.warning('Označ alespoň jednu správnou odpověď', 'Varování');
        return false;
    }
    return true;
}
function GatherData() {
    const header = document.getElementById('Header').value.trim();
    const description = document.getElementById('Description').value.trim();
    const questionTypeId = parseInt(document.getElementById('QuestionTypeId').value);
    const fieldId = parseInt(document.getElementById('FieldId').value);
    const options = [];
    const optionInputs = document.querySelectorAll('.option-text');
    optionInputs.forEach((input, idx) => {
        options.push({
            text: input.value.trim(),
            imageBase64: null,
            isCorrect: document.getElementById(`isCorrect_${idx}`)?.checked || false
        });
    });
    const data = {
        header: header,
        description: description,
        questionTypeId: questionTypeId,
        FieldId: fieldId,
        options: options
    };
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
    // Header
    const headerInput = document.getElementById('Header');

    headerInput.addEventListener('input', (e) => {
        const preview = document.getElementById('previewHeader')

        if (preview) {
            preview.textContent = e.target.value || '';
        }
    })

    // Description

    const descriptionInput = document.getElementById('Description');

    descriptionInput.addEventListener('input', (e) => {
        const preview = document.getElementById('previewDescription')

        if (preview) {
            preview.textContent = e.target.value || '';
        }
    })

    // Generate Button

    const generateBtn = document.getElementById('Generate')

    generateBtn.addEventListener('click', async (e) => {
        await generateOptions();
    });

})

// Event delegation for dynamically added option inputs

// CorrectInput
document.addEventListener('input', (e) => {

    const el = e.target
    const value = el.value;
    const name = el.getAttribute('name');

    // Option text input
    if (name == 'optionText') {
        const dataIndex = el.dataset.index;

        const previewOption = document.querySelector(`.radio-option[data-option-index="${dataIndex}"] .radio-label`);

        if (previewOption) {
            previewOption.textContent = `${String.fromCharCode(65 + parseInt(dataIndex))}) ${value || `Možnost ${String.fromCharCode(65 + parseInt(dataIndex))}`}`;
        }
    }
})

document.addEventListener('change', (e) => {
    const el = e.target;
    const value = el.value;
    const name = el.getAttribute('name');

    // Correct answer checkbox
    if (name == 'CorrectInput') {
        const id = el.closest('.option-card').getAttribute('data-option-index');
        const optionInput = document.querySelector(`.radio-option[data-option-index="${id}"]`);
        const border = el.closest('.option-card');
        if (el.checked) {
            border.classList.add('correct')
            border.classList.remove('incorrect')
            optionInput.classList.add('correct')
            optionInput.classList.remove('incorrect')
        }
        else {
            border.classList.remove('correct')
            border.classList.add('incorrect')
            optionInput.classList.remove('correct')
            optionInput.classList.add('incorrect')
        }
    }
})
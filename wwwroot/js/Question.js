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
            Count: parseInt(toFetch),
        };

        const response = await fetch(`/api/QuestionView/AnswerOption/`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        })

        const result = await response.text();

        if (result.success) {
            toastr.success(result.message || 'Možnosti vygenerovány', 'Úspěch');
        }
        else {
            toastr.warning(result.message || 'Chyba při generování možností', 'Chyba');
        }
    }
    else // remove last n options
    {
        const toRemove = optionsContainer.length - count;

        for (let i = 0; i < toRemove; i++) {
            optionsContainer[optionsContainer.length - 1 - i].remove();
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
    let hasUnfilledOption = false;
    optionInputs.forEach((input, idx) => {
        const text = input.value.trim();
        const isCorrect = document.getElementById(`isCorrect_${idx}`)?.checked || false;

        if (!text) {
            toastr.warning(`Vyplň text pro možnost ${String.fromCharCode(65 + idx)}`, 'Varování');
            hasUnfilledOption = true;
            return false;
        }
        if (isCorrect) hasCorrect = true;
    });

    if (hasUnfilledOption) {
        return false;
    }

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


let optionCounter = 0;

window.addEventListener('DOMContentLoaded', () => {
    if (!document.getElementById('edit')) {
        generateOptions();
    }
    else {
        document.getElementById('previewHeader').textContent = document.getElementById('Header').value;
        document.getElementById('previewDescription').textContent = document.getElementById('Description').value;

    }
    
    document.getElementById('Header').addEventListener('input', updatePreview);
    document.getElementById('Description').addEventListener('input', updatePreview);
});

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


function generateOptions() {
    const count = parseInt(document.getElementById('optionCount').value) || 4;
    if (count < 2 || count > 10) {
        toastr.warning('Počet možností musí být mezi 2 a 10', 'Varování');
        return;
    }

    const container = document.getElementById('optionsContainer');
    container.innerHTML = '<h6 class="fw-bold mb-3"><i class="bi bi-ui-checks text-orange"></i> Možnosti odpovědí:</h6>';

    for (let i = 0; i < count; i++) {
        const optionHtml = `
            <div class="card mb-2 option-card" data-option-index="${i}">
                <div class="card-body p-3">
                    <div class="d-flex align-items-start gap-2">
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" 
                                   id="isCorrect_${i}" onchange="updatePreview()">
                            <label class="form-check-label fw-bold text-success" for="isCorrect_${i}">
                                Správná
                            </label>
                        </div>
                        <div class="flex-grow-1">
                            <label class="form-label small mb-1">Text odpovědi:</label>
                            <input type="text" class="form-control option-text" 
                                   data-index="${i}" placeholder="Možnost ${String.fromCharCode(65 + i)}" 
                                   oninput="updatePreview()" required />
                        </div>
                    </div>
                </div>
            </div>
        `;
        container.insertAdjacentHTML('beforeend', optionHtml);
    }
    updatePreview();
}

// Update preview in real-time
function updatePreview() {
    const header = document.getElementById('Header').value || 'Nadpis otázky';
    const description = document.getElementById('Description').value || 'Popis otázky se zobrazí zde...';

    document.getElementById('previewHeader').textContent = header;
    document.getElementById('previewDescription').textContent = description;

    // Update options preview
    const radioGroup = document.querySelector('#previewOptions .radio-group');
    radioGroup.innerHTML = '';

    const options = document.querySelectorAll('.option-text');  
    const correctAnswers = [];

    options.forEach((opt, idx) => {
        const text = opt.value || `Možnost ${String.fromCharCode(65 + idx)}`;
        const isCorrect = document.getElementById(`isCorrect_${idx}`)?.checked;

        if (isCorrect) {
            correctAnswers.push(`${String.fromCharCode(65 + idx)}) ${text}`);
        }

        const optionHtml = `
            <label class="radio-option ${isCorrect ? 'correct-option' : ''}">
                <input type="radio" name="preview_answer" value="${idx}" disabled />
                <span class="radio-label">${String.fromCharCode(65 + idx)}) ${text}</span>
            </label>
        `;
        radioGroup.insertAdjacentHTML('beforeend', optionHtml);
    });

    // Update correct answers list
    const correctList = document.getElementById('correctAnswersList');
    if (correctAnswers.length > 0) {
        correctList.innerHTML = correctAnswers.map(a => `<div class="text-success"><i class="bi bi-check-circle-fill me-1"></i>${a}</div>`).join('');
    } else {
        correctList.innerHTML = '<span class="text-muted">Zatím nejsou označeny žádné správné odpovědi</span>';
    }
}
function disableInputs(choice) {
    const inputs = document.querySelectorAll('input, select, textarea, button');
    inputs.forEach(input => input.disabled = choice);
}

function disableSubmitButton(choice) {
    const submitButton = document.getElementById('submit');
    submitButton.disabled = choice;
}

function validateQuestion() {
    

    //try {
    //    loadingScreen(true);
    //    const response = await fetch('/Question/CreateQuestion', {
    //        method: 'POST',
    //        headers: { 'Content-Type': 'application/json' },
    //        body: JSON.stringify(data)
    //    })

    //    const result = await response.json();

    //    if (response.ok) {
    //        toastr.success(result.message || 'Otázka vytvořena!', 'Úspěch');
    //        setTimeout(() => {
    //            window.location.href = '/Question/Index';
    //        }, 1500);
    //    } else 
    //    {
    //        loadingScreen(false)
    //        toastr.error(result.message || 'Chyba při vytváření otázky', 'Chyba');
    //    }
    //} catch (err) {
    //    loadingScreen(false)
    //    toastr.error('Chyba při komunikaci se serverem', 'Chyba');
    //    console.error(err);
    //}
}

function resetForm() {
    document.getElementById('questionForm').reset();
    document.getElementById('optionsContainer').innerHTML = '';
    updatePreview();
    toastr.info('Formulář byl resetován', 'Info');
}


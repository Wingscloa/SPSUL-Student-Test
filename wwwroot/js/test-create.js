// ============================================
// TEST CREATE - MULTI-STEP FORM & PREVIEW
// ============================================

let currentStep = 1;
const totalSteps = 3;
let selectedQuestions = new Set();
let testData = {
    name: '',
    fieldId: 0,
    fieldName: '',
    description: '',
    timeLimit: 45,
    questionIds: []
};

document.addEventListener('DOMContentLoaded', function () {

    var timeout = setTimeout(function () {
        initializeEventListeners();
        initSelect2();
    }, 500);
});

// ============================================
// SELECT2 INITIALIZATION
// ============================================
function initSelect2() {
    $('#studentField').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: '-- Vyberte předmět --',
    });

    $('#questionTypeFilter').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: 'Všechny typy',
    });

    // Wire Select2 change events into existing handlers
    $('#studentField').on('select2:select select2:clear', updatePreview);
    $('#questionTypeFilter').on('select2:select select2:clear change', filterQuestions);
}

// ============================================
// EVENT LISTENERS
// ============================================
function initializeEventListeners() {
    // Navigation buttons
    document.getElementById('nextBtn').addEventListener('click', nextStep);
    document.getElementById('prevBtn').addEventListener('click', prevStep);
    document.getElementById('submitBtn').addEventListener('click', submitTest);

    // Form inputs with real-time preview
    document.getElementById('testName').addEventListener('input', updatePreview);
    document.getElementById('studentField').addEventListener('change', updatePreview);
    document.getElementById('description').addEventListener('input', updatePreview);
    document.getElementById('timeLimit').addEventListener('input', updatePreview);
    document.getElementById('noTimeLimit').addEventListener('click', toggleTimeLimit);

    // Question filtering
    document.getElementById('questionSearch').addEventListener('input', filterQuestions);
    document.getElementById('questionTypeFilter').addEventListener('change', filterQuestions);

    // Question selection
    document.querySelectorAll('.question-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', handleQuestionSelection);
    });

    document.getElementById('clearSelection').addEventListener('click', clearAllSelections);

    // Question card click
    document.querySelectorAll('.question-card').forEach(card => {
        card.addEventListener('click', function(e) {
            if (e.target.type !== 'checkbox') {
                const checkbox = this.querySelector('.question-checkbox');
                checkbox.checked = !checkbox.checked;
                checkbox.dispatchEvent(new Event('change'));
            }
        });
    });
}

// ============================================
// STEP NAVIGATION
// ============================================
function nextStep() {
    if (!validateCurrentStep()) {
        return;
    }

    if (currentStep < totalSteps) {
        // Hide current step
        document.getElementById(`step${currentStep}`).classList.remove('active');
        document.querySelector(`[data-step="${currentStep}"]`).classList.remove('active');
        document.querySelector(`[data-step="${currentStep}"]`).classList.add('completed');

        currentStep++;

        // Show next step
        document.getElementById(`step${currentStep}`).classList.add('active');
        document.querySelector(`[data-step="${currentStep}"]`).classList.add('active');

        updateNavigationButtons();
        updateSectionTitle();
        
        if (currentStep === 3) {
            updateSummary();
            updateFullPreview();
        }
    }
}

function prevStep() {
    if (currentStep > 1) {
        document.getElementById(`step${currentStep}`).classList.remove('active');
        document.querySelector(`[data-step="${currentStep}"]`).classList.remove('active');

        currentStep--;

        document.getElementById(`step${currentStep}`).classList.add('active');
        document.querySelector(`[data-step="${currentStep}"]`).classList.remove('completed');
        document.querySelector(`[data-step="${currentStep}"]`).classList.add('active');

        updateNavigationButtons();
        updateSectionTitle();
    }
}

function updateNavigationButtons() {
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const submitBtn = document.getElementById('submitBtn');

    prevBtn.disabled = currentStep === 1;

    if (currentStep === totalSteps) {
        nextBtn.classList.add('d-none');
        submitBtn.classList.remove('d-none');
    } else {
        nextBtn.classList.remove('d-none');
        submitBtn.classList.add('d-none');
    }
}

function updateSectionTitle() {
    const titles = {
        1: 'Základní údaje testu',
        2: 'Výběr otázek',
        3: 'Kontrola a náhled'
    };
    document.getElementById('sectionTitle').textContent = titles[currentStep];
}

// ============================================
// VALIDATION
// ============================================
function validateCurrentStep() {
if (currentStep === 1) {
    const name = document.getElementById('testName').value.trim();
    const fieldId = $('#studentField').val();

    if (!name || name.length < 3) {
        toastr.error('Název testu musí mít alespoň 3 znaky.');
        return false;
    }

    if (!fieldId) {
        toastr.error('Musíte vybrat předmět.');
        return false;
    }

    testData.name = name;
    testData.fieldId = parseInt(fieldId);
    testData.fieldName = $('#studentField option:selected').text();
    testData.description = document.getElementById('description').value.trim();
    testData.timeLimit = document.getElementById('timeLimit').value ? parseInt(document.getElementById('timeLimit').value) : null;

    return true;
}

    if (currentStep === 2) {
        if (selectedQuestions.size === 0) {
            toastr.error('Musíte vybrat alespoň 1 otázku.');
            return false;
        }

        testData.questionIds = Array.from(selectedQuestions);
        return true;
    }

    return true;
}

// ============================================
// QUESTION SELECTION
// ============================================
function handleQuestionSelection(e) {
    const checkbox = e.target;
    const questionId = parseInt(checkbox.value);
    const card = checkbox.closest('.question-card');

    if (checkbox.checked) {
        selectedQuestions.add(questionId);
        card.classList.add('selected');
    } else {
        selectedQuestions.delete(questionId);
        card.classList.remove('selected');
    }

    updateSelectedCount();
}

function updateSelectedCount() {
    document.getElementById('selectedCount').textContent = selectedQuestions.size;
}

function clearAllSelections() {
    selectedQuestions.clear();
    document.querySelectorAll('.question-checkbox').forEach(checkbox => {
        checkbox.checked = false;
    });
    document.querySelectorAll('.question-card').forEach(card => {
        card.classList.remove('selected');
    });
    updateSelectedCount();
}

// ============================================
// QUESTION FILTERING
// ============================================
function filterQuestions() {
const searchTerm = document.getElementById('questionSearch').value.toLowerCase();
const typeFilter = $('#questionTypeFilter').val() || '';

    document.querySelectorAll('.question-card').forEach(card => {
        const name = card.dataset.questionName;
        const type = card.dataset.questionType;

        const matchesSearch = !searchTerm || name.includes(searchTerm);
        const matchesType = !typeFilter || type === typeFilter;

        if (matchesSearch && matchesType) {
            card.style.display = '';
        } else {
            card.style.display = 'none';
        }
    });
}

// ============================================
// TIME LIMIT TOGGLE
// ============================================
function toggleTimeLimit() {
    const timeLimitInput = document.getElementById('timeLimit');
    if (timeLimitInput.disabled) {
        timeLimitInput.disabled = false;
        timeLimitInput.value = 45;
        document.getElementById('noTimeLimit').innerHTML = '<i class="bi bi-infinity"></i> Bez limitu';
    } else {
        timeLimitInput.disabled = true;
        timeLimitInput.value = '';
        document.getElementById('noTimeLimit').innerHTML = '<i class="bi bi-clock"></i> Nastavit limit';
    }
    updatePreview();
}

// ============================================
// PREVIEW UPDATE
// ============================================
function updatePreview() {
const name = document.getElementById('testName').value || 'Název testu';
const fieldName = $('#studentField option:selected').text() || 'Předmět';
    const timeLimit = document.getElementById('timeLimit').value;
    const timeLimitText = timeLimit ? `${timeLimit} minut` : 'Bez limitu';

    const previewHTML = `
        <div class="preview-test-header">
            <div class="preview-test-title">${name}</div>
            <div class="preview-meta">
                <div class="preview-meta-item">
                    <i class="bi bi-book"></i>
                    <span>${fieldName}</span>
                </div>
                <div class="preview-meta-item">
                    <i class="bi bi-clock"></i>
                    <span>${timeLimitText}</span>
                </div>
            </div>
        </div>
        <p class="text-muted text-center">Po výběru otázek se zde zobrazí náhled testu</p>
    `;

    document.getElementById('previewContent').innerHTML = previewHTML;
}

// ============================================
// SUMMARY UPDATE
// ============================================
function updateSummary() {
    document.getElementById('summaryName').textContent = testData.name;
    document.getElementById('summaryField').textContent = testData.fieldName;
    document.getElementById('summaryTime').textContent = testData.timeLimit ? `${testData.timeLimit} minut` : 'Bez limitu';
    document.getElementById('summaryQuestions').textContent = `${selectedQuestions.size} otázek`;
}

// ============================================
// FULL PREVIEW UPDATE
// ============================================
function updateFullPreview() {
    let previewHTML = `
        <div class="preview-test-header">
            <div class="preview-test-title">${testData.name}</div>
            <div class="preview-meta">
                <div class="preview-meta-item">
                    <i class="bi bi-book"></i>
                    <span>${testData.fieldName}</span>
                </div>
                <div class="preview-meta-item">
                    <i class="bi bi-clock"></i>
                    <span>${testData.timeLimit ? testData.timeLimit + ' minut' : 'Bez limitu'}</span>
                </div>
                <div class="preview-meta-item">
                    <i class="bi bi-list-check"></i>
                    <span>${selectedQuestions.size} otázek</span>
                </div>
            </div>
        </div>
    `;

    let questionNumber = 1;
    selectedQuestions.forEach(questionId => {
        const questionCard = document.querySelector(`[data-question-id="${questionId}"]`);
        const questionTitle = questionCard.querySelector('.form-check-label').textContent;
        const questionDesc = questionCard.querySelector('.question-card-body small').textContent;

        previewHTML += `
            <div class="preview-question">
                <div class="mb-2">
                    <span class="preview-question-number">${questionNumber}</span>
                    <strong>${questionTitle}</strong>
                </div>
                <p class="text-muted mb-3">${questionDesc}</p>
                <div class="preview-option">A) Možnost A</div>
                <div class="preview-option">B) Možnost B</div>
                <div class="preview-option">C) Možnost C</div>
                <div class="preview-option">D) Možnost D</div>
            </div>
        `;
        questionNumber++;
    });

    document.getElementById('previewContent').innerHTML = previewHTML;
}

// ============================================
// SUBMIT TEST
// ============================================
async function submitTest() {
    if (!validateCurrentStep()) {
        return;
    }

    const submitBtn = document.getElementById('submitBtn');
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Vytváření...';

    try {
        const response = await fetch('/Test/CreateTest', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                Name: testData.name,
                StudentFieldId: testData.fieldId,
                Description: testData.description,
                TimeLimit: testData.timeLimit,
                QuestionIds: testData.questionIds
            })
        });

        const result = await response.json();

        if (response.ok) {
            toastr.success(result.message || 'Test byl úspěšně vytvořen!');
            setTimeout(() => {
                window.location.href = '/Test/Index';
            }, 1500);
        } else {
            toastr.error(result.message || 'Chyba při vytváření testu.');
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="bi bi-check-circle me-2"></i>Vytvořit test';
        }
    } catch (error) {
        console.error('Error:', error);
        toastr.error('Nastala chyba při komunikaci se serverem.');
        submitBtn.disabled = false;
        submitBtn.innerHTML = '<i class="bi bi-check-circle me-2"></i>Vytvořit test';
    }
}

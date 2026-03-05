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
var $container = $('.test-create-container');

$('#studentField').select2({
    theme: 'bootstrap-5',
    width: '100%',
    placeholder: '-- Vyberte predmet --',
    allowClear: true,
    dropdownParent: $container
});

$('#questionTypeFilter').select2({
    theme: 'bootstrap-5',
    width: '100%',
    placeholder: 'Vsechny typy',
    allowClear: true,
    dropdownParent: $container
});

    // Wire Select2 change events into existing handlers
    $('#studentField').on('select2:select select2:clear', updatePreview);
    $('#questionTypeFilter').on('select2:select select2:clear change', filterQuestions);

    $('#questionFieldFilter').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: 'Vsechny predmety',
        allowClear: true,
        dropdownParent: $container
    });
    $('#questionFieldFilter').on('select2:select select2:clear change', filterQuestions);
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
    document.getElementById('questionFieldFilter').addEventListener('change', filterQuestions);

    // Question selection
    document.querySelectorAll('.question-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', handleQuestionSelection);
    });

    document.getElementById('clearSelection').addEventListener('click', clearAllSelections);

    // Question card click (but not on toggle button)
    document.querySelectorAll('.question-card').forEach(card => {
        card.addEventListener('click', function(e) {
            if (e.target.type === 'checkbox' || e.target.closest('.toggle-options-btn')) return;
            const checkbox = this.querySelector('.question-checkbox');
            checkbox.checked = !checkbox.checked;
            checkbox.dispatchEvent(new Event('change'));
        });
    });

    // Toggle options panel
    document.querySelectorAll('.toggle-options-btn').forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.stopPropagation();
            const card = this.closest('.question-card');
            const panel = card.querySelector('.question-options-panel');
            const isOpen = panel.style.display !== 'none';
            panel.style.display = isOpen ? 'none' : 'block';
            this.classList.toggle('open', !isOpen);
        });
    });

    // Carousel nav
    document.getElementById('prevPreviewBtn').addEventListener('click', function() {
        if (previewIndex > 0) { previewIndex--; renderPreviewSlide(); }
    });
    document.getElementById('nextPreviewBtn').addEventListener('click', function() {
        if (previewIndex < previewQuestions.length - 1) { previewIndex++; renderPreviewSlide(); }
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
        } else {
            document.getElementById('previewNav').classList.add('d-none');
        }
    }
}

function prevStep() {
    if (currentStep > 1) {
        document.getElementById(`step${currentStep}`).classList.remove('active');
        document.querySelector(`[data-step="${currentStep}"]`).classList.remove('active');

        if (currentStep === 3) {
            document.getElementById('previewNav').classList.add('d-none');
        }

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
    updateSelectedQuestionsList();
}

function updateSelectedCount() {
    document.getElementById('selectedCount').textContent = selectedQuestions.size;
}

function updateSelectedQuestionsList() {
    var listEl = document.getElementById('selectedQuestionsList');
    var itemsEl = document.getElementById('selectedQuestionsItems');
    if (!listEl || !itemsEl) return;

    if (selectedQuestions.size === 0) {
        listEl.style.display = 'none';
        return;
    }

    listEl.style.display = '';
    var html = '';
    var idx = 1;
    selectedQuestions.forEach(function(qId) {
        var card = document.querySelector('[data-question-id="' + qId + '"]');
        if (!card) return;
        var title = card.querySelector('.form-check-label').textContent.trim();
        var badge = card.querySelector('.badge');
        var badgeText = badge ? badge.textContent.trim() : '';
        html += '<div class="selected-question-item d-flex align-items-center gap-2 py-1 px-2 mb-1 rounded" style="background:#f8f9fa;">'
            + '<span class="badge bg-orange text-white" style="min-width:24px;">' + idx + '</span>'
            + '<span class="flex-grow-1 small">' + title + '</span>'
            + (badgeText ? '<span class="badge bg-info small">' + badgeText + '</span>' : '')
            + '<button type="button" class="btn btn-sm btn-link text-danger p-0 ms-1 remove-selected-btn" data-qid="' + qId + '" title="Odebrat">'
            + '<i class="bi bi-x-circle"></i></button>'
            + '</div>';
        idx++;
    });
    itemsEl.innerHTML = html;

    // Wire remove buttons
    itemsEl.querySelectorAll('.remove-selected-btn').forEach(function(btn) {
        btn.addEventListener('click', function(e) {
            e.stopPropagation();
            var removeId = parseInt(this.dataset.qid);
            var cb = document.querySelector('.question-checkbox[value="' + removeId + '"]');
            if (cb) { cb.checked = false; cb.dispatchEvent(new Event('change')); }
        });
    });
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
    updateSelectedQuestionsList();
}

// ============================================
// QUESTION FILTERING
// ============================================
function filterQuestions() {
    const searchTerm = document.getElementById('questionSearch').value.toLowerCase();
    const typeFilter = $('#questionTypeFilter').val() || '';
    const fieldFilter = $('#questionFieldFilter').val() || '';
    let visibleCount = 0;

    document.querySelectorAll('.question-card').forEach(card => {
        const name = card.dataset.questionName;
        const type = card.dataset.questionType;
        const field = card.dataset.questionField;

        const matchesSearch = !searchTerm || name.includes(searchTerm);
        const matchesType = !typeFilter || type === typeFilter;
        const matchesField = !fieldFilter || field === fieldFilter;

        if (matchesSearch && matchesType && matchesField) {
            card.style.display = '';
            visibleCount++;
        } else {
            card.style.display = 'none';
        }
    });

    document.getElementById('availableCount').textContent = visibleCount + ' otazek';

    var emptyMsg = document.getElementById('noQuestionsMessage');
    if (emptyMsg) {
        emptyMsg.style.display = visibleCount === 0 ? '' : 'none';
    }
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
var name = document.getElementById('testName').value || 'Nazev testu';
var fieldName = $('#studentField option:selected').text() || 'Predmet';
    var description = document.getElementById('description').value.trim();
    var timeLimit = document.getElementById('timeLimit').value;
    var timeLimitText = timeLimit ? `${timeLimit} minut` : 'Bez limitu';

    var descHTML = description
        ? `<div class="preview-description mt-3"><i class="bi bi-text-paragraph text-muted me-2"></i><span class="text-muted">${description}</span></div>`
        : '';

    var previewHTML = `
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
            ${descHTML}
        </div>
        <p class="text-muted text-center">Po vyberu otazek se zde zobrazi nahled testu</p>
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
// FULL PREVIEW UPDATE (carousel)
// ============================================
let previewQuestions = [];
let previewIndex = 0;

function updateFullPreview() {
    previewQuestions = [];
    previewIndex = 0;

    let questionNumber = 1;
    selectedQuestions.forEach(questionId => {
        const questionCard = document.querySelector(`[data-question-id="${questionId}"]`);
        const questionTitle = questionCard.querySelector('.form-check-label').textContent.trim();
        const questionDesc = questionCard.querySelector('.question-card-body small').textContent.trim();
        let options = [];
        try { options = JSON.parse(questionCard.dataset.questionOptions || '[]'); } catch(e) {}

        previewQuestions.push({
            number: questionNumber++,
            title: questionTitle,
            description: questionDesc,
            options: options
        });
    });

    // Show carousel nav
    const nav = document.getElementById('previewNav');
    if (previewQuestions.length > 0) {
        nav.classList.remove('d-none');
    } else {
        nav.classList.add('d-none');
    }

    renderPreviewHeader();
    renderPreviewSlide();
}

function renderPreviewHeader() {
var descHTML = testData.description
    ? '<div class="preview-description mt-2"><i class="bi bi-text-paragraph text-muted me-2"></i><span class="text-muted">' + testData.description + '</span></div>'
    : '';

let headerHTML = `
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
                <span>${selectedQuestions.size} otazek</span>
            </div>
        </div>
        ${descHTML}
    </div>
`;

    const contentEl = document.getElementById('previewContent');
    // Keep header + slide container
    contentEl.innerHTML = headerHTML + '<div id="previewSlide"></div>';
}

function renderPreviewSlide() {
    if (previewQuestions.length === 0) return;

    const q = previewQuestions[previewIndex];
    const letters = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];

    let optionsHTML = '';
    q.options.forEach((opt, j) => {
        const correctClass = opt.isCorrect ? ' correct' : '';
        optionsHTML += `
            <div class="preview-option${correctClass}">
                <strong class="me-2">${letters[j] || (j+1)})</strong>
                ${opt.text}
                ${opt.isCorrect ? '<i class="bi bi-check-circle-fill text-success ms-auto"></i>' : ''}
            </div>`;
    });

    const slideHTML = `
        <div class="preview-question preview-slide">
            <div class="mb-2">
                <span class="preview-question-number">${q.number}</span>
                <strong>${q.title}</strong>
            </div>
            <p class="text-muted mb-3">${q.description}</p>
            ${optionsHTML}
        </div>
    `;

    const slideEl = document.getElementById('previewSlide');
    if (slideEl) slideEl.innerHTML = slideHTML;

    // Update counter
    document.getElementById('previewCounter').textContent =
        `${previewIndex + 1}/${previewQuestions.length}`;
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

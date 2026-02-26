// ============================================
// STATE
// ============================================
const LOGIN_ID = '@Html.Raw(Model.LoginId)';
const TOTAL_QUESTIONS = @Model.Questions.Count;
const ELAPSED_AT_LOAD = Math.max(0, @((int)(DateTime.Now - Model.StartedAt).TotalSeconds));
const TIME_LIMIT_SEC = @(Model.TimeLimitMinutes.HasValue ? Model.TimeLimitMinutes.Value * 60 : -1);
const SAVED_QUESTION_INDEX = @Model.CurrentQuestionIndex;
const pageLoadedAt = Date.now();
let currentQuestion = SAVED_QUESTION_INDEX;
let answers = {};
let autoSaveInterval;
let timerExpired = false;
let saveTimeout = null;
let saving = false;

// ============================================
// INIT - load existing answers, restore position
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    // Load existing answers (Html.Raw prevents Razor HTML-encoding the JSON)
    try {
        @foreach(var ans in Model.ExistingAnswers)
{
    <text>
        answers[@ans.QuestionId] = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(ans.SelectedOptions));
    </text>
}
            } catch (e) {
    console.error('Error loading saved answers:', e);
}

updateNavButtons();
updateAnsweredCount();
startTimer();

// Restore question position
if (SAVED_QUESTION_INDEX > 0 && SAVED_QUESTION_INDEX < TOTAL_QUESTIONS) {
    goToQuestion(SAVED_QUESTION_INDEX);
}

// Auto-save every 30s as a safety net
autoSaveInterval = setInterval(saveProgress, 30000);

// Save on tab switch
document.addEventListener('visibilitychange', function () {
    if (document.hidden) saveProgressSync();
});

// Save before leaving (reliable with sendBeacon)
window.addEventListener('beforeunload', function () {
    saveProgressSync();
});
        });

// ============================================
// NAVIGATION
// ============================================
function goToQuestion(index) {
    if (index < 0 || index >= TOTAL_QUESTIONS) return;

    document.querySelectorAll('.question-card').forEach(c => c.classList.remove('active'));
    document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));

    var card = document.querySelector('.question-card[data-index="' + index + '"]');
    var navBtn = document.querySelector('.nav-btn[data-index="' + index + '"]');

    if (card) card.classList.add('active');
    if (navBtn) navBtn.classList.add('active');

    var finishSection = document.getElementById('finishSection');
    if (finishSection) finishSection.style.display = 'none';

    currentQuestion = index;

    window.scrollTo({ top: 0, behavior: 'smooth' });

    // Save position after navigation
    debouncedSave();
}

// ============================================
// OPTION SELECTION
// ============================================
function selectOption(el, questionIndex) {
    var card = el.closest('.question-card');
    if (!card) return;

    var questionId = parseInt(card.dataset.questionId);
    if (isNaN(questionId)) return;

    // Single-choice: deselect all, select this one
    card.querySelectorAll('.option-item').forEach(o => o.classList.remove('selected'));
    el.classList.add('selected');

    var selectedTexts = [];
    card.querySelectorAll('.option-item.selected').forEach(o => {
        selectedTexts.push(o.dataset.text);
    });

    answers[questionId] = selectedTexts;
    updateNavButtons();
    updateAnsweredCount();

    // Save immediately after answering
    debouncedSave();
}

// ============================================
// UI UPDATES
// ============================================
function updateNavButtons() {
    document.querySelectorAll('.question-card').forEach(card => {
        var idx = parseInt(card.dataset.index);
        var qId = parseInt(card.dataset.questionId);
        var navBtn = document.querySelector('.nav-btn[data-index="' + idx + '"]');
        if (!navBtn) return;

        if (answers[qId] && answers[qId].length > 0) {
            navBtn.classList.add('answered');
        } else {
            navBtn.classList.remove('answered');
        }
    });
}

function updateAnsweredCount() {
    var count = Object.values(answers).filter(a => a && a.length > 0).length;
    document.getElementById('answeredCount').textContent = count;
}

function showSaveIndicator() {
    var el = document.getElementById('saveIndicator');
    el.style.opacity = '1';
    setTimeout(function () { el.style.opacity = '0'; }, 1500);
}

// ============================================
// TIMER
// ============================================
function startTimer() {
    var timerEl = document.getElementById('timer');
    var timerWrap = document.getElementById('timerWrap');

    function update() {
        var clientDelta = Math.floor((Date.now() - pageLoadedAt) / 1000);
        var elapsedSec = ELAPSED_AT_LOAD + clientDelta;

        if (TIME_LIMIT_SEC > 0) {
            var remaining = Math.max(0, TIME_LIMIT_SEC - elapsedSec);
            var mins = Math.floor(remaining / 60);
            var secs = remaining % 60;
            timerEl.textContent = String(mins).padStart(2, '0') + ':' + String(secs).padStart(2, '0');

            if (remaining <= 60 && remaining > 0) {
                timerWrap.style.color = '#ff4444';
                timerWrap.style.fontWeight = '700';
            }

            if (remaining <= 0 && !timerExpired) {
                timerExpired = true;
                timerEl.textContent = '00:00';
                toastr.warning('Čas vypršel! Test bude automaticky odevzdán.');
                setTimeout(function () { autoFinish(); }, 1000);
            }
        } else {
            var mins = Math.floor(elapsedSec / 60);
            var secs = elapsedSec % 60;
            timerEl.textContent = String(mins).padStart(2, '0') + ':' + String(secs).padStart(2, '0');
        }
    }
    update();
    setInterval(update, 1000);
}

async function autoFinish() {
    clearInterval(autoSaveInterval);
    try {
        var res = await fetch('/Test/FinishTest', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ loginId: LOGIN_ID, answers: buildAnswerList(), currentQuestionIndex: currentQuestion })
        });
        var data = await res.json();
        if (res.ok) showResult(data);
        else toastr.error('Chyba při odevzdání.');
    } catch {
        toastr.error('Chyba komunikace.');
    }
}

// ============================================
// SAVE PROGRESS (debounced async + sync beacon)
// ============================================
function buildPayload() {
    return {
        loginId: LOGIN_ID,
        answers: buildAnswerList(),
        currentQuestionIndex: currentQuestion
    };
}

function debouncedSave() {
    if (saveTimeout) clearTimeout(saveTimeout);
    saveTimeout = setTimeout(saveProgress, 500);
}

async function saveProgress() {
    if (saving) return;
    saving = true;
    try {
        var res = await fetch('/Test/SaveProgress', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(buildPayload())
        });
        if (res.ok) showSaveIndicator();
    } catch (e) {
        console.warn('Auto-save failed:', e);
    } finally {
        saving = false;
    }
}

// Synchronous save for beforeunload / visibilitychange (uses sendBeacon)
function saveProgressSync() {
    try {
        var payload = JSON.stringify(buildPayload());
        navigator.sendBeacon('/Test/SaveProgress', new Blob([payload], { type: 'application/json' }));
    } catch (e) {
        console.warn('Beacon save failed:', e);
    }
}

function buildAnswerList() {
    var list = [];
    document.querySelectorAll('.question-card').forEach(card => {
        var qId = parseInt(card.dataset.questionId);
        list.push({
            questionId: qId,
            selectedOptions: answers[qId] || []
        });
    });
    return list;
}

// ============================================
// FINISH
// ============================================
function showFinishConfirm() {
    var count = Object.values(answers).filter(a => a && a.length > 0).length;
    document.getElementById('finalAnswered').textContent = count;

    document.querySelectorAll('.question-card').forEach(c => c.classList.remove('active'));
    document.getElementById('finishSection').style.display = 'block';

    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function hideFinishConfirm() {
    document.getElementById('finishSection').style.display = 'none';
    goToQuestion(currentQuestion);
}

async function finishTest() {
    var btn = document.getElementById('finishBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Odevzdávám...';

    clearInterval(autoSaveInterval);

    try {
        var res = await fetch('/Test/FinishTest', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ loginId: LOGIN_ID, answers: buildAnswerList(), currentQuestionIndex: currentQuestion })
        });

        var data = await res.json();

        if (res.ok) {
            showResult(data);
        } else {
            toastr.error(data.message || 'Chyba při odevzdání.');
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-check-circle me-2"></i>Odevzdat';
        }
    } catch (e) {
        toastr.error('Chyba komunikace se serverem.');
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-check-circle me-2"></i>Odevzdat';
    }
}

function showResult(data) {
    var overlay = document.getElementById('resultOverlay');
    var pct = data.successPct;

    document.getElementById('resultScore').textContent = pct + '%';
    document.getElementById('resultDetail').textContent =
        'Správně ' + data.correct + ' z ' + data.total + ' otázek';

    if (pct >= 80) {
        document.getElementById('resultIcon').innerHTML = '<i class="bi bi-trophy-fill" style="color:#4caf50"></i>';
        document.getElementById('resultScore').style.color = '#4caf50';
        document.getElementById('resultMessage').textContent = 'Výborně!';
    } else if (pct >= 50) {
        document.getElementById('resultIcon').innerHTML = '<i class="bi bi-hand-thumbs-up-fill" style="color:#ff9800"></i>';
        document.getElementById('resultScore').style.color = '#ff9800';
        document.getElementById('resultMessage').textContent = 'Dobře!';
    } else {
        document.getElementById('resultIcon').innerHTML = '<i class="bi bi-book-fill" style="color:#f44336"></i>';
        document.getElementById('resultScore').style.color = '#f44336';
        document.getElementById('resultMessage').textContent = 'Je třeba zapracovat';
    }

    overlay.classList.add('show');
}
// ============================================
// EXAMPLE QUESTIONS
// ============================================
const QUESTIONS = [
    {
        id: 1,
        header: "Hlavní město České republiky",
        description: "Které město je hlavním městem České republiky?",
        type: "Uzavřená otázka",
        options: [
            { text: "Brno", correct: false },
            { text: "Praha", correct: true },
            { text: "Ostrava", correct: false },
            { text: "Plzeň", correct: false }
        ]
    },
    {
        id: 2,
        header: "Výsledek výrazu",
        description: "Kolik je 15 × 4 + 20?",
        type: "Uzavřená otázka",
        options: [
            { text: "60", correct: false },
            { text: "80", correct: true },
            { text: "100", correct: false },
            { text: "75", correct: false }
        ]
    },
    {
        id: 3,
        header: "Chemická značka vody",
        description: "Jaký je chemický vzorec vody?",
        type: "Uzavřená otázka",
        options: [
            { text: "CO₂", correct: false },
            { text: "NaCl", correct: false },
            { text: "H₂O", correct: true },
            { text: "O₂", correct: false }
        ]
    },
    {
        id: 4,
        header: "Programovací jazyk",
        description: "Který z následujících NENÍ programovací jazyk?",
        type: "Uzavřená otázka",
        options: [
            { text: "Python", correct: false },
            { text: "HTML", correct: true },
            { text: "Java", correct: false },
            { text: "C#", correct: false }
        ]
    },
    {
        id: 5,
        header: "Autor Babičky",
        description: "Kdo napsal román Babička?",
        type: "Uzavřená otázka",
        options: [
            { text: "Karel Čapek", correct: false },
            { text: "Božena Němcová", correct: true },
            { text: "Alois Jirásek", correct: false },
            { text: "Jan Neruda", correct: false }
        ]
    }
];

const LETTERS = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];
let answers = {};
let currentQuestion = 0;
let evaluated = false;

// ============================================
// BUILD UI
// ============================================
function buildUI() {
    const nav = document.getElementById('navGrid');
    const area = document.getElementById('questionArea');
    nav.innerHTML = '';
    area.innerHTML = '';

    QUESTIONS.forEach((q, i) => {
        // Nav button
        nav.innerHTML += `<button class="nav-btn ${i === 0 ? 'active' : ''}" data-index="${i}" onclick="goTo(${i})">${i + 1}</button>`;

        // Options
        let optionsHtml = '';
        q.options.forEach((opt, j) => {
            optionsHtml += `
                    <div class="option-item" data-qid="${q.id}" data-text="${opt.text}" onclick="selectOption(this, ${i})">
                        <div class="option-marker">${LETTERS[j]}</div>
                        <div class="option-text">${opt.text}</div>
                    </div>`;
        });

        // Footer nav
        const prevBtn = i > 0
            ? `<button class="btn btn-outline-secondary" onclick="goTo(${i - 1})"><i class="bi bi-arrow-left me-2"></i>Předchozí</button>`
            : '<div></div>';
        const nextBtn = i < QUESTIONS.length - 1
            ? `<button class="btn-orange" onclick="goTo(${i + 1})">Další<i class="bi bi-arrow-right ms-2"></i></button>`
            : `<button class="btn-orange" onclick="showFinish()"><i class="bi bi-check-circle me-2"></i>Vyhodnotit</button>`;

        area.innerHTML += `
                <div class="question-card ${i === 0 ? 'active' : ''}" data-index="${i}" data-qid="${q.id}">
                    <div class="d-flex align-items-center mb-3">
                        <span class="question-number">${i + 1}</span>
                        <span class="question-type-badge">${q.type}</span>
                    </div>
                    <div class="question-header">${q.header}</div>
                    <div class="question-desc">${q.description}</div>
                    <div class="options-list">${optionsHtml}</div>
                    <div class="question-footer">${prevBtn}${nextBtn}</div>
                </div>`;
    });

    // Finish section
    area.innerHTML += `
            <div class="finish-section" id="finishSection">
                <i class="bi bi-send-check" style="font-size: 4rem; color: #ff8a00;"></i>
                <h3 class="mt-3">Vyhodnotit ukázkový test?</h3>
                <p class="text-muted">
                    Zodpovězeno <strong><span id="finalAnswered">0</span></strong> z <strong>${QUESTIONS.length}</strong> otázek.
                    <br>Po vyhodnocení uvidíš správné odpovědi přímo u otázek.
                </p>
                <div class="d-flex gap-3 justify-content-center mt-3 flex-wrap">
                    <button class="btn btn-outline-secondary btn-lg" onclick="hideFinish()">
                        <i class="bi bi-arrow-left me-2"></i>Zpět k testu
                    </button>
                    <button class="btn-orange btn-lg" onclick="evaluateTest()">
                        <i class="bi bi-check-circle me-2"></i>Vyhodnotit
                    </button>
                </div>
            </div>`;

    document.getElementById('evalNavBtn').style.display = '';
}

// ============================================
// NAVIGATION
// ============================================
function goTo(index) {
    if (index < 0 || index >= QUESTIONS.length) return;
    document.querySelectorAll('.question-card').forEach(c => c.classList.remove('active'));
    document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));
    document.querySelector(`.question-card[data-index="${index}"]`).classList.add('active');
    document.querySelector(`.nav-btn[data-index="${index}"]`).classList.add('active');
    const finish = document.getElementById('finishSection');
    if (finish) finish.style.display = 'none';
    currentQuestion = index;
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// ============================================
// SELECTION
// ============================================
function selectOption(el, qIndex) {
    if (evaluated) return;
    const card = el.closest('.question-card');
    card.querySelectorAll('.option-item').forEach(o => o.classList.remove('selected'));
    el.classList.add('selected');
    answers[QUESTIONS[qIndex].id] = el.dataset.text;
    updateNav();
}

function updateNav() {
    QUESTIONS.forEach((q, i) => {
        const btn = document.querySelector(`.nav-btn[data-index="${i}"]`);
        btn.classList.toggle('answered', !!answers[q.id]);
    });
    document.getElementById('answeredCount').textContent = Object.keys(answers).length;
}

// ============================================
// FINISH / EVALUATE
// ============================================
function showFinish() {
    if (evaluated) return;
    document.getElementById('finalAnswered').textContent = Object.keys(answers).length;
    document.querySelectorAll('.question-card').forEach(c => c.classList.remove('active'));
    document.getElementById('finishSection').style.display = 'block';
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function hideFinish() {
    document.getElementById('finishSection').style.display = 'none';
    goTo(currentQuestion);
}

function evaluateTest() {
    evaluated = true;
    let correct = 0;

    QUESTIONS.forEach((q, i) => {
        const card = document.querySelector(`.question-card[data-index="${i}"]`);
        const selected = answers[q.id] || null;

        card.querySelectorAll('.option-item').forEach(opt => {
            const text = opt.dataset.text;
            const optData = q.options.find(o => o.text === text);
            const isCorrect = optData?.correct;
            const isSelected = text === selected;

            opt.classList.remove('selected');

            if (isCorrect) {
                opt.classList.add('correct');
            } else if (isSelected && !isCorrect) {
                opt.classList.add('wrong');
            }
        });

        if (selected && q.options.find(o => o.text === selected)?.correct) {
            correct++;
        }
    });

    const pct = Math.round((correct / QUESTIONS.length) * 100);

    // Show result overlay
    document.getElementById('resultScore').textContent = pct + '%';
    document.getElementById('resultDetail').textContent =
        `Správně ${correct} z ${QUESTIONS.length} otázek`;

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

    document.getElementById('finishSection').style.display = 'none';
    document.getElementById('resultOverlay').classList.add('show');
}

// ============================================
// RESET
// ============================================
function resetTest() {
    answers = {};
    currentQuestion = 0;
    evaluated = false;
    document.getElementById('resultOverlay').classList.remove('show');
    buildUI();
}

// ============================================
// INIT
// ============================================
document.addEventListener('DOMContentLoaded', buildUI);
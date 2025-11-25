let lottieAnimation;

function initLottieAnimation() {
    lottieAnimation = lottie.loadAnimation({
        container: document.getElementById('lottie-container'),
        renderer: 'svg', // 'svg', 'canvas', 'html'
        loop: true,
        autoplay: true,
        path: 'animations\loading.json' // Příklad URL
    });
    }

function updateProgress() {
    const progressFill = document.getElementById('progress-fill');
    let progress = 0;
            
    const interval = setInterval(() => {
    progress += Math.random() * 15;
            if (progress >= 100) {
    progress = 100;
    progressFill.style.width = progress + '%';
    clearInterval(interval);
                setTimeout(() => {
    hideLoading();
                }, 500);
            } else {
    progressFill.style.width = progress + '%';
            }
        }, 200);
    }
function hideLoading() {
        const overlay = document.getElementById('loading-overlay');
overlay.classList.add('hidden');

// Zastavení animace pro úsporu výkonu
if (lottieAnimation) {
    lottieAnimation.pause();
        }

        // Úplné odebrání po animaci
        setTimeout(() => {
    overlay.style.display = 'none';
        }, 500);
    }

// Zobrazení loading screenu (pro testování)
function showLoading() {
        const overlay = document.getElementById('loading-overlay');
overlay.style.display = 'flex';
overlay.classList.remove('hidden');

if (lottieAnimation) {
    lottieAnimation.play();
        }

// Reset progress baru
document.getElementById('progress-fill').style.width = '0%';
updateProgress();
    }

// Inicializace při načtení stránky
document.addEventListener('DOMContentLoaded', function() {
    initLottieAnimation();

// Simulace načítání stránky
updateProgress();

        // Pro ASP.NET Core - můžeš skrýt loading až po AJAX volání
        // hideLoading(); // Zavolej když je vše načteno
    });

showLoading()
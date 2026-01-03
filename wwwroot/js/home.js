(function () {
    const success = 68; // % úspěšnost
    const fail = 100 - success;

    // Center text plugin
    const centerText = {
        id: 'centerText',
        afterDraw(chart, args, options) {
            const { ctx, chartArea: { width, height } } = chart;
            ctx.save();
            ctx.font = `700 ${options.size || 48}px Inter, system-ui, -apple-system, Segoe UI, Roboto, Arial`;
            ctx.fillStyle = options.color || '#222';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText(`${Math.round(options.value)}%`, width / 2, height / 2);
            ctx.restore();
        }
    };

    const ctx = document.getElementById('successDonut');
    if (!ctx) return;

    let centerValue = 0;

    const chart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Úspěšnost', 'Neúspěšnost'],
            datasets: [{
                data: [0, 100], // animate from 0
                backgroundColor: ['#ff8a00', '#4b4b4b'],
                borderWidth: 0,
                hoverOffset: 6
            }]
        },
        options: {
            responsive: true,
            cutout: '70%',
            animation: {
                duration: 1200,
                easing: 'easeOutQuart',
                onProgress: (anim) => {
                    const p = anim.currentStep / anim.numSteps;
                    centerValue = success * p;
                    chart.options.plugins.centerText.value = centerValue;
                }
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: (ctx) => `${ctx.label}: ${ctx.parsed}%`
                    }
                },
                centerText: { value: centerValue, size: 44, color: '#222' }
            }
        },
        plugins: [centerText]
    });

    // Animate dataset values and UI with GSAP
    requestAnimationFrame(() => {
        chart.data.datasets[0].data = [success, fail];
        chart.update();

        // Subtle page entrance
        gsap.from('.hero-title-top, .hero-title-bottom', { y: 20, opacity: 0, duration: 0.6, stagger: 0.1, ease: 'power2.out' });
        gsap.from('.btn-primary', { y: 12, opacity: 0, duration: 0.5, delay: 0.2, ease: 'power2.out' });
        gsap.from('.donut-wrap', { scale: 0.9, opacity: 0, duration: 0.6, delay: 0.25, ease: 'back.out(1.7)' });
        gsap.from('#testsTable tbody tr', { y: 8, opacity: 0, duration: 0.4, stagger: 0.05, ease: 'power1.out', delay: 0.15 });
    });

    // Search filter
    const input = document.getElementById('searchInput');
    const rows = () => Array.from(document.querySelectorAll('#testsTable tbody tr'));
    input?.addEventListener('input', (e) => {
        const q = e.target.value.toLowerCase().trim();
        rows().forEach(r => {
            const text = r.innerText.toLowerCase();
            r.style.display = text.includes(q) ? '' : 'none';
        });
    });
})();
document.addEventListener('DOMContentLoaded', function () {
    // Animate stat numbers
    document.querySelectorAll('.stat-value[data-target]').forEach(el => {
        const target = parseFloat(el.dataset.target);
        const suffix = el.dataset.suffix || '';
        const duration = 1200;
        const steps = 50;
        const increment = target / steps;
        let current = 0;
        let step = 0;
        const interval = setInterval(() => {
            current += increment;
            step++;
            if (step >= steps) {
                clearInterval(interval);
                el.textContent = (Number.isInteger(target) ? target : target.toFixed(1)) + suffix;
            } else {
                el.textContent = Math.floor(current) + suffix;
            }
        }, duration / steps);
    });

    // Success rate chart
    const ctx = document.getElementById('successRateChart');
    if (ctx) {
        const labels = @Html.Raw(monthlyLabels);
        const values = @Html.Raw(monthlyValues);

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels.length ? labels : ['Žádná data'],
                datasets: [
                    {
                        label: 'Průměrná úspěšnost (%)',
                        data: values.length ? values : [0],
                        borderColor: '#ff8a00',
                        backgroundColor: 'rgba(255, 138, 0, 0.1)',
                        tension: 0.4,
                        fill: true,
                        pointRadius: 5,
                        pointHoverRadius: 7,
                        pointBackgroundColor: '#ff8a00',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2
                    },
                    {
                        label: 'Cílová hodnota',
                        data: Array(Math.max(labels.length, 1)).fill(75),
                        borderColor: '#198754',
                        backgroundColor: 'transparent',
                        borderDash: [5, 5],
                        tension: 0,
                        fill: false,
                        pointRadius: 0
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 15, usePointStyle: true } },
                    tooltip: {
                        mode: 'index', intersect: false,
                        backgroundColor: 'rgba(0,0,0,.8)', padding: 12, cornerRadius: 8,
                        callbacks: { label: ctx => (ctx.dataset.label || '') + ': ' + ctx.parsed.y + '%' }
                    }
                },
                scales: {
                    y: { beginAtZero: true, max: 100, ticks: { callback: v => v + '%' }, grid: { color: 'rgba(0,0,0,.05)' } },
                    x: { grid: { display: false } }
                }
            }
        });
    }
});
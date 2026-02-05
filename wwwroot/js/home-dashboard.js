// ============================================
// TEACHER DASHBOARD - CHART.JS INITIALIZATION
// ============================================

document.addEventListener('DOMContentLoaded', function() {
    
    // Success Rate Chart
    const ctx = document.getElementById('successRateChart');
    
    if (ctx) {
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Leden', 'Únor', 'Březen', 'Duben', 'Květen', 'Červen', 'Červenec'],
                datasets: [
                    {
                        label: 'Průměrná úspěšnost (%)',
                        data: [72, 75, 78, 76, 82, 80, 85],
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
                        data: [75, 75, 75, 75, 75, 75, 75],
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
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            padding: 15,
                            font: {
                                size: 12
                            },
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        mode: 'index',
                        intersect: false,
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        padding: 12,
                        cornerRadius: 8,
                        titleFont: {
                            size: 14,
                            weight: 'bold'
                        },
                        bodyFont: {
                            size: 13
                        },
                        callbacks: {
                            label: function(context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                if (context.parsed.y !== null) {
                                    label += context.parsed.y + '%';
                                }
                                return label;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        max: 100,
                        ticks: {
                            callback: function(value) {
                                return value + '%';
                            },
                            font: {
                                size: 11
                            }
                        },
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)',
                            drawBorder: false
                        }
                    },
                    x: {
                        grid: {
                            display: false,
                            drawBorder: false
                        },
                        ticks: {
                            font: {
                                size: 11
                            }
                        }
                    }
                },
                interaction: {
                    mode: 'nearest',
                    axis: 'x',
                    intersect: false
                }
            }
        });
    }

    // Animate numbers on page load
    animateNumbers();
});

// ============================================
// NUMBER ANIMATION
// ============================================
function animateNumbers() {
    const statValues = document.querySelectorAll('.stat-value');
    
    statValues.forEach(stat => {
        const text = stat.textContent;
        const number = parseInt(text);
        
        // Skip if not a number
        if (isNaN(number)) return;
        
        const duration = 1500; // ms
        const steps = 60;
        const increment = number / steps;
        let current = 0;
        let step = 0;
        
        const interval = setInterval(() => {
            current += increment;
            step++;
            
            if (step >= steps) {
                clearInterval(interval);
                stat.textContent = text; // Original text with % or other suffix
            } else {
                stat.textContent = Math.floor(current) + text.replace(/\d+/, '');
            }
        }, duration / steps);
    });
}

// ============================================
// REAL-TIME UPDATES (Optional - for future)
// ============================================
function fetchDashboardData() {
    // TODO: Implement API call to get real data
    // fetch('/api/dashboard/stats')
    //     .then(response => response.json())
    //     .then(data => {
    //         updateDashboard(data);
    //     });
}

function updateDashboard(data) {
    // TODO: Update dashboard elements with real data
    console.log('Dashboard updated with:', data);
}

// Optional: Auto-refresh every 5 minutes
// setInterval(fetchDashboardData, 5 * 60 * 1000);

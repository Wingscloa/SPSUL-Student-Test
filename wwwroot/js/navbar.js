// ============================================
// MODERN NAVBAR - INTERACTIONS & ANIMATIONS
// ============================================

document.addEventListener('DOMContentLoaded', function() {
    initNavbar();
});

function initNavbar() {
    const navbar = document.getElementById('mainNavbar');
    if (!navbar) return;

    // Scroll detection - add/remove .scrolled class
    let lastScroll = 0;
    let ticking = false;

    window.addEventListener('scroll', function() {
        lastScroll = window.scrollY;

        if (!ticking) {
            window.requestAnimationFrame(function() {
                handleScroll();
                ticking = false;
            });
            ticking = true;
        }
    });

    function handleScroll() {
        if (lastScroll > 50) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
    }

    // Active page detection
    setActivePage();

    // Ripple effect on nav links
    addRippleEffect();

    // Keyboard navigation enhancement
    enhanceKeyboardNav();
}

// ============================================
// ACTIVE PAGE DETECTION
// ============================================
function setActivePage() {
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.nav-link:not(.PrimaryButton)');

    navLinks.forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase() || '';
        
        // Remove existing active class
        link.classList.remove('active');

        // Check if current page matches link
        if (href && currentPath.includes(href) && href !== '/') {
            link.classList.add('active');
        } else if (currentPath === '/' && href === '/') {
            link.classList.add('active');
        }
    });
}

// ============================================
// RIPPLE EFFECT
// ============================================
function addRippleEffect() {
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');

            this.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
}

// ============================================
// KEYBOARD NAVIGATION ENHANCEMENT
// ============================================
function enhanceKeyboardNav() {
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach((link, index) => {
        // Add keyboard support
        link.addEventListener('keydown', function(e) {
            // Arrow keys navigation
            if (e.key === 'ArrowRight') {
                e.preventDefault();
                const next = navLinks[index + 1] || navLinks[0];
                next.focus();
            } else if (e.key === 'ArrowLeft') {
                e.preventDefault();
                const prev = navLinks[index - 1] || navLinks[navLinks.length - 1];
                prev.focus();
            }
        });

        // Visual focus enhancement
        link.addEventListener('focus', function() {
            this.style.outline = '2px solid rgba(255, 138, 0, 0.5)';
            this.style.outlineOffset = '2px';
        });

        link.addEventListener('blur', function() {
            this.style.outline = 'none';
        });
    });
}

// ============================================
// NAVBAR HIDE ON SCROLL DOWN (Optional)
// ============================================
function initAutoHideNavbar() {
    const navbar = document.getElementById('mainNavbar');
    let lastScroll = 0;

    window.addEventListener('scroll', () => {
        const currentScroll = window.scrollY;

        if (currentScroll > lastScroll && currentScroll > 100) {
            // Scrolling down
            navbar.style.transform = 'translateY(-100%)';
        } else {
            // Scrolling up
            navbar.style.transform = 'translateY(0)';
        }

        lastScroll = currentScroll;
    });
}

// Uncomment to enable auto-hide navbar:
// initAutoHideNavbar();

// ============================================
// SMOOTH SCROLL TO TOP ON LOGO CLICK
// ============================================
const navbarBrand = document.querySelector('.navbar-brand');
if (navbarBrand) {
    navbarBrand.addEventListener('click', function(e) {
        // Only if we're on home page
        if (window.location.pathname === '/' || window.location.pathname.toLowerCase().includes('/home')) {
            e.preventDefault();
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        }
    });
}

// ============================================
// NOTIFICATION BADGE (Optional)
// ============================================
function updateNavBadge(linkSelector, count) {
    const link = document.querySelector(linkSelector);
    if (!link) return;

    let badge = link.querySelector('.badge');
    
    if (count > 0) {
        if (!badge) {
            badge = document.createElement('span');
            badge.className = 'badge bg-danger';
            link.appendChild(badge);
        }
        badge.textContent = count > 99 ? '99+' : count;
    } else if (badge) {
        badge.remove();
    }
}

// Example usage:
// updateNavBadge('.nav-link[href*="Detail"]', 5); // 5 pending reviews

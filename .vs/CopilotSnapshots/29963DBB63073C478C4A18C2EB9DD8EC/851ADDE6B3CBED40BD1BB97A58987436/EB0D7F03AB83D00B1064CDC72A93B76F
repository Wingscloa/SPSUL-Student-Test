// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Wait for document to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
    // Prevent any scrolling during animation
    document.body.style.overflow = 'hidden';
    
    // Allow scrolling after animation completes
    setTimeout(function() {
        document.body.style.overflow = 'auto';
    }, 4000); // Animation takes approximately 3.5s
    
    // Make sure the animation plays correctly even if page is refreshed
    setTimeout(function() {
        const letters = document.querySelectorAll('.logo-letter');
        letters.forEach(letter => {
            letter.classList.add('converge-letters');
        });
    }, 1500);
});

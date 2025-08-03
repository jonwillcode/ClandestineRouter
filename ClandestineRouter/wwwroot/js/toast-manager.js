// wwwroot/js/toast-manager.js
// Pure client-side toast management - no server callbacks

let activeTimers = new Map();

export function initializeToasts() {
    // Clear any existing timers
    clearAllTimers();

    // Find all toast elements
    const toasts = document.querySelectorAll('.feedback-toast[data-message-id]');

    toasts.forEach(toast => {
        const messageId = toast.getAttribute('data-message-id');
        const duration = parseInt(toast.getAttribute('data-duration') || '5000');
        const isPersistent = toast.getAttribute('data-persistent') === 'true';

        // Skip if already processed or persistent
        if (toast.classList.contains('toast-initialized') || isPersistent) {
            return;
        }

        // Mark as initialized
        toast.classList.add('toast-initialized');

        // Add slide-in animation
        toast.classList.add('toast-slide-in');

        // Set up auto-dismiss timer
        if (duration > 0) {
            const timer = setTimeout(() => {
                dismissToast(messageId);
            }, duration);

            activeTimers.set(messageId, timer);
        }

        //console.log(`Initialized toast ${messageId} with ${duration}ms duration`);
    });
}

export function closeToast(messageId) {
    // Clear timer if exists
    if (activeTimers.has(messageId)) {
        clearTimeout(activeTimers.get(messageId));
        activeTimers.delete(messageId);
    }

    // Animate out and remove
    dismissToast(messageId);
}

function dismissToast(messageId) {
    const toast = document.querySelector(`.feedback-toast[data-message-id="${messageId}"]`);
    if (!toast) return;

    //console.log(`Dismissing toast ${messageId}`);

    // Add fade-out animation
    toast.classList.add('toast-slide-out');

    // Remove from DOM after animation
    setTimeout(() => {
        if (toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }

        // Clean up timer reference
        activeTimers.delete(messageId);
    }, 300); // Match animation duration
}

function clearAllTimers() {
    activeTimers.forEach(timer => clearTimeout(timer));
    activeTimers.clear();
}

// Clean up on page unload
window.addEventListener('beforeunload', clearAllTimers);
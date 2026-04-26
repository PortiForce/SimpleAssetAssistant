const focusableQuery =
    'button:not([disabled]), [href], input:not([disabled]), ' +
    'select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

let activeCleanup = null;

export function openModal(modalElement, dotNetRef) {
    if (activeCleanup) {
        activeCleanup();
    }

    const previouslyFocused = document.activeElement;
    const focusable = Array.from(modalElement.querySelectorAll(focusableQuery));

    if (focusable.length > 0) {
        focusable[0].focus();
    } else {
        modalElement.focus();
    }

    function onKeyDown(e) {
        if (e.key === 'Escape') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('CancelFromJs');
            return;
        }

        if (e.key === 'Tab' && focusable.length > 0) {
            const first = focusable[0];
            const last = focusable[focusable.length - 1];

            if (e.shiftKey && document.activeElement === first) {
                e.preventDefault();
                last.focus();
            } else if (!e.shiftKey && document.activeElement === last) {
                e.preventDefault();
                first.focus();
            }
        }
    }

    document.addEventListener('keydown', onKeyDown);

    activeCleanup = () => {
        document.removeEventListener('keydown', onKeyDown);

        if (previouslyFocused && typeof previouslyFocused.focus === 'function') {
            previouslyFocused.focus();
        }

        activeCleanup = null;
    };
}

export function closeModal() {
    if (activeCleanup) {
        activeCleanup();
    }
}

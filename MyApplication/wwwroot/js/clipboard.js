// wwwroot/js/clipboard.js
window.copyToClipboard = (text) => {
    try {
        navigator.clipboard.writeText(text);
    } catch (e) {
        // Fallback for older browsers
        const ta = document.createElement("textarea");
        ta.value = text;
        ta.style.position = "fixed";
        ta.style.opacity = "0";
        document.body.appendChild(ta);
        ta.focus();
        ta.select();
        document.execCommand("copy");
        document.body.removeChild(ta);
    }
};

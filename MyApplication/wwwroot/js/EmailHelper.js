// wwwroot/js/emailhelper.js
window.downloadFileFromBase64 = (fileName, contentType, base64) => {
    const a = document.createElement('a');
    a.href = `data:${contentType};base64,${base64}`;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
};

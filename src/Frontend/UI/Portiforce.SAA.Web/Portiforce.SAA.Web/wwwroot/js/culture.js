window.portiforce = window.portiforce || {};
window.portiforce.getBrowserCulture = () => {
    const culture =
        (navigator.languages && navigator.languages.length
            ? navigator.languages[0]
            : navigator.language) || "en-US";

    return culture;
};
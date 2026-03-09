window.portiforce = window.portiforce || {};
window.portiforce.getBrowserCulture = () =>
	(navigator.languages && navigator.languages.length ? navigator.languages[0] : navigator.language) || "en-US";
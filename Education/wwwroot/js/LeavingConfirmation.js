function confirmLeaving() {
    return "Are you sure you want to close this page?";
}

window.onbeforeunload = confirmLeaving;

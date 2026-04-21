document.addEventListener('click', function (e) {
    var closeBtn = e.target.closest('.page-alert-close');
    if (!closeBtn) {
        return;
    }

    var toast = closeBtn.closest('.page-alert');
    if (toast) {
        toast.remove();
    }

    var stack = document.querySelector('.page-alerts');
    if (stack && stack.querySelectorAll('.page-alert').length === 0) {
        stack.remove();
    }
});

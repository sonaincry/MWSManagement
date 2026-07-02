window.ToastCommon = (function () {
    function show(type, message, options) {
        if (!message || message.trim() === '') {
            return;
        }

        options = options || {};

        var timeout = options.timeout || 5000;
        var position = options.position || 'top-right';

        type = normalizeType(type);

        var iconMap = {
            success: 'fa-check-circle',
            danger: 'fa-times-circle',
            error: 'fa-times-circle',
            info: 'fa-info-circle',
            warning: 'fa-exclamation-triangle'
        };

        var icon = iconMap[type] || 'fa-info-circle';

        var container = document.getElementById('toastContainer');

        if (!container) {
            container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-container-custom ' + position;
            document.body.appendChild(container);
        }

        var toast = document.createElement('div');
        toast.className = 'toast-alert-custom toast-alert-' + type + ' fade show';
        toast.setAttribute('role', 'alert');

        toast.innerHTML =
            '<i class="fas ' + icon + '"></i>' +
            '<span class="toast-message">' + escapeHtml(message) + '</span>' +
            '<button type="button" class="close" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>';

        container.appendChild(toast);

        var closeBtn = toast.querySelector('.close');

        if (closeBtn) {
            closeBtn.addEventListener('click', function () {
                closeToast(toast);
            });
        }

        setTimeout(function () {
            closeToast(toast);
        }, timeout);
    }

    function closeToast(toast) {
        if (!toast || !toast.parentNode) {
            return;
        }

        toast.classList.remove('show');

        setTimeout(function () {
            if (toast && toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 200);
    }

    function normalizeType(type) {
        if (!type) {
            return 'info';
        }

        type = type.toLowerCase();

        if (type === 'error') {
            return 'danger';
        }

        if (type === 'warn') {
            return 'warning';
        }

        return type;
    }

    function escapeHtml(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    return {
        show: show,
        success: function (message, options) {
            show('success', message, options);
        },
        error: function (message, options) {
            show('danger', message, options);
        },
        danger: function (message, options) {
            show('danger', message, options);
        },
        warning: function (message, options) {
            show('warning', message, options);
        },
        info: function (message, options) {
            show('info', message, options);
        }
    };
})();
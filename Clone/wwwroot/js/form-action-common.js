window.FormActionCommon = (function () {
    function buildUrl(baseUrl, action, params) {
        var query = new URLSearchParams();

        query.set('action', action);

        if (params) {
            Object.keys(params).forEach(function (key) {
                var value = params[key];

                if (value !== undefined && value !== null && value !== '') {
                    query.set(key, value);
                }
            });
        }

        return baseUrl + '?' + query.toString();
    }

    function initDeleteConfirm(options) {
        options = options || {};

        var formId = options.formId || 'formMain';
        var deleteButtonId = options.deleteButtonId || 'btnDelete';
        var confirmButtonId = options.confirmButtonId || 'commonConfirmDelete';
        var modalId = options.modalId || 'commonDeleteModal';

        var form = document.getElementById(formId);
        var btnDelete = document.getElementById(deleteButtonId);
        var btnConfirmDelete = document.getElementById(confirmButtonId);

        if (!form || !btnDelete || !btnConfirmDelete) {
            return;
        }

        btnDelete.addEventListener('click', function () {
            if (options.title) {
                var titleEl = document.getElementById('commonDeleteTitle');
                if (titleEl) titleEl.innerText = options.title;
            }

            if (options.message) {
                var messageEl = document.getElementById('commonDeleteMessage');
                if (messageEl) messageEl.innerText = options.message;
            }

            $('#' + modalId).modal('show');
        });

        btnConfirmDelete.addEventListener('click', function () {
            $('#' + modalId).modal('hide');

            var params = {};

            if (typeof options.getParams === 'function') {
                params = options.getParams();
            }

            var baseUrl = options.url || window.location.pathname;

            form.action = buildUrl(baseUrl, 'delete', params);

            form.submit();
        });
    }

    return {
        initDeleteConfirm: initDeleteConfirm,
        buildUrl: buildUrl
    };
})();
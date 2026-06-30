window.AppGrid = {
    openModal: function (url, title) {
        var frame = document.getElementById('appGridModalFrame');
        var loading = document.getElementById('appGridModalLoading');
        var modalTitle = document.getElementById('appGridModalTitle');

        if (!frame) {
            window.location.href = url;
            return;
        }

        var finalUrl = url;

        if (finalUrl.indexOf('?') >= 0) {
            finalUrl += '&popup=true';
        } else {
            finalUrl += '?popup=true';
        }

        modalTitle.innerText = title || 'Form';

        frame.style.visibility = 'hidden';
        loading.style.display = 'flex';
        frame.src = finalUrl;

        $('#appGridModal').modal('show');

        frame.onload = function () {
            loading.style.display = 'none';
            frame.style.visibility = 'visible';
        };
    },

    closeModal: function (reloadParent) {
        $('#appGridModal').modal('hide');

        var frame = document.getElementById('appGridModalFrame');
        var loading = document.getElementById('appGridModalLoading');

        if (frame) {
            frame.src = '';
            frame.style.visibility = 'hidden';
        }

        if (loading) {
            loading.style.display = 'flex';
        }

        if (reloadParent === true) {
            window.location.reload();
        }
    },

    create: function (options) {
        var gridId = options.gridId || 'Grid';
        var keyField = options.keyField || 'id';
        var formUrl = options.formUrl || '';

        var actions = options.actions || {};
        var selectedRow = null;
        var toolbar = buildToolbar(actions, options);

        var grid = new ej.grids.Grid({
            dataSource: options.dataSource || [],

            allowFiltering: options.allowFiltering ?? true,
            allowSorting: options.allowSorting ?? true,
            allowGrouping: options.allowGrouping ?? false,
            allowTextWrap: options.allowTextWrap ?? true,
            allowResizing: options.allowResizing ?? true,
            allowPaging: options.allowPaging ?? true,
            allowSelection: options.allowSelection ?? true,
            allowExcelExport: actions.excelExport ?? true,

            filterSettings: options.filterSettings || { type: 'CheckBox' },

            pageSettings: options.pageSettings || {
                currentPage: 1,
                pageSize: 20,
                pageSizes: ["10", "20", "50", "100", "All"]
            },

            selectionSettings: {
                type: 'Multiple',
                checkboxOnly: false,
                persistSelection: false
            },

            showColumnMenu: true,
            gridLines: 'Horizontal',
            height: '500px',
            width: '100%',

            editSettings: options.editSettings || {
                allowEditing: actions.inlineEdit ?? false,
                allowDeleting: actions.inlineDelete ?? false,
                mode: 'Normal'
            },

            showColumnMenu: options.showColumnMenu ?? true,
            gridLines: options.gridLines || 'Horizontal',

            columns: buildColumns(options.columns || [], actions, options),

            toolbar: toolbar,

            dataBound: function () {
                toggleEditDelete(false);

                if (this.dataSource && this.dataSource.length > 0) {
                    setTimeout(() => {
                        this.autoFitColumns();
                    }, 50);
                }

                if (typeof options.dataBound === 'function') {
                    options.dataBound(grid);
                }
            },

            rowSelected: function (args) {
                selectedRow = args.data;

                toggleEditDelete(true);

                if (typeof options.rowSelected === 'function') {
                    options.rowSelected(args, grid);
                }
            },

            rowDeselected: function (args) {
                selectedRow = null;

                toggleEditDelete(false);

                if (typeof options.rowDeselected === 'function') {
                    options.rowDeselected(args, grid);
                }
            },

            rowSelecting: function () {
                // Không clear selection ở đây
            },

            recordDoubleClick: function (args) {
                var row = args.rowData || args.data || selectedRow;

                if (!row) {
                    return;
                }

                selectedRow = row;

                if (typeof options.recordDoubleClick === 'function') {
                    options.recordDoubleClick(row, grid);
                    return;
                }

                if (options.openEditOnDoubleClick === true) {
                    if (typeof options.onEdit === 'function') {
                        options.onEdit(row, grid);
                    } else {
                        openEditPage(row);
                    }
                }
            },

            toolbarClick: function (args) {
                handleToolbarClick(args);
            },

            actionBegin: function (args) {
                handleInlineCrud(args);

                if (typeof options.actionBegin === 'function') {
                    options.actionBegin(args, grid);
                }
            },

            actionFailure: function (args) {
                console.error('Grid error:', args);

                if (typeof options.actionFailure === 'function') {
                    options.actionFailure(args, grid);
                }
            }
        });

        grid.appendTo('#' + gridId);

        // ✅ AutoFit khi resize window
        let resizeTimer;
        window.addEventListener('resize', function () {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(function () {
                if (grid && grid.dataSource && grid.dataSource.length > 0) {
                    grid.autoFitColumns();
                }
            }, 250);
        });

        grid.appGrid = {
            getSelectedRecord: getSelectedRecord,
            getSelectedRecords: getSelectedRecords,
            getSelectedKey: getSelectedKey,
            toggleEditDelete: toggleEditDelete
        };

        return grid;

        function buildToolbar(actions, options) {
            var toolbar = [];

            if (actions.excelExport !== false) {
                toolbar.push('ExcelExport');
            }

            if (actions.search !== false) {
                toolbar.push('Search');
            }

            if (
                actions.importExcel ||
                actions.refresh ||
                actions.add ||
                actions.edit ||
                actions.delete ||
                options.extraToolbar
            ) {
                toolbar.push({ type: 'Separator' });
            }

            if (actions.importExcel) {
                toolbar.push({
                    text: options.importText || 'Excel Import',
                    tooltipText: 'Import from Excel',
                    prefixIcon: 'e-upload',
                    id: 'ImportExcel'
                });
            }

            if (actions.refresh) {
                toolbar.push({
                    text: options.refreshText || 'Refresh',
                    tooltipText: 'Refresh',
                    prefixIcon: 'e-refresh',
                    id: 'RefreshCustom'
                });
            }

            if (actions.add) {
                toolbar.push({
                    text: options.addText || 'Add',
                    tooltipText: 'Add',
                    prefixIcon: 'e-add',
                    id: 'AddCustom'
                });
            }

            if (actions.edit) {
                toolbar.push({
                    text: options.editText || 'Edit',
                    tooltipText: 'Edit',
                    prefixIcon: 'e-edit',
                    id: 'EditCustom'
                });
            }

            if (actions.delete) {
                toolbar.push({
                    text: options.deleteText || 'Delete',
                    tooltipText: 'Delete',
                    prefixIcon: 'e-delete',
                    id: 'DeleteCustom'
                });
            }

            if (options.extraToolbar && Array.isArray(options.extraToolbar)) {
                options.extraToolbar.forEach(function (item) {
                    toolbar.push(item);
                });
            }

            return toolbar;
        }

        function buildColumns(columns, actions, options) {
            var result = columns.slice();

            if (options.enableCheckbox === true) {
                var hasCheckbox = result.some(function (col) {
                    return col.type === 'checkbox';
                });

                if (!hasCheckbox) {
                    result.unshift({
                        type: 'checkbox',
                        width: 50
                    });
                }
            }

            if (actions.commandColumn) {
                result.push({
                    headerText: options.commandHeaderText || 'Actions',
                    width: options.commandWidth || 120,
                    allowFiltering: false,
                    allowSorting: false,
                    commands: [
                        { type: 'Edit', buttonOption: { cssClass: 'e-flat e-primary', iconCss: 'e-edit e-icons' } },
                        { type: 'Delete', buttonOption: { cssClass: 'e-flat e-danger', iconCss: 'e-delete e-icons' } },
                        { type: 'Save', buttonOption: { cssClass: 'e-flat e-success', iconCss: 'e-update e-icons' } },
                        { type: 'Cancel', buttonOption: { cssClass: 'e-flat e-secondary', iconCss: 'e-cancel e-icons' } }
                    ]
                });
            }

            return result;
        }

        function handleToolbarClick(args) {
            var itemId = args.item.id || '';

            console.log('Toolbar clicked:', itemId);
            console.log('Selected records:', grid.getSelectedRecords());

            if (itemId.endsWith('_excelexport')) {
                grid.excelExport();
                return;
            }

            if (itemId.endsWith('ImportExcel')) {
                if (typeof options.onImportExcel === 'function') {
                    options.onImportExcel(grid);
                }
                return;
            }

            if (itemId.endsWith('RefreshCustom')) {
                if (typeof options.onRefresh === 'function') {
                    options.onRefresh(grid);
                } else {
                    window.location.reload();
                }
                return;
            }

            if (itemId.endsWith('AddCustom')) {
                if (typeof options.onAdd === 'function') {
                    options.onAdd(grid);
                } else {
                    window.location.href = buildFormUrl('create');
                }
                return;
            }

            if (itemId.endsWith('EditCustom')) {
                var row = getSelectedRecord();

                if (!row) {
                    alert('Please select one row to edit.');
                    return;
                }

                if (typeof options.onEdit === 'function') {
                    options.onEdit(row, grid);
                } else {
                    openEditPage(row);
                }

                return;
            }

            if (itemId.endsWith('DeleteCustom')) {
                var rows = getSelectedRecords();

                if (!rows || rows.length === 0) {
                    alert('Please select row(s) to delete.');
                    return;
                }

                if (typeof options.onDelete === 'function') {
                    options.onDelete(rows, grid);
                } else {
                    confirmDelete(rows);
                }

                return;
            }

            if (typeof options.toolbarClick === 'function') {
                options.toolbarClick(args, grid);
            }
        }

        function handleInlineCrud(args) {
            if (args.requestType === 'save' && options.updateUrl) {
                var token = getAntiForgeryToken();

                postJson(options.updateUrl, args.data, token)
                    .then(function (data) {
                        if (!data.success) {
                            alert('Update failed: ' + data.message);
                        } else if (typeof options.onUpdated === 'function') {
                            options.onUpdated(args.data, grid);
                        }
                    })
                    .catch(function (err) {
                        alert('Update error: ' + err.message);
                    });
            }

            if (args.requestType === 'delete' && options.deleteUrl) {
                if (!confirm(options.deleteMessage || 'Are you sure you want to delete this record?')) {
                    args.cancel = true;
                    return;
                }

                var row = args.data && args.data.length ? args.data[0] : args.data;
                var token = getAntiForgeryToken();

                postJson(options.deleteUrl, row, token)
                    .then(function (data) {
                        if (!data.success) {
                            alert('Delete failed: ' + data.message);
                        } else if (typeof options.onDeleted === 'function') {
                            options.onDeleted(row, grid);
                        }
                    })
                    .catch(function (err) {
                        alert('Delete error: ' + err.message);
                    });
            }
        }

        function getSelectedRecord() {
            var selected = grid.getSelectedRecords();

            if (selected && selected.length > 0) {
                return selected[0];
            }

            return selectedRow;
        }

        function getSelectedRecords() {
            var selected = grid.getSelectedRecords();

            if (selected && selected.length > 0) {
                return selected;
            }

            if (selectedRow) {
                return [selectedRow];
            }

            return [];
        }

        function getSelectedKey() {
            var row = getSelectedRecord();
            if (!row) return null;
            return row[keyField];
        }

        function toggleEditDelete(enable) {
            var items = [];

            if (actions.edit) {
                items.push('EditCustom');
            }

            if (actions.delete) {
                items.push('DeleteCustom');
            }

            if (items.length > 0 && grid.toolbarModule) {
                grid.toolbarModule.enableItems(items, enable);
            }
        }

        function buildFormUrl(action, row) {
            if (!formUrl) return '#';

            var params = new URLSearchParams();

            var actionParamName = options.actionParamName || 'action';
            var idParamName = options.idParamName || 'id';

            if (action === 'create') {
                params.append(actionParamName, action);
            } else {
                if (options.rowParams && typeof options.rowParams === 'object') {
                    Object.keys(options.rowParams).forEach(function (paramName) {
                        var fieldName = options.rowParams[paramName];
                        var value = row ? row[fieldName] : null;

                        if (value !== null && value !== undefined && value !== '') {
                            params.append(paramName, value);
                        }
                    });
                } else {
                    var key = row ? row[keyField] : getSelectedKey();

                    if (key === null || key === undefined || key === '') {
                        return '#';
                    }

                    params.append(idParamName, key);
                }

                if (options.includeActionParam !== false) {
                    params.append(actionParamName, action);
                }
            }

            if (options.formParams && typeof options.formParams === 'object') {
                Object.keys(options.formParams).forEach(function (name) {
                    var value = options.formParams[name];

                    if (value !== null && value !== undefined && value !== '') {
                        params.append(name, value);
                    }
                });
            }

            return formUrl + '?' + params.toString();
        }

        function openEditPage(row) {
            if (!row) {
                alert('Please select one row to edit.');
                return;
            }

            window.location.href = buildFormUrl('edit', row);
        }

        function confirmDelete(rows) {
            if (!Array.isArray(rows)) {
                rows = [rows];
            }

            if (rows.length === 0) {
                alert('Please select row(s) to delete.');
                return;
            }

            var message = rows.length > 1
                ? 'Are you sure you want to delete ' + rows.length + ' selected records?'
                : (options.deleteMessage || 'Are you sure you want to permanently delete this record?');

            var confirmDialog = ej.popups.DialogUtility.confirm({
                title: options.deleteTitle || 'Delete Item',
                content: message,
                okButton: {
                    text: 'OK',
                    click: function () {
                        confirmDialog.hide();

                        if (options.deleteUrl) {
                            deleteByAjax(rows);
                        } else {
                            window.location.href = buildFormUrl('delete', rows[0]);
                        }
                    }
                },
                cancelButton: {
                    text: 'Cancel',
                    click: function () {
                        confirmDialog.hide();
                    }
                },
                position: { X: 'center', Y: 'center' },
                closeOnEscape: true
            });
        }

        function deleteByAjax(rows) {
            var token = getAntiForgeryToken();

            if (!Array.isArray(rows)) {
                rows = [rows];
            }

            postJson(options.deleteUrl, rows, token)
                .then(function (data) {
                    if (!data.success) {
                        alert('Delete failed: ' + (data.message || 'Unknown error'));
                        return;
                    }

                    if (typeof options.onDeleted === 'function') {
                        options.onDeleted(rows, grid, data);
                        return;
                    }

                    if (typeof options.onRefresh === 'function') {
                        options.onRefresh(grid);
                    } else {
                        window.location.reload();
                    }
                })
                .catch(function (err) {
                    alert('Delete error: ' + err.message);
                });
        }

        function buildDeletePayload(rows) {
            if (!rows || rows.length === 0) {
                return [];
            }

            var isMultiDelete = options.multiDelete === true || actions.multiDelete === true;

            var targetRows = isMultiDelete ? rows : [rows[0]];

            if (options.deleteParams && typeof options.deleteParams === 'object') {
                var mappedRows = targetRows.map(function (row) {
                    var item = {};

                    Object.keys(options.deleteParams).forEach(function (paramName) {
                        var fieldName = options.deleteParams[paramName];
                        item[paramName] = row[fieldName];
                    });

                    return item;
                });

                return isMultiDelete ? mappedRows : mappedRows[0];
            }

            var defaultRows = targetRows.map(function (row) {
                return {
                    id: row[keyField]
                };
            });

            return isMultiDelete ? defaultRows : defaultRows[0];
        }

        function getAntiForgeryToken() {
            var tokenElement = document.querySelector(
                options.tokenSelector || 'input[name="__RequestVerificationToken"]'
            );

            return tokenElement ? tokenElement.value : '';
        }

        function postJson(url, data, token) {
            return fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(data)
            }).then(function (res) {
                return res.json();
            });
        }
    },

    load: function (grid, url) {
        if (!grid || !url) return;

        grid.showSpinner();

        fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(function (r) {
                return r.json();
            })
            .then(function (data) {
                grid.dataSource = data;
                grid.hideSpinner();

                // AutoFit sau khi load
                setTimeout(function () {
                    if (grid && grid.dataSource && grid.dataSource.length > 0) {
                        grid.autoFitColumns();
                    }
                }, 150);
            })
            .catch(function (err) {
                console.error('Load failed:', err);
                grid.hideSpinner();
            });
    },

    buildUrl: function (baseUrl, handler, params) {
        var query = new URLSearchParams();

        if (handler) {
            query.append('handler', handler);
        }

        if (params) {
            Object.keys(params).forEach(function (key) {
                var value = params[key];

                if (value !== null && value !== undefined && value !== '') {
                    query.append(key, value);
                }
            });
        }

        return baseUrl + '?' + query.toString();
    }
};


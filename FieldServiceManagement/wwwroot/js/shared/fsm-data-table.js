(function () {
    if (window.FSMDataTable) return;

    const instances = {};

    function initTable(root) {
        const name = root.dataset.fsmTable;
        if (!name || instances[name]) return;

        const table = root.querySelector('.fsm-table');
        if (!table) return;
        const tbody = table.querySelector('tbody');
        if (!tbody) return;

        const searchBtn = root.querySelector('[data-fsm-search-toggle]');
        const searchWrapper = root.querySelector('[data-fsm-search-wrapper]');
        const cancelSearch = root.querySelector('[data-fsm-search-cancel]');
        const searchInput = root.querySelector('[data-fsm-search-input]');
        const headers = table.querySelectorAll('th.fsm-sortable');
        const prevBtn = root.querySelector('[data-fsm-page-prev]');
        const nextBtn = root.querySelector('[data-fsm-page-next]');
        const pageLabel = root.querySelector('[data-fsm-page-label]');
        const perPageSel = root.querySelector('[data-fsm-per-page]');
        const totalLabel = root.querySelector('[data-fsm-total-count]');
        const searchColumns = (root.dataset.fsmSearchColumns || '0,1,2,3')
            .split(',').map(n => parseInt(n, 10));
        const colCount = table.querySelectorAll('thead th').length || 1;
        const emptyTitle = root.dataset.emptyTitle || 'No records found';
        const emptyMsg = root.dataset.emptyMessage || 'No records match your search or filters.';
        const emptyIcon = root.dataset.emptyIcon || 'bi bi-inboxes';

        function getAllDataRows() {
            return Array.from(tbody.querySelectorAll('tr')).filter(r => !r.dataset.static);
        }

        let emptyStateRow = document.createElement('tr');
        emptyStateRow.style.display = 'none';
        emptyStateRow.dataset.static = 'true';
        emptyStateRow.innerHTML = `
            <td colspan="${colCount}" class="border-bottom-0">
                <div class="fsm-empty-state-data-table">
                    <div class="fsm-empty-icon"><i class="${emptyIcon}"></i></div>
                    <h6 class="fw-semibold mb-1">${emptyTitle}</h6>
                    <p class="text-muted mb-3 w-50">${emptyMsg}</p>
                </div>
            </td>`;
        tbody.appendChild(emptyStateRow);

        let masterRows = getAllDataRows();
        let allRows = masterRows.slice();
        let currentCol = null;
        let ascending = true;
        let currentPage = 1;
        let perPage = parseInt(perPageSel?.value, 10) || 10;
        let advancedFilterFn = null;

        function matchesSearch(row) {
            const value = (searchInput?.value || '').toLowerCase().trim();
            if (!value) return true;
            return searchColumns.some(i =>
                (row.cells[i]?.textContent || '').toLowerCase().includes(value)
            );
        }

        function recompute() {
            allRows = masterRows.filter(row =>
                matchesSearch(row) && (typeof advancedFilterFn !== 'function' || advancedFilterFn(row))
            );
            currentPage = 1;
            render();
        }

        function render() {
            const total = allRows.length;
            const totalPages = Math.max(1, Math.ceil(total / perPage));
            currentPage = Math.min(Math.max(1, currentPage), totalPages);
            const start = (currentPage - 1) * perPage;

            getAllDataRows().forEach(row => { row.style.display = 'none'; });
            allRows.forEach((row, i) => {
                row.style.display = (i >= start && i < start + perPage) ? '' : 'none';
            });

            if (pageLabel) pageLabel.textContent = `${currentPage} / ${totalPages}`;
            if (prevBtn) prevBtn.disabled = currentPage <= 1;
            if (nextBtn) nextBtn.disabled = currentPage >= totalPages;
            emptyStateRow.style.display = total === 0 ? '' : 'none';
            if (totalLabel) totalLabel.textContent = total;
        }

        searchBtn?.addEventListener('click', () => {
            searchWrapper?.classList.add('active');
            searchBtn.classList.add('d-none');
            setTimeout(() => searchInput?.focus(), 200);
        });

        cancelSearch?.addEventListener('click', () => {
            searchWrapper?.classList.remove('active');
            searchBtn?.classList.remove('d-none');
            if (searchInput) searchInput.value = '';
            recompute();
        });

        searchInput?.addEventListener('keyup', recompute);

        headers.forEach(th => {
            th.style.cursor = 'pointer';
            th.addEventListener('click', () => {
                const col = th.dataset.sort;
                ascending = currentCol === col ? !ascending : true;
                currentCol = col;

                headers.forEach(h => {
                    const icon = h.querySelector('.fsm-sort-icon i');
                    if (icon) icon.className = 'bi bi-chevron-expand';
                });
                const activeIcon = th.querySelector('.fsm-sort-icon i');
                if (activeIcon) activeIcon.className = ascending ? 'bi bi-chevron-up' : 'bi bi-chevron-down';

                masterRows.sort((a, b) => {
                    const aText = a.dataset.sortValue ?? a.cells[0]?.innerText.trim() ?? '';
                    const bText = b.dataset.sortValue ?? b.cells[0]?.innerText.trim() ?? '';
                    return ascending ? aText.localeCompare(bText) : bText.localeCompare(aText);
                });

                masterRows.forEach(row => tbody.appendChild(row));
                if (emptyStateRow.parentElement === tbody) tbody.appendChild(emptyStateRow);

                recompute();
            });
        });

        prevBtn?.addEventListener('click', () => { if (currentPage > 1) { currentPage--; render(); } });
        nextBtn?.addEventListener('click', () => {
            const totalPages = Math.max(1, Math.ceil(allRows.length / perPage));
            if (currentPage < totalPages) { currentPage++; render(); }
        });
        perPageSel?.addEventListener('change', () => {
            perPage = parseInt(perPageSel.value, 10) || 10;
            currentPage = 1;
            render();
        });

        render();

        instances[name] = {
            applyAdvancedFilter(fn) { advancedFilterFn = fn; recompute(); },
            clearAdvancedFilter() { advancedFilterFn = null; recompute(); },
        };
    }

    document.querySelectorAll('[data-fsm-table]').forEach(initTable);

    window.FSMDataTable = { instances, init: initTable };
})();
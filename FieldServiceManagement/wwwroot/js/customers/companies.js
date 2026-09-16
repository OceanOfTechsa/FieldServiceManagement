// ── Shared enum-dropdown component ───────────────────────────────


// ── DOM refs ──────────────────────────────────────────────────────
const searchBtn = document.getElementById('searchBtn');
const searchWrapper = document.getElementById('searchWrapper');
const cancelSearch = document.getElementById('cancelSearch');
const searchInput = document.getElementById('searchInput');
const table = document.querySelector('.fsm-table');
const tbody = table.querySelector('tbody');

// ── Empty state row (search) ──────────────────────────────────────
const emptyStateRow = document.createElement('tr');
emptyStateRow.id = 'emptyStateRow';
emptyStateRow.style.display = 'none';
emptyStateRow.dataset.static = 'true';
emptyStateRow.innerHTML = `
            <td colspan="9">
                <div class="fsm-empty-state">
                    <div class="fsm-empty-icon"><i class="bi bi-buildings"></i></div>
                    <h6 class="fw-semibold mb-1">No companies found</h6>
                    <p class="text-muted small mb-3">No companies match your search.</p>
                </div>
            </td>`;
tbody.appendChild(emptyStateRow);

// ── Data rows only (excludes static rows) ─────────────────────────
const dataRows = Array.from(tbody.querySelectorAll('tr')).filter(r => !r.dataset.static);

// ── Search ────────────────────────────────────────────────────────
searchBtn.addEventListener('click', () => {
    searchWrapper.classList.add('active');
    searchBtn.classList.add('d-none');
    setTimeout(() => searchInput.focus(), 200);
});

cancelSearch.addEventListener('click', () => {
    searchWrapper.classList.remove('active');
    searchBtn.classList.remove('d-none');
    searchInput.value = '';
    dataRows.forEach(r => r.style.display = '');
    emptyStateRow.style.display = 'none';
    // re-run pagination to restore correct page state
    currentPage = 1;
    render();
});

searchInput.addEventListener('keyup', () => {
    const value = searchInput.value.toLowerCase().trim();
    let visible = 0;

    dataRows.forEach(row => {
        const match = [0, 1, 2].some(i => row.cells[i]?.textContent.toLowerCase().includes(value));
        row.style.display = match ? '' : 'none';
        if (match) visible++;
    });

    emptyStateRow.style.display = visible === 0 ? '' : 'none';
});

// ── Sort + Pagination ─────────────────────────────────────────────
const headers = table.querySelectorAll('th.fsm-sortable');
const prevBtn = document.querySelector('.fsm-page-btn:first-of-type');
const nextBtn = document.querySelector('.fsm-page-btn:last-of-type');
const pageLabel = document.querySelector('.fsm-page-current');
const perPageSel = document.querySelector('.fsm-per-page');

// allRows excludes static rows so pagination never touches emptyStateRow
let allRows = dataRows.slice();
let currentCol = null;
let ascending = true;
let currentPage = 1;
let perPage = 10;

function render() {
    const total = allRows.length;
    const totalPages = Math.max(1, Math.ceil(total / perPage));
    currentPage = Math.min(currentPage, totalPages);
    const start = (currentPage - 1) * perPage;

    allRows.forEach((row, i) => {
        row.style.display = (i >= start && i < start + perPage) ? '' : 'none';
    });

    if (pageLabel) pageLabel.textContent = `${currentPage} / ${totalPages}`;
    if (prevBtn) prevBtn.disabled = currentPage <= 1;
    if (nextBtn) nextBtn.disabled = currentPage >= totalPages;
}

// ── Sorting ───────────────────────────────────────────────────────
headers?.forEach(th => {
    th.style.cursor = 'pointer';
    th.addEventListener('click', () => {
        const col = th.dataset.sort;
        ascending = currentCol === col ? !ascending : true;
        currentCol = col;

        headers.forEach(h => h.querySelector('.fsm-sort-icon i').className = 'bi bi-chevron-expand');
        th.querySelector('.fsm-sort-icon i').className = ascending ? 'bi bi-chevron-up' : 'bi bi-chevron-down';

        allRows.sort((a, b) => {
            const aText = a.dataset.sortValue ?? a.cells[0]?.innerText.trim() ?? '';
            const bText = b.dataset.sortValue ?? b.cells[0]?.innerText.trim() ?? '';
            return ascending ? aText.localeCompare(bText) : bText.localeCompare(aText);
        });

        allRows.forEach(row => tbody.appendChild(row));
        currentPage = 1;
        render();
    });
});

// ── Pagination controls ───────────────────────────────────────────
prevBtn?.addEventListener('click', () => {
    if (currentPage > 1) { currentPage--; render(); }
});

nextBtn?.addEventListener('click', () => {
    if (currentPage < Math.ceil(allRows.length / perPage)) { currentPage++; render(); }
});

perPageSel?.addEventListener('change', () => {
    perPage = parseInt(perPageSel.value) || 10;
    currentPage = 1;
    render();
});

// ── Initial render ────────────────────────────────────────────────
render();
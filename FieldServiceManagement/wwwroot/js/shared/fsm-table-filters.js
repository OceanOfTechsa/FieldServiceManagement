(function () {
    if (window.FSMTableFilters) return;

    const OPERATORS = [
        { value: 'eq', label: 'Is equal to' },
        { value: 'neq', label: 'Is not equal to' },
        { value: 'contains', label: 'Contains' },
    ];

    function initFilters(panel) {
        const name = panel.dataset.fsmFilterPanel;
        if (!name || panel.dataset.fsmFiltersInit === 'true') return;
        panel.dataset.fsmFiltersInit = 'true';

        const toggle = document.querySelector(`[data-fsm-filter-toggle="${name}"]`);
        const body = document.querySelector(`[data-fsm-filter-body="${name}"]`);
        const badge = document.querySelector(`[data-fsm-filter-badge="${name}"]`);
        const clearBtn = document.querySelector(`[data-fsm-filter-clear="${name}"]`);
        const applyBtn = document.querySelector(`[data-fsm-filter-apply="${name}"]`);
        const fieldsEl = document.querySelector(`script[data-fsm-filter-fields="${name}"]`);
        if (!toggle || !body || !fieldsEl) return;

        let FIELDS = [];
        try { FIELDS = JSON.parse(fieldsEl.textContent); } catch { FIELDS = []; }

        if (panel.parentElement !== document.body) document.body.appendChild(panel);

        FIELDS.forEach(field => {
            const row = document.createElement('div');
            row.className = 'fsm-filter-row';
            row.dataset.key = field.key;
            row.dataset.type = field.type;

            const label = document.createElement('label');
            label.className = 'fsm-filter-row-label';
            label.textContent = field.label;

            const opSelect = document.createElement('select');
            opSelect.className = 'notif-filter-select fsm-filter-op';
            OPERATORS.forEach(op => {
                const opt = document.createElement('option');
                opt.value = op.value;
                opt.textContent = op.label;
                opSelect.appendChild(opt);
            });

            let valueControl;
            if (field.type === 'bool') {
                valueControl = document.createElement('select');
                valueControl.className = 'notif-filter-select fsm-filter-value';
                [['', 'Any'], ['true', 'Yes'], ['false', 'No']].forEach(([v, text]) => {
                    const opt = document.createElement('option');
                    opt.value = v;
                    opt.textContent = text;
                    valueControl.appendChild(opt);
                });
            } else if (field.type === 'date') {
                valueControl = document.createElement('input');
                valueControl.type = 'date';
                valueControl.className = 'notif-filter-date fsm-filter-value';
            } else if (field.type === 'number') {
                valueControl = document.createElement('input');
                valueControl.type = 'number';
                valueControl.className = 'form-control fsm-filter-value';
            } else {
                valueControl = document.createElement('input');
                valueControl.type = 'text';
                valueControl.className = 'form-control fsm-filter-value';
                valueControl.placeholder = 'Value…';
            }

            row.appendChild(label);
            row.appendChild(opSelect);
            row.appendChild(valueControl);
            body.appendChild(row);
        });

        function positionPanel() {
            const rect = toggle.getBoundingClientRect();
            const gap = 4;
            const vw = window.innerWidth;
            const vh = window.innerHeight;
            const panelWidth = panel.offsetWidth || 340;

            let left = rect.right - panelWidth;
            left = Math.max(8, Math.min(left, vw - panelWidth - 8));

            const spaceBelow = vh - rect.bottom - gap;
            const spaceAbove = rect.top - gap;
            const preferBelow = spaceBelow >= 200 || spaceBelow >= spaceAbove;

            const maxHeight = Math.max(160, (preferBelow ? spaceBelow : spaceAbove) - 8);
            panel.style.maxHeight = `${maxHeight}px`;

            const panelHeight = Math.min(panel.scrollHeight, maxHeight);

            panel.style.left = `${left}px`;
            panel.style.top = preferBelow
                ? `${rect.bottom + gap}px`
                : `${rect.top - gap - panelHeight}px`;
        }

        function openPanel() {
            panel.classList.add('show');
            toggle.setAttribute('aria-expanded', 'true');
            positionPanel();
        }
        function closePanel() {
            panel.classList.remove('show');
            toggle.setAttribute('aria-expanded', 'false');
        }

        toggle.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            panel.classList.contains('show') ? closePanel() : openPanel();
        });

        document.addEventListener('click', (e) => {
            if (!panel.classList.contains('show')) return;
            if (panel.contains(e.target) || toggle.contains(e.target)) return;
            closePanel();
        });

        window.addEventListener('resize', () => {
            if (panel.classList.contains('show')) positionPanel();
        });

        document.addEventListener('scroll', (e) => {
            if (!panel.classList.contains('show')) return;
            if (panel.contains(e.target)) return;
            closePanel();
        }, true);

        function readCriteria() {
            return Array.from(body.querySelectorAll('.fsm-filter-row'))
                .map(row => ({
                    key: row.dataset.key,
                    op: row.querySelector('.fsm-filter-op')?.value,
                    value: row.querySelector('.fsm-filter-value')?.value ?? '',
                }))
                .filter(c => c.value !== '');
        }

        function matchRow(row, criteria) {
            return criteria.every(c => {
                const raw = (row.dataset[c.key] ?? '').toString().toLowerCase();
                const val = c.value.toLowerCase();
                switch (c.op) {
                    case 'neq': return raw !== val;
                    case 'contains': return raw.includes(val);
                    case 'eq':
                    default: return raw === val;
                }
            });
        }

        applyBtn?.addEventListener('click', () => {
            const criteria = readCriteria();
            if (badge) {
                badge.classList.toggle('d-none', criteria.length === 0);
                // badge.textContent = criteria.length || '';
            }
            const tableApi = window.FSMDataTable?.instances?.[name];
            if (tableApi) {
                criteria.length
                    ? tableApi.applyAdvancedFilter(row => matchRow(row, criteria))
                    : tableApi.clearAdvancedFilter();
            }
            closePanel();
        });

        clearBtn?.addEventListener('click', () => {
            body.querySelectorAll('.fsm-filter-op').forEach(el => { el.selectedIndex = 0; });
            body.querySelectorAll('.fsm-filter-value').forEach(el => { el.value = ''; });
            badge?.classList.add('d-none');
            window.FSMDataTable?.instances?.[name]?.clearAdvancedFilter();
        });
    }

    document.querySelectorAll('[data-fsm-filter-panel]').forEach(initFilters);

    window.FSMTableFilters = { init: initFilters };
})();
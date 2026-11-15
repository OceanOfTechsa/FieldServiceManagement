// One upload card on an import page: choosing, checking and removing a CSV file
class FileUploader {
    constructor(sectionId, inputId, howtoId = null) {
        this.section = document.getElementById(sectionId);
        if (!this.section) return;

        this.input = document.getElementById(inputId);
        this.uploadLabel = this.section.querySelector('.fsm-upload-label');
        this.noUploadLabel = this.section.querySelector('.bg-white.w-100.rounded-2.d-none');
        this.attachmentAccordion = this.section.querySelector('.accordion');
        this.howto = howtoId ? document.getElementById(howtoId) : null;   // the "how do you want to import" card, on the main file only

        this.fileNameEl = this.section.querySelector('.file-name');
        this.fileInfoEl = this.section.querySelector('.text-muted.smaller');
        this.progressContainer = this.section.querySelector('[id^="uploadProgressContainer"]');
        this.progressBar = this.section.querySelector('[id^="uploadProgressBar"]');
        this.errorEl = this.section.querySelector('[id^="upload-error"]');
        this.errorMsg = this.section.querySelector('[id^="uploadErrorMsg"]');

        this.currentFile = null;
        this.init();
    }

    init() {
        if (this.input) this.input.addEventListener('change', e => this.handleFileSelect(e));
        const closeBtn = this.section.querySelector('[id^="close-error"]');
        if (closeBtn) closeBtn.addEventListener('click', () => this.hideError());
    }

    handleFileSelect(e) {
        const file = e.target.files[0];
        if (!file) return;

        this.hideError();

        if (!file.name.toLowerCase().endsWith('.csv')) {
            this.showError("Only .CSV files are allowed.");
            e.target.value = '';
            return;
        }

        const maxBytes = typeof IMPORT_LIMITS !== "undefined" ? IMPORT_LIMITS.maxFileBytes : 1024 * 1024;   // set by the page from the plan
        if (file.size > maxBytes) {
            this.showError(`File size must not be larger than ${maxBytes / (1024 * 1024)}MB.`);
            e.target.value = '';
            return;
        }

        this.currentFile = file;
        this.updateFilePreview(file);
        this.showUploadedUI();
    }

    updateFilePreview(file) {
        if (this.fileNameEl) this.fileNameEl.textContent = this.truncateFileName(file.name);
        if (this.fileInfoEl) this.fileInfoEl.textContent = `Size: ${this.formatFileSize(file.size)}`;
    }

    formatFileSize(bytes) {
        if (bytes < 1024) return bytes + " B";
        if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
        return (bytes / (1024 * 1024)).toFixed(2) + " MB";
    }

    truncateFileName(name, max = 22) {
        if (name.length <= max) return name;
        const ext = name.substring(name.lastIndexOf('.'));
        return name.substring(0, max - 3) + '...' + ext;
    }

    showUploadedUI() {
        this.uploadLabel?.classList.add("d-none");
        this.noUploadLabel?.classList.remove("d-none");
        this.attachmentAccordion?.classList.remove("d-none");
        if (this.howto) this.howto.classList.remove("d-none");

        if (this.progressContainer) {
            this.progressContainer.classList.remove("d-none");
            this.simulateUploadProgress();
        }
    }

    simulateUploadProgress() {
        let progress = 0;
        if (!this.progressBar) return;
        this.progressBar.style.width = '0%';

        const interval = setInterval(() => {
            progress += Math.random() * 25;
            if (progress > 100) progress = 100;
            this.progressBar.style.width = `${progress}%`;
            if (progress >= 100) {
                clearInterval(interval);
                setTimeout(() => this.progressContainer.classList.add("d-none"), 800);
                enableNextButton();   // defined by the page
            }
        }, 150);
    }

    showError(message) {
        if (this.errorMsg) this.errorMsg.textContent = message;
        this.errorEl?.classList.remove('d-none');
        this.uploadLabel?.classList.remove("rounded-2");
        this.uploadLabel?.classList.add("rounded-top");
    }

    hideError() {
        this.errorEl?.classList.add('d-none');
        this.uploadLabel?.classList.add("rounded-2");
        this.uploadLabel?.classList.remove("rounded-top");
    }

    removeFile() {
        if (this.input) this.input.value = '';
        this.currentFile = null;
        this.uploadLabel?.classList.remove("d-none");
        this.noUploadLabel?.classList.add("d-none");
        this.attachmentAccordion?.classList.add("d-none");
        if (this.howto) this.howto.classList.add("d-none");
        this.progressContainer?.classList.add("d-none");
        enableNextButton();   // defined by the page
    }
}
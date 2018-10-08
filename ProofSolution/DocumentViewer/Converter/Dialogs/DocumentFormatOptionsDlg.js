// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.
// All Rights Reserved.
// *************************************************************
var HTML5Demos;
(function (HTML5Demos) {
    var DocumentViewerDemo;
    (function (DocumentViewerDemo) {
        var Converter;
        (function (Converter) {
            var Dialogs;
            (function (Dialogs) {
                var DocumentFormatOptionsDlg = /** @class */ (function () {
                    function DocumentFormatOptionsDlg() {
                        var _this = this;
                        this.inner = null;
                        this.el = null;
                        this.onHide = function () {
                            _this.inner.hide();
                        };
                        this.pdfAdvancedOptionsBtn_Click = function (e) {
                            var options = _this._documentFormat.options;
                            options.documentType = parseInt($(_this.el.options.pdf.documentTypeSelect).val());
                            options.imageOverText = $(_this.el.options.pdf.imageOverTextCheckbox).is(":checked");
                            _this._advancedPdfOptionsDlg.show(_this._totalPages, options);
                        };
                        this.htmUseBackgroundColorCheckbox_Click = function (e) {
                            // Disable/Enable color picker
                            var useBackgroundColor = $(e.currentTarget).is(":checked");
                            $(_this.el.options.htm.backgroundColor).spectrum(useBackgroundColor ? "enable" : "disable");
                        };
                        this.applyBtn_Click = function (e) {
                            _this.inner.hide();
                            var format = _this._documentFormat;
                            var options;
                            switch (format.extension) {
                                case "pdf":
                                    options = format.options;
                                    options.documentType = parseInt($(_this.el.options.pdf.documentTypeSelect).val(), 10);
                                    options.imageOverText = $(_this.el.options.pdf.imageOverTextCheckbox).is(":checked");
                                    break;
                                case "doc":
                                    options = format.options;
                                    options.textMode = $(_this.el.options.doc.frameTextCheckbox).is(":checked") ? lt.Document.Writer.DocumentTextMode.framed : lt.Document.Writer.DocumentTextMode.nonFramed;
                                    break;
                                case "rtf":
                                    options = format.options;
                                    options.textMode = $(_this.el.options.rtf.frameTextCheckbox).is(":checked") ? lt.Document.Writer.DocumentTextMode.framed : lt.Document.Writer.DocumentTextMode.nonFramed;
                                    break;
                                case "docx":
                                    options = format.options;
                                    options.textMode = $(_this.el.options.docx.frameTextCheckbox).is(":checked") ? lt.Document.Writer.DocumentTextMode.framed : lt.Document.Writer.DocumentTextMode.nonFramed;
                                    break;
                                case "htm":
                                    options = format.options;
                                    options.fontEmbedMode = parseInt($(_this.el.options.htm.embedFontModeSelectElement).val(), 10);
                                    options.useBackgroundColor = $(_this.el.options.htm.useBackgroundColorCheckbox).is(":checked");
                                    options.backgroundColor = $(_this.el.options.htm.backgroundColor).spectrum("get").toHexString();
                                    break;
                                case "txt":
                                    options = format.options;
                                    options.documentType = parseInt($(_this.el.options.txt.documentTypeSelect).val(), 10);
                                    options.addPageNumber = $(_this.el.options.txt.addPageNumberCheckbox).is(":checked");
                                    options.addPageBreak = $(_this.el.options.txt.addPageBreakCheckbox).is(":checked");
                                    options.formatted = $(_this.el.options.txt.formattedCheckbox).is(":checked");
                                    break;
                            }
                        };
                        var root = $("#dlgDocFormatOpts");
                        this.el = {
                            title: "#dlgDocFormatOpts_Title",
                            options: {
                                allContainers: "#dlgDocFormatOpts .dlg-format-opts-container",
                                pdf: {
                                    container: "#dlgDocFormatOpts_PDF",
                                    // Options
                                    documentTypeSelect: "#dlgDocFormatOpts_PDF_DocumentSelect",
                                    advancedOptionsBtn: "#dlgDocFormatOpts_PDF_AdvancedOptions",
                                    imageOverTextCheckbox: "#dlgDocFormatOpts_PDF_ImageOverText"
                                },
                                doc: {
                                    container: "#dlgDocFormatOpts_DOC",
                                    // Options
                                    frameTextCheckbox: "#dlgDocFormatOpts_DOC_FrameText"
                                },
                                rtf: {
                                    container: "#dlgDocFormatOpts_RTF",
                                    // Options
                                    frameTextCheckbox: "#dlgDocFormatOpts_RTF_FrameText"
                                },
                                htm: {
                                    container: "#dlgDocFormatOpts_HTM",
                                    // Options
                                    embedFontModeSelectElement: "#dlgDocFormatOpts_HTM_EmbedFontMode",
                                    useBackgroundColorCheckbox: "#dlgDocFormatOpts_HTM_UseBackgroundColor",
                                    backgroundColor: "#dlgDocFormatOpts_HTM_BackgroundColor"
                                },
                                txt: {
                                    container: "#dlgDocFormatOpts_TXT",
                                    // Options
                                    documentTypeSelect: "#dlgDocFormatOpts_TXT_DocumentSelect",
                                    addPageNumberCheckbox: "#dlgDocFormatOpts_TXT_AddPageNumber",
                                    addPageBreakCheckbox: "#dlgDocFormatOpts_TXT_AddPageBreak",
                                    formattedCheckbox: "#dlgDocFormatOpts_TXT_Formatted"
                                },
                                emf: {
                                    container: "#dlgDocFormatOpts_EMF"
                                    // No options
                                },
                                docx: {
                                    container: "#dlgDocFormatOpts_DOCX",
                                    // Options
                                    frameTextCheckbox: "#dlgDocFormatOpts_DOCX_FrameText"
                                },
                                none: {
                                    container: "#dlgDocFormatOpts_NONE"
                                    // No options
                                }
                            },
                            applyBtn: "#dlgDocFormatOpts_Apply",
                            hide: "#dlgDocFormatOpts .dlg-close"
                        };
                        this.inner = new lt.Demos.Dialogs.InnerDialog(root);
                        $(this.el.hide).on("click", this.onHide);
                        $(this.el.options.pdf.advancedOptionsBtn).on("click", this.pdfAdvancedOptionsBtn_Click);
                        $(this.el.options.htm.useBackgroundColorCheckbox).on("click", this.htmUseBackgroundColorCheckbox_Click);
                        $(this.el.applyBtn).on("click", this.applyBtn_Click);
                        // Init advanced PDF options dialog
                        this._advancedPdfOptionsDlg = new Converter.Dialogs.AdvancedPdfOptionsDlg();
                        // Set up the colorpicker for HTML
                        $(this.el.options.htm.backgroundColor).spectrum({ showAlpha: true, preferredFormat: "hex", disabled: true });
                    }
                    DocumentFormatOptionsDlg.prototype.dispose = function () {
                        $(this.el.hide).off("click", this.onHide);
                        this.onHide = null;
                        $(this.el.options.pdf.advancedOptionsBtn).off("click", this.pdfAdvancedOptionsBtn_Click);
                        $(this.el.options.htm.useBackgroundColorCheckbox).off("click", this.htmUseBackgroundColorCheckbox_Click);
                        $(this.el.applyBtn).off("click", this.applyBtn_Click);
                        this.pdfAdvancedOptionsBtn_Click = null;
                        this.htmUseBackgroundColorCheckbox_Click = null;
                        this.applyBtn_Click = null;
                        this.inner.dispose();
                        this.inner = null;
                        this.el = null;
                    };
                    DocumentFormatOptionsDlg.prototype.show = function (totalPages, documentFormat) {
                        this._totalPages = totalPages;
                        this._documentFormat = documentFormat;
                        // Set the dialog name to format friendly name
                        $(this.el.title).text(documentFormat.friendlyName + " (" + documentFormat.extension.toUpperCase() + ")");
                        // Hide all controls divs
                        lt.Demos.Utils.Visibility.toggle($(this.el.options.allContainers), false);
                        var visibleContainer = "";
                        // Show the selected control div
                        switch (documentFormat.extension) {
                            case "pdf":
                                visibleContainer = this.el.options.pdf.container;
                                var pdfOptions = documentFormat.options;
                                $(this.el.options.pdf.documentTypeSelect).prop("selectedIndex", pdfOptions.documentType);
                                $(this.el.options.pdf.imageOverTextCheckbox).prop("checked", pdfOptions.imageOverText);
                                break;
                            case "doc":
                                visibleContainer = this.el.options.doc.container;
                                var docOptions = documentFormat.options;
                                $(this.el.options.doc.frameTextCheckbox).prop("checked", docOptions.textMode === lt.Document.Writer.DocumentTextMode.framed);
                                break;
                            case "rtf":
                                visibleContainer = this.el.options.rtf.container;
                                var rtfOptions = documentFormat.options;
                                $(this.el.options.rtf.frameTextCheckbox).prop("checked", rtfOptions.textMode === lt.Document.Writer.DocumentTextMode.framed);
                                break;
                            case "docx":
                                visibleContainer = this.el.options.docx.container;
                                var docxOptions = documentFormat.options;
                                $(this.el.options.docx.frameTextCheckbox).prop("checked", docxOptions.textMode === lt.Document.Writer.DocumentTextMode.framed);
                                break;
                            case "htm":
                                visibleContainer = this.el.options.htm.container;
                                var htmlOptions = documentFormat.options;
                                $(this.el.options.htm.embedFontModeSelectElement).prop("selectedIndex", htmlOptions.fontEmbedMode);
                                $(this.el.options.htm.useBackgroundColorCheckbox).prop("checked", htmlOptions.useBackgroundColor);
                                $(this.el.options.htm.backgroundColor).spectrum({ color: htmlOptions.backgroundColor });
                                break;
                            case "txt":
                                visibleContainer = this.el.options.txt.container;
                                var textOptions = documentFormat.options;
                                // use utf8 instead of ansi
                                if (textOptions.documentType == lt.Document.Writer.TextDocumentType.ansi)
                                    textOptions.documentType = lt.Document.Writer.TextDocumentType.utf8;
                                $(this.el.options.txt.documentTypeSelect).prop("selectedIndex", textOptions.documentType);
                                $(this.el.options.txt.addPageNumberCheckbox).prop("checked", textOptions.addPageNumber);
                                $(this.el.options.txt.addPageBreakCheckbox).prop("checked", textOptions.addPageNumber);
                                $(this.el.options.txt.formattedCheckbox).prop("checked", textOptions.formatted);
                                break;
                            case "emf":
                                visibleContainer = this.el.options.emf.container;
                                break;
                            default:
                                visibleContainer = this.el.options.none.container;
                                break;
                        }
                        lt.Demos.Utils.Visibility.toggle($(visibleContainer), true);
                        this.inner.show();
                    };
                    return DocumentFormatOptionsDlg;
                }());
                Dialogs.DocumentFormatOptionsDlg = DocumentFormatOptionsDlg;
            })(Dialogs = Converter.Dialogs || (Converter.Dialogs = {}));
        })(Converter = DocumentViewerDemo.Converter || (DocumentViewerDemo.Converter = {}));
    })(DocumentViewerDemo = HTML5Demos.DocumentViewerDemo || (HTML5Demos.DocumentViewerDemo = {}));
})(HTML5Demos || (HTML5Demos = {}));

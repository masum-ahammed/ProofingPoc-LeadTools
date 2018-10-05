// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.
// All Rights Reserved.
// *************************************************************
var HTML5Demos;
(function (HTML5Demos) {
    var Dialogs;
    (function (Dialogs) {
        //Annotations Load Options 
        var AnnotationsLoadOption;
        (function (AnnotationsLoadOption) {
            AnnotationsLoadOption[AnnotationsLoadOption["none"] = 0] = "none";
            AnnotationsLoadOption[AnnotationsLoadOption["embedded"] = 1] = "embedded";
            AnnotationsLoadOption[AnnotationsLoadOption["external"] = 2] = "external";
        })(AnnotationsLoadOption || (AnnotationsLoadOption = {}));
        var FileUrlLoadOption;
        (function (FileUrlLoadOption) {
            FileUrlLoadOption[FileUrlLoadOption["sample"] = 0] = "sample";
            FileUrlLoadOption[FileUrlLoadOption["url"] = 1] = "url";
        })(FileUrlLoadOption || (FileUrlLoadOption = {}));
        // Custom event args for the UploadDocumentDlg load event
        var UploadDocumentEventArgs = /** @class */ (function () {
            function UploadDocumentEventArgs() {
                this.loadEmbeddedAnnotations = false;
            }
            return UploadDocumentEventArgs;
        }());
        Dialogs.UploadDocumentEventArgs = UploadDocumentEventArgs;
        // Custom event args for the OpenDocumentFromUrlDlg load event
        var OpenDocumentFromUrlEventArgs = /** @class */ (function () {
            function OpenDocumentFromUrlEventArgs() {
                this.fileUrl = "";
                this.annotationsUrl = "";
                this.loadEmbeddedAnnotations = false;
            }
            return OpenDocumentFromUrlEventArgs;
        }());
        Dialogs.OpenDocumentFromUrlEventArgs = OpenDocumentFromUrlEventArgs;
        // Custom event args for the OpenDocumentFromUrlDlg load event
        var OpenFromDocumentStorageEventArgs = /** @class */ (function () {
            function OpenFromDocumentStorageEventArgs() {
                this.loadEmbeddedAnnotations = false;
            }
            return OpenFromDocumentStorageEventArgs;
        }());
        Dialogs.OpenFromDocumentStorageEventArgs = OpenFromDocumentStorageEventArgs;
        var UploadDocumentDlg = /** @class */ (function () {
            function UploadDocumentDlg() {
                var _this = this;
                this.inner = null;
                this.el = null;
                this.onHide = function () {
                    _this.inner.hide();
                };
                this.uploadBtn_Click = function (e) {
                    // Get the file object
                    var documentFile = $(_this.el.documentFileInput)[0].files[0];
                    if (!documentFile) {
                        alert("Please choose a document file first.");
                        return;
                    }
                    var args = new UploadDocumentEventArgs();
                    args.documentFile = documentFile;
                    var selectedAnnotationsLoadOption = $(_this.el.annotationsLoadOptionsRadioBtns).filter(':checked').val();
                    if (selectedAnnotationsLoadOption === AnnotationsLoadOption.none) {
                        args.loadEmbeddedAnnotations = false;
                        args.annotationFile = null;
                    }
                    else if (selectedAnnotationsLoadOption == AnnotationsLoadOption.embedded) {
                        args.loadEmbeddedAnnotations = true;
                        args.annotationFile = null;
                    }
                    else if (selectedAnnotationsLoadOption == AnnotationsLoadOption.external) {
                        args.loadEmbeddedAnnotations = false;
                        var annotationsFile = $(_this.el.annotationsFileInput)[0].files[0];
                        if (!annotationsFile) {
                            alert("Please choose an annotations file first.");
                            return;
                        }
                        args.annotationFile = annotationsFile;
                    }
                    _this.inner.hide();
                    if (_this.onUpload)
                        _this.onUpload(args);
                };
                this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked = function (e) {
                    var selectedAnnotationsLoadOption = $(e.currentTarget).val();
                    // If loading external annotations, enable annotations file input
                    $(_this.el.annotationsFileInput).prop("disabled", !(selectedAnnotationsLoadOption == AnnotationsLoadOption.external));
                };
                var root = $("#dlgUploadDoc");
                this.el = {
                    documentFileInput: "#dlgUploadDoc_DocumentFile",
                    annotationsLoadOptionsRadioBtns: "#dlgUploadDoc input[name=dlgUploadDoc_AnnotationsLoadOptions]",
                    annotationsFileInput: "#dlgUploadDoc_AnnotationsFile",
                    uploadBtn: "#dlgUploadDoc_Upload",
                    hide: "#dlgUploadDoc .dlg-close"
                };
                this.inner = new lt.Demos.Dialogs.InnerDialog(root);
                this.inner.onRootClick = this.onHide;
                $(this.el.hide).on("click", this.onHide);
                // Reset the dialog input elements, to avoid cached data
                $(this.el.documentFileInput).val("");
                $(this.el.annotationsFileInput).val("");
                var radioBtns = $(this.el.annotationsLoadOptionsRadioBtns);
                radioBtns.first().click();
                radioBtns.on("click", this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked);
                $(this.el.uploadBtn).on("click", this.uploadBtn_Click);
            }
            UploadDocumentDlg.prototype.dispose = function () {
                $(this.el.hide).off("click", this.onHide);
                this.onHide = null;
                $(this.el.annotationsLoadOptionsRadioBtns).off("click", this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked);
                this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked = null;
                $(this.el.uploadBtn).off("click", this.uploadBtn_Click);
                this.uploadBtn_Click = null;
                this.inner.onRootClick = null;
                this.inner.dispose();
                this.inner = null;
                this.el = null;
            };
            UploadDocumentDlg.prototype.show = function () {
                this.inner.show();
            };
            return UploadDocumentDlg;
        }());
        Dialogs.UploadDocumentDlg = UploadDocumentDlg;
        var OpenDocumentFromUrlDlg = /** @class */ (function () {
            function OpenDocumentFromUrlDlg(sampleDocuments) {
                var _this = this;
                this.inner = null;
                this.el = null;
                this._sampleDocuments = null;
                this._selectedLoadAnnotations = AnnotationsLoadOption.none;
                this._selectedLoadFileUrl = FileUrlLoadOption.sample;
                this.onHide = function () {
                    _this.inner.hide();
                };
                this.urlLoadOptionsRadioBtnsGroup_BtnClicked = function (e) {
                    var fileUrl = parseInt($(e.currentTarget).val(), 10);
                    _this._selectedLoadFileUrl = fileUrl;
                    _this.updateUI();
                };
                this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked = function (e) {
                    var selectedAnnotationsLoadOption = parseInt($(e.currentTarget).val());
                    _this._selectedLoadAnnotations = selectedAnnotationsLoadOption;
                    _this.updateUI();
                };
                this.loadBtn_Click = function (e) {
                    var args = new OpenDocumentFromUrlEventArgs();
                    var urlLoadOption = _this._selectedLoadFileUrl;
                    var annLoadOption = _this._selectedLoadAnnotations;
                    if (urlLoadOption === FileUrlLoadOption.sample) {
                        var selectedSampleIndex = $(_this.el.fileSampleSelectElement).find(":selected").index();
                        var sample = _this._sampleDocuments[selectedSampleIndex];
                        // If using a sample document, no annotations
                        annLoadOption = AnnotationsLoadOption.none;
                        args.fileUrl = OpenDocumentFromUrlDlg.getSampleUrl(sample);
                    }
                    else {
                        var documentUrl = $(_this.el.fileUrlTextInput).val();
                        if (documentUrl)
                            documentUrl = documentUrl.trim();
                        if (!documentUrl) {
                            alert("Must enter a document URL first");
                            return;
                        }
                        args.fileUrl = documentUrl;
                    }
                    if (annLoadOption == AnnotationsLoadOption.none) {
                        args.loadEmbeddedAnnotations = false;
                        args.annotationsUrl = null;
                    }
                    else if (annLoadOption == AnnotationsLoadOption.embedded) {
                        args.loadEmbeddedAnnotations = true;
                        args.annotationsUrl = null;
                    }
                    else if (annLoadOption == AnnotationsLoadOption.external) {
                        args.loadEmbeddedAnnotations = false;
                        var annotationsUrl = $(_this.el.annotationsUrlTextInput).val();
                        if (annotationsUrl)
                            annotationsUrl = annotationsUrl.trim();
                        if (!annotationsUrl) {
                            alert("Must enter an external annotations URL first");
                            return;
                        }
                        args.annotationsUrl = $(_this.el.annotationsUrlTextInput).val();
                    }
                    _this.inner.hide();
                    if (_this.onLoad)
                        _this.onLoad(args);
                };
                this._sampleDocuments = sampleDocuments.slice();
                var root = $("#dlgOpenUrl");
                this.el = {
                    urlLoadOptionsRadioBtns: "#dlgOpenUrl input[name=dlgOpenUrl_UrlOptions]",
                    fileSampleSelectElement: "#dlgOpenUrl_FileSelect",
                    fileUrlTextInput: "#dlgOpenUrl_FileUrl",
                    annotationsLoadOptionsRadioBtns: "#dlgOpenUrl input[name=dlgOpenUrl_AnnotationsLoadOptions]",
                    annotationsUrlTextInput: "#dlgOpenUrl_AnnotationsUrl",
                    loadBtn: "#dlgOpenUrl_Load",
                    hide: "#dlgOpenUrl .dlg-close"
                };
                this.inner = new lt.Demos.Dialogs.InnerDialog(root);
                this.inner.onRootClick = this.onHide;
                $(this.el.hide).on("click", this.onHide);
                // Reset the dialog input elements, to avoid cached data
                var $fileSelectElement = $(this.el.fileSampleSelectElement);
                $fileSelectElement.empty();
                $fileSelectElement.prop("selectedIndex", 0);
                // Add the options to the <select>
                this._sampleDocuments.forEach(function (documentUrl, index) {
                    var $option = $(document.createElement("option")).text(documentUrl);
                    if (index === 0)
                        $option.attr("selected", "");
                    $fileSelectElement.append($option);
                });
                $(this.el.urlLoadOptionsRadioBtns).on("click", this.urlLoadOptionsRadioBtnsGroup_BtnClicked);
                $(this.el.annotationsLoadOptionsRadioBtns).on("click", this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked);
                $(this.el.loadBtn).on("click", this.loadBtn_Click);
                this.updateUI();
            }
            OpenDocumentFromUrlDlg.prototype.dispose = function () {
                $(this.el.hide).off("click", this.onHide);
                this.onHide = null;
                $(this.el.urlLoadOptionsRadioBtns).off("click", this.urlLoadOptionsRadioBtnsGroup_BtnClicked);
                $(this.el.annotationsLoadOptionsRadioBtns).off("click", this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked);
                $(this.el.loadBtn).off("click", this.loadBtn_Click);
                this.urlLoadOptionsRadioBtnsGroup_BtnClicked = null;
                this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked = null;
                this.loadBtn_Click = null;
                this.inner.onRootClick = null;
                this.inner.dispose();
                this.inner = null;
                this.el = null;
            };
            OpenDocumentFromUrlDlg.prototype.show = function () {
                this.inner.show();
            };
            OpenDocumentFromUrlDlg.prototype.updateUI = function () {
                $(this.el.fileUrlTextInput).prop("disabled", this._selectedLoadFileUrl !== FileUrlLoadOption.url);
                $(this.el.annotationsLoadOptionsRadioBtns).prop("disabled", this._selectedLoadFileUrl !== FileUrlLoadOption.url);
                $(this.el.annotationsUrlTextInput).prop("disabled", this._selectedLoadFileUrl !== FileUrlLoadOption.url || this._selectedLoadAnnotations !== AnnotationsLoadOption.external);
                $(this.el.fileSampleSelectElement).prop("disabled", this._selectedLoadFileUrl !== FileUrlLoadOption.sample);
            };
            OpenDocumentFromUrlDlg.getSampleUrl = function (name) {
                if (name.indexOf("http") === 0) {
                    return name;
                }
                else {
                    var newDocumentUrl = 'Samples/' + name;
                    var serviceBase = lt.Document.DocumentFactory.serviceUri;
                    var serviceApiPath = lt.Document.DocumentFactory.serviceApiPath;
                    if (serviceApiPath) {
                        var serviceApiPathIndex = serviceBase.lastIndexOf(serviceApiPath);
                        if (serviceApiPathIndex !== -1) {
                            serviceBase = serviceBase.substring(0, serviceApiPathIndex);
                        }
                    }
                    if (serviceBase.charAt(serviceBase.length - 1) !== "/")
                        serviceBase += "/";
                    return serviceBase + newDocumentUrl;
                }
            };
            return OpenDocumentFromUrlDlg;
        }());
        Dialogs.OpenDocumentFromUrlDlg = OpenDocumentFromUrlDlg;
        var OpenFromDocumentStorageDlg = /** @class */ (function () {
            function OpenFromDocumentStorageDlg() {
                var _this = this;
                this.inner = null;
                this.el = null;
                this.onHide = function () {
                    _this.inner.hide();
                };
                this._openFromDocumentStorageEventArgs = null;
                this.openDocumentFromOneDriveBtn_Clicked = function (e) {
                    _this._loadingAnnotationsFile = false;
                    _this._oneDriveHelper.open();
                };
                this.openDocumentFromSharePointBtn_Clicked = function (e) {
                    _this._loadingAnnotationsFile = false;
                    _this._sharePointHelper.open();
                };
                this.openDocumentFromGoogleDriveBtn_Clicked = function (e) {
                    _this._loadingAnnotationsFile = false;
                    _this._googleDriveHelper.open();
                };
                this.openAnnotationsFromOneDriveBtn_Clicked = function (e) {
                    _this._loadingAnnotationsFile = true;
                    _this._oneDriveHelper.open();
                };
                this.openAnnotationsFromSharePointBtn_Clicked = function (e) {
                    _this._loadingAnnotationsFile = true;
                    _this._sharePointHelper.open();
                };
                this.openAnnotationsFromGoogleDriveBtn_Clicked = function (e) {
                    _this._loadingAnnotationsFile = true;
                    _this._googleDriveHelper.open();
                };
                // Open done handler 
                this.openDone = function (file) {
                    if (file) {
                        if (!_this._loadingAnnotationsFile) {
                            // Open document file
                            $(_this.el.document.name).text(file.name);
                            _this._openFromDocumentStorageEventArgs.documentFile = file;
                        }
                        else {
                            // Open annotations file
                            $(_this.el.annotations.name).text(file.name);
                            _this._openFromDocumentStorageEventArgs.annotationsFile = file;
                        }
                    }
                };
                this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked = function (e) {
                    var selectedAnnotationsLoadOption = $(e.currentTarget).val();
                    var loadExternal = selectedAnnotationsLoadOption == AnnotationsLoadOption.external;
                    // If loading external annotations, enable annotations url text input
                    $(_this.el.annotations.sharePointBtn).prop("disabled", !loadExternal);
                    // They must be disabled on Microsoft Edge
                    if (lt.LTHelper.browser !== lt.LTBrowser.edge) {
                        $(_this.el.annotations.oneDriveBtn).prop("disabled", !loadExternal || !_this._oneDriveHelper || !_this._oneDriveHelper.isRegisteredForLoadSave);
                        $(_this.el.annotations.googleDriveBtn).prop("disabled", !loadExternal || !_this._googleDriveHelper || !_this._googleDriveHelper.isRegisteredForLoad);
                    }
                };
                this.loadBtn_Click = function (e) {
                    var args = _this._openFromDocumentStorageEventArgs;
                    if (!args.documentFile) {
                        alert("Please choose a document to load.");
                        return;
                    }
                    var selectedAnnotationsLoadOption = parseInt($(_this.el.annotations.loadOptionsRadioBtns).filter(':checked').val(), 10);
                    switch (selectedAnnotationsLoadOption) {
                        case AnnotationsLoadOption.none:
                            args.loadEmbeddedAnnotations = false;
                            args.annotationsFile = null;
                            break;
                        case AnnotationsLoadOption.embedded:
                            args.loadEmbeddedAnnotations = true;
                            args.annotationsFile = null;
                            break;
                        case AnnotationsLoadOption.external:
                            args.loadEmbeddedAnnotations = false;
                            if (!args.annotationsFile) {
                                alert("Please choose an annotations file to load.");
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    _this.inner.hide();
                    if (_this.onLoad)
                        _this.onLoad(args);
                };
                var root = $("#dlgOpenCloud");
                this.el = {
                    infoText: "#dlgOpenCloud_InfoText",
                    document: {
                        oneDriveBtn: "#dlgOpenCloud_Document_OneDrive",
                        sharePointBtn: "#dlgOpenCloud_Document_SharePoint",
                        googleDriveBtn: "#dlgOpenCloud_Document_GoogleDrive",
                        name: "#dlgOpenCloud_Document_File"
                    },
                    annotations: {
                        loadOptionsRadioBtns: "#dlgOpenCloud input[name=dlgOpenCloud_Annotations_LoadOptions]",
                        oneDriveBtn: "#dlgOpenCloud_Annotations_OneDrive",
                        sharePointBtn: "#dlgOpenCloud_Annotations_SharePoint",
                        googleDriveBtn: "#dlgOpenCloud_Annotations_GoogleDrive",
                        name: "#dlgOpenCloud_Annotations_File"
                    },
                    loadBtn: "#dlgOpenCloud_Load",
                    hide: "#dlgOpenCloud .dlg-close",
                };
                this.inner = new lt.Demos.Dialogs.InnerDialog(root);
                $(this.el.hide).on("click", this.onHide);
                // Reset the dialog input elements, to avoid cached data
                $(this.el.annotations.loadOptionsRadioBtns).first().click();
                $(this.el.annotations.loadOptionsRadioBtns).on("click", this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked);
                $(this.el.loadBtn).on("click", this.loadBtn_Click);
                // Create the arguments
                this._openFromDocumentStorageEventArgs = new OpenFromDocumentStorageEventArgs();
                this._oneDriveHelper = new HTML5Demos.DriveHelper.LTOneDrive.OneDriveHelper();
                this._googleDriveHelper = new HTML5Demos.DriveHelper.LTGoogleDrive.GoogleDriveHelper();
            }
            OpenFromDocumentStorageDlg.prototype.dispose = function () {
                $(this.el.hide).off("click", this.onHide);
                this.onHide = null;
                $(this.el.annotations.loadOptionsRadioBtns).off("click", this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked);
                this.annotationsLoadOptionsRadioBtnsGroup_BtnClicked = null;
                $(this.el.loadBtn).off("click", this.loadBtn_Click);
                this.loadBtn_Click = null;
                this.inner.onRootClick = null;
                this.inner.dispose();
                this.inner = null;
                this.el = null;
            };
            OpenFromDocumentStorageDlg.prototype.show = function () {
                this.inner.show();
            };
            Object.defineProperty(OpenFromDocumentStorageDlg.prototype, "googleDriveHelper", {
                get: function () {
                    return this._googleDriveHelper;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(OpenFromDocumentStorageDlg.prototype, "sharePointHelper", {
                set: function (value) {
                    this._sharePointHelper = value;
                },
                enumerable: true,
                configurable: true
            });
            // SharePoint should be set and Google Drive should be registered before calling init
            OpenFromDocumentStorageDlg.prototype.init = function () {
                // OneDrive
                this._oneDriveHelper.openDone = this.openDone;
                // SharePoint
                this._sharePointHelper.openDone = this.openDone;
                // GoogleDrive
                // if IE9, Google Drive (which is not supported) will throw an error. So don't create it.
                if (!(lt.LTHelper.browser === lt.LTBrowser.internetExplorer && lt.LTHelper.version <= 9)) {
                    this._googleDriveHelper.openDone = this.openDone;
                }
                // Right now Google Drive and Microsoft OneDrive will get blocked on Microsoft Edge, so we disable them.
                if (lt.LTHelper.browser == lt.LTBrowser.edge) {
                    $(this.el.infoText).text("Opening files from Microsoft OneDrive or Google Drive is not currently supported by Microsoft Edge.");
                    $(this.el.document.oneDriveBtn).prop("disabled", true);
                    $(this.el.document.googleDriveBtn).prop("disabled", true);
                    $(this.el.annotations.oneDriveBtn).prop("disabled", true);
                    $(this.el.annotations.googleDriveBtn).prop("disabled", true);
                }
                else {
                    var googleIsRegistered = this._googleDriveHelper && this._googleDriveHelper.isRegisteredForLoad;
                    var oneDriveIsRegistered = this._oneDriveHelper && this._oneDriveHelper.isRegisteredForLoadSave;
                    if (googleIsRegistered) {
                        $(this.el.document.googleDriveBtn).on("click", this.openDocumentFromGoogleDriveBtn_Clicked);
                        $(this.el.annotations.googleDriveBtn).on("click", this.openAnnotationsFromGoogleDriveBtn_Clicked);
                    }
                    else {
                        $(this.el.document.googleDriveBtn).prop("disabled", true);
                        $(this.el.annotations.googleDriveBtn).prop("disabled", true);
                    }
                    if (oneDriveIsRegistered) {
                        $(this.el.document.oneDriveBtn).on("click", this.openDocumentFromOneDriveBtn_Clicked);
                        $(this.el.annotations.oneDriveBtn).on("click", this.openAnnotationsFromOneDriveBtn_Clicked);
                    }
                    else {
                        $(this.el.document.oneDriveBtn).prop("disabled", true);
                        $(this.el.annotations.oneDriveBtn).prop("disabled", true);
                    }
                    var vendorsDisabled = "";
                    if (!googleIsRegistered) {
                        vendorsDisabled = "Google Drive";
                    }
                    if (!oneDriveIsRegistered) {
                        if (!googleIsRegistered)
                            vendorsDisabled += " and OneDrive are";
                        else
                            vendorsDisabled += "OneDrive is";
                    }
                    else if (!googleIsRegistered) {
                        vendorsDisabled += " is";
                    }
                    if (vendorsDisabled) {
                        $(this.el.infoText).text(vendorsDisabled + " not registered for loading.");
                    }
                    lt.Demos.Utils.Visibility.toggle($(this.el.infoText), !!vendorsDisabled);
                }
                $(this.el.document.sharePointBtn).on("click", this.openDocumentFromSharePointBtn_Clicked);
                $(this.el.annotations.sharePointBtn).on("click", this.openAnnotationsFromSharePointBtn_Clicked);
            };
            return OpenFromDocumentStorageDlg;
        }());
        Dialogs.OpenFromDocumentStorageDlg = OpenFromDocumentStorageDlg;
    })(Dialogs = HTML5Demos.Dialogs || (HTML5Demos.Dialogs = {}));
})(HTML5Demos || (HTML5Demos = {}));

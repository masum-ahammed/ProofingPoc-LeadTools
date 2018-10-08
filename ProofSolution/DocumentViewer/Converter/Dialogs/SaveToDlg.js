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
                var SaveToDlg = /** @class */ (function () {
                    function SaveToDlg() {
                        var _this = this;
                        this.inner = null;
                        this.el = null;
                        this.localSaveBtn_Click = function (e) {
                            var url = _this._convertItem.url;
                            var win = null;
                            if (lt.LTHelper.browser === lt.LTBrowser.internetExplorer) {
                                win = window.open("");
                                win.navigate(url);
                            }
                            else {
                                win = window.open(url);
                            }
                            if (win == null || typeof (win) == "undefined")
                                alert("A Popup Blocker may have blocked opening a new window.\nIf this is the case, disable the Popup Blocker for this page and try again.");
                        };
                        this.closeBtn_Click = function () {
                            if (!_this._remaining || _this._remaining.length == 0)
                                _this.inner.hide();
                            else
                                _this.showConvertItems(_this._remaining);
                        };
                        this.saveToOneDriveBtn_Click = function (e) {
                            lt.Demos.Utils.Visibility.toggle($(_this.el.export.fileSavingImage), true);
                            lt.Demos.Utils.Visibility.toggle($(_this.el.export.googleSaveContainer), false);
                            var fileName = $(_this.el.export.fileNameTextInput).val() || _this._fileOriginalName;
                            fileName = fileName.trim();
                            _this._oneDriveHelper.save(_this._convertItem.url, fileName);
                        };
                        this.saveToSharePointBtn_Click = function (e) {
                            lt.Demos.Utils.Visibility.toggle($(_this.el.export.fileSavingImage), true);
                            lt.Demos.Utils.Visibility.toggle($(_this.el.export.googleSaveContainer), false);
                            var fileName = $(_this.el.export.fileNameTextInput).val() || _this._fileOriginalName;
                            fileName = fileName.trim();
                            _this._sharePointHelper.save(_this._convertItem.url, fileName);
                        };
                        this.saveToGoogleDriveBtn_Click = function (e) {
                            var fileName = $(_this.el.export.fileNameTextInput).val() || _this._fileOriginalName;
                            fileName = fileName.trim();
                            _this._googleDriveHelper.showSaveButton(_this._convertItem.url, fileName);
                            lt.Demos.Utils.Visibility.toggle($(_this.el.export.googleSaveContainer), true);
                        };
                        var root = $("#dlgExportSave");
                        this.el = {
                            message: "#dlgExportSave_Message",
                            cache: {
                                container: "#dlgExportSave_Cache",
                                header: "#dlgExportSave_Cache_Header",
                                input: "#dlgExportSave_Cache_Input"
                            },
                            export: {
                                container: "#dlgExportSave_Export",
                                header: "#dlgExportSave_Export_Header",
                                localSaveBtn: "#dlgExportSave_LocalSave",
                                btnGroup: "#dlgExportSave_Group",
                                saveToOneDriveBtn: "#dlgExportSave_SaveToOneDrive",
                                saveToSharePointBtn: "#dlgExportSave_SaveToSharePoint",
                                saveToGoogleDriveBtn: "#dlgExportSave_SaveToGoogleDrive",
                                googleSaveContainer: "#dlgExportSave_GoogleSaveContainer",
                                fileNameTextInput: "#dlgExportSave_FileName",
                                fileSavingImage: "#dlgExportSave_FileSaving",
                            },
                            closeNextBtn: "#dlgExportSave_CloseNext",
                        };
                        this.inner = new lt.Demos.Dialogs.InnerDialog(root);
                    }
                    SaveToDlg.prototype.dispose = function () {
                        this.inner.dispose();
                        this.inner = null;
                        this.el = null;
                    };
                    SaveToDlg.prototype.init = function (sharepointHelper) {
                        var _this = this;
                        this._sharePointHelper = sharepointHelper;
                        // OneDrive
                        this._oneDriveHelper = new HTML5Demos.DriveHelper.LTOneDrive.OneDriveHelper();
                        this._oneDriveHelper.saveDone = function (error) { return _this.saveDone(error); };
                        // SharePoint
                        this._sharePointHelper.saveDone = function (error) { return _this.saveDone(error); };
                        // GoogleDrive
                        // if IE, Google Drive save is not supported.
                        this._googleDriveHelper = new HTML5Demos.DriveHelper.LTGoogleDrive.GoogleDriveHelper();
                        lt.Demos.Utils.Visibility.toggle($(this.el.export.googleSaveContainer), false);
                        lt.Demos.Utils.Visibility.toggle($(this.el.export.fileSavingImage), false);
                        if (DocumentViewerDemo.DocumentViewerDemoApp.isMobileVersion) {
                            // Create more space for titles in mobile
                            var group = $(this.el.export.btnGroup);
                            group.addClass("btn-group-vertical");
                            var childrenOfChildren = group.children().children();
                            group.empty().append(childrenOfChildren);
                        }
                        $(this.el.export.localSaveBtn).on("click", this.localSaveBtn_Click);
                        $(this.el.export.saveToSharePointBtn).on("click", this.saveToSharePointBtn_Click);
                        var googleDriveCanSave = this._googleDriveHelper && this._googleDriveHelper.canSave;
                        var oneDriveIsRegistered = this._oneDriveHelper && this._oneDriveHelper.isRegisteredForLoadSave;
                        if (googleDriveCanSave)
                            $(this.el.export.saveToGoogleDriveBtn).on("click", this.saveToGoogleDriveBtn_Click);
                        else
                            $(this.el.export.saveToGoogleDriveBtn).prop("disabled", true);
                        if (oneDriveIsRegistered)
                            $(this.el.export.saveToOneDriveBtn).on("click", this.saveToOneDriveBtn_Click);
                        else
                            $(this.el.export.saveToOneDriveBtn).prop("disabled", true);
                        var vendorsDisabled = "";
                        if (!oneDriveIsRegistered) {
                            vendorsDisabled += "OneDrive must be registered before saving is available. ";
                        }
                        if (!googleDriveCanSave) {
                            // Cannot save in IE
                            vendorsDisabled += "Save to Google Drive is not available for this browser.";
                        }
                        if (vendorsDisabled) {
                            $(this.el.message).text(vendorsDisabled);
                        }
                        lt.Demos.Utils.Visibility.toggle($(this.el.message), !!vendorsDisabled);
                        $(this.el.closeNextBtn).on("click", this.closeBtn_Click);
                    };
                    SaveToDlg.prototype.show = function (cacheId, convertItems) {
                        var hasConvertItems = !!(convertItems && convertItems.length);
                        if (!cacheId && !hasConvertItems)
                            return;
                        // Show the headers only if both are needed
                        var hasBoth = !!cacheId && hasConvertItems;
                        lt.Demos.Utils.Visibility.toggle($(this.el.cache.header), hasBoth);
                        lt.Demos.Utils.Visibility.toggle($(this.el.export.header), hasBoth);
                        lt.Demos.Utils.Visibility.toggle($(this.el.cache.container), !!cacheId);
                        if (cacheId) {
                            $(this.el.cache.input).val(cacheId);
                        }
                        lt.Demos.Utils.Visibility.toggle($(this.el.export.container), hasConvertItems);
                        if (hasConvertItems) {
                            this.showConvertItems(convertItems);
                        }
                        this.inner.show();
                    };
                    SaveToDlg.prototype.showConvertItems = function (convertItems) {
                        var convertItem = convertItems[0];
                        this._remaining = convertItems.slice(1, convertItems.length);
                        var lengthRemaining = this._remaining.length;
                        if (lengthRemaining)
                            $(this.el.closeNextBtn).text("Next (" + lengthRemaining + ")");
                        else
                            $(this.el.closeNextBtn).text("Close");
                        var name = convertItem.name;
                        $(this.el.export.fileNameTextInput).val(name);
                        // Fix the URL for this convertItem by adding to the relative end
                        convertItem.url = lt.Document.DocumentFactory.serviceUri + "/" + convertItem.url;
                        this._convertItem = convertItem;
                        this._fileOriginalName = name;
                    };
                    // Save done handler 
                    SaveToDlg.prototype.saveDone = function (error) {
                        lt.Demos.Utils.Visibility.toggle($(this.el.export.fileSavingImage), false);
                        if (error)
                            alert(error);
                    };
                    return SaveToDlg;
                }());
                Dialogs.SaveToDlg = SaveToDlg;
                var ExportJobDlg = /** @class */ (function () {
                    function ExportJobDlg() {
                        var _this = this;
                        this.inner = null;
                        this.el = null;
                        this.onHide = function () {
                            _this.inner.hide();
                        };
                        this._statusJobData = null;
                        this.onLoad = null;
                        this.loadBtn_Click = function (e) {
                            if (_this.onLoad)
                                _this.onLoad(_this._statusJobData.outputDocumentUri);
                            _this.inner.hide();
                        };
                        var root = $("#dlgExportJob");
                        this.el = {
                            uri: {
                                input: "#dlgExportJob_Uri_Input",
                                loadBtn: "#dlgExportJob_Uri_Load",
                            },
                            export: {
                                localSaveLink: "#dlgExportJob_LocalSave",
                            },
                            hide: "#dlgExportJob .dlg-close"
                        };
                        this.inner = new lt.Demos.Dialogs.InnerDialog(root);
                        $(this.el.hide).on("click", this.onHide);
                        $(this.el.uri.loadBtn).on("click", this.loadBtn_Click);
                    }
                    ExportJobDlg.prototype.dispose = function () {
                        $(this.el.hide).off("click", this.onHide);
                        this.onHide = null;
                        $(this.el.uri.loadBtn).off("click", this.loadBtn_Click);
                        this.onLoad = null;
                        this._statusJobData = null;
                        this.inner.dispose();
                        this.inner = null;
                        this.el = null;
                    };
                    ExportJobDlg.prototype.show = function (statusJobData) {
                        this._statusJobData = statusJobData;
                        var url = lt.Document.Service.Custom.createEndpointGetUrl("Factory", "DownloadDocument", {
                            uri: statusJobData.outputDocumentUri,
                            includeAnnotations: true,
                            userData: lt.Document.DocumentFactory.serviceUserData
                        }, true);
                        $(this.el.export.localSaveLink).attr("href", url);
                        $(this.el.uri.input).val(statusJobData.outputDocumentUri);
                        this.inner.show();
                    };
                    return ExportJobDlg;
                }());
                Dialogs.ExportJobDlg = ExportJobDlg;
            })(Dialogs = Converter.Dialogs || (Converter.Dialogs = {}));
        })(Converter = DocumentViewerDemo.Converter || (DocumentViewerDemo.Converter = {}));
    })(DocumentViewerDemo = HTML5Demos.DocumentViewerDemo || (HTML5Demos.DocumentViewerDemo = {}));
})(HTML5Demos || (HTML5Demos = {}));

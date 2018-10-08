// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.
// All Rights Reserved.
// *************************************************************
var HTML5Demos;
(function (HTML5Demos) {
    var DocumentViewerDemo;
    (function (DocumentViewerDemo) {
        // Contains the interactive part
        var ComparisonPart = /** @class */ (function () {
            function ComparisonPart(main) {
                // Reference to the DocumentViewerDemoApp
                this._mainApp = null;
                this._opacity = .5;
                // Comparison menu items
                this.headerToolbar_ComparisonMenu = {
                    sideBySideMenuItem: "#sideBySide",
                    overlayMenuItem: "#overlay",
                    cycleActiveMenuItem: "#cycleActive",
                    syncEnabledMenuItem: "#syncEnabled",
                    isSyncSourceMenuItem: "#isSyncSource",
                    isSyncTargetMenuItem: "#isSyncTarget",
                };
                // Shortcuts
                this.shortcuts = {
                    sideBySideBtn: "#sideBySide_shortcut",
                    overlayBtn: "#overlay_shortcut",
                    cycleActiveBtn: "#cycleActive_shortcut",
                    syncEnabledBtn: "#syncEnabled_shortcut",
                    opacitySlider: "#syncOpacitySlider",
                };
                this._mainApp = main;
                this.initComparisonUI();
            }
            ComparisonPart.prototype.initComparisonUI = function () {
                var _this = this;
                // Side by side and Overlay
                var sideBySide_Click = this._mainApp.setComparisonMode.bind(this._mainApp, DocumentViewerDemo.ComparisonMode.sideBySide);
                $(this.headerToolbar_ComparisonMenu.sideBySideMenuItem).on("click", sideBySide_Click);
                $(this.shortcuts.sideBySideBtn).on("click", sideBySide_Click);
                var overlay_Click = this._mainApp.setComparisonMode.bind(this._mainApp, DocumentViewerDemo.ComparisonMode.overlay);
                $(this.headerToolbar_ComparisonMenu.overlayMenuItem).on("click", overlay_Click);
                $(this.shortcuts.overlayBtn).on("click", overlay_Click);
                // Cycle
                var cycle_Click = this.cycleActiveDocumentViewer.bind(this);
                $(this.headerToolbar_ComparisonMenu.cycleActiveMenuItem).on("click", cycle_Click);
                $(this.shortcuts.cycleActiveBtn).on("click", cycle_Click);
                // Enabled
                var syncEnabled_Click = this.toggleSyncEnabled.bind(this);
                $(this.headerToolbar_ComparisonMenu.syncEnabledMenuItem).on("click", syncEnabled_Click);
                $(this.shortcuts.syncEnabledBtn).on("click", syncEnabled_Click);
                // Is Source and Is Target
                var syncSource_Click = this.toggleSyncSource.bind(this);
                $(this.headerToolbar_ComparisonMenu.isSyncSourceMenuItem).on("click", syncSource_Click);
                var syncTarget_Click = this.toggleSyncTarget.bind(this);
                $(this.headerToolbar_ComparisonMenu.isSyncTargetMenuItem).on("click", syncTarget_Click);
                // Overlay Opacity Slider
                // Use a jquery slider, since input=range sliders are not supported in IE9
                $(this.shortcuts.opacitySlider).slider({
                    min: 0,
                    max: 1,
                    step: .01,
                    value: this._opacity,
                    slide: function (e, ui) {
                        if (_this._opacity != ui.value) {
                            _this._opacity = ui.value;
                            _this.updateOpacity();
                        }
                    }
                });
                this.updateOpacity();
                this.updateComparisonUI();
            };
            ComparisonPart.prototype.updateComparisonUI = function () {
                var mode = this._mainApp.comparisonMode;
                var isSideBySide = mode === DocumentViewerDemo.ComparisonMode.sideBySide;
                lt.Demos.Utils.UI.toggleChecked($(this.headerToolbar_ComparisonMenu.sideBySideMenuItem).find(".icon"), isSideBySide);
                lt.Demos.Utils.UI.toggleChecked($(this.shortcuts.sideBySideBtn), isSideBySide);
                lt.Demos.Utils.UI.toggleChecked($(this.headerToolbar_ComparisonMenu.overlayMenuItem).find(".icon"), !isSideBySide);
                lt.Demos.Utils.UI.toggleChecked($(this.shortcuts.overlayBtn), !isSideBySide);
                // Show slider only for Overlay
                $(this.shortcuts.opacitySlider).toggle(!isSideBySide);
                this.updateOpacity();
                if (this._mainApp.activeDocumentViewer) {
                    this.updateComparisonActiveSourceTargetUI();
                }
            };
            ComparisonPart.prototype.updateComparisonActiveSourceTargetUI = function () {
                var imageViewer = this._mainApp.activeDocumentViewer.view.imageViewer;
                var isDisabled = lt.Controls.ImageViewer.isSyncDisabled(imageViewer.syncId);
                var isSource = imageViewer.isSyncSource;
                var isTarget = imageViewer.isSyncTarget;
                lt.Demos.Utils.UI.toggleChecked($(this.headerToolbar_ComparisonMenu.syncEnabledMenuItem).find(".icon"), !isDisabled);
                lt.Demos.Utils.UI.toggleChecked($(this.shortcuts.syncEnabledBtn), !isDisabled);
                lt.Demos.Utils.UI.toggleChecked($(this.headerToolbar_ComparisonMenu.isSyncSourceMenuItem).find(".icon"), isSource);
                $(this.headerToolbar_ComparisonMenu.isSyncSourceMenuItem).prop("disabled", isDisabled);
                lt.Demos.Utils.UI.toggleChecked($(this.headerToolbar_ComparisonMenu.isSyncTargetMenuItem).find(".icon"), isTarget);
                $(this.headerToolbar_ComparisonMenu.isSyncTargetMenuItem).prop("disabled", isDisabled);
                this.updateOpacity();
            };
            ComparisonPart.prototype.updateOpacity = function () {
                var opacity = this._mainApp.comparisonMode === DocumentViewerDemo.ComparisonMode.overlay ? this._opacity : 1;
                var $inactive = $("." + DocumentViewerDemo.DocumentViewerDemoApp.inactiveComparisonClass);
                var $bottom = $inactive.first();
                $bottom.css("opacity", 1);
                var $opacityElements = $inactive.not($bottom).add("." + DocumentViewerDemo.DocumentViewerDemoApp.activeComparisonClass);
                $opacityElements.css("opacity", opacity);
            };
            ComparisonPart.prototype.cycleActiveDocumentViewer = function () {
                if (this._mainApp.activeDocumentViewer) {
                    var index = this._mainApp.activeDocumentViewer.tag.index;
                    var newIndex = (index + 1) % this._mainApp.documentViewers.length;
                    this._mainApp.setActiveDocumentViewer(newIndex);
                    this.updateComparisonActiveSourceTargetUI();
                }
            };
            ComparisonPart.prototype.toggleSyncSource = function () {
                if (this._mainApp.activeDocumentViewer) {
                    this._mainApp.activeDocumentViewer.view.imageViewer.isSyncSource = !this._mainApp.activeDocumentViewer.view.imageViewer.isSyncSource;
                    this.updateComparisonActiveSourceTargetUI();
                }
            };
            ComparisonPart.prototype.toggleSyncTarget = function () {
                if (this._mainApp.activeDocumentViewer) {
                    this._mainApp.activeDocumentViewer.view.imageViewer.isSyncTarget = !this._mainApp.activeDocumentViewer.view.imageViewer.isSyncTarget;
                    this.updateComparisonActiveSourceTargetUI();
                }
            };
            ComparisonPart.prototype.toggleSyncEnabled = function () {
                if (this._mainApp.activeDocumentViewer) {
                    var syncId = this._mainApp.activeDocumentViewer.view.imageViewer.syncId;
                    if (lt.Controls.ImageViewer.isSyncDisabled(syncId))
                        lt.Controls.ImageViewer.enableSync(syncId);
                    else
                        lt.Controls.ImageViewer.disableSync(syncId);
                    this.updateComparisonActiveSourceTargetUI();
                }
            };
            ComparisonPart._topOverlayClass = "top";
            return ComparisonPart;
        }());
        DocumentViewerDemo.ComparisonPart = ComparisonPart;
    })(DocumentViewerDemo = HTML5Demos.DocumentViewerDemo || (HTML5Demos.DocumentViewerDemo = {}));
})(HTML5Demos || (HTML5Demos = {}));

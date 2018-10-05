// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.
// All Rights Reserved.
// *************************************************************
var HTML5Demos;
(function (HTML5Demos) {
    var DocumentViewerDemo;
    (function (DocumentViewerDemo) {
        // This demo only supports rendering of existing annotations.
        var AnnotationRenderer = /** @class */ (function () {
            function AnnotationRenderer(documentViewer) {
                // Reference to the DocumentViewer
                this._documentViewer = null;
                // So we can switch the renderers when doing custom rendering more
                this._loadPictureTimeout = -1;
                // annotations Objects buttons
                this._annotationsObjectsBtns = ".annotationObjectBtn";
                this._documentViewer = documentViewer;
            }
            AnnotationRenderer.prototype.initAutomation = function () {
                if (this._documentViewer.annotations == null)
                    return;
                // Get the automation manager from the document viewer
                var automationManager = this._documentViewer.annotations.automationManager;
                // Create the manager helper. This sets the rendering engine
                this._automationManagerHelper = new lt.Demos.Annotations.AutomationManagerHelper(automationManager, "Resources");
                // Inform the document viewer that automation manager helper is created
                this._documentViewer.annotations.initialize();
                // Update our automation objects (set transparency, etc)
                //this._automationManagerHelper.updateAutomationObjects();
                //this._automationObjectsList = new lt.Demos.Annotations.AutomationObjectsListControl();
                this._documentViewer.commands.run(lt.Document.Viewer.DocumentViewerCommands.annotationsUserModeRender, null);
            };
            AnnotationRenderer.prototype.handleContainersAddedOrRemoved = function () {
                if (this._automationObjectsList)
                    this._automationObjectsList.populate();
            };
            AnnotationRenderer.prototype.handleCreateAutomation = function () {
                var _this = this;
                if (!this._documentViewer.hasDocument)
                    return;
                // Get the automation object from the document viewer
                var automation = this._documentViewer.annotations.automation;
                if (automation == null)
                    return;
                // Optional: If the document is PDF then switch annotations to use PDF mode
                // This will instruct the document viewer to render automation in a similar manner to Adobe Acrobat where
                var mimeType = this._documentViewer.document.mimeType;
                if (mimeType && mimeType.toLowerCase() == "application/pdf") {
                    automation.manager.usePDFMode = true;
                }
                else {
                    automation.manager.usePDFMode = false;
                }
                var automationControl = this._documentViewer.annotations.automationControl;
                var automationManager = this._documentViewer.annotations.automationManager;
                var renderingEngine = this._documentViewer.annotations.automationManager.renderingEngine;
                // Hook to the events
                renderingEngine.loadPicture.add(function (sender, e) { return _this.renderingEngine_LoadPicture(sender, e); });
                automationManager.resources = this.loadResources();
            };
            AnnotationRenderer.prototype.loadResources = function () {
                var resources = new lt.Annotations.Engine.AnnResources();
                var rubberStampsResources = resources.rubberStamps;
                var imagesResources = resources.images;
                var objects = "Resources/Objects/";
                var rubberStamps = "Resources/Objects/RubberStamps/";
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampApproved] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Approved.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampAssigned] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Assigned.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampClient] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Client.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampChecked] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Checked.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampCopy] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Copy.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampDraft] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Draft.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampExtended] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Extended.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampFax] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Fax.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampFaxed] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Faxed.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampImportant] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Important.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampInvoice] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Invoice.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampNotice] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Notice.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampPaid] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Paid.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampOfficial] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Official.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampOnFile] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Onfile.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampPassed] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Passed.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampPending] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Pending.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampProcessed] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Processed.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampReceived] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Received.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampRejected] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Rejected.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampRelease] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Release.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampSent] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Sent.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampShipped] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Shipped.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampTopSecret] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "TopSecret.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampUrgent] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Urgent.png");
                rubberStampsResources[lt.Annotations.Engine.AnnRubberStampType.stampVoid] = new lt.Annotations.Engine.AnnPicture(rubberStamps + "Void.png");
                imagesResources[0] = new lt.Annotations.Engine.AnnPicture(objects + "Point.png");
                imagesResources[1] = new lt.Annotations.Engine.AnnPicture(objects + "Lock.png");
                imagesResources[2] = new lt.Annotations.Engine.AnnPicture(objects + "Hotspot.png");
                imagesResources[3] = new lt.Annotations.Engine.AnnPicture(objects + "Audio.png");
                imagesResources[4] = new lt.Annotations.Engine.AnnPicture(objects + "Video.png");
                imagesResources[5] = new lt.Annotations.Engine.AnnPicture(objects + "EncryptPrimary.png");
                imagesResources[6] = new lt.Annotations.Engine.AnnPicture(objects + "EncryptSecondary.png");
                imagesResources[7] = new lt.Annotations.Engine.AnnPicture(objects + "Note.png");
                imagesResources[8] = new lt.Annotations.Engine.AnnPicture(objects + "StickyNote.png");
                return resources;
            };
            AnnotationRenderer.prototype.closeDocument = function () {
                // The document has been closed or a new one is set, clear the load picture timeout if we have any
                if (this._loadPictureTimeout !== -1) {
                    clearTimeout(this._loadPictureTimeout);
                    this._loadPictureTimeout = -1;
                }
            };
            AnnotationRenderer.prototype.renderingEngine_LoadPicture = function (sender, e) {
                var _this = this;
                // The renderingEngine.loadPicture occurs for every annotation object that has an embedded image
                // So instead of re-rendering the annotations every time one of these images is loaded, we will use a timer
                // to group the paints together for optimization.
                if (this._loadPictureTimeout !== -1) {
                    return;
                }
                this._loadPictureTimeout = setTimeout(function () {
                    _this._loadPictureTimeout = -1;
                    _this._documentViewer.annotations.automation.invalidate(lt.LeadRectD.empty);
                }, 1000);
            };
            return AnnotationRenderer;
        }());
        DocumentViewerDemo.AnnotationRenderer = AnnotationRenderer;
    })(DocumentViewerDemo = HTML5Demos.DocumentViewerDemo || (HTML5Demos.DocumentViewerDemo = {}));
})(HTML5Demos || (HTML5Demos = {}));

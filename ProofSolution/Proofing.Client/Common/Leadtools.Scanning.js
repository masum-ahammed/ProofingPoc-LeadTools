// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.
// All Rights Reserved.
// *************************************************************
var lt;
(function (lt) {
    var Scanning;
    (function (Scanning) {
        var JavaScript;
        (function (JavaScript) {
            var SaneScanning = (function () {
                function SaneScanning(portNumber) {
                    this._service = new lt.Sane.JavaScript.SaneService(portNumber);
                }
                SaneScanning.prototype.init = function (onSuccess, onFailure) {
                    this._service.init(null, onSuccess, onFailure);
                };
                SaneScanning.prototype.getStatus = function (onSuccess, onFailure) {
                    this._service.getStatus(null, function (saneStatus) {
                        var status = new lt.Scanning.JavaScript.ScanningStatus();
                        status.isScanning = saneStatus.isScanning;
                        status.pageCount = saneStatus.pageCount;
                        status.selectedSource = saneStatus.selectedSource;
                        status.errCode = saneStatus.error.code;
                        status.errMessage = saneStatus.error.message;
                        onSuccess(status);
                    }, onFailure);
                };
                SaneScanning.prototype.getSources = function (onSuccess, onFailure) {
                    this._service.getSources(null, function (saneSources) {
                        var sources = [];
                        for (var i = 0; i < saneSources.length; i++)
                            sources.push(saneSources[i].name);
                        onSuccess(sources);
                    }, onFailure);
                };
                SaneScanning.prototype.selectSource = function (sourceName, onSuccess, onFailure) {
                    this._service.selectSource(sourceName, null, onSuccess, onFailure);
                };
                SaneScanning.prototype.acquire = function (onSuccess, onFailure) {
                    var _this = this;
                    this._service.acquire(null, function () {
                        _this.getStatus(onSuccess, onFailure);
                    }, onFailure);
                };
                SaneScanning.prototype.getPage = function (pageNumber, format, bpp, width, height) {
                    return this._service.getPage(pageNumber, format, bpp, width, height, null);
                };
                SaneScanning.prototype.stop = function (onSuccess, onFailure) {
                    this._service.stop(null, onSuccess, onFailure);
                };
                SaneScanning.prototype.start = function (onSuccess, onFailure) {
                    this._service.start(null, onSuccess, onFailure);
                };
                SaneScanning.prototype.applyImageProcessingCommand = function (firstPageNumber, lastPageNumber, commandName, args, onSuccess, onFailure) {
                    this._service.applyImageProcessingCommand(firstPageNumber, lastPageNumber, commandName, args, null, onSuccess, onFailure);
                };
                SaneScanning.prototype.getImageProcessingPreview = function (pageNumber, commandName, args, width, height, format, bpp) {
                    return this._service.getImageProcessingPreview(pageNumber, commandName, args, width, height, format, bpp, null);
                };
                SaneScanning.prototype.runCommand = function (commandName, args, userData) {
                    return this._service.runCommand(commandName, args, userData);
                };
                SaneScanning.prototype.getHandle = function () {
                    return this._service;
                };
                return SaneScanning;
            }());
            JavaScript.SaneScanning = SaneScanning;
        })(JavaScript = Scanning.JavaScript || (Scanning.JavaScript = {}));
    })(Scanning = lt.Scanning || (lt.Scanning = {}));
})(lt || (lt = {}));
var lt;
(function (lt) {
    var Scanning;
    (function (Scanning) {
        var JavaScript;
        (function (JavaScript) {
            var TwainScanning = (function () {
                function TwainScanning(serviceUrl) {
                    this._service = new lt.Twain.JavaScript.TwainService(serviceUrl);
                }
                TwainScanning.prototype.init = function (onSuccess, onFailure) {
                    this._service.init("", "", "", "", null, onSuccess, onFailure);
                };
                TwainScanning.prototype.getStatus = function (onSuccess, onFailure) {
                    this._service.getStatus(null, function (twainStatus) {
                        var status = new lt.Scanning.JavaScript.ScanningStatus();
                        status.isScanning = twainStatus.isScanning;
                        status.pageCount = twainStatus.pagesCount;
                        status.selectedSource = twainStatus.selectedSource;
                        status.errCode = twainStatus.error.code;
                        status.errMessage = twainStatus.error.message;
                        onSuccess(status);
                    }, onFailure);
                };
                TwainScanning.prototype.isAvailable = function (onSuccess, onFailure) {
                    this._service.isAvailable(null, onSuccess, onFailure);
                };
                TwainScanning.prototype.getSources = function (onSuccess, onFailure) {
                    this._service.getSources(null, function (twainSources) {
                        var sources = [];
                        for (var i = 0; i < twainSources.length; i++)
                            sources.push(twainSources[i].name);
                        onSuccess(sources);
                    }, onFailure);
                };
                TwainScanning.prototype.selectSource = function (sourceName, onSuccess, onFailure) {
                    this._service.selectSource(sourceName, null, onSuccess, onFailure);
                };
                TwainScanning.prototype.acquire = function (onSuccess, onFailure) {
                    var _this = this;
                    this._service.acquire(lt.Twain.JavaScript.TwainUserInterfaceFlags.Modal | lt.Twain.JavaScript.TwainUserInterfaceFlags.Show, null, function () {
                        _this.getStatus(onSuccess, onFailure);
                    }, onFailure);
                };
                TwainScanning.prototype.getPage = function (pageNumber, format, bpp, width, height) {
                    return this._service.getPage(pageNumber, format, bpp, width, height, null);
                };
                TwainScanning.prototype.stop = function (onSuccess, onFailure) {
                    this._service.stop(null, onSuccess, onFailure);
                };
                TwainScanning.prototype.start = function (onSuccess, onFailure) {
                    this._service.start(null, onSuccess, onFailure);
                };
                TwainScanning.prototype.applyImageProcessingCommand = function (firstPageNumber, lastPageNumber, commandName, args, onSuccess, onFailure) {
                    this._service.applyImageProcessingCommand(firstPageNumber, lastPageNumber, commandName, args, null, onSuccess, onFailure);
                };
                TwainScanning.prototype.getImageProcessingPreview = function (pageNumber, commandName, args, width, height, format, bpp) {
                    return this._service.getImageProcessingPreview(pageNumber, commandName, args, width, height, format, bpp, null);
                };
                TwainScanning.prototype.runCommand = function (commandName, args, userData) {
                    return this._service.runCommand(commandName, args, userData);
                };
                TwainScanning.prototype.getHandle = function () {
                    return this._service;
                };
                return TwainScanning;
            }());
            JavaScript.TwainScanning = TwainScanning;
        })(JavaScript = Scanning.JavaScript || (Scanning.JavaScript = {}));
    })(Scanning = lt.Scanning || (lt.Scanning = {}));
})(lt || (lt = {}));
var lt;
(function (lt) {
    var Twain;
    (function (Twain) {
        var JavaScript;
        (function (JavaScript) {
            (function (TwainContainerType) {
                TwainContainerType[TwainContainerType["DontCare32"] = -1] = "DontCare32";
                TwainContainerType[TwainContainerType["Array"] = 3] = "Array";
                TwainContainerType[TwainContainerType["Enumeration"] = 4] = "Enumeration";
                TwainContainerType[TwainContainerType["OneValue"] = 5] = "OneValue";
                TwainContainerType[TwainContainerType["Range"] = 6] = "Range";
                TwainContainerType[TwainContainerType["DsmCodeId"] = 63] = "DsmCodeId";
                TwainContainerType[TwainContainerType["DontCare8"] = 255] = "DontCare8";
                TwainContainerType[TwainContainerType["DsmId"] = 461] = "DsmId";
                TwainContainerType[TwainContainerType["IconId"] = 962] = "IconId";
                TwainContainerType[TwainContainerType["DontCare16"] = 65535] = "DontCare16";
            })(JavaScript.TwainContainerType || (JavaScript.TwainContainerType = {}));
            var TwainContainerType = JavaScript.TwainContainerType;
            (function (TwainUserInterfaceFlags) {
                TwainUserInterfaceFlags[TwainUserInterfaceFlags["None"] = 0] = "None";
                TwainUserInterfaceFlags[TwainUserInterfaceFlags["Show"] = 1] = "Show";
                TwainUserInterfaceFlags[TwainUserInterfaceFlags["Modal"] = 2] = "Modal";
                TwainUserInterfaceFlags[TwainUserInterfaceFlags["KeepOpen"] = 32] = "KeepOpen";
                TwainUserInterfaceFlags[TwainUserInterfaceFlags["MemoryCheckImageInfo"] = 64] = "MemoryCheckImageInfo";
                TwainUserInterfaceFlags[TwainUserInterfaceFlags["ImageSizeUndefined"] = 128] = "ImageSizeUndefined";
            })(JavaScript.TwainUserInterfaceFlags || (JavaScript.TwainUserInterfaceFlags = {}));
            var TwainUserInterfaceFlags = JavaScript.TwainUserInterfaceFlags;
            (function (TwainItemType) {
                TwainItemType[TwainItemType["Int8"] = 0] = "Int8";
                TwainItemType[TwainItemType["Int16"] = 1] = "Int16";
                TwainItemType[TwainItemType["Int32"] = 2] = "Int32";
                TwainItemType[TwainItemType["Uint8"] = 3] = "Uint8";
                TwainItemType[TwainItemType["Uint16"] = 4] = "Uint16";
                TwainItemType[TwainItemType["Uint32"] = 5] = "Uint32";
                TwainItemType[TwainItemType["Bool"] = 6] = "Bool";
                TwainItemType[TwainItemType["Fix32"] = 7] = "Fix32";
                TwainItemType[TwainItemType["Frame"] = 8] = "Frame";
                TwainItemType[TwainItemType["Str32"] = 9] = "Str32";
                TwainItemType[TwainItemType["Str64"] = 10] = "Str64";
                TwainItemType[TwainItemType["Str128"] = 11] = "Str128";
                TwainItemType[TwainItemType["Str255"] = 12] = "Str255";
                TwainItemType[TwainItemType["Str1024"] = 13] = "Str1024";
                TwainItemType[TwainItemType["Uni512"] = 14] = "Uni512";
                TwainItemType[TwainItemType["Handle"] = 15] = "Handle";
            })(JavaScript.TwainItemType || (JavaScript.TwainItemType = {}));
            var TwainItemType = JavaScript.TwainItemType;
            (function (TwainGetCapabilityMode) {
                TwainGetCapabilityMode[TwainGetCapabilityMode["DontGet"] = 0] = "DontGet";
                TwainGetCapabilityMode[TwainGetCapabilityMode["GetCurrent"] = 3] = "GetCurrent";
                TwainGetCapabilityMode[TwainGetCapabilityMode["GetDefault"] = 4] = "GetDefault";
                TwainGetCapabilityMode[TwainGetCapabilityMode["GetValues"] = 5] = "GetValues";
            })(JavaScript.TwainGetCapabilityMode || (JavaScript.TwainGetCapabilityMode = {}));
            var TwainGetCapabilityMode = JavaScript.TwainGetCapabilityMode;
            (function (TwainCapabilityType) {
                TwainCapabilityType[TwainCapabilityType["TransferCount"] = 1] = "TransferCount";
                TwainCapabilityType[TwainCapabilityType["ImageCompression"] = 256] = "ImageCompression";
                TwainCapabilityType[TwainCapabilityType["ImagePixelType"] = 257] = "ImagePixelType";
                TwainCapabilityType[TwainCapabilityType["ImageUnits"] = 258] = "ImageUnits";
                TwainCapabilityType[TwainCapabilityType["ImageTransferMechanism"] = 259] = "ImageTransferMechanism";
                TwainCapabilityType[TwainCapabilityType["Author"] = 4096] = "Author";
                TwainCapabilityType[TwainCapabilityType["Caption"] = 4097] = "Caption";
                TwainCapabilityType[TwainCapabilityType["FeederEnabled"] = 4098] = "FeederEnabled";
                TwainCapabilityType[TwainCapabilityType["FeederLoaded"] = 4099] = "FeederLoaded";
                TwainCapabilityType[TwainCapabilityType["TimeDate"] = 4100] = "TimeDate";
                TwainCapabilityType[TwainCapabilityType["SupportedCaps"] = 4101] = "SupportedCaps";
                TwainCapabilityType[TwainCapabilityType["ExtendedCaps"] = 4102] = "ExtendedCaps";
                TwainCapabilityType[TwainCapabilityType["AutoFeed"] = 4103] = "AutoFeed";
                TwainCapabilityType[TwainCapabilityType["ClearPage"] = 4104] = "ClearPage";
                TwainCapabilityType[TwainCapabilityType["FeedPage"] = 4105] = "FeedPage";
                TwainCapabilityType[TwainCapabilityType["RewindPage"] = 4106] = "RewindPage";
                TwainCapabilityType[TwainCapabilityType["Indicators"] = 4107] = "Indicators";
                TwainCapabilityType[TwainCapabilityType["SupportedCapsExt"] = 4108] = "SupportedCapsExt";
                TwainCapabilityType[TwainCapabilityType["PaperDetectable"] = 4109] = "PaperDetectable";
                TwainCapabilityType[TwainCapabilityType["UIControllable"] = 4110] = "UIControllable";
                TwainCapabilityType[TwainCapabilityType["DeviceOnline"] = 4111] = "DeviceOnline";
                TwainCapabilityType[TwainCapabilityType["AutoScan"] = 4112] = "AutoScan";
                TwainCapabilityType[TwainCapabilityType["ThumbnailsEnabled"] = 4113] = "ThumbnailsEnabled";
                TwainCapabilityType[TwainCapabilityType["Duplex"] = 4114] = "Duplex";
                TwainCapabilityType[TwainCapabilityType["DuplexEnabled"] = 4115] = "DuplexEnabled";
                TwainCapabilityType[TwainCapabilityType["EnabledSuiOnly"] = 4116] = "EnabledSuiOnly";
                TwainCapabilityType[TwainCapabilityType["CustomDSData"] = 4117] = "CustomDSData";
                TwainCapabilityType[TwainCapabilityType["Endorser"] = 4118] = "Endorser";
                TwainCapabilityType[TwainCapabilityType["JobControl"] = 4119] = "JobControl";
                TwainCapabilityType[TwainCapabilityType["Alarms"] = 4120] = "Alarms";
                TwainCapabilityType[TwainCapabilityType["AlarmVolume"] = 4121] = "AlarmVolume";
                TwainCapabilityType[TwainCapabilityType["AutomaticCapture"] = 4122] = "AutomaticCapture";
                TwainCapabilityType[TwainCapabilityType["TimeBeforeFirstCapture"] = 4123] = "TimeBeforeFirstCapture";
                TwainCapabilityType[TwainCapabilityType["TimeBetweenCaptures"] = 4124] = "TimeBetweenCaptures";
                TwainCapabilityType[TwainCapabilityType["ClearBuffers"] = 4125] = "ClearBuffers";
                TwainCapabilityType[TwainCapabilityType["MaxBatchBuffers"] = 4126] = "MaxBatchBuffers";
                TwainCapabilityType[TwainCapabilityType["DeviceTimeDate"] = 4127] = "DeviceTimeDate";
                TwainCapabilityType[TwainCapabilityType["PowerSupply"] = 4128] = "PowerSupply";
                TwainCapabilityType[TwainCapabilityType["CameraPreviewUI"] = 4129] = "CameraPreviewUI";
                TwainCapabilityType[TwainCapabilityType["DeviceEvent"] = 4130] = "DeviceEvent";
                TwainCapabilityType[TwainCapabilityType["SerialNumber"] = 4132] = "SerialNumber";
                TwainCapabilityType[TwainCapabilityType["Printer"] = 4134] = "Printer";
                TwainCapabilityType[TwainCapabilityType["PrinterEnabled"] = 4135] = "PrinterEnabled";
                TwainCapabilityType[TwainCapabilityType["PrinterIndex"] = 4136] = "PrinterIndex";
                TwainCapabilityType[TwainCapabilityType["PrinterMode"] = 4137] = "PrinterMode";
                TwainCapabilityType[TwainCapabilityType["PrinterString"] = 4138] = "PrinterString";
                TwainCapabilityType[TwainCapabilityType["PrinterSuffix"] = 4139] = "PrinterSuffix";
                TwainCapabilityType[TwainCapabilityType["Language"] = 4140] = "Language";
                TwainCapabilityType[TwainCapabilityType["FeederAlignment"] = 4141] = "FeederAlignment";
                TwainCapabilityType[TwainCapabilityType["FeederOrder"] = 4142] = "FeederOrder";
                TwainCapabilityType[TwainCapabilityType["ReacquireAllowed"] = 4144] = "ReacquireAllowed";
                TwainCapabilityType[TwainCapabilityType["BatteryMinutes"] = 4146] = "BatteryMinutes";
                TwainCapabilityType[TwainCapabilityType["BatteryPercentage"] = 4147] = "BatteryPercentage";
                TwainCapabilityType[TwainCapabilityType["CameraSide"] = 4148] = "CameraSide";
                TwainCapabilityType[TwainCapabilityType["Segmented"] = 4149] = "Segmented";
                TwainCapabilityType[TwainCapabilityType["CameraEnabled"] = 4150] = "CameraEnabled";
                TwainCapabilityType[TwainCapabilityType["CameraOrder"] = 4151] = "CameraOrder";
                TwainCapabilityType[TwainCapabilityType["MicrEnabled"] = 4152] = "MicrEnabled";
                TwainCapabilityType[TwainCapabilityType["FeederPrep"] = 4153] = "FeederPrep";
                TwainCapabilityType[TwainCapabilityType["FeederPocket"] = 4154] = "FeederPocket";
                TwainCapabilityType[TwainCapabilityType["ImageAutoBright"] = 4352] = "ImageAutoBright";
                TwainCapabilityType[TwainCapabilityType["ImageBrightness"] = 4353] = "ImageBrightness";
                TwainCapabilityType[TwainCapabilityType["ImageContrast"] = 4355] = "ImageContrast";
                TwainCapabilityType[TwainCapabilityType["ImageCustomHalftone"] = 4356] = "ImageCustomHalftone";
                TwainCapabilityType[TwainCapabilityType["ImageExposureTime"] = 4357] = "ImageExposureTime";
                TwainCapabilityType[TwainCapabilityType["ImageFilter"] = 4358] = "ImageFilter";
                TwainCapabilityType[TwainCapabilityType["ImageFlashUsed"] = 4359] = "ImageFlashUsed";
                TwainCapabilityType[TwainCapabilityType["ImageGamma"] = 4360] = "ImageGamma";
                TwainCapabilityType[TwainCapabilityType["ImageHalftones"] = 4361] = "ImageHalftones";
                TwainCapabilityType[TwainCapabilityType["ImageHighlight"] = 4362] = "ImageHighlight";
                TwainCapabilityType[TwainCapabilityType["ImageImageFileFormat"] = 4364] = "ImageImageFileFormat";
                TwainCapabilityType[TwainCapabilityType["ImageLampState"] = 4365] = "ImageLampState";
                TwainCapabilityType[TwainCapabilityType["ImageLightSource"] = 4366] = "ImageLightSource";
                TwainCapabilityType[TwainCapabilityType["ImageOrientation"] = 4368] = "ImageOrientation";
                TwainCapabilityType[TwainCapabilityType["ImagePhysicalWidth"] = 4369] = "ImagePhysicalWidth";
                TwainCapabilityType[TwainCapabilityType["ImagePhysicalHeight"] = 4370] = "ImagePhysicalHeight";
                TwainCapabilityType[TwainCapabilityType["ImageShadow"] = 4371] = "ImageShadow";
                TwainCapabilityType[TwainCapabilityType["ImageFrames"] = 4372] = "ImageFrames";
                TwainCapabilityType[TwainCapabilityType["ImageXNativeResolution"] = 4374] = "ImageXNativeResolution";
                TwainCapabilityType[TwainCapabilityType["ImageYNativeResolution"] = 4375] = "ImageYNativeResolution";
                TwainCapabilityType[TwainCapabilityType["ImageXResolution"] = 4376] = "ImageXResolution";
                TwainCapabilityType[TwainCapabilityType["ImageYResolution"] = 4377] = "ImageYResolution";
                TwainCapabilityType[TwainCapabilityType["ImageMaxFrames"] = 4378] = "ImageMaxFrames";
                TwainCapabilityType[TwainCapabilityType["ImageTiles"] = 4379] = "ImageTiles";
                TwainCapabilityType[TwainCapabilityType["ImageBitOrder"] = 4380] = "ImageBitOrder";
                TwainCapabilityType[TwainCapabilityType["ImageCcittKFactor"] = 4381] = "ImageCcittKFactor";
                TwainCapabilityType[TwainCapabilityType["ImageLightPath"] = 4382] = "ImageLightPath";
                TwainCapabilityType[TwainCapabilityType["ImagePixelFlavor"] = 4383] = "ImagePixelFlavor";
                TwainCapabilityType[TwainCapabilityType["ImagePlanarChunky"] = 4384] = "ImagePlanarChunky";
                TwainCapabilityType[TwainCapabilityType["ImageRotation"] = 4385] = "ImageRotation";
                TwainCapabilityType[TwainCapabilityType["ImageSupportedSizes"] = 4386] = "ImageSupportedSizes";
                TwainCapabilityType[TwainCapabilityType["ImageThreshold"] = 4387] = "ImageThreshold";
                TwainCapabilityType[TwainCapabilityType["ImageXScaling"] = 4388] = "ImageXScaling";
                TwainCapabilityType[TwainCapabilityType["ImageYScaling"] = 4389] = "ImageYScaling";
                TwainCapabilityType[TwainCapabilityType["ImageBitOrderCodes"] = 4390] = "ImageBitOrderCodes";
                TwainCapabilityType[TwainCapabilityType["ImagePixelFlavorCodes"] = 4391] = "ImagePixelFlavorCodes";
                TwainCapabilityType[TwainCapabilityType["ImageJpegPixelType"] = 4392] = "ImageJpegPixelType";
                TwainCapabilityType[TwainCapabilityType["ImageTimeFill"] = 4394] = "ImageTimeFill";
                TwainCapabilityType[TwainCapabilityType["ImageBitDepth"] = 4395] = "ImageBitDepth";
                TwainCapabilityType[TwainCapabilityType["ImageBitDepthReduction"] = 4396] = "ImageBitDepthReduction";
                TwainCapabilityType[TwainCapabilityType["ImageUndefinedImageSize"] = 4397] = "ImageUndefinedImageSize";
                TwainCapabilityType[TwainCapabilityType["ImageImageDataSet"] = 4398] = "ImageImageDataSet";
                TwainCapabilityType[TwainCapabilityType["ImageExtImageInfo"] = 4399] = "ImageExtImageInfo";
                TwainCapabilityType[TwainCapabilityType["ImageMinimumHeight"] = 4400] = "ImageMinimumHeight";
                TwainCapabilityType[TwainCapabilityType["ImageMinimumWidth"] = 4401] = "ImageMinimumWidth";
                TwainCapabilityType[TwainCapabilityType["AutoDiscardBlankPages"] = 4404] = "AutoDiscardBlankPages";
                TwainCapabilityType[TwainCapabilityType["ImageFlipRotation"] = 4406] = "ImageFlipRotation";
                TwainCapabilityType[TwainCapabilityType["ImageBarcodeDetectionEnabled"] = 4407] = "ImageBarcodeDetectionEnabled";
                TwainCapabilityType[TwainCapabilityType["ImageSupportedBarcodeTypes"] = 4408] = "ImageSupportedBarcodeTypes";
                TwainCapabilityType[TwainCapabilityType["ImageBarcodeMaxSearchPriorities"] = 4409] = "ImageBarcodeMaxSearchPriorities";
                TwainCapabilityType[TwainCapabilityType["ImageBarcodeSearchPriorities"] = 4410] = "ImageBarcodeSearchPriorities";
                TwainCapabilityType[TwainCapabilityType["ImageBarcodeSearchMode"] = 4411] = "ImageBarcodeSearchMode";
                TwainCapabilityType[TwainCapabilityType["ImageBarcodeMaxRetries"] = 4412] = "ImageBarcodeMaxRetries";
                TwainCapabilityType[TwainCapabilityType["ImageBarcodeTimeout"] = 4413] = "ImageBarcodeTimeout";
                TwainCapabilityType[TwainCapabilityType["ImageZoomFactor"] = 4414] = "ImageZoomFactor";
                TwainCapabilityType[TwainCapabilityType["ImagePatchCodeDetectionEnabled"] = 4415] = "ImagePatchCodeDetectionEnabled";
                TwainCapabilityType[TwainCapabilityType["ImageSupportedPatchCodeTypes"] = 4416] = "ImageSupportedPatchCodeTypes";
                TwainCapabilityType[TwainCapabilityType["ImagePatchCodeMaxSearchPriorities"] = 4417] = "ImagePatchCodeMaxSearchPriorities";
                TwainCapabilityType[TwainCapabilityType["ImagePatchCodeSearchPriorities"] = 4418] = "ImagePatchCodeSearchPriorities";
                TwainCapabilityType[TwainCapabilityType["ImagePatchCodeSearchMode"] = 4419] = "ImagePatchCodeSearchMode";
                TwainCapabilityType[TwainCapabilityType["ImagePatchCodeMaxRetries"] = 4420] = "ImagePatchCodeMaxRetries";
                TwainCapabilityType[TwainCapabilityType["ImagePatchCodeTimeout"] = 4421] = "ImagePatchCodeTimeout";
                TwainCapabilityType[TwainCapabilityType["ImageFlashUsed2"] = 4422] = "ImageFlashUsed2";
                TwainCapabilityType[TwainCapabilityType["ImageImageFilter"] = 4423] = "ImageImageFilter";
                TwainCapabilityType[TwainCapabilityType["ImageNoiseFilter"] = 4424] = "ImageNoiseFilter";
                TwainCapabilityType[TwainCapabilityType["ImageOverScan"] = 4425] = "ImageOverScan";
                TwainCapabilityType[TwainCapabilityType["ImageAutomaticBorderDetection"] = 4432] = "ImageAutomaticBorderDetection";
                TwainCapabilityType[TwainCapabilityType["ImageAutomaticDeskew"] = 4433] = "ImageAutomaticDeskew";
                TwainCapabilityType[TwainCapabilityType["ImageAutomaticRotate"] = 4434] = "ImageAutomaticRotate";
                TwainCapabilityType[TwainCapabilityType["ImageJpegQuality"] = 4435] = "ImageJpegQuality";
                TwainCapabilityType[TwainCapabilityType["ImageFeederType"] = 4436] = "ImageFeederType";
                TwainCapabilityType[TwainCapabilityType["ImageIccProfile"] = 4437] = "ImageIccProfile";
                TwainCapabilityType[TwainCapabilityType["ImageAutoSize"] = 4438] = "ImageAutoSize";
                TwainCapabilityType[TwainCapabilityType["AudioAudioFileFormat"] = 4609] = "AudioAudioFileFormat";
                TwainCapabilityType[TwainCapabilityType["AudioTransferMechanism"] = 4610] = "AudioTransferMechanism";
                TwainCapabilityType[TwainCapabilityType["CustomBase"] = 32768] = "CustomBase";
            })(JavaScript.TwainCapabilityType || (JavaScript.TwainCapabilityType = {}));
            var TwainCapabilityType = JavaScript.TwainCapabilityType;
            (function (RasterImageFormat) {
                RasterImageFormat[RasterImageFormat["Unknown"] = 0] = "Unknown";
                RasterImageFormat[RasterImageFormat["Pcx"] = 1] = "Pcx";
                RasterImageFormat[RasterImageFormat["Gif"] = 2] = "Gif";
                RasterImageFormat[RasterImageFormat["Tif"] = 3] = "Tif";
                RasterImageFormat[RasterImageFormat["Tga"] = 4] = "Tga";
                RasterImageFormat[RasterImageFormat["Cmp"] = 5] = "Cmp";
                RasterImageFormat[RasterImageFormat["Bmp"] = 6] = "Bmp";
                RasterImageFormat[RasterImageFormat["Jpeg"] = 10] = "Jpeg";
                RasterImageFormat[RasterImageFormat["TifJpeg"] = 11] = "TifJpeg";
                RasterImageFormat[RasterImageFormat["Os2"] = 14] = "Os2";
                RasterImageFormat[RasterImageFormat["Wmf"] = 15] = "Wmf";
                RasterImageFormat[RasterImageFormat["Eps"] = 16] = "Eps";
                RasterImageFormat[RasterImageFormat["TifLzw"] = 17] = "TifLzw";
                RasterImageFormat[RasterImageFormat["Jpeg411"] = 21] = "Jpeg411";
                RasterImageFormat[RasterImageFormat["TifJpeg411"] = 22] = "TifJpeg411";
                RasterImageFormat[RasterImageFormat["Jpeg422"] = 23] = "Jpeg422";
                RasterImageFormat[RasterImageFormat["TifJpeg422"] = 24] = "TifJpeg422";
                RasterImageFormat[RasterImageFormat["Ccitt"] = 25] = "Ccitt";
                RasterImageFormat[RasterImageFormat["Lead1Bit"] = 26] = "Lead1Bit";
                RasterImageFormat[RasterImageFormat["CcittGroup31Dim"] = 27] = "CcittGroup31Dim";
                RasterImageFormat[RasterImageFormat["CcittGroup32Dim"] = 28] = "CcittGroup32Dim";
                RasterImageFormat[RasterImageFormat["CcittGroup4"] = 29] = "CcittGroup4";
                RasterImageFormat[RasterImageFormat["Abc"] = 32] = "Abc";
                RasterImageFormat[RasterImageFormat["Cals"] = 50] = "Cals";
                RasterImageFormat[RasterImageFormat["Mac"] = 51] = "Mac";
                RasterImageFormat[RasterImageFormat["Img"] = 52] = "Img";
                RasterImageFormat[RasterImageFormat["Msp"] = 53] = "Msp";
                RasterImageFormat[RasterImageFormat["Wpg"] = 54] = "Wpg";
                RasterImageFormat[RasterImageFormat["Ras"] = 55] = "Ras";
                RasterImageFormat[RasterImageFormat["Pct"] = 56] = "Pct";
                RasterImageFormat[RasterImageFormat["Pcd"] = 57] = "Pcd";
                RasterImageFormat[RasterImageFormat["Dxf12"] = 58] = "Dxf12";
                RasterImageFormat[RasterImageFormat["Dxf"] = 58] = "Dxf";
                RasterImageFormat[RasterImageFormat["Fli"] = 61] = "Fli";
                RasterImageFormat[RasterImageFormat["Cgm"] = 62] = "Cgm";
                RasterImageFormat[RasterImageFormat["EpsTiff"] = 63] = "EpsTiff";
                RasterImageFormat[RasterImageFormat["EpsWmf"] = 64] = "EpsWmf";
                RasterImageFormat[RasterImageFormat["FaxG31Dim"] = 66] = "FaxG31Dim";
                RasterImageFormat[RasterImageFormat["FaxG32Dim"] = 67] = "FaxG32Dim";
                RasterImageFormat[RasterImageFormat["FaxG4"] = 68] = "FaxG4";
                RasterImageFormat[RasterImageFormat["WfxG31Dim"] = 69] = "WfxG31Dim";
                RasterImageFormat[RasterImageFormat["WfxG4"] = 70] = "WfxG4";
                RasterImageFormat[RasterImageFormat["IcaG31Dim"] = 71] = "IcaG31Dim";
                RasterImageFormat[RasterImageFormat["IcaG32Dim"] = 72] = "IcaG32Dim";
                RasterImageFormat[RasterImageFormat["IcaG4"] = 73] = "IcaG4";
                RasterImageFormat[RasterImageFormat["Os22"] = 74] = "Os22";
                RasterImageFormat[RasterImageFormat["Png"] = 75] = "Png";
                RasterImageFormat[RasterImageFormat["Psd"] = 76] = "Psd";
                RasterImageFormat[RasterImageFormat["RawIcaG31Dim"] = 77] = "RawIcaG31Dim";
                RasterImageFormat[RasterImageFormat["RawIcaG32Dim"] = 78] = "RawIcaG32Dim";
                RasterImageFormat[RasterImageFormat["RawIcaG4"] = 79] = "RawIcaG4";
                RasterImageFormat[RasterImageFormat["Fpx"] = 80] = "Fpx";
                RasterImageFormat[RasterImageFormat["FpxSingleColor"] = 81] = "FpxSingleColor";
                RasterImageFormat[RasterImageFormat["FpxJpeg"] = 82] = "FpxJpeg";
                RasterImageFormat[RasterImageFormat["FpxJpegQFactor"] = 83] = "FpxJpegQFactor";
                RasterImageFormat[RasterImageFormat["BmpRle"] = 84] = "BmpRle";
                RasterImageFormat[RasterImageFormat["TifCmyk"] = 85] = "TifCmyk";
                RasterImageFormat[RasterImageFormat["TifLzwCmyk"] = 86] = "TifLzwCmyk";
                RasterImageFormat[RasterImageFormat["TifPackBits"] = 87] = "TifPackBits";
                RasterImageFormat[RasterImageFormat["TifPackBitsCmyk"] = 88] = "TifPackBitsCmyk";
                RasterImageFormat[RasterImageFormat["DicomGray"] = 89] = "DicomGray";
                RasterImageFormat[RasterImageFormat["DicomColor"] = 90] = "DicomColor";
                RasterImageFormat[RasterImageFormat["WinIco"] = 91] = "WinIco";
                RasterImageFormat[RasterImageFormat["WinCur"] = 92] = "WinCur";
                RasterImageFormat[RasterImageFormat["TifYcc"] = 93] = "TifYcc";
                RasterImageFormat[RasterImageFormat["TifLzwYcc"] = 94] = "TifLzwYcc";
                RasterImageFormat[RasterImageFormat["TifPackbitsYcc"] = 95] = "TifPackbitsYcc";
                RasterImageFormat[RasterImageFormat["Exif"] = 96] = "Exif";
                RasterImageFormat[RasterImageFormat["ExifYcc"] = 97] = "ExifYcc";
                RasterImageFormat[RasterImageFormat["ExifJpeg"] = 98] = "ExifJpeg";
                RasterImageFormat[RasterImageFormat["ExifJpeg422"] = 98] = "ExifJpeg422";
                RasterImageFormat[RasterImageFormat["Awd"] = 99] = "Awd";
                RasterImageFormat[RasterImageFormat["ExifJpeg411"] = 101] = "ExifJpeg411";
                RasterImageFormat[RasterImageFormat["PbmAscii"] = 102] = "PbmAscii";
                RasterImageFormat[RasterImageFormat["PbmBinary"] = 103] = "PbmBinary";
                RasterImageFormat[RasterImageFormat["PgmAscii"] = 104] = "PgmAscii";
                RasterImageFormat[RasterImageFormat["PgmBinary"] = 105] = "PgmBinary";
                RasterImageFormat[RasterImageFormat["PpmAscii"] = 106] = "PpmAscii";
                RasterImageFormat[RasterImageFormat["PpmBinary"] = 107] = "PpmBinary";
                RasterImageFormat[RasterImageFormat["Cut"] = 108] = "Cut";
                RasterImageFormat[RasterImageFormat["Xpm"] = 109] = "Xpm";
                RasterImageFormat[RasterImageFormat["Xbm"] = 110] = "Xbm";
                RasterImageFormat[RasterImageFormat["IffIlbm"] = 111] = "IffIlbm";
                RasterImageFormat[RasterImageFormat["IffCat"] = 112] = "IffCat";
                RasterImageFormat[RasterImageFormat["Xwd"] = 113] = "Xwd";
                RasterImageFormat[RasterImageFormat["Clp"] = 114] = "Clp";
                RasterImageFormat[RasterImageFormat["Jbig"] = 115] = "Jbig";
                RasterImageFormat[RasterImageFormat["Emf"] = 116] = "Emf";
                RasterImageFormat[RasterImageFormat["IcaIbmMmr"] = 117] = "IcaIbmMmr";
                RasterImageFormat[RasterImageFormat["RawIcaIbmMmr"] = 118] = "RawIcaIbmMmr";
                RasterImageFormat[RasterImageFormat["Ani"] = 119] = "Ani";
                RasterImageFormat[RasterImageFormat["LaserData"] = 121] = "LaserData";
                RasterImageFormat[RasterImageFormat["IntergraphRle"] = 122] = "IntergraphRle";
                RasterImageFormat[RasterImageFormat["IntergraphVector"] = 123] = "IntergraphVector";
                RasterImageFormat[RasterImageFormat["Dwg"] = 124] = "Dwg";
                RasterImageFormat[RasterImageFormat["DicomRleGray"] = 125] = "DicomRleGray";
                RasterImageFormat[RasterImageFormat["DicomRleColor"] = 126] = "DicomRleColor";
                RasterImageFormat[RasterImageFormat["DicomJpegGray"] = 127] = "DicomJpegGray";
                RasterImageFormat[RasterImageFormat["DicomJpegColor"] = 128] = "DicomJpegColor";
                RasterImageFormat[RasterImageFormat["Cals4"] = 129] = "Cals4";
                RasterImageFormat[RasterImageFormat["Cals2"] = 130] = "Cals2";
                RasterImageFormat[RasterImageFormat["Cals3"] = 131] = "Cals3";
                RasterImageFormat[RasterImageFormat["Xwd10"] = 132] = "Xwd10";
                RasterImageFormat[RasterImageFormat["Xwd11"] = 133] = "Xwd11";
                RasterImageFormat[RasterImageFormat["Flc"] = 134] = "Flc";
                RasterImageFormat[RasterImageFormat["Kdc"] = 135] = "Kdc";
                RasterImageFormat[RasterImageFormat["Drw"] = 136] = "Drw";
                RasterImageFormat[RasterImageFormat["Plt"] = 137] = "Plt";
                RasterImageFormat[RasterImageFormat["TifCmp"] = 138] = "TifCmp";
                RasterImageFormat[RasterImageFormat["TifJbig"] = 139] = "TifJbig";
                RasterImageFormat[RasterImageFormat["TifDxf"] = 140] = "TifDxf";
                RasterImageFormat[RasterImageFormat["TifUnknown"] = 141] = "TifUnknown";
                RasterImageFormat[RasterImageFormat["Sgi"] = 142] = "Sgi";
                RasterImageFormat[RasterImageFormat["SgiRle"] = 143] = "SgiRle";
                RasterImageFormat[RasterImageFormat["Dwf"] = 145] = "Dwf";
                RasterImageFormat[RasterImageFormat["RasPdf"] = 146] = "RasPdf";
                RasterImageFormat[RasterImageFormat["RasPdfG31Dim"] = 147] = "RasPdfG31Dim";
                RasterImageFormat[RasterImageFormat["RasPdfG32Dim"] = 148] = "RasPdfG32Dim";
                RasterImageFormat[RasterImageFormat["RasPdfG4"] = 149] = "RasPdfG4";
                RasterImageFormat[RasterImageFormat["RasPdfJpeg"] = 150] = "RasPdfJpeg";
                RasterImageFormat[RasterImageFormat["RasPdfJpeg422"] = 151] = "RasPdfJpeg422";
                RasterImageFormat[RasterImageFormat["RasPdfJpeg411"] = 152] = "RasPdfJpeg411";
                RasterImageFormat[RasterImageFormat["Raw"] = 153] = "Raw";
                RasterImageFormat[RasterImageFormat["TifCustom"] = 155] = "TifCustom";
                RasterImageFormat[RasterImageFormat["RawRgb"] = 156] = "RawRgb";
                RasterImageFormat[RasterImageFormat["RawRle4"] = 157] = "RawRle4";
                RasterImageFormat[RasterImageFormat["RawRle8"] = 158] = "RawRle8";
                RasterImageFormat[RasterImageFormat["RawBitfields"] = 159] = "RawBitfields";
                RasterImageFormat[RasterImageFormat["RawPackBits"] = 160] = "RawPackBits";
                RasterImageFormat[RasterImageFormat["RawJpeg"] = 161] = "RawJpeg";
                RasterImageFormat[RasterImageFormat["RawCcitt"] = 162] = "RawCcitt";
                RasterImageFormat[RasterImageFormat["FaxG31DimNoEol"] = 162] = "FaxG31DimNoEol";
                RasterImageFormat[RasterImageFormat["Jp2"] = 163] = "Jp2";
                RasterImageFormat[RasterImageFormat["J2k"] = 164] = "J2k";
                RasterImageFormat[RasterImageFormat["Cmw"] = 165] = "Cmw";
                RasterImageFormat[RasterImageFormat["TifJ2k"] = 166] = "TifJ2k";
                RasterImageFormat[RasterImageFormat["TifCmw"] = 167] = "TifCmw";
                RasterImageFormat[RasterImageFormat["Mrc"] = 168] = "Mrc";
                RasterImageFormat[RasterImageFormat["Gerber"] = 169] = "Gerber";
                RasterImageFormat[RasterImageFormat["Wbmp"] = 170] = "Wbmp";
                RasterImageFormat[RasterImageFormat["JpegLab"] = 171] = "JpegLab";
                RasterImageFormat[RasterImageFormat["JpegLab411"] = 172] = "JpegLab411";
                RasterImageFormat[RasterImageFormat["JpegLab422"] = 173] = "JpegLab422";
                RasterImageFormat[RasterImageFormat["GeoTiff"] = 174] = "GeoTiff";
                RasterImageFormat[RasterImageFormat["TifLead1Bit"] = 175] = "TifLead1Bit";
                RasterImageFormat[RasterImageFormat["TifMrc"] = 177] = "TifMrc";
                RasterImageFormat[RasterImageFormat["RawLzw"] = 178] = "RawLzw";
                RasterImageFormat[RasterImageFormat["RasPdfLzw"] = 179] = "RasPdfLzw";
                RasterImageFormat[RasterImageFormat["TifAbc"] = 180] = "TifAbc";
                RasterImageFormat[RasterImageFormat["Nap"] = 181] = "Nap";
                RasterImageFormat[RasterImageFormat["JpegRgb"] = 182] = "JpegRgb";
                RasterImageFormat[RasterImageFormat["Jbig2"] = 183] = "Jbig2";
                RasterImageFormat[RasterImageFormat["RawIcaAbic"] = 184] = "RawIcaAbic";
                RasterImageFormat[RasterImageFormat["Abic"] = 185] = "Abic";
                RasterImageFormat[RasterImageFormat["TifAbic"] = 186] = "TifAbic";
                RasterImageFormat[RasterImageFormat["TifJbig2"] = 187] = "TifJbig2";
                RasterImageFormat[RasterImageFormat["RasPdfJbig2"] = 188] = "RasPdfJbig2";
                RasterImageFormat[RasterImageFormat["TifZip"] = 189] = "TifZip";
                RasterImageFormat[RasterImageFormat["IcaAbic"] = 190] = "IcaAbic";
                RasterImageFormat[RasterImageFormat["AfpIcaAbic"] = 191] = "AfpIcaAbic";
                RasterImageFormat[RasterImageFormat["Postscript"] = 222] = "Postscript";
                RasterImageFormat[RasterImageFormat["Svg"] = 247] = "Svg";
                RasterImageFormat[RasterImageFormat["Ptoca"] = 249] = "Ptoca";
                RasterImageFormat[RasterImageFormat["Sct"] = 250] = "Sct";
                RasterImageFormat[RasterImageFormat["Pcl"] = 251] = "Pcl";
                RasterImageFormat[RasterImageFormat["Afp"] = 252] = "Afp";
                RasterImageFormat[RasterImageFormat["IcaUncompressed"] = 253] = "IcaUncompressed";
                RasterImageFormat[RasterImageFormat["RawIcaUncompressed"] = 254] = "RawIcaUncompressed";
                RasterImageFormat[RasterImageFormat["Shp"] = 255] = "Shp";
                RasterImageFormat[RasterImageFormat["Smp"] = 256] = "Smp";
                RasterImageFormat[RasterImageFormat["SmpG31Dim"] = 257] = "SmpG31Dim";
                RasterImageFormat[RasterImageFormat["SmpG32Dim"] = 258] = "SmpG32Dim";
                RasterImageFormat[RasterImageFormat["SmpG4"] = 259] = "SmpG4";
                RasterImageFormat[RasterImageFormat["Cmx"] = 261] = "Cmx";
                RasterImageFormat[RasterImageFormat["TgaRle"] = 262] = "TgaRle";
                RasterImageFormat[RasterImageFormat["Kdc120"] = 263] = "Kdc120";
                RasterImageFormat[RasterImageFormat["Kdc40"] = 264] = "Kdc40";
                RasterImageFormat[RasterImageFormat["Kdc50"] = 265] = "Kdc50";
                RasterImageFormat[RasterImageFormat["Dcs"] = 266] = "Dcs";
                RasterImageFormat[RasterImageFormat["TifxJbig"] = 269] = "TifxJbig";
                RasterImageFormat[RasterImageFormat["TifxJbigT43"] = 270] = "TifxJbigT43";
                RasterImageFormat[RasterImageFormat["TifxJbigT43ItuLab"] = 271] = "TifxJbigT43ItuLab";
                RasterImageFormat[RasterImageFormat["TifxJbigT43Gs"] = 272] = "TifxJbigT43Gs";
                RasterImageFormat[RasterImageFormat["TifxFaxG4"] = 273] = "TifxFaxG4";
                RasterImageFormat[RasterImageFormat["TifxFaxG31D"] = 274] = "TifxFaxG31D";
                RasterImageFormat[RasterImageFormat["TifxFaxG32D"] = 275] = "TifxFaxG32D";
                RasterImageFormat[RasterImageFormat["TifxJpeg"] = 276] = "TifxJpeg";
                RasterImageFormat[RasterImageFormat["Ecw"] = 277] = "Ecw";
                RasterImageFormat[RasterImageFormat["RasRle"] = 288] = "RasRle";
                RasterImageFormat[RasterImageFormat["Dxf13"] = 290] = "Dxf13";
                RasterImageFormat[RasterImageFormat["ClpRle"] = 291] = "ClpRle";
                RasterImageFormat[RasterImageFormat["Dcr"] = 292] = "Dcr";
                RasterImageFormat[RasterImageFormat["DicomJ2kGray"] = 293] = "DicomJ2kGray";
                RasterImageFormat[RasterImageFormat["DicomJ2kColor"] = 294] = "DicomJ2kColor";
                RasterImageFormat[RasterImageFormat["Fit"] = 295] = "Fit";
                RasterImageFormat[RasterImageFormat["Crw"] = 296] = "Crw";
                RasterImageFormat[RasterImageFormat["DwfTextAsPolyline"] = 297] = "DwfTextAsPolyline";
                RasterImageFormat[RasterImageFormat["Cin"] = 298] = "Cin";
                RasterImageFormat[RasterImageFormat["EpsPostscript"] = 300] = "EpsPostscript";
                RasterImageFormat[RasterImageFormat["IntergraphCcittG4"] = 301] = "IntergraphCcittG4";
                RasterImageFormat[RasterImageFormat["Sff"] = 302] = "Sff";
                RasterImageFormat[RasterImageFormat["IffIlbmUncompressed"] = 303] = "IffIlbmUncompressed";
                RasterImageFormat[RasterImageFormat["IffCatUncompressed"] = 304] = "IffCatUncompressed";
                RasterImageFormat[RasterImageFormat["RtfRaster"] = 305] = "RtfRaster";
                RasterImageFormat[RasterImageFormat["Wmz"] = 307] = "Wmz";
                RasterImageFormat[RasterImageFormat["AfpIcaG31Dim"] = 309] = "AfpIcaG31Dim";
                RasterImageFormat[RasterImageFormat["AfpIcaG32Dim"] = 310] = "AfpIcaG32Dim";
                RasterImageFormat[RasterImageFormat["AfpIcaG4"] = 311] = "AfpIcaG4";
                RasterImageFormat[RasterImageFormat["AfpIcaUncompressed"] = 312] = "AfpIcaUncompressed";
                RasterImageFormat[RasterImageFormat["AfpIcaIbmMmr"] = 313] = "AfpIcaIbmMmr";
                RasterImageFormat[RasterImageFormat["LeadMrc"] = 314] = "LeadMrc";
                RasterImageFormat[RasterImageFormat["TifLeadMrc"] = 315] = "TifLeadMrc";
                RasterImageFormat[RasterImageFormat["Txt"] = 316] = "Txt";
                RasterImageFormat[RasterImageFormat["PdfLeadMrc"] = 317] = "PdfLeadMrc";
                RasterImageFormat[RasterImageFormat["Hdp"] = 318] = "Hdp";
                RasterImageFormat[RasterImageFormat["HdpGray"] = 319] = "HdpGray";
                RasterImageFormat[RasterImageFormat["HdpCmyk"] = 320] = "HdpCmyk";
                RasterImageFormat[RasterImageFormat["PngIco"] = 321] = "PngIco";
                RasterImageFormat[RasterImageFormat["Xps"] = 322] = "Xps";
                RasterImageFormat[RasterImageFormat["Jpx"] = 323] = "Jpx";
                RasterImageFormat[RasterImageFormat["XpsJpeg"] = 324] = "XpsJpeg";
                RasterImageFormat[RasterImageFormat["XpsJpeg422"] = 325] = "XpsJpeg422";
                RasterImageFormat[RasterImageFormat["XpsJpeg411"] = 326] = "XpsJpeg411";
                RasterImageFormat[RasterImageFormat["Mng"] = 327] = "Mng";
                RasterImageFormat[RasterImageFormat["MngGray"] = 329] = "MngGray";
                RasterImageFormat[RasterImageFormat["MngJng"] = 330] = "MngJng";
                RasterImageFormat[RasterImageFormat["MngJng411"] = 331] = "MngJng411";
                RasterImageFormat[RasterImageFormat["MngJng422"] = 332] = "MngJng422";
                RasterImageFormat[RasterImageFormat["RasPdfCmyk"] = 333] = "RasPdfCmyk";
                RasterImageFormat[RasterImageFormat["RasPdfLzwCmyk"] = 334] = "RasPdfLzwCmyk";
                RasterImageFormat[RasterImageFormat["Mif"] = 335] = "Mif";
                RasterImageFormat[RasterImageFormat["E00"] = 336] = "E00";
                RasterImageFormat[RasterImageFormat["Tdb"] = 337] = "Tdb";
                RasterImageFormat[RasterImageFormat["TdbVista"] = 338] = "TdbVista";
                RasterImageFormat[RasterImageFormat["Snp"] = 339] = "Snp";
                RasterImageFormat[RasterImageFormat["AfpIm1"] = 340] = "AfpIm1";
                RasterImageFormat[RasterImageFormat["Xls"] = 341] = "Xls";
                RasterImageFormat[RasterImageFormat["Doc"] = 342] = "Doc";
                RasterImageFormat[RasterImageFormat["Anz"] = 343] = "Anz";
                RasterImageFormat[RasterImageFormat["Ppt"] = 344] = "Ppt";
                RasterImageFormat[RasterImageFormat["PptJpeg"] = 345] = "PptJpeg";
                RasterImageFormat[RasterImageFormat["PptPng"] = 346] = "PptPng";
                RasterImageFormat[RasterImageFormat["Jpm"] = 347] = "Jpm";
                RasterImageFormat[RasterImageFormat["Vff"] = 348] = "Vff";
                RasterImageFormat[RasterImageFormat["PclXl"] = 349] = "PclXl";
                RasterImageFormat[RasterImageFormat["Docx"] = 350] = "Docx";
                RasterImageFormat[RasterImageFormat["Xlsx"] = 351] = "Xlsx";
                RasterImageFormat[RasterImageFormat["Pptx"] = 352] = "Pptx";
                RasterImageFormat[RasterImageFormat["Jxr"] = 353] = "Jxr";
                RasterImageFormat[RasterImageFormat["JxrGray"] = 354] = "JxrGray";
                RasterImageFormat[RasterImageFormat["JxrCmyk"] = 355] = "JxrCmyk";
                RasterImageFormat[RasterImageFormat["Jls"] = 356] = "Jls";
                RasterImageFormat[RasterImageFormat["Jxr422"] = 357] = "Jxr422";
                RasterImageFormat[RasterImageFormat["Jxr420"] = 358] = "Jxr420";
                RasterImageFormat[RasterImageFormat["DcfArw"] = 359] = "DcfArw";
                RasterImageFormat[RasterImageFormat["DcfRaf"] = 360] = "DcfRaf";
                RasterImageFormat[RasterImageFormat["DcfOrf"] = 361] = "DcfOrf";
                RasterImageFormat[RasterImageFormat["DcfCr2"] = 362] = "DcfCr2";
                RasterImageFormat[RasterImageFormat["DcfNef"] = 363] = "DcfNef";
                RasterImageFormat[RasterImageFormat["DcfRw2"] = 364] = "DcfRw2";
                RasterImageFormat[RasterImageFormat["DcfCasio"] = 365] = "DcfCasio";
                RasterImageFormat[RasterImageFormat["DcfPentax"] = 366] = "DcfPentax";
                RasterImageFormat[RasterImageFormat["JlsLine"] = 367] = "JlsLine";
                RasterImageFormat[RasterImageFormat["JlsSample"] = 368] = "JlsSample";
                RasterImageFormat[RasterImageFormat["Htm"] = 369] = "Htm";
                RasterImageFormat[RasterImageFormat["Mob"] = 370] = "Mob";
                RasterImageFormat[RasterImageFormat["Pub"] = 371] = "Pub";
                RasterImageFormat[RasterImageFormat["Ing"] = 372] = "Ing";
                RasterImageFormat[RasterImageFormat["IngRle"] = 373] = "IngRle";
                RasterImageFormat[RasterImageFormat["IngAdaptiveRle"] = 374] = "IngAdaptiveRle";
                RasterImageFormat[RasterImageFormat["IngG4"] = 375] = "IngG4";
                RasterImageFormat[RasterImageFormat["Dwfx"] = 376] = "Dwfx";
                RasterImageFormat[RasterImageFormat["IcaJpeg"] = 377] = "IcaJpeg";
                RasterImageFormat[RasterImageFormat["IcaJpeg411"] = 378] = "IcaJpeg411";
                RasterImageFormat[RasterImageFormat["IcaJpeg422"] = 379] = "IcaJpeg422";
                RasterImageFormat[RasterImageFormat["DcfDng"] = 380] = "DcfDng";
                RasterImageFormat[RasterImageFormat["RawFlate"] = 381] = "RawFlate";
                RasterImageFormat[RasterImageFormat["RawRle"] = 382] = "RawRle";
                RasterImageFormat[RasterImageFormat["DicomJlsGray"] = 383] = "DicomJlsGray";
                RasterImageFormat[RasterImageFormat["DicomJlsColor"] = 384] = "DicomJlsColor";
                RasterImageFormat[RasterImageFormat["Pst"] = 385] = "Pst";
                RasterImageFormat[RasterImageFormat["Msg"] = 386] = "Msg";
                RasterImageFormat[RasterImageFormat["Eml"] = 387] = "Eml";
                RasterImageFormat[RasterImageFormat["RasPdfJpx"] = 388] = "RasPdfJpx";
                RasterImageFormat[RasterImageFormat["DicomJpxGray"] = 389] = "DicomJpxGray";
                RasterImageFormat[RasterImageFormat["DicomJpxColor"] = 390] = "DicomJpxColor";
                RasterImageFormat[RasterImageFormat["JpegCmyk"] = 391] = "JpegCmyk";
                RasterImageFormat[RasterImageFormat["JpegCmyk411"] = 392] = "JpegCmyk411";
                RasterImageFormat[RasterImageFormat["JpegCmyk422"] = 393] = "JpegCmyk422";
                RasterImageFormat[RasterImageFormat["TifJpegCmyk"] = 394] = "TifJpegCmyk";
                RasterImageFormat[RasterImageFormat["TifJpegCmyk411"] = 395] = "TifJpegCmyk411";
                RasterImageFormat[RasterImageFormat["TifJpegCmyk422"] = 396] = "TifJpegCmyk422";
                RasterImageFormat[RasterImageFormat["Svgz"] = 397] = "Svgz";
            })(JavaScript.RasterImageFormat || (JavaScript.RasterImageFormat = {}));
            var RasterImageFormat = JavaScript.RasterImageFormat;
            (function (TwainVersion) {
                TwainVersion[TwainVersion["Version1"] = 1] = "Version1";
                TwainVersion[TwainVersion["Version2"] = 2] = "Version2";
            })(JavaScript.TwainVersion || (JavaScript.TwainVersion = {}));
            var TwainVersion = JavaScript.TwainVersion;
        })(JavaScript = Twain.JavaScript || (Twain.JavaScript = {}));
    })(Twain = lt.Twain || (lt.Twain = {}));
})(lt || (lt = {}));
var lt;
(function (lt) {
    var Scanning;
    (function (Scanning) {
        var JavaScript;
        (function (JavaScript) {
            var ScanningStatus = (function () {
                function ScanningStatus() {
                }
                return ScanningStatus;
            }());
            JavaScript.ScanningStatus = ScanningStatus;
        })(JavaScript = Scanning.JavaScript || (Scanning.JavaScript = {}));
    })(Scanning = lt.Scanning || (lt.Scanning = {}));
})(lt || (lt = {}));
var lt;
(function (lt) {
    var Sane;
    (function (Sane) {
        var JavaScript;
        (function (JavaScript) {
            var SaneError = (function () {
                function SaneError() {
                }
                return SaneError;
            }());
            JavaScript.SaneError = SaneError;
            var SaneStatus = (function () {
                function SaneStatus() {
                }
                return SaneStatus;
            }());
            JavaScript.SaneStatus = SaneStatus;
            var SaneSourceInformation = (function () {
                function SaneSourceInformation() {
                }
                return SaneSourceInformation;
            }());
            JavaScript.SaneSourceInformation = SaneSourceInformation;
            var SaneService = (function () {
                function SaneService(portNumber) {
                    this._serviceUrl = "http://127.0.0.1:{0}/".format(portNumber);
                    this._id = "";
                }
                SaneService.prototype.init = function (userData, onSuccess, onFailure) {
                    var _this = this;
                    var data = {
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Init".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (sessionId) {
                        _this._id = sessionId;
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.getStatus = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}GetStatus".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (saneStatus) {
                        onSuccess(saneStatus);
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.getSources = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}GetSources".format(this._serviceUrl),
                        'type': "POST",
                        data: JSON.stringify(data),
                        contentType: "application/json"
                    })
                        .done(function (sources) {
                        onSuccess(sources);
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.selectSource = function (sourceName, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        sourceName: sourceName,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}SelectSource".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.acquire = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Acquire".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.getPage = function (pageNumber, format, bpp, width, height, userData) {
                    return "{0}GetPage?id={1}&pageNumber={2}&format={3}&bpp={4}&width={5}&height={6}&userData={7}&time={8}".format(this._serviceUrl, this._id, pageNumber, format, bpp, width, height, userData, $.now());
                };
                SaneService.prototype.stop = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Stop".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data),
                        async: false
                    })
                        .done(function () {
                        if (onSuccess != null)
                            onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        if (onFailure != null)
                            onFailure(errorThrown);
                    });
                };
                SaneService.prototype.start = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Start".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.applyImageProcessingCommand = function (firstPageNumber, lastPageNumber, commandName, args, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        firstPageNumber: firstPageNumber,
                        lastPageNumber: lastPageNumber,
                        commandName: commandName,
                        arguments: JSON.stringify(args),
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Run".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.getImageProcessingPreview = function (pageNumber, commandName, args, width, height, format, bpp, userData) {
                    return "{0}Preview?id={1}&pageNumber={2}&commandName={3}&arguments={4}&width={5}&height={6}&format={7}&bpp={8}&userData={9}&time={10}".format(this._serviceUrl, this._id, pageNumber, commandName, JSON.stringify(args), width, height, format, bpp, userData, $.now());
                };
                SaneService.prototype.abortAcquire = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}AbortAcquire".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.setOptionValue = function (optionName, value, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        optionName: optionName,
                        value: value,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}setOptionValue".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.getOptionValue = function (optionName, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        optionName: optionName,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}getOptionValue".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (value) {
                        onSuccess(value);
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.deletePage = function (pageNumber, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        pageNumber: pageNumber,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}DeletePage".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                SaneService.prototype.runCommand = function (commandName, args, userData) {
                    return "{0}RunCommand?id={1}&commandName={2}&arguments={3}&userData={9}".format(this._serviceUrl, this._id, commandName, JSON.stringify(args), userData);
                };
                return SaneService;
            }());
            JavaScript.SaneService = SaneService;
        })(JavaScript = Sane.JavaScript || (Sane.JavaScript = {}));
    })(Sane = lt.Sane || (lt.Sane = {}));
})(lt || (lt = {}));
String.prototype.format = function () {
    var formatted = this;
    for (var i = 0; i < arguments.length; i++) {
        var regexp = new RegExp('\\{' + i + '\\}', 'gi');
        formatted = formatted.replace(regexp, arguments[i]);
    }
    return formatted;
};
var lt;
(function (lt) {
    var Twain;
    (function (Twain) {
        var JavaScript;
        (function (JavaScript) {
            var TwainError = (function () {
                function TwainError() {
                }
                return TwainError;
            }());
            JavaScript.TwainError = TwainError;
            var TwainStatus = (function () {
                function TwainStatus() {
                }
                return TwainStatus;
            }());
            JavaScript.TwainStatus = TwainStatus;
            var TwainSourceInformation = (function () {
                function TwainSourceInformation() {
                }
                return TwainSourceInformation;
            }());
            JavaScript.TwainSourceInformation = TwainSourceInformation;
            var TwainArrayCapability = (function () {
                function TwainArrayCapability() {
                }
                return TwainArrayCapability;
            }());
            JavaScript.TwainArrayCapability = TwainArrayCapability;
            var TwainEnumerationCapability = (function () {
                function TwainEnumerationCapability() {
                }
                return TwainEnumerationCapability;
            }());
            JavaScript.TwainEnumerationCapability = TwainEnumerationCapability;
            var TwainCapabilityBase = (function () {
                function TwainCapabilityBase() {
                }
                return TwainCapabilityBase;
            }());
            JavaScript.TwainCapabilityBase = TwainCapabilityBase;
            var TwainOneValueCapability = (function () {
                function TwainOneValueCapability() {
                }
                return TwainOneValueCapability;
            }());
            JavaScript.TwainOneValueCapability = TwainOneValueCapability;
            var TwainRangeCapability = (function () {
                function TwainRangeCapability() {
                }
                return TwainRangeCapability;
            }());
            JavaScript.TwainRangeCapability = TwainRangeCapability;
            var TwainCapability = (function () {
                function TwainCapability() {
                }
                return TwainCapability;
            }());
            JavaScript.TwainCapability = TwainCapability;
            var TwainService = (function () {
                function TwainService(url) {
                    this._serviceUrl = url;
                    this._id = "";
                }
                TwainService.prototype.init = function (manufacturer, productFamily, version, application, userData, onSuccess, onFailure) {
                    var _this = this;
                    var data = {
                        manufacturer: manufacturer,
                        productFamily: productFamily,
                        version: version,
                        application: application,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Init".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (sessionId) {
                        _this._id = sessionId;
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.getStatus = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}GetStatus".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (twainStatus) {
                        onSuccess(twainStatus);
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.isAvailable = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}IsAvailable".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (available) {
                        onSuccess(available);
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.getSources = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}QuerySourceInformation".format(this._serviceUrl),
                        'type': "POST",
                        data: JSON.stringify(data),
                        contentType: "application/json"
                    })
                        .done(function (sources) {
                        onSuccess(sources);
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.selectSource = function (sourceName, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        sourceName: sourceName,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}SelectSource".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.acquire = function (flags, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        flags: flags,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Acquire".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.getPage = function (pageNumber, format, bpp, width, height, userData) {
                    return "{0}GetPage?id={1}&pageNumber={2}&format={3}&bpp={4}&width={5}&height={6}&userData={7}&time={8}".format(this._serviceUrl, this._id, pageNumber, format, bpp, width, height, userData, $.now());
                };
                TwainService.prototype.stop = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Stop".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data),
                        async: false
                    })
                        .done(function () {
                        if (onSuccess != null)
                            onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        if (onFailure != null)
                            onFailure(errorThrown);
                    });
                };
                TwainService.prototype.start = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Start".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.applyImageProcessingCommand = function (firstPageNumber, lastPageNumber, commandName, args, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        firstPageNumber: firstPageNumber,
                        lastPageNumber: lastPageNumber,
                        commandName: commandName,
                        arguments: JSON.stringify(args),
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}Run".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.getImageProcessingPreview = function (pageNumber, commandName, args, width, height, format, bpp, userData) {
                    return "{0}Preview?id={1}&pageNumber={2}&commandName={3}&arguments={4}&width={5}&height={6}&format={7}&bpp={8}&userData={9}&time={10}".format(this._serviceUrl, this._id, pageNumber, commandName, JSON.stringify(args), width, height, format, bpp, userData, $.now());
                };
                TwainService.prototype.abortAcquire = function (userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}AbortAcquire".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.setCapabilityValue = function (capabilityType, itemType, value, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        capabilityType: capabilityType,
                        itemType: itemType,
                        value: value,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}SetCapabilityValue".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.getCapability = function (capabilityType, mode, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        capabilityType: capabilityType,
                        mode: mode,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}GetCapability".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (cap) {
                        onSuccess(JSON.parse(cap));
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.deletePage = function (pageNumber, userData, onSuccess, onFailure) {
                    var data = {
                        id: this._id,
                        pageNumber: pageNumber,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}DeletePage".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function () {
                        onSuccess();
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                TwainService.prototype.runCommand = function (commandName, args, userData) {
                    return "{0}RunCommand?id={1}&commandName={2}&arguments={3}&userData={9}".format(this._serviceUrl, this._id, commandName, JSON.stringify(args), userData);
                };
                TwainService.prototype.setVersion = function (version, userData, onSuccess, onFailure) {
                    var data = {
                        version: version,
                        userData: userData
                    };
                    $.ajax({
                        url: "{0}SetVersion".format(this._serviceUrl),
                        'type': "POST",
                        contentType: "application/json",
                        data: JSON.stringify(data)
                    })
                        .done(function (status) {
                        onSuccess(status);
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                        onFailure(errorThrown);
                    });
                };
                return TwainService;
            }());
            JavaScript.TwainService = TwainService;
        })(JavaScript = Twain.JavaScript || (Twain.JavaScript = {}));
    })(Twain = lt.Twain || (lt.Twain = {}));
})(lt || (lt = {}));
// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.IO;
using System.Web;
using System.Reflection;
using System.Net.Http;
using System.Web.Hosting;
using System.Linq;

using Leadtools.Codecs;
using Leadtools.Caching;
using Leadtools.Barcode;
using Leadtools.Document;

using Leadtools.Ocr;
using System.Net.Sockets;
using System.Diagnostics;
using Leadtools.Annotations.Engine;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Leadtools.Services.Tools.Helpers
{
   public enum ServiceVendor
   {
      Dropbox = 0,
      GoogleDrive = 1,
      OneDrive = 2,
      SharePoint = 3
   }
   public enum OcrEngineStatus
   {
      /// <summary>
      ///   The OCR Engine was not set, and thus is not being used.
      /// </summary>
      Unset = 0,

      /// <summary>
      ///   An error occurred with the setup.
      /// </summary>
      Error = 1,

      /// <summary>
      ///   The OCR Engine should be working normally.
      /// </summary>
      Ready = 2
   }
   internal static class ServiceHelper
   {
      public const string Key_CORS_Origins = "CORS.Origins";
      public const string Key_CORS_Headers = "CORS.Headers";
      public const string Key_CORS_Methods = "CORS.Methods";
      public const string Key_CORS_MaxAge = "CORS.MaxAge";

      public const string Key_Access_Passcode = "Access.Passcode";

      public const string Key_Application_DrawEngineType = "lt.Application.DrawEngineType";
      public const string Key_Application_ShadowFontMode = "lt.Application.ShadowFontMode";
      public const string Key_Application_ShadowFontsDirectory = "lt.Application.ShadowFontsDirectory";

      public const string Key_Application_AllowTempFilesFromDisk = "lt.Application.AllowTempFilesFromDisk";
      public const string Key_Application_TempDirectory = "lt.Application.TempDirectory";

      public const string Key_Application_ReturnRequestUserData = "lt.Application.ReturnRequestUserData";

      public const string Key_License_FilePath = "lt.License.FilePath";
      public const string Key_License_DeveloperKey = "lt.License.DeveloperKey";

      public const string Key_Cache_ConfigFile = "lt.Cache.ConfigFile";
      public const string Key_Cache_SlidingExpiration = "lt.Cache.SlidingExpiration";

      public const string Key_PreCache_DictionaryXml = "lt.PreCache.DictionaryXml";
      public const string Default_PreCache_DictionaryXml = @".\App_Data\PreCacheDictionary.xml";

      public const string Key_PreCache_Directory = "lt.PreCache.Directory";
      public const string Default_PreCache_Directory = @".\precache";

      public const string Key_Document_MimeTypesFile = "lt.Document.MimeTypesFile";
      public const string Key_Document_OnlyAllowedMimeTypes = "lt.Document.OnlyAllowedMimeTypes";
      public const string Key_Document_AutoUpdateHistory = "lt.Document.AutoUpdateHistory";

      public const string Key_RasterCodecs_HtmlDomainWhitelistFile = "lt.RasterCodecs.HtmlDomainWhitelistFile";
      public const string Key_RasterCodecs_OptionsFilePath = "lt.RasterCodecs.OptionsFilePath";

      public const string Key_Barcodes_Reader_OptionsFilePath = "lt.Barcodes.Reader.OptionsFilePath";

      public const string Key_Ocr_EngineType = "lt.Ocr.EngineType";
      public const string Key_Ocr_RuntimeDirectory = "lt.Ocr.RuntimeDirectory";

      public const string Key_DocumentConverter_UseExternal = "lt.DocumentConverter.UseExternal";
      public const string Key_DocumentConverter_ExePath = "lt.DocumentConverter.ExePath";
      public const string Key_DocumentConverter_ForceStreaming = "lt.DocumentConverter.ForceStreaming";
      public const string Key_DocumentConverter_MaximumPages = "lt.DocumentConverter.MaximumPages";
      public const string Key_DocumentConverter_SavePDFA = "lt.DocumentConverter.SavePDFA";

      public const string Key_Svg_GZip = "lt.Svg.GZip";

      public static string CORSOrigins { get; private set; }
      public static string CORSHeaders { get; private set; }
      public static string CORSMethods { get; private set; }
      public static long CORSMaxAge { get; private set; }
      static ServiceHelper()
      {
         // Try to set our CORS options from local.config.
         // If nothing's there, use "*" (all) for each of them
         CORSOrigins = GetSettingValue(Key_CORS_Origins);
         if (string.IsNullOrWhiteSpace(CORSOrigins))
            CORSOrigins = "*";
         CORSHeaders = GetSettingValue(Key_CORS_Headers);
         if (string.IsNullOrWhiteSpace(CORSHeaders))
            CORSHeaders = "*";
         CORSMethods = GetSettingValue(Key_CORS_Methods);
         if (string.IsNullOrWhiteSpace(CORSMethods))
            CORSMethods = "*";
         CORSMaxAge = GetSettingInteger(Key_CORS_MaxAge, -1);
      }

      public static string GetSettingValue(string key)
      {
         var currentContext = HttpContext.Current;
         string value = null;
         if (currentContext != null)
         {
            if (ConfigurationManager.AppSettings[key] != null)
               value = ConfigurationManager.AppSettings[key];
         }
         if (value == null)
         {
            // Load it from the config file
            var config = ConfigurationManager.OpenExeConfiguration(typeof(ServiceHelper).Assembly.Location);
            if (config != null)
            {
               // Get the appSettings section
               var appSettings = (AppSettingsSection)config.GetSection("appSettings");
               if (appSettings != null)
               {
                  if (appSettings.Settings[key] != null)
                     value = appSettings.Settings[key].Value;
                  else
                     value = string.Empty;
               }
            }
         }

         return value;
      }

      public static bool GetSettingBoolean(string key)
      {
         var stringVal = ServiceHelper.GetSettingValue(key);
         bool temp = false;
         if (bool.TryParse(stringVal, out temp))
            return temp;
         return false;
      }

      public static int GetSettingInteger(string key, int defaultValue)
      {
         var stringVal = ServiceHelper.GetSettingValue(key);
         int temp = defaultValue;
         if (int.TryParse(stringVal, out temp))
            return temp;
         return defaultValue;
      }

      public static bool IsLicenseChecked
      {
         get;
         set;
      }

      public static bool IsKernelExpired
      {
         get;
         set;
      }

      // If true, we will track changes to the document or annotations
      private static bool _autoUpdateHistory = false;
      public static bool AutoUpdateHistory
      {
         get { return _autoUpdateHistory; }
      }

      public static void InitializeService()
      {
         /* This method is called by Application_Start of the web service
          * We will initialize the global and static objects used through out the demos and
          * Each controller will be able to use these same objects.
          * Controller-specific initialization is performed in InitializeController
          */

         // Set the license, initialize the cache and various objects

         // For the license, the TestController.Ping method is used to check the status of this
         // So save the values here to get them later
         try
         {
            SetLicense();
            IsKernelExpired = RasterSupport.KernelExpired;
         }
         catch
         {
            IsKernelExpired = true;
         }

         if (!IsKernelExpired)
         {
            // The license is OK, continue
            try
            {
               // This setting disables disk access when creating temp files
               if (!GetSettingBoolean(Key_Application_AllowTempFilesFromDisk))
                  RasterDefaults.TempFileMode = LeadTempFileMode.Memory;

               string tempDirectory = GetSettingValue(ServiceHelper.Key_Application_TempDirectory);
               if (!string.IsNullOrEmpty(tempDirectory))
               {
                  tempDirectory = ServiceHelper.GetAbsolutePath(tempDirectory);
                  RasterDefaults.TemporaryDirectory = tempDirectory;
               }

               SetMultiplatformSupport();

               CreateCache();
               PreCacheHelper.CreatePreCache();
               SetRasterCodecsOptions(DocumentFactory.RasterCodecsTemplate, 0);

               LoadMimeTypesWhitelist();

               if (GetSettingBoolean(Key_Document_OnlyAllowedMimeTypes))
               {
                  /*
                   * If true, all unspecified mimeTypes are automatically considered "denied".
                   * This effectively means that only mimeTypes in the "allowed" list are accepted.
                   */
                  DocumentFactory.MimeTypes.DefaultStatus = DocumentMimeTypeStatus.Denied;
               }

               _autoUpdateHistory = GetSettingBoolean(Key_Document_AutoUpdateHistory);
               _returnRequestUserData = GetSettingBoolean(Key_Application_ReturnRequestUserData);
            }
            catch
            {
               // Let this pass, it is checked again in TestController.Ping
            }

            CreateOCREngine();
            CreateAnnRenderingEngine();
         }
      }

      public static void CleanupService()
      {
         /* This method is called by Application_End of the web service
          * We will clean up and destroy the global static objects created in
          * InitializeService
          */
         if (_ocrEngine != null)
         {
            _ocrEngine.Dispose();
            _ocrEngine = null;
         }
      }

      public static void InitializeController()
      {
         /* This method is called by the constructor of each controller
          * It is assumed that InitializeService has been called before by
          * Application_Start of the web service
          * 
          * Do any per-request setup here
          */
      }

      private static bool _returnRequestUserData = false;
      public static bool ReturnRequestUserData
      {
         get { return _returnRequestUserData; }
      }

      private static string _multiplatformSupportStatus = "Not Ready";
      public static string MultiplatformSupportStatus
      {
         get { return _multiplatformSupportStatus; }
      }

      public static void SetMultiplatformSupport()
      {
         try
         {
            // Set the optional multi-platform support
            // Refer to https://www.leadtools.com/help/leadtools/v20/dh/to/leadtools-drawing-engine-and-multi-platform-consideration.html

            // Get the current options
            DrawEngineOptions options = DrawEngine.GetOptions();

            var drawEngineTypeString = ServiceHelper.GetSettingValue(ServiceHelper.Key_Application_DrawEngineType);
            if (!string.IsNullOrEmpty(drawEngineTypeString))
            {
               options.EngineType = (DrawEngineType)Enum.Parse(typeof(DrawEngineType), drawEngineTypeString, true);
            }

            var shadowFontModeString = ServiceHelper.GetSettingValue(ServiceHelper.Key_Application_ShadowFontMode);
            if (!string.IsNullOrEmpty(shadowFontModeString))
            {
               options.ShadowFontMode = (DrawShadowFontMode)Enum.Parse(typeof(DrawShadowFontMode), shadowFontModeString, true);
            }

            DrawEngine.SetOptions(options);

            // Set the shadow fonts directory
            string shadowFontsDirectory = GetSettingValue(ServiceHelper.Key_Application_ShadowFontsDirectory);
            shadowFontsDirectory = ServiceHelper.GetAbsolutePath(shadowFontsDirectory);
            if (!string.IsNullOrEmpty(shadowFontsDirectory))
            {
               if (Directory.Exists(shadowFontsDirectory))
               {
                  // Set the shadow fonts
                  RasterDefaults.SetResourceDirectory(LEADResourceDirectory.Fonts, shadowFontsDirectory);
               }
               else
               {
                  throw new InvalidOperationException(string.Format("Unable to set shadow fonts because the file {0} does not exist or is not a directory.", shadowFontsDirectory));
               }
            }

            _multiplatformSupportStatus = "Ready";
         }
         catch
         {
            _multiplatformSupportStatus = "Error";
            throw;
         }
      }

      public static void SetLicense()
      {
         /*
          * Set the license and license key here.
          * While this is called with each call to the service,
          * the lines below with RasterSupport.KernelExpired
          * will exit early to avoid checking repeatedly.
          */

         if (!RasterSupport.KernelExpired)
         {
            IsLicenseChecked = true;
            return;
         }

         // file path may be relative or absolute
         // dev key may be relative, absolute, or the full text
         string licensePath = null;
         string devKey = null;

         // First check the config file (appSettings)
         var licSetting = GetSettingValue(Key_License_FilePath);
         if (!string.IsNullOrEmpty(licSetting))
         {
            // If settings are wrong from here on out, we need to throw an exception
            licSetting = GetAbsolutePath(licSetting);
            if (File.Exists(licSetting))
            {
               licensePath = licSetting;

               // devKey can be relative, absolute, or file contents
               var devKeySetting = GetSettingValue(Key_License_DeveloperKey);
               if (devKeySetting != null)
                  devKeySetting = devKeySetting.Trim();

               if (devKeySetting != null && IsAbsolutePath(devKeySetting) && File.Exists(devKeySetting))
               {
                  var devKeyFile = devKeySetting;
                  devKey = File.ReadAllText(devKeyFile);
               }
               else if (devKeySetting != null)
               {
                  // Could be a relative path or a developer key, see if the file exist
                  var devKeyFile = GetAbsolutePath(devKeySetting);
                  if (File.Exists(devKeyFile))
                     devKey = File.ReadAllText(devKeyFile);
                  else
                     devKey = devKeySetting;
               }
               else
               {
                  throw new InvalidOperationException("Developer key in configuration was invalid.");
               }
            }
            else
            {
               throw new InvalidOperationException("License file path in configuration does not exist.");
            }
         }
         else
         {
            // Was not found there, check the bin folder
            var currentContext = HttpContext.Current;
            if (currentContext != null && currentContext.Server != null)
            {
               string licBinPath = currentContext.Server.MapPath(@"~/bin/LEADTOOLS.LIC");
               if (File.Exists(licBinPath))
               {
                  licensePath = licBinPath;
                  // get value for devKey, process to get file contents
                  string devKeyBinPath = currentContext.Server.MapPath(@"~/bin/LEADTOOLS.LIC.key");
                  if (File.Exists(devKeyBinPath))
                     devKey = File.ReadAllText(devKeyBinPath);
               }
            }
         }

         if (!string.IsNullOrEmpty(licensePath) && !string.IsNullOrEmpty(devKey))
         {
            IsLicenseChecked = true;
            RasterSupport.SetLicense(licensePath, devKey);
         }
      }

      public static void CheckCacheAccess()
      {
         // Check if the cache directory setup by the user in the config file is valid and accessible

         // Do this by loading up the cache, adding a region, an item and deleting it
         // This mimics what the documents library will do
         if (_cache == null)
            throw new InvalidOperationException("Cache has not been setup");

         // If this is the default FileCache then try to add/remove an item
         // This check can be performed with all caches but is extra important with
         // FileCache since forgetting to setup the correct read/write access
         // on the cache directory is a common issue when setting up the service
         if (_cache is FileCache)
         {
            var regionName = Guid.NewGuid().ToString("N");
            var policy = CreatePolicy();
            var key = "key";
            _cache.Add(key, "data", policy, regionName);
            // Verify
            var data = _cache.Get(key, regionName) as string;
            if (data == null || string.CompareOrdinal(data, "data") != 0)
               throw new InvalidOperationException("Could not read cache item");

            // Delete
            _cache.DeleteItem(key, regionName);
         }
      }

      public static string GetAbsolutePath(string relativePath)
      {
         if (string.IsNullOrEmpty(relativePath) || relativePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
         {
            // Not a legal path
            return relativePath;
         }

         relativePath = relativePath.Trim();
         if (!Path.IsPathRooted(relativePath))
            relativePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, relativePath);
         return relativePath;
      }

      public static bool IsAbsolutePath(string relativePath)
      {
         if (string.IsNullOrEmpty(relativePath) || relativePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
         {
            // Not a legal path
            return false;
         }

         return Path.IsPathRooted(relativePath.Trim());
      }

      public static void CreateCache()
      {
         // Called by InitializeService the first time the service is run
         // Initialize the global Cache object

         var cacheConfigFile = GetSettingValue(Key_Cache_ConfigFile);
         cacheConfigFile = GetAbsolutePath(cacheConfigFile);
         if (string.IsNullOrEmpty(cacheConfigFile))
            throw new InvalidOperationException("Set the cache config file location in '" + Key_Cache_ConfigFile + "' in the configuration file");

         ObjectCache objectCache = null;

         // Set the base directory of the cache (for resolving any relative paths) to this project's path
         var additional = new Dictionary<string, string>();
         additional.Add(ObjectCache.BASE_DIRECTORY_KEY, HostingEnvironment.ApplicationPhysicalPath);

         try
         {
            using (var cacheConfigStream = File.OpenRead(cacheConfigFile))
               objectCache = ObjectCache.CreateFromConfigurations(cacheConfigStream, additional);
         }
         catch (Exception ex)
         {
            throw new InvalidOperationException(string.Format("Cannot load cache configuration from '{0}'", cacheConfigFile), ex);
         }

         _cache = objectCache;
      }

      public static void UpdateCacheSettings(HttpResponseMessage response)
      {
         TimeSpan slidingExpiration;
         var value = GetSettingValue(Key_Cache_SlidingExpiration);
         if (value != null)
            value = value.Trim();
         if (string.IsNullOrEmpty(value) || !TimeSpan.TryParse(value, out slidingExpiration))
            slidingExpiration = TimeSpan.FromMinutes(60);
         response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
         {
            Public = true,
            MaxAge = slidingExpiration
         };
      }

      public static CacheItemPolicy CreatePolicy()
      {
         var policy = new CacheItemPolicy();

         TimeSpan slidingExpiration;
         var value = GetSettingValue(Key_Cache_SlidingExpiration);
         if (value != null)
            value = value.Trim();
         if (!string.IsNullOrEmpty(value) && TimeSpan.TryParse(value, out slidingExpiration))
            policy.SlidingExpiration = slidingExpiration;

         return policy;
      }

      public static CacheItemPolicy CreateForeverPolicy()
      {
         // Creates a 3-year policy (for pre-cached items)
         var policy = new CacheItemPolicy();
         policy.AbsoluteExpiration = DateTime.MaxValue;
         return policy;
      }

      // LEADTOOLS Documents library uses 300 as the default DPI, so we use the same
      public const int DefaultResolution = 300;

      public static void SetRasterCodecsOptions(RasterCodecs rasterCodecs, int resolution)
      {
         // Set up any extra options to use here
         if (resolution == 0)
            resolution = DefaultResolution;

         // Set the load resolution
         rasterCodecs.Options.Wmf.Load.XResolution = resolution;
         rasterCodecs.Options.Wmf.Load.YResolution = resolution;
         rasterCodecs.Options.RasterizeDocument.Load.XResolution = resolution;
         rasterCodecs.Options.RasterizeDocument.Load.YResolution = resolution;

         if (GetSettingBoolean(Key_DocumentConverter_SavePDFA))
         {
            CodecsPdfSaveOptions pdfSaveOptions = rasterCodecs.Options.Pdf.Save;
            pdfSaveOptions.Version = CodecsRasterPdfVersion.PdfA;
         }

         String domainWhitelist = LoadHtmlDomainWhitelist();
         if (!string.IsNullOrEmpty(domainWhitelist))
         {
            CodecsHtmlLoadOptions htmlLoadOptions = rasterCodecs.Options.Html.Load;
            htmlLoadOptions.DomainWhitelist = domainWhitelist;
         }

         // See if we have an options file in the config
         var value = GetSettingValue(Key_RasterCodecs_OptionsFilePath);
         value = GetAbsolutePath(value);
         if (!string.IsNullOrEmpty(value))
            rasterCodecs.LoadOptions(value);

         /* In Web API, resources are pulled from a Temp folder.
          * So rastercodecs needs to be given an initial path that corresponds
          * to the /bin folder.
          * There is an after-build target that copies these files from the proper /bin20 folder.
          */
         var binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);
         if (Directory.Exists(binPath) && File.Exists(Path.Combine(binPath, @"Leadtools.Pdf.Utilities.dll")))
         {
            rasterCodecs.Options.Pdf.InitialPath = binPath;
         }
      }

      public static void LoadMimeTypesWhitelist()
      {
         string mimeTypesFileName = GetSettingValue(Key_Document_MimeTypesFile);
         mimeTypesFileName = GetAbsolutePath(mimeTypesFileName);
         if (string.IsNullOrEmpty(mimeTypesFileName))
            return;

         MimeTypesConfig mimeTypesConfig = null;

         try
         {
            using (StreamReader sr = new StreamReader(mimeTypesFileName))
               mimeTypesConfig = JsonConvert.DeserializeObject<MimeTypesConfig>(sr.ReadToEnd());
         }
         catch (Exception e)
         {
            Trace.WriteLine(string.Format("MimeTypes list not loaded: {0}", e.Message));
            return;
         }

         if (mimeTypesConfig == null)
            return;
         var entries = DocumentFactory.MimeTypes.Entries;

         string[] allowed = mimeTypesConfig.Allowed;
         if (allowed != null && allowed.Length > 0)
         {
            foreach (string allowedMimeType in allowed)
               entries.Add(allowedMimeType, DocumentMimeTypeStatus.Allowed);
         }

         string[] denied = mimeTypesConfig.Denied;
         if (denied != null && denied.Length > 0)
         {
            foreach (string allowedMimeType in denied)
               entries.Add(allowedMimeType, DocumentMimeTypeStatus.Denied);
         }
      }

      public static string LoadHtmlDomainWhitelist()
      {
         string domainWhitelistFileName = GetSettingValue(Key_RasterCodecs_HtmlDomainWhitelistFile);
         domainWhitelistFileName = GetAbsolutePath(domainWhitelistFileName);
         if (string.IsNullOrEmpty(domainWhitelistFileName))
            return null;

         HtmlDomainWhitelistConfig domainWhitelistConfig = null;

         try
         {
            using (StreamReader sr = new StreamReader(domainWhitelistFileName))
               domainWhitelistConfig = JsonConvert.DeserializeObject<HtmlDomainWhitelistConfig>(sr.ReadToEnd());
         }
         catch (Exception e)
         {
            Trace.WriteLine(string.Format("Html Domain Whitelist not loaded: {0}", e.Message));
            return null;
         }

         if (domainWhitelistConfig == null)
            return null;

         string config = "";
         string[] whitelisted = domainWhitelistConfig.Whitelisted;
         if (whitelisted != null && whitelisted.Length > 0)
         {
            for (int i = 0; i < whitelisted.Length; i++)
            {
               config += whitelisted[i];
               if (i != whitelisted.Length - 1)
                  config += "|";
            }
         }
         return config;
      }

      public static bool SetBarcodeReadOptions(BarcodeReader reader)
      {
         // See if we have an options file in the config
         var value = GetSettingValue(Key_Barcodes_Reader_OptionsFilePath);
         value = GetAbsolutePath(value);
         if (string.IsNullOrEmpty(value))
         {
            // Return false to indicate that the user did not set any barcode options.
            // We will try different options ourselves.
            return false;
         }

         reader.LoadOptions(value);
         return true;
      }

      public static void InitBarcodeReader(BarcodeReader reader, bool doublePass)
      {
         // Default options to read most barcodes
         reader.ImageType = BarcodeImageType.Unknown;

         // Both directions for 1D
         OneDBarcodeReadOptions oneDOptions = reader.GetDefaultOptions(BarcodeSymbology.UPCA) as OneDBarcodeReadOptions;
         oneDOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

         GS1DatabarStackedBarcodeReadOptions gs1Options = reader.GetDefaultOptions(BarcodeSymbology.GS1DatabarStacked) as GS1DatabarStackedBarcodeReadOptions;
         gs1Options.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

         FourStateBarcodeReadOptions fourStateOptions = reader.GetDefaultOptions(BarcodeSymbology.USPS4State) as FourStateBarcodeReadOptions;
         fourStateOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

         PatchCodeBarcodeReadOptions patchCodeOptions = reader.GetDefaultOptions(BarcodeSymbology.PatchCode) as PatchCodeBarcodeReadOptions;
         patchCodeOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

         PostNetPlanetBarcodeReadOptions postNetOptions = reader.GetDefaultOptions(BarcodeSymbology.PostNet) as PostNetPlanetBarcodeReadOptions;
         postNetOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

         PharmaCodeBarcodeReadOptions pharmaCodeOptions = reader.GetDefaultOptions(BarcodeSymbology.PharmaCode) as PharmaCodeBarcodeReadOptions;
         pharmaCodeOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

         // Double pass
         oneDOptions.EnableDoublePass = doublePass;

         DatamatrixBarcodeReadOptions dataMatrixOptions = reader.GetDefaultOptions(BarcodeSymbology.Datamatrix) as DatamatrixBarcodeReadOptions;
         dataMatrixOptions.EnableDoublePass = doublePass;

         PDF417BarcodeReadOptions pdf417Options = reader.GetDefaultOptions(BarcodeSymbology.PDF417) as PDF417BarcodeReadOptions;
         pdf417Options.EnableDoublePass = doublePass;

         MicroPDF417BarcodeReadOptions microPdf4127Options = reader.GetDefaultOptions(BarcodeSymbology.MicroPDF417) as MicroPDF417BarcodeReadOptions;
         microPdf4127Options.EnableDoublePass = doublePass;

         QRBarcodeReadOptions qrOptions = reader.GetDefaultOptions(BarcodeSymbology.QR) as QRBarcodeReadOptions;
         qrOptions.EnableDoublePass = doublePass;

         reader.ImageType = BarcodeImageType.Unknown;
      }

      public static void SafeDeleteFile(string fileName)
      {
         if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
         {
            try
            {
               File.Delete(fileName);
            }
            catch { }
         }
      }

      // Global Cache object used by all controllers.
      // This object is created during service initialization
      private static ObjectCache _cache = null;
      public static ObjectCache Cache
      {
         get { return _cache; }
      }

      private static OcrEngineStatus _OcrEngineStatus = OcrEngineStatus.Unset;
      public static OcrEngineStatus OcrEngineStatus
      {
         get { return ServiceHelper._OcrEngineStatus; }
      }

      // Global IOcrEngine instance used by all the controllers
      // This object is created during service initialization
      private static IOcrEngine _ocrEngine;
      public static IOcrEngine GetOCREngine()
      {
         return _ocrEngine;
      }

      public static string CheckOCRRuntimeDirectory()
      {
         // Check if we are running on a machine that has LEADTOOLS installed, try to get the path automatically
         var appPath = HostingEnvironment.ApplicationPhysicalPath;
         string[] ocrRuntimeDirs =
         {
            @"..\..\..\..\..\..\Bin\Common\OcrLEADRuntime",
         };

         // Leave this as a loop in the event that we decide to add more options later.
         foreach (var runtimeDir in ocrRuntimeDirs)
         {
            var dir = Path.GetFullPath(Path.Combine(appPath, runtimeDir));
            if (Directory.Exists(dir) && Directory.EnumerateFileSystemEntries(dir).Any())
               return dir;
         }

         // Not found
         return null;
      }

      public static void CreateOCREngine()
      {
         if (_ocrEngine != null)
            _ocrEngine.Dispose();

         // Reset the OCR Engine Status
         _OcrEngineStatus = OcrEngineStatus.Unset;

         var engineTypeString = ServiceHelper.GetSettingValue(ServiceHelper.Key_Ocr_EngineType);
         if (string.IsNullOrEmpty(engineTypeString))
            return;

         var engineType = OcrEngineType.LEAD;
         try
         {
            // not necessary since we set to LEAD OCR above, but here as an example.
            if (engineTypeString.Equals("lead", StringComparison.OrdinalIgnoreCase))
               engineType = OcrEngineType.LEAD;
            else if (engineTypeString.Equals("omnipage", StringComparison.OrdinalIgnoreCase))
               engineType = OcrEngineType.OmniPage;
         }
         catch
         {
            // Error with engine type
            _OcrEngineStatus = OcrEngineStatus.Error;
            return;
         }

         // Check for a location of the OCR Runtime
         var runtimeDirectory = ServiceHelper.GetSettingValue(ServiceHelper.Key_Ocr_RuntimeDirectory);
         runtimeDirectory = ServiceHelper.GetAbsolutePath(runtimeDirectory);
         if (string.IsNullOrEmpty(runtimeDirectory))
            runtimeDirectory = CheckOCRRuntimeDirectory();

         // Use LEAD OCR engine
         var ocrEngine = OcrEngineManager.CreateEngine(engineType, true);

         try
         {
            // Start it up
            ocrEngine.Startup(null, null, null, runtimeDirectory);
            _ocrEngine = ocrEngine;
            _OcrEngineStatus = OcrEngineStatus.Ready;
         }
         catch
         {
            ocrEngine.Dispose();
            _OcrEngineStatus = OcrEngineStatus.Error;
            System.Diagnostics.Trace.WriteLine("The OCR Engine could not be started. This application will continue to run, but without OCR functionality.");
         }
      }

      public static string GetFileUri(Uri uri)
      {
         if (!uri.IsAbsoluteUri)
            return uri.ToString();
         if (uri.Scheme == Uri.UriSchemeFile)
            return uri.LocalPath;
         else
            return null;
      }

      public static void CopyStream(Stream source, Stream target)
      {
         const int bufferSize = 1024 * 64;
         var buffer = new byte[bufferSize];
         int bytesRead = 0;
         do
         {
            bytesRead = source.Read(buffer, 0, bufferSize);
            if (bytesRead > 0)
               target.Write(buffer, 0, bytesRead);
         }
         while (bytesRead > 0);
      }

      private static bool CanViewInBrowser(string contentType)
      {
         if (string.IsNullOrEmpty(contentType))
            return false;
         switch (contentType)
         {
            case "application/pdf":
            //case "application/xml": Uncomment to allow XML to be viewed in browsers that support it
            case "application/json":
            case "text/plain":
            case "text/html":
            case "text/css":
            case "image/png":
            case "image/jpeg":
            case "image/gif":
               return true;

            default:
               return false;
         }
      }

      public static void SetResponseViewFileName(HttpResponseMessage response, string fileName, string browserViewContentType)
      {
         bool tryViewInBrowser = CanViewInBrowser(browserViewContentType);

         // "Content-Disposition: Inline" specifies that the document should be opened with the browser's viewer if possible and gives a name to the downloaded item.
         var disposition = new ContentDisposition
         {
            DispositionType = tryViewInBrowser ? DispositionTypeNames.Inline : DispositionTypeNames.Attachment,
            FileName = fileName
         };
         response.Content.Headers.Add("Content-Disposition", disposition.ToString());
      }

      public static string RemoveExtension(string filename, string extension)
      {
         if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(extension))
            return filename;

         if (!extension.StartsWith("."))
            extension = "." + extension;

         if (filename.EndsWith(extension))
            return filename.Substring(0, filename.LastIndexOf(extension));

         return filename;
      }

      // Global AnnRenderingEngine instance used by all the controllers
      // This object is created during service initialization
      private static AnnRenderingEngine _renderingEngine;
      public static AnnRenderingEngine GetAnnRenderingEngine()
      {
         return _renderingEngine;
      }

      public static void CreateAnnRenderingEngine()
      {
         if (_renderingEngine != null)
            return;

         try
         {
            string annotationsEngineAssemblyName = typeof(AnnRenderingEngine).Assembly.FullName;
            string annotationsRenderingAssemblyName = annotationsEngineAssemblyName.Replace("Leadtools.Annotations.Engine", "Leadtools.Annotations.Rendering.WinForms");
            AssemblyName assemblyName = new AssemblyName(annotationsRenderingAssemblyName);
            Assembly annotationsRenderingAssembly = Assembly.Load(assemblyName);
            _renderingEngine = annotationsRenderingAssembly.CreateInstance("Leadtools.Annotations.Rendering.AnnWinFormsRenderingEngine") as AnnRenderingEngine;
         }
         catch { }
      }

      // We should not let the user use file scheme (unsafe and security issue) to upload files.
      public static void CheckUriScheme(Uri uri)
      {
         bool isFileScheme = false;
         try
         {
            if (uri.IsFile || uri.IsUnc || uri.Scheme == Uri.UriSchemeFile)
               isFileScheme = true;
         }
         catch { }

         if (isFileScheme)
            throw new ArgumentException("uri scheme not supported by this implementation");
      }
   }

   class MimeTypesConfig
   {
      [DataMember(Name = "allowed")]
      public string[] Allowed { get; set; }

      [DataMember(Name = "denied")]
      public string[] Denied { get; set; }
   }

   class HtmlDomainWhitelistConfig
   {
      [DataMember(Name = "whitelisted")]
      public string[] Whitelisted { get; set; }
   }
}

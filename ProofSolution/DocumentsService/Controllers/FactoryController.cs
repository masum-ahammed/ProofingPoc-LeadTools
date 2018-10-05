// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Diagnostics;

using Leadtools.Document;
using Leadtools.Demos;
using Leadtools.Codecs;
using Leadtools.Caching;

using Leadtools.DocumentViewer.Models.Factory;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.Services.Tools.Helpers;
using Leadtools.Services.Models;
using Leadtools.Services.Models.PreCache;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.IO.Packaging;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used with the DocumentFactory class of the LEADTOOLS Document JavaScript library.
   /// </summary>
   public class FactoryController : ApiController
   {
      public FactoryController()
      {
         ServiceHelper.InitializeController();
      }

      /// <summary>
      ///  Loads the specified document from the cache, if possible. Errors if the document is not in the cache.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document could not be loaded from the cache")]
      [HttpPost]
      public LoadFromCacheResponse LoadFromCache(LoadFromCacheRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            // Return null if the document does not exist in the cache
            // If you want to throw an error then call:
            // DocumentHelper.CheckLoadFromCache(document);

            if (document != null && document.CacheStatus == DocumentCacheStatus.NotSynced)
            {
               // This means this document was uploaded and never loaded, make sure it does not delete itself after we dispose it and perform the same action as if
               // a document was loaded from a URI
               CacheController.TrySetCacheUri(document);

               if (ServiceHelper.AutoUpdateHistory)
                  document.History.AutoUpdate = true;

               ServiceHelper.SetRasterCodecsOptions(document.RasterCodecs, (int)document.Pages.DefaultResolution);
               document.AutoDeleteFromCache = false;
               document.AutoDisposeDocuments = true;
               document.AutoSaveToCache = false;
               document.SaveToCache();
            }

            return new LoadFromCacheResponse { Document = document };
         }
      }

      /// <summary>
      ///  Creates and stores an entry for the image at the URI, returning the appropriate Document data.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The uri data could not be loaded")]
      [HttpPost, HttpGet] // Support GET only for testing
      public LoadFromUriResponse LoadFromUri(LoadFromUriRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         if (request.Uri == null)
            throw new ArgumentException("uri must be specified");

         ServiceHelper.CheckUriScheme(request.Uri);
         if (request.Options != null && request.Options.AnnotationsUri != null)
            ServiceHelper.CheckUriScheme(request.Options.AnnotationsUri);

         if (request.Resolution < 0)
            throw new ArgumentException("Resolution must be a value greater than or equal to zero");

         var cache = ServiceHelper.Cache;

         var loadOptions = new LoadDocumentOptions();
         loadOptions.Cache = cache;
         loadOptions.UseCache = cache != null;
         loadOptions.CachePolicy = ServiceHelper.CreatePolicy();
         loadOptions.WebClient = null; // Use default

         if (request.Options != null)
         {
            loadOptions.DocumentId = request.Options.DocumentId;
            loadOptions.AnnotationsUri = request.Options.AnnotationsUri;
            loadOptions.Name = request.Options.Name;
            loadOptions.Password = request.Options.Password;
            loadOptions.LoadEmbeddedAnnotations = request.Options.LoadEmbeddedAnnotations;
            loadOptions.MaximumImagePixelSize = request.Options.MaximumImagePixelSize;
            loadOptions.FirstPageNumber = request.Options.FirstPageNumber;
            loadOptions.LastPageNumber = request.Options.LastPageNumber;
         }

         // Get the document name
         var documentName = request.Uri.ToString();

         // Check if this document was uploaded, then hope the user has set LoadDocumentOptions.Name to the original file name
         if (DocumentFactory.IsUploadDocumentUri(request.Uri) && !string.IsNullOrEmpty(loadOptions.Name))
         {
            // Use that instead
            documentName = loadOptions.Name;
         }

         // Most image file formats have a signature that can be used to detect to detect the type of the file.
         // However, some formats supported by LEADTOOLS do not, such as plain text files (TXT) or DXF CAD format or 
         // For these, we detect the MIME type from the file extension if available and set it in the load document options and the
         // documents library will use this value if it fails to detect the file format from the data.

         if (!string.IsNullOrEmpty(documentName))
            loadOptions.MimeType = RasterCodecs.GetExtensionMimeType(documentName);

         LEADDocument document = null;
         try
         {
            // first, check if this is pre-cached
            if (PreCacheHelper.PreCacheExists)
            {
               string documentId = PreCacheHelper.CheckDocument(request.Uri, loadOptions.MaximumImagePixelSize);
               if (documentId != null)
               {
                  var precachedDocument = DocumentFactory.LoadFromCache(cache, documentId);
                  // Instead of returning the same pre-cached document, we'll return a cloned version.
                  // This allows the user to make changes (get/set annotations) without affecting the pre-cached version.
                  document = precachedDocument.Clone(cache, new CloneDocumentOptions()
                  {
                     CachePolicy = ServiceHelper.CreatePolicy()
                  });
               }
            }

            // else, load normally
            if (document == null)
            {
               document = DocumentFactory.LoadFromUri(request.Uri, loadOptions);
               if (document == null)
               {
                  // This document was rejected due to its mimeType.
                  throw new InvalidOperationException("Document at URI '" + request.Uri + "' uses a blocked mimeType");
               }
            }

            CacheController.TrySetCacheUri(document);

            if (ServiceHelper.AutoUpdateHistory)
               document.History.AutoUpdate = true;

            ServiceHelper.SetRasterCodecsOptions(document.RasterCodecs, request.Resolution);
            document.AutoDeleteFromCache = false;
            document.AutoDisposeDocuments = true;
            document.AutoSaveToCache = false;
            document.SaveToCache();

            /* 
             * NOTE: Use the line below to add this new document
                  * to the pre-cache. By doing so, everyone loading a document from
             * that URL will get a copy of the same document from the cache/pre-cache.
             * 
             * if (!isInPrecache)
             *  PreCacheHelper.AddExistingDocument(request.Uri, document);
             */
            return new LoadFromUriResponse { Document = document };
         }
         finally
         {
            if (document != null)
               document.Dispose();
         }
      }

      /// <summary>
      ///  Creates a link that a document can be uploaded to for storing in the cache. Meant to be used with UploadDocument.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [ServiceErrorAttribute(Message = "The cache could not create an upload url")]
      [HttpPost]
      public BeginUploadResponse BeginUpload(BeginUploadRequest request)
      {
         var cache = ServiceHelper.Cache;
         var uploadOptions = request.Options;
         if (uploadOptions == null)
            uploadOptions = new UploadDocumentOptions();

         if (string.IsNullOrEmpty(uploadOptions.DocumentId) && !string.IsNullOrEmpty(request.DocumentId))
            uploadOptions.DocumentId = request.DocumentId;

         uploadOptions.Cache = cache;
         Uri uploadUri = DocumentFactory.BeginUpload(uploadOptions);
         return new BeginUploadResponse { UploadUri = uploadUri };
      }

      /// <summary>
      /// Uploads a chunk of data to the specified URL in the cache.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document data could not be uploaded")]
      [HttpPost]
      public Response UploadDocument(UploadDocumentRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         byte[] byteArray = null;
         if (request.Data != null)
            byteArray = System.Convert.FromBase64String(request.Data);

         var cache = ServiceHelper.Cache;
         DocumentFactory.UploadDocument(cache, request.Uri, byteArray, 0, byteArray != null ? byteArray.Length : 0);
         return new Response();
      }

      /// <summary>
      ///  Optional - marks the end of uploading data for the specified URL in the cache. The service can execute commands to do
      ///  additional processing with this fully-uploaded document.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [ServiceErrorAttribute(Message = "The document upload could not be completed")]
      [HttpPost]
      public Response EndUpload(EndUploadRequest request)
      {
         // Note - we cannot always expect this endpoint to get called or be successful.
         if (request != null)
         {
            var cache = ServiceHelper.Cache;
            DocumentFactory.EndUpload(cache, request.Uri);
         }
         return new Response();
      }

      /// <summary>
      /// Aborts the document upload.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document upload could not be aborted")]
      [HttpPost]
      public Response AbortUploadDocument(AbortUploadDocumentRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         // Only useful in cases where routing matches but uri is null (like ".../CancelUpload?uri")

         try
         {
            var cache = ServiceHelper.Cache;
            DocumentFactory.AbortUploadDocument(cache, request.Uri);
         }
         catch
         {
            //ignore any error
         }

         return new Response();
      }

      /// <summary>
      ///  Saves the specified document to the cache. If the document is not in the cache it will be created.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document could not be saved to the cache")]
      [HttpPost]
      public SaveToCacheResponse SaveToCache(SaveToCacheRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");
         if (request.Descriptor == null)
            throw new ArgumentNullException("Descriptor");

         var cache = ServiceHelper.Cache;

         // First try to load it from the cache, if success, update it. Otherwise, assume it is not there and create a new document

         using (var document = DocumentFactory.LoadFromCache(cache, request.Descriptor.DocumentId))
         {
            if (document != null)
            {
               // Update it
               document.UpdateFromDocumentDescriptor(request.Descriptor);
               document.AutoDeleteFromCache = false;
               document.AutoDisposeDocuments = true;
               document.AutoSaveToCache = false;
               document.SaveToCache();
               return new SaveToCacheResponse { Document = document };
            }
         }

         // Above failed, create a new one.
         var createOptions = new CreateDocumentOptions();
         createOptions.Descriptor = request.Descriptor;
         createOptions.Cache = cache;
         createOptions.UseCache = cache != null;
         createOptions.CachePolicy = ServiceHelper.CreatePolicy();
         using (var document = DocumentFactory.Create(createOptions))
         {
            if (document == null)
               throw new InvalidOperationException("Failed to create document");

            CacheController.TrySetCacheUri(document);

            if (ServiceHelper.AutoUpdateHistory)
               document.History.AutoUpdate = true;

            document.AutoDeleteFromCache = false;
            document.AutoDisposeDocuments = true;
            document.AutoSaveToCache = false;
            document.SaveToCache();
            return new SaveToCacheResponse { Document = document };
         }
      }

      /* Deletes the document immediately from the cache.
       * Usually changing AutoDeleteFromCache to true
       * would only delete the document when the cache is purged,
       * but it also deletes it the next time the document is cleaned
       * up - which happens to be right after we are finished changing
       * the autoDeleteFromCache property. So it's immediate.
       */
      /// <summary>
      /// Deletes the document from the cache.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document could not be deleted", MethodName = "DeleteFromCache")]
      [HttpPost]
      public Response Delete(DeleteRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         DocumentHelper.DeleteDocument(request.DocumentId, true, !request.AllowNonExisting);

         return new Response();
      }

      /// <summary>
      /// Purges the cache of all outdated items. Requires a passcode set on the service's configuration.
      /// </summary>
      // used to check the policies and remove outstanding cache items.
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The cache could not be purged")]
      [HttpPost, HttpGet]
      public Response PurgeCache(string passcode = null)
      {
         string passToCheck = ServiceHelper.GetSettingValue(ServiceHelper.Key_Access_Passcode);
         if (!string.IsNullOrWhiteSpace(passToCheck) && passcode != passToCheck)
            throw new ServiceException("Cache cannot be purged - passcode is incorrect", HttpStatusCode.Unauthorized);

         var cache = ServiceHelper.Cache;
         var fileCache = cache as FileCache;
         if (fileCache != null)
            fileCache.CheckPolicies();
         return new Response();
      }

      /// <summary>
      /// Checks the policies of the cache items and returns statistics, without deleting expired items.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The cache statistics could not be retrieved")]
      [HttpGet]
      public GetCacheStatisticsResponse GetCacheStatistics(string passcode = null)
      {
         string passToCheck = ServiceHelper.GetSettingValue(ServiceHelper.Key_Access_Passcode);
         if (!string.IsNullOrWhiteSpace(passToCheck) && passcode != passToCheck)
            throw new ServiceException("Cache statistics cannot be retrieved - passcode is incorrect", HttpStatusCode.Unauthorized);

         Caching.CacheStatistics statistics = null;
         var cache = ServiceHelper.Cache;
         if (cache is FileCache)
            statistics = cache.GetStatistics();
         return new GetCacheStatisticsResponse()
         {
            Statistics = statistics
         };
      }

      /// <summary>
      /// Adds the specified document to the cache with an unlimited expiration and all document data. Future calls to LoadFromUri may return
      /// this document (matched by URI).
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document could not be pre-cached")]
      [HttpPost]
      public PreCacheDocumentResponse PreCacheDocument(PreCacheDocumentRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         string passToCheck = ServiceHelper.GetSettingValue(ServiceHelper.Key_Access_Passcode);
         if (!string.IsNullOrWhiteSpace(passToCheck) && request.Passcode != passToCheck)
            throw new ServiceException("Document cannot be pre-cached - passcode is incorrect", HttpStatusCode.Unauthorized);

         if (request.Uri == null)
            throw new ArgumentException("uri must be specified");

         ServiceHelper.CheckUriScheme(request.Uri);

         if (!PreCacheHelper.PreCacheExists)
         {
            // Return an empty item
            return new PreCacheDocumentResponse();
         }

         var cache = ServiceHelper.Cache;

         // Get the cache options, if none, use All (means if the user did not pass a value, we will cache everything in the document)
         if (request.CacheOptions == DocumentCacheOptions.None)
            request.CacheOptions = DocumentCacheOptions.All;

         return PreCacheHelper.AddDocument(cache, request);
      }

      /// <summary>
      /// Returns all the entries in the pre-cache.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The pre-cache dictionary could not be returned")]
      [HttpGet]
      public ReportPreCacheResponse ReportPreCache([FromUri] ReportPreCacheRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         string passToCheck = ServiceHelper.GetSettingValue(ServiceHelper.Key_Access_Passcode);
         if (!string.IsNullOrWhiteSpace(passToCheck) && request.Passcode != passToCheck)
            throw new ServiceException("Pre-cache cannot be reported - passcode is incorrect", HttpStatusCode.Unauthorized);

         if (!PreCacheHelper.PreCacheExists)
         {
            // Return an empty report
            return new ReportPreCacheResponse();
         }

         var cache = ServiceHelper.Cache;
         return PreCacheHelper.ReportDocuments(cache, request.Clean);
      }

      /// <summary>
      /// Checks that the mimetype for an uploaded document is acceptable to load.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document cache info could not be retrieved")]
      [HttpPost]
      public CheckCacheInfoResponse CheckCacheInfo(CheckCacheInfoRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var cache = ServiceHelper.Cache;
         DocumentCacheInfo cacheInfo = DocumentFactory.GetDocumentCacheInfo(cache, request.Uri.ToString());

         if (cacheInfo == null)
            return new CheckCacheInfoResponse();

         var documentMimeType = cacheInfo.MimeType;
         var status = DocumentFactory.MimeTypes.GetStatus(documentMimeType);
         var isAccepted = status != DocumentMimeTypeStatus.Denied;

         var serviceCacheInfo = new CacheInfo
         {
            IsVirtual = cacheInfo.IsVirtual,
            IsLoaded = cacheInfo.IsLoaded,
            HasAnnotations = cacheInfo.HasAnnotations,
            Name = cacheInfo.Name,
            MimeType = documentMimeType,
            IsMimeTypeAccepted = isAccepted,
            PageCount = cacheInfo.PageCount
         };

         return new CheckCacheInfoResponse
         {
            CacheInfo = serviceCacheInfo
         };
      }

      /// <summary>
      /// Downloads the annotations of a document for external use.
      /// If no annotations exist, an empty XML file is returned.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The annotations could not be downloaded")]
      [HttpGet]
      public HttpResponseMessage DownloadAnnotations([FromUri] DownloadAnnotationsRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         if ((request.DocumentId == null && request.Uri == null) || (request.DocumentId != null && request.Uri != null))
            throw new InvalidOperationException("DocumentId or Uri must not be null, but not both");

         var cache = ServiceHelper.Cache;
         DocumentCacheInfo cacheInfo;

         if (request.Uri != null)
            cacheInfo = DocumentFactory.GetDocumentCacheInfo(cache, request.Uri);
         else
            cacheInfo = DocumentFactory.GetDocumentCacheInfo(cache, request.DocumentId);
         DocumentHelper.CheckCacheInfo(cacheInfo);

         var name = "download";
         if (!string.IsNullOrEmpty(cacheInfo.Name))
         {
            var documentMimeType = cacheInfo.MimeType;
            var documentExtension = RasterCodecs.GetMimeTypeExtension(documentMimeType);
            name = ServiceHelper.RemoveExtension(cacheInfo.Name, documentExtension);
         }

         var annotationsName = string.Format("{0}_ann.xml", name);

         var responseFileName = annotationsName;
         var responseContentType = "application/xml";

         var documentId = cacheInfo.DocumentId;

         Action<Stream, HttpContent, TransportContext> write = (outputStream, content, context) =>
         {
            DocumentFactory.DownloadAnnotations(cache, documentId, 0, -1, outputStream);
            outputStream.Close();
         };

         var response = new HttpResponseMessage(HttpStatusCode.OK);

         // For "Save to Google Drive" access, we must have the appropriate CORS headers.
         // See https://developers.google.com/drive/v3/web/savetodrive
         response.Headers.Remove("Access-Control-Allow-Headers");
         response.Headers.Add("Access-Control-Allow-Headers", "Range");
         response.Headers.Remove("Access-Control-Expose-Headers");
         response.Headers.Add("Access-Control-Expose-Headers", "Cache-Control, Content-Encoding, Content-Range");

         response.Content = new PushStreamContent(write, new MediaTypeHeaderValue(responseContentType));

         ServiceHelper.SetResponseViewFileName(response, responseFileName, responseContentType);

         return response;
      }

      /// <summary>
      /// Downloads a document (and possibly annotations) for external use.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The item could not be downloaded")]
      [HttpGet]
      public HttpResponseMessage DownloadDocument([FromUri] DownloadDocumentRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         if ((request.DocumentId == null && request.Uri == null) || (request.DocumentId != null && request.Uri != null))
            throw new InvalidOperationException("DocumentId or Uri must not be null, but not both");

         var cache = ServiceHelper.Cache;
         DocumentCacheInfo cacheInfo;

         if (request.Uri != null)
            cacheInfo = DocumentFactory.GetDocumentCacheInfo(cache, request.Uri);
         else
            cacheInfo = DocumentFactory.GetDocumentCacheInfo(cache, request.DocumentId);
         DocumentHelper.CheckCacheInfo(cacheInfo);

         var documentMimeType = cacheInfo.MimeType;
         var documentExtension = RasterCodecs.GetMimeTypeExtension(documentMimeType);
         if (string.IsNullOrEmpty(documentExtension))
            documentExtension = "data";

         var name = "download";
         if (!string.IsNullOrEmpty(cacheInfo.Name))
            name = ServiceHelper.RemoveExtension(cacheInfo.Name, documentExtension);

         var documentName = string.Format("{0}.{1}", name, documentExtension);
         var annotationsName = string.Format("{0}_ann.xml", name);

         var responseFileName = documentName;
         var responseContentType = documentMimeType;

         bool zipAnnotations = request.IncludeAnnotations && cacheInfo.HasAnnotations;
         if (zipAnnotations)
         {
            // We will create a ZIP file
            responseFileName = string.Format("{0}.zip", name);
            responseContentType = MediaTypeNames.Application.Zip;
         }

         var documentId = cacheInfo.DocumentId;

         Action<Stream, HttpContent, TransportContext> write = (outputStream, content, context) =>
         {
            if (zipAnnotations)
            {
               // We must create a new memory stream because Package.Open tries to request position
               using (var zipStream = new MemoryStream())
               {
                  using (var package = Package.Open(zipStream, FileMode.CreateNew))
                  {
                     var documentPart = package.CreatePart(new Uri(string.Format("/{0}", documentName), UriKind.Relative), documentMimeType);
                     Stream documentStream = documentPart.GetStream();
                     DocumentFactory.DownloadDocument(cache, documentId, 0, -1, documentStream);
                     documentStream.Close();

                     var annotationsPart = package.CreatePart(new Uri(string.Format("/{0}", annotationsName), UriKind.Relative), "text/xml");
                     Stream annotationsStream = annotationsPart.GetStream();
                     DocumentFactory.DownloadAnnotations(cache, documentId, 0, -1, annotationsStream);
                     annotationsStream.Close();
                  }

                  zipStream.Position = 0;
                  ServiceHelper.CopyStream(zipStream, outputStream);
               }
            }
            else
            {
               // Just the document
               DocumentFactory.DownloadDocument(cache, documentId, 0, -1, outputStream);
            }
            outputStream.Close();
         };

         var response = new HttpResponseMessage(HttpStatusCode.OK);

         // For "Save to Google Drive" access, we must have the appropriate CORS headers.
         // See https://developers.google.com/drive/v3/web/savetodrive
         response.Headers.Remove("Access-Control-Allow-Headers");
         response.Headers.Add("Access-Control-Allow-Headers", "Range");
         response.Headers.Remove("Access-Control-Expose-Headers");
         response.Headers.Add("Access-Control-Expose-Headers", "Cache-Control, Content-Encoding, Content-Range");

         response.Content = new PushStreamContent(write, new MediaTypeHeaderValue(responseContentType));

         ServiceHelper.SetResponseViewFileName(response, responseFileName, responseContentType);

         return response;
      }
   }
}

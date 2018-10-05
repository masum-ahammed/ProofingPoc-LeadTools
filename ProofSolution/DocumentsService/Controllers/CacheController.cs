// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Leadtools.Caching;
using Leadtools.Codecs;
using Leadtools.Document;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.Services.Tools.Helpers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web;
using System.Web.Http;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used to return public items from the cache.
   /// </summary>
   public class CacheController : ApiController
   {
      public CacheController()
      {
         ServiceHelper.InitializeController();
      }

      [NonAction]
      public static void TrySetCacheUri(LEADDocument document)
      {
         /* If your cache is in a separate service, you can customize the URL that is used to access the cached item
          * data.
          * Alternatively, if using the LEADTOOLS FileCache, you can set the CacheVirtualDirectory value
          * to something like "cache". When returning from the cache, a document's CacheUri will be
          * pre-set to [CacheVirtualDirectory]/[CacheRegion]/[CacheItemId].
          * This is optimal for simple cases where the CacheVirtualDirectory points to a file system that is
          * hosted via IIS.
          * 
          * If Document.CacheUri is null, it is set in the JavaScript to access the CacheController.GetDocumentData
          * method below.
          */

         //if (document != null)
         //{
         //   document.CacheUri = new Uri(string.Format("http://my-cache.com/getItem?id={0}", document.DocumentId));
         //}
      }

      /// <summary>
      ///   Retrieves the original document data as a stream; used primarily for loading a PDF Document's stream
      ///   for use in client-side PDF Rendering/Text Extraction in Leadtools.Document.Viewer.
      /// </summary>
      /// <param name="documentId">The identifier for this document.</param>
      /// <returns>A stream to the original document data.</returns>
      [AlwaysCorsFilter]
      [ServiceErrorAttribute(Message = "The document data could not be loaded")]
      [HttpGet]
      public HttpResponseMessage GetDocumentData(string documentId)
      {
         var cache = ServiceHelper.Cache;
         
         var cacheInfo = DocumentFactory.GetDocumentCacheInfo(cache, documentId);
         DocumentHelper.CheckCacheInfo(cacheInfo);

         string contentType = cacheInfo.MimeType;

         var response = new HttpResponseMessage(HttpStatusCode.OK);
         
         // Required headers for client-side PDF streaming
         response.Headers.Remove("Accept-Ranges");
         response.Headers.Remove("Access-Control-Expose-Headers");
         response.Headers.Add("Access-Control-Expose-Headers", "Accept-Ranges, Content-Encoding, Content-Length");

         Action<Stream, HttpContent, TransportContext> write = (stream, content, context) =>
         {
            DocumentFactory.DownloadDocument(cache, documentId, 0, -1, stream);
            stream.Close();
         };

         response.Content = new PushStreamContent(write, new MediaTypeHeaderValue(contentType));
         return response;
      }

      /* Creates a URL that is returned to the client.
       * The client will call this URL to view the conversion result.
       * We return a URL that looks like a direct folder access. This is possible
       * due to the routing setup in WebApiConfig.cs and Web.config.
       */
      [NonAction]
      public static Uri CreateConversionResultUri(string region, string key)
      {
         return new Uri(string.Format("Cache/Item/{0}/{1}", region, key), UriKind.Relative);
      }

      /// <summary>
      ///   Retrieves a cache item. Generally this URI should be set up with authentication and accessed with POST to protect sensitive information.
      ///   <see cref="CreateConversionResultUri(string, string)">CreateConversionResultUri</see> is the source of this URI for the client; conversion results are sent to the client
      ///   with a relative URI to be accessed from here.
      ///   This mainly acts as a simple map to the cache directory.
      /// </summary>
      /// <param name="region">The region (folder) in the cache.</param>
      /// <param name="key">The key (filename) in the cache.</param>
      /// <returns></returns>
      [AlwaysCorsFilter]
      [ServiceErrorAttribute(Message = "The cache item could not be returned")]
      [HttpGet]
      public HttpResponseMessage Item(string region, string key)
      {
         var cache = ServiceHelper.Cache as FileCache;

         if (cache == null)
            throw new InvalidOperationException("FileCache must be used for this operation");

         Uri itemUri = cache.GetItemExternalResource(key, region, false);
         string fullPath = itemUri.LocalPath;

         if (!File.Exists(fullPath))
            throw new ServiceException("File not found", HttpStatusCode.NotFound);

         var response = new HttpResponseMessage();

         // For "Save to Google Drive" access, we must have the appropriate CORS headers.
         // See https://developers.google.com/drive/v3/web/savetodrive
         response.Headers.Remove("Access-Control-Allow-Headers");
         response.Headers.Add("Access-Control-Allow-Headers", "Range");
         response.Headers.Remove("Access-Control-Expose-Headers");
         response.Headers.Add("Access-Control-Expose-Headers", "Cache-Control, Content-Encoding, Content-Range");

         try
         {
            string contentType = "application/octet-stream";
            try
            {
               var extension = Path.GetExtension(fullPath);

               // ZIP is not handled by RasterCodecs, so check it here
               if (!string.IsNullOrEmpty(extension) && !extension.EndsWith("zip", StringComparison.OrdinalIgnoreCase))
               {
                  // Use RasterCodecs helper method to get it
                  var extensionContentType = RasterCodecs.GetExtensionMimeType(extension);
                  if (!string.IsNullOrWhiteSpace(extensionContentType))
                     contentType = extensionContentType;
               }
            }
            catch { }

            Action<Stream, HttpContent, TransportContext> write = (stream, content, context) =>
            {
               try
               {
                  using (var fs = File.OpenRead(fullPath))
                     ServiceHelper.CopyStream(fs, stream);
               }
               finally
               {
                  stream.Close();
               }
            };

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new PushStreamContent(write, new MediaTypeHeaderValue(contentType));
            ServiceHelper.SetResponseViewFileName(response, key, contentType);

            return response;
         }
         catch (IOException)
         {
            throw new ServiceException("File could not be streamed", HttpStatusCode.InternalServerError);
         }
      }
   }
}

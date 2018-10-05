// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Leadtools.DocumentViewer.Models.Images;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.DocumentViewer.Tools.Helpers;
using Leadtools.Services.Tools.Helpers;
using Leadtools;
using Leadtools.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used with the DocumentImages class of the LEADTOOLS Document JavaScript library.
   /// </summary>
   public class ImagesController : ApiController
   {
      /* These endpoints will not necessarily return objects,
       * since most of the time the returned streams
       * will be set directly to a URL.
       */

      /* We use HttpResponseMessage now in WebApi because
       * if we just returned a stream, WebApi would try to automatically
       * serialize it as JSON or XML. By constructing our own response
       * we will not have the body content serialized automatically.
       * See PageController for more examples.
       */

      public ImagesController()
      {
         ServiceHelper.InitializeController();
      }

      /// <summary>
      /// Retrieves thumbnails as a grid, instead of as individual images.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The thumbnails could not be retrieved")]
      [HttpGet, AlwaysCorsFilter]
      public HttpResponseMessage GetThumbnailsGrid([FromUri] GetThumbnailsGridRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request", "must not be null");

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId", "must not be null");

         if (request.FirstPageNumber < 0)
            throw new ArgumentException("'firstPageNumber' must be a value greater than or equal to 0");

         var firstPageNumber = request.FirstPageNumber;
         var lastPageNumber = request.LastPageNumber;

         // Default is page 1 and -1
         if (firstPageNumber == 0)
            firstPageNumber = 1;
         if (lastPageNumber == 0)
            lastPageNumber = -1;

         if (request.Width < 0 || request.Height < 0)
            throw new ArgumentException("'width' and 'height' must be value greater than or equal to 0");
         if (request.MaximumGridWidth < 0)
            throw new ArgumentException("'maximumGridWidth' must be a value greater than or equal to 0");

         // Get the image format
         var saveFormat = SaveImageFormat.GetFromMimeType(request.MimeType);

         // Now load the document
         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            DocumentHelper.CheckLoadFromCache(document);

            if (request.Width > 0 && request.Height > 0)
               document.Images.ThumbnailPixelSize = new LeadSize(request.Width, request.Height);

            using (var image = document.Images.GetThumbnailsGrid(firstPageNumber, lastPageNumber, request.MaximumGridWidth))
            {
               Stream stream = ImageSaver.SaveImage(image, document.RasterCodecs, saveFormat, request.MimeType, 0, 0);

               // If we just return the stream, Web Api will try to serialize it.
               // If the return type is "HttpResponseMessage" it will not serialize
               // and you can set the content as you wish.
               var result = new HttpResponseMessage();
               result.Content = new StreamContent(stream);
               ServiceHelper.UpdateCacheSettings(result);
               return result;
            }
         }
      }
   }
}

// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Leadtools.DocumentViewer.Models.Page;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.DocumentViewer.Tools.Helpers;
using Leadtools.Services.Tools.Helpers;
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;
using Leadtools.Document;
using Leadtools.Ocr;
using Leadtools.Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Leadtools.Services.Models;
using System.Web.Hosting;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used with the DocumentPage class of the LEADTOOLS Document JavaScript library.
   /// </summary>
   public class PageController : ApiController
   {
      /* These endpoints will not necessarily return objects,
       * since most of the time the returned streams
       * will be set directly to a URL.
       */

      /* We use HttpResponseMessage now in WebApi because
       * if we just returned a stream, WebApi would try to automatically
       * serialize it as JSON or XML. By constructing our own response
       * we will not have the body content serialized automatically.
       */

      public PageController()
      {
         ServiceHelper.InitializeController();
      }

      /// <summary>
      /// Gets the image for this page of the document - not as SVG.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document's page image could not be loaded")]
      [HttpGet, AlwaysCorsFilter]
      public HttpResponseMessage GetImage([FromUri] GetImageRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var pageNumber = request.PageNumber;

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId must not be null");

         if (pageNumber < 0)
            throw new ArgumentException("'pageNumber' must be a value greater than or equal to 0");

         // Default is page 1
         if (pageNumber == 0)
            pageNumber = 1;

         if (request.Resolution < 0)
            throw new ArgumentException("'resolution' must be a value greater than or equal to 0");

         // Sanity check on other parameters
         if (request.QualityFactor < 0 || request.QualityFactor > 100)
            throw new ArgumentException("'qualityFactor' must be a value between 0 and 100");

         if (request.Width < 0 || request.Height < 0)
            throw new ArgumentException("'width' and 'height' must be value greater than or equal to 0");

         // Get the image format
         var saveFormat = SaveImageFormat.GetFromMimeType(request.MimeType);

         // Now load the document
         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            DocumentHelper.CheckLoadFromCache(document);
            DocumentHelper.CheckPageNumber(document, pageNumber);

            var documentPage = document.Pages[pageNumber - 1];
            using (var image = documentPage.GetImage(request.Resolution))
            {
               // Resize it (will only resize if both width and height are not 0), will also take care of FAX images (with different resolution)
               ImageResizer.ResizeImage(image, request.Width, request.Height);
               var stream = ImageSaver.SaveImage(image, document.RasterCodecs, saveFormat, request.MimeType, request.BitsPerPixel, request.QualityFactor);

               // If we just return the stream, Web Api will try to serialize it.
               // If the return type is "HttpResponseMessage" it will not serialize
               // and you can set the content as you wish.
               var response = new HttpResponseMessage();
               response.Content = new StreamContent(stream);
               ServiceHelper.UpdateCacheSettings(response);
               return response;
            }
         }
      }

      //private const string smallest_GIF = "data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==";
      private const string smallest_GIF_base64 = "R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==";

      /// <summary>
      /// Gets the SVG back image if one exists for this document page's SVG. If not, returns the smallest possible GIF so the request 
      /// does not fail.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "Part of the document's page image could not be loaded")]
      [HttpGet, AlwaysCorsFilter]
      public HttpResponseMessage GetSvgBackImage([FromUri] GetSvgBackImageRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var pageNumber = request.PageNumber;

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId must not be null");

         if (pageNumber < 0)
            throw new ArgumentException("'pageNumber' must be a value greater than or equals to 0");

         // Default is page 1
         if (pageNumber == 0)
            pageNumber = 1;

         if (request.Resolution < 0)
            throw new ArgumentException("'resolution' must be a value greater than or equal to 0");

         // Sanity check on other parameters
         if (request.QualityFactor < 0 || request.QualityFactor > 100)
            throw new ArgumentException("'qualityFactor' must be a value between 0 and 100");

         if (request.Width < 0 || request.Height < 0)
            throw new ArgumentException("'width' and 'height' must be value greater than or equal to 0");

         // Get the image format
         var saveFormat = SaveImageFormat.GetFromMimeType(request.MimeType);

         var rasterBackColor = RasterColor.White;

         if (request.BackColor != null)
         {
            try
            {
               rasterBackColor = RasterColor.FromHtml(request.BackColor);
            }
            catch (Exception ex)
            {
               Trace.WriteLine(string.Format("GetImage - Error:{1}{0}documentId:{2} pageNumber:{3}", Environment.NewLine, ex.Message, request.DocumentId, pageNumber), "Error");
            }
         }

         // Now load the document
         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            DocumentHelper.CheckLoadFromCache(document);
            DocumentHelper.CheckPageNumber(document, pageNumber);

            var documentPage = document.Pages[pageNumber - 1];
            var image = documentPage.GetSvgBackImage(rasterBackColor, request.Resolution);
            if (image != null)
            {
               try
               {
                  // Resize it (will only resize if both width and height are not 0), will also take care of FAX images (with different resolution)
                  ImageResizer.ResizeImage(image, request.Width, request.Height);
                  var stream = ImageSaver.SaveImage(image, document.RasterCodecs, saveFormat, request.MimeType, request.BitsPerPixel, request.QualityFactor);

                  // If we just return the stream, Web Api will try to serialize it.
                  // If the return type is "HttpResponseMessage" it will not serialize
                  // and you can set the content as you wish.
                  var response = new HttpResponseMessage();
                  response.Content = new StreamContent(stream);
                  ServiceHelper.UpdateCacheSettings(response);
                  return response;
               }
               finally
               {
                  image.Dispose();
               }
            }
            else
            {
               // Instead of throwing an exception, let's return the smallest possible GIF
               //throw new ServiceException("No SVG Back Image exists", HttpStatusCode.NotFound);

               var response = new HttpResponseMessage();
               var data = Convert.FromBase64String(PageController.smallest_GIF_base64);
               var ms = new MemoryStream(data);
               response.Content =  new StreamContent(ImageSaver.PrepareStream(ms, "image/gif"));
               ServiceHelper.UpdateCacheSettings(response);
               return response;
            }
         }
      }

      /// <summary>
      /// Gets a smaller version of this image for use as a thumbnail.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document's page thumbnail could not be loaded")]
      [HttpGet, AlwaysCorsFilter]
      public HttpResponseMessage GetThumbnail([FromUri] GetThumbnailRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var pageNumber = request.PageNumber;

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId must not be null");

         if (pageNumber < 0)
            throw new ArgumentException("'pageNumber' must be a value greater than or equals to 0");

         // Default is page 1
         if (pageNumber == 0)
            pageNumber = 1;

         if (request.Width < 0 || request.Height < 0)
            throw new ArgumentException("'width' and 'height' must be value greater than or equal to 0");

         // Get the image format
         var saveFormat = SaveImageFormat.GetFromMimeType(request.MimeType);

         // Now load the document
         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            DocumentHelper.CheckLoadFromCache(document);
            DocumentHelper.CheckPageNumber(document, pageNumber);

            if (request.Width > 0 && request.Height > 0)
               document.Images.ThumbnailPixelSize = new LeadSize(request.Width, request.Height);

            var documentPage = document.Pages[pageNumber - 1];
            using (var image = documentPage.GetThumbnailImage())
            {
               var stream = ImageSaver.SaveImage(image, document.RasterCodecs, saveFormat, request.MimeType, 0, 0);

               // If we just return the stream, Web Api will try to serialize it.
               // If the return type is "HttpResponseMessage" it will not serialize
               // and you can set the content as you wish.
               var response = new HttpResponseMessage();
               response.Content = new StreamContent(stream);
               ServiceHelper.UpdateCacheSettings(response);
               return response;
            }
         }
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      private static Stream ToStream(SvgDocument svgDocument, bool gzip)
      {
         var ms = new MemoryStream();

         var svgSaveOptions = new SvgSaveOptions();

         if (gzip)
         {
            using (var compressed = new GZipStream(ms, CompressionMode.Compress, true))
            {
               // unfortunately svgDocument.SaveToStream wants to read the stream current position
               // and GZipStream.Position is not supported
               // svgDocument.SaveToStream(compressed, new SvgSaveOptions() { });

               // Save to a temp stream first
               using (var tempStream = new MemoryStream())
               {
                  svgDocument.SaveToStream(tempStream, svgSaveOptions);
                  tempStream.Position = 0;
                  ServiceHelper.CopyStream(tempStream, compressed);
               }
            }
         }
         else
         {
            svgDocument.SaveToStream(ms, svgSaveOptions);
         }

         ms.Position = 0;
         return ms;
      }

      /// <summary>
      /// Gets the page image as an SVG.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document's page image could not be loaded")]
      /*
       * To support POST, use the below lines instead:
       * // [HttpPost, AlwaysCorsFilter]
       * // public HttpResponseMessage GetSvg(GetSvgRequest request)
       */
      [HttpGet, AlwaysCorsFilter]
      public HttpResponseMessage GetSvg([FromUri] GetSvgRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         int pageNumber = request.PageNumber;

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId must not be null");

         if (pageNumber < 0)
            throw new ArgumentException("'pageNumber' must be a value greater than or equals to 0");

         // Default is page 1
         if (pageNumber == 0)
            pageNumber = 1;

         // Now load the document
         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            DocumentHelper.CheckLoadFromCache(document);
            DocumentHelper.CheckPageNumber(document, pageNumber);

            document.Images.UnembedSvgImages = request.UnembedImages;

            var documentPage = document.Pages[pageNumber - 1];
            var loadOptions = new CodecsLoadSvgOptions();
            loadOptions.ForceTextPath = (request.Options & DocumentGetSvgOptions.ForceTextPath) == DocumentGetSvgOptions.ForceTextPath;
            loadOptions.ForceRealText = (request.Options & DocumentGetSvgOptions.ForceRealText) == DocumentGetSvgOptions.ForceRealText;
            loadOptions.DropImages = (request.Options & DocumentGetSvgOptions.DropImages) == DocumentGetSvgOptions.DropImages;
            loadOptions.DropShapes = (request.Options & DocumentGetSvgOptions.DropShapes) == DocumentGetSvgOptions.DropShapes;
            loadOptions.DropText = (request.Options & DocumentGetSvgOptions.DropText) == DocumentGetSvgOptions.DropText;
            loadOptions.ForConversion = (request.Options & DocumentGetSvgOptions.ForConversion) == DocumentGetSvgOptions.ForConversion;
            loadOptions.IgnoreXmlParsingErrors = (request.Options & DocumentGetSvgOptions.IgnoreXmlParsingErrors) == DocumentGetSvgOptions.IgnoreXmlParsingErrors;

            using (var svgDocument = documentPage.GetSvg(loadOptions))
            {
               if (svgDocument != null)
               {
                  if (!svgDocument.IsFlat)
                     svgDocument.Flat(null);

                  if (!svgDocument.IsRenderOptimized)
                     svgDocument.BeginRenderOptimize();

                  var svgBounds = svgDocument.Bounds;
                  if (!svgBounds.IsValid)
                     svgDocument.CalculateBounds(false);
               }

               if (svgDocument != null)
               {
                  var gzip = ServiceHelper.GetSettingBoolean(ServiceHelper.Key_Svg_GZip);
                  var stream = ToStream(svgDocument, gzip);

                  // HttpContext is Web Api's version of WebOperationContext
                  //var currentContext = WebOperationContext.Current;
                  var currentContext = HttpContext.Current;
                  if (currentContext != null)
                  {
                     if (gzip)
                        currentContext.Response.Headers.Add("Content-Encoding", "gzip");

                     currentContext.Response.ContentType = "image/svg+xml";
                     currentContext.Response.Headers.Add("ContentLength", stream.Length.ToString());
                  }

                  // If we just return the stream, Web Api will try to serialize it.
                  // If the return type is "HttpResponseMessage" it will not serialize
                  // and you can set the content as you wish.
                  var response = new HttpResponseMessage();
                  response.Content = new StreamContent(stream);
                  ServiceHelper.UpdateCacheSettings(response);
                  return response;
               }
               else
               {
                  return null;
               }
            }
         }
      }

      /// <summary>
      ///   Retrieves the text for this document page, potentially through OCR.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document page's text could not be retrieved")]
      [HttpPost]
      public GetTextResponse GetText(GetTextRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var pageNumber = request.PageNumber;

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId must not be null");

         if (pageNumber < 0)
            throw new ArgumentException("'pageNumber' must be a value greater than or equal to 0");

         // Default is page 1
         if (pageNumber == 0)
            pageNumber = 1;

         IOcrEngine ocrEngine = null;

         try
         {
            // Now load the document
            var cache = ServiceHelper.Cache;
            using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
            {
               DocumentHelper.CheckLoadFromCache(document);
               DocumentHelper.CheckPageNumber(document, pageNumber);

               document.Text.TextExtractionMode = request.TextExtractionMode;

               var documentPage = document.Pages[pageNumber - 1];

               if (document.Text.TextExtractionMode != DocumentTextExtractionMode.OcrOnly && !document.Images.IsSvgSupported)
               {
                  ocrEngine = ServiceHelper.GetOCREngine();
                  if (ocrEngine != null)
                     document.Text.OcrEngine = ocrEngine;
               }

               var pageText = documentPage.GetText(request.Clip);
               return new GetTextResponse { PageText = pageText };
            }
         }
         catch (Exception ex)
         {
            Trace.WriteLine(string.Format("GetText - Error:{1}{0}documentId:{2} pageNumber:{3}", Environment.NewLine, ex.Message, request.DocumentId, pageNumber), "Error");
            throw;
         }
      }

      /// <summary>
      /// Reads any barcodes that may exist on this page.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The barcodes for the page could not be read")]
      [HttpPost]
      public ReadBarcodesResponse ReadBarcodes(ReadBarcodesRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var pageNumber = request.PageNumber;

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId must not be null");

         if (pageNumber < 0)
            throw new ArgumentException("'pageNumber' must be a value greater than or equal to 0");

         // Default is page 1
         if (pageNumber == 0)
            pageNumber = 1;

         try
         {
            // Now load the document
            var cache = ServiceHelper.Cache;
            using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
            {
               DocumentHelper.CheckLoadFromCache(document);
               DocumentHelper.CheckPageNumber(document, pageNumber);

               // Set the options
               var barcodeEngine = new BarcodeEngine();
               document.Barcodes.BarcodeEngine = barcodeEngine;
               var barcodeReader = barcodeEngine.Reader;

               // Get the symbologies to read
               var symbologies = new List<BarcodeSymbology>();
               if (request.Symbologies != null && request.Symbologies.Length > 0)
                  symbologies.AddRange(request.Symbologies);
               else
                  symbologies.AddRange(barcodeReader.GetAvailableSymbologies());

               // Load the options from config
               bool usedCustomOptions = ServiceHelper.SetBarcodeReadOptions(barcodeReader);
               if (!usedCustomOptions)
                  ServiceHelper.InitBarcodeReader(barcodeReader, false);

               var documentPage = document.Pages[pageNumber - 1];
               var barcodes = documentPage.ReadBarcodes(request.Bounds, request.MaximumBarcodes, symbologies.ToArray());
               if (barcodes.Length == 0 && !usedCustomOptions)
               {
                  // Did not find any barcodes, try again with double pass enabled
                  ServiceHelper.InitBarcodeReader(barcodeReader, true);

                  // Do not read MicroPDF417 in this pass since it is too slow
                  if (symbologies != null && symbologies.Contains(BarcodeSymbology.MicroPDF417))
                     symbologies.Remove(BarcodeSymbology.MicroPDF417);

                  // Try again
                  barcodes = documentPage.ReadBarcodes(request.Bounds, request.MaximumBarcodes, symbologies.ToArray());
               }

               // If we found any barcodes, parse the ECI data if available
               foreach (var barcode in barcodes)
               {
                  if (barcode.Symbology == BarcodeSymbology.QR || barcode.Symbology == BarcodeSymbology.MicroQR)
                  {
                     string eciData = BarcodeData.ParseECIData(barcode.GetData());
                     if (!string.IsNullOrEmpty(eciData))
                     {
                        barcode.Value = eciData;
                     }
                  }
               }

               return new ReadBarcodesResponse { Barcodes = barcodes };
            }
         }
         catch (Exception ex)
         {
            Trace.WriteLine(string.Format("ReadBarcodes - Error:{1}{0}documentId:{2} pageNumber:{3}", Environment.NewLine, ex.Message, request.DocumentId, pageNumber), "Error");
            throw;
         }
      }

      /// <summary>
      /// Gets any annotations that may exist on this page.
      /// </summary>
      // Also applies to Document.Annotations.GetAnnotations
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The annotations for the page could not be retrieved")]
      [HttpPost]
      public GetAnnotationsResponse GetAnnotations(GetAnnotationsRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         return AnnotationMethods.GetAnnotations(request);
      }

      /// <summary>
      /// Sets the annotations for this page.
      /// </summary>
      // Also applies to Document.Annotations.SetAnnotations
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The annotations for the page could not be set")]
      [HttpPost]
      public Response SetAnnotations(SetAnnotationsRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         return AnnotationMethods.SetAnnotations(request);
      }
   }
}

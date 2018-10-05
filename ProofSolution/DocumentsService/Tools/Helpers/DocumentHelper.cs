// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Leadtools;
using Leadtools.Document;
using Leadtools.Document.Converter;
using Leadtools.Document.Writer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Leadtools.Services.Tools.Helpers
{
   internal static class DocumentHelper
   {
      public static void CheckLoadFromCache(LEADDocument document)
      {
         if (document == null)
            throw new InvalidOperationException("Document was not found in the cache");
      }

      public static void CheckCacheInfo(DocumentCacheInfo cacheInfo)
      {
         if (cacheInfo == null)
            throw new InvalidOperationException("Document was not found in the cache");
      }

      public static void CheckPageNumber(LEADDocument document, int pageNumber)
      {
         if (pageNumber > document.Pages.Count)
            throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "Must be a value between 1 and " + document.Pages.Count);
      }

      public static void DeleteDocument(string documentId, bool preventIfPreCached, bool throwIfNull)
      {
         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, documentId))
         {
            if (throwIfNull)
               DocumentHelper.CheckLoadFromCache(document);

            if (document != null)
            {
               // Check if it's one of our pre-cached documents. 
               // If it is, don't remove it from the cache.
               if (PreCacheHelper.PreCacheExists && document.Uri != null)
               {
                  if (preventIfPreCached && PreCacheHelper.CheckDocument(document.Uri, document.Images.MaximumImagePixelSize) != null)
                     return;
                  else
                     PreCacheHelper.RemoveDocument(document.Uri, new int[] { document.Images.MaximumImagePixelSize });
               }

               document.AutoDeleteFromCache = true;
               // But not the children documents (if any)
               foreach (var child in document.Documents)
               {
                  child.AutoDeleteFromCache = false;
               }
               document.AutoDisposeDocuments = true;
            }
         }
      }
   }

   [Serializable]
   [DataContract]
   public class ServiceDocumentConverterJobData
   {
      public ServiceDocumentConverterJobData()
      {
         JobErrorMode = DocumentConverterJobErrorMode.Continue;
         PageNumberingTemplate = "##name##_Page(##page##).##extension##";
         EnableSvgConversion = true;
         SvgImagesRecognitionMode = DocumentConverterSvgImagesRecognitionMode.Auto;
      }

      [DataMember(Name = "jobErrorMode")]
      public DocumentConverterJobErrorMode JobErrorMode { get; set; }

      [DataMember(Name = "pageNumberingTemplate")]
      public string PageNumberingTemplate { get; set; }

      [DataMember(Name = "enableSvgConversion")]
      public bool EnableSvgConversion { get; set; }

      [DataMember(Name = "svgImagesRecognitionMode")]
      public DocumentConverterSvgImagesRecognitionMode SvgImagesRecognitionMode { get; set; }

      [DataMember(Name = "emptyPageMode")]
      public DocumentConverterEmptyPageMode EmptyPageMode { get; set; }

      [DataMember(Name = "preprocessorDeskew")]
      public bool PreprocessorDeskew { get; set; }

      [DataMember(Name = "preprocessorOrient")]
      public bool PreprocessorOrient { get; set; }

      [DataMember(Name = "preprocessorInvert")]
      public bool PreprocessorInvert { get; set; }

      [DataMember(Name = "inputDocumentFirstPageNumber")]
      public int InputDocumentFirstPageNumber { get; set; }

      [DataMember(Name = "inputDocumentLastPageNumber")]
      public int InputDocumentLastPageNumber { get; set; }

      [DataMember(Name = "documentFormat")]
      public DocumentFormat DocumentFormat { get; set; }

      [DataMember(Name = "rasterImageFormat")]
      public RasterImageFormat RasterImageFormat { get; set; }

      [DataMember(Name = "rasterImageBitsPerPixel")]
      public int RasterImageBitsPerPixel { get; set; }

      // We deserialize this field manually from JSON Object to a specific DocumentOptions type
      [DataMember(Name = "documentOptions")]
      public JObject DocumentOptions { get; set; }

      [DataMember(Name = "jobName")]
      public string JobName { get; set; }

      [DataMember(Name = "annotationsMode")]
      public DocumentConverterAnnotationsMode AnnotationsMode { get; set; }

      [DataMember(Name = "documentName")]
      public string DocumentName { get; set; }

      [DataMember(Name = "outputDocumentId")]
      public string OutputDocumentId { get; set; }

      [DataMember(Name = "annotations")]
      public string Annotations { get; set; }
   }
}

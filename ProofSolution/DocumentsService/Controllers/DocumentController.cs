// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Net;
using System.Web.Http;

using Leadtools.Services.Models.Document;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.Document;
using Leadtools.DocumentViewer.Tools.Helpers;
using Leadtools.Services.Tools.Helpers;
using System.Collections.Generic;
using System.Diagnostics;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used with the LEADDocument class of the LEADTOOLS Document JavaScript library.
   /// </summary>
   public class DocumentController : ApiController
   {
      public DocumentController()
      {
         ServiceHelper.InitializeController();
      }

      /// <summary>
      /// Returns a decrypted version of the document when passed the correct password, or throws an exception.
      /// </summary>
      /// <param name="request">A <see cref="DecryptRequest">DecryptRequest</see> containing an identifier and password.</param>
      /// <returns>A <see cref="DecryptResponse">DecryptResponse</see> containing all new document information.</returns>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document's decryption failed")]
      [HttpPost]
      public DecryptResponse Decrypt(DecryptRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         if (request.DocumentId == null)
            throw new ArgumentException("documentId must not be null");

         var cache = ServiceHelper.Cache;

         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            DocumentHelper.CheckLoadFromCache(document);

            if (!document.Decrypt(request.Password))
               throw new ServiceException("Incorrect Password", HttpStatusCode.Forbidden);

            document.SaveToCache();
            return new DecryptResponse { Document = document };
         }
      }

      /// <summary>
      /// Runs the conversion specified by the conversion job data on the document, and stores the result to the cache.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document could not be converted")]
      [HttpPost]
      public ConvertResponse Convert(ConvertRequest request)
      {
         return ConverterHelper.Convert(request.DocumentId, request.JobData);
      }

      // For debugging - if true, document history will be logged to the console
      private static bool _loggingDocumentHistory = false;

      /// <summary>
      /// Retrieves changes to the document since the history was last cleared.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document history could not be retrieved")]
      [HttpPost]
      public GetHistoryResponse GetHistory(GetHistoryRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         // Must have the documentId you'd like to add annotations to.
         // If you only have the document cache URI, DocumentFactory.LoadFromUri needs to be called.
         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentNullException("documentId");

         IList<DocumentHistoryItem> items = null;

         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            // Ensure we have the document.
            DocumentHelper.CheckLoadFromCache(document);

            DocumentHistory history = document.History;
            items = history.GetItems();

            if (items != null && request.ClearHistory)
            {
               history.Clear();
               document.SaveToCache();
            }
         }

         if (_loggingDocumentHistory)
            ShowHistory(request.DocumentId, items);

         DocumentHistoryItem[] itemsArray = new DocumentHistoryItem[items.Count];
         items.CopyTo(itemsArray, 0);
         GetHistoryResponse response = new GetHistoryResponse
         {
            Items = itemsArray
         };
         return response;
      }

      private static void ShowHistory(string documentId, IList<DocumentHistoryItem> items)
      {
         if (items == null || items.Count == 0)
            return;

         Trace.WriteLine(string.Format("History for document '{0}'", documentId));
         foreach (DocumentHistoryItem item in items)
         {
            Trace.WriteLine(string.Format("   User: '{0}' Timestamp: {1} Comment: '{2}' Change: '{3}' PageNumber: {4}",
               item.UserId != null ? item.UserId : "[null]",
               item.Timestamp,
               item.Comment != null ? item.Comment : "[null]",
               GetName(item.ModifyType),
               item.PageNumber));
         }
      }

      private static string GetName(DocumentHistoryModifyType value)
      {
         switch (value)
         {
            case DocumentHistoryModifyType.Created: return "Created";
            case DocumentHistoryModifyType.Decrypted: return "Decrypted";
            case DocumentHistoryModifyType.Pages: return "Pages";
            case DocumentHistoryModifyType.PageViewPerspective: return "Page ViewPerspective";
            case DocumentHistoryModifyType.PageAnnotations: return "Page Annotations";
            case DocumentHistoryModifyType.PageMarkDeleted: return "Page MarkDeleted";
            case DocumentHistoryModifyType.PageImage: return "Page Image";
            case DocumentHistoryModifyType.PageSvgBackImage: return "Page SvgBackImage";
            case DocumentHistoryModifyType.PageSvg: return "Page Svg";
            case DocumentHistoryModifyType.PageText: return "Page Text";
            case DocumentHistoryModifyType.PageLinks: return "Page Links";
            default: return "Unknown";
         }
      }
   }
}

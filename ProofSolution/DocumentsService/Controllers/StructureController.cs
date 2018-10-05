// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Leadtools.DocumentViewer.Models.Structure;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.DocumentViewer.Tools.Helpers;
using Leadtools.Services.Tools.Helpers;
using Leadtools.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used with the DocumentStructure class of the LEADTOOLS Document JavaScript library.
   /// </summary>
   public class StructureController : ApiController
   {
      public StructureController()
      {
         ServiceHelper.InitializeController();
      }

      /// <summary>
      /// Parses the structure of the document; only needs to be called once per document.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The document's structure could not be parsed", MethodName = "Parse")]
      [HttpPost]
      public ParseStructureResponse ParseStructure(ParseStructureRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentException("documentId must not be null");

         // Now load the document
         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            DocumentHelper.CheckLoadFromCache(document);

            if (!document.IsStructureSupported)
            {
               return new ParseStructureResponse();
            }

            if (!document.Structure.IsParsed)
            {
               document.Structure.ParseBookmarks = request.ParseBookmarks;
               document.Structure.ParsePageLinks = request.ParsePageLinks;
               document.Structure.Parse();
               document.SaveToCache();
            }

            var pageLinks = new List<DocumentLink[]>();
            var bookmarks = new List<DocumentBookmark>();

            bookmarks.AddRange(document.Structure.Bookmarks);

            foreach (var page in document.Pages)
            {
               var links = page.GetLinks();
               pageLinks.Add(links);
            }

            return new ParseStructureResponse
            {
               Bookmarks = bookmarks.ToArray(),
               PageLinks = pageLinks.ToArray()
            };
         }
      }
   }
}

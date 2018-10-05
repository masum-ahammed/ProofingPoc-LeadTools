// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Leadtools.Services.Models;

namespace Leadtools.DocumentViewer.Models.Images
{
   [DataContract]
   public class GetThumbnailsGridRequest : Request
   {
      /// <summary>
      /// The document to get thumbnails from.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// The first page number to use.
      /// </summary>
      [DataMember(Name = "firstPageNumber")]
      public int FirstPageNumber { get; set; }

      /// <summary>
      /// The last page number to use.
      /// </summary>
      [DataMember(Name = "lastPageNumber")]
      public int LastPageNumber { get; set; }

      /// <summary>
      /// Optional - the mimetype to use for the images.
      /// Not currently sent by the Leadtools.Document.JavaScript library.
      /// </summary>
      [DataMember(Name = "mimeType")]
      public string MimeType { get; set; }

      /// <summary>
      /// The maximum width for the grid.
      /// </summary>
      [DataMember(Name = "maximumGridWidth")]
      public int MaximumGridWidth { get; set; }

      /// <summary>
      /// The actual width for the grid.
      /// </summary>
      [DataMember(Name = "width")]
      public int Width { get; set; }

      /// <summary>
      /// The actual height for the grid.
      /// </summary>
      [DataMember(Name = "height")]
      public int Height { get; set; }
   }
}

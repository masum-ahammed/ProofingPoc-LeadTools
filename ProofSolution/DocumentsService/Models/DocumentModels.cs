// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Leadtools.Services.Tools.Helpers;
using Leadtools.Services.Models;
using Leadtools.Document;

namespace Leadtools.Services.Models.Document
{
   [DataContract]
   public class DecryptRequest : Request
   {
      /// <summary>
      /// The document's identification number.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// The attempted password for the document.
      /// </summary>
      [DataMember(Name = "password")]
      public string Password { get; set; }
   }
   [DataContract]
   public class DecryptResponse : Response
   {
      /// <summary>
      /// The decrypted document's information.
      /// </summary>
      [DataMember(Name = "document")]
      public LEADDocument Document { get; set; }
   }

   [DataContract]
   public class ConvertRequest : Request
   {
      /// <summary>
      /// The ID of the document to convert.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// The relevant job data that will be used to understand the conversion needed.
      /// </summary>
      [DataMember(Name = "jobData")]
      public ServiceDocumentConverterJobData JobData { get; set; }
   }

   [DataContract]
   public class ConvertItem
   {
      /// <summary>
      /// The user-friendly name of the item being returned, for downloading.
      /// </summary>
      [DataMember(Name = "name")]
      public string Name { get; set; }

      /// <summary>
      /// The URL of the item being returned, for downloading.
      /// </summary>
      [DataMember(Name = "url")]
      public Uri Url { get; set; }

      /// <summary>
      /// The mimetype of the converted item.
      /// </summary>
      [DataMember(Name = "mimeType")]
      public string MimeType { get; set; }

      /// <summary>
      /// The file length, in bytes, of the converted item.
      /// </summary>
      [DataMember(Name = "length")]
      public long Length { get; set; }
   }

   [DataContract]
   public class ConvertResponse : Response
   {
      /// <summary>
      /// The cache ID of the item (only used for documents)
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// If the item had to be served as one archived file, it exists here.
      /// </summary>
      [DataMember(Name = "archive")]
      public ConvertItem Archive { get; set; }

      /// <summary>
      /// The newly converted document, if it was not archived.
      /// </summary>
      [DataMember(Name = "document")]
      public ConvertItem Document { get; set; }

      /// <summary>
      /// The converted annotations, if not archived.
      /// </summary>
      [DataMember(Name = "annotations")]
      public ConvertItem Annotations { get; set; }
   }

   [DataContract]
   public class GetHistoryRequest : Request
   {
      /// <summary>
      /// The ID of the document to get the history of.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// Whether or not to clear the history after retrieving it.
      /// </summary>
      [DataMember(Name = "clearHistory")]
      public bool ClearHistory { get; set; }
   }

   [DataContract]
   public class GetHistoryResponse : Response
   {
      /// <summary>
      /// The document history items.
      /// </summary>
      [DataMember(Name = "items")]
      public DocumentHistoryItem[] Items { get; set; }
   }
}

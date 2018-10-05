// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

using Leadtools.Document;
using Leadtools.Caching;
using Leadtools.Services.Models;

namespace Leadtools.DocumentViewer.Models.Factory
{
   [DataContract]
   public class LoadFromCacheRequest : Request
   {
      /// <summary>
      /// The ID to load from the cache (which must be retrieved from an item after LoadFromUri was called, and saved).
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }
   }

   [DataContract]
   public class LoadFromCacheResponse : Response
   {
      /// <summary>
      /// The serialized LEADDocument instance.
      /// </summary>
      [DataMember(Name = "document")]
      public LEADDocument Document { get; set; }
   }

   [DataContract]
   public class LoadFromUriRequest : Request
   {
      /// <summary>
      /// The options to use when loading this document (serialized Leadtools.Document.LoadDocumentOptions instance).
      /// </summary>
      [DataMember(Name = "options")]
      public LoadDocumentOptions Options { get; set; }

      /// <summary>
      /// The URI to the document to be loaded.
      /// </summary>
      [DataMember(Name = "uri")]
      public Uri Uri { get; set; }

      /// <summary>
      ///   The resolution to load the document at. To use the default, pass 0.
      /// </summary>
      [DataMember(Name = "resolution")]
      public int Resolution { get; set; }
   }

   [DataContract]
   public class LoadFromUriResponse : Response
   {
      /// <summary>
      /// The serialized LEADDocument instance.
      /// </summary>
      [DataMember(Name = "document")]
      public LEADDocument Document { get; set; }
   }

   [DataContract]
   public class BeginUploadRequest : Request
   {
      /// <summary>
      /// The ID to use for the new document, or null to create a new random DocumentId.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// The options to use for uploading the document.
      /// </summary>
      [DataMember(Name = "options")]
      public UploadDocumentOptions Options { get; set; }
   }

   [DataContract]
   public class BeginUploadResponse : Response
   {
      /// <summary>
      /// The URI to which the document should be uploaded to the cache.
      /// </summary>
      [DataMember(Name = "uploadUri")]
      public Uri UploadUri { get; set; }
   }

   [DataContract]
   public class UploadDocumentRequest : Request
   {
      /// <summary>
      /// The uri, retrieved from BeginUpload, that is used for uploading.
      /// </summary>
      [DataMember(Name = "uri")]
      public Uri Uri { get; set; }

      /// <summary>
      /// The data to upload.
      /// </summary>
      [DataMember(Name = "data")]
      public string Data { get; set; }
   }

   [DataContract]
   public class EndUploadRequest : Request
   {
      /// <summary>
      /// The uri, retrieved from BeginUpload, that is used for uploading.
      /// </summary>
      [DataMember(Name = "uri")]
      public Uri Uri { get; set; }
   }

   [DataContract]
   public class AbortUploadDocumentRequest : Request
   {
      /// <summary>
      /// The URI from BeginUpload to stop loading to.
      /// </summary>
      [DataMember(Name = "uri")]
      public Uri Uri { get; set; }
   }

   [DataContract]
   public class GetCacheStatisticsResponse : Response
   {
      /// <summary>
      /// The cache statistics information (serialized Leadtools.Caching.CacheStatistics)
      /// </summary>
      [DataMember(Name = "statistics")]
      public CacheStatistics Statistics { get; set; }
   }

   [DataContract]
   public class SaveToCacheRequest : Request
   {
      /// <summary>
      /// The data to use when creating or saving this document - a serialized instance of Leadtools.Document.DocumentDescriptor.
      /// </summary>
      [DataMember(Name = "descriptor")]
      public DocumentDescriptor Descriptor { get; set; }
   }

   [DataContract]
   public class SaveToCacheResponse : Response
   {
      /// <summary>
      /// The serialized LEADDocument instance.
      /// </summary>
      [DataMember(Name = "document")]
      public LEADDocument Document { get; set; }
   }

   [DataContract]
   public class DeleteRequest : Request
   {
      /// <summary>
      /// The document to delete.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// Do not throw an exception if the document does not exist in the cache
      /// </summary>
      [DataMember(Name = "allowNonExisting")]
      public bool AllowNonExisting { get; set; }
   }

   [DataContract]
   public class CheckCacheInfoRequest : Request
   {
      /// <summary>
      /// The URI to the document to verify the mimetype for. This may be a cache URI.
      /// </summary>
      [DataMember(Name = "uri")]
      public Uri Uri { get; set; }
   }

   [DataContract]
   public class CacheInfo
   {
      /// <summary>
      /// Whether or not the document is virtual.
      /// </summary>
      [DataMember(Name = "isVirtual")]
      public bool IsVirtual { get; set; }

      /// <summary>
      /// Whether or not the document is loaded already.
      /// </summary>
      [DataMember(Name = "isLoaded")]
      public bool IsLoaded { get; set; }

      /// <summary>
      /// Whether or not the document has annotations.
      /// </summary>
      [DataMember(Name = "hasAnnotations")]
      public bool HasAnnotations { get; set; }

      /// <summary>
      /// The document name, if one is set.
      /// </summary>
      [DataMember(Name = "name")]
      public string Name { get; set; }

      /// <summary>
      /// The reported mimeType of the document.
      /// </summary>
      [DataMember(Name = "mimeType")]
      public string MimeType { get; set; }

      /// <summary>
      /// Whether or not the mimeType is acceptable.
      /// </summary>
      [DataMember(Name = "isMimeTypeAccepted")]
      public bool IsMimeTypeAccepted { get; set; }

      /// <summary>
      /// The page count of the document.
      /// </summary>
      [DataMember(Name = "pageCount")]
      public int PageCount { get; set; }
   }

   [DataContract]
   public class CheckCacheInfoResponse : Response
   {
      /// <summary>
      /// The cache info for the document. If null, the document does not exist.
      /// </summary>
      [DataMember(Name = "cacheInfo")]
      public CacheInfo CacheInfo { get; set; }
   }

   [DataContract]
   public class DownloadAnnotationsRequest : Request
   {
      /// <summary>
      /// The ID of the annotations in the cache to download. Cannot be used if URI is used.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// The URI to the annotations to download. Cannot be used if ID is used.
      /// This may be a cache URI.
      /// </summary>
      [DataMember(Name = "uri")]
      public Uri Uri { get; set; }
   }

   [DataContract]
   public class DownloadDocumentRequest : Request
   {
      /// <summary>
      /// The ID of the document in the cache to download. Cannot be used if URI is used.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// The URI to the document to download. Cannot be used if ID is used.
      /// This may be a cache URI.
      /// </summary>
      [DataMember(Name = "uri")]
      public Uri Uri { get; set; }

      /// <summary>
      /// If true, external annotations will be returned as well (the result will be a ZIP).
      /// </summary>
      [DataMember(Name = "includeAnnotations")]
      public bool IncludeAnnotations { get; set; }
   }
}

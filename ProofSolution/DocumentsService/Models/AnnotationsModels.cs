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

namespace Leadtools.DocumentViewer.Models.Annotations
{
   [DataContract]
   public class SetAnnotationsIBMRequest : Request
   {
      /// <summary>
      /// The ID of the document to set annotations to.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }

      /// <summary>
      /// The annotation objects to convert.
      /// </summary>
      [DataMember(Name = "annotations")]
      public IBMAnnotation[] Annotations { get; set; }
   }

   [DataContract]
   public class IBMAnnotation
   {
      /// <summary>
      /// The IBM Annotation as an XML string.
      /// </summary>
      [DataMember(Name = "annotation")]
      public string Annotation { get; set; }

      /// <summary>
      /// The password to lock the object with. If null or empty, it will not be used.
      /// </summary>
      [DataMember(Name = "password")]
      public string Password { get; set; }

      /// <summary>
      /// The user ID to associate with this IBM Annotation.
      /// </summary>
      [DataMember(Name = "userId")]
      public string UserId { get; set; }
   }

   [DataContract]
   public class SetAnnotationsIBMResponse : Response
   {
   }

   [DataContract]
   public class GetAnnotationsIBMRequest : Request
   {
      /// <summary>
      /// The ID of the document to get annotations history from.
      /// </summary>
      [DataMember(Name = "documentId")]
      public string DocumentId { get; set; }
   }

   [DataContract]
   public class GetAnnotationsIBMResponse : Response
   {
      /// <summary>
      /// Dictionary of added IBM-P8 annotations (where the key is the annotation guid).
      /// </summary>
      [DataMember(Name = "added")]
      public Dictionary<String, String> Added { get; set; }

      /// <summary>
      /// Dictionary of modified IBM-P8 annotations (where the key is the annotation guid).
      /// </summary>
      [DataMember(Name = "modified")]
      public Dictionary<String, String> Modified { get; set; }

      /// <summary>
      /// Dictionary of deleted IBM-P8 annotations (where the key is the annotation guid and value is null).
      /// </summary>
      [DataMember(Name = "deleted")]
      public Dictionary<String, String> Deleted { get; set; }
   }
}

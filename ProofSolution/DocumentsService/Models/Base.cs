// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System.Runtime.Serialization;

namespace Leadtools.Services.Models
{
   /// <summary>
   /// A base request type that all other requests derive from.
   /// </summary>
   [DataContract]
   public class Request
   {
      /// <summary>
      /// Any arbitrary data the user may wish to pass along through the LEADTOOLS Document library to the service.
      /// </summary>
      [DataMember(Name = "userData")]
      public string UserData { get; set; }
   }

   /// <summary>
   /// A base response type that all other responses derive from.
   /// </summary>
   [DataContract]
   public class Response
   {
      /// <summary>
      /// Any arbitrary data the user may wish to pass along from the service through the LEADTOOLS Document library.
      /// </summary>
      [DataMember(Name = "userData")]
      public string UserData { get; set; }
   }
}

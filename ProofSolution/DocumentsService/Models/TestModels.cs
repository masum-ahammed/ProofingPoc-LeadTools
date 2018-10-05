// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Runtime.Serialization;
using Leadtools.Services.Models;

namespace Leadtools.DocumentViewer.Models.Test
{
   [DataContract]
   public class PingResponse : Response
   {
      /// <summary>
      /// A simple message, for testing.
      /// </summary>
      [DataMember(Name = "message")]
      public string Message { get; set; }

      /// <summary>
      /// The current time, so the user may tell if it was cached.
      /// </summary>
      [DataMember(Name = "time")]
      public DateTime Time { get; set; }

      /// <summary>
      /// Whether or not the license was able to be checked.
      /// </summary>
      [DataMember(Name = "isLicenseChecked")]
      public bool IsLicenseChecked { get; set; }

      /// <summary>
      /// Whether or not the license is expired.
      /// </summary>
      [DataMember(Name = "isLicenseExpired")]
      public bool IsLicenseExpired { get; set; }

      /// <summary>
      /// The type of kernel - evaluation, for example.
      /// </summary>
      [DataMember(Name = "kernelType")]
      public string KernelType { get; set; }

      /// <summary>
      /// Whether the cache was accessed successfully.
      /// </summary>
      [DataMember(Name = "isCacheAccessible")]
      public bool IsCacheAccessible { get; set; }

      /// <summary>
      /// The value of the OCREngineStatus enum indicating the OCR Engine Status.
      /// </summary>
      [DataMember(Name = "ocrEngineStatus")]
      public int OcrEngineStatus { get; set; }

      /// <summary>
      /// The service version.
      /// </summary>
      [DataMember(Name = "serviceVersion")]
      public string ServiceVersion { get; set; }

      /// <summary>
      /// The kernel version.
      /// </summary>
      [DataMember(Name = "kernelVersion")]
      public string KernelVersion { get; set; }

      /// <summary>
      /// The multi-platform support.
      /// </summary>
      [DataMember(Name = "multiplatformSupportStatus")]
      public string MultiplatformSupportStatus { get; set; }
   }

   [DataContract]
   public class HeartbeatResponse : Response
   {
      /// <summary>
      /// The current time, so the user may tell if it was cached.
      /// </summary>
      [DataMember(Name = "time")]
      public DateTime Time { get; set; }
   }
}

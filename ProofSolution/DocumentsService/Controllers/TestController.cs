// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Leadtools;

using Leadtools.Services.Tools.Exceptions;
using Leadtools.Services.Models;
using Leadtools.DocumentViewer.Models.Test;
using Leadtools.Services.Tools.Helpers;
using System.Runtime.Serialization;
using System.Reflection;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used with the DocumentFactory class of the LEADTOOLS Document JavaScript library.
   /// </summary>
   public class TestController : ApiController
   {
      /* This Ping() method is used to detect that everything is working fine
       * before a demo begins. Otherwise, errors from loading the initial document
       * may tell the wrong story because the user hasn't set up the service yet.
       */

      /// <summary>
      ///   Pings the service to ensure a connection, returning data about the status of the LEADTOOLS license.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The service is not available", MethodName = "VerifyService")]
      [HttpPost]
      [ActionName("Ping")]
      public PingResponse PostPing(Request request)
      {
         return Ping(request);
      }

      /// <summary>
      ///   Pings the service to ensure a connection, returning data about the status of the LEADTOOLS license.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The service is not available", MethodName = "VerifyService")]
      [HttpGet]
      [ActionName("Ping")]
      public PingResponse GetPing([FromUri] Request request)
      {
         return Ping(request);
      }

      [NonAction]
      public PingResponse Ping(Request request)
      {
         Trace.WriteLine("Leadtools Documents Service: Ready");

         var response = new PingResponse();
         response.Time = DateTime.Now;
         bool ready = true;

         // Get the Service Version
         var serviceAssembly = typeof(TestController).Assembly;
         var serviceFileVersionInfo = FileVersionInfo.GetVersionInfo(serviceAssembly.Location);
         response.ServiceVersion = serviceFileVersionInfo.FileVersion;

         // Get Kernel Version
         try
         {
            var leadAssembly = typeof(RasterImage).Assembly;
            var leadFileVersionInfo = FileVersionInfo.GetVersionInfo(leadAssembly.Location);
            response.KernelVersion = leadFileVersionInfo.FileVersion.Replace(",", ".");
         }
         catch
         {
            ready = false;
         }

         Trace.WriteLine("Getting Toolkit status");
         response.IsLicenseChecked = ServiceHelper.IsLicenseChecked;
         response.IsLicenseExpired = ServiceHelper.IsKernelExpired;
         if (response.IsLicenseChecked)
            response.KernelType = RasterSupport.KernelType.ToString().ToUpper();
         else
            response.KernelType = null;

         if (response.IsLicenseExpired || !response.IsLicenseChecked)
            ready = false;

         try
         {
            ServiceHelper.CheckCacheAccess();
            response.IsCacheAccessible = true;
         }
         catch (Exception)
         {
            response.IsCacheAccessible = false;
            ready = false;
         }

         // Add OCR Status
         response.OcrEngineStatus = (int)ServiceHelper.OcrEngineStatus;
         response.MultiplatformSupportStatus = ServiceHelper.MultiplatformSupportStatus;
         response.Message = ready ? "Ready" : "Not Ready";

         return response;
      }

      /// <summary>
      ///   Pings the service to ensure a connection.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The service is not available", MethodName = "Heartbeat")]
      [HttpPost]
      [ActionName("Heartbeat")]
      public HeartbeatResponse PostHeartbeat(Request request)
      {
         return new HeartbeatResponse
         {
            Time = DateTime.Now
         };
      }

      /// <summary>
      ///   Pings the service to ensure a connection.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The service is not available", MethodName = "Heartbeat")]
      [HttpGet]
      [ActionName("Heartbeat")]
      public HeartbeatResponse GetHeartbeat([FromUri] Request request)
      {
         return new HeartbeatResponse
         {
            Time = DateTime.Now
         };
      }

      /// <summary>
      /// A test method, not used, to show the use of "userData".
      /// </summary>
      // Modify and return user data
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The user data could not be accessed")]
      [HttpPost]
      public Response CheckUserData(Request request)
      {
         var userData = request.UserData;
         object newUserData = new ReturnUserDataObject()
         {
            Data = userData,
            Message = "Welcome to the Documents Service: " + DateTime.Now.ToLongTimeString()
         };
         return new Response
         {
            UserData = Newtonsoft.Json.JsonConvert.SerializeObject(newUserData)
         };
      }

      [DataContract]
      internal class ReturnUserDataObject
      {
         [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
         [DataMember]
         public string Data { get; set; }

         [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
         [DataMember]
         public string Message { get; set; }
      }
   }
}

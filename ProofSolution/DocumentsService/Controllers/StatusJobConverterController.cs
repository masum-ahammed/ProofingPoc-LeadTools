// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Web.Http;

using Leadtools.Services.Tools.Exceptions;
using Leadtools.Services.Tools.Helpers;
using Leadtools.Services.Models;
using Leadtools.Services.Models.Document;
using Leadtools.Document.Converter;
using Leadtools.Services.Models.StatusJobConverter;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used with the StatusJobDataRunner class of the LEADTOOLS Document JavaScript library. 
   /// </summary>
   public class StatusJobConverterController : ApiController
   {
      public StatusJobConverterController()
      {
         ServiceHelper.InitializeController();
      }

      /// <summary>
      /// Runs the status job conversion specified by the conversion job data on the document.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The conversion job could not be started")]
      [HttpPost]
      public RunConvertJobResponse Run(RunConvertJobRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");
         return ConverterHelper.RunConvertJob(request.DocumentId, request.JobData);
      }

      /// <summary>
      /// Queries the status of the conversion job.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The conversion job could not be queried")]
      [HttpPost]
      public QueryConvertJobStatusResponse Query(QueryConvertJobStatusRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var cache = ServiceHelper.Cache;
         return new QueryConvertJobStatusResponse
         {
            jobData = StatusJobDataRunner.QueryJobStatus(cache, request.UserToken, request.JobToken)
         };
      }

      /// <summary>
      /// Deletes the status entry for the conversion job.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The conversion job cache entry could not be deleted")]
      [HttpPost]
      public Response Delete(DeleteConvertJobStatusRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var cache = ServiceHelper.Cache;
         StatusJobDataRunner.DeleteJob(cache, request.UserToken, request.JobToken);
         return new Response();
      }

      /// <summary>
      /// Aborts the conversion job.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The conversion job could not be aborted")]
      [HttpPost]
      public Response Abort(AbortConvertJobRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         var cache = ServiceHelper.Cache;
         StatusJobDataRunner.AbortJob(cache, request.UserToken, request.JobToken);
         return new Response();
      }
   }
}

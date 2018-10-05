// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Leadtools.Services.Models;
using Leadtools.Services.Tools.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Leadtools.Services.Tools.Exceptions
{
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
   public sealed class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
   {
      /* The order of execution is ExceptionLogger, ExceptionFilter, ExceptionHandler.
       * Loggers receive all exceptions, Filter receives a subset of them, and Handler
       * receives only exceptions for which an HttpResponse can be sent.
       * 
       * More info: Web Api Error Handling
       * (http://www.asp.net/web-api/overview/error-handling)
       * 
       * Filters are the only of the three [Logger, Filter, Handler] which 
       * can (1) get the actionContext, useful for getting info about the top-level
       * method that threw the exception, and (2) edit the Exception to include
       * that information.
       */

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
      public override void OnException(HttpActionExecutedContext actionExecutedContext)
      {
         ServiceError serviceError = ToServiceError(actionExecutedContext);
         actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(serviceError.StatusCode, serviceError);
      }

      private static ServiceError ToServiceError(HttpActionExecutedContext actionExecutedContext)
      {
         ServiceError serviceError = new ServiceError();

         /*
          * Steps:
          * - Find the actual cause from the exception.
          *    - Get the message and detail
          *    - Get the link and exceptionType and leadCode
          * - Get the method information from the request context.
          * - Get the userData information from the request context, if necessary.
          */

         SetExceptionInformation(serviceError, actionExecutedContext.Exception);
         SetActionInformation(serviceError, actionExecutedContext);

         // If configured, return the request userData.
         if (ServiceHelper.ReturnRequestUserData)
         {
            Request request = null;
            // Check if any argument is the service Request model (that would contain UserData, potentially).
            var args = actionExecutedContext.ActionContext.ActionArguments.Values.ToArray();
            foreach (var arg in args)
            {
               if (arg is Request)
               {
                  request = (Request)arg;
                  break;
               }
            }

            if (request != null)
               serviceError.UserData = request.UserData;
         }

         return serviceError;
      }

      private static void SetExceptionInformation(ServiceError error, Exception ex)
      {
         if (ex == null)
            return;

         // AggregateException often wraps a real exception when we use a filter.
         if (ex is AggregateException)
         {
            Exception trueEx = ex.InnerException;
            if (trueEx != null)
               ex = trueEx;
         }

         // ServiceException often wraps a real exception.
         if (ex is ServiceException)
         {
            HttpStatusCode statusCode = ((ServiceException)ex).StatusCode;
            error.StatusCode = statusCode;
            Exception trueEx = ex.InnerException;
            if (trueEx != null)
               ex = trueEx;
         }

         // Set the detail and exception type
         error.Detail = ex.Message;
         error.ExceptionType = ex.GetType().ToString();

         // Customize based on exception type

         var webException = ex as WebException;
         // For bad urls, etc
         if (webException != null && webException.Status == WebExceptionStatus.ProtocolError && webException.Response != null)
         {
            try
            {
               error.StatusCode = ((HttpWebResponse)webException.Response).StatusCode;
            }
            catch
            { }
         }

         // add more here, if necessary.
         var rasterException = ex as RasterException;
         if (rasterException != null)
         {
            error.Code = (int)rasterException.Code;
            Exception inner = rasterException.InnerException;
            if (inner != null && error.Code == 0)
            {
               error.ExceptionType = inner.GetType().ToString();
            }
            else
            {
               error.Link = rasterException.HelpLink != null ? rasterException.HelpLink : "https://www.leadtools.com/help/leadtools/v20/dh/l/rasterexceptioncode.html";
            }
         }

         var ocrException = ex as Ocr.OcrException;
         if (ocrException != null)
         {
            error.Code = (int)ocrException.Code;
            error.Link = ocrException.HelpLink != null ? ocrException.HelpLink : "https://www.leadtools.com/help/leadtools/v20/dh/fo/ocrexception.html";
         }

         var barcodeException = ex as Barcode.BarcodeException;
         if (barcodeException != null)
         {
            error.Code = (int)barcodeException.Code;
            error.Link = barcodeException.HelpLink != null ? barcodeException.HelpLink : "https://www.leadtools.com/help/leadtools/v20/dh/ba/barcodeexceptioncode.html";
         }
      }

      private static void SetActionInformation(ServiceError error, HttpActionExecutedContext actionExecutedContext)
      {
         // Get the name of the action ("Decrypt") and controller ("DocumentController")
         HttpActionContext actionContext = actionExecutedContext.ActionContext;
         IHttpController controller = actionContext.ControllerContext.Controller;

         string actionName = actionContext.ActionDescriptor.ActionName;

         try
         {
            // Get the value from the attribute on the method.
            ServiceErrorAttribute serviceErrorAttribute = (ServiceErrorAttribute)GetCustomAttribute(controller.GetType().GetMethod(actionName), typeof(ServiceErrorAttribute), true);

            if (serviceErrorAttribute != null)
            {
               if (serviceErrorAttribute.Message != null)
                  error.Message = serviceErrorAttribute.Message;

               if (serviceErrorAttribute.MethodName != null)
                  actionName = serviceErrorAttribute.MethodName;
            }
         }
         catch { }

         var controllerDescriptor = actionContext.ActionDescriptor.ControllerDescriptor;
         if (controllerDescriptor != null)
         {
            var controllerName = controllerDescriptor.ControllerName;
            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
               actionName = string.Format("{0}.{1}", controllerName, actionName);
         }

         error.MethodName = actionName;
      }
   }
}

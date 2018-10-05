// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Leadtools.Services.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace Leadtools.Services.Tools.Helpers
{
   /* We already have CORS installed globally, but that won't work for GET requests for images.
    * This filter is applied to the actions that return images that are placed directly into 
    * image elements, so that LEADTOOLS can modify them with canvas without getting errors.
    * See "CORS Enabled Image" https://developer.mozilla.org/en-US/docs/Web/HTML/CORS_enabled_image
    * 
    * Note, we only use this filter for origins and do not specify headers or methods.
    */
   public class AlwaysCorsFilter : ActionFilterAttribute
   {
      public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
      {
         if (actionExecutedContext != null && actionExecutedContext.Response != null)
         {
            actionExecutedContext.Response.Headers.Remove("Access-Control-Allow-Origin");
            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", ServiceHelper.CORSOrigins);
         }
         base.OnActionExecuted(actionExecutedContext);
      }
   }
}

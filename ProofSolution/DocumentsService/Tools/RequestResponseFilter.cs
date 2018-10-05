using Leadtools.DocumentViewer.Models.Structure;
using Leadtools.Services.Models;
using Leadtools.Services.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DocumentsService.Tools
{
   public class RequestResponseFilter : IActionFilter
   {
      public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
      {
         Request request = null;
         // Check if any argument is the service Request model (that would contain UserData, potentially).
         var args = actionContext.ActionArguments.Values.ToArray();
         foreach (var arg in args)
         {
            if (arg is Request)
            {
               request = (Request)arg;
               break;
            }
         }

         // ================================
         // Execute the request
         var response = continuation();
         response.Wait();
         // ================================

         if (request != null && ServiceHelper.ReturnRequestUserData &&  response.Result != null)
         {
            var objectContent = response.Result.Content as ObjectContent;
            if (objectContent != null)
            {
               var value = objectContent.Value as Response;
               if (value != null && value.UserData == null)
                  value.UserData = request.UserData;
            }
         }

         return response;
      }

      public bool AllowMultiple
      {
         get { return true; }
      }
   }
}
// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using DocumentsService.Tools;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.Services.Tools.Helpers;
using Newtonsoft.Json.Serialization;
using System;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

namespace Leadtools.DocumentViewer
{
   public static class WebApiConfig
   {
      public static void Register(HttpConfiguration config)
      {
         // Web API configuration and services
         // DocumentsService Config below

         var cors = new EnableCorsAttribute(ServiceHelper.CORSOrigins, ServiceHelper.CORSHeaders, ServiceHelper.CORSMethods);
         cors.PreflightMaxAge = ServiceHelper.CORSMaxAge;
         config.EnableCors(cors);

         // Add our new Exception Logger
         config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger());

         // Add our new Exception Filter
         config.Filters.Add(new GlobalExceptionFilterAttribute());

         // Add an Activator to handle errors only in the controller constructor
         config.Services.Replace(typeof(IHttpControllerActivator), new ExceptionHandlingControllerActivator(config.Services.GetHttpControllerActivator()));

         // Add Request/Response Filter
         config.Filters.Add(new RequestResponseFilter());

         // Web API routes
         config.MapHttpAttributeRoutes();

         // For retrieving items from URL from the cache, like converted documents
         // For more info on routing: http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-and-action-selection
         // Also note the "DocumentViewerCacheHandler" in web.config
         config.Routes.MapHttpRoute(
            name: "DocumentViewerCaching",
            routeTemplate: "api/Cache/Item/{region}/{key}",
            defaults: new { controller = "Cache", action = "Item" }
         );

         // If you change "api", change it in CacheController as well.
         config.Routes.MapHttpRoute(
             name: "DocumentViewerApi",
             routeTemplate: "api/{controller}/{action}",
             defaults: null
         );

         // Modify the JSON serializer
         // Makes property names lowercase on serialization, and makes sure objects
         // like LeadRectD and BarcodeData aren't serialized from their "toString" method.
         config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new NotToStringContractResolver();
      }

      /* 
    * A "ContractResolver" in Newtonsoft's Json.NET is used to control how objects are
    * deserialized and serialized with JSON. in Application_Start(), we change it from
    * "DefaultContractResolver" to this one.
    * NotToStringContractResolver comes from CamelCasePropertyNamesContractResolver,
    * which makes all properties camelCase.
    * 
    * The added code below is used when the serializer is serializing the types for 
    * the return. We override the CreateContract method to see what contract was
    * going to be used. If we had a non-string that was being serialized to a string,
    * we overrule that decision and make it serialize as an object.
    * 
    * relevant area of Json.NET source:
    * https://github.com/JamesNK/Newtonsoft.Json/blob/52d9c0fca365ebc4342c612490a9d8bde4f65841/Src/Newtonsoft.Json/Serialization/DefaultContractResolver.cs
    */
      class NotToStringContractResolver : CamelCasePropertyNamesContractResolver
      {
         protected override JsonContract CreateContract(Type objectType)
         {
            JsonContract contract = base.CreateContract(objectType);
            if (objectType != typeof(string) && (contract is JsonStringContract))
            {
               // We don't want a string contract unless the objectType was actually a string
               return base.CreateObjectContract(objectType);
            }
            return base.CreateContract(objectType);
         }
      }
   }
}

// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using Newtonsoft.Json.Serialization;
using System;
using System.Web.Http;
using System.Web.Mvc;
using Leadtools.Services.Tools.Helpers;

namespace Leadtools.DocumentViewer
{
   public class DocumentsService : System.Web.HttpApplication
   {
      protected void Application_Start(object sender, EventArgs e)
      {
         // Add our Help Pages configuration (http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/creating-api-help-pages)
         AreaRegistration.RegisterAllAreas();

         // Set up our routing.
         GlobalConfiguration.Configure(WebApiConfig.Register);

         ServiceHelper.InitializeService();
      }

      protected void Application_End()
      {
         ServiceHelper.CleanupService();
      }
   }
}

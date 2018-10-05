// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System.IO;
using System.Web;

using Leadtools;
using Leadtools.Codecs;

namespace Leadtools.DocumentViewer.Tools.Helpers
{
   internal static class ImageSaver
   {
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      public static Stream SaveImage(RasterImage image, RasterCodecs rasterCodecs, SaveImageFormat saveFormat, string mimeType, int bitsPerPixel, int qualityFactor)
      {
         // We need to find out the format, bits/pixel and quality factor
         // If the user gave as a format, use it
         if (saveFormat == null)
         {
            // If the user did not give us a format, use PNG
            saveFormat = new PngImageFormat();
            mimeType = "image/png";
         }

         saveFormat.PrepareToSave(rasterCodecs, image, bitsPerPixel, qualityFactor);

         // Save it to a memory stream
         var ms = new MemoryStream();
         rasterCodecs.Save(image, ms, saveFormat.ImageFormat, saveFormat.BitsPerPixel);
         return PrepareStream(ms, mimeType);
      }

      public static Stream PrepareStream(Stream stream, string mimeType)
      {
         stream.Position = 0;

         // Set the MIME type and Content-Type if there is a valid web context
         var currentContext = HttpContext.Current;
         if (currentContext != null)
         {
            currentContext.Response.ContentType = mimeType;
            currentContext.Response.Headers.Add("ContentLength", stream.Length.ToString());
         }
         return stream;
      }
   }
}

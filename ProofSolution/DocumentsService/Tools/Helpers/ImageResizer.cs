﻿// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
 using System;

using Leadtools;
using Leadtools.ImageProcessing;

namespace Leadtools.DocumentViewer.Tools.Helpers
{
   internal static class ImageResizer
   {
      // Optionally resizes the image before saving it (always preserving the original aspect ratio)
      public static void ResizeImage(RasterImage image, int width, int height)
      {
         SizeCommand sizeCommand;
         int resizeWidth;
         int resizeHeight;

         // First check if its a FAX image (with different resolution), if so, resize it too
         if (image.XResolution != 0 && image.YResolution != 0 && Math.Abs(image.XResolution - image.YResolution) > 2)
         {
            // Yes
            if (image.XResolution > image.YResolution)
            {
               resizeWidth = image.ImageWidth;
               resizeHeight = (int)((double)image.ImageHeight * (double)image.XResolution / (double)image.YResolution);
            }
            else
            {
               resizeHeight = image.ImageHeight;
               resizeWidth = (int)((double)image.ImageWidth * (double)image.YResolution / (double)image.XResolution);
            }

            sizeCommand = new SizeCommand(resizeWidth, resizeHeight, RasterSizeFlags.Resample | RasterSizeFlags.ScaleToGray);
            sizeCommand.Run(image);

            image.XResolution = Math.Max(image.XResolution, image.YResolution);
            image.YResolution = image.XResolution;
         }

         // Check user resize options, and resize only if needed
         if ((width == 0 && height == 0) ||
            (image.ImageWidth <= width && image.ImageHeight <= height))
            return;

         resizeWidth = width;
         resizeHeight = height;

         // If width or height is 0, means the other is a fixed value and the missing value must be calculated
         // saving the aspect ratio
         if (resizeHeight == 0)
            resizeHeight = (int)((double)image.ImageHeight * (double)resizeWidth / (double)image.ImageWidth + 0.5);
         else if (resizeWidth == 0)
            resizeWidth = (int)((double)image.ImageWidth * (double)resizeHeight / (double)image.ImageHeight + 0.5);

         // Calculate the destination size
         var rc = new LeadRect(0, 0, resizeWidth, resizeHeight);
         rc = RasterImage.CalculatePaintModeRectangle(
            image.ImageWidth,
            image.ImageHeight,
            rc,
            RasterPaintSizeMode.Fit,
            RasterPaintAlignMode.Near,
            RasterPaintAlignMode.Near);

         // Resize it, use Resample (for colored images) | ScaleToGray (for B/W images)
         sizeCommand = new SizeCommand(rc.Width, rc.Height, RasterSizeFlags.Resample | RasterSizeFlags.ScaleToGray);
         sizeCommand.Run(image);

         // Note, if the image was 1BPP, ScaleToGray converts it to 8, the format of the returned image is dealt with
         // in PrepareToSave

         // Since we resized the image, the original DPI is not correct anymore
         image.XResolution = 96;
         image.YResolution = 96;
      }
   }
}

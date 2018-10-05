// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Diagnostics;

using Leadtools.Document;
using Leadtools.Demos;
using Leadtools.Codecs;
using Leadtools.Caching;

using Leadtools.DocumentViewer.Models.Factory;
using Leadtools.Services.Tools.Exceptions;
using Leadtools.Services.Tools.Helpers;
using Leadtools.Services.Models;
using Leadtools.Services.Models.PreCache;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.IO.Packaging;
using Leadtools.DocumentViewer.Models.Annotations;
using Leadtools.Annotations.Engine;

namespace Leadtools.DocumentViewer.Controllers
{
   /// <summary>
   /// Used for setting/getting annotations with external applications.
   /// </summary>
   public class AnnotationsController : ApiController
   {
      public AnnotationsController()
      {
         ServiceHelper.InitializeController();
      }

      /*
       * #REF IBM_Annotations_Support
       * 
       * The below endpoints (SetAnnotationsIBM, GetAnnotationsIBM) are used to convert IBMP8 annotations XML to LEADTOOLS AnnObject instances
       * for use with the LEADTOOLS DocumentViewer. Due to differences between the IBMP8 spec and LEADTOOLS, certain LEADTOOLS Annotations
       * objects are not yet supported for conversion to/from IBMP8.
       * 
       * A rough list of support is below, along with the Id (from AnnObject.Id).
       * 
       * See the GetAnnotationsIBM endpoint for information on the response.
       * 
       * Supported:
       *    Line (-2),
       *    Rectangle (-3),
       *    Ellipse (-4),
       *    Polyline (-5),
       *    Polygon (-6),
       *    Pointer (-9),
       *    Freehand (-10),
       *    Hilite (-11),
       *    Text (-12),
       *    Note (-15),
       *    Stamp (-16),
       *    StickyNote (-32),
       * 
       * Partially Supported:
       *    TextRollup (-13) becomes a Text (-12) object during conversion to IBM.
       *    TextPointer (-14) becomes a Text (-12) object during conversion to IBM.
       *    Redaction (-22) becomes an opaque Rectangle (-3) object during conversion to IBM.
       * 
       * Unsupported:
       *    Curve (-7),
       *    ClosedCurve (-8),
       *    RubberStamp (-17),
       *    Hotspot (-18),
       *    FreehandHotspot (-19),
       *    Button (-20),
       *    Point (-21),
       *    Ruler (-23),
       *    PolyRuler (-24),
       *    Protractor (-25),
       *    CrossProduct (-26),
       *    Encrypt (-27),
       *    Audio (-28),
       *    RichText (-29),
       *    Media (-30),
       *    Image (-31),
       *    TextHilite (-33),
       *    TextStrikeout (-34),
       *    TextUnderline (-35),
       *    TextRedaction (-36)
       */

      /// <summary>
      ///  Sets IBM P8 annotations into the document.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The IBM annotations could not be set")]
      [HttpPost]
      public SetAnnotationsIBMResponse SetAnnotationsIBM(SetAnnotationsIBMRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         // Must have the documentId you'd like to add annotations to.
         // If you only have the document cache URI, DocumentFactory.LoadFromUri needs to be called.
         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentNullException("documentId");

         // Check that we have annotations.
         if (request.Annotations == null)
            throw new ArgumentNullException("annotations");

         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            // Ensure we have the document.
            DocumentHelper.CheckLoadFromCache(document);

            // If the document is read-only then we won't be able to modify its settings. Temporarily change this state.
            bool wasReadOnly = document.IsReadOnly;
            document.IsReadOnly = false;

            // Get the IBM annotations from the request.
            IBMAnnotation[] ibmAnnotationObjects = request.Annotations;

            // Start converting. We need a rendering engine instance to help with measuring font sizes.
            AnnRenderingEngine renderingEngine = ServiceHelper.GetAnnRenderingEngine();

            // We have different options for reading the annotation...
            IBMP8ReadOptions readOptions = new IBMP8ReadOptions();
            readOptions.RenderingEngine = renderingEngine;

            int pagesCount = document.Pages.Count;
            // We have all the IBM objects converted to LEAD; now, add them to their respective page containers.
            Dictionary<int, AnnContainer> modifiedContainers = new Dictionary<int, AnnContainer>();

            for (int i = 0; i < ibmAnnotationObjects.Length; i++)
            {
               // Our IBM annotation, as an XML string.
               IBMAnnotation ibmObj = ibmAnnotationObjects[i];
               if (ibmObj == null || string.IsNullOrEmpty(ibmObj.Annotation))
                  continue;

               try
               {
                  // Before converting, get the target page number.
                  string ibmPageNumberValue = AnnCodecs.ReadIBMP8PropDescAttr(ibmObj.Annotation, AnnCodecs.IBM_PAGENUMBER);
                  int pageNumber = 1;
                  if (!string.IsNullOrEmpty(ibmPageNumberValue))
                     int.TryParse(ibmPageNumberValue, out pageNumber);

                  // Make sure the page exists.
                  // If zero, set to page 1; if outside of the document range, disregard it.
                  if (pageNumber == 0)
                     pageNumber = 1;
                  else if (pageNumber > pagesCount)
                     continue;

                  // Get its container (one per page) and add the object to it.
                  DocumentPage documentPage = document.Pages[pageNumber - 1];

                  AnnContainer container = null;
                  if (modifiedContainers.ContainsKey(pageNumber))
                  {
                     container = modifiedContainers[pageNumber];
                  }
                  else
                  {
                     container = documentPage.GetAnnotations(true);
                     modifiedContainers.Add(pageNumber, container);
                  }

                  readOptions.Mapper = container.Mapper;

                  // Convert to a LEADTOOLS AnnObject.
                  // See "#REF IBM_Annotations_Support" for support info
                  AnnObject leadObj = AnnCodecs.ConvertFromIBMP8(ibmObj.Annotation, readOptions);
                  if (leadObj == null)
                  {
                     Trace.WriteLine("Conversion from IBM Annotation not supported for item at index " + i);
                     continue;
                  }

                  // Add the supplied properties
                  if (!string.IsNullOrEmpty(ibmObj.Password))
                  {
                     leadObj.Lock(ibmObj.Password);
                  }
                  if (!string.IsNullOrEmpty(ibmObj.UserId))
                  {
                     leadObj.UserId = ibmObj.UserId;
                     Dictionary<string, string> metadata = leadObj.Metadata;
                     string key = AnnObject.AuthorMetadataKey;
                     if (metadata.ContainsKey(key))
                     {
                        if (string.IsNullOrEmpty(metadata[key]))
                           metadata[key] = ibmObj.UserId;
                     }
                     else
                     {
                        metadata.Add(key, ibmObj.UserId);
                     }
                  }
                  container.Children.Add(leadObj);
               }
               catch (Exception e)
               {
                  Trace.WriteLine(string.Format("Failed to convert IBM Annotation at index {0}: {1}", i, e.Message));
               }
            }

            // Set the modified containers in the document.
            AnnContainer[] containers = modifiedContainers.Values.ToArray();
            document.Annotations.SetAnnotations(containers);

            // Reset the read-only value from above before saving into the cache.
            document.IsReadOnly = wasReadOnly;
            // Enable history tracking from this point forward so that calls to retrieve the IBM Annotations will have a history from this set operation.
            document.History.AutoUpdate = true;

            // Clear any old history.
            AnnHistory history = document.Annotations.GetHistory();
            if (history != null)
            {
               history.Clear();
               document.Annotations.SetHistory(history);
            }
            document.SaveToCache();

            return new SetAnnotationsIBMResponse();
         }
      }

      // For debugging - if true, annotations history will be logged to the console
      private static bool _loggingLEADToIBM = false;

      // For processing - as an example, write to disk.
      private static string _processToDiskRoot = null;

      /// <summary>
      ///  Gets changed IBM P8 annotations from the document.
      /// </summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ServiceErrorAttribute(Message = "The IBM annotations could not be retrieved")]
      [HttpPost]
      public GetAnnotationsIBMResponse GetAnnotationsIBM(GetAnnotationsIBMRequest request)
      {
         if (request == null)
            throw new ArgumentNullException("request");

         // Must have the documentId you'd like to add annotations to.
         // If you only have the document cache URI, DocumentFactory.LoadFromUri needs to be called.
         if (string.IsNullOrEmpty(request.DocumentId))
            throw new ArgumentNullException("documentId");

         // The IBM annotations will be stored here (key: GUID, value: XML string).
         Dictionary<AnnHistoryItemState, Dictionary<string, string>> ibmObjects = null;

         var cache = ServiceHelper.Cache;
         using (var document = DocumentFactory.LoadFromCache(cache, request.DocumentId))
         {
            // Ensure we have the document.
            DocumentHelper.CheckLoadFromCache(document);

            // Use the history to return all added, modified, and deleted IBM annotations.
            ibmObjects = ProcessAnnotationsHistory(document);

         }

         // Here (if defined) we send the converted annotations to disk as an example.
         if (ibmObjects != null && _processToDiskRoot != null)
         {
            try
            {
               string dir = _processToDiskRoot + "/" + request.DocumentId;
               ProcessToDisk(dir, ibmObjects);
            }
            catch (Exception e)
            {
               Trace.WriteLine(string.Format("Failed to process converted annotations to disk: {0}", e.Message));
            }
         }

         GetAnnotationsIBMResponse response = new GetAnnotationsIBMResponse();
         if (ibmObjects != null)
         {
            // See #REF IBM_Annotations_Support
            if (ibmObjects.ContainsKey(AnnHistoryItemState.Added))
               response.Added = ibmObjects[AnnHistoryItemState.Added];
            if (ibmObjects.ContainsKey(AnnHistoryItemState.Modified))
               response.Modified = ibmObjects[AnnHistoryItemState.Modified];
            if (ibmObjects.ContainsKey(AnnHistoryItemState.Deleted))
               response.Deleted = ibmObjects[AnnHistoryItemState.Deleted];
         }

         return response;
      }

      /*
       * See #REF IBM_Annotations_Support.
       * If true, unsupported types will have their response value set as the constant.
       */
      public static string UNSUPPORTED_VALUE = "UNSUPPORTED";
      public static bool ReturnUnsupported = true;

      private Dictionary<AnnHistoryItemState, Dictionary<string, string>> ProcessAnnotationsHistory(LEADDocument document)
      {
         // Get the history (which is updated each time SetAnnotations/SaveToCache is called).
         AnnHistory history = document.Annotations.GetHistory();
         if (history == null)
            return null;

         // Condense the history to get all the changes since the last time the history was cleared in one list.
         history.Condense();
         if (history.Items.Count == 0)
            return null;

         string documentId = document.DocumentId;

         bool logging = _loggingLEADToIBM;
         if (logging)
         {
            Trace.WriteLine(string.Format("Logging: LEADTOOLS annotations to IBM annotations for document '{0}'", documentId));
            Trace.WriteLine("  Condensed history of changes since last save/set:");
            var items = (List<AnnHistoryItem>)history.Items;
            foreach (AnnHistoryItem item in items)
            {
               // You also have access to the user information here.
               Trace.WriteLine(string.Format("    {0} {1} {2}", item.Guid, item.State, item.Timestamp));
            }
         }

         // Get all annotation containers (and the objects within them).
         AnnContainer[] containers = document.Annotations.GetAnnotations(false);

         // Process all the added objects. LEAD tracks both "Added" and "AddedAndModified" (cases
         // where annotations were added and then modified from the default). Here, we combine them
         // for a simple "ADDED" list.
         Dictionary<string, string> addedIBMObjects = new Dictionary<string, string>();

         // Get the guids for the added objects from our history object.
         string[] guids = history.GetGuidForState(AnnHistoryItemState.Added);
         ProcessObjects(addedIBMObjects, documentId, guids, AnnHistoryItemState.Added, containers);
         guids = history.GetGuidForState(AnnHistoryItemState.AddedAndModified);
         ProcessObjects(addedIBMObjects, documentId, guids, AnnHistoryItemState.Added, containers);

         // Process all modified objects
         Dictionary<string, string> modifiedIBMObjects = new Dictionary<string, string>();
         string[] modifiedGuids = history.GetGuidForState(AnnHistoryItemState.Modified);
         ProcessObjects(modifiedIBMObjects, documentId, modifiedGuids, AnnHistoryItemState.Modified, containers);

         // Process all deleted objects (note, we only get the guids for them; no IBM objects will be here)
         Dictionary<string, string> deletedIBMObjects = new Dictionary<string, string>();
         guids = history.GetGuidForState(AnnHistoryItemState.Deleted);
         ProcessObjects(deletedIBMObjects, documentId, guids, AnnHistoryItemState.Deleted, containers);

         // Clear the history since we updated everything.
         history.Clear();
         // Set the history again (so it will be saved in the cache for this document).
         document.Annotations.SetHistory(history);

         // List of converted IBM objects that has been modified
         Dictionary<AnnHistoryItemState, Dictionary<string, string>> ibmObjects = new Dictionary<AnnHistoryItemState, Dictionary<string, string>>();
         ibmObjects.Add(AnnHistoryItemState.Added, addedIBMObjects);
         ibmObjects.Add(AnnHistoryItemState.Modified, modifiedIBMObjects);
         ibmObjects.Add(AnnHistoryItemState.Deleted, deletedIBMObjects);

         return ibmObjects;
      }

      private static void ProcessObjects(Dictionary<string, string> ibmStrings, string documentId, string[] guids, AnnHistoryItemState state, AnnContainer[] containers)
      {
         if (guids.Length == 0)
            return;

         bool logging = _loggingLEADToIBM;
         if (logging)
         {
            Trace.WriteLine(string.Format("Processing '{0}' objects", state));
         }

         // Use the write options to customize the conversion.
         IBMP8WriteOptions writeOptions = new IBMP8WriteOptions();
         // We know the destination page number from the container, so we don't need to infer it.
         writeOptions.InferPageNumberFromMetadata = false;
         writeOptions.InferForMultiPageTiffFromMetadata = false;

         // Deleted objects are a special case - we won't have their annotations to convert! We will only have the guid.
         if (state == AnnHistoryItemState.Deleted)
         {
            foreach (string guid in guids)
            {
               if (logging)
                  Trace.WriteLine(string.Format("     {0}", guid));
               ibmStrings.Add(guid, null);
            }
         }
         else
         {
            // Get the LEADTOOLS annotation objects for each container from the guids.
            var objects = AnnContainer.FindObjectsByGuid(containers, guids);

            foreach (int containerIndex in objects.Keys)
            {
               var container = containers[containerIndex];
               var containerObjects = objects[containerIndex];

               // Customize the write options for this specific container.
               writeOptions.PageNumber = container.PageNumber;
               writeOptions.Mapper = container.Mapper;

               if (logging)
               {
                  Trace.WriteLine(string.Format("  Page {0}", container.PageNumber));
               }
               // Convert each AnnObject back to an IBM annotation XML string.
               foreach (AnnObject leadObj in containerObjects)
               {
                  string guid = leadObj.Guid;

                  string outputLogMessage = null;
                  if (logging)
                     outputLogMessage = string.Format("     {0} ({1}) ({2})", leadObj.FriendlyName, leadObj.Id, guid);

                  // See "#REF IBM_Annotations_Support" for support info
                  string ibmObjString = AnnCodecs.ConvertToIBMP8(leadObj, writeOptions);
                  // If null, the LEAD AnnObject is not supported in the IBMP8 spec.
                  if (ibmObjString == null)
                  {
                     if (logging)
                        Trace.WriteLine(string.Format("{0} UNSUPPORTED", outputLogMessage));
                     if (ReturnUnsupported)
                     {
                        // Still add it to the response
                        ibmStrings.Add(guid, UNSUPPORTED_VALUE);
                     }
                     continue;
                  }
                  else
                  {
                     if (logging)
                        Trace.WriteLine(outputLogMessage);
                  }

                  // The GUID for added objects are auto-generated by LEAD to random values.
                  // You may modify them (or any attribute) here:
                  guid = "mod-" + leadObj.Guid;
                  // Change the guid in the XML
                  ibmObjString = AnnCodecs.WriteIBMP8PropDescAttr(ibmObjString, AnnCodecs.IBM_ID, guid, writeOptions);

                  // Update the guid for our response, too
                  ibmStrings.Add(guid, ibmObjString);
               }
            }
         }
      }

      private static void ProcessToDisk(string dir, Dictionary<AnnHistoryItemState, Dictionary<string, string>> ibmObjects)
      {

         // Also separate by time, since saving can occur multiple times
         string nowTime = DateTime.Now.ToString("MMddyy_Hmmss");
         dir = dir + "/" + nowTime;

         if (ibmObjects.ContainsKey(AnnHistoryItemState.Added))
            DumpObjects(dir, "added", ibmObjects[AnnHistoryItemState.Added]);
         if (ibmObjects.ContainsKey(AnnHistoryItemState.Modified))
            DumpObjects(dir, "modified", ibmObjects[AnnHistoryItemState.Modified]);
         if (ibmObjects.ContainsKey(AnnHistoryItemState.Deleted))
            DumpObjects(dir, "deleted", ibmObjects[AnnHistoryItemState.Deleted]);
      }

      // A helper method for dumping the converted annotations back to disk.
      private static void DumpObjects(string dir, string subDir, Dictionary<string, string> ibmObjects)
      {
         try
         {
            string directory = Path.Combine(dir, subDir);
            Directory.CreateDirectory(directory);

            string allAsText = "";
            foreach (KeyValuePair<string, string> ibmObject in ibmObjects)
            {
               string guid = ibmObject.Key;
               string xml = ibmObject.Value;
               allAsText += string.Format("// {0}\n{1}\n", guid, xml);
               File.WriteAllText(Path.Combine(directory, guid) + ".xml", xml);
            }
            if (allAsText.Length > 0)
               File.WriteAllText(Path.Combine(directory, "all.txt"), allAsText);
         }
         catch (Exception ex)
         {
            throw new InvalidOperationException("Error writing converted file to disk", ex);
         }
      }
   }
}

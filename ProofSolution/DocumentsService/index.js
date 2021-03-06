﻿// *************************************************************
// Copyright (c) 1991-2018 LEAD Technologies, Inc.
// All Rights Reserved.
// *************************************************************
/* This file is a common file between the .NET and Java Documents Services.
 * It is linked to and copied into each project when that project is built. */

var OcrEngineStatus = [];
OcrEngineStatus[OcrEngineStatus[0] = "Unset"] = 0;
OcrEngineStatus[OcrEngineStatus[1] = "Error"] = 1;
OcrEngineStatus[OcrEngineStatus[2] = "Ready"] = 2;

$(document).ready(function () {

   // quickly creates a "key: value" element structure for viewing.
   function createStatEntry(key, val, isExpectation) {
      var $label = $(document.createElement("label")).text(key);
      var $val = $(document.createElement("span")).text(val);
      if (!isExpectation) $val.addClass("error")
      return $(document.createElement("div")).append($label).append($val);
   }

   // Ping
   var firstTimeCompleted = false;
   var $pingStats = $("#pingStats");
   var $refreshPingStatistics = $("#refreshPingStatistics");
   var pingResults = [
      { label: "Status: ", key: "message", expectation: "Ready" },
      { label: "Was License Checked: ", key: "isLicenseChecked", expectation: true },
      { label: "Is License Expired: ", key: "isLicenseExpired", expectation: false },
      { label: "Kernel Type: ", key: "kernelType", expectation: "Release" },
      { label: "Is Cache Accessible: ", key: "isCacheAccessible", expectation: true },
   ];
   var refreshPingInfo = function () {
      $pingStats.addClass("hide");
      $refreshPingStatistics.prop('disabled', true);
      $.ajax("./api/Test/Ping", {
         headers: { 'cache-control': 'no-cache' }
      }).fail(function (jqXHR) {
         (window.console && console.log(jqXHR));
         $pingStats.empty().text("The ping statistics could not be retrieved. Check the console.");
      }).done(function (result) {
         $pingStats.empty();
         if (result) {
            if (result["time"]) {
               var $label = $(document.createElement("label")).text("Updated: ");
               var $val = $(document.createElement("span")).text(new Date(result["time"]).toTimeString());
               var $holder = $(document.createElement("div")).append($label).append($val);
               $pingStats.append($holder);
            }
            pingResults.forEach(function (keyValOb) {
               var val = result[keyValOb.key];
               var isExpectation = val === keyValOb.expectation;
               if (keyValOb.expectation && typeof val === 'string' || val instanceof String)
                  isExpectation = val.toLowerCase() === keyValOb.expectation.toLowerCase();
               if (val != null && val != undefined) {
                  $pingStats.append(createStatEntry(keyValOb.label, val, isExpectation));
               }
            })
            var ocrStatus = parseInt(result["ocrEngineStatus"], 10);
            if (!isNaN(ocrStatus)) {
               $pingStats.append(createStatEntry("OCR Engine Status: ", OcrEngineStatus[ocrStatus], ocrStatus === OcrEngineStatus.Ready))
            }
            if (result["kernelVersion"])
               $pingStats.append(createStatEntry("Kernel Version: ", result["kernelVersion"], true));
            if (result["serviceVersion"])
               $pingStats.append(createStatEntry("Service Version: ", result["serviceVersion"], true));
            if (result["multiplatformSupportStatus"])
               $pingStats.append(createStatEntry("Multi-platform Support Status: ", result["multiplatformSupportStatus"], true));
         }
         else
            $pingStats.empty().text("No ping data could be read.");
      }).always(function () {
         setTimeout(function () {
            $pingStats.removeClass("hide");
            $refreshPingStatistics.prop('disabled', false);

            if (!firstTimeCompleted) {
               firstTimeCompleted = true;
               refreshCacheInfo();
               refreshPreCacheInfo(false);
            }
         }, 500)
      })
   }
   refreshPingInfo();
   $refreshPingStatistics.click(refreshPingInfo);

   // Caching
   var $cacheOpsPasscode = $("#cacheOpsPasscode");
   var $cacheStats = $("#cacheStats");
   var $purgeButton = $("#purgeCacheButton");
   var $refreshCacheStatistics = $("#refreshCacheStatistics");
   var expiredItemsCount = 0;

   var refreshCacheInfo = function () {
      // change our element states
      $cacheStats.addClass("hide");
      $refreshCacheStatistics.prop('disabled', true);
      $purgeButton.prop('disabled', true);

      // make the call
      var params = {};
      var cacheOpsPasscode = $cacheOpsPasscode.val()
      if (cacheOpsPasscode)
         params["passcode"] = cacheOpsPasscode;
      $.ajax("./api/Factory/GetCacheStatistics", {
         data: params,
         headers: { 'cache-control': 'no-cache' }
      }).fail(function (jqXHR) {
         var errMessage = "The cache statistics could not be retrieved.";
         if (jqXHR && jqXHR.status === 401)
            errMessage += " The passcode was incorrect."
         $cacheStats.empty().text(errMessage);
      }).done(function (data) {
         var statistics = data["statistics"];
         if (data && statistics) {
            // for simplicity's sake, just manually make this
            var regions = statistics["regions"];
            var $regionsLabel = $(document.createElement("label")).text("Regions: ");
            var $regions = $(document.createElement("span")).text(regions);
            var $reg = $(document.createElement("div")).append($regionsLabel).append($regions);

            var items = statistics["items"];
            var $itemsLabel = $(document.createElement("label")).text("Total Items: ");
            var $items = $(document.createElement("span")).text(items);
            var $it = $(document.createElement("div")).append($itemsLabel).append($items);

            expiredItemsCount = parseInt(statistics["expiredItems"], 10);
            var $expiredLabel = $(document.createElement("label")).text("Expired Items: ");
            var $expired = $(document.createElement("span")).text(expiredItemsCount).addClass("expired");
            var $exp = $(document.createElement("div")).append($expiredLabel).append($expired);

            $cacheStats.empty().append($reg).append($it).append($exp);
         }
         else
            $cacheStats.empty().text("No cache data exists.");
      }).always(function () {
         // Set a timeout so it doesn't flicker.
         setTimeout(function () {
            $cacheStats.removeClass("hide");
            $refreshCacheStatistics.prop('disabled', false);
            if (expiredItemsCount > 0)
               $purgeButton.prop('disabled', false);
         }, 500)
      })
   }

   // Purge the cache, then refresh the info.
   $refreshCacheStatistics.click(refreshCacheInfo);
   $purgeButton.click(function () {
      $cacheStats.addClass("hide");
      $refreshCacheStatistics.prop('disabled', true);
      $purgeButton.prop('disabled', true);
      var params = {};
      var cacheOpsPasscode = $cacheOpsPasscode.val();
      if (cacheOpsPasscode)
         params["passcode"] = cacheOpsPasscode;
      $.ajax("./api/Factory/PurgeCache", {
         data: params,
         headers: { 'cache-control': 'no-cache' }
      }).fail(function (jqXHR) {
         alert("The cache could not be purged.");
         $cacheStats.removeClass("hide");
         $refreshCacheStatistics.prop('disabled', false);
         $purgeButton.prop('disabled', false);
         var errMessage = "The cache statistics could not be retrieved.";
         if (jqXHR && jqXHR.status === 401)
            errMessage += " The passcode was incorrect.";
         $cacheStats.empty().text(errMessage);
      }).done(function (data) {
         refreshCacheInfo();
      })
   })

   // handling an item returned from the pre-cache
   function handleItem(item, didPreCache, asError) {
      var hashKey = item["hashKey"];
      var uri = item["uri"];
      var items = item["items"];
      if (items == null || items.length == 0)
         throw "nothing pre-cached";
      var length = items.length;

      var $title = $(document.createElement("div"));
      if (uri != null && uri.length > 0) {
         // make the title
         var $titleKey = $(document.createElement("span")).text(uri);
         if (asError)
            $titleKey.addClass("error");
         $title.append($titleKey)
      }
      var $list = $(document.createElement("ul"));
      var $firstItem = $(document.createElement("li"));
      if (didPreCache)
         $firstItem.text(" pre-cached with " + length + " size(s) at " + hashKey);
      else
         $firstItem.text(hashKey + " - " + length + " size(s)");
      $list.append($firstItem);
      items.forEach(function (sizeItem) {
         // create the list item
         var $listItem = $(document.createElement("li"));
         var id = sizeItem["documentId"];
         var size = sizeItem["maximumImagePixelSize"];
         var $listItemVal = $(document.createElement("span")).text("(" + size + "px) ");
         var $listItemKey = $(document.createElement("label"));
         if (didPreCache) {
            // overwrite
            var seconds = sizeItem["seconds"];
            $listItemKey.text(id + " - " + seconds + " second(s)");
         } else {
            var reads = sizeItem["reads"];
            $listItemKey.text(id + " - " + reads + " read(s)");
         }
         $listItem.append($listItemVal).append($listItemKey);
         $list.append($listItem);
      });
      return {
         title: $title,
         list: $list
      };
   }

   // Pre-caching
   var $preCacheInput = $("#preCacheInput");
   var $addPreCacheButton = $("#addToPreCache");
   var $preCacheResponse = $("#preCacheResponse");

   var $refreshPreCacheStatistics = $("#refreshPreCacheStatistics");
   var $cleanPreCacheButton = $("#cleanPreCache");
   var $preCacheStats = $("#preCacheStats");

   // only allow us to pre-cache when the url input has something in it
   $preCacheInput.keyup(function () {
      $addPreCacheButton.prop("disabled", $preCacheInput.val().length == 0);
   })

   var addToPreCache = function () {
      // disable buttons, etc
      $preCacheResponse.addClass("hide");
      $refreshPreCacheStatistics.prop("disabled", true);
      $cleanPreCacheButton.prop("disabled", true);
      $addPreCacheButton.prop("disabled", true);
      $preCacheInput.prop("disabled", true);

      var params = {
         uri: $preCacheInput.val(),
         expiryDate: null, // null = forever
         cacheOptions: 0, // 0 = all
         maximumImagePixelSizes: null, // null = [4096, 2048], the defaults
      };
      var cacheOpsPasscode = $cacheOpsPasscode.val();
      if (cacheOpsPasscode)
         params["passcode"] = cacheOpsPasscode;
      $.ajax("./api/Factory/PreCacheDocument", {
         'type': "POST",
         contentType: "application/json",
         data: JSON.stringify(params)
      }).fail(function (jqXHR) {
         var errMessage = "The document could not be pre-cached.";
         if (jqXHR && jqXHR.status === 401)
            errMessage += " The passcode was incorrect.";
         $preCacheResponse.empty().text(errMessage);
      }).done(function (data) {
         var failText = "The document was pre-cached, but did not return any information.";
         $preCacheResponse.empty();
         try {
            var item = data["item"];
            if (data == null || item == null)
               throw "no pre-cache response";

            var elements = handleItem(item, true, false);

            // add the title and the list
            $preCacheResponse.append(elements.title).append(elements.list);

         } catch (e) {
            $preCacheResponse.text(failText);
            (window.console && console.log(e));
         }
         refreshPreCacheInfo(true);
      }).always(function () {
         setTimeout(function () {
            $preCacheResponse.removeClass("hide");
            $refreshPreCacheStatistics.prop("disabled", false);
            $cleanPreCacheButton.prop("disabled", false);
            $addPreCacheButton.prop("disabled", false);
            $preCacheInput.prop("disabled", false);
         }, 500)
      })
   }
   $addPreCacheButton.click(addToPreCache);

   var refreshPreCacheInfo = function (clean) {
      $refreshPreCacheStatistics.prop("disabled", true);
      $cleanPreCacheButton.prop("disabled", true);
      $addPreCacheButton.prop("disabled", true);
      $preCacheInput.prop("disabled", true);
      $preCacheStats.addClass("hide");

      var params = {
         clean: clean,
      };
      var cacheOpsPasscode = $cacheOpsPasscode.val();
      if (cacheOpsPasscode)
         params["passcode"] = cacheOpsPasscode;
      $.ajax("./api/Factory/ReportPreCache", {
         data: params,
         headers: { 'cache-control': 'no-cache' }
      }).fail(function (jqXHR) {
         var errMessage = "The pre-cache could not be retrieved.";
         if (jqXHR && jqXHR.status === 401)
            errMessage += " The passcode was incorrect.";
         $preCacheStats.empty().text(errMessage);
      }).done(function (data) {
         $preCacheStats.empty();
         try {
            var entries = data["entries"];
            var removed = data["removed"];
            if (data == null || ((entries == null || entries.length == 0) && (removed == null || removed.length == 0))) {
               $preCacheStats.text("The pre-cache is empty.")
               return;
            }

            if (entries.length > 0) {
               var $entriesGroup = $(document.createElement("div")).append($(document.createElement("h3")).text("Entries:"));
               entries.forEach(function (responseItem, index) {
                  var elements = handleItem(responseItem, false, false);
                  $entriesGroup.append(elements.title).append(elements.list);
               })
               $preCacheStats.append($entriesGroup);
            }
            if (removed.length > 0) {
               var $removedGroup = $(document.createElement("div")).append($(document.createElement("h3")).text("Removed:"));
               removed.forEach(function (responseItem, index) {
                  var elements = handleItem(responseItem, false, true);
                  $removedGroup.append(elements.title).append(elements.list);
               })
               $preCacheStats.append($removedGroup);
            }
            if (!clean)
               $preCacheStats.prepend($(document.createElement("p")).text("(The pre-cache was not cleaned.)"))
         } catch (e) {
            $preCacheStats.text("The pre-cache information was not parsed correctly.")
            (window.console && console.log(e));
         }
      }).always(function () {
         setTimeout(function () {
            $preCacheStats.removeClass("hide");
            $refreshPreCacheStatistics.prop("disabled", false);
            $cleanPreCacheButton.prop("disabled", false);
            $addPreCacheButton.prop("disabled", false);
            $preCacheInput.prop("disabled", false);
         }, 500)
      })
   }
   $refreshPreCacheStatistics.click(function () {
      refreshPreCacheInfo(true);
   });
})
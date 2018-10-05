"use strict";
(function ($) {
    $.fn.checkByClass = function (value) {
        var element = this;
        if (value != null) {
            if (typeof value === "boolean") {
                if (value) {
                    var elementTag = element.prop("tagName");
                    var elementName = element.attr("name");
                    $(elementTag + "[name=" + elementName + "]").removeClass("checked");
                }
            }
            else if (typeof value === "string") {
                $(value).removeClass("checked");
            }
        }
        element.addClass("checked");
        return element;
    };
    $.fn.display = function (selector) {
        var element = this;
        if (selector != null && selector.length > 0)
            $(selector).hide();
        element.show();
        return element;
    };
    $.fn.isChecked = function () {
        var element = this;
        return element.hasClass("checked");
    };
    $.each(["show", "hide"], function (i, ev) {
        var el = $.fn[ev];
        $.fn[ev] = function () {
            var element = this;
            element.trigger(ev);
            return el.apply(element);
        };
    });
})(jQuery);

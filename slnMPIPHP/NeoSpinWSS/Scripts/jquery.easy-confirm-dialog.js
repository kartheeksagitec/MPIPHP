/**
 * jQuery Easy Confirm Dialog plugin 1.2
 */
(function($) {
    $.easyconfirm = {};
    $.easyconfirm.locales = {};
    $.easyconfirm.locales.enUS = {
        title: 'Are you sure?',
        text: 'Are you sure that you want to perform this action?',
        button: ['Cancel', 'Confirm'],
        closeText: ''
    };
    $.easyconfirm.locales.svSE = {
        title: 'Är du säker?',
        text: 'Är du säker på att du vill genomföra denna åtgärden?',
        button: ['Avbryt', 'Bekräfta'],
        closeText: 'stäng'
    };

    $.fn.easyconfirm = function(options) {

        var _attr = $.fn.attr;

        $.fn.attr = function(attr, value) {
            // Let the original attr() do its work.
            var returned = _attr.apply(this, arguments);

            // Fix for jQuery 1.6+
            if (attr == 'title' && returned === undefined) 
                returned = '';

            return returned;
        };

        var options = jQuery.extend({
            eventType: 'click',
            icon: 'help'
        }, options);

        var locale = jQuery.extend({}, $.easyconfirm.locales.enUS, options.locale);

        // Shortcut to eventType.
        var type = options.eventType;

        return this.each(function() {
            var target = this;
            var $target = jQuery(target);

            // If no events present then and if there is a valid url, then trigger url change
            var urlClick = function() {
                    if (target.href) {
                        var length = String(target.href).length;
                        if (target.href.substring(length - 1, length) != '#') 
                            document.location = target.href;
                    }
                };

            // If any handlers where bind before triggering, lets save them and add them later
            var saveHandlers = function() {
                    var events = jQuery.data(target, 'events');
                    if (events) {
                        target._handlers = new Array();
                        for (var i in events[type]) {
                            target._handlers.push(events[type][i]);
                        }

                        $target.off(type);
                    }
                };
            // Re-bind old events
            var rebindHandlers = function() {
                    if (target._handlers !== undefined) {
                        jQuery.each(target._handlers, function() {
                            $target.on(type, this);
                        });
                    }
                };

                if ($target.attr('Text') !== null && $target.attr('Text').length > 0)
                locale.text = $target.attr('Text');

            var dialog = (options.dialog === undefined || typeof(options.dialog) != 'object') ? 
                $('<div class="dialog confirm">' + locale.text + '</div>') : 
                options.dialog;

            var buttons = {};
            buttons[locale.button[1]] = function() {
                // Unbind overriding handler and let default actions pass through
                $target.off(type, handler);

                // Close dialog
                $(dialog).dialog("close");

                // Check if there is any events on the target
                if (jQuery.data(target, 'events')) {
                    // Trigger click event.
                    $target.trigger('click');
                }
                else {
                    // No event trigger new url
                    urlClick();
                }

                init();

            };
            buttons[locale.button[0]] = function() {
                $(dialog).dialog("close");
            };

            $(dialog).dialog({
                autoOpen: false,
                resizable: false,
                draggable: true,
                closeOnEscape: true,
                width: 'auto',
                minHeight: 120,
                maxHeight: 200,
                buttons: buttons,
                title: locale.title,
                closeText: locale.closeText,
                modal: true
            });

            // Handler that will override all other actions
            var handler = function(event) {
                    $(dialog).dialog('open');
                    event.stopImmediatePropagation();
                    event.preventDefault();
                    return false;
                };

           var init = function() {
//                    saveHandlers();
                    $target.on(type, handler);
//                    rebindHandlers();
                };

            init();

        });

    };
})(jQuery);


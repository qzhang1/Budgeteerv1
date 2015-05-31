(function ($) {
    $.fn.extend({
        formatInput: function (settings) {
            var $elem = $(this);
            settings = $.extend({
                errback: null
            }, settings);
            $elem.bind("keyup.filter_input", $.fn.formatEvent);
        },
        formatEvent: function (e) {
            var elem = $(this);
            var initial_value = elem.val();
            elem.val($.fn.insertCommas(initial_value));
        },
        insertCommas: function (number) {
            //var b = number.exec(/(\.d{1,2})?/);
            //number = b.replace(/\.\d{1,}/, b);
            
            return number.replace(/,/g, "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
        }
    });
})(jQuery);
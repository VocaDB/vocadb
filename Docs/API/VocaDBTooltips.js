// DEPRECATED. This script is not being maintained and may be a security issue. USE IT AT YOUR OWN RISK.
// Please post a comment at https://github.com/VocaDB/vocadb/issues/892 if you have need for an updated tooltip script.

// Add qtip tooltip to all VocaDB album and artist links on page
jQuery(document).ready(function () {
    jQuery("a[href^='http://vocadb.net/'], a[href^='https://vocadb.net/']").each(function (_, elem) {
        var regex = /http(s)?:\/\/vocadb\.net\/((Artist|Album|Song)\/Details|(Ar|Al|E|S|T))\/(\d+)/g;
        var href = jQuery(elem).attr("href");
        var match = regex.test(href);
        if (match) {
            jQuery(elem).qtip({
                content: {
                    text: 'Loading...',
                    ajax: {
                        url: 'https://vocadb.net/Ext/EntryToolTip',
                        type: 'GET',
                        dataType: 'jsonp',
                        data: { url: href },
                        success: function (data) {
                            this.set('content.text', data);
                        }
                    }
                },
                position: {
                    container: jQuery('#container')
                },
                style: {
                    classes: "tooltip-wide"
                }
            });
        }
    });
});
//# sourceMappingURL=VocaDBTooltips.js.map
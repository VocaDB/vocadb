// Add qtip tooltip to all VocaDB album and artist links on page

jQuery(document).ready(function() {
	jQuery("a[href^='http://vocadb.net/']").each(function() {
	
		var elem = this;
		var regex = /http:\/\/vocadb\.net\/((Artist|Album|Song)\/Details|(Ar|Al|S))\/(\d+)/g;
		var href = jQuery(elem).attr("href");
		var match = regex.test(href);
		
		if (match) {
					
			jQuery(elem).qtip({
				content: {
                    text: 'Loading...',
                    ajax: {
                        url: 'http://vocadb.net/Ext/EntryToolTip',
                        type: 'GET',
						dataType: 'jsonp',
                        data: { url: href },
						success: function(data, status) {
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
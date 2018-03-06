// Add qtip tooltip to all VocaDB album and artist links on page

interface jQueryInt {
	(doc: HTMLDocument | HTMLElement | string): any;
}

declare var jQuery: jQueryInt;

jQuery(document).ready(() => {
	jQuery("a[href^='http://vocadb.net/']").each((_, elem: HTMLElement) => {

		var regex = /http:\/\/vocadb\.net\/((Artist|Album|Song)\/Details|(Ar|Al|E|S|T))\/(\d+)/g;
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
						success: function (data: string) {
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
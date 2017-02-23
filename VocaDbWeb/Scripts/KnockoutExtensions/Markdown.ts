/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />

interface KnockoutBindingHandlers {

	markdown: KnockoutBindingHandler;

}

// Renders an observable as HTML processed by Markdown (marked library, https://github.com/chjj/marked). 
// Read-only - the resulted markdown cannot be edited.
ko.bindingHandlers.markdown = {
	update: (element: HTMLElement, valueAccessor: () => KnockoutObservable<string>) => {

		var val: string = ko.unwrap(valueAccessor());

		if (!val) {
			$(element).text(val);
			return;
		}

		// Using GitHub-flavored markdown with simple line breaks and HTML sanitation.
		marked(val, { gfm: true, breaks: true, sanitize: true }, (error, content: string) => {

			if (error)
				$(element).val(error);
			else
				$(element).html(content);

		});

	}
}
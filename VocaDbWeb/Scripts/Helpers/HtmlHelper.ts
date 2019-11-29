
//module vdb.helpers {
	
	export class HtmlHelper {
		
		// Bolds and HTML encodes a term
		public static boldAndHtmlEncode(text: string, term: string) {

			if (!text || !term)
				return text;

			var index = text.toLowerCase().indexOf(term.toLowerCase());

			if (index < 0)
				return HtmlHelper.htmlEncode(text);

			var actualTerm = text.substring(index, index + term.length);

			// Encode parts before match, the match itself and after match.
			return HtmlHelper.htmlEncode(text.substr(0, index)) + "<b>" + HtmlHelper.htmlEncode(actualTerm) + "</b>" + HtmlHelper.htmlEncode(text.substr(index + term.length));

		}

		public static htmlEncode(value: string) {
			//create a in-memory div, set it's inner text(which jQuery automatically encodes)
			//then grab the encoded contents back out. The div never exists on the page.
			return $('<div/>').text(value).html();
		}

	}

//}
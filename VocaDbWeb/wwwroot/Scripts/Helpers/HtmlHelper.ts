export default class HtmlHelper {
  // Bolds and HTML encodes a term
  public static boldAndHtmlEncode(text: string, term: string): string {
    if (!text || !term) return text;

    var index = text.toLowerCase().indexOf(term.toLowerCase());

    if (index < 0) return HtmlHelper.htmlEncode(text);

    var actualTerm = text.substring(index, index + term.length);

    // Encode parts before match, the match itself and after match.
    return (
      HtmlHelper.htmlEncode(text.substr(0, index)) +
      '<b>' +
      HtmlHelper.htmlEncode(actualTerm) +
      '</b>' +
      HtmlHelper.htmlEncode(text.substr(index + term.length))
    );
  }

  public static formatMarkdown(
    value: string,
    callback?: (err: any, content: string) => void,
  ): void {
    if (!value) callback(null, '');
    // Using GitHub-flavored markdown with simple line breaks and HTML sanitation.
    marked(value, { gfm: true, breaks: true, sanitize: true }, callback);
  }

  public static htmlEncode(value: string): string {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out. The div never exists on the page.
    return $('<div/>').text(value).html();
  }
}

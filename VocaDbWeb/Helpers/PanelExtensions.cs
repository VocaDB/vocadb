using System;
using System.Web.Mvc;

namespace VocaDb.Web.Helpers {

	public static class PanelExtensions {

		public static IDisposable BeginErrorPanel(this HtmlHelper html) {
			return new PanelScope(html, PanelType.Error);
		}

		public static IDisposable BeginHighlightPanel(this HtmlHelper html) {
			return new PanelScope(html, PanelType.Highlight);
		}

		public static IDisposable BeginPanel(this HtmlHelper html) {
			return new PanelScope(html, PanelType.Normal);
		}

	}

	public enum PanelType {

		Normal,

		Highlight,

		Error

	}

	public class PanelScope : IDisposable {

		private readonly HtmlHelper html;

		public PanelScope(HtmlHelper html, PanelType panelType) {

			this.html = html;

			string panelClass = string.Empty;

			switch (panelType) {
				case PanelType.Highlight:
					panelClass = "ui-state-highlight ";
					break;
				case PanelType.Error:
					panelClass = "ui-state-error ";
					break;
			}

			html.ViewContext.Writer.WriteLine(
				"<div class=\"ui-widget\">" +
				"<div class=\"" + panelClass + " ui-corner-all panel\">"
			);

		}

		public void Dispose() {

			html.ViewContext.Writer.WriteLine(
				"</div></div>"
			);

		}

	}

}
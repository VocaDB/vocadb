using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.Rankings {

	public class VocaloidismWVRParser {

		private static readonly Regex numRegex = new Regex(@"\d+");

		private string GetNicoId(string nicoUrl) {

			return VideoService.NicoNicoDouga.GetIdByUrl(nicoUrl);

		}

		private RankingContract GetSongs(Stream htmlStream, string encodingStr) {
			
			var encoding = (!string.IsNullOrEmpty(encodingStr) ? Encoding.GetEncoding(encodingStr) : Encoding.UTF8);

			var songs = new List<SongInRankingContract>();

			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);

			var middleElem = doc.DocumentNode.SelectSingleNode("//td[@id = 'main']");
			//var headLineElems = middleElem.("//div[contains(class, post-headline)]");
			var titleDiv = middleElem.SelectSingleNode("div[2]/div[1]/h1");
			var titleText = titleDiv.InnerText;
			var titleNumPart = numRegex.Match(titleText);
			var wvrId = int.Parse(titleNumPart.Value);

			var postDiv = middleElem.SelectSingleNode("//div[contains(class, category-vocaloid-ranking)]");
			var ulElem = postDiv.SelectSingleNode("//blockquote/ul");
			var liElems = ulElem.Elements("li");

			foreach (var liElem in liElems) {
				ProcessElem(liElem, songs);
			}

			return new RankingContract { WVRId = wvrId, Name = titleText, Songs = songs.ToArray() };

		}

		private void ProcessElem(HtmlNode liElem, List<SongInRankingContract> songs) {

			var spanElem = liElem.Element("span");

			if (spanElem == null)
				return;

			var strongElem = spanElem.SelectSingleNode("strong");

			if (strongElem == null)
				return;	// List has items without order number (like "ED song") - skip them

			var orderNumStr = strongElem.InnerText;
			var numPart = numRegex.Match(orderNumStr);
			var orderNum = int.Parse(numPart.Value);

			var aElem = liElem.Element("a");
			var nicoUrl = aElem.Attributes["href"].Value;
			var nicoId = GetNicoId(nicoUrl);

			var name = aElem.InnerText;

			songs.Add(new SongInRankingContract { Name = name, NicoId = nicoId, SortIndex = orderNum });

			foreach (var childElem in liElem.ChildNodes)
				ProcessElem(childElem, songs);
		}

		public RankingContract GetSongs(Uri url) {

			var request = WebRequest.Create(url);
			using (var response = request.GetResponse()) {
				var enc = response.Headers[HttpResponseHeader.ContentEncoding];
				return GetSongs(response.GetResponseStream(), enc);				
			}

			/*using (var client = WebRequest.Create(url)) {
				
				client.

				using (var stream = client.OpenRead(url)) {

					return GetSongs(stream);

				}

			}*/

		}

	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.Service.Helpers {

	[TestClass]
	public class ArtistExternalUrlParserTests {

		private void TestGetExternalUrl(string input, string expected) {
			
			var result = new ArtistExternalUrlParser().GetExternalUrl(input);

			Assert.AreEqual(expected, result, input);

		}

		[TestMethod]
		public void Partial() {
			
			TestGetExternalUrl("mylist/6667938", "http://www.nicovideo.jp/mylist/6667938");

		}

		[TestMethod]
		public void Full() {
			
			TestGetExternalUrl("http://www.nicovideo.jp/mylist/6667938", "http://www.nicovideo.jp/mylist/6667938");

		}

		[TestMethod]
		public void NoMatch() {
			
			TestGetExternalUrl("http://www.nicovideo.jp", null);

		}

	}

}

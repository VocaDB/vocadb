using System;

namespace VocaDb.Web.Code {

	public class SloganGenerator {

		private static readonly string[] slogans = {
			"Now with 3939% more Miku",
			"Telling you who's whoo.", 
			"1st place to check.", 
			"We got APIs and ApiMikus.", 
			"Use the search, Luka!",
			"shu-t up and enjoy",
			"Now with 39% more Miku",
			"Our site uses Micookies"
		};

		public static string Generate() {

			var result = slogans[0] ?? slogans[new Random().Next(slogans.Length)];

			return result;

		}

	}

}
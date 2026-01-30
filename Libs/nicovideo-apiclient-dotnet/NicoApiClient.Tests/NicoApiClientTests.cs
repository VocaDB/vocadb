using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.NicoApi.Tests.TestData;

namespace VocaDb.NicoApi.Tests {

	/// <summary>
	/// Unit tests for <see cref="NicoApiClient"/>.
	/// </summary>
	[TestClass]
	public class NicoApiClientTests {

        private static NicoResponse GetResponse(Stream stream) {
            using (stream) {
                return XmlRequest.GetXmlResponse<NicoResponse>(stream);  
            }
        } 

        private static VideoDataResult GetResult(Stream stream) => new NicoApiClient().ParseResponse(GetResponse(stream));

        [TestMethod]
        public void GetResponse_Ok() {
			
            var result = GetResult(TestDataHelper.NicoResponseOk);

            Assert.AreEqual("【初音ミク】１７：００【オリジナル曲】", result.Title, "Title");
            Assert.AreEqual("https://tn.smilevideo.jp/smile?i=12464004", result.ThumbUrl, "ThumbUrl");
            Assert.IsNotNull(result.UploadDate, "UploadDate");
            Assert.AreEqual(new DateTime(2010, 10, 17).Date, result.UploadDate.Value.Date, "UploadDate");
            Assert.AreEqual(178, result.LengthSeconds, "LengthSeconds");
            Assert.AreEqual("14270239", result.AuthorId, "AuthorId");
            Assert.AreEqual("ProjectDIVAチャンネル", result.Author, "Author");
            Assert.AreEqual(11, result.Tags.Length, "Tags.Length");
            Assert.IsTrue(result.Tags.Any(t => t.Name == "VOCALOID"), "Found tag");

        }

        [TestMethod]
        public void GetResponse_Error() {
			
            var response = GetResponse(TestDataHelper.NicoResponseError);

            try {
                new NicoApiClient().ParseResponse(response);
                Assert.Fail("Should throw");
            } catch (NicoApiException x) {
                Assert.AreEqual("not found or invalid", x.NicoError, "NicoError");
            }

        }

		[TestMethod]
		public void ParseLength_LessThan10Mins() {

			var result = NicoApiClient.ParseLength("3:09");

			Assert.AreEqual(189, result, "result");

		}

		[TestMethod]
		public void ParseLength_MoreThan10Mins() {

			var result = NicoApiClient.ParseLength("39:39");

			Assert.AreEqual(2379, result, "result");

		}

		[TestMethod]
		public void ParseLength_MoreThan60Mins() {

			var result = NicoApiClient.ParseLength("339:39");

			Assert.AreEqual(20379, result, "result");

		}

	}

}

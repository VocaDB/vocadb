using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	/// <summary>
	/// Code from https://github.com/iDoRecall/email-normalize/blob/0938e0a4710c6fc076c50dd42ea2886b2984e219/tests.js
	/// </summary>
	[TestClass]
	public class MailAddressNormalizerTests {

		private async Task<string> NormalizedAddress(string address, MailAddressNormalizerOptions options = MailAddressNormalizerOptions.None) {
			return await MailAddressNormalizer.NormalizeAsync(address, options);
		}

		[TestMethod]
		public async Task OnlySupportedDomains() {
			Assert.AreEqual("a.b.c+tag@example.com", await NormalizedAddress("a.b.c+tag@example.com"));
		}

		[TestMethod]
		public async Task GmailDots() {Assert.AreEqual("abc@gmail.com", await NormalizedAddress("a.b.c@gmail.com"));
			Assert.AreEqual("a.b.c@yahoo.com", await NormalizedAddress("a.b.c@yahoo.com"));
		}

		[TestMethod]
		public async Task Plus() {
			Assert.AreEqual("abc@gmail.com", await NormalizedAddress("a.b.c+tag@gmail.com"));
			Assert.AreEqual("a.b.c+tag@yahoo.com", await NormalizedAddress("a.b.c+tag@yahoo.com"));
		}

		[TestMethod]
		public async Task NonStandardTlds() {
			Assert.AreEqual("a.b.c+tag@something.co.uk", await NormalizedAddress("a.b.c+tag@something.co.uk"));
			Assert.AreEqual("abc@something.co.uk", await NormalizedAddress("a.b.c+tag@something.co.uk", MailAddressNormalizerOptions.ForceRemoveDots | MailAddressNormalizerOptions.ForceRemoveTags));
		}

		[TestMethod]
		public async Task Yahoo() {
			Assert.AreEqual("a.b.c+tag@yahoo.com", await NormalizedAddress("a.b.c+tag@yahoo.com"));
			Assert.AreEqual("a.b.c@yahoo.com", await NormalizedAddress("a.b.c-tag@yahoo.com"));
			Assert.AreEqual("a.b.c@yahoo.co.uk", await NormalizedAddress("a.b.c-tag@yahoo.co.uk"));
			Assert.AreEqual("a@yahoo.ro", await NormalizedAddress("a-b.c-tag@yahoo.ro"));
		}

		[TestMethod]
		public async Task Microsoft() {
			Assert.AreEqual("a.b.c@outlook.com", await NormalizedAddress("a.b.c+tag@outlook.com"));
			Assert.AreEqual("a.b.c-tag@hotmail.com", await NormalizedAddress("a.b.c-tag@hotmail.com"));
			Assert.AreEqual("a.b.c-tag@outlook.co.uk", await NormalizedAddress("a.b.c-tag@outlook.co.uk"));
			Assert.AreEqual("a.b.c@live.com", await NormalizedAddress("a.b.c+d@live.com"));
		}

		[TestMethod]
		public async Task GoogleAppsForWork() {
			Assert.AreEqual("a.b.c+tag@idorecall.com", await NormalizedAddress("a.b.c+tag@idorecall.com"));
			Assert.AreEqual("abc@blueseed.com", await NormalizedAddress("a.b.c+tag@blueseed.com", MailAddressNormalizerOptions.DetectProvider));
		}

		[TestMethod]
		public async Task Fastmail() {
			Assert.AreEqual("a.b.c@fastmail.com", await NormalizedAddress("a.b.c+tag@fastmail.com"));
			Assert.AreEqual("a.b.c@fastmail.fm", await NormalizedAddress("a.b.c+tag@fastmail.fm"));
			// http://outcoldman.com/en/archive/2014/05/08/fastmail/
			Assert.AreEqual("denis+tag@outcoldman.com", await NormalizedAddress("denis+tag@outcoldman.com"));
		}

		[TestMethod]
		public async Task AsyncTestGoogleAppsForWork() {
			Assert.AreEqual("abc@blueseed.com", await NormalizedAddress("a.b.c+tag@blueseed.com", MailAddressNormalizerOptions.DetectProvider));
		}

		[TestMethod]
		public async Task AsyncTestFastmail() {
			Assert.AreEqual("notpublic@denis.gladkikh.email", await NormalizedAddress("notpublic+tag@denis.gladkikh.email", MailAddressNormalizerOptions.DetectProvider));
		}

		[TestMethod]
		public async Task AsyncTestNoSpecialProvider() {
			Assert.AreEqual("ad.missions+impossible@stanford.edu", await NormalizedAddress("ad.missions+impossible@stanford.edu", MailAddressNormalizerOptions.DetectProvider));
		}

	}

}

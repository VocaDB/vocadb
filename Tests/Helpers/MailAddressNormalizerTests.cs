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

		private async void TestNormalizedEmail(string expected, string given, MailAddressNormalizerOptions options = MailAddressNormalizerOptions.None) {
			Assert.AreEqual(expected, await NormalizedAddress(given, options));
		}

		[TestMethod]
		public void OnlySupportedDomains() {
			TestNormalizedEmail("a.b.c+tag@example.com", "a.b.c+tag@example.com");
		}

		[TestMethod]
		public void GmailDots() {TestNormalizedEmail("abc@gmail.com", "a.b.c@gmail.com");
			TestNormalizedEmail("a.b.c@yahoo.com", "a.b.c@yahoo.com");
		}

		[TestMethod]
		public void Plus() {
			TestNormalizedEmail("abc@gmail.com", "a.b.c+tag@gmail.com");
			TestNormalizedEmail("a.b.c+tag@yahoo.com", "a.b.c+tag@yahoo.com");
		}

		[TestMethod]
		public void NonStandardTlds() {
			TestNormalizedEmail("a.b.c+tag@something.co.uk", "a.b.c+tag@something.co.uk");
			TestNormalizedEmail("abc@something.co.uk", "a.b.c+tag@something.co.uk", MailAddressNormalizerOptions.ForceRemoveDots | MailAddressNormalizerOptions.ForceRemoveTags);
		}

		[TestMethod]
		public void Yahoo() {
			TestNormalizedEmail("a.b.c+tag@yahoo.com", "a.b.c+tag@yahoo.com");
			TestNormalizedEmail("a.b.c@yahoo.com", "a.b.c-tag@yahoo.com");
			TestNormalizedEmail("a.b.c@yahoo.co.uk", "a.b.c-tag@yahoo.co.uk");
			TestNormalizedEmail("a@yahoo.ro", "a-b.c-tag@yahoo.ro");
		}

		[TestMethod]
		public void Microsoft() {
			TestNormalizedEmail("a.b.c@outlook.com", "a.b.c+tag@outlook.com");
			TestNormalizedEmail("a.b.c-tag@hotmail.com", "a.b.c-tag@hotmail.com");
			TestNormalizedEmail("a.b.c-tag@outlook.co.uk", "a.b.c-tag@outlook.co.uk");
			TestNormalizedEmail("a.b.c@live.com", "a.b.c+d@live.com");
		}

		[TestMethod]
		public void GoogleAppsForWork() {
			TestNormalizedEmail("a.b.c+tag@idorecall.com", "a.b.c+tag@idorecall.com");
			TestNormalizedEmail("abc@blueseed.com", "a.b.c+tag@blueseed.com", MailAddressNormalizerOptions.DetectProvider);
		}

		[TestMethod]
		public void Fastmail() {
			TestNormalizedEmail("a.b.c@fastmail.com", "a.b.c+tag@fastmail.com");
			TestNormalizedEmail("a.b.c@fastmail.fm", "a.b.c+tag@fastmail.fm");
			// http://outcoldman.com/en/archive/2014/05/08/fastmail/
			TestNormalizedEmail("denis+tag@outcoldman.com", "denis+tag@outcoldman.com");
		}

		[TestMethod]
		public void AsyncTestGoogleAppsForWork() {
			TestNormalizedEmail("abc@blueseed.com", "a.b.c+tag@blueseed.com", MailAddressNormalizerOptions.DetectProvider);
		}

		[TestMethod]
		public void AsyncTestFastmail() {
			TestNormalizedEmail("notpublic@denis.gladkikh.email", "notpublic+tag@denis.gladkikh.email", MailAddressNormalizerOptions.DetectProvider);
		}

		[TestMethod]
		public void AsyncTestNoSpecialProvider() {
			TestNormalizedEmail("ad.missions+impossible@stanford.edu", "ad.missions+impossible@stanford.edu", MailAddressNormalizerOptions.DetectProvider);
		}

	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DnsClient;

namespace VocaDb.Model.Helpers {

	[Flags]
	public enum MailAddressNormalizerOptions {
		None = 0,
		ForceRemoveDots = 1,
		ForceRemoveTags = 2,
		DetectProvider = 4
	}

	/// <summary>
	/// Code from https://github.com/iDoRecall/email-normalize/blob/0938e0a4710c6fc076c50dd42ea2886b2984e219/email.js
	/// </summary>
	public static class MailAddressNormalizer {

		public static readonly Dictionary<string, char> hostsWithTags = new Dictionary<string, char> {
			// Google only has two Gmail domains: https://en.wikipedia.org/wiki/List_of_Google_domains
			{ "gmail.com", '+' },
			{ "googlemail.com", '+' },
			{ "google.com", '+' },  // corporate email addresses; TODO presumably country domains also receive corporate email?
			// Microsoft
			{ "outlook.com", '+' },
			{ "hotmail.com", '+' },
			{ "live.com", '+' },
			// Fastmail - https://www.fastmail.com/help/receive/addressing.html TODO: whatever@username.fastmail.com -> username@fastmail.com
			{ "fastmail.com", '+' },
			{ "fastmail.fm", '+' },
			// Yahoo Mail Plus accounts, per https://en.wikipedia.org/wiki/Yahoo!_Mail#Email_domains, use hyphens - http://www.cnet.com/forums/discussions/did-yahoo-break-disposable-email-addresses-mail-plus-395088/
			{ "yahoo.com.ar", '-' },
			{ "yahoo.com.au", '-' },
			{ "yahoo.at", '-' },
			{ "yahoo.be/fr", '-' },
			{ "yahoo.be/nl", '-' },
			{ "yahoo.com.br", '-' },
			{ "ca.yahoo.com", '-' },
			{ "qc.yahoo.com", '-' },
			{ "yahoo.com.co", '-' },
			{ "yahoo.com.hr", '-' },
			{ "yahoo.cz", '-' },
			{ "yahoo.dk", '-' },
			{ "yahoo.fi", '-' },
			{ "yahoo.fr", '-' },
			{ "yahoo.de", '-' },
			{ "yahoo.gr", '-' },
			{ "yahoo.com.hk", '-' },
			{ "yahoo.hu", '-' },
			{ "yahoo.co.in/yahoo.in", '-' },
			{ "yahoo.co.id", '-' },
			{ "yahoo.ie", '-' },
			{ "yahoo.co.il", '-' },
			{ "yahoo.it", '-' },
			{ "yahoo.co.jp", '-' },
			{ "yahoo.com.my", '-' },
			{ "yahoo.com.mx", '-' },
			{ "yahoo.ae", '-' },
			{ "yahoo.nl", '-' },
			{ "yahoo.co.nz", '-' },
			{ "yahoo.no", '-' },
			{ "yahoo.com.ph", '-' },
			{ "yahoo.pl", '-' },
			{ "yahoo.pt", '-' },
			{ "yahoo.ro", '-' },
			{ "yahoo.ru", '-' },
			{ "yahoo.com.sg", '-' },
			{ "yahoo.co.za", '-' },
			{ "yahoo.es", '-' },
			{ "yahoo.se", '-' },
			{ "yahoo.ch/fr", '-' },
			{ "yahoo.ch/de", '-' },
			{ "yahoo.com.tw", '-' },
			{ "yahoo.co.th", '-' },
			{ "yahoo.com.tr", '-' },
			{ "yahoo.co.uk", '-' },
			{ "yahoo.com", '-' },
			{ "yahoo.com.vn", '-' }
		};

		public static string Normalize(MailAddress address, MailAddressNormalizerOptions options = MailAddressNormalizerOptions.None) => NormalizeAsync(address, options).Result;

		public static string Normalize(string address, MailAddressNormalizerOptions options = MailAddressNormalizerOptions.None) => NormalizeAsync(address, options).Result;

		public static async Task<string> NormalizeAsync(MailAddress address, MailAddressNormalizerOptions options = MailAddressNormalizerOptions.None) {
			var user = address.User;
			var host = address.Host.ToLower();

			if (options.HasFlag(MailAddressNormalizerOptions.ForceRemoveTags))
				user = Regex.Replace(user, @"[-+=].*", "");
			else {
				if (hostsWithTags.TryGetValue(host, out var separator))
					user = user.Split(separator)[0];
			}

			if (options.HasFlag(MailAddressNormalizerOptions.ForceRemoveDots) || Regex.IsMatch(host, @"^(gmail|googlemail|google)\.com$"))
				user = Regex.Replace(user, @"\.", "");

			if (host == "googlemail.com")
				host = "gmail.com";

			if (options.HasFlag(MailAddressNormalizerOptions.DetectProvider)) {
				// Detect custom domain email hosting providers TODO providers from https://news.ycombinator.com/item?id=8533588

				static string ProcessMXRecords(DnsString exchange, string user) {
					if (Regex.IsMatch(exchange.Value, @"aspmx.*google.*\.com\.?$", RegexOptions.IgnoreCase))
						return Regex.Replace(user.Split('+')[0], @"\.", "");

					if (Regex.IsMatch(exchange.Value, @"\.messagingengine\.com\.?$", RegexOptions.IgnoreCase))
						return user.Split('+')[0];

					return user;
				}

				var client = new LookupClient();
				var result = await client.QueryAsync(host, QueryType.MX);

				foreach (var record in result.Answers.MxRecords())
					user = ProcessMXRecords(record.Exchange, user);

				return user + "@" + host;
			}

			return user + "@" + host;
		}

		public static async Task<string> NormalizeAsync(string address, MailAddressNormalizerOptions options = MailAddressNormalizerOptions.None) => await NormalizeAsync(new MailAddress(address), options);

	}

}
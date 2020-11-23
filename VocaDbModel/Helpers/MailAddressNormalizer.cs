// Code from https://github.com/iDoRecall/email-normalize/blob/0938e0a4710c6fc076c50dd42ea2886b2984e219/email.js

using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Helpers
{
	public static class MailAddressNormalizer
	{
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

		/// <summary>
		/// Normalize an email address by removing the dots and address tag.
		/// </summary>
		/// <exception cref="ArgumentNullException">address is null.</exception>
		/// <exception cref="ArgumentException">address is System.String.Empty ("").</exception>
		/// <exception cref="FormatException">address is not in a recognized format.</exception>
		public static string Normalize(MailAddress address)
		{
			var user = address.User;
			var host = address.Host.ToLower();

			if (hostsWithTags.TryGetValue(host, out var separator))
				user = user.Split(separator)[0];

			if (Regex.IsMatch(host, @"^(gmail|googlemail|google)\.com$"))
				user = Regex.Replace(user, @"\.", "");

			if (host == "googlemail.com")
				host = "gmail.com";

			return user + "@" + host;
		}

		/// <summary>
		/// Normalize an email address by removing the dots and address tag.
		/// </summary>
		/// <exception cref="ArgumentNullException">address is null.</exception>
		/// <exception cref="ArgumentException">address is System.String.Empty ("").</exception>
		/// <exception cref="FormatException">address is not in a recognized format.</exception>
		public static string Normalize(string address) => Normalize(new MailAddress(address));
	}
}
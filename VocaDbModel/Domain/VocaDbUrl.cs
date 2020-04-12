using System;
using System.Diagnostics;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain {

	/// <summary>
	/// URL object with domain information (either main or static).
	/// Can be absolute or relative.
	/// </summary>
	[DebuggerDisplay("{DebugString}")]
	public class VocaDbUrl {

		public static VocaDbUrl Empty { get; } = new VocaDbUrl(string.Empty, UrlDomain.Main, UriKind.Absolute);

		public VocaDbUrl(string url, UrlDomain domain, UriKind kind) {
			Url = url;
			Domain = domain;
			Kind = kind;
		}

		public UrlDomain Domain { get; }
		public UriKind Kind { get; }
		public string Url { get; }

		public string DebugString => $"{Url} ({Domain})";
		public bool IsEmpty => string.IsNullOrEmpty(Url);

		public VocaDbUrl ToAbsolute() {

			switch (Kind) {
				case UriKind.Absolute:
					return this;
				case UriKind.Relative when Domain == UrlDomain.Main:
					return new VocaDbUrl(VocaUriBuilder.Absolute(Url), Domain, UriKind.Absolute);
				case UriKind.Relative when Domain == UrlDomain.Static:
					return new VocaDbUrl(VocaUriBuilder.StaticResource(Url), Domain, UriKind.Absolute);
				case UriKind.RelativeOrAbsolute when Domain == UrlDomain.Main:
					return new VocaDbUrl(VocaUriBuilder.AbsoluteFromUnknown(Url, true), Domain, UriKind.Absolute);
				case UriKind.Relative when Domain == UrlDomain.External:
				case UriKind.RelativeOrAbsolute when Domain == UrlDomain.Static:
				case UriKind.RelativeOrAbsolute when Domain == UrlDomain.External:
					throw new NotSupportedException($"No way to convert to absolute URL: {DebugString}");
				default:
					throw new InvalidOperationException(DebugString);
			}

		}

		public VocaDbUrl ToAbsoluteIfNotMain() => Domain == UrlDomain.Main ? this : ToAbsolute();

		public override string ToString() => Url;

	}

	public enum UrlDomain {
		/// <summary>
		/// https://vocadb.net
		/// </summary>
		Main,
		/// <summary>
		/// https://static.vocadb.net
		/// </summary>
		Static,
		/// <summary>
		/// External website
		/// </summary>
		External
	}
}

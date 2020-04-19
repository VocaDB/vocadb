using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain {

	/// <summary>
	/// URL object with domain information (either main, static or external).
	/// Can be absolute or relative.
	/// URLs are immutable.
	/// </summary>
	[DebuggerDisplay("{DebugString}")]
	public sealed class VocaDbUrl : IEquatable<VocaDbUrl> {

		public static readonly VocaDbUrl Empty = new VocaDbUrl(string.Empty, UrlDomain.Main, UriKind.Absolute);

		/// <summary>
		/// Creates an external URL.
		/// External URLs are always absolute.
		/// </summary>
		/// <param name="url">External URL, should be absolute. For example "https://nicovideo.cdn.nimg.jp/thumbnails/393939"</param>
		/// <returns>URL object.</returns>
		public static VocaDbUrl External(string url) => new VocaDbUrl(url, UrlDomain.External, UriKind.Absolute);
		public static IEnumerable<VocaDbUrl> External(IEnumerable<string> urls) => urls?.Select(url => External(url));

		public VocaDbUrl(string url, UrlDomain domain, UriKind kind) {
			Url = url ?? string.Empty;
			Domain = domain;
			Kind = kind;
		}

		public UrlDomain Domain { get; }
		public UriKind Kind { get; }
		public string Url { get; }

		public string DebugString => $"{Url} ({Domain})";
		public bool IsEmpty => string.IsNullOrEmpty(Url);

		public VocaDbUrl NullIfEmpty => IsEmpty ? null : this;

		/// <summary>
		/// Ensures that URL starts with scheme (http/https).
		/// </summary>
		public VocaDbUrl EnsureScheme() => new VocaDbUrl(UrlHelper.MakeLink(Url), Domain, Kind);

		/// <summary>
		/// Converts URL to absolute (with scheme and domain), if possible.
		/// External URLs cannot be converted.
		/// </summary>
		/// <returns>Absolute URL. Cannot be null.</returns>
		/// <exception cref="NotSupportedException">URL cannot be converted to absolute.</exception>
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

		/// <summary>
		/// Convert to <see cref="Uri"/>.
		/// </summary>
		/// <exception cref="UriFormatException">If <see cref="Url"/> does not represent valid <see cref="Uri"/>.</exception>
		public Uri ToUri() => new Uri(Url, Kind);

		/// <summary>
		/// Validates current URL.
		/// </summary>
		/// <exception cref="UriFormatException">If the URL is not valid.</exception>
		public void Validate() { ToUri(); }

		public override string ToString() => Url;

		public bool Equals(VocaDbUrl other) {
			return other != null && other.Domain == Domain && other.Url == Url;
		}

		public override bool Equals(object obj) => Equals(obj as VocaDbUrl);

		public override int GetHashCode() {
			int hashCode = -120357769;
			hashCode = hashCode * -1521134295 + Domain.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Url);
			return hashCode;
		}

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

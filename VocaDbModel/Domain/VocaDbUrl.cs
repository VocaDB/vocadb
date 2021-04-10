#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// URL object with domain information (either main, static or external).
	/// Can be absolute or relative.
	/// URLs are immutable.
	/// </summary>
	[DebuggerDisplay("{DebugString}")]
	public class VocaDbUrl : IEquatable<VocaDbUrl>
	{
		public static VocaDbUrl Empty { get; } = new(string.Empty, UrlDomain.Main, UriKind.Absolute);
		public static VocaDbUrl External(string url) => new(url, UrlDomain.External, UriKind.Absolute);

		public VocaDbUrl(string url, UrlDomain domain, UriKind kind)
		{
			Url = url;
			Domain = domain;
			Kind = kind;
		}

		public UrlDomain Domain { get; }
		public UriKind Kind { get; }
		public string Url { get; }

		public string DebugString => $"{Url} ({Domain})";
		public bool IsEmpty => string.IsNullOrEmpty(Url);

		/// <summary>
		/// Converts URL to absolute (with scheme and domain), if possible.
		/// External URLs cannot be converted.
		/// </summary>
		/// <returns>Absolute URL. Cannot be null.</returns>
		/// <exception cref="NotSupportedException">URL cannot be converted to absolute.</exception>
		public VocaDbUrl ToAbsolute()
		{
			switch (Kind)
			{
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

#nullable enable
		public bool Equals(VocaDbUrl? other)
		{
			return other != null && other.Domain == Domain && other.Url == Url;
		}

		public override bool Equals(object? obj) => Equals(obj as VocaDbUrl);
#nullable disable

		public override int GetHashCode()
		{
			int hashCode = -120357769;
			hashCode = hashCode * -1521134295 + Domain.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Url);
			return hashCode;
		}
	}

	public enum UrlDomain
	{
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

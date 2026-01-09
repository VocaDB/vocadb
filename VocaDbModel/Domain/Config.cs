#nullable disable

namespace VocaDb.Model.Domain;

public class Config : IEntryWithIntId
{
	public virtual int Id { get; set; }
	public virtual string Type { get; set; }
	public virtual string Value { get; set; }
	public virtual DateTime Updated { get; set; }

	public Config()
	{
		Updated = DateTime.Now;
	}

	public Config(string type, string value) : this()
	{
		Type = type;
		Value = value;
	}
}

public static class ConfigType
{
	public const string Frontpage = "Frontpage";
	// Future config types:
	// public const string SiteSettings = "SiteSettings";
	// public const string FeatureFlags = "FeatureFlags";
}

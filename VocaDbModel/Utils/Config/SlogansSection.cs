using System.Configuration;
using System.Linq;
using System.Xml;

namespace VocaDb.Model.Utils.Config
{
	public class SlogansSection : ConfigurationSection
	{
		public SloganElement[] Slogans { get; set; }
	}

	public class SlogansSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			var slogans = section.ChildNodes.Cast<XmlNode>().Select(n => new SloganElement(n.InnerText));

			return new SlogansSection { Slogans = slogans.ToArray() };
		}
	}

	public class SloganElement
	{
		public SloganElement(string value)
		{
			Value = value;
		}

		public string Value { get; set; }
	}
}

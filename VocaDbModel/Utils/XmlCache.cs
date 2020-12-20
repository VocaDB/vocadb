#nullable disable

using System.Collections.Generic;
using System.Xml.Linq;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Utils
{
	public class XmlCache<T>
	{
		private readonly IDictionary<int, T> _cached = new Dictionary<int, T>();

		public T Deserialize(int key, XDocument doc)
		{
			if (_cached.ContainsKey(key))
				return _cached[key];

			var data = XmlHelper.DeserializeFromXml<T>(doc);

			_cached.Add(key, data);

			return data;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Utils {

	public class XmlCache<T> {

		private readonly IDictionary<int, T> cached = new Dictionary<int, T>();

		public T Deserialize(int key, XDocument doc) {

			if (cached.ContainsKey(key))
				return cached[key];

			var data = XmlHelper.DeserializeFromXml<T>(doc);

			cached.Add(key, data);

			return data;

		}

	}

}

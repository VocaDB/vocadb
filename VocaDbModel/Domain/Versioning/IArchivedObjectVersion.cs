using System.Xml.Linq;

namespace VocaDb.Model.Domain.Versioning
{
	public interface IArchivedObjectVersion : IDatabaseObject
	{
		XDocument Data { get; }

		bool Hidden { get; set; }
	}
}

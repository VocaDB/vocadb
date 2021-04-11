#nullable disable

namespace VocaDb.Model.Domain
{
	public class EntryIdAndName
	{
		public int Id { get; set; }

		public string Name { get; set; }

#nullable enable
		public override string ToString()
		{
			return $"{Name} [{Id}]";
		}
#nullable disable
	}
}

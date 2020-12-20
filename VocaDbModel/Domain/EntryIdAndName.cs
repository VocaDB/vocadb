#nullable disable

namespace VocaDb.Model.Domain
{
	public class EntryIdAndName
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return $"{Name} [{Id}]";
		}
	}
}

#nullable disable

namespace VocaDb.Model.DataContracts;

public class FrontpageBannerContract
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string ImageUrl { get; set; }
	public string LinkUrl { get; set; }
	public bool Enabled { get; set; }
	public int SortIndex { get; set; }
}

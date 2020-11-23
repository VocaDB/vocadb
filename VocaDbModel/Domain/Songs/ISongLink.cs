namespace VocaDb.Model.Domain.Songs
{

	/// <summary>
	/// Child entity linked to a song.
	/// </summary>
	public interface ISongLink
	{

		Song Song { get; set; }

	}
}

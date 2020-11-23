using System.Runtime.Serialization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistForEventContract : ObjectRefContract
	{
		public ArchivedArtistForEventContract() { }

		public ArchivedArtistForEventContract(ArtistForEvent entry)
		{
			Roles = entry.Roles;

			if (entry.Artist != null)
			{
				Id = entry.Artist.Id;
				NameHint = entry.Artist.DefaultName;
			}
			else
			{
				NameHint = entry.Name;
			}
		}

		[DataMember]
		public ArtistEventRoles Roles { get; set; }
	}
}

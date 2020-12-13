using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists
{
	public class ArtistMap : ClassMap<Artist>
	{
		public ArtistMap()
		{
			Cache.ReadWrite();

			Id(m => m.Id);
			Map(m => m.ArtistType).Not.Nullable();
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.PictureMime).Length(32).Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.BaseVoicebank).Nullable();

			HasMany(m => m.AllAlbums).Table("ArtistsForAlbums")
				.Inverse()
				.Cascade.All()
				.Cache.ReadWrite();
			HasMany(m => m.AllEvents).Table("ArtistsForEvents")
				.Inverse();
			HasMany(m => m.AllGroups)
				.Inverse()
				.KeyColumn("[Member]")
				.Cascade.All()
				.Cache.ReadWrite();
			HasMany(m => m.AllSongs).Table("ArtistsForSongs")
				.Inverse()
				.Cascade.All()
				.Cache.ReadWrite();
			HasMany(m => m.AllMembers).Inverse().KeyColumn("[Group]").Cache.ReadWrite();
			HasMany(m => m.ChildVoicebanks)
				.Inverse()
				.KeyColumn("[BaseVoicebank]")
				.Cache.ReadWrite();
			HasMany(m => m.AllComments).Inverse().Cascade.AllDeleteOrphan().OrderBy("Created");
			HasMany(m => m.Hits).Inverse().Cascade.AllDeleteOrphan();
			HasMany(m => m.WebLinks).Table("ArtistWebLinks").Inverse().Cascade.All().Cache.ReadWrite();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Artist]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Description, c =>
			{
				c.Map(m => m.Original).Column("Description").Not.Nullable().Length(int.MaxValue);
				c.Map(m => m.English).Column("DescriptionEng").Not.Nullable().Length(int.MaxValue);
			});

			Component(m => m.Names, c =>
			{
				c.Map(m => m.AdditionalNamesString).Not.Nullable().Length(1024);
				c.HasMany(m => m.Names).Table("ArtistNames").KeyColumn("[Artist]").Inverse().Cascade.All().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 =>
				{
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
				});
			});

			Component(m => m.Picture, c =>
			{
				c.Map(m => m.Bytes, "PictureBytes").Length(int.MaxValue);
			}).LazyLoad();

			Component(m => m.Pictures, c =>
			{
				c.HasMany(m => m.Pictures).KeyColumn("[Artist]").Inverse().Cascade.All().Cache.ReadWrite();
			});

			Component(m => m.ReleaseDate, c => c.Map(m => m.DateTime).Column("ReleaseDate").Nullable());

			Component(m => m.Tags, c =>
			{
				c.HasMany(m => m.Usages).Table("ArtistTagUsages").KeyColumn("[Artist]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			});

			//HasMany(m => m.Hits).Inverse().Cascade.AllDeleteOrphan();
			HasMany(m => m.OwnerUsers).Inverse().Cache.ReadWrite();
			HasMany(m => m.Users).Inverse();
		}
	}

	public class ArchivedArtistVersionMap : ClassMap<ArchivedArtistVersion>
	{
		public ArchivedArtistVersionMap()
		{
			Id(m => m.Id);

			Map(m => m.AgentName).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Hidden).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.PictureMime).Length(32).Nullable();
			Map(m => m.Reason).Length(30).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Artist);
			References(m => m.Author);

			Component(m => m.Diff, c =>
			{
				c.Map(m => m.ChangedFieldsString, ClassConventions.EscapeColumn("ChangedFields")).Length(1000).Not.Nullable();
				c.Map(m => m.IsSnapshot).Not.Nullable();
			});

			Component(m => m.Picture, c =>
			{
				c.Map(m => m.Bytes, "PictureBytes").Length(int.MaxValue);
			}).LazyLoad();
		}
	}

	public class ArtistForArtistMap : ClassMap<ArtistForArtist>
	{
		public ArtistForArtistMap()
		{
			Table("GroupsForArtists");
			Id(m => m.Id);

			Map(m => m.LinkType).Not.Nullable();
			References(m => m.Parent).Column("[Group]").Not.Nullable();
			References(m => m.Member).Not.Nullable();
		}
	}

	public class ArtistHitMap : EntryHitMap<ArtistHit, Artist> { }
}

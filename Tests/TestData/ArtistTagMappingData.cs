#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Tags;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.TestData;

public class ArtistTagMappingData
{
	public Tag AceTag { get; private set; }
	public Tag PocketSingerTag { get; private set; }
	public Tag PocketSingerChildTag { get; private set; }
	public Tag PocketSingerGrandchildTag { get; private set; }
	public Tag PocketSingerGreatGrandchildTag { get; private set; }
	public Tag CeVioTag { get; private set; }
	public Tag SynthesizerVTag { get; private set; }
	public Tag VoicevoxTag { get; private set; }
	public Tag UtauTag { get; private set; }
	public Tag ProducerTag { get; private set; }
	public Tag ArtistsTag { get; private set; }
	public Tag AlbumTag { get; private set; }

	public Artist AceUntagged { get; private set; }
	public Artist AceTagged { get; private set; }
	public Artist AceTaggedWithOtherTag { get; private set; }
	public Artist AceTaggedVocaloid { get; private set; }
	public Artist CeVioArtist { get; private set; }
	public Artist SynthesizerVArtist { get; private set; }
	public Artist VoicevoxArtist { get; private set; }
	public Artist UtauArtist { get; private set; }
	public Artist ProducerArtist { get; private set; }
	public Artist UnrelatedArtist { get; private set; }
	public Artist ArtistsTaggedArtist { get; private set; }
	public Artist PocketSingerTaggedArtist { get; private set; }
	public Artist ChildMappedArtist { get; private set; }
	public Artist GrandchildMappedArtist { get; private set; }
	public Artist GreatGrandchildMappedArtist { get; private set; }

	public IEnumerable<Tag> Tags => new[]
	{
		AceTag,
		PocketSingerTag,
		PocketSingerChildTag,
		PocketSingerGrandchildTag,
		PocketSingerGreatGrandchildTag,
		CeVioTag,
		SynthesizerVTag,
		VoicevoxTag,
		UtauTag,
		ProducerTag,
		ArtistsTag,
		AlbumTag
	};

	public IEnumerable<Artist> Artists => new[]
	{
		AceUntagged,
		AceTagged,
		AceTaggedWithOtherTag,
		AceTaggedVocaloid,
		CeVioArtist,
		SynthesizerVArtist,
		VoicevoxArtist,
		UtauArtist,
		ProducerArtist,
		UnrelatedArtist,
		ArtistsTaggedArtist,
		PocketSingerTaggedArtist,
		ChildMappedArtist,
		GrandchildMappedArtist,
		GreatGrandchildMappedArtist
	};

	public IEnumerable<EntryTypeToTagMapping> Mappings => new[]
	{
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.ACEVirtualSinger), AceTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.CeVIO), CeVioTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.SynthesizerV), SynthesizerVTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.VOICEVOX), VoicevoxTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.UTAU), UtauTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.Producer), ProducerTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.OtherVoiceSynthesizer), PocketSingerChildTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.Vocalist), PocketSingerGrandchildTag),
		new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Artist, ArtistType.NewType), PocketSingerGreatGrandchildTag),
		new EntryTypeToTagMapping(new EntryTypeAndSubType(EntryType.Artist), ArtistsTag),
		new EntryTypeToTagMapping(new EntryTypeAndSubType(EntryType.Album, "Album"), AlbumTag)
	};

	public void Save<T>(FakeRepository<T> repository) where T : class, IDatabaseObject
	{
		repository.Save(Tags.ToArray());
		repository.Save(Artists.ToArray());
		repository.Save(Mappings.ToArray());
	}

	private static void TagArtist(Artist artist, Tag tag)
	{
		var usage = new ArtistTagUsage(artist, tag);
		artist.Tags.Usages.Add(usage);
		tag.AllArtistTagUsages.Add(usage);
	}

	public static ArtistTagMappingData Create()
	{
		var data = new ArtistTagMappingData
		{
			AceTag = CreateEntry.Tag("ACE虚拟歌姬", id: 8863),
			PocketSingerTag = CreateEntry.Tag("Pocket Singer", id: 9680),
			PocketSingerChildTag = CreateEntry.Tag("Pocket Singer voices", id: 9101),
			PocketSingerGrandchildTag = CreateEntry.Tag("Pocket Singer legacy", id: 9102),
			PocketSingerGreatGrandchildTag = CreateEntry.Tag("Pocket Singer archive", id: 9103),
			CeVioTag = CreateEntry.Tag("CeVIO", id: 2838),
			SynthesizerVTag = CreateEntry.Tag("Synthesizer V", id: 6745),
			VoicevoxTag = CreateEntry.Tag("VOICEVOX", id: 8533),
			UtauTag = CreateEntry.Tag("UTAU", id: 447),
			ProducerTag = CreateEntry.Tag("music producer", id: 300),
			ArtistsTag = CreateEntry.Tag("artists", id: 7372),
			AlbumTag = CreateEntry.Tag("original album", id: 12628)
		};

		data.PocketSingerTag.Parent = data.AceTag;
		data.AceTag.AllChildren.Add(data.PocketSingerTag);

		data.PocketSingerChildTag.Parent = data.PocketSingerTag;
		data.PocketSingerTag.AllChildren.Add(data.PocketSingerChildTag);

		data.PocketSingerGrandchildTag.Parent = data.PocketSingerChildTag;
		data.PocketSingerChildTag.AllChildren.Add(data.PocketSingerGrandchildTag);

		data.PocketSingerGreatGrandchildTag.Parent = data.PocketSingerGrandchildTag;
		data.PocketSingerGrandchildTag.AllChildren.Add(data.PocketSingerGreatGrandchildTag);

		data.AceUntagged = CreateEntry.Artist(ArtistType.ACEVirtualSinger, name: "404FCOUSE");
		data.AceTagged = CreateEntry.Artist(ArtistType.ACEVirtualSinger, name: "70D");
		data.AceTaggedWithOtherTag = CreateEntry.Artist(ArtistType.ACEVirtualSinger, name: "Aaron");
		data.AceTaggedVocaloid = CreateEntry.Artist(ArtistType.Vocaloid, name: "Yamine Renri");
		data.CeVioArtist = CreateEntry.Artist(ArtistType.CeVIO, name: "Ci flower");
		data.SynthesizerVArtist = CreateEntry.Artist(ArtistType.SynthesizerV, name: "Amara");
		data.VoicevoxArtist = CreateEntry.Artist(ArtistType.VOICEVOX, name: "WhiteCUL");
		data.UtauArtist = CreateEntry.Artist(ArtistType.UTAU, name: "Kasane Teto");
		data.ProducerArtist = CreateEntry.Artist(ArtistType.Producer, name: "40mP");
		data.UnrelatedArtist = CreateEntry.Artist(ArtistType.Vocaloid, name: "Hatsune Miku");
		data.ArtistsTaggedArtist = CreateEntry.Artist(ArtistType.Vocaloid, name: "Kagamine Rin");
		data.PocketSingerTaggedArtist = CreateEntry.Artist(ArtistType.Vocaloid, name: "Sora");
		data.ChildMappedArtist = CreateEntry.Artist(ArtistType.OtherVoiceSynthesizer, name: "Child Voice");
		data.GrandchildMappedArtist = CreateEntry.Artist(ArtistType.Vocalist, name: "Legacy Voice");
		data.GreatGrandchildMappedArtist = CreateEntry.Artist(ArtistType.NewType, name: "Archive Voice");

		TagArtist(data.AceTagged, data.AceTag);
		TagArtist(data.AceTaggedVocaloid, data.AceTag);
		TagArtist(data.AceTaggedWithOtherTag, data.SynthesizerVTag);
		TagArtist(data.UtauArtist, data.UtauTag);
		TagArtist(data.ArtistsTaggedArtist, data.ArtistsTag);
		TagArtist(data.PocketSingerTaggedArtist, data.PocketSingerTag);

		return data;
	}
}

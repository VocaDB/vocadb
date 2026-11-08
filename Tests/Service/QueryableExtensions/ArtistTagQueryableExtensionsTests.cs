#nullable disable

using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.QueryableExtensions;

[TestClass]
public class ArtistTagQueryableExtensionsTests
{
	private readonly FakeArtistRepository _repository = new();
	private readonly ArtistTagMappingData _data = ArtistTagMappingData.Create();

	public ArtistTagQueryableExtensionsTests()
	{
		_data.Save(_repository);
	}

	private Artist[] Find(int[] tagIds, bool childTags = false)
	{
		using var context = _repository.CreateContext();
		return context.Query<Artist>().WhereHasTagsOrTypes(tagIds, childTags, context).ToArray();
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_MappedArtistTypeWithoutTag()
	{
		var result = Find(new[] { _data.AceTag.Id });

		result.Should().Contain(_data.AceUntagged);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_TaggedArtistOfUnmappedType()
	{
		var result = Find(new[] { _data.AceTag.Id });

		result.Should().Contain(_data.AceTaggedVocaloid);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_ExcludesUnmappedArtistTypeWithoutTag()
	{
		var result = Find(new[] { _data.AceTag.Id });

		result.Should().NotContain(_data.UnrelatedArtist);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_EachMappedTagIncludesItsType()
	{
		var cases = new[]
		{
			(_data.CeVioTag, _data.CeVioArtist),
			(_data.SynthesizerVTag, _data.SynthesizerVArtist),
			(_data.VoicevoxTag, _data.VoicevoxArtist),
			(_data.UtauTag, _data.UtauArtist),
			(_data.ProducerTag, _data.ProducerArtist)
		};

		foreach (var (tag, artist) in cases)
		{
			Find(new[] { tag.Id }).Should().Contain(artist, $"{artist.DefaultName} is linked to {tag.DefaultName}");
		}
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_ExcludesArtistsOfOtherMappedTypes()
	{
		var result = Find(new[] { _data.CeVioTag.Id });

		result.Should().NotContain(_data.SynthesizerVArtist);
		result.Should().NotContain(_data.AceUntagged);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_ChildTaggedArtistWithChildTags()
	{
		var result = Find(new[] { _data.AceTag.Id }, childTags: true);

		result.Should().Contain(_data.PocketSingerTaggedArtist);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_ChildTaggedArtistWithoutChildTags()
	{
		var result = Find(new[] { _data.AceTag.Id });

		result.Should().NotContain(_data.PocketSingerTaggedArtist);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_TypesMappedUpToThreeChildLevels()
	{
		var result = Find(new[] { _data.AceTag.Id }, childTags: true);

		result.Should().Contain(_data.ChildMappedArtist);
		result.Should().Contain(_data.GrandchildMappedArtist);
		result.Should().NotContain(_data.GreatGrandchildMappedArtist);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_MultipleTagsRequireAll()
	{
		var result = Find(new[] { _data.AceTag.Id, _data.SynthesizerVTag.Id });

		result.Should().Contain(_data.AceTaggedWithOtherTag);
		result.Should().NotContain(_data.AceUntagged);
		result.Should().NotContain(_data.SynthesizerVArtist);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_MappingWithoutSubTypeUsesTagUsages()
	{
		var result = Find(new[] { _data.ArtistsTag.Id });

		result.Should().ContainSingle().Which.Should().Be(_data.ArtistsTaggedArtist);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_IgnoresMappingsOfOtherEntryTypes()
	{
		Find(new[] { _data.AlbumTag.Id }).Should().BeEmpty();
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_UnknownTagIdReturnsNoArtists()
	{
		Find(new[] { 424242 }).Should().BeEmpty();
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_TaggedMappedArtistAppearsOnce()
	{
		var result = Find(new[] { _data.AceTag.Id });

		result.Count(a => a == _data.AceTagged).Should().Be(1);
	}

	[TestMethod]
	public void WhereHasTagsOrTypes_NoTagIdsReturnsAllArtists()
	{
		Find(Array.Empty<int>()).Length.Should().Be(_data.Artists.Count());
		Find(null).Length.Should().Be(_data.Artists.Count());
	}
}

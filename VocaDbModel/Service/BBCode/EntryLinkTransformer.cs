using System.Text;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.BBCode;

/// <summary>
/// Transforms entry page references (such as /Artist/3939) into links.
/// </summary>
public class EntryLinkTransformer : IBBCodeElementTransformer
{
	private readonly IEntryLinkFactory _linkFactory;

	public EntryLinkTransformer(IEntryLinkFactory linkFactory)
	{
		ParamIs.NotNull(() => linkFactory);

		_linkFactory = linkFactory;
	}

	private readonly Regex[] linkMatchers = new[] {
		new Regex(@"/(Album|Artist|Song|SongList|Tag|User)/Details/(\w+)"),
		new Regex(@"/(S)/(\w+)"),
	};

	private string GetLink(Match match)
	{
		var entryTypeName = match.Groups[1].Value;
		var entryIdStr = match.Groups[2].Value;

		if (entryTypeName == "S")
			entryTypeName = "Song";

		if (Enum.TryParse(entryTypeName, true, out EntryType entryType) && int.TryParse(entryIdStr, out int entryId))
			return _linkFactory.CreateEntryLink(entryType, entryId, match.Value);
		else
			return match.Value;
	}

	public void ApplyTransform(StringBuilder bbCode)
	{
		ParamIs.NotNull(() => bbCode);

		BBCodeConverter.RegexReplace(bbCode, linkMatchers[0], GetLink);
		BBCodeConverter.RegexReplace(bbCode, linkMatchers[1], GetLink);
	}
}

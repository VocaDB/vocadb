#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags;

[Obsolete]
public class TagCategoryContract
{
	public TagCategoryContract() { }

#nullable enable
	public TagCategoryContract(
		string name,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IEnumerable<Tag> tags
	)
	{
		ParamIs.NotNull(() => name);
		ParamIs.NotNull(() => tags);

		Name = name;
		Tags = tags.Select(t => new TagForApiContract(t, languagePreference, permissionContext, TagOptionalFields.AdditionalNames)).ToArray();
	}
#nullable disable

	public string Name { get; init; }

	public TagForApiContract[] Tags { get; init; }
}

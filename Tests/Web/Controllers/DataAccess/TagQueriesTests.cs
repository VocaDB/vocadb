#nullable disable

using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="TagQueries"/>.
	/// </summary>
	[TestClass]
	public class TagQueriesTests
	{
		private InMemoryImagePersister _imagePersister;
		private FakePermissionContext _permissionContext;
		private TagQueries _queries;
		private FakeTagRepository _repository;
		private Tag _tag;
		private Tag _tag2;
		private User _user;

		private ArchivedTagVersion GetArchivedVersion(Tag tag)
		{
			return _repository.List<ArchivedTagVersion>().FirstOrDefault(a => a.Tag.Id == tag.Id);
		}

		private Tag CreateAndSaveTag(string englishName)
		{
			var t = CreateEntry.Tag(englishName);

			_repository.Save(t);

			foreach (var name in t.Names)
				_repository.Save(name);

			return t;
		}

		private TagUsage CreateTagUsage(Tag tag, ReleaseEvent releaseEvent)
		{
			var usage = _repository.Save(new EventTagUsage(releaseEvent, tag));
			tag.AllEventTagUsages.Add(usage);
			releaseEvent.Tags.Usages.Add(usage);
			return usage;
		}

		private Stream TestImage()
		{
			return ResourceHelper.TestImage();
		}

		[TestInitialize]
		public void SetUp()
		{
			_repository = new FakeTagRepository();

			_tag = CreateAndSaveTag("Appearance Miku");
			_tag2 = CreateAndSaveTag("MMD");

			_user = new User("User", "123", "test@test.com", PasswordHashAlgorithms.Default) { GroupId = UserGroupId.Moderator };
			_repository.Add(_user);

			_permissionContext = new FakePermissionContext(new UserWithPermissionsContract(_user, ContentLanguagePreference.Default));

			_imagePersister = new InMemoryImagePersister();
			_queries = new TagQueries(_repository, _permissionContext, new FakeEntryLinkFactory(), _imagePersister, _imagePersister, new FakeUserIconFactory(), new EnumTranslations(), new FakeObjectCache());
		}

		[TestMethod]
		public async Task Create()
		{
			var result = await _queries.Create("Apimiku");

			result.Name.Should().Be("Apimiku", "Created tag name");
			var tagFromRepo = _repository.Load(result.Id);
			tagFromRepo.DefaultName.Should().Be("Apimiku", "Tag found from repository");
		}

		[TestMethod]
		public void Create_Duplicate()
		{
			_queries.Awaiting(subject => subject.Create("Appearance Miku")).Should().Throw<DuplicateTagNameException>();
		}

		[TestMethod]
		public async Task GetDetails_RecentEvents()
		{
			void AssertContainsEvent(TagDetailsContract details, ReleaseEvent releaseEvent)
			{
				details.Stats.Events.Any(e => e.Id == releaseEvent.Id).Should().BeTrue("Contains " + releaseEvent);
			}

			var standaloneEvent = CreateEntry.ReleaseEvent("Miku party");
			var otherEvent = CreateEntry.ReleaseEvent("Magical Mirai");
			var eventSeries = CreateEntry.EventSeries("VocaTRAVers");
			var seriesUsage = _repository.Save(new EventSeriesTagUsage(eventSeries, _tag));
			eventSeries.Tags.Usages.Add(seriesUsage);
			_tag.AllEventSeriesTagUsages.Add(seriesUsage);
			var oldSeriesEvent = CreateEntry.ReleaseEvent("VocaTRAVers 1", date: DateTime.Now.AddDays(-30));
			oldSeriesEvent.SetSeries(eventSeries);
			var recentSeriesEvent = CreateEntry.ReleaseEvent("VocaTRAVers 2", date: DateTime.Now);
			recentSeriesEvent.SetSeries(eventSeries);
			_repository.Save(standaloneEvent, otherEvent, oldSeriesEvent, recentSeriesEvent);
			_repository.Save(eventSeries);
			_repository.Save(CreateTagUsage(_tag, standaloneEvent), CreateTagUsage(_tag, oldSeriesEvent), CreateTagUsage(_tag, recentSeriesEvent));

			var result = await _queries.GetDetailsAsync(_tag.Id);

			result.Stats.EventCount.Should().Be(2, "EventCount");
			result.Stats.Events.Length.Should().Be(2, "Events.Length");
			result.Stats.EventSeriesCount.Should().Be(1, "EventSeriesCount");
			result.Stats.EventSeries.Length.Should().Be(1, "EventSeries.Length");
			AssertContainsEvent(result, standaloneEvent);
			AssertContainsEvent(result, recentSeriesEvent);
			result.Stats.EventSeries.Any(e => e.Id == eventSeries.Id).Should().BeTrue("Contains " + eventSeries);
		}

		[TestMethod]
		public void MoveToTrash()
		{
			var oldCount = _repository.Count<Tag>();

			_queries.MoveToTrash(_tag.Id, string.Empty);

			_repository.Count<Tag>().Should().Be(oldCount - 1, "One tag was removed");
			_repository.Contains(_tag).Should().BeFalse("Tag was removed from repository");

			_repository.Count<TrashedEntry>().Should().Be(1, "Trashed entry was created");
			var trashed = _repository.List<TrashedEntry>().First();
			trashed.EntryType.Should().Be(EntryType.Tag, "Trashed entry type");
			trashed.EntryId.Should().Be(_tag.Id, "Trashed entry ID");
			trashed.Name.Should().Be(_tag.DefaultName, "Trashed entry name");
		}

		[TestMethod]
		public void MoveToTrash_NoDeletePermission()
		{
			_user.GroupId = UserGroupId.Regular;
			_permissionContext.RefreshLoggedUser(_repository);

			_queries.Invoking(subject => subject.MoveToTrash(_tag.Id, string.Empty)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void MoveToTrash_NoEditPermission()
		{
			_user.GroupId = UserGroupId.Trusted;
			_permissionContext.RefreshLoggedUser(_repository);
			_tag.Status = EntryStatus.Locked;

			_queries.Invoking(subject => subject.MoveToTrash(_tag.Id, string.Empty)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void MoveToTrash_DeleteRelatedEntries()
		{
			_repository.Save(new TagReport(_tag, TagReportType.InvalidInfo, _user, "test", "test", null));
			_repository.Save(new TagReport(_tag2, TagReportType.InvalidInfo, _user, "test", "test", null));
			var song = _repository.Save(CreateEntry.Song());
			var tagUsage = _repository.Save(song.AddTag(_tag).Result);
			_tag.AllSongTagUsages.Add(tagUsage);

			_queries.MoveToTrash(_tag.Id, "test");

			_repository.Count<TagReport>().Should().Be(1, "Tag report was deleted");
			_repository.List<TagReport>().First().Entry.Should().Be(_tag2, "Report for the other tag still exists");
			song.Tags.HasTag(_tag).Should().BeFalse("Tag was removed from song");
		}

		[TestMethod]
		public void GetTagsByCategories()
		{
			_tag.CategoryName = "Animation";

			var result = _queries.GetTagsByCategories();

			result.Length.Should().Be(2, "Number of categories");
			var firstCategory = result[0];
			firstCategory.Name.Should().Be("Animation", "First category name");
			firstCategory.Tags.Length.Should().Be(1, "Number of tags in the Animation category");
			firstCategory.Tags[0].Name.Should().Be("Appearance Miku", "First tag in the Animation category");
		}

		[TestMethod]
		public void Merge_ToEmpty()
		{
			var target = _repository.Save(new Tag());

			_queries.Merge(_tag.Id, target.Id);

			target.Names.AllValues.FirstOrDefault().Should().Be("Appearance Miku", "Name was copied");

			var mergeRecord = _repository.List<TagMergeRecord>().FirstOrDefault(m => m.Source == _tag.Id);
			mergeRecord.Should().NotBeNull("Merge record was created");
			mergeRecord.Target.Should().Be(target, "Merge record target");
		}

		[TestMethod]
		public void Merge_MoveUsages()
		{
			// Arrange
			Action<Song, Tag> AddTag = (s, tag) =>
			{
				var u = s.AddTag(tag);
				tag.AllSongTagUsages.Add(u.Result);
				u.Result.CreateVote(_user);
			};

			var song = CreateEntry.Song();
			var song2 = CreateEntry.Song();
			var song3 = CreateEntry.Song();
			_repository.Save(song, song2, song3);

			AddTag(song, _tag);
			AddTag(song2, _tag);

			var target = _repository.Save(new Tag());
			AddTag(song2, target);
			AddTag(song3, target);

			// Act
			_queries.Merge(_tag.Id, target.Id);

			// Assert
			target.UsageCount.Should().Be(3, "Tag's UsageCount");
			target.AllSongTagUsages.Count.Should().Be(3, "Number of song tag usages");
			song.Tags.Usages.Count.Should().Be(1, "Number of usages for the first song");

			var usage = target.AllSongTagUsages.FirstOrDefault(s => s.Entry == song);
			usage.Should().NotBeNull("Found usage");
			usage.Votes.Count.Should().Be(1, "Number of votes");
			song.Tags.Usages.Contains(usage).Should().BeTrue("Same usage was added to song");
		}

		[TestMethod]
		public void Merge_NoPermissions()
		{
			_user.GroupId = UserGroupId.Regular;
			_permissionContext.RefreshLoggedUser(_repository);

			var target = new Tag();
			_repository.Save(target);

			_queries.Invoking(subject => subject.Merge(_tag.Id, target.Id)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void Merge_TransitiveMergeRecord()
		{
			var target = CreateAndSaveTag("target");
			var newTarget = CreateAndSaveTag("newTarget");

			_queries.Merge(_tag.Id, target.Id);
			_queries.Merge(target.Id, newTarget.Id);

			_repository.Count<TagMergeRecord>().Should().Be(2, "Two merge records created");
			var mergeRecord = _repository.List<TagMergeRecord>().FirstOrDefault(m => m.Source == _tag.Id);
			mergeRecord.Should().NotBeNull("Found merge record");
			mergeRecord.Target.Id.Should().Be(newTarget.Id, "Target was updated");
		}

		// Merge target is in related tags. Tag cannot be related to itself, so this it's skipped.
		[TestMethod]
		public void Merge_TargetInRelatedTags()
		{
			var target = _repository.Save(new Tag());
			_tag.AddRelatedTag(target);
			target.RelatedTags.Count.Should().Be(1, "Number of related tags");

			_queries.Merge(_tag.Id, target.Id);

			target.RelatedTags.Count.Should().Be(0, "Related tag (self) was not added");
		}

		[TestMethod]
		public void Merge_Parent()
		{
			var parent = _repository.Save(new Tag("parent"));
			var target = _repository.Save(new Tag("target"));
			_tag.SetParent(parent);

			_queries.Merge(_tag.Id, target.Id);

			target.Parent.Should().Be(parent, "Parent was set");
		}

		[TestMethod]
		public void Merge_Parent_IgnoreSelf()
		{
			var target = _repository.Save(new Tag("target"));
			_tag.SetParent(target);

			_queries.Merge(_tag.Id, target.Id);

			target.Parent.Should().BeNull("Parent was not set");
		}

		[TestMethod]
		public void Merge_FollowedTags()
		{
			var user2 = _repository.Save(CreateEntry.User());
			var target = _repository.Save(new Tag("target"));
			_user.AddTag(_tag);
			user2.AddTag(target);

			_queries.Merge(_tag.Id, target.Id);

			target.TagsForUsers.Count.Should().Be(2, "Followed tags were migrated");
			target.TagsForUsers.Any(t => t.User == _user).Should().BeTrue("Follow was migrated");
		}

		[TestMethod]
		public void Merge_TagMappings()
		{
			_repository.Save(_tag.CreateMapping("ApiMiku"));
			var target = _repository.Save(new Tag("target"));
			_queries.Merge(_tag.Id, target.Id);

			target.Mappings.Any(m => m.SourceTag == "ApiMiku").Should().BeTrue("Mapping was moved to target tag");
			_repository.List<TagMapping>().Any(m => m.SourceTag == "ApiMiku" && m.Tag.Equals(target)).Should().BeTrue("Mapped was saved to DB");
		}

		[TestMethod]
		public void Update_Description()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			updated.Description = new EnglishTranslatedStringContract { Original = "mikumikudance.wikia.com/wiki/Miku_Hatsune_Appearance_(Mamama)", English = string.Empty };

			_queries.Update(updated, null);

			_tag.Description.Original.Should().Be(updated.Description.Original, "Description was updated");

			var archivedVersion = _repository.List<ArchivedTagVersion>().FirstOrDefault(a => a.Tag.Id == _tag.Id);
			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(TagEditableFields.Description, "Changed fields");
		}

		[TestMethod]
		public void Update_Image()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			using (var stream = TestImage())
			{
				_queries.Update(updated, new UploadedFileContract { Mime = MediaTypeNames.Image.Jpeg, Stream = stream });
			}

			var thumb = new EntryThumb(_tag, MediaTypeNames.Image.Jpeg, ImagePurpose.Main);
			_imagePersister.HasImage(thumb, ImageSize.Original).Should().BeTrue("Original image was saved");
			_imagePersister.HasImage(thumb, ImageSize.SmallThumb).Should().BeTrue("Small thumbnail was saved");

			var archivedVersion = GetArchivedVersion(_tag);
			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(TagEditableFields.Picture, "Changed fields");
		}

		[TestMethod]
		public void Update_Name()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			updated.Names[0].Value = "Api Miku";

			_queries.Update(updated, null);

			_tag.DefaultName.Should().Be("Api Miku", "EnglishName");

			var archivedVersion = GetArchivedVersion(_tag);
			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(TagEditableFields.Names, "Changed fields");
		}

		[TestMethod]
		public void Update_Name_DuplicateWithAnotherTag()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			updated.Names[0].Value = "MMD";

			_queries.Invoking(subject => subject.Update(updated, null)).Should().Throw<DuplicateTagNameException>();
		}

		[TestMethod]
		public void Update_Name_DuplicateTranslation()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			updated.Names = updated.Names.Concat(new[] { new LocalizedStringWithIdContract { Value = "Appearance Miku", Language = ContentLanguageSelection.Romaji } }).ToArray();

			_queries.Invoking(subject => subject.Update(updated, null)).Should().Throw<DuplicateTagNameException>();
		}


		[TestMethod]
		public void Update_Name_DuplicateKana()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext)
			{
				Names = new[] {
					new LocalizedStringWithIdContract {Value = "コノザマ", Language = ContentLanguageSelection.Japanese},
					new LocalizedStringWithIdContract {Value = "このざま", Language = ContentLanguageSelection.Japanese},
				}.ToArray()
			};

			_queries.Invoking(subject => subject.Update(updated, null)).Should().Throw<DuplicateTagNameException>();
		}

		[TestMethod]
		public void Update_Parent()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			updated.Parent = new TagBaseContract(_tag2, ContentLanguagePreference.English);

			_queries.Update(updated, null);

			_tag.Parent.Should().Be(_tag2, "Parent");
			_tag2.Children.Contains(_tag).Should().BeTrue("Parent contains child tag");

			var archivedVersion = GetArchivedVersion(_tag);
			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(TagEditableFields.Parent, "Changed fields");
		}

		[TestMethod]
		public void Update_Parent_IgnoreSelf()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			updated.Parent = new TagBaseContract(_tag, ContentLanguagePreference.English);

			_queries.Update(updated, null);

			_tag.Parent.Should().BeNull("Parent");

			var archivedVersion = GetArchivedVersion(_tag);
			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(TagEditableFields.Nothing, "Changed fields");
		}

		[TestMethod]
		public void Update_Parent_Renamed()
		{
			var updated = new TagForEditContract(_tag, false, _permissionContext);
			_tag2.TranslatedName.Default = "Api Miku";
			updated.Parent = new TagBaseContract(_tag2, ContentLanguagePreference.English);

			_queries.Update(updated, null);

			_tag.Parent.Should().Be(_tag2, "Parent");
			_tag2.Children.Contains(_tag).Should().BeTrue("Parent contains child tag");
		}

		[TestMethod]
		public void UpdateMappings_Add()
		{
			_queries.UpdateMappings(new[] {
				new TagMappingContract {
					SourceTag = "apimiku",
					Tag = new TagBaseContract(_tag, ContentLanguagePreference.Default)
				}
			});

			var mappings = _repository.List<TagMapping>();
			mappings.Count.Should().Be(1, "Mapping was saved");
			mappings[0].Tag.Should().Be(_tag, "Tag");
			mappings[0].SourceTag.Should().Be("apimiku", "SourceTag");
		}

		[TestMethod]
		public void UpdateMappings_Remove()
		{
			_repository.Save(_tag.CreateMapping("apimiku"));

			_repository.List<TagMapping>().Count.Should().Be(1, "Precondition: mapping exists in database");
			_tag.Mappings.Count.Should().Be(1, "Precondition: mapping exists for tag");

			_queries.UpdateMappings(new TagMappingContract[0]);

			_repository.List<TagMapping>().Count.Should().Be(0, "Mapping was deleted");
			_tag.Mappings.Count.Should().Be(0, "Mapping was removed from tag");
		}
	}
}

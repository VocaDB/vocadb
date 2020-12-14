#nullable disable

using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
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
		private InMemoryImagePersister imagePersister;
		private FakePermissionContext permissionContext;
		private TagQueries queries;
		private FakeTagRepository repository;
		private Tag tag;
		private Tag tag2;
		private User user;

		private ArchivedTagVersion GetArchivedVersion(Tag tag)
		{
			return repository.List<ArchivedTagVersion>().FirstOrDefault(a => a.Tag.Id == tag.Id);
		}

		private Tag CreateAndSaveTag(string englishName)
		{
			var t = CreateEntry.Tag(englishName);

			repository.Save(t);

			foreach (var name in t.Names)
				repository.Save(name);

			return t;
		}

		private TagUsage CreateTagUsage(Tag tag, ReleaseEvent releaseEvent)
		{
			var usage = repository.Save(new EventTagUsage(releaseEvent, tag));
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
			repository = new FakeTagRepository();

			tag = CreateAndSaveTag("Appearance Miku");
			tag2 = CreateAndSaveTag("MMD");

			user = new User("User", "123", "test@test.com", PasswordHashAlgorithms.Default) { GroupId = UserGroupId.Moderator };
			repository.Add(user);

			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(user, ContentLanguagePreference.Default));

			imagePersister = new InMemoryImagePersister();
			queries = new TagQueries(repository, permissionContext, new FakeEntryLinkFactory(), imagePersister, imagePersister, new FakeUserIconFactory(), new EnumTranslations(), new FakeObjectCache());
		}

		[TestMethod]
		public async Task Create()
		{
			var result = await queries.Create("Apimiku");

			Assert.AreEqual("Apimiku", result.Name, "Created tag name");
			var tagFromRepo = repository.Load(result.Id);
			Assert.AreEqual("Apimiku", tagFromRepo.DefaultName, "Tag found from repository");
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateTagNameException))]
		public async Task Create_Duplicate()
		{
			await queries.Create("Appearance Miku");
		}

		[TestMethod]
		public async Task GetDetails_RecentEvents()
		{
			void AssertContainsEvent(TagDetailsContract details, ReleaseEvent releaseEvent)
			{
				Assert.IsTrue(details.Stats.Events.Any(e => e.Id == releaseEvent.Id), "Contains " + releaseEvent);
			}

			var standaloneEvent = CreateEntry.ReleaseEvent("Miku party");
			var otherEvent = CreateEntry.ReleaseEvent("Magical Mirai");
			var eventSeries = CreateEntry.EventSeries("VocaTRAVers");
			var seriesUsage = repository.Save(new EventSeriesTagUsage(eventSeries, tag));
			eventSeries.Tags.Usages.Add(seriesUsage);
			tag.AllEventSeriesTagUsages.Add(seriesUsage);
			var oldSeriesEvent = CreateEntry.ReleaseEvent("VocaTRAVers 1", date: DateTime.Now.AddDays(-30));
			oldSeriesEvent.SetSeries(eventSeries);
			var recentSeriesEvent = CreateEntry.ReleaseEvent("VocaTRAVers 2", date: DateTime.Now);
			recentSeriesEvent.SetSeries(eventSeries);
			repository.Save(standaloneEvent, otherEvent, oldSeriesEvent, recentSeriesEvent);
			repository.Save(eventSeries);
			repository.Save(CreateTagUsage(tag, standaloneEvent), CreateTagUsage(tag, oldSeriesEvent), CreateTagUsage(tag, recentSeriesEvent));

			var result = await queries.GetDetailsAsync(tag.Id);

			Assert.AreEqual(2, result.Stats.EventCount, "EventCount");
			Assert.AreEqual(2, result.Stats.Events.Length, "Events.Length");
			Assert.AreEqual(1, result.Stats.EventSeriesCount, "EventSeriesCount");
			Assert.AreEqual(1, result.Stats.EventSeries.Length, "EventSeries.Length");
			AssertContainsEvent(result, standaloneEvent);
			AssertContainsEvent(result, recentSeriesEvent);
			Assert.IsTrue(result.Stats.EventSeries.Any(e => e.Id == eventSeries.Id), "Contains " + eventSeries);
		}

		[TestMethod]
		public void MoveToTrash()
		{
			var oldCount = repository.Count<Tag>();

			queries.MoveToTrash(tag.Id, string.Empty);

			Assert.AreEqual(oldCount - 1, repository.Count<Tag>(), "One tag was removed");
			Assert.IsFalse(repository.Contains(tag), "Tag was removed from repository");

			Assert.AreEqual(1, repository.Count<TrashedEntry>(), "Trashed entry was created");
			var trashed = repository.List<TrashedEntry>().First();
			Assert.AreEqual(EntryType.Tag, trashed.EntryType, "Trashed entry type");
			Assert.AreEqual(tag.Id, trashed.EntryId, "Trashed entry ID");
			Assert.AreEqual(tag.DefaultName, trashed.Name, "Trashed entry name");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void MoveToTrash_NoDeletePermission()
		{
			user.GroupId = UserGroupId.Regular;
			permissionContext.RefreshLoggedUser(repository);

			queries.MoveToTrash(tag.Id, string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void MoveToTrash_NoEditPermission()
		{
			user.GroupId = UserGroupId.Trusted;
			permissionContext.RefreshLoggedUser(repository);
			tag.Status = EntryStatus.Locked;

			queries.MoveToTrash(tag.Id, string.Empty);
		}

		[TestMethod]
		public void MoveToTrash_DeleteRelatedEntries()
		{
			repository.Save(new TagReport(tag, TagReportType.InvalidInfo, user, "test", "test", null));
			repository.Save(new TagReport(tag2, TagReportType.InvalidInfo, user, "test", "test", null));
			var song = repository.Save(CreateEntry.Song());
			var tagUsage = repository.Save(song.AddTag(tag).Result);
			tag.AllSongTagUsages.Add(tagUsage);

			queries.MoveToTrash(tag.Id, "test");

			Assert.AreEqual(1, repository.Count<TagReport>(), "Tag report was deleted");
			Assert.AreEqual(tag2, repository.List<TagReport>().First().Entry, "Report for the other tag still exists");
			Assert.IsFalse(song.Tags.HasTag(tag), "Tag was removed from song");
		}

		[TestMethod]
		public void GetTagsByCategories()
		{
			tag.CategoryName = "Animation";

			var result = queries.GetTagsByCategories();

			Assert.AreEqual(2, result.Length, "Number of categories");
			var firstCategory = result[0];
			Assert.AreEqual("Animation", firstCategory.Name, "First category name");
			Assert.AreEqual(1, firstCategory.Tags.Length, "Number of tags in the Animation category");
			Assert.AreEqual("Appearance Miku", firstCategory.Tags[0].Name, "First tag in the Animation category");
		}

		[TestMethod]
		public void Merge_ToEmpty()
		{
			var target = repository.Save(new Tag());

			queries.Merge(tag.Id, target.Id);

			Assert.AreEqual("Appearance Miku", target.Names.AllValues.FirstOrDefault(), "Name was copied");

			var mergeRecord = repository.List<TagMergeRecord>().FirstOrDefault(m => m.Source == tag.Id);
			Assert.IsNotNull(mergeRecord, "Merge record was created");
			Assert.AreEqual(target, mergeRecord.Target, "Merge record target");
		}

		[TestMethod]
		public void Merge_MoveUsages()
		{
			// Arrange
			Action<Song, Tag> AddTag = (s, tag) =>
			{
				var u = s.AddTag(tag);
				tag.AllSongTagUsages.Add(u.Result);
				u.Result.CreateVote(user);
			};

			var song = CreateEntry.Song();
			var song2 = CreateEntry.Song();
			var song3 = CreateEntry.Song();
			repository.Save(song, song2, song3);

			AddTag(song, tag);
			AddTag(song2, tag);

			var target = repository.Save(new Tag());
			AddTag(song2, target);
			AddTag(song3, target);

			// Act
			queries.Merge(tag.Id, target.Id);

			// Assert
			Assert.AreEqual(3, target.UsageCount, "Tag's UsageCount");
			Assert.AreEqual(3, target.AllSongTagUsages.Count, "Number of song tag usages");
			Assert.AreEqual(1, song.Tags.Usages.Count, "Number of usages for the first song");

			var usage = target.AllSongTagUsages.FirstOrDefault(s => s.Entry == song);
			Assert.IsNotNull(usage, "Found usage");
			Assert.AreEqual(1, usage.Votes.Count, "Number of votes");
			Assert.IsTrue(song.Tags.Usages.Contains(usage), "Same usage was added to song");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Merge_NoPermissions()
		{
			user.GroupId = UserGroupId.Regular;
			permissionContext.RefreshLoggedUser(repository);

			var target = new Tag();
			repository.Save(target);

			queries.Merge(tag.Id, target.Id);
		}

		[TestMethod]
		public void Merge_TransitiveMergeRecord()
		{
			var target = CreateAndSaveTag("target");
			var newTarget = CreateAndSaveTag("newTarget");

			queries.Merge(tag.Id, target.Id);
			queries.Merge(target.Id, newTarget.Id);

			Assert.AreEqual(2, repository.Count<TagMergeRecord>(), "Two merge records created");
			var mergeRecord = repository.List<TagMergeRecord>().FirstOrDefault(m => m.Source == tag.Id);
			Assert.IsNotNull(mergeRecord, "Found merge record");
			Assert.AreEqual(newTarget.Id, mergeRecord.Target.Id, "Target was updated");
		}

		// Merge target is in related tags. Tag cannot be related to itself, so this it's skipped.
		[TestMethod]
		public void Merge_TargetInRelatedTags()
		{
			var target = repository.Save(new Tag());
			tag.AddRelatedTag(target);
			Assert.AreEqual(1, target.RelatedTags.Count, "Number of related tags");

			queries.Merge(tag.Id, target.Id);

			Assert.AreEqual(0, target.RelatedTags.Count, "Related tag (self) was not added");
		}

		[TestMethod]
		public void Merge_Parent()
		{
			var parent = repository.Save(new Tag("parent"));
			var target = repository.Save(new Tag("target"));
			tag.SetParent(parent);

			queries.Merge(tag.Id, target.Id);

			Assert.AreEqual(parent, target.Parent, "Parent was set");
		}

		[TestMethod]
		public void Merge_Parent_IgnoreSelf()
		{
			var target = repository.Save(new Tag("target"));
			tag.SetParent(target);

			queries.Merge(tag.Id, target.Id);

			Assert.IsNull(target.Parent, "Parent was not set");
		}

		[TestMethod]
		public void Merge_FollowedTags()
		{
			var user2 = repository.Save(CreateEntry.User());
			var target = repository.Save(new Tag("target"));
			user.AddTag(tag);
			user2.AddTag(target);

			queries.Merge(tag.Id, target.Id);

			Assert.AreEqual(2, target.TagsForUsers.Count, "Followed tags were migrated");
			Assert.IsTrue(target.TagsForUsers.Any(t => t.User == user), "Follow was migrated");
		}

		[TestMethod]
		public void Merge_TagMappings()
		{
			repository.Save(tag.CreateMapping("ApiMiku"));
			var target = repository.Save(new Tag("target"));
			queries.Merge(tag.Id, target.Id);

			Assert.IsTrue(target.Mappings.Any(m => m.SourceTag == "ApiMiku"), "Mapping was moved to target tag");
			Assert.IsTrue(repository.List<TagMapping>().Any(m => m.SourceTag == "ApiMiku" && m.Tag.Equals(target)), "Mapped was saved to DB");
		}

		[TestMethod]
		public void Update_Description()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			updated.Description = new EnglishTranslatedStringContract { Original = "mikumikudance.wikia.com/wiki/Miku_Hatsune_Appearance_(Mamama)", English = string.Empty };

			queries.Update(updated, null);

			Assert.AreEqual(updated.Description.Original, tag.Description.Original, "Description was updated");

			var archivedVersion = repository.List<ArchivedTagVersion>().FirstOrDefault(a => a.Tag.Id == tag.Id);
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(TagEditableFields.Description, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public void Update_Image()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			using (var stream = TestImage())
			{
				queries.Update(updated, new UploadedFileContract { Mime = MediaTypeNames.Image.Jpeg, Stream = stream });
			}

			var thumb = new EntryThumb(tag, MediaTypeNames.Image.Jpeg, ImagePurpose.Main);
			Assert.IsTrue(imagePersister.HasImage(thumb, ImageSize.Original), "Original image was saved");
			Assert.IsTrue(imagePersister.HasImage(thumb, ImageSize.SmallThumb), "Small thumbnail was saved");

			var archivedVersion = GetArchivedVersion(tag);
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(TagEditableFields.Picture, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public void Update_Name()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			updated.Names[0].Value = "Api Miku";

			queries.Update(updated, null);

			Assert.AreEqual("Api Miku", tag.DefaultName, "EnglishName");

			var archivedVersion = GetArchivedVersion(tag);
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(TagEditableFields.Names, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateTagNameException))]
		public void Update_Name_DuplicateWithAnotherTag()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			updated.Names[0].Value = "MMD";

			queries.Update(updated, null);
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateTagNameException))]
		public void Update_Name_DuplicateTranslation()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			updated.Names = updated.Names.Concat(new[] { new LocalizedStringWithIdContract { Value = "Appearance Miku", Language = ContentLanguageSelection.Romaji } }).ToArray();

			queries.Update(updated, null);
		}


		[TestMethod]
		[ExpectedException(typeof(DuplicateTagNameException))]
		public void Update_Name_DuplicateKana()
		{
			var updated = new TagForEditContract(tag, false, permissionContext)
			{
				Names = new[] {
					new LocalizedStringWithIdContract {Value = "コノザマ", Language = ContentLanguageSelection.Japanese},
					new LocalizedStringWithIdContract {Value = "このざま", Language = ContentLanguageSelection.Japanese},
				}.ToArray()
			};

			queries.Update(updated, null);
		}

		[TestMethod]
		public void Update_Parent()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			updated.Parent = new TagBaseContract(tag2, ContentLanguagePreference.English);

			queries.Update(updated, null);

			Assert.AreEqual(tag2, tag.Parent, "Parent");
			Assert.IsTrue(tag2.Children.Contains(tag), "Parent contains child tag");

			var archivedVersion = GetArchivedVersion(tag);
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(TagEditableFields.Parent, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public void Update_Parent_IgnoreSelf()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			updated.Parent = new TagBaseContract(tag, ContentLanguagePreference.English);

			queries.Update(updated, null);

			Assert.IsNull(tag.Parent, "Parent");

			var archivedVersion = GetArchivedVersion(tag);
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(TagEditableFields.Nothing, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public void Update_Parent_Renamed()
		{
			var updated = new TagForEditContract(tag, false, permissionContext);
			tag2.TranslatedName.Default = "Api Miku";
			updated.Parent = new TagBaseContract(tag2, ContentLanguagePreference.English);

			queries.Update(updated, null);

			Assert.AreEqual(tag2, tag.Parent, "Parent");
			Assert.IsTrue(tag2.Children.Contains(tag), "Parent contains child tag");
		}

		[TestMethod]
		public void UpdateMappings_Add()
		{
			queries.UpdateMappings(new[] {
				new TagMappingContract {
					SourceTag = "apimiku",
					Tag = new TagBaseContract(tag, ContentLanguagePreference.Default)
				}
			});

			var mappings = repository.List<TagMapping>();
			Assert.AreEqual(1, mappings.Count, "Mapping was saved");
			Assert.AreEqual(tag, mappings[0].Tag, "Tag");
			Assert.AreEqual("apimiku", mappings[0].SourceTag, "SourceTag");
		}

		[TestMethod]
		public void UpdateMappings_Remove()
		{
			repository.Save(tag.CreateMapping("apimiku"));

			Assert.AreEqual(1, repository.List<TagMapping>().Count, "Precondition: mapping exists in database");
			Assert.AreEqual(1, tag.Mappings.Count, "Precondition: mapping exists for tag");

			queries.UpdateMappings(new TagMappingContract[0]);

			Assert.AreEqual(0, repository.List<TagMapping>().Count, "Mapping was deleted");
			Assert.AreEqual(0, tag.Mappings.Count, "Mapping was removed from tag");
		}
	}
}

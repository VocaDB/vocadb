using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Tags;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Tags {

	/// <summary>
	/// Tests for <see cref="Tag"/>.
	/// </summary>
	[TestClass]
	public class TagTests {

		private readonly Tag tag;
		private readonly Tag tag2;

		public TagTests() {
			tag = CreateEntry.Tag("rock");
			tag2 = CreateEntry.Tag("metal");
		}

		[TestMethod]
		public void AddRelatedTag() {

			tag.AddRelatedTag(tag2);

			Assert.IsTrue(tag.RelatedTags.Any(t => t.LinkedTag.Equals(tag2)), "Related tag was added from the owner side");
			Assert.IsTrue(tag2.RelatedTags.Any(t => t.LinkedTag.Equals(tag)), "Owner tag was added from the linked side");

		}

		[TestMethod]
		public void Delete() {

			var relatedTag = tag2;
			tag.AddRelatedTag(relatedTag);

			tag.Delete();

			Assert.IsFalse(relatedTag.RelatedTags.Any(t => t.LinkedTag.Equals(tag)), "Tag was removed from the linked side");

		}

		[TestMethod]
		public void SetParent() {

			tag.SetParent(tag2);

			Assert.AreEqual(tag2, tag.Parent, "Parent");
			Assert.IsTrue(tag2.Children.Contains(tag), "Child tag was added for parent tag");

		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void SetParent_Self() {
			tag.SetParent(tag);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void SetParent_Ancestor() {
			tag.SetParent(tag2);
			tag2.SetParent(tag); // Circular parenthood not allowed
		}

		[TestMethod]
		public void SyncRelatedTags() {

			var tag3 = CreateEntry.Tag("power metal");
			var repository = new FakeTagRepository(tag, tag2, tag3);

			tag.AddRelatedTag(tag2);

			var result = tag.SyncRelatedTags(new[] { tag2, tag3 }, repository.Load);

			Assert.AreEqual(1, result.Added.Length, "Number of added items");
			Assert.AreEqual(1, result.Unchanged.Length, "Number of unchanged items");
			Assert.AreEqual(0, result.Removed.Length, "Number of removed items");

			Assert.AreEqual(2, tag.RelatedTags.Count, "Number of related tags for tag1");
			Assert.AreEqual(1, tag2.RelatedTags.Count, "Number of related tags for tag2");

		}

	}

}
